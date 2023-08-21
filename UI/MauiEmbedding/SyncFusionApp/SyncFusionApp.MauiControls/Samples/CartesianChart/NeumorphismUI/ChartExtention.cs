#region Copyright Syncfusion Inc. 2001-2023.
// Copyright Syncfusion Inc. 2001-2023. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using SyncFusionApp.MauiControls.Samples.Base.CustomView;
using Syncfusion.Maui.Charts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncFusionApp.MauiControls.Samples.CartesianChart.SfCartesianChart
{
    public class SfNeumorphismColumnSeries : ColumnSeries
    {
        public SfNeumorphismColumnSeries()
        {
            Drawable = new SfNeumorphismDrawer();
        }

        public SfNeumorphismDrawer Drawable
        {
            get { return (SfNeumorphismDrawer)GetValue(DrawableProperty); }
            set { SetValue(DrawableProperty, value); }
        }

        public static readonly BindableProperty DrawableProperty =
            BindableProperty.Create(nameof(Drawable), typeof(SfNeumorphismDrawer), typeof(SfNeumorphismColumnSeries), defaultValue: null, propertyChanged: OnDrawablePropertyChanged);


        protected override ChartSegment CreateSegment()
        {
            return new SfNeumorphismColumnSegment(Drawable);
        }

        protected static void OnDrawablePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {

        }

    }

    public class SfNeumorphismColumnSegment : ColumnSegment
    {

        private SfNeumorphismDrawer Drawable;

        public SfNeumorphismColumnSegment(SfNeumorphismDrawer drawable)
        {
            Drawable = drawable;
        }

        protected override void Draw(ICanvas canvas)
        {
            if (Series is ColumnSeries series && series.ActualYAxis is NumericalAxis yAxis)
            {
                var top = yAxis.ValueToPoint(Convert.ToDouble(yAxis.Maximum ?? double.NaN));

                var trackRect = new RectF() { Left = Left, Top = top, Right = Right, Bottom = Bottom };

                Drawable.Draw(canvas, trackRect);
            }
        }

    }

}
