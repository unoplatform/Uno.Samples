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
    [HtmlElement("input")]
    public partial class HtmlNumberInput : FrameworkElement
    {
        public HtmlNumberInput()
        {
            // Set a background to ensure pointer events are allowed
            Background = new SolidColorBrush(Colors.Transparent);

            this.SetHtmlAttribute("type", "number");

            this.ExecuteJavascript("element.addEventListener(\"change\", ()=>element.dispatchEvent(new CustomEvent(\"value\", {detail: element.value})));");
            this.RegisterHtmlCustomEventHandler("value", OnHtmlValueChanged);
        }

        private void OnHtmlValueChanged(object sender, HtmlCustomEventArgs e)
        {
            if (long.TryParse(e.Detail, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo, out var value))
            {
                Value = value;
            }
            else
            {
                Console.Error.WriteLine($"Unable to parse {e.Detail} as a valid number.");
            }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value", typeof(long), typeof(HtmlNumberInput), new PropertyMetadata(default(long), propertyChangedCallback: OnValueChanged));

        public long Value
        {
            get => (long)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        private static void OnValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
        {
            if (o is HtmlNumberInput input && args.NewValue is long value)
            {
                input.ExecuteJavascript($"element.value={value.ToStringInvariant()};");
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            // Delegate measurement to Html <input> element
            return this.MeasureHtmlView(availableSize, false);
        }
    }
}