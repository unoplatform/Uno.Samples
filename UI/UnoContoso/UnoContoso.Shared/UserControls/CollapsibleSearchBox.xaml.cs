//  ---------------------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// 
//  The MIT License (MIT)
// 
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
// 
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
// 
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
//  ---------------------------------------------------------------------------------

using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace UnoContoso.UserControls
{
    public sealed partial class CollapsibleSearchBox : UserControl
    {
        private double RequestedWidth = 32;

        public CollapsibleSearchBox()
        {
            InitializeComponent();
        }

        #region ItemsSource

        public object ItemsSource
        {
            get { return (object)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(object), typeof(CollapsibleSearchBox), new PropertyMetadata(null, ItemsSourceChanged));

        #endregion

        private static void ItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (CollapsibleSearchBox)d;
            control.SetItemsSource();
        }

        private void SetItemsSource()
        {
            searchBox.ItemsSource = ItemsSource;
        }

        private void SearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason != AutoSuggestionBoxTextChangeReason.UserInput) return;
            Text = sender.Text;
        }

        #region Text

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(CollapsibleSearchBox), new PropertyMetadata(null, TextChanged));

        private static void TextChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var control = (CollapsibleSearchBox)dependencyObject;
            control.searchBox.Text = args.NewValue.ToString();
        }

        #endregion

        private void SearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            QueryText = args.QueryText;
        }

        #region QueryText

        public string QueryText
        {
            get { return (string)GetValue(QueryTextProperty); }
            set { SetValue(QueryTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for QueryText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty QueryTextProperty =
            DependencyProperty.Register("QueryText", typeof(string), typeof(CollapsibleSearchBox), new PropertyMetadata(null));

        #endregion


        #region CollapseWidth

        public double CollapseWidth
        {
            get { return (double)GetValue(CollapseWidthProperty); }
            set { SetValue(CollapseWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CollapseWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CollapseWidthProperty =
            DependencyProperty.Register("CollapseWidth", typeof(double), typeof(CollapsibleSearchBox), new PropertyMetadata(0.0));

        #endregion

        private void CollapsableSearchBox_Loaded(object sender, RoutedEventArgs e)
        {
            RequestedWidth = Width;
            //iOS
            SetState(Microsoft.UI.Xaml.Window.Current.Bounds.Width);
            Microsoft.UI.Xaml.Window.Current.SizeChanged += Current_SizeChanged;
            searchBox.QuerySubmitted += SearchBox_QuerySubmitted;
            searchBox.TextChanged += SearchBox_TextChanged;
        }

        private void CollapsableSearchBox_Unloaded(object sender, RoutedEventArgs e)
        {
            Microsoft.UI.Xaml.Window.Current.SizeChanged -= Current_SizeChanged;
            searchBox.QuerySubmitted -= SearchBox_QuerySubmitted;
            searchBox.TextChanged -= SearchBox_TextChanged;
        }

        private void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            SetState(e.Size.Width);
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            SetState(Microsoft.UI.Xaml.Window.Current.Bounds.Width);
            searchButton.IsChecked = false;
        }

        private void SetState(double width)
        {
            if (width <= CollapseWidth)
            {
                VisualStateManager.GoToState(this, "CollapsedState", false);
                Width = 32;
            }
            else
            {
                VisualStateManager.GoToState(this, "OpenState", false);
                Width = RequestedWidth;
            }
        }

        private void SearchButton_Checked(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "OpenState", false);
            Width = RequestedWidth;
            if (searchBox != null)
            {
                searchBox.Focus(FocusState.Programmatic);
            }
        }
    }
}
