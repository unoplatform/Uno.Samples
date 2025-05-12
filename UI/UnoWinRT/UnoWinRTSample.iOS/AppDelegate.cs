using Windows.Devices.Sensors;
using UIKit;
using Foundation;

namespace UnoWinRTSample.iOS
{
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        public override UIWindow? Window { get; set; }
        private UILabel? _accelerometerLabel;
        private Accelerometer? _accelerometer;

        public override bool FinishedLaunching(UIApplication application, NSDictionary? launchOptions)
        {
            Window = new UIWindow(UIScreen.MainScreen.Bounds);

            var vc = new UIViewController();
            _accelerometerLabel = new UILabel(Window!.Frame)
            {
                BackgroundColor = UIColor.SystemBackground,
                TextAlignment = UITextAlignment.Center,
                Text = "Waiting for accelerometer...",
                AutoresizingMask = UIViewAutoresizing.All,
                Lines = 3
            };
            vc.View!.AddSubview(_accelerometerLabel);
            Window.RootViewController = vc;
            Window.MakeKeyAndVisible();

            _accelerometer = Accelerometer.GetDefault();
            if (_accelerometer != null)
            {
                _accelerometer.ReportInterval = 100;
                _accelerometer.ReadingChanged += Accelerometer_ReadingChanged;
            }
            else
            {
                _accelerometerLabel.Text = "Accelerometer not available";
            }

            return true;
        }

        private void Accelerometer_ReadingChanged(Accelerometer sender, AccelerometerReadingChangedEventArgs args)
        {
            var x = args.Reading.AccelerationX;
            var y = args.Reading.AccelerationY;
            var z = args.Reading.AccelerationZ;

            // Update the UI on the main thread
            InvokeOnMainThread(() =>
            {
                _accelerometerLabel!.Text = $"X: {x:F3}\nY: {y:F3}\nZ: {z:F3}";
            });
        }
    }
}
