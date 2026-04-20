using GridWatch.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Input;
using Uno.Extensions.Navigation;

namespace GridWatch.Presentation;

public sealed partial class LoginPage : Page
{
    private readonly AuthService _authService;

    public LoginPage()
    {
        this.InitializeComponent();
        _authService = AuthService.Instance;
    }

    private void OnLoginClick(object sender, RoutedEventArgs e)
    {
        PerformLogin();
    }

    private void OnKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            PerformLogin();
        }
    }

    private void PerformLogin()
    {
        var username = UsernameBox.Text?.Trim() ?? string.Empty;
        var password = PasswordBox.Password?.Trim() ?? string.Empty;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            ErrorText.Text = "Please enter your username and password.";
            ErrorText.Visibility = Visibility.Visible;
            return;
        }

        var success = _authService.Login(username, password);
        if (success)
        {
            ErrorText.Visibility = Visibility.Collapsed;
            var navigator = App.ServiceProvider?.GetService<INavigator>();
            _ = navigator?.NavigateRouteAsync(this, "Main");
        }
        else
        {
            ErrorText.Text = "Invalid credentials. Use: admin / grid2024";
            ErrorText.Visibility = Visibility.Visible;
        }
    }
}
