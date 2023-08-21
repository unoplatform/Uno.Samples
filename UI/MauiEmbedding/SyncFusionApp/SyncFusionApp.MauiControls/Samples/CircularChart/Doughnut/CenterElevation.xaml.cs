#region Copyright Syncfusion Inc. 2001-2023.
// Copyright Syncfusion Inc. 2001-2023. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Microsoft.Maui.Graphics;
using SyncFusionApp.MauiControls.Samples.Base;
using Syncfusion.Maui.Charts;
using Syncfusion.Maui.Core;
using System.Collections.ObjectModel;
using System.Globalization;

namespace SyncFusionApp.MauiControls.Samples.CircularChart.SfCircularChart
{
    public partial class CenterElevation : SampleView
    {
        public CenterElevation()
        {
            InitializeComponent();
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
#if IOS
            if (IsCardView)
            {
                Chart1.WidthRequest = 350;
                Chart1.HeightRequest = 400;
                Chart1.VerticalOptions = LayoutOptions.Start;
            }
#endif
        }

    }
}

