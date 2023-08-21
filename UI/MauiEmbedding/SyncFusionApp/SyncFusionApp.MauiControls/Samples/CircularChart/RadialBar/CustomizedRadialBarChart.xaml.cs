#region Copyright Syncfusion Inc. 2001-2023.
// Copyright Syncfusion Inc. 2001-2023. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using SyncFusionApp.MauiControls.Samples.Base;
using Syncfusion.Maui.Core;
using System.Collections;
using System.Collections.ObjectModel;
using System.Globalization;

namespace SyncFusionApp.MauiControls.Samples.CircularChart.SfCircularChart
{
    public partial class CustomizedRadialBarChart : SampleView
    {
        public CustomizedRadialBarChart()
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
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();
            chart.Handler?.DisconnectHandler();
        }
    }

    public class IndexToItemSourceConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is LegendItem legendItem)
            {
                List<object?> collection = new List<object?>();
                collection.Add(legendItem.Item);
                return collection;
            }

            return null;
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}

