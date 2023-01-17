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
    public partial class HtmlTimePicker : FrameworkElement
    {
        public HtmlTimePicker()
        {
            // Set a background to ensure pointer events are allowed
            Background = new SolidColorBrush(Colors.Transparent);

            this.SetHtmlAttribute("type", "time");

            this.ExecuteJavascript("element.addEventListener(\"change\", ()=>element.dispatchEvent(new CustomEvent(\"value\", {detail: element.value})));");
            this.RegisterHtmlCustomEventHandler("value", OnHtmlValueChanged);
        }

        private static readonly string[] _timeFormats = { "hh\\:mm", "hh\\:mm\\:ss" };

        private void OnHtmlValueChanged(object sender, HtmlCustomEventArgs e)
        {
            if (TimeSpan.TryParseExact(e.Detail, _timeFormats, DateTimeFormatInfo.InvariantInfo, TimeSpanStyles.None, out var time))
            {
                Time = time;
            }
            else
            {
                Console.Error.WriteLine($"Unable to parse {e.Detail} as a valid Time.");
            }
        }

        public static readonly DependencyProperty TimeProperty = DependencyProperty.Register(
            "Time", typeof(TimeSpan), typeof(HtmlTimePicker), new PropertyMetadata(default(TimeSpan), propertyChangedCallback: OnTimeChanged));

        public TimeSpan Time
        {
            get => (TimeSpan)GetValue(TimeProperty);
            set => SetValue(TimeProperty, value);
        }

        private static void OnTimeChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
        {
            if (o is HtmlTimePicker picker && args.NewValue is TimeSpan time)
            {
                var value = time.ToString("hh\\:mm\\:ss", DateTimeFormatInfo.InvariantInfo);
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
