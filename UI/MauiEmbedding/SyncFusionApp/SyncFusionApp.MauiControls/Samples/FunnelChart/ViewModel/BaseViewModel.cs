#region Copyright Syncfusion Inc. 2001-2023.
// Copyright Syncfusion Inc. 2001-2023. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using SyncfusionApp.MauiControls.Samples.Base;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SyncfusionApp.MauiControls.Samples.FunnelChart.SfFunnelChart
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<Brush> PaletteBrushes { get; set; }
        public ObservableCollection<Brush> SelectionBrushes { get; set; }

        public ObservableCollection<Brush> CustomColor_1 { get; set; }
        public ObservableCollection<Brush> CustomColor_2 { get; set; }

        private bool enableAnimation = true;
        public bool EnableAnimation
        {
            get { return enableAnimation; }
            set
            {
                enableAnimation = value;
                OnPropertyChanged("EnableAnimation");
            }
        }

        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public BaseViewModel()
        {
            PaletteBrushes = new ObservableCollection<Brush>()
            {
               new SolidColorBrush(Color.FromArgb("#314A6E")),
                 new SolidColorBrush(Color.FromArgb("#48988B")),
                 new SolidColorBrush(Color.FromArgb("#5E498C")),
                 new SolidColorBrush(Color.FromArgb("#74BD6F")),
                 new SolidColorBrush(Color.FromArgb("#597FCA"))
            };

            SelectionBrushes = new ObservableCollection<Brush>()
            {
                new SolidColorBrush(Color.FromArgb("#40314A6E")),
                new SolidColorBrush(Color.FromArgb("#4048988B")),
                new SolidColorBrush(Color.FromArgb("#405E498C")),
                new SolidColorBrush(Color.FromArgb("#4074BD6F")),
                new SolidColorBrush(Color.FromArgb("#40597FCA"))
            };

            CustomColor_1 = new ObservableCollection<Brush>()
            {
                new SolidColorBrush(Color.FromArgb("#95DB9C")),
                new SolidColorBrush(Color.FromArgb("#B95375")),
                new SolidColorBrush(Color.FromArgb("#56BBAF")),
                new SolidColorBrush(Color.FromArgb("#606D7F")),
                new SolidColorBrush(Color.FromArgb("#E99941")),
                new SolidColorBrush(Color.FromArgb("#327DBE")),
                new SolidColorBrush(Color.FromArgb("#E7695A")),
                new SolidColorBrush(Color.FromArgb("#2D4552")),
                new SolidColorBrush(Color.FromArgb("#4E9B8F")),
                new SolidColorBrush(Color.FromArgb("#E9A56C")),
            };

            CustomColor_2 = new ObservableCollection<Brush>()
            {
                new SolidColorBrush(Color.FromArgb("#95DB9C")),
                new SolidColorBrush(Color.FromArgb("#3F69C5")),
                new SolidColorBrush(Color.FromArgb("#D97559")),
                new SolidColorBrush(Color.FromArgb("#606D7F")),
                new SolidColorBrush(Color.FromArgb("#E99941"))
            };
        }
    }
}
