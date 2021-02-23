using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TimeEntryUno.Shared.Helpers
{
    internal static class ErrorDialogHelper
    {
        internal static async Task ShowErrorAsync(string titleResourceKey, string contentResourceKey)
        {
            await CoreApplication.MainView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                var errorDialog = new ContentDialog
                {
                    Title = ErrorMessageHelper.GetErrorMessageResource(titleResourceKey),
                    Content = ErrorMessageHelper.GetErrorMessageResource(contentResourceKey)
                }.SetPrimaryButton(ErrorMessageHelper.GetErrorMessageResource("Ok"));
                await errorDialog.ShowOneAtATimeAsync();
            });
        }

        internal static async Task ShowFatalErrorAsync<TErrorPage>(string titleResourceKey, string contentResourceKey)
        where TErrorPage : Page, new()
        {
            await CoreApplication.MainView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () => 
                {
                    //var errorDialog = new ContentDialog
                    //{
                    //    Title = ErrorMessageHelper.GetErrorMessageResource(titleResourceKey),
                    //    Content = ErrorMessageHelper.GetErrorMessageResource(contentResourceKey)
                    //}.SetPrimaryButton(ErrorMessageHelper.GetErrorMessageResource("ExitApp"));
                    var errorDialog = new ContentDialog
                    {
                        Title = ErrorMessageHelper.GetErrorMessageResource(titleResourceKey),
                        Content = ErrorMessageHelper.GetErrorMessageResource(contentResourceKey)
                    };

                    errorDialog.Closing += ErrorDialog_Closing;

                    await errorDialog.ShowOneAtATimeAsync();

                    // if the error is fatal, then the app should either be closed, or disabled
#if __WASM__
                    Window.Current.Content = new TErrorPage();
#else
                    Application.Current.Exit();
#endif
                });
        }

        private static void ErrorDialog_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            args.Cancel = true;
        }
    }
}
