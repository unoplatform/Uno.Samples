using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Uno.UI.Runtime.WebAssembly;
using System;
using Uno.Extensions;
using System.Globalization;

namespace HtmlControls
{
    [HtmlElement("select")]
    [ContentProperty(Name = nameof(Options))]
    public partial class HtmlSelect : FrameworkElement
    {
        public ObservableCollection<HtmlOption> Options { get; } = new ObservableCollection<HtmlOption>();

        public HtmlSelect()
        {
            // Set a background to ensure pointer events are allowed
            Background = new SolidColorBrush(Colors.Transparent);

            Options.CollectionChanged += OnOptionsChanged;

            this.ExecuteJavascript("element.addEventListener(\"change\", ()=>element.dispatchEvent(new CustomEvent(\"value\", {detail: \"\"+element.selectedIndex})));");
            this.ExecuteJavascript("element.addEventListener(\"loaded\", ()=>element.dispatchEvent(new CustomEvent(\"value\", {detail: \"\"+element.selectedIndex})));");
            this.RegisterHtmlCustomEventHandler("value", OnHtmlSelectionChanged);
        }

        private void OnHtmlSelectionChanged(object sender, HtmlCustomEventArgs e)
        {
            if(int.TryParse(e.Detail, System.Globalization.NumberStyles.Integer | NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo, out var index))
            {
                SelectedIndex = index;

                if (index < 0)
                {
                    // No selection
                    SelectedValue = null;
                }
                else
                {
                    SelectedValue = Options[index].Value;
                }
            }
        }

        public static readonly DependencyProperty SelectedValueProperty = DependencyProperty.Register(
            "SelectedValue", typeof(string), typeof(HtmlSelect), new PropertyMetadata(default(string), propertyChangedCallback: OnSelectedValueChanged));

        public string SelectedValue
        {
            get => (string)GetValue(SelectedValueProperty);
            set => SetValue(SelectedValueProperty, value);
        }

        private static void OnSelectedValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
        {
            if (o is HtmlSelect select && args.NewValue is string selectedValue)
            {
                // TODO
            }
        }

        public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register(
            "SelectedIndex", typeof(int), typeof(HtmlSelect), new PropertyMetadata(default(int), propertyChangedCallback: OnSelectedIndexChanged));

        public int SelectedIndex
        {
            get => (int)GetValue(SelectedIndexProperty);
            set => SetValue(SelectedIndexProperty, value);
        }

        private static void OnSelectedIndexChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
        {
            if (o is HtmlSelect select && args.NewValue is int selectedIndex)
            {
                select.ExecuteJavascript($"element.selectedIndex={selectedIndex};");
            }
        }

        private void OnOptionsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch(e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        var index = e.NewStartingIndex;
                        foreach(var newItem in e.NewItems)
                        {
                            AddChild(newItem as UIElement, index++);
                        }
                        break;
                    }
                case NotifyCollectionChangedAction.Remove:
                    {
                        foreach (var removedItem in e.OldItems)
                        {
                            RemoveChild(removedItem as UIElement);
                        }
                        break;
                    }
                default:
                    throw new NotSupportedException($"CollectionChanged Action \"{e.Action}\" not supported yet.");
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            // Delegate measurement to Html <select> element
            return this.MeasureHtmlView(availableSize, false);
        }
    }
}
