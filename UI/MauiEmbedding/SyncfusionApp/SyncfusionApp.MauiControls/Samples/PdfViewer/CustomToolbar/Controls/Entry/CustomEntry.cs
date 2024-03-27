#region Copyright Syncfusion Inc. 2001-2023.
// Copyright Syncfusion Inc. 2001-2023. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace SyncfusionApp.MauiControls.Samples.PdfViewer.SfPdfViewer
{
    public class CustomEntry : Entry
    { 
        public CustomEntry()
        {
            this.HandlerChanged += OnHandlerChanged;
            this.Focused += CustomEntryFocused;
            IsSpellCheckEnabled = false;
            IsTextPredictionEnabled = false;
        }

        private void CustomEntryFocused(object? sender, FocusEventArgs e)
        {
            if (e.IsFocused == true)
                ShowKeyboard();
            else
                HideKeyboard();
        }

        public void ShowKeyboard()
        {
#if ANDROID            
            if (this.Handler != null)
            {
                if (this.Handler.PlatformView is Android.Widget.TextView textView)
                {
                    ShowKeyboard(textView);
                }
            }
#endif
        }

        public void HideKeyboard()
        {
#if ANDROID
            if (this.Handler != null)
            {
                if (this.Handler.PlatformView is Android.Widget.TextView textView)
                {
                    HideKeyboard(textView);
                }
            }
#endif            
        }

#if ANDROID
        private void ShowKeyboard(Android.Views.View inputView)
        {
            if (inputView.Context == null)
            {
                return;
            }
            using (var inputMethodManager = (Android.Views.InputMethods.InputMethodManager?)inputView.Context.GetSystemService(Android.Content.Context.InputMethodService))
            {
                inputMethodManager?.ShowSoftInput(inputView, Android.Views.InputMethods.ShowFlags.Forced);
            }
        }

        private void HideKeyboard(Android.Views.View inputView)
        {
            using (var inputMethodManager = (Android.Views.InputMethods.InputMethodManager?)inputView.Context?.GetSystemService(Android.Content.Context.InputMethodService))
            {
                if (inputMethodManager != null)
                {
                    var token = Platform.CurrentActivity?.CurrentFocus?.WindowToken;
                    inputMethodManager.HideSoftInputFromWindow(token, Android.Views.InputMethods.HideSoftInputFlags.None);
                    Platform.CurrentActivity?.Window?.DecorView.ClearFocus();
                }
            }
        }
#endif

        private void OnHandlerChanged(object? sender, EventArgs e)
        {
            if (this.Handler != null)
            {
                var handler = this.Handler as Microsoft.Maui.Handlers.EntryHandler;
                if (handler != null)
                {
#if ANDROID
                    handler.PlatformView.SetSelectAllOnFocus(true);
#elif IOS || MACCATALYST
                    // TODO: Disabled to fix CI
                    // handler.PlatformView.EditingDidBegin += (s, e) =>
                    // {
                    //     handler.PlatformView.PerformSelector(new ObjCRuntime.Selector("selectAll"), null, 0.0f);
                    // };
#elif WINDOWS
                    handler.PlatformView.Padding= new Microsoft.UI.Xaml.Thickness(4);
                    handler.PlatformView.GotFocus += (s, e) =>
                    {
                        handler.PlatformView.SelectAll();
                    };
#endif
                }
            }
        }
    }
}