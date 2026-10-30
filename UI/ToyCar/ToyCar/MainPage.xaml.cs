using Microsoft.UI.Xaml.Input;
using Windows.Devices.Sensors;

namespace ToyCar;

/// <summary>
/// Showcases <see cref="TwoPaneView"/> adaptive layout together with the
/// <see cref="HingeAngleSensor"/> on foldable devices.
/// </summary>
public sealed partial class MainPage : Page
{
	// The hinge sensor is only implemented on Android; every other target returns null
	// from GetDefaultAsync(), so the sample stays fully interactive via drag instead.
	//
	// Two classes of hardware report very different angle ranges:
	//
	//   Surface Duo / Neo (what this sample was written for) - two physically separate
	//   screens on a 0-360 hinge. 180 means the screens lie flat side by side; 360 means
	//   they are folded all the way back so the panels face away from each other.
	//
	//   Single-screen foldables (Galaxy Z Fold, Pixel Fold, iPhone Duo) - one continuous
	//   inner display with a crease, on a 0-180 hinge. 180 means fully unfolded and flat;
	//   0 means shut. There is no 360 on these devices.
	//
	// So FlatAngleDegrees is the one pose both classes share, and is what puts the car on
	// the fold. FullyOpenAngleDegrees only ever matches on a Duo - it is kept deliberately
	// so the original dual-screen behaviour still works on that hardware.
	private const double FlatAngleDegrees = 180d;
	private const double FullyOpenAngleDegrees = 360d;

	// Real hinges report a continuous angle, so the poses are matched on a band rather than
	// an exact value.
	private const double AngleToleranceDegrees = 10d;

	// Nothing in the API reports the hinge's range - only its report thresholds - so the
	// device class is inferred from what the sensor actually sends: an angle beyond this can
	// only come from a 0-360 dual-screen hinge.
	private const double DualScreenDetectionAngle = 200d;

	// Below this the foldable is considered shut and the car sits at its resting position.
	private const double ClosedAngleDegrees = 20d;

	private bool _isDualScreenHinge;

	// Left edge of the right pane, and whether the callouts are on screen: the car's travel
	// limit depends on both.
	private double _paneLeftEdge;
	private bool _featuresPanelShown;

	// Both the drag and the hinge write the car's position, so they need a precedence rule.
	// A deliberate drag should not be undone by hinge noise - a device held in the hand never
	// reports a perfectly steady angle - but a real fold should take the car back. So a drag
	// wins until the hinge moves further than a hand tremor could explain.
	private const double HingeOverridesDragDegrees = 8d;
	private bool _positionSetByDrag;
	private double _hingeAngleAtDrag;

	private readonly TranslateTransform _dragUpperTranslation = new();
	private readonly TranslateTransform _dragLowerTranslation = new();

	// Tracks whether the car is parked at the end of its travel, so the animations are
	// started and stopped on the transition rather than on every pointer delta.
	private bool _isAtDragLimit;
	private bool _isSceneAnimationStarted;
	private double _carBandHeight = 400d;
	private double _previousAngle;
	// Resting X of both cars: they share one absolute position so the blue car and the sketch
	// car form a single car straddling the fold, aligned with the left edge of the description
	// text. Cached rather than read live so the margins and MaxDragX cannot disagree mid-drag.
	private double _contentLeftOffset = 129d + 10d + 10d;

	private double MeasureContentLeftOffset() =>
		NumberPlate.Visibility == Visibility.Visible
			? 129d + 10d + 10d   // number plate + its right margin + the title's left margin
			: 24d + 10d;         // plate collapsed: the content inset + the title's left margin
	// Width of the car artwork, and the gap kept between it and the right edge.
	private const double CarWidth = 220d;
	private const double CarRightPadding = 40d;

	// The features panel sits left of the car so its callouts frame it, but its labels are
	// on that left side - letting it slide past the pane edge cuts the text off.
	// Design size of the features artwork, and the pane width below which the number plate is
	// pure decoration the layout cannot afford.
	private const double FeaturesPanelWidth = 398d;
	private const double FeaturesPanelHeight = 242d;
	private const double MinPaneWidthForPlate = 420d;

	// The copy below the plate - heading, bullets, price and the VIEW DETAILS button - needs
	// about this much room once the plate has taken its 125. Below it the plate is dropped:
	// landscape on a foldable leaves the column 352dp, and the button was being squeezed to a
	// 4dp sliver against the car band.
	private const double MinContentHeightForPlate = 420d;
	private const double NavBarHeight = 72d;

	// TwoPaneView's own default breakpoint for switching to two panes.
	private const double TwoPaneViewWideThreshold = 641d;

	private const double FeaturesPanelOffset = 180d;
	private const double FeaturesPanelLeftPadding = 8d;
	// A pointer release that never reaches the app - let go outside the window, or one the
	// browser swallows - leaves the manipulation open, and the car then follows a mouse with
	// no button held. The drag is gated on this rather than trusting the manipulation to end.
	private bool _isDragging;

	private HingeAngleSensor? _hinge;

	public MainPage()
	{
		this.InitializeComponent();

		MainRoot.SizeChanged += MainRoot_SizeChanged;
		TouchRectangle.ManipulationDelta += TouchRectangle_ManipulationDelta;
		TouchRectangle.PointerPressed += TouchRectangle_PointerPressed;
		TouchRectangle.PointerReleased += TouchRectangle_PointerReleased;
		TouchRectangle.PointerCanceled += TouchRectangle_PointerCanceled;
		TouchRectangle.PointerCaptureLost += TouchRectangle_PointerCaptureLost;
		TouchRectangle.PointerMoved += TouchRectangle_PointerMoved;

		// Apply the translations to the elements.
		TouchRectangle.RenderTransform = _dragUpperTranslation;
		DragLeftCar.RenderTransform = _dragLowerTranslation;
		DragRightCar.RenderTransform = _dragLowerTranslation;
		FeaturesPanel.RenderTransform = _dragLowerTranslation;

		// The spin-up ends at exactly the loop's angular velocity and on a whole
		// revolution, so starting the loop here is seamless in both speed and angle.
		WheelSpinUpStoryboard.Completed += OnWheelSpinUpCompleted;

		Loaded += OnLoaded;
		Unloaded += OnUnloaded;
	}

	private async void OnLoaded(object sender, RoutedEventArgs e)
	{
		try
		{
			_hinge = await HingeAngleSensor.GetDefaultAsync();
		}
		catch (Exception ex)
		{
			SensorStatus.Content = $"Hinge sensor unavailable: {ex.Message}";
			return;
		}

		if (_hinge is null)
		{
			SensorStatus.Content = "No hinge sensor on this platform - drag the car instead";
			AngleValue.Content = "Angle value: n/a";
			return;
		}

		_hinge.ReadingChanged += HingeAngleSensor_ReadingChanged;
		SensorStatus.Content = "Hinge sensor detected - fold the device";
	}

	private void OnUnloaded(object sender, RoutedEventArgs e)
	{
		MainRoot.SizeChanged -= MainRoot_SizeChanged;
		TouchRectangle.ManipulationDelta -= TouchRectangle_ManipulationDelta;
		TouchRectangle.PointerPressed -= TouchRectangle_PointerPressed;
		TouchRectangle.PointerReleased -= TouchRectangle_PointerReleased;
		TouchRectangle.PointerCanceled -= TouchRectangle_PointerCanceled;
		TouchRectangle.PointerCaptureLost -= TouchRectangle_PointerCaptureLost;
		TouchRectangle.PointerMoved -= TouchRectangle_PointerMoved;

		if (_hinge is not null)
		{
			_hinge.ReadingChanged -= HingeAngleSensor_ReadingChanged;
			_hinge = null;
		}

		WheelSpinUpStoryboard.Completed -= OnWheelSpinUpCompleted;

		Loaded -= OnLoaded;
		Unloaded -= OnUnloaded;
	}

	// Where the car sits astride the seam: the pose the whole sample is built around.
	private double SeamPositionX => (MainRoot.ActualWidth / 2) - _contentLeftOffset - 100;

	// Furthest the car may travel. With the callouts hidden it is simply the car plus a gap
	// from the right edge. With them shown the whole composition has to land inside the pane:
	// the panel is drawn 180dp to the left of the car, so the car has to travel far enough to
	// pull the panel's left edge in. Nudging the panel on its own instead - which is what an
	// earlier version did - moves it off the offset its leader lines were drawn for, and they
	// stop meeting the car.
	private double MaxDragX
	{
		get
		{
			var carOnly = Math.Max(0, MainRoot.ActualWidth - _contentLeftOffset - CarWidth - CarRightPadding);

			if (!_featuresPanelShown)
			{
				return carOnly;
			}

			var toFitPanel = _paneLeftEdge + FeaturesPanelLeftPadding + FeaturesPanelOffset - _contentLeftOffset;
			var carStaysOnScreen = MainRoot.ActualWidth - _contentLeftOffset - CarWidth - FeaturesPanelLeftPadding;

			return Math.Max(0, Math.Min(Math.Max(carOnly, toFitPanel), carStaysOnScreen));
		}
	}

	private void OnWheelSpinUpCompleted(object? sender, object e) => WheelLoopStoryboard.Begin();

	// Chosen here rather than with AdaptiveTriggers: overlapping minimum-width and
	// minimum-height triggers did not resolve predictably (a w>=820 state won over an
	// h>=800 one, leaving a 250px band where 400 was intended). Applying the state up front
	// also settles the number plate's visibility before the offsets below are measured.
	private void ApplyLayoutState()
	{
		var w = MainRoot.ActualWidth;
		var h = MainRoot.ActualHeight;

		// What matters is the width of a single pane, not of the window: in wide mode
		// TwoPaneView halves it, so a 673dp foldable only gives each side 336dp. Sizing the
		// decoration off the window was why the number plate ate 40% of a pane.
		var pane = w > TwoPaneViewWideThreshold ? w / 2 : w;

		var layout =
			h >= 800 ? "RegularLayout" :
			w >= 820 && h >= 680 ? "MediumWidePanel" :
			h >= 680 ? "MediumLayout" :
			w >= 820 && h >= 600 ? "CompactWidePanel" :
			h >= 520 ? "CompactLayout" :
			"TinyLayout";

		VisualStateManager.GoToState(this, layout, false);

		_carBandHeight = layout switch
		{
			"RegularLayout" => 400d,
			"MediumWidePanel" or "CompactWidePanel" => 250d,
			"TinyLayout" => 140d,
			_ => 170d,
		};
		// Shown at its design size only. Scaling it down looked tempting - it is pure artwork -
		// but its leader lines are drawn for a 220dp car at a fixed offset, and the car is a
		// separate element that does not scale with it. Shrinking the panel alone left the
		// callouts pointing at empty space, so it is all-or-nothing: either the pane can hold
		// the artwork at 1:1, or the panel stays out.
		// The composition is 180dp of offset plus the wider of the panel's overhang and the car,
		// so it needs that much pane - not the panel's width alone.
		var compositionWidth = FeaturesPanelOffset + Math.Max(FeaturesPanelWidth - FeaturesPanelOffset, CarWidth);
		var panelFits = pane >= compositionWidth + 2 * FeaturesPanelLeftPadding && h >= 600;

		_paneLeftEdge = w > TwoPaneViewWideThreshold ? w / 2 : 0;
		_featuresPanelShown = panelFits;

		// One or the other, never both: the full composition where it fits, the bare labels
		// where it does not.
		CompactFeatures.Visibility = panelFits ? Visibility.Collapsed : Visibility.Visible;

		FeaturesPanel.Width = FeaturesPanelWidth;
		FeaturesPanel.Height = FeaturesPanelHeight;

		VisualStateManager.GoToState(this, panelFits ? "FeaturesPanelShown" : "FeaturesPanelHidden", false);

		// A plate that would swallow a third of a narrow pane is dropped whatever the height.
		// Both of these are written here rather than by the states' setters: the decision turns
		// on the pane width, which a state keyed on the window cannot see, and a property the
		// code has assigned once stops responding to those setters - so a state that later set
		// the plate back to Visible left it there, overflowing a 336dp pane. The plate's column
		// is also the copy's left inset.
		// Width alone is not enough: the plate is decorative and yields to the copy, so it also
		// has to leave the column enough height for the button to survive.
		var contentHeight = h - NavBarHeight - _carBandHeight;
		var plateFits = pane >= MinPaneWidthForPlate && contentHeight >= MinContentHeightForPlate;
		NumberPlate.Visibility = plateFits ? Visibility.Visible : Visibility.Collapsed;
		Pane1Content.Margin = new Thickness(
			plateFits ? 0 : 24,
			0,
			0,
			layout == "RegularLayout" ? 0 : _carBandHeight);
	}

	private void MainRoot_SizeChanged(object sender, SizeChangedEventArgs e)
	{
		ApplyLayoutState();

		// Making sure we are resetting the X axis and the Y axis if the MainRoot is resized.
		_dragUpperTranslation.X = 0;
		_dragLowerTranslation.X = 0;
		_isAtDragLimit = false;
		_dragLowerTranslation.Y = _dragUpperTranslation.Y;

		CarStoryboard.Stop();
		StopDriveAnimations();

		// The callouts belong to the car at the end of its travel, and the car has just been
		// sent back to the start, so they go with it. Leaving them holding at full opacity was
		// what showed the labels beside a car still sitting on the left.
		HideFeatureCallouts();

		// Adjusting the margins on the right side so it will be at the same level as the left side.
		// The balloon must clear the top of the window exactly as its cycle ends, otherwise the
		// time it spends out of sight depends on the window height - short windows lost it early
		// and left a long empty sky. Its rest position is the scene row's base, so the distance
		// to the top edge is the height above the car band, plus its own 99px and a little margin.
		BalloonRise.To = -(MainRoot.ActualHeight - _carBandHeight + 40);
		RestartSceneForNewSize();

		_contentLeftOffset = MeasureContentLeftOffset();
		var leftOffset = _contentLeftOffset;
		var rightOffset = -((MainRoot.ActualWidth / 2) - leftOffset);

		DragLeftCar.Margin = new Thickness(leftOffset, 0, 0, 0);
		TouchRectangle.Margin = new Thickness(leftOffset, 0, 0, 0);
		DragRightCar.Margin = new Thickness(rightOffset, 0, 0, 0);

		// Exactly the offset the artwork was drawn for - no nudging, so the leader lines always
		// meet the car. Fitting it on screen is MaxDragX's job instead.
		FeaturesPanel.Margin = new Thickness(rightOffset - FeaturesPanelOffset, 0, 0, 0);
	}

	private void TouchRectangle_PointerPressed(object sender, PointerRoutedEventArgs e) => _isDragging = true;

	private void TouchRectangle_PointerReleased(object sender, PointerRoutedEventArgs e) => EndDrag();

	private void TouchRectangle_PointerCanceled(object sender, PointerRoutedEventArgs e) => EndDrag();

	private void TouchRectangle_PointerCaptureLost(object sender, PointerRoutedEventArgs e) => EndDrag();

	// The self-healing half, for the case where no release arrives at all: a mouse moving with
	// its button up cannot be dragging, whatever the manipulation still believes.
	private void TouchRectangle_PointerMoved(object sender, PointerRoutedEventArgs e)
	{
		if (_isDragging
			&& e.Pointer.PointerDeviceType == Microsoft.UI.Input.PointerDeviceType.Mouse
			&& !e.GetCurrentPoint(TouchRectangle).Properties.IsLeftButtonPressed)
		{
			EndDrag();
		}
	}

	private void EndDrag()
	{
		_isDragging = false;
		TouchRectangle.ReleasePointerCaptures();
	}

	// ManipulationDelta data is loaded into the translation transforms and applied to the
	// upper touch rectangle and the lower car images.
	private void TouchRectangle_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
	{
		if (e.IsInertial)
		{
			return;
		}

		// Nothing is holding the car: a stale manipulation must not keep moving it.
		if (!_isDragging)
		{
			return;
		}

		// Clamp rather than reverting to the previous position: the original code assigned
		// the stale lower translation back onto the upper one at the limit, so the car
		// rubber-banded backwards on every delta instead of coming to rest.
		var x = Math.Clamp(_dragUpperTranslation.X + e.Delta.Translation.X, 0, MaxDragX);

		_dragUpperTranslation.X = x;
		_dragLowerTranslation.X = x;
		_dragLowerTranslation.Y = _dragUpperTranslation.Y;

		// Remember that the position came from the user's hand, and at what hinge angle, so a
		// small wobble of the device does not immediately snatch the car back.
		_positionSetByDrag = true;
		_hingeAngleAtDrag = _previousAngle;

		// Only act on the crossing. Driving these every delta restarted the storyboards
		// from frame zero many times a second, which is what made the last pixels stutter.
		var atLimit = x >= MaxDragX;
		if (atLimit == _isAtDragLimit)
		{
			return;
		}

		_isAtDragLimit = atLimit;

		if (atLimit)
		{
			StartOrRestartAnimations();
			ShowFeatureCallouts();
		}
		else
		{
			CarStoryboard.Stop();
			StopDriveAnimations();
			HideFeatureCallouts();
		}
	}

	private void HingeAngleSensor_ReadingChanged(HingeAngleSensor sender, HingeAngleSensorReadingChangedEventArgs args)
	{
		var angleValue = args.Reading.AngleInDegrees;

		DispatcherQueue.TryEnqueue(() =>
		{
			AngleValue.Content = $"Angle value: {angleValue:F1}";

			if (angleValue > DualScreenDetectionAngle)
			{
				_isDualScreenHinge = true;
			}

			// Making sure we are not moving the cars again while the angle is unchanged.
			if (Math.Abs(angleValue - _previousAngle) < 0.5)
			{
				return;
			}

			_previousAngle = angleValue;

			if (_isDualScreenHinge)
			{
				ApplyDualScreenPose(angleValue);
			}
			else
			{
				ApplyFoldablePose(angleValue);
			}
		});
	}

	// Surface Duo / Neo: two separate screens on a 0-360 hinge, with two meaningful poses.
	private void ApplyDualScreenPose(double angleValue)
	{
		// A drag outranks hinge jitter; a real fold takes precedence again.
		if (_positionSetByDrag)
		{
			if (Math.Abs(angleValue - _hingeAngleAtDrag) < HingeOverridesDragDegrees)
			{
				return;
			}

			_positionSetByDrag = false;
		}

		if (Math.Abs(angleValue - FlatAngleDegrees) <= AngleToleranceDegrees)
		{
			// Opened flat, the two screens side by side: park the car across the seam - the
			// shot the original dual-screen demo was built around.
			CarStoryboard.Stop();
			StopDriveAnimations();

			_isAtDragLimit = false;
			_dragLowerTranslation.X = SeamPositionX;
			_dragLowerTranslation.Y = _dragUpperTranslation.Y;

			_dragUpperTranslation.X = _dragLowerTranslation.X;

			// The car is mid-screen now, not at the end of its travel, so the callouts go.
			HideFeatureCallouts();
		}
		else if (angleValue >= FullyOpenAngleDegrees - AngleToleranceDegrees)
		{
			// Folded all the way back so the panels face away from each other, which only a
			// 0-360 hinge can reach: drive the car.
			_dragLowerTranslation.X = MaxDragX;
			_dragLowerTranslation.Y = _dragUpperTranslation.Y;

			_dragUpperTranslation.X = _dragLowerTranslation.X;
			_dragUpperTranslation.Y = _dragLowerTranslation.Y;

			_isAtDragLimit = true;
			StartOrRestartAnimations();

			// The one pose that puts the car where the callouts were drawn for, so this is the
			// only place besides the drag that shows them.
			ShowFeatureCallouts();
		}
	}

	// Single-screen foldables (Galaxy Z Fold, Pixel Fold, iPhone Duo): one continuous inner
	// display on a 0-180 hinge, so there is no back-to-back pose to trigger the drive. Instead
	// the fold itself becomes the gesture - opening the device drives the car across the
	// crease, and it reaches the end of its travel, with the animation, exactly when the
	// device lies flat.
	private void ApplyFoldablePose(double angleValue)
	{
		// A drag outranks hinge jitter; a real fold takes precedence again.
		if (_positionSetByDrag)
		{
			if (Math.Abs(angleValue - _hingeAngleAtDrag) < HingeOverridesDragDegrees)
			{
				return;
			}

			_positionSetByDrag = false;
		}

		// Opening the device walks the car towards the seam and it arrives exactly as the
		// device lies flat - the same pose a Duo shows at 180 degrees, just reached
		// continuously instead of at a threshold, because a 0-180 hinge has no second pose to
		// switch on. The animations run there, so the payoff is the car spanning the crease
		// with its wheels turning. Driving it further, to the far right and the feature
		// callouts, stays the drag's job on every platform - exactly the division of labour
		// the Duo had between its hinge and the swipe.
		var isFullyOpen = angleValue >= FlatAngleDegrees - AngleToleranceDegrees;
		var openness = Math.Clamp(
			(angleValue - ClosedAngleDegrees) / (FlatAngleDegrees - ClosedAngleDegrees), 0, 1);

		var x = isFullyOpen ? SeamPositionX : openness * SeamPositionX;

		_dragUpperTranslation.X = x;
		_dragLowerTranslation.X = x;
		_dragLowerTranslation.Y = _dragUpperTranslation.Y;

		if (isFullyOpen == _isAtDragLimit)
		{
			return;
		}

		_isAtDragLimit = isFullyOpen;

		if (isFullyOpen)
		{
			StartOrRestartAnimations();
		}
		else
		{
			CarStoryboard.Stop();
			StopDriveAnimations();
		}

		// The callouts belong to the far-right pose only; the seam is not it.
		HideFeatureCallouts();
	}

	// The callouts are laid out for the car at the very end of its travel, so these two are
	// the only ways they are ever shown or hidden - anything that moves the car away from
	// there has to call the second one.
	private void ShowFeatureCallouts()
	{
		FeaturesHideStoryboard.Stop();
		FeaturesRevealStoryboard.Begin();
	}

	private void HideFeatureCallouts()
	{
		FeaturesRevealStoryboard.Stop();
		FeaturesHideStoryboard.Begin();
	}

	private void StopDriveAnimations()
	{
		WheelSpinUpStoryboard.Stop();
		WheelLoopStoryboard.Stop();

		if (_isSceneAnimationStarted)
		{
			SceneStoryboard.Pause();
		}
	}

	private void RestartSceneForNewSize()
	{
		if (_isSceneAnimationStarted)
		{
			SceneStoryboard.Stop();
			_isSceneAnimationStarted = false;
		}
	}

	private void StartOrRestartAnimations()
	{
		CarStoryboard.Stop();
		CarStoryboard.Begin();
		StopDriveAnimations();
		WheelSpinUpStoryboard.Begin();

		// The scenery moves with the car: resume where it left off rather than restarting,
		// so the clouds do not jump back on every drag.
		if (_isSceneAnimationStarted)
		{
			SceneStoryboard.Resume();
		}
		else
		{
			SceneStoryboard.Begin();
			_isSceneAnimationStarted = true;
		}
	}
}
