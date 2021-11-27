using Demo.Helpers;

using Microsoft.Graph;
using Microsoft.Identity.Client;

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using File = System.IO.File;

namespace Demo.CloudProvider
{
    public class OneDrive
    {

        #region OAuthSettings

        protected static class OAuthenticationSettings
        {
            public const string ApplicationId = "4f554894-133f-44c9-92fe-bdcb164ddaa0";
            public const string RedirectUri = "soloApp://redirect";//"msal4f554894-133f-44c9-92fe-bdcb164ddaa0://auth";
            public static readonly string[] Scopes = new string[] { AppConstants.FilesReadWriteAppFolder, AppConstants.UserRead, AppConstants.DeviceRead };

            public static Uri Authority = new Uri("https://login.microsoftonline.com/consumers/oauth2/v2.0/");
        }

        #endregion

        #region Property(ies)

        private static GraphServiceClient graphServiceClient;

        private static IPublicClientApplication pca;

        // UIParent used by Android version of the app
        public static object AuthenticationUIParent { get; private set; }

        // Keychain security group used by iOS version of the app
        public static string IOSKeychainSecurityGroup { get; private set; }

        #endregion

        #region Graph Initiialization(s)

        public static void BuildPublicClientApplication()
        {
            var builder = PublicClientApplicationBuilder.Create(OAuthenticationSettings.ApplicationId).WithAuthority(OAuthenticationSettings.Authority).WithRedirectUri(OAuthenticationSettings.RedirectUri);


            if (!string.IsNullOrEmpty(IOSKeychainSecurityGroup))
            {
                builder = builder.WithIosKeychainSecurityGroup(IOSKeychainSecurityGroup);
            }

            pca = builder.Build();
            
        }

        public static async Task<AuthenticationResult> InitializeWithInteractiveProviderAsync()
        {

            var accounts = await pca.GetAccountsAsync();

            try
            {
                var builder = pca.AcquireTokenInteractive(OAuthenticationSettings.Scopes)
                         .WithAccount(accounts.FirstOrDefault())
                         .WithPrompt(Microsoft.Identity.Client.Prompt.SelectAccount);

                if (App.AuthenticationUiParent != null)
                {
                    builder = builder
                        .WithParentActivityOrWindow(App.AuthenticationUiParent);
                }



#if NETFX_CORE || __ANDROID__ || __IOS__
                builder.WithUseEmbeddedWebView(true);
#endif

                var result = await builder.ExecuteAsync();

                return result;
            }
            catch (Exception exception)
            {
                throw exception;
                //await Dialogs.ExceptionDialogAsync(exception);
                //return null;
            }
        }

        public static async Task<AuthenticationResult> InitializeWithSilentProviderAsync()
        {
            var accounts = await pca.GetAccountsAsync();

            try
            {
                var result = await pca.AcquireTokenSilent(OAuthenticationSettings.Scopes, accounts.FirstOrDefault())
                .ExecuteAsync().ConfigureAwait(false);

                return result;
            }
            catch (Exception exception)
            {
                //await Dialogs.ExceptionDialogAsync(exception);
                return null;
            }
        }

        public static async Task InitializeGraphClientAsync(AuthenticationResult authenticationResult)
        {
            try
            {
                graphServiceClient = new GraphServiceClient(new DelegateAuthenticationProvider(
                async (requestMessage) =>
                {
                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authenticationResult.AccessToken);
                }));
            }
            catch (Exception exception)
            {
                await Dialogs.ExceptionDialogAsync(exception);
            }

        }

        public static async Task RemoveAccountsAsync()
        {
            var accounts = await pca.GetAccountsAsync();
            while (accounts.Any())
            {
                await pca.RemoveAsync(accounts.First());
                accounts = await pca.GetAccountsAsync();
            }           
        }

        #endregion

        #region BackUp & Restoration Implementation(s)

        public static async Task<bool> BackUp(string databaseName)
        {
            try
            {
                await BackupDatabaseAsync(databaseName);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public static async Task<bool> Restore(string databaseName)
        {
            try
            {
                await RestoreDatabaseAsync(databaseName);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        #endregion

        #region Helper Method(s)

        public static async Task<DriveItem> GetAppFolderAsync()
        {
            try
            {
                if (graphServiceClient == null)
                {
                    var authResult = await InitializeWithSilentProviderAsync();
                    await InitializeGraphClientAsync(authResult);

                }

                return await graphServiceClient.Me.Drive.Special.AppRoot.Request().GetAsync();
            }
            catch (Exception exception)
            {
                Debug.WriteLine("GetAppsFolder Error: " + exception.Message);
                await Dialogs.GenericDialogAsync("OneDrive GetAppsFolder Error", exception.Message, "OK");
                return null;
            }
        }

        public static  async Task<(string username, string email, bool isSignedIn)?> AccountInfo()
        {
            try
            {
                if (graphServiceClient == null)
                {
                    var authResult = await InitializeWithSilentProviderAsync();
                    await InitializeGraphClientAsync(authResult);
                }

                var account = await graphServiceClient.Me.Request().Select(user => new
                {
                    user.DisplayName,
                    user.Mail,
                    user.UserPrincipalName
                }).GetAsync();

                return (username: account.DisplayName, email: string.IsNullOrEmpty(account.Mail) ? account.UserPrincipalName : account.Mail, isSignedIn: true);
            }
            catch (Exception exception)
            {
                Debug.WriteLine("Account Info Error: " + exception.Message);
                await Dialogs.GenericDialogAsync("OneDrive GetAppsFolder Error", exception.Message, "OK");
                return null;
            }            
        }

        #endregion

        #region Sync Method(s)

        private static async Task RestoreDatabaseAsync(string databaseName)
        {
            try
            {
                Stream stream = null;
                if (graphServiceClient == null)
                {
                    var authResult = await InitializeWithSilentProviderAsync();
                    await InitializeGraphClientAsync(authResult);

                }

                var databasePath = await graphServiceClient.Me.Drive.Special.AppRoot.Children[databaseName].Request().GetAsync();

                if (databasePath != null)
                {
                    stream = await graphServiceClient.Me.Drive.Special.AppRoot.Children[databaseName].Content.Request().GetAsync();

                    var destinationPath = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), databaseName));
                    using (var databaseDriveItem = File.Create(destinationPath))
                    {
                        stream.Seek(0, SeekOrigin.Begin);
                        await stream.CopyToAsync(databaseDriveItem);
                    }
                }
            }
            catch (Exception exception)
            {
                await Dialogs.ExceptionDialogAsync(exception);
            }
        }

        private static async Task BackupDatabaseAsync(string databaseName)
        {
            try
            {
                var sourcPath = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), databaseName));
                var databaseData = await File.ReadAllBytesAsync(sourcPath);
                var stream = new MemoryStream(databaseData);

                if (graphServiceClient == null)
                {
                    var authResult = await InitializeWithSilentProviderAsync();
                    await InitializeGraphClientAsync(authResult);

                }

                await graphServiceClient.Me.Drive.Special.AppRoot.Children[databaseName].Content.Request().PutAsync<DriveItem>(stream);
            }
            catch (Exception exception)
            {
                await Dialogs.ExceptionDialogAsync(exception);
            }
        }

        #endregion
    }
}
