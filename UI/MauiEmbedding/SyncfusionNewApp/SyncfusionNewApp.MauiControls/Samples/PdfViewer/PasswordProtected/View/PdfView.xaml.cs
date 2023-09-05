#region Copyright Syncfusion Inc. 2001-2023.
// Copyright Syncfusion Inc. 2001-2023. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace SyncfusionApp.MauiControls.Samples.PdfViewer.SfPdfViewer;
[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class PdfView : ContentView
{
    public PdfView(PasswordProtectedViewModel viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();
    }

    internal void Unload()
    {
        PdfViewer?.UnloadDocument();
        PdfViewer?.Handler?.DisconnectHandler();
    }

    private void PdfViewerDocumentLoadFailed(object sender, Syncfusion.Maui.PdfViewer.DocumentLoadFailedEventArgs e)
    {
        if (e.Message == "Document loading has been cancelled")
        {
            if (this.BindingContext is PasswordProtectedViewModel bindingContext)
            {
                bindingContext.ToggleContent();
            }
        }
    }
}