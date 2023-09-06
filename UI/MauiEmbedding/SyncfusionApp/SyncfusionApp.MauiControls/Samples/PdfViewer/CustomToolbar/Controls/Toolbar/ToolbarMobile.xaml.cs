#region Copyright Syncfusion Inc. 2001-2023.
// Copyright Syncfusion Inc. 2001-2023. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace SyncfusionApp.MauiControls.Samples.PdfViewer.SfPdfViewer;

public partial class ToolbarMobile : ToolbarView
{
	public ToolbarMobile()
	{
		InitializeComponent();
        AssignControls();
	}

    void AssignControls()
    {
        PageNumberEntry = pageNumberEntry;
        SearchButton = searchButton;
        OutlineButton = outlineButton;
        PageCountLabel = pageCountLabel;
    }
}