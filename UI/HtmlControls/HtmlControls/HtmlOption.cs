using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;
using Uno.UI.Runtime.WebAssembly;

namespace HtmlControls
{
    [HtmlElement("option")]
    [ContentProperty(Name = nameof(Content))]
    public partial class HtmlOption : UIElement
    {
        public static DependencyProperty ContentProperty { get; } = DependencyProperty.Register(
            "ContentProperty", typeof(string), typeof(HtmlOption), new PropertyMetadata(null, OnContentChanged));

        public string Content
        {
            get => (string)GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        private static void OnContentChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
        {
            if (o is HtmlOption option)
            {
                if (args.NewValue is string str)
                {
                    option.ClearChildren();
                    option.SetHtmlContent(str);
                }
            }
        }

        public static DependencyProperty ValueProperty { get; } = DependencyProperty.Register(
            "ValueProperty", typeof(string), typeof(HtmlOption), new PropertyMetadata(null, OnValueChanged));

        public string Value
        {
            get => (string)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        private static void OnValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
        {
            if (o is HtmlOption option)
            {
                if (args.NewValue is string str)
                {
                    option.SetHtmlAttribute("value", str);
                }
            }
        }
    }
}
