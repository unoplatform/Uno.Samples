using UnoContoso.ViewModels;
using Microsoft.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace UnoContoso.Views
{
    public sealed partial class CustomerListView : UserControl
    {
        public CustomerListView()
        {
            InitializeComponent();
        }

        public CustomerListViewModel ViewModel
        {
            get { return DataContext as CustomerListViewModel; }
        }

    }
}
