using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace UnoBluetoothExplorer
{
    [Activity(
            MainLauncher = true,
            ConfigurationChanges = global::Uno.UI.ActivityHelper.AllConfigChanges,
            WindowSoftInputMode = SoftInput.AdjustPan | SoftInput.StateHidden
        )]
    public class MainActivity : Microsoft.UI.Xaml.ApplicationActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            RequestBluetoothPermissions();
        }

        private void RequestBluetoothPermissions()
        {
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.M)
            {
                if ((CheckSelfPermission(Manifest.Permission.Bluetooth) != Permission.Granted) || (CheckSelfPermission(Manifest.Permission.BluetoothConnect) != Permission.Granted))
                {
                    if (ShouldShowRequestPermissionRationale(Manifest.Permission.Bluetooth) || ShouldShowRequestPermissionRationale(Manifest.Permission.BluetoothConnect))
                    {
                        AlertDialog.Builder builder = new AlertDialog.Builder(this);
                        builder.SetTitle("Permission")
                            .SetMessage("This app needs Bluetooth permissions to connect");
                        builder.Create().Show();
                    }

                    RequestPermissions(new string[] { Manifest.Permission.Bluetooth, Manifest.Permission.BluetoothConnect }, 1);
                }
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}

