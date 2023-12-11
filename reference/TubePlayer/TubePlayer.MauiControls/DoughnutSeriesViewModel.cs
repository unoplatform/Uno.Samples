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
    public class DoughnutSeriesViewModel : BaseViewModel
    {
        public ObservableCollection<ChartDataModel> DoughnutSeriesData { get; set; }
        public ObservableCollection<ChartDataModel> SemiCircularData { get; set; }
        public ObservableCollection<ChartDataModel> CenterElevationData { get; set; }
        public ObservableCollection<ChartDataModel> GroupToData { get; set; }

        private int selectedIndex = 1;        
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                selectedIndex = value;
                UpdateIndex(value);
                base.OnPropertyChanged("SelectedIndex");
            }
        }
        private string name = "";
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                base.OnPropertyChanged("Name");
            }
        }
        private int value1;
        public int Value
        {
            get { return value1; }
            set
            {
                value1 = value;
                base.OnPropertyChanged("Value");
            }
        }

        private int total = 357580;
        public int Total
        {
            get { return total; }
            set
            {
                total = value;
            }
        }

        private void UpdateIndex(int value)
        {
            if (value >= 0 && value < DoughnutSeriesData.Count)
            {
                var model = DoughnutSeriesData[value];
                if(model != null && model.Name != null)
                {
                    Name = model.Name;
                    double sum = DoughnutSeriesData.Sum(item => item.Value);
                    double SelectedItemsPercentage = model.Value / sum * 100;
                    SelectedItemsPercentage = Math.Floor(SelectedItemsPercentage * 100) / 100;
                    Value = (int)SelectedItemsPercentage;
                }               
            }
        }

        public DoughnutSeriesViewModel()
        {
            DoughnutSeriesData = new ObservableCollection<ChartDataModel>
            {
                new ChartDataModel("Labor", 10),
                new ChartDataModel("Legal", 8),
                new ChartDataModel("Production", 7),
                new ChartDataModel("License", 5),
                new ChartDataModel("Facilities", 10),
                new ChartDataModel("Taxes", 6),
                new ChartDataModel("Insurance", 18)
           };

            SemiCircularData = new ObservableCollection<ChartDataModel>
            {
                new ChartDataModel("Product A", 750),
                new ChartDataModel("Product B", 463),
                new ChartDataModel("Product C", 389),
                new ChartDataModel("Product D", 697),
                new ChartDataModel("Product E", 251)
            };

            CenterElevationData = new ObservableCollection<ChartDataModel>
            {
                new ChartDataModel("Brazil",8),
                new ChartDataModel("China",12),
                new ChartDataModel("Others",30),
                new ChartDataModel("USA",50),
            };

            GroupToData = new ObservableCollection<ChartDataModel>
            {
                new ChartDataModel("US",51.30,0.39),
                new ChartDataModel("China",20.90,0.16),
                new ChartDataModel("Japan",11.00,0.08),
                new ChartDataModel("France",4.40,0.03),
                new ChartDataModel("UK",4.30,0.03),
                new ChartDataModel ("Canada",4.00,0.03),
                new ChartDataModel("Germany",3.70,0.03),
                new ChartDataModel("Italy",2.90,0.02),
                new ChartDataModel("KY",2.70,0.02),
                new ChartDataModel("Brazil",2.40,0.02),
                new ChartDataModel("South Korea",2.20,0.02),
                new ChartDataModel("Australia",2.20,0.02),
                new ChartDataModel("Netherlands",1.90,0.01),
                new ChartDataModel("Spain",1.90,0.01),
                new ChartDataModel("India",1.30,0.01),
                new ChartDataModel("Ireland",1.00,0.01),
                new ChartDataModel("Mexico",1.00,0.01),
                new ChartDataModel("Luxembourg",0.90,0.01),
            };
        }
    }
}
