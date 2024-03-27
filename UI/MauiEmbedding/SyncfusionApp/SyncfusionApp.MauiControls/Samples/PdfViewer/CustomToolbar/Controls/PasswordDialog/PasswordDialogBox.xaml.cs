#region Copyright Syncfusion Inc. 2001-2023.
// Copyright Syncfusion Inc. 2001-2023. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Runtime.CompilerServices;
namespace SyncfusionApp.MauiControls.Samples.PdfViewer.SfPdfViewer;

public partial class PasswordDialogBox : ContentView
{
    public string? Password { get; set; }
    
#if WINDOWS
    internal Entry? passwordEntry { get; set; }
#endif

    /// <summary>
    /// Occurs when the password dialog box is closed.
    /// </summary>
    public event EventHandler<EventArgs>? PasswordDialogBoxClosed;

    public PasswordDialogBox()
    {
        InitializeComponent();
#if WINDOWS
        this.passwordEntry = passwordBlock;
        okButton.HandlerChanged += OkButton_HandlerChanged;
        cancelButton.HandlerChanged += CancelButton_HandlerChanged;
#endif
    }

#if ANDROID
    protected override async void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
#else
    protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
#endif
        if (propertyName == "IsVisible")
        {
            if (this.IsVisible == true)
            {
                passwordBlock.Focus();
#if ANDROID
                if (passwordBlock?.Handler!.PlatformView is Android.Widget.TextView textView)
                {
                    await Task.Delay(10);
                    ShowKeyboard(textView);
                }
#endif
            }
            else
            {
                ResetHelperTextAppearance();
                passwordBorder.Stroke = Color.FromArgb("66000000");
                helperText.Text = "Enter password";
#if ANDROID
                if (passwordBlock?.Handler != null)
                {
                    if (passwordBlock?.Handler.PlatformView is Android.Widget.TextView textView)
                        HideKeyboard(textView);
                }
#endif
            }
        }
        base.OnPropertyChanged(propertyName);
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

    private void CancelButton_Clicked(object sender, EventArgs e)
    {
        Password = null;
        passwordBlock.Text = "";
        // Fire the event when the password dialog box is closed.
        PasswordDialogBoxClosed?.Invoke(this, EventArgs.Empty);
        this.IsVisible = false;
    }

    private void OkButton_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(passwordBlock.Text) == false)
        {
            Password = passwordBlock.Text;
            passwordBlock.Text = "";
            // Fire the event when the password dialog box is closed.
            PasswordDialogBoxClosed?.Invoke(this, EventArgs.Empty);
            this.IsVisible = false;
        }
        else
        {
            helperText.TextColor = Color.FromArgb("#F40606");
            helperText.Opacity = 1;
            passwordBorder.Stroke = Color.FromArgb("#F40606");
            helperText.Text = "Password cannot be empty";
        }
    }

    private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (sender is CheckBox checkBox)
        {
            if (checkBox.IsChecked)
                passwordBlock.IsPassword = false;
            else
                passwordBlock.IsPassword = true;
        }
    }

    void ResetHelperTextAppearance()
    {
        helperText.TextColor = Color.FromArgb("FF000000");
        helperText.Opacity = 0.6;
    }

    private void passwordBlock_TextChanged(object sender, TextChangedEventArgs e)
    {
        ResetHelperTextAppearance();
        helperText.TextColor = Color.FromArgb("FF000000");
        helperText.Opacity = 0.6;
        passwordBorder.Stroke = Color.FromArgb("66000000");
        if (string.IsNullOrEmpty(e.NewTextValue) == false)
        {
            helperText.Text = "";
        }
        else
        {
            helperText.Text = "Enter password";
        }
    }

    private void PasswordEntry_HandlerChanged(object sender, EventArgs e)
    {
        if (passwordBlock.Handler != null)
        {
            var handler = passwordBlock.Handler as Microsoft.Maui.Handlers.EntryHandler;
            if (handler != null)
            {
#if WINDOWS
                handler.PlatformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
#elif IOS || MACCATALYST
                // TODO: Disabled to fix CI
                //handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
#endif
            }
        }
    }

#if WINDOWS
    private void OkButton_HandlerChanged(object? sender, EventArgs e)
    {
        var handler = okButton.Handler as Microsoft.Maui.Handlers.ButtonHandler;
        if (handler != null)
        {
            handler.PlatformView.PointerEntered += PlatformView_OkButtonPointerEntered;
            handler.PlatformView.PointerExited += PlatformView_OkButtonPointerExited;
        }
    }

    private void CancelButton_HandlerChanged(object? sender, EventArgs e)
    {
        var handler = cancelButton.Handler as Microsoft.Maui.Handlers.ButtonHandler;
        if (handler != null)
        {
            handler.PlatformView.PointerEntered += PlatformView_CancelButtonPointerEntered;
            handler.PlatformView.PointerExited += PlatformView_CancelButtonPointerExited;
        }
    }

    private void PlatformView_CancelButtonPointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        if (cancelButton.Handler != null)
            cancelButton.BackgroundColor = Color.FromArgb("#00000000");
    }

    private void PlatformView_CancelButtonPointerEntered(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        if (cancelButton.Handler != null)
            cancelButton.BackgroundColor = Color.FromArgb("#0F000000"); 
    }

    private void PlatformView_OkButtonPointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        if (okButton.Handler != null)
            okButton.BackgroundColor = Color.FromArgb("#00000000");
    }

    private void PlatformView_OkButtonPointerEntered(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        if (okButton.Handler != null)
            okButton.BackgroundColor = Color.FromArgb("#0F000000");
    }
#endif
}