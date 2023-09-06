#region Copyright Syncfusion Inc. 2001-2023.
// Copyright Syncfusion Inc. 2001-2023. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Microsoft.Maui.Platform;
using SyncfusionApp.MauiControls.Samples.Base;
using Syncfusion.Maui.PdfViewer;

namespace SyncfusionApp.MauiControls.Samples.PdfViewer.SfPdfViewer;
[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class CustomToolbar : SampleView
{
    private string? previousDocument = string.Empty;
    //It is used to delay the current thread's execution, until the user enters the password.
    ManualResetEvent manualResetEvent = new ManualResetEvent(false);
    ToolbarView? toolbar;

#if ANDROID || IOS
    private ViewCell? lastCell;
#endif

    public SearchView SearchView { get; set; }

    public CustomToolbar()
    {
        InitializeComponent();
#if ANDROID || IOS
        SearchView = MobileSearchView;
        toolbar = MobileToolbar;
#else
        SearchView = DesktopSearchView;
        toolbar = DesktopToolbar;
#endif
        UpdateToolbarProperties();
        SearchView.SearchHelper = PdfViewer;
        SearchView.NoMatchesFound += NoMatchesFound;

#if WINDOWS
        //Note: Due to known MAUI issue (#13714) in Entry control when the property "IsVisible=false" by default, the mentioned property is set after the control is loaded.
        if (SearchView.SearchInputEntry != null)
            SearchView.SearchInputEntry.Loaded += SearchInputEntryLoaded;
#endif            
    }

    void UpdateToolbarProperties()
    {
        if (toolbar != null)
        {
            toolbar.ParentView = this;
            toolbar.PdfViewer = PdfViewer;
        }
    }

    private void SearchInputEntryLoaded(object? sender, EventArgs e)
    {
        if (SearchView != null)
            SearchView.IsVisible = false;
    }

    private void NoMatchesFound(object? sender, EventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() => messageBox.Show("Search Result", "No matches were found"));
    }

    /// <summary>
    /// Handles when the message box close button is clicked.
    /// </summary>
    private void MessageBox_CloseClicked(object? sender, CloseClickedEventArgs? e)
    {
        if (this.BindingContext is CustomToolbarViewModel bindingContext && e?.Title == "Incorrect Password")
            bindingContext.UpdateFileName("Password protected PDF");
    }

    internal void CloseAllDialogs()
    {
        if (BindingContext is CustomToolbarViewModel bindingContext)
            bindingContext?.CloseAllDialogsCommand?.Execute(true);
    }

    /// <summary>
    /// Handles when leaving the current page
    /// </summary>
    public override void OnDisappearing()
    {
        base.OnDisappearing();
        PdfViewer?.UnloadDocument();
        PdfViewer?.Handler?.DisconnectHandler();
    }

    /// <summary>
    /// Handles when a Pdf is tapped.
    /// </summary>
    private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
    {
#if ANDROID
        if (Platform.CurrentActivity?.CurrentFocus != null)
            Platform.CurrentActivity.HideKeyboard(Platform.CurrentActivity.CurrentFocus);
#endif
        if (this.BindingContext is CustomToolbarViewModel bindingContext && bindingContext.IsFilePickerVisible)
        {
            bindingContext.IsFilePickerVisible = false;
        }
    }

    /// <summary>
    /// Handles when a file picker content is tapped.
    /// </summary>
    private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        if (this.BindingContext is CustomToolbarViewModel bindingContext)
        {
            if (bindingContext.IsFilePickerVisible)
            {
                if (!passwordDialog.IsVisible || previousDocument != e.Item.ToString())
                    bindingContext.UpdateFileName(e.Item.ToString());
                bindingContext.IsFilePickerVisible = false;
                previousDocument = e.Item.ToString();
            }
        }
    }

    /// <summary>
    /// Handles password requested event.
    /// </summary>
    private void PdfViewer_PasswordRequested(object sender, PasswordRequestedEventArgs e)
    {
        e.Handled = true;

        //Show the password dialog.
        passwordDialog.Dispatcher.Dispatch(() => {
            passwordDialog.IsVisible = true;
#if WINDOWS
            passwordDialog.passwordEntry?.Focus();
#endif
        });

        //Block the current thread until user enters the password.
        manualResetEvent.WaitOne();
        manualResetEvent.Reset();

        if (!string.IsNullOrEmpty(passwordDialog.Password))
        {
            e.Password = passwordDialog.Password;
            passwordDialog.Password = null;
        }
    }

    /// <summary>
    /// Handles hyperlink tapped event.
    /// </summary>
    private void PdfViewer_HyperlinkClicked(object sender, HyperlinkClickedEventArgs e)
    {
        SearchView?.Close();
        CloseAllDialogs();
        e.Handled = true;
        if (e.Uri != null)
        {
            MainThread.BeginInvokeOnMainThread(() => hyperlinkDialog.Show("Open Web Page", "Do you want to open\n" + e.Uri + "?", e.Uri));
        }
    }

    /// <summary>
    /// Handles when the password dialog box is closed.
    /// </summary>
    private void passwordDialog_PasswordDialogClosed(object sender, EventArgs e)
    {
        //Release the current waiting thread to execute.
        manualResetEvent.Set();
    }

    /// <summary>
    /// Handles when a document fails to load.
    /// </summary>
    private void PdfViewer_DocumentLoadFailed(object sender, DocumentLoadFailedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            toolbar!.SearchButton!.IsEnabled = false;
            toolbar!.OutlineButton!.IsEnabled = false;
            SearchView?.Close();
            CloseAllDialogs();
        });
        e.Handled = true;
        if (e.Message == "Invalid cross reference table.")
            MainThread.BeginInvokeOnMainThread(() => messageBox.Show("Error", "Failed to load the PDF document."));
        else if (e.Message == "Can't open an encrypted document. The password is invalid." && !passwordDialog.IsVisible)
            MainThread.BeginInvokeOnMainThread(() => messageBox.Show("Incorrect Password", "The password you entered is incorrect. Please try again.", "OK"));
    }

    /// <summary>
    /// Handles when a file picker content is tapped.
    /// </summary>
    private void ViewCell_Tapped(object sender, EventArgs e)
    {
#if ANDROID || IOS
        if (lastCell != null)
            lastCell.View.BackgroundColor = Colors.Transparent;
        var viewCell = (ViewCell)sender;
        if (viewCell.View != null)
        {
            viewCell.View.BackgroundColor = Color.FromArgb("#0A000000");
            lastCell = viewCell;
        }
#endif
    }

    /// <summary>
    /// Handles when a document is loaded.
    /// </summary>
    private void PdfViewer_DocumentLoaded(object sender, EventArgs e)
    {
        if (this.BindingContext is CustomToolbarViewModel bindingContext)
        {
            bindingContext.IsDocumentLoaded = true;
        }
        MainThread.BeginInvokeOnMainThread(() =>
        {
            toolbar!.SearchButton!.IsEnabled = true;
            toolbar!.OutlineButton!.IsEnabled = true;
        });
    }

    /// <summary>
    /// Handles when a document is unloaded.
    /// </summary>
    private void PdfViewer_DocumentUnloaded(object sender, EventArgs e)
    {
        if (this.BindingContext is CustomToolbarViewModel bindingContext)
        {
            bindingContext.IsDocumentLoaded = false;
        }
        toolbar!.SearchButton!.IsEnabled = false;
        toolbar!.OutlineButton!.IsEnabled = false;
        SearchView?.Close();
    }

    private void PdfViewer_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(PdfViewer.IsOutlineViewVisible) && !PdfViewer.IsOutlineViewVisible)
        {
            toolbar!.PageNumberEntry?.HideKeyboard();
        }
        if (e.PropertyName == nameof(PdfViewer.IsOutlineViewVisible) && DeviceInfo.Current.Idiom == DeviceIdiom.Phone)
        {
            toolbar!.IsVisible = !PdfViewer.IsOutlineViewVisible;
        }
    }
}