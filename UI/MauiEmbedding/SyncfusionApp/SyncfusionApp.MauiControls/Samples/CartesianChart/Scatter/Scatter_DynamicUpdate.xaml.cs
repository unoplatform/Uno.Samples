#region Copyright Syncfusion Inc. 2001-2023.
// Copyright Syncfusion Inc. 2001-2023. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using SyncfusionApp.MauiControls.Samples.Base;
using Syncfusion.Maui.Charts;

namespace SyncfusionApp.MauiControls.Samples.CartesianChart.SfCartesianChart
{
    public partial class Scatter_DynamicUpdate : SampleView
    {
        public Scatter_DynamicUpdate()
        {
            InitializeComponent();
            if (!(BaseConfig.RunTimeDeviceLayout == SBLayout.Mobile))
                viewModel.StartTimer();
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            if (BaseConfig.RunTimeDeviceLayout == SBLayout.Mobile)
            {
                viewModel.StopTimer();
                viewModel.StartTimer();
            }
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();
            if (viewModel != null)
            {
                viewModel.StopTimer();
            }

            Chart1.Handler?.DisconnectHandler();
        }
    }
}
