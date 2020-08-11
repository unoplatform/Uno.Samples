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
using Windows.UI.Xaml.Navigation;

namespace ToyCar
{
	public sealed partial class MainPage : Page
	{
		private TranslateTransform dragUpperTranslation;
		private TranslateTransform dragLowerTranslation;

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

			// Apply the translation to the Rectangles.
			TouchRectangle.RenderTransform = this.dragUpperTranslation;
			DragLeftCar.RenderTransform = this.dragLowerTranslation;
			DragRightCar.RenderTransform = this.dragLowerTranslation;

			Unloaded += OnUnloaded;
		}

		private void MainRoot_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			DragRightCar.Margin = new Thickness(-(MainRoot.ActualWidth / 2), 0, 0, 0);
			dragUpperTranslation.X = 0;
			dragLowerTranslation.X = 0;
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

				if (dragUpperTranslation.X > 0 && dragUpperTranslation.X < (MainRoot.ActualWidth * 0.7))
				{
					//Making sure the SketchCar is not bouncing and rotating wheels under this threshold.
					CarStoryboard.Stop();
					// Move the lower cars along the x-axis.
					dragLowerTranslation.X = dragUpperTranslation.X;
				}
				else
				{
					// Keep the upper rectangle at the same position as the lower rectangle.
					dragUpperTranslation.X = dragLowerTranslation.X;
					//Starting the SketchCar bouncing and rotating wheels animations. 
					CarStoryboard.Begin();
				}
			}
		}
	}
}
