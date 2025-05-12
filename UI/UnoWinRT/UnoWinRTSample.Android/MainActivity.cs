using Windows.Devices.Sensors;

namespace UnoWinRTSample.Android
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private TextView? _accelerometerTextView;

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            // Find the TextView for displaying accelerometer data
            _accelerometerTextView = FindViewById<TextView>(Resource.Id.textViewAccelerometer);

            var accelerometer = Accelerometer.GetDefault();
            if (accelerometer != null)
            {
                accelerometer.ReadingChanged += Accelerometer_ReadingChanged;
            }
        }

        private void Accelerometer_ReadingChanged(Accelerometer sender, AccelerometerReadingChangedEventArgs args)
        {
            var x = args.Reading.AccelerationX;
            var y = args.Reading.AccelerationY;
            var z = args.Reading.AccelerationZ;

            // Update the UI with the accelerometer data on the main thread
            RunOnUiThread(() =>
            {
                _accelerometerTextView?.SetText(
                    $"X: {x:F3}\nY: {y:F3}\nZ: {z:F3}",
                    TextView.BufferType.Normal
                );
            });
        }
    }
}