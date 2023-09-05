#region Copyright Syncfusion Inc. 2001-2023.
// Copyright Syncfusion Inc. 2001-2023. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using SyncfusionApp.MauiControls.Samples.Base;
using Syncfusion.Maui.PdfViewer;
using System.Globalization;
using System.Resources;
namespace SyncfusionApp.MauiControls.Samples.PdfViewer.SfPdfViewer;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class RightToLeft : SampleView
{
    CultureInfo initialCulture;
    public RightToLeft()
    {
        InitializeComponent();

        // Backup the device's current culture before setting the new culture to demonstrate RTL.
        initialCulture = CultureInfo.CurrentUICulture;
        
        //Set the device's culture to Arabic to demonstrate RTL.
        CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("ar-AE");

		string basePath = "SyncfusionApp.MauiControls.Samples.Resources";
		if (BaseConfig.IsIndividualSB)
			basePath = "SyncfusionApp.MauiControls.Samples.PdfViewer.Samples.PdfViewer.RightToLeft.Localization";
        SfPdfViewerResources.ResourceManager = new ResourceManager($"{basePath}.SfPdfViewer", Application.Current!.GetType().Assembly);
    }

    public override void OnDisappearing()
    {
        base.OnDisappearing();

        //Restore the device's original culture while navigating out of the RightToLeft sample. 
        CultureInfo.DefaultThreadCurrentUICulture = initialCulture;
    }
}