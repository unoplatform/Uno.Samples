#region Copyright Syncfusion Inc. 2001-2023.
// Copyright Syncfusion Inc. 2001-2023. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.ObjectModel;

namespace SyncfusionApp.MauiControls.Samples.CartesianChart.SfCartesianChart
{
    public class SelectionViewModel : BaseViewModel
    {
        public ObservableCollection<ChartDataModel> SelectionData { get; set; }

        public SelectionViewModel()
        {
            DateTime date = new(2017, 3, 01);
            Random r = new();
            SelectionData = new ObservableCollection<ChartDataModel>();
            for (int i = 0; i < 20; i++)
            {
                SelectionData.Add(new ChartDataModel(date, r.Next(10, 65)));
                date = date.AddHours(1);
            }
        }
    }
}
