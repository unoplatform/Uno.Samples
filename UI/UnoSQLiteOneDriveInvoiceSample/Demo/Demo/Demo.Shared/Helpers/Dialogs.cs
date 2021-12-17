using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Windows.UI.Xaml.Controls;

namespace Demo.Helpers
{
    public static class Dialogs
    {
        public static async Task ExceptionDialogAsync(Exception exception)
        {

            var dialog = new ContentDialog 
            {
                Title = $"Error occured at {exception.Source}",
                Content = $" Details : {exception.Message}",
                CloseButtonText = "OK"
            };

            await dialog.ShowAsync();
        }

        public static async Task GenericDialogAsync(string title, string content, string buttonText)
        {
            var dialog = new ContentDialog
            {
                Title = title,
                Content = content,
                CloseButtonText = buttonText
            };

            await dialog.ShowAsync();
        }
    }
}
