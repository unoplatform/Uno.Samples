using System;
using TimeEntryUno.Shared.Helpers;
using TimeEntryUno.Shared.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace TimeEntryUno.Shared.Views.Login
{
    public sealed partial class LoginForm : ContentDialog
    {
        public string ErrorMessage { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool? StaySignedIn { get; set; } = false;
        public bool IsBusy { get; set; }

        public LoginForm()
        {
            this.InitializeComponent();
            AuthenticationService.Instance.LoggedIn += Instance_LoggedIn;
            AuthenticationService.Instance.LoginFailed += Instance_LoginFailed;
            this.Closing += LoginForm_Closing;
        }

        private void LoginForm_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            if (IsBusy)
            {
                args.Cancel = true;
            }
            else
            {
                AuthenticationService.Instance.LoggedIn -= Instance_LoggedIn;
                AuthenticationService.Instance.LoginFailed -= Instance_LoginFailed;
            }
        }

        private void Instance_LoggedIn(object sender, EventArgs e)
        {
            IsBusy = false;
            Hide();
        }

        private void Instance_LoginFailed(object sender, EventArgs e)
        {
            ErrorMessage = ErrorMessageHelper.GetErrorMessageResource("LoginDialogLoginFailed");
            IsEnabled = true;
            IsBusy = false;
            Bindings.Update();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = ErrorMessageHelper.GetErrorMessageResource("LoginDialogUserNameRequired");
            }
            else
            {
                ErrorMessage = string.Empty;
                IsEnabled = false;
                IsBusy = true;

                _ = AuthenticationService.Instance.LoginUser(UserName, Password);
            }

            Bindings.Update();

            // We only close if we log in successfully or cancel
            args.Cancel = true;
        }

        public Visibility IsBusyVisible()
        {
            return IsBusy ? Visibility.Visible : Visibility.Collapsed;
        }

        public Visibility IsErrorMessageVisible()
        {
            return string.IsNullOrWhiteSpace(ErrorMessage) ? Visibility.Collapsed : Visibility.Visible;
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

    }
}
