using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using Windows.Foundation;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Uno.Extensions;
using Uno.UI.Runtime.WebAssembly;

namespace HtmlControls
{
    [HtmlElement("meter")]
    public partial class HtmlMeter : FrameworkElement
    {
        public static readonly DependencyProperty MinProperty = DependencyProperty.Register(
            "Min", typeof(double), typeof(HtmlMeter), new PropertyMetadata(0d, propertyChangedCallback: OnMinChanged));

        public double Min
        {
            get => (double)GetValue(MinProperty);
            set => SetValue(MinProperty, value);
        }

        private static void OnMinChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
        {
            if (o is HtmlMeter meter && args.NewValue is double min)
            {
                meter.SetHtmlAttribute("min", min.ToString());
            }
        }

        public static readonly DependencyProperty MaxProperty = DependencyProperty.Register(
            "Max", typeof(double), typeof(HtmlMeter), new PropertyMetadata(1.0d, propertyChangedCallback: OnMaxChanged));

        public double Max
        {
            get => (double)GetValue(MaxProperty);
            set => SetValue(MaxProperty, value);
        }

        private static void OnMaxChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
        {
            if (o is HtmlMeter meter && args.NewValue is double max)
            {
                meter.SetHtmlAttribute("max", max.ToString());
            }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value", typeof(double), typeof(HtmlMeter), new PropertyMetadata(default(double), propertyChangedCallback: OnValueChanged));

        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        private static void OnValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
        {
            if (o is HtmlMeter meter && args.NewValue is double value)
            {
                meter.SetHtmlAttribute("value", value.ToString());
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            // Delegate measurement to Html <meter> element
            return this.MeasureHtmlView(availableSize, false);
        }
    }
}