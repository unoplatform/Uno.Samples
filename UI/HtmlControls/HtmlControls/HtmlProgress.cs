using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Uno.Extensions;
using Uno.UI.Runtime.WebAssembly;

namespace HtmlControls
{
    [HtmlElement("progress")]
    public partial class HtmlProgress : FrameworkElement
    {
        public HtmlProgress()
        {
            // Set a background to ensure pointer events are allowed
            Background = new SolidColorBrush(Colors.Transparent);
        }

        public static readonly DependencyProperty MaxProperty = DependencyProperty.Register(
            "Max", typeof(double), typeof(HtmlProgress), new PropertyMetadata(1.0d, propertyChangedCallback: OnMaxChanged));

        public double Max
        {
            get => (double)GetValue(MaxProperty);
            set => SetValue(MaxProperty, value);
        }

        private static void OnMaxChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
        {
            if (o is HtmlProgress progress && args.NewValue is double max)
            {
                progress.SetAttribute("max", max.ToStringInvariant());
            }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value", typeof(double), typeof(HtmlProgress), new PropertyMetadata(default(double), propertyChangedCallback: OnValueChanged));

        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        private static void OnValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
        {
            if (o is HtmlProgress progress && args.NewValue is double value)
            {
                progress.SetAttribute("value", value.ToStringInvariant());
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            // Delegate measurement to Html <progress> element
            return this.MeasureHtmlView(availableSize, false);
        }
    }
}