#region Copyright Syncfusion Inc. 2001-2023.
// Copyright Syncfusion Inc. 2001-2023. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.ObjectModel;
using Microsoft.Maui.Dispatching;

namespace SyncFusionApp.MauiControls.Samples.CartesianChart.SfCartesianChart
{
    internal class DynamicAnimationViewModel : BaseViewModel
    {
        private ObservableCollection<ChartDataModel> motionAnimation = new ObservableCollection<ChartDataModel>();
        private ObservableCollection<ChartDataModel> dynamicBubbleMotionAnimation = new ObservableCollection<ChartDataModel>();
        private ObservableCollection<ChartDataModel> dynamicRangeColumnMotionAnimation = new ObservableCollection<ChartDataModel>();
        public ObservableCollection<ChartDataModel> MotionAnimation
        {
            get { return motionAnimation; }
            set
            {
                motionAnimation = value;
                OnPropertyChanged("MotionAnimation");
            }
        }

        public ObservableCollection<ChartDataModel> DynamicBubbleMotionAnimation
        {
            get { return dynamicBubbleMotionAnimation; }
            set
            {
                dynamicBubbleMotionAnimation = value;
                OnPropertyChanged("DynamicBubbleMotionAnimation");
            }
        }

        public ObservableCollection<ChartDataModel> DynamicRangeColumnMotionAnimation
        {
            get { return dynamicRangeColumnMotionAnimation; }
            set
            {
                dynamicRangeColumnMotionAnimation = value;
                OnPropertyChanged("DynamicRangeColumnMotionAnimation");
            }
        }

        private bool canStopTimer;

        public DynamicAnimationViewModel()
        {
            var r = new Random();
            MotionAnimation = new ObservableCollection<ChartDataModel>();
            DynamicBubbleMotionAnimation = new ObservableCollection<ChartDataModel>();
            DynamicRangeColumnMotionAnimation = new ObservableCollection<ChartDataModel>();

            for (int i = 0; i < 7; i++)
            {
                MotionAnimation.Add(new ChartDataModel(i, r.Next(5, 90)));
            }

            for (int i = 0; i <= 7; i++)
            {
                DynamicBubbleMotionAnimation.Add(new ChartDataModel(i + 1, r.Next(15, 90), r.Next(0, 20)));
            }

            for (int i = 0; i < 6; i++)
            {
                DynamicRangeColumnMotionAnimation.Add(new ChartDataModel(i + 1, r.Next(5, 45), r.Next(46,95)));
            }
        }

        public void StopTimer()
        {
            canStopTimer = true;
        }

        public async void StartTimer()
        {
            await Task.Delay(500);
            if (Application.Current != null)
                Application.Current.Dispatcher.StartTimer(new TimeSpan(0, 0, 0, 2, 500), UpdateData);

            canStopTimer = false;
        }

        private bool UpdateData()
        {
            if (canStopTimer) return false;

            var r = new Random();
            var data = new ObservableCollection<ChartDataModel>();
            var dataBubble = new ObservableCollection<ChartDataModel>();
            var rangeColumnData = new ObservableCollection<ChartDataModel>();
            for (int i = 0; i < 7; i++)
            {
                data.Add(new ChartDataModel(i, r.Next(5, 90)));
            }

            for (int i = 0; i <= 7; i++)
            {
                dataBubble.Add(new ChartDataModel(i + 1, r.Next(5, 90), r.Next(0, 20)));
            }

            for (int i = 0; i < 6; i++)
            {
                rangeColumnData.Add(new ChartDataModel(i + 1, r.Next(5, 45), r.Next(46, 95)));
            }


            MotionAnimation = data;
            DynamicBubbleMotionAnimation = dataBubble;
            DynamicRangeColumnMotionAnimation = rangeColumnData;

            return true;
        }
    }
}
