#region Copyright Syncfusion Inc. 2001-2023.
// Copyright Syncfusion Inc. 2001-2023. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Maui.Charts;
using System.Collections.ObjectModel;

namespace SyncFusionApp.MauiControls.Samples.CartesianChart.SfCartesianChart
{
    public class CartesianTrackballViewModel : BaseViewModel
    {
        public ObservableCollection<ChartDataModel> ChartData1 { get; set; }
        public string[] DisplayMode => new string[] { "FloatAllPoints", "NearestPoint" };

        public DateTime Minimum { get; set; }
        public DateTime Maximum { get; set; }

        public CartesianTrackballViewModel()
        {
            Minimum = new DateTime(2000, 2, 11);
            Maximum = new DateTime(2006, 2, 11);
            ChartData1 = new ObservableCollection<ChartDataModel>()
            {
                new ChartDataModel(new DateTime(2000,2,11), 15, 39,60),
                new ChartDataModel(new DateTime(2000,9,14),20,30,55),
                new ChartDataModel(new DateTime(2001,2,11),25,28,48),
                new ChartDataModel(new DateTime(2001,9,16),21,35,57),
                new ChartDataModel(new DateTime(2002,2,7),13,39,62),
                new ChartDataModel(new DateTime(2002,9,7),18,41,64),
                new ChartDataModel(new DateTime(2003,2,11),24,45,57),
                new ChartDataModel(new DateTime(2003,9,14),23,48,53),
                new ChartDataModel(new DateTime(2004,2,6),19,54,63),
                new ChartDataModel(new DateTime(2004,9,6),31,55,50),
                new ChartDataModel(new DateTime(2005,2,11),39,57,66),
                new ChartDataModel(new DateTime(2005,9,11),50,60,65),
                new ChartDataModel(new DateTime(2006,2,11),24,60,79),
            };
        }
    }
}
