#region Copyright Syncfusion Inc. 2001-2023.
// Copyright Syncfusion Inc. 2001-2023. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace SyncfusionApp.MauiControls.Samples.PdfViewer.SfPdfViewer;

public partial class HyperlinkDialog : ContentView
{
    public string? uriString { get; set; }

    public event EventHandler<EventArgs?>? HyperlinkDialogClosed;

    public HyperlinkDialog()
    {
        InitializeComponent();
    }

    internal void Show(string title, string message, string link)
    {
        this.IsVisible = true;
        Title.Text = title;
        Message.Text = message;
        uriString = link;
    }

    private void Open_Clicked(object sender, EventArgs e)
    {
        if (uriString != null)
        {
            Browser.Default.OpenAsync(uriString, BrowserLaunchMode.SystemPreferred);
        }
        HyperlinkDialogClosed?.Invoke(this, EventArgs.Empty);
        this.IsVisible = false;
    }

    private void Cancel_Clicked(object sender, EventArgs e)
    {
        HyperlinkDialogClosed?.Invoke(this, EventArgs.Empty);
        this.IsVisible = false;
    }

    private void Close_Clicked(object sender, EventArgs e)
    {
        HyperlinkDialogClosed?.Invoke(this, EventArgs.Empty);
        this.IsVisible = false;
    }

}