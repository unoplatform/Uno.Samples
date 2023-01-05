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
    [HtmlElement("input")]
    public partial class HtmlDatePicker : FrameworkElement
    {
        public HtmlDatePicker()
        {
            // Set a background to ensure pointer events are allowed
            Background = new SolidColorBrush(Colors.Transparent);

            this.SetHtmlAttribute("type", "date");

            this.ExecuteJavascript("element.addEventListener(\"change\", ()=>element.dispatchEvent(new CustomEvent(\"value\", {detail: element.value})));");
            this.RegisterHtmlCustomEventHandler("value", OnHtmlValueChanged);
        }

        private static readonly string[] _dateFormats = { "yyyy-MM-dd", "yy-M-d" };

        private void OnHtmlValueChanged(object sender, HtmlCustomEventArgs e)
        {
            if (DateTime.TryParseExact(e.Detail, _dateFormats, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AssumeLocal, out var date))
            {
                Date = date;
            }
            else
            {
                Console.Error.WriteLine($"Unable to parse {e.Detail} as a valid Date.");
            }
        }

        public static readonly DependencyProperty DateProperty = DependencyProperty.Register(
            "Date", typeof(DateTime), typeof(HtmlDatePicker), new PropertyMetadata(default(DateTime), propertyChangedCallback: OnDateChanged));

        public DateTime Date
        {
            get => (DateTime)GetValue(DateProperty);
            set => SetValue(DateProperty, value);
        }

        private static void OnDateChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
        {
            if (o is HtmlDatePicker picker && args.NewValue is DateTime date)
            {
                var value = date.ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo);
                picker.ExecuteJavascript($"element.value=\"{value}\";");
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            // Delegate measurement to Html <input> element
            return this.MeasureHtmlView(availableSize, false);
        }
    }
}