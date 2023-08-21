#region Copyright Syncfusion Inc. 2001-2023.
// Copyright Syncfusion Inc. 2001-2023. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using SyncFusionApp.MauiControls.Samples.Base;
using Syncfusion.Maui.Charts;

namespace SyncFusionApp.MauiControls.Samples.CartesianChart.SfCartesianChart
{
    public partial class CartesianTooltip : SampleView
    {
        public CartesianTooltip()
        {
            InitializeComponent();
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            hyperLinkLayout.IsVisible = !IsCardView;
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();
            tooltipChart.Handler?.DisconnectHandler();
        }
    }
}
