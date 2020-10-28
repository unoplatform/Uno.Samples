using Microsoft.Toolkit.Uwp.UI.Controls;
using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using UnoContoso.Helpers;
using UnoContoso.Model;
using Windows.UI.Xaml;

namespace UnoContoso.Behaviors
{
    public class DataGridBehavior : Behavior<DataGrid>
    {
        protected override void OnAttached()
        {
            AssociatedObject.KeyDown += AssociatedObject_KeyDown;
            AssociatedObject.RightTapped += AssociatedObject_RightTapped;
            AssociatedObject.Sorting += AssociatedObject_Sorting;
        }

        private void AssociatedObject_Sorting(object sender, DataGridColumnEventArgs e)
        {
            var datas = AssociatedObject.ItemsSource as ObservableCollection<object>;
            AssociatedObject.Sort(e.Column, datas.Sort);
        }

        private void AssociatedObject_RightTapped(object sender, Windows.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            AssociatedObject.SelectedItem = (e.OriginalSource as FrameworkElement).DataContext;
        }

        private void AssociatedObject_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            
        }

        protected override void OnDetaching()
        {
            AssociatedObject.KeyDown -= AssociatedObject_KeyDown;
            AssociatedObject.RightTapped -= AssociatedObject_RightTapped;
            AssociatedObject.Sorting -= AssociatedObject_Sorting;
        }
    }
}
