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
public partial class MultipleColorBubbleSeries : SampleView
{
	public MultipleColorBubbleSeries()
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
                Chart2.WidthRequest = 350;
                Chart2.HeightRequest = 400;
                Chart2.VerticalOptions = LayoutOptions.Start;
            }
#endif
        if (!IsCardView)
        {
            Chart2.Title = (Label)layout.Resources["title"];
            yAxis.Title = new ChartAxisTitle() { Text = "Gross Amount in Billions" }; xAxis.Title = new ChartAxisTitle() { Text = "Movie Budget" };
        }
    }

    public override void OnDisappearing()
    {
        base.OnDisappearing();
        Chart2.Handler?.DisconnectHandler();
    }

    private void yAxis_LabelCreated(object sender, ChartAxisLabelEventArgs e)
    {
        double position = e.Position;
        if (position > 0 && position <= 10000)
        {
            string text = (position / 1000).ToString();
            e.Label = $"{text}B";
        }
        else
        {
            e.Label = $"{position}B";
        }
    }
}