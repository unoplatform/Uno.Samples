using Demo.Database.Entities;
using Demo.Helpers;
using Demo.ViewModels;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Demo.Pages
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
