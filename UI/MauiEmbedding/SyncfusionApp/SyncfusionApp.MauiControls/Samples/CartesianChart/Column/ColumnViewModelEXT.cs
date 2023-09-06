#region Copyright Syncfusion Inc. 2001-2023.
// Copyright Syncfusion Inc. 2001-2023. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.ObjectModel;

namespace SyncfusionApp.MauiControls.Samples.CartesianChart.SfCartesianChart
{
    public class ColumnViewModelEXT : BaseViewModel
    {
        public ObservableCollection<ChartDataModel> ColumnData { get; set; }

        public ColumnViewModelEXT()
        {
            EnableAnimation = true;
            ColumnData = new ObservableCollection<ChartDataModel>()
            {
                //new ChartDataModel("HP Inc", 12.54),
                //new ChartDataModel("Lenovo", 13.46),
                new ChartDataModel("Apple", 4.56),
                new ChartDataModel("Dell" ,9.18),
                new ChartDataModel("ASUS", 5.29),
                //new ChartDataModel("Like", 38000),
                //new ChartDataModel("View", 65000),
                //new ChartDataModel("Share", 25900),
            };
        }
    }
}
