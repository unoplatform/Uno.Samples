#if ANDROID
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.Text.Util;
using Android.Widget;
#endif
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Uno.UI.Extensions;

// To learn about building custom controls see 
// https://learn.microsoft.com/windows/apps/winui/winui3/xaml-templated-controls-csharp-winui-3

namespace MigrateRenderersSample
{
    public sealed partial class RichLabel : Control
    {
        public static DependencyProperty TextProperty = DependencyProperty.Register(
    nameof(Text),
    typeof(string),
    typeof(RichLabel),
    new PropertyMetadata(default(string), new PropertyChangedCallback(OnTextChanged)));

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RichLabel? labelControl = d as RichLabel;
            string? s = e.NewValue as string;
            if (labelControl != null)
            {
                labelControl.UpdateText();
            }
        }

#if IOS
        UITextView? _textView;
#elif ANDROID
        TextView? _textView;
#else
        object? _textView;
#endif
        public RichLabel()
        {
            this.DefaultStyleKey = typeof(RichLabel);
        }

        private void UpdateText()
        {
            if (_textView != null)
            {
#if IOS
                _textView.Text = this.Text;
#elif ANDROID
                _textView.Text = this.Text;
                Linkify.AddLinks(_textView, MatchOptions.All);
#endif
            }
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Border b = (Border)GetTemplateChild("RootBorder");
#if IOS
            _textView = new UITextView();
            _textView.DataDetectorTypes = UIDataDetectorType.All;
            _textView.Editable = false;
            _textView.BackgroundColor = null;
            var wrapped = VisualTreeHelper.AdaptNative(_textView);
            wrapped.HorizontalAlignment = HorizontalAlignment.Stretch;
            wrapped.VerticalAlignment = VerticalAlignment.Stretch;
            b.Child = wrapped;
#elif ANDROID
            _textView = new Android.Widget.TextView(this.Context);
            _textView.LinksClickable = true;
            var wrapped = VisualTreeHelper.AdaptNative(_textView);
            wrapped.HorizontalAlignment = HorizontalAlignment.Stretch;
            wrapped.VerticalAlignment = VerticalAlignment.Stretch;
            b.Child = wrapped;
#else
            _textView = "unused";
#endif

            UpdateText();
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
    }
}
