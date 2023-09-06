using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Sensors;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;

namespace ToyCar
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private TranslateTransform dragUpperTranslation;
        private TranslateTransform dragLowerTranslation;
        private bool isSceneAnimationStarted = false;
        private bool isCarAnimationStarted = false;
        private double previousAngle = 0.0;
        private double leftSideThresholdRatio = 0.1;
        private double rightSideThresholdRatio = 0.65;
        private HingeAngleSensor hinge;

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
            if (isSceneAnimationStarted)
            {
                SceneStoryboard.Pause();
            }

            // Ajusting the margins on the right side so it will be at the same level as the margins on the left side.
            DragLeftCar.Margin = new Thickness(MainRoot.ActualWidth * leftSideThresholdRatio, 0, 0, 0);
            TouchRectangle.Margin = new Thickness(MainRoot.ActualWidth * leftSideThresholdRatio, 0, 0, 0);
            DragRightCar.Margin = new Thickness(-((MainRoot.ActualWidth / 2) - (MainRoot.ActualWidth * leftSideThresholdRatio)), 0, 0, 0);
            FeaturesPanel.Margin = new Thickness(-((MainRoot.ActualWidth / 2) - (MainRoot.ActualWidth * leftSideThresholdRatio)) - 180, 0, 0, 0);
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            // Unsubscribe from SizeChanged, ManipulationDelta and ReadingChanged events.
            MainRoot.SizeChanged -= MainRoot_SizeChanged;
            TouchRectangle.ManipulationDelta -= TouchRectangle_ManipulationDelta;
            hinge.ReadingChanged -= HingeAngleSensor_ReadingChanged;
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

                if (dragUpperTranslation.X >= 0 && dragUpperTranslation.X < (MainRoot.ActualWidth * rightSideThresholdRatio))
                {
                    //Making sure the SketchCar is not bouncing and rotating wheels under this threshold.
                    CarStoryboard.Stop();

                    // Making sure the scene animation is paused at the same time if it was already started.
                    if (isSceneAnimationStarted)
                    {
                        SceneStoryboard.Pause();
                    }

                    // Move the lower cars along the x-axis and keeping the same upper y-axis.
                    dragLowerTranslation.X = dragUpperTranslation.X;
                    dragLowerTranslation.Y = dragUpperTranslation.Y;
                }
                else if (dragUpperTranslation.X >= (MainRoot.ActualWidth * rightSideThresholdRatio))
                {
                    // Keep the upper rectangle at the same position as the lower rectangle.
                    dragUpperTranslation.X = dragLowerTranslation.X;
                    dragUpperTranslation.Y = dragLowerTranslation.Y;

                    // Making sure the SketchCar bouncing and rotating wheels animations above the threshold
                    // begin for the first time or correctly being stopped and begin again if already started.
                    if (isCarAnimationStarted)
                    {
                        CarStoryboard.Stop();
                        CarStoryboard.Begin();
                    }
                    else
                    {
                        CarStoryboard.Begin();
                        isCarAnimationStarted = true;
                    }

                    // Making sure the scene animation begin for the first time or resume if paused.
                    if (isSceneAnimationStarted)
                    {
                        SceneStoryboard.Resume();
                    }
                    else
                    {
                        SceneStoryboard.Begin();
                        isSceneAnimationStarted = true;
                    }
                }
                else
                {
                    // Keep the upper rectangle at the same position as the lower rectangle.
                    dragUpperTranslation.X = dragLowerTranslation.X;
                    dragUpperTranslation.Y = dragLowerTranslation.Y;

                    // Making sure the scene animation is paused at the same time if it was already started.
                    if (isSceneAnimationStarted)
                    {
                        SceneStoryboard.Pause();
                    }
                }
            }
        }

        private async void HingeAngleSensor_ReadingChanged(HingeAngleSensor sender, HingeAngleSensorReadingChangedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var angleValue = args.Reading.AngleInDegrees;

                AngleValue.Content = "Angle value: " + angleValue.ToString();

                //Making sure we are not moving again the cars if we are keeping the same angle
                if (angleValue != previousAngle)
                {
                    previousAngle = angleValue;

                    // When the dual-screen device is half-opened
                    if (angleValue == 180)
                    {
                        //Making sure the SketchCar is not bouncing and rotating wheels.
                        CarStoryboard.Stop();

                        // Making sure the scene animation is paused at the same time if it was already started.
                        if (isSceneAnimationStarted)
                        {
                            SceneStoryboard.Pause();
                        }

                        // Move the lower cars along the x-axis in the middle of the screen and keeping the same upper y-axis.
                        dragLowerTranslation.X = (MainRoot.ActualWidth / 2) - (MainRoot.ActualWidth * leftSideThresholdRatio) - 100;
                        dragLowerTranslation.Y = dragUpperTranslation.Y;

                        // Keep the upper rectangle at the same position as the lower rectangle.
                        dragUpperTranslation.X = dragLowerTranslation.X;
                    }
                    // When the dual-screen device is fully opened, meaning that both screens are facing aware from each other.
                    else if (angleValue == 360)
                    {
                        // Move the lower cars along the x-axis on the right side of the screen and keeping the same upper y-axis.
                        dragLowerTranslation.X = MainRoot.ActualWidth * rightSideThresholdRatio;
                        dragLowerTranslation.Y = dragUpperTranslation.Y;

                        // Keep the upper rectangle at the same position as the lower rectangle.
                        dragUpperTranslation.X = dragLowerTranslation.X;
                        dragUpperTranslation.Y = dragLowerTranslation.Y;

                        // Making sure the SketchCar bouncing and rotating wheels animations begin for
                        // the first time or correctly being stopped and begin again if already started.
                        if (isCarAnimationStarted)
                        {
                            CarStoryboard.Stop();
                            CarStoryboard.Begin();
                        }
                        else
                        {
                            CarStoryboard.Begin();
                            isCarAnimationStarted = true;
                        }

                        // Making sure the scene animation begin for the first time or resume if paused.
                        if (isSceneAnimationStarted)
                        {
                            SceneStoryboard.Resume();
                        }
                        else
                        {
                            SceneStoryboard.Begin();
                            isSceneAnimationStarted = true;
                        }
                    }
                }
            });
        }
    }
}
