using SQLiteOneDriveInvoiceSample.Database.Enums;
using SQLiteOneDriveInvoiceSample.Presentation;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SQLiteOneDriveInvoiceSample.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Settings : Page
    {
        public SettingsVM ViewModel { get; set; } = new SettingsVM();
        
        public Settings()
        {
            this.InitializeComponent();            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (ViewModel == null)
            {
                ViewModel = new SettingsVM();
            }
            base.OnNavigatedTo(e);
        }

        private async void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            ViewModel = DataContext as SettingsVM;
            var toggleSwitch = sender as ToggleSwitch;

            if (toggleSwitch?.IsOn == true)
            {
                await ViewModel.OneDriveSetupAsync();
            }
            else if(toggleSwitch?.IsOn == false)
            {
                await ViewModel.LogOutAsync();
            }
        }

        private async void BackUpButtonClicked(object sender, RoutedEventArgs e)
        {
            if (ViewModel == null)
            {
                ViewModel = DataContext as SettingsVM;
            }

            await ViewModel.BackUp();
        }


        private async void RestoreButtonClicked(object sender, RoutedEventArgs e)
        {
            if (ViewModel == null)
            {
                ViewModel = DataContext as SettingsVM;
            }

            await ViewModel.Restore();
        }

        private async void SaveAccountDetailsClicked(object sender, RoutedEventArgs e)
        {
            if (ViewModel == null)
            {
                ViewModel = DataContext as SettingsVM;
            }

            await ViewModel.SaveAccount(ViewModel.UserAccount);
        }

        private async void SaveAddressDetailsClicked(object sender, RoutedEventArgs e)
        {
            if (ViewModel == null)
            {
                ViewModel = DataContext as SettingsVM;
            }

            await ViewModel.SaveAddress(ViewModel.UserAddress);
        }

        private async void AddressChecked(object sender, RoutedEventArgs e)
        {
            if (ViewModel == null)
            {
                ViewModel = DataContext as SettingsVM;
            }
#if NETFX_CORE
            if(ViewModel == null) { return; }
#endif
            ViewModel.UserAddress.Type = AddressType.Billing;
        }

        private async void AddressUnChecked(object sender, RoutedEventArgs e)
        {
            if (ViewModel == null)
            {
                ViewModel = DataContext as SettingsVM;
            }
#if NETFX_CORE
            if (ViewModel == null) { return; }
#endif
            ViewModel.UserAddress.Type = AddressType.Shipping;
        }

        private async void AddressIndeterminate(object sender, RoutedEventArgs e)
        {
            if (ViewModel == null)
            {
                ViewModel = DataContext as SettingsVM;
            }
#if NETFX_CORE
            if (ViewModel == null) { return; }
#endif
            ViewModel.UserAddress.Type = AddressType.Both;
        }
    }
}
