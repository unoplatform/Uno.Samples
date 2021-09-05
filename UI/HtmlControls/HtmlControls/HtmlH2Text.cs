using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Uno.UI.Runtime.WebAssembly;

namespace HtmlControls
{
    [HtmlElement("h2")]
    public partial class HtmlH2Text : FrameworkElement
    {
        public HtmlH2Text()
        {
            // Set a background to ensure pointer events are allowed
            Background = new SolidColorBrush(Colors.Transparent);

            // Avoid flicking while we're measuring the element
            this.SetCssStyle("overflow", "hidden");
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(HtmlH2Text), new PropertyMetadata("", propertyChangedCallback: OnTextChanged));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        private static void OnTextChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
        {
            if (o is HtmlH2Text element && args.NewValue is string text)
            {
                element.SetHtmlContent(text);
                element.InvalidateMeasure();
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            // Delegate measurement to Html <progress> element
            return this.MeasureHtmlView(availableSize, false);
        }
    }


}
