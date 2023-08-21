#region Copyright Syncfusion Inc. 2001-2023.
// Copyright Syncfusion Inc. 2001-2023. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.ObjectModel;

namespace SyncFusionApp.MauiControls.Samples.CartesianChart.SfCartesianChart
{
    public class CategoryAxisViewModel : BaseViewModel
    {
        public ObservableCollection<ChartDataModel> DataCollection1 { get; set; }

        public CategoryAxisViewModel()
        {
            DataCollection1 = new ObservableCollection<ChartDataModel>()
            {
                new ChartDataModel("Korea",39),
                new ChartDataModel("India",20),
                new ChartDataModel("Africa",  61),
                new ChartDataModel("China",65),
                new ChartDataModel("France",45),
                new ChartDataModel("Saudi", 10),
                //new ChartDataModel("London", 16),
                //new ChartDataModel("Mexico",31)
            };
        }
    }
}
