using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace ToyCar
{
	public sealed partial class MainPage : Page
	{
		private TranslateTransform dragUpperTranslation;
		private TranslateTransform dragLowerTranslation;
		private bool isAnimationStarted = false;

		public MainPage()
		{
			this.InitializeComponent();

			// Listener for the MainRoot SizeChanged event.
			MainRoot.SizeChanged += MainRoot_SizeChanged;

			// Listener for the ManipulationDelta event.
			TouchRectangle.ManipulationDelta += TouchRectangle_ManipulationDelta;

			// New translations transforms populated in the ManipulationDelta handler.
			dragUpperTranslation = new TranslateTransform();
			dragLowerTranslation = new TranslateTransform();

			// Apply the translations to the elements.
			TouchRectangle.RenderTransform = this.dragUpperTranslation;
			DragLeftCar.RenderTransform = this.dragLowerTranslation;
			DragRightCar.RenderTransform = this.dragLowerTranslation;
			FeaturesPanel.RenderTransform = this.dragLowerTranslation;

			Unloaded += OnUnloaded;
		}

		private void MainRoot_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			// Making sure we are resetting the X axis and the Y axis if the MainRoot is resized (Can be reviewed in the long term).
			dragUpperTranslation.X = 0;
			dragLowerTranslation.X = 0;
			dragLowerTranslation.Y = dragUpperTranslation.Y;

			// Making sure the car animation is stopped.
			CarStoryboard.Stop();

			// Making sure the scene animation is paused if it was already started.
			if (isAnimationStarted)
			{
				SceneStoryboard.Pause();
			}

			// Ajusting the margins on the right side so it will be at the same level as the margins on the left side.
			DragLeftCar.Margin = new Thickness(MainRoot.ActualWidth * 0.1, 0, 0, 0);
			TouchRectangle.Margin = new Thickness(MainRoot.ActualWidth * 0.1, 0, 0, 0);
			DragRightCar.Margin = new Thickness(-((MainRoot.ActualWidth / 2) - (MainRoot.ActualWidth * 0.1)), 0, 0, 0);
			FeaturesPanel.Margin = new Thickness(-((MainRoot.ActualWidth / 2) - (MainRoot.ActualWidth * 0.1)) - 180, 0, 0, 0);
		}

		private void OnUnloaded(object sender, RoutedEventArgs e)
		{
			// Unsubscribe from SizeChanged and ManipulationDelta events.
			MainRoot.SizeChanged -= MainRoot_SizeChanged;
			TouchRectangle.ManipulationDelta -= TouchRectangle_ManipulationDelta;
		}

		// Handler for the ManipulationDelta event.
		// ManipulationDelta data is loaded into the
		// translations transforms and applied to the
		// upper touch rectangle and lower car images.
		void TouchRectangle_ManipulationDelta(object sender,
			ManipulationDeltaRoutedEventArgs e)
		{
			// TO-DO: Need to do more tests with inertia on all platforms.
			if (e.IsInertial != true)
			{
				// Move the upper touch rectangle along the x-axis.
				dragUpperTranslation.X += e.Delta.Translation.X;

				if (dragUpperTranslation.X >= 0 && dragUpperTranslation.X < (MainRoot.ActualWidth * 0.65))
				{
					//Making sure the SketchCar is not bouncing and rotating wheels under this threshold.
					CarStoryboard.Stop();

					// Making sure the scene animation is paused at the same time if it was already started.
					if (isAnimationStarted)
					{
						SceneStoryboard.Pause();
					}

					// Move the lower cars along the x-axis and keeping the same upper y-axis.
					dragLowerTranslation.X = dragUpperTranslation.X;
					dragLowerTranslation.Y = dragUpperTranslation.Y;
				}
				else if (dragUpperTranslation.X >= (MainRoot.ActualWidth * 0.65))
				{
					// Keep the upper rectangle at the same position as the lower rectangle.
					dragUpperTranslation.X = dragLowerTranslation.X;
					dragUpperTranslation.Y = dragLowerTranslation.Y;

					//Starting the SketchCar bouncing and rotating wheels animations above the threshold.
					CarStoryboard.Begin();

					// Making sure the scene animation begin for the first time or resume if paused.
					if (isAnimationStarted)
					{
						SceneStoryboard.Resume();
					}
					else 
					{ 
						SceneStoryboard.Begin();
						isAnimationStarted = true;
					}
				}
				else
				{
					// Keep the upper rectangle at the same position as the lower rectangle.
					dragUpperTranslation.X = dragLowerTranslation.X;
					dragUpperTranslation.Y = dragLowerTranslation.Y;

					// Making sure the scene animation is paused at the same time if it was already started.
					if (isAnimationStarted)
					{
						SceneStoryboard.Pause();
					}
				}
			}
		}
	}
}
