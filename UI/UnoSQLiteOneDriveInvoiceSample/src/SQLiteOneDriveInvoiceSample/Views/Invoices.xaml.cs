using SQLiteOneDriveInvoiceSample.Database.Entities;
using SQLiteOneDriveInvoiceSample.Helpers;
using SQLiteOneDriveInvoiceSample.Presentation;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SQLiteOneDriveInvoiceSample.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Invoices : Page
    {
        public Invoices()
        {
            this.InitializeComponent();
        }

        
        
        public static string PriceOfItems(ObservableCollection<ItemBlob> items, string currency) => $"{currency} {items?.Sum(item => item.Price)}";

        public static string EnumToString(Enum enumObject) => enumObject.ToString();

        public static string DateFormat(DateTime dateTime, bool isIssueDate) => isIssueDate ? $"Issued : {dateTime.ToString("d")}" : $"Due : {dateTime.ToString("d")}";

       
        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private async void ViewClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var invoice = button.DataContext as Invoice;
            var details = $"Client : {invoice.Client.Name}, {Environment.NewLine}Items : {invoice.ItemsBlob}, {Environment.NewLine}Due Date : {invoice.DueDate.ToString("d")}, {Environment.NewLine}Issued Date : {invoice.IssueDate.ToString("d")}";
            await Dialogs.GenericDialogAsync($"Invoice Details", details, "Close");
        }

        
        private void DeleteClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var invoice = button.DataContext as Invoice;
            var vm = DataContext as InvoicesVM;
            vm.DeleteEntity(invoice);
        }
    }
}
