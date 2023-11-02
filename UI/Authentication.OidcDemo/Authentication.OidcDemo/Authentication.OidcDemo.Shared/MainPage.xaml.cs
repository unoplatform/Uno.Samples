using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Browser;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;

namespace Authentication.OidcDemo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            PrepareClient();
        }

        private OidcClient _oidcClient;
        private AuthorizeState _loginState;
        private Uri _logoutUrl;

        private async void PrepareClient()
        {
            
            var redirectUri = WebAuthenticationBroker.GetCurrentApplicationCallbackUri().OriginalString;

            // Create options for endpoint discovery
            var options = new OidcClientOptions
            {
                Authority = "https://demo.duendesoftware.com/",
                ClientId = "interactive.confidential",
                ClientSecret = "secret",
                Scope = "openid profile email api offline_access",
                RedirectUri = redirectUri,
                PostLogoutRedirectUri = redirectUri,
                //ResponseMode = OidcClientOptions.AuthorizeResponseMode.Redirect,
                //Flow = OidcClientOptions.AuthenticationFlow.AuthorizationCode
            };

            options.Browser = new WebAuthenticatorBrowser();

            // Create the client. In production application, this is often created and stored
            // directly in the Application class.
            _oidcClient = new OidcClient(options);

            // Invoke Discovery and prepare a request state, containing the nonce.
            // This is done here to ensure the discovery mechanism is done before
            // the user clicks on the SignIn button. Since the opening of a web window
            // should be done during the handling of a user interaction (here it's the button click),
            // it will be too late to reach the discovery endpoint.
            // Not doing this could trigger popup blocker mechanisms in browsers.
            _loginState = await _oidcClient.PrepareLoginAsync();
            btnSignin.IsEnabled = true;

            // Same for logout url.
            _logoutUrl = new Uri(await _oidcClient.PrepareLogoutAsync(new LogoutRequest()));
            btnSignout.IsEnabled = true;
        }

        private async void SignIn_Clicked(object sender, RoutedEventArgs e)
        {
            var startUri = new Uri(_loginState.StartUrl);

            // Important: there should be NO await before calling .AuthenticateAsync() - at least
            // on WebAssembly, in order to prevent triggering the popup blocker mechanisms.
            var userResult = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, startUri);

            if (userResult.ResponseStatus != WebAuthenticationStatus.Success)
            {
                txtAuthResult.Text = "Canceled";
                // Error or user cancellation
                return;
            }

            // User authentication process completed successfully.
            // Now we need to get authorization tokens from the response
            var authenticationResult = await _oidcClient.ProcessResponseAsync(userResult.ResponseData, _loginState);

            if (authenticationResult.IsError)
            {
                var errorMessage = authenticationResult.Error;
                // TODO: do something with error message
                txtAuthResult.Text = $"Error {errorMessage}";
                return;
            }

            // That's completed. Here you have to token, ready to do something
            var token = authenticationResult.AccessToken;
            var refreshToken = authenticationResult.RefreshToken;

            // TODO: make something useful with the tokens
            txtAuthResult.Text = $"Success, token is {token}";
        }

        private async void SignOut_Clicked(object sender, RoutedEventArgs e)
        {
            // Important: there should be NO other awaits before calling .AuthenticateAsync() - at least
            // on WebAssembly, in order to prevent triggering the popup blocker mechanisms.
            await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, _logoutUrl);
        }
    }


    internal class WebAuthenticatorBrowser : IBrowser
    {
        public async Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken cancellationToken = default)
        {
            try
            {
#if WINDOWS && !HAS_UNO
			var userResult = await WinUIEx.WebAuthenticator.AuthenticateAsync(new Uri(options.StartUrl), new Uri(options.EndUrl));
			var callbackurl = $"{options.EndUrl}/?{string.Join("&", userResult.Properties.Select(x => $"{x.Key}={x.Value}"))}";
			return new BrowserResult
			{
				Response = callbackurl
			};
#else
                var userResult = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, new Uri(options.StartUrl), new Uri(options.EndUrl));

                return new BrowserResult
                {
                    Response = userResult.ResponseData
                };
#endif
            }
            catch (Exception ex)
            {
                return new BrowserResult()
                {
                    ResultType = BrowserResultType.UnknownError,
                    Error = ex.ToString()
                };
            }
        }


    }
}
