#region Copyright Syncfusion Inc. 2001-2023.
// Copyright Syncfusion Inc. 2001-2023. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using SyncfusionApp.MauiControls.Samples.Base;
using Syncfusion.Maui.Charts;
using System.Windows.Input;

namespace SyncfusionApp.MauiControls.Samples.CartesianChart.SfCartesianChart
{
    public partial class StackedBar: SampleView
    {
        public StackedBar()
        {
            InitializeComponent();
        }
        public override void OnAppearing()
        {
            base.OnAppearing();
#if IOS
            if (IsCardView)
            {
                chart.WidthRequest = 350;
                chart.HeightRequest = 400;
                chart.VerticalOptions = LayoutOptions.Start;
            }
#endif
            //hyperLinkLayout.IsVisible = !IsCardView;
            if (!IsCardView)
            {
                chart.Title = (Label)layout.Resources["title"]; ;
                yAxis.Title = new ChartAxisTitle() { Text = "Profit" };
                xAxis.Title = new ChartAxisTitle() { Text = "Product" };
            }
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();
            chart.Handler?.DisconnectHandler();
        }
    }
}
