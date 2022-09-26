namespace TimeEntryRia.LoginUI
{
    using System.Globalization;
    using System.ServiceModel.DomainServices.Client.ApplicationServices;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// <see cref="UserControl"/> class that shows the current login status and allows login and logout.
    /// </summary>
    public partial class LoginStatus : UserControl
    {
        private readonly AuthenticationService authService = WebContext.Current.Authentication;

        /// <summary>
        /// Creates a new <see cref="LoginStatus"/> instance.
        /// </summary>
        public LoginStatus()
        {
            this.InitializeComponent();

            this.welcomeText.SetBinding(TextBlock.TextProperty, WebContext.Current.CreateOneWayBinding("User.DisplayName", new StringFormatValueConverter(ApplicationStrings.WelcomeMessage)));
            this.authService.LoggedIn += this.Authentication_LoggedIn;
            this.authService.LoggedOut += this.Authentication_LoggedOut;
            this.UpdateLoginState();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            LoginRegistrationWindow loginWindow = new LoginRegistrationWindow();
            loginWindow.Show();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            this.authService.Logout(logoutOperation =>
            {
                if (logoutOperation.HasError)
                {
                    ErrorWindow.CreateNew(logoutOperation.Error);
                    logoutOperation.MarkErrorAsHandled();
                }
            }, /* userState */ null);
        }

        private void Authentication_LoggedIn(object sender, AuthenticationEventArgs e)
        {
            this.UpdateLoginState();
        }

        private void Authentication_LoggedOut(object sender, AuthenticationEventArgs e)
        {
            this.UpdateLoginState();
        }

        private void UpdateLoginState()
        {
            if (WebContext.Current.User.IsAuthenticated)
            {
                VisualStateManager.GoToState(this, (WebContext.Current.Authentication is WindowsAuthentication) ? "windowsAuth" : "loggedIn", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "loggedOut", true);
            }
        }
    }
}
