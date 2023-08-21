#region Copyright Syncfusion Inc. 2001-2023.
// Copyright Syncfusion Inc. 2001-2023. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SyncFusionApp.MauiControls.Samples.CartesianChart.SfCartesianChart
{
    public class NeumorphismViewModel : BaseViewModel
    {
        public ObservableCollection<ChartDataModel> NeumorphismColumnData { get; set; }
        public ObservableCollection<ChartDataModel> NeumorphismSpAreaData { get; set; }

        public NeumorphismViewModel()
        {
            NeumorphismColumnData = new ObservableCollection<ChartDataModel>
            {
                new ChartDataModel("Jan", 51),
                new ChartDataModel("Feb", 81),
                new ChartDataModel("Mar", 45),
                new ChartDataModel("Apr", 33),
                new ChartDataModel("May", 61),           
#if WINDOWS || MACCATALYST
                new ChartDataModel("Jun", 81),
                new ChartDataModel("Jul", 45),
                new ChartDataModel("Aug", 33),
                new ChartDataModel("Sep", 60)
#endif

           };

            NeumorphismSpAreaData = new ObservableCollection<ChartDataModel>
            {
                new ChartDataModel("Jan", 5,15),
                new ChartDataModel("Feb", 25,91),
                new ChartDataModel("Mar", 40,70),
                new ChartDataModel("Apr", 110,35),
                new ChartDataModel("May", 55,45),
                new ChartDataModel("Jun", 25,91),
                new ChartDataModel("Jul", 60,21),
            };
        }
    }
}
