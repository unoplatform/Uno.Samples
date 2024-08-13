using SQLiteOneDriveInvoiceSample.Database.Entities;
using SQLiteOneDriveInvoiceSample.Presentation;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SQLiteOneDriveInvoiceSample.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Clients : Page
    {
        //public ClientsVM ViewModel { get; set; }
        public Clients()
        {
            //ViewModel = new ClientsVM();
            this.InitializeComponent();
            
        }

        public static string EnumToString(Enum enumObject) => enumObject.ToString();

        
        private void ItemTemplate_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void clientsGridView_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void ViewClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
        }

        private void EditClick(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var client = button.DataContext as Client;
            var vm = DataContext as ClientsVM;
            vm.DeleteEntity(client);
        }

    }
}
