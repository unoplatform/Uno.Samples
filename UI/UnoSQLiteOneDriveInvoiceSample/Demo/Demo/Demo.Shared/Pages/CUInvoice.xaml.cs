using Demo.Database.Entities;
using Demo.ViewModels;

using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;



// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Demo.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CUInvoice : Page
    {
        public InvoiceVM ViewModel { get; set; }
        public CUInvoice()
        {
            ViewModel = new InvoiceVM(new Invoice());
            this.InitializeComponent();
            itemsList.Items.Add(new ItemBlob());
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (ViewModel == null)
            {
                ViewModel = new InvoiceVM(new Invoice());
            }
        }

        public static DateTimeOffset GetDateFromNow(double days = 0) => new DateTimeOffset(DateTime.UtcNow.AddDays(days));

        public void SetIssuedDate(DateTimeOffset? date)
        {
            if (date != null)
            {
                ViewModel.Entity.IssueDate = date.Value.DateTime;
            }

        }

        public void SetDueDate(DateTimeOffset? date)
        {
            if (date != null)
            {
                ViewModel.Entity.DueDate = date.Value.DateTime;
            }
            
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            SetIssuedDate(issuedDate.SelectedDate);
            SetDueDate(dueDate.SelectedDate);
            //ViewModel.Items = itemsList.Items
            ViewModel.SaveEntity();
            
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            itemsList.Items.Clear();
            ViewModel.Entity = new Invoice();
            itemsList.Items.Add(new ItemBlob());
        }

        private void Additem_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            itemsList.Items.Add(new ItemBlob());

        }
        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            itemsList.Items.RemoveAt(itemsList.Items.Count - 1);
        }

    }
}
