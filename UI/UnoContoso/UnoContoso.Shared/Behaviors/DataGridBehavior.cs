using Microsoft.Toolkit.Uwp.UI.Controls;
using Microsoft.Xaml.Interactivity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using UnoContoso.Helpers;
using UnoContoso.Model;
using UnoContoso.Models;
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
            switch(AssociatedObject.ItemsSource)
            {
                case IList<CustomerWrapper> customers:
                    AssociatedObject.Sort(e.Column, customers.Sort);
                    break;
                case IList<Order> orders:
                    AssociatedObject.Sort(e.Column, orders.Sort);
                    break;
            }
        }

        private void AssociatedObject_RightTapped(object sender, Windows.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            var selectedElement = e.OriginalSource as FrameworkElement;
            switch(selectedElement.DataContext)
            {
                case Order order:
                    AssociatedObject.SelectedItem = order;
                    break;
                case CustomerWrapper customer:
                    AssociatedObject.SelectedItem = customer;
                    break;
                default:
                    e.Handled = true;
                    break;
            }
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
