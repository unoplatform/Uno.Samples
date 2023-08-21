#region Copyright Syncfusion Inc. 2001-2023.
// Copyright Syncfusion Inc. 2001-2023. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using SyncFusionApp.MauiControls.Samples.Base;
using Syncfusion.Maui.Charts;

namespace SyncFusionApp.MauiControls.Samples.CartesianChart.SfCartesianChart;

public partial class StepAreaChart : SampleView
{
    int month = int.MaxValue;

    public StepAreaChart()
	{
		InitializeComponent();

        if ((BaseConfig.RunTimeDeviceLayout == SBLayout.Mobile))
        {
            xAxis.AutoScrollingDelta = 18;
            xAxis.AutoScrollingMode = ChartAutoScrollingMode.Start;
        }
    }

    private void Primary_LabelCreated(object? sender, ChartAxisLabelEventArgs e)
    {
        DateTime baseDate = new(1899, 12, 30);
        var date = baseDate.AddDays(e.Position);
        if (date.Month != month)
        {
            ChartAxisLabelStyle labelStyle = new();
            labelStyle.LabelFormat = "MMM-dd";
            labelStyle.FontAttributes = FontAttributes.Bold;
            e.LabelStyle = labelStyle;

            month = date.Month;
        }
        else
        {
            ChartAxisLabelStyle labelStyle = new();
            labelStyle.LabelFormat = "dd";
            e.LabelStyle = labelStyle;
        }
    }

    public override void OnAppearing()
    {
        base.OnAppearing();

#if IOS
            if (IsCardView)
            {
                Chart.WidthRequest = 350;
                Chart.HeightRequest = 400;
                Chart.VerticalOptions = LayoutOptions.Start;
            }
#endif

        if (!IsCardView)
        { 
            yAxis.Title = new ChartAxisTitle() { Text = "Stock Price" }; xAxis.Title = new ChartAxisTitle() { Text = "Date" };
        }
    }

    public override void OnDisappearing()
    {
        base.OnDisappearing();
        Chart.Handler?.DisconnectHandler();
    }

}