#if __WASM__
using System;
using Uno.Foundation;
using Uno.UI.Runtime.WebAssembly;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endif

namespace kahua.host.uno.control.htmlView
{
#if __WASM__
    
    public partial class WasmHtmlControl : Control
    {
        public static readonly DependencyProperty HtmlContentProperty =
            DependencyProperty.Register(nameof(HtmlContent), typeof(string), typeof(WasmHtmlControl), new PropertyMetadata(null, OnHtmlContentPropertyChanged));

        public string HtmlContent
        {
            get => (string)GetValue(HtmlContentProperty);
            set => SetValue(HtmlContentProperty, value);
        }

        public static readonly DependencyProperty JsContentProperty =
            DependencyProperty.Register(nameof(JsContent), typeof(string), typeof(WasmHtmlControl), new PropertyMetadata(null, OnJsContentPropertyChanged));

        public string JsContent
        {
            get => (string)GetValue(JsContentProperty);
            set => SetValue(JsContentProperty, value);
        }

        public WasmHtmlControl()
        {
            Loaded += WasmHtmlControl_Loaded;

            Background = new SolidColorBrush(Colors.Transparent);
            this.SetHtmlAttribute("frameborder", "0");
        }

        private void WasmHtmlControl_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateContent();
        }

        private static void OnHtmlContentPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            if (source is WasmHtmlControl control && control.IsLoaded)
                control.UpdateContent();
        }

        private static void OnJsContentPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            if (source is WasmHtmlControl control && control.IsLoaded)
                control.UpdateContent();
        }

        private void UpdateContent()
        {
            if (!string.IsNullOrEmpty(HtmlContent))
            {
                Console.WriteLine($"WasmHtmlControl.HtmlContent = {HtmlContent}");
                this.SetHtmlContent(HtmlContent);
                this.SetCssStyle("pointer-events", "all");
            }
            if (!string.IsNullOrEmpty(JsContent))
            {
                Console.WriteLine($"WasmHtmlControl.JsContent = {JsContent}");
                this.ExecuteJavascript(JsContent);
            }
        }
    }
#endif
}