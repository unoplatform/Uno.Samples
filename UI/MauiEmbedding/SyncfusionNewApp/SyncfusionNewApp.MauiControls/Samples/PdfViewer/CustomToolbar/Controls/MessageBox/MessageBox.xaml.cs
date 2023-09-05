#region Copyright Syncfusion Inc. 2001-2023.
// Copyright Syncfusion Inc. 2001-2023. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace SyncfusionApp.MauiControls.Samples.PdfViewer.SfPdfViewer;

public partial class MessageBox : ContentView
{
    public event EventHandler<CloseClickedEventArgs?>? CloseClicked;
    public MessageBox()
    {
        InitializeComponent();
    }

    private void Ok_Clicked(object sender, EventArgs e)
    {
        Hide();
        CloseClickedEventArgs closeClickedEventArgs = new CloseClickedEventArgs(Title.Text);
        CloseClicked?.Invoke(this, closeClickedEventArgs);
    }

    internal void Show(string title, string message, string closeText = "CLOSE")
    {
        this.IsVisible = true;
        Title.Text = title;
        Message.Text = message;
        OkButton.Text = closeText;
    }

    internal void Hide()
    {
        this.IsVisible = false;
    }
}

public class CloseClickedEventArgs : EventArgs
{
    public string? Title { get; set; }
    public CloseClickedEventArgs(string title)
    {
        this.Title = title;
    }
}