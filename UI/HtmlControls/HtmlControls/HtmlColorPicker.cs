using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using Windows.Foundation;
using Microsoft.UI;
using Windows.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Uno.Extensions;
using Uno.UI.Runtime.WebAssembly;

namespace HtmlControls
{
    [HtmlElement("input")]
    public partial class HtmlColorPicker : FrameworkElement
    {
        public HtmlColorPicker()
        {
            // Set a background to ensure pointer events are allowed
            Background = new SolidColorBrush(Colors.Transparent);

            this.SetHtmlAttribute("type", "Color");

            this.ExecuteJavascript("element.addEventListener(\"change\", ()=>element.dispatchEvent(new CustomEvent(\"value\", {detail: element.value})));");
            this.RegisterHtmlCustomEventHandler("value", OnHtmlValueChanged);
        }

        private static readonly string[] _ColorFormats = { "yyyy-MM-dd", "yy-M-d" };

        private void OnHtmlValueChanged(object sender, HtmlCustomEventArgs e)
        {
            if (TryParseColor(e.Detail, out var color))
            {
                Color = color;
            }
            else
            {
                Console.Error.WriteLine($"Unable to parse {e.Detail} as a valid Color.");
            }
        }

        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
            "Color", typeof(Color), typeof(HtmlColorPicker), new PropertyMetadata(default(Color), propertyChangedCallback: OnColorChanged));

        public Color Color
        {
            get => (Color)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        private static void OnColorChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
        {
            if (o is HtmlColorPicker picker && args.NewValue is Color color)
            {
                var rgb = $"#{color.R:x2}{color.G:x2}{color.B:x2}";
                picker.ExecuteJavascript($"element.value=\"{rgb}\";");
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            // Delegate measurement to Html <input> element
            return this.MeasureHtmlView(availableSize, false);
        }

        private static bool TryParseColor(string str, out Color color)
        {
            if (str.Length != 7)
            {
                color = default;
                return false;
            }

            var r = byte.Parse(str.Substring(1, 2), NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo);
            var g = byte.Parse(str.Substring(3, 2), NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo);
            var b = byte.Parse(str.Substring(5, 2), NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo);

            color = Color.FromArgb(255, r, g, b);
            return true;
        }
    }
}