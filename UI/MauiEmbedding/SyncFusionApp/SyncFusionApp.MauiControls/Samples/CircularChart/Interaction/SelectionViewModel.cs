#region Copyright Syncfusion Inc. 2001-2023.
// Copyright Syncfusion Inc. 2001-2023. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.ObjectModel;

namespace SyncfusionApp.MauiControls.Samples.CircularChart.SfCircularChart
{
    public class SelectionViewModel : BaseViewModel
    {
        public ObservableCollection<ChartDataModel> CircularData { get; set; }

        public SelectionViewModel()
        {
            CircularData = new ObservableCollection<ChartDataModel>()
            {
                new ChartDataModel("CHN", 17, 54, 9),
                new ChartDataModel("USA", 19, 67, 14),
                new ChartDataModel("IDN", 29, 65, 6),
                new ChartDataModel("JAP", 13, 61, 26),
                new ChartDataModel("BRZ", 24, 68, 8)
            };
        }
    }
}
