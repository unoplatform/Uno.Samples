#region Copyright Syncfusion Inc. 2001-2023.
// Copyright Syncfusion Inc. 2001-2023. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using SyncfusionApp.MauiControls.Samples.Base;
using Syncfusion.Maui.Charts;
using System.Globalization;

namespace SyncfusionApp.MauiControls.Samples.CartesianChart.SfCartesianChart
{
    public partial class SeriesSelection : SampleView
    {
        public SeriesSelection()
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
                chart.WidthRequest = 350;
                chart.HeightRequest = 400;
                chart.VerticalOptions = LayoutOptions.Start;
            }
#endif
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();
            chart.Handler?.DisconnectHandler();
        }

        List<SolidColorBrush> Brushes = new List<SolidColorBrush>
        {
             new SolidColorBrush(Color.FromArgb("#314A6E")),
                 new SolidColorBrush(Color.FromArgb("#48988B")),
                 new SolidColorBrush(Color.FromArgb("#5E498C")),
                 new SolidColorBrush(Color.FromArgb("#74BD6F")),
                 new SolidColorBrush(Color.FromArgb("#597FCA"))
        };

        List<SolidColorBrush> AlphaBrushes = new List<SolidColorBrush>
        {
             new SolidColorBrush(Color.FromArgb("#40314A6E")),
                 new SolidColorBrush(Color.FromArgb("#4048988B")),
                 new SolidColorBrush(Color.FromArgb("#405E498C")),
                 new SolidColorBrush(Color.FromArgb("#4074BD6F")),
                 new SolidColorBrush(Color.FromArgb("#40597FCA"))
        };

        private void checkbox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            seriesSelection.Type = e.Value ? ChartSelectionType.Multiple : ChartSelectionType.SingleDeselect;
            SelectedIndexes.Clear();
            foreach (var series in chart.Series)
            {
                series.Fill = Brushes[chart.Series.IndexOf(series)];
            }
        }

        List<int> SelectedIndexes = new List<int>();

        private void seriesSelection_SelectionChanging(object sender, ChartSelectionChangingEventArgs e)
        {
            foreach (var index in e.NewIndexes)
            {
                if (!SelectedIndexes.Contains(index))
                    SelectedIndexes.Add(index);
            }
           
            var type = seriesSelection.Type;

            if ((type != ChartSelectionType.Multiple && e.OldIndexes.Count > 0 && e.NewIndexes.Count == 0 )|| (type == ChartSelectionType.Multiple && SelectedIndexes.Count == 0))
            {
                foreach (var series in chart.Series)
                {
                    series.Fill = Brushes[chart.Series.IndexOf(series)];
                }
            }
            else
            {
                if (type != ChartSelectionType.Multiple || (type == ChartSelectionType.Multiple && SelectedIndexes.Count == 1))
                {
                    foreach (var series in chart.Series)
                    {
                        series.Fill = AlphaBrushes[chart.Series.IndexOf(series)];
                    }
                }

                foreach (var index in e.NewIndexes)
                {
                    chart.Series[index].Fill = Brushes[index];
                }

                foreach (var index in e.OldIndexes)
                {
                    chart.Series[index].Fill = AlphaBrushes[index];
                    if (SelectedIndexes.Contains(index))
                        SelectedIndexes.Remove(index);
                }
            }
        }
    }
}