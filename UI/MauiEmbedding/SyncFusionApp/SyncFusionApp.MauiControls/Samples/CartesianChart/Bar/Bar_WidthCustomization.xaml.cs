#region Copyright Syncfusion Inc. 2001-2023.
// Copyright Syncfusion Inc. 2001-2023. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using SyncfusionApp.MauiControls.Samples.Base;

namespace SyncfusionApp.MauiControls.Samples.CartesianChart.SfCartesianChart
{
    public partial class Bar_WidthCustomization : SampleView
    {
        public Bar_WidthCustomization()
        {
            InitializeComponent();
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            hyperLinkLayout.IsVisible = !IsCardView;
#if IOS
            if (IsCardView)
            {
                Chart3.WidthRequest = 350;
                Chart3.HeightRequest = 400;
                Chart3.VerticalOptions = LayoutOptions.Start;
            }
#endif
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();
            Chart3.Handler?.DisconnectHandler();
        }
    }
}
