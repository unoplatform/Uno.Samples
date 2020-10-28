using Microsoft.Toolkit.Uwp.Helpers;
using Prism.Commands;
using Prism.Ioc;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Uno.Extensions;
using UnoContoso.Model;
using UnoContoso.Models;
using UnoContoso.Repository;
using Windows.ApplicationModel.Email;
using Windows.UI.Core;

namespace UnoContoso.ViewModels
{
    public class OrderListViewModel : ViewModelBase
    {
        private readonly IContosoRepository _contosoRepository;

        private Customer _selectedCustomer;

        /// <summary>
        /// Gets or sets the selected customer.
        /// </summary>
        public Customer SelectedCustomer
        {
            get => _selectedCustomer;
            set => SetProperty(ref _selectedCustomer, value);
        }

        private Order _selectedOrder;
        public Order SelectedOrder
        {
            get { return _selectedOrder; }
            set 
            {
                if (SetProperty(ref _selectedOrder, value))
                {
                    // Clear out the existing customer.
                    SelectedCustomer = null;
                    if (_selectedOrder != null)
                    {
                        Task.Run(() => LoadCustomer(_selectedOrder.CustomerId));
                    }
                    RaisePropertyChanged(nameof(SelectedOrderGrandTotalFormatted));
                }

            }
        }

        /// <summary>
        /// Gets a formatted version of the selected order's grand total value.
        /// </summary>
        public string SelectedOrderGrandTotalFormatted => (SelectedOrder?.GrandTotal ?? 0).ToString("n");


        private async void LoadCustomer(Guid customerId)
        {
            var customer = await _contosoRepository.Customers.GetAsync(customerId);
            await DispatcherHelper.ExecuteOnUIThreadAsync(
                () => 
                {
                    SelectedCustomer = customer;
                });
        }

        /// <summary>
        /// Gets the unfiltered collection of all orders. 
        /// </summary>
        private IList<Order> MasterOrdersList { get; } = new List<Order>();

        /// <summary>
        /// Gets the orders to display.
        /// </summary>
        public IList<Order> Orders { get; private set; } = new ObservableCollection<Order>();

        public ICommand EditCommand { get; set; }

        public ICommand DeleteCommand { get; set; }

        public ICommand RefreshCommand { get; set; }

        public ICommand EmailCommand { get; set; }

        private string _searchBoxText;
        public string SearchBoxText
        {
            get { return _searchBoxText; }
            set { SetProperty(ref _searchBoxText, value); }
        }

        private string _queryText;
        public string QueryText
        {
            get { return _queryText; }
            set { SetProperty(ref _queryText, value); }
        }

        private IList<string> _suggestItems;

        public IList<string> SuggestItems
        {
            get { return _suggestItems; }
            set { SetProperty(ref _suggestItems, value); }
        }

        public OrderListViewModel()
        {
        }

        public OrderListViewModel(IContainerProvider containerProvider,
            IContosoRepository contosoRepository)
            : base(containerProvider)
        {
            Title = "Orders";
            _contosoRepository = contosoRepository;

            Init();
        }

        private void Init()
        {
            EditCommand = new DelegateCommand(OnEdit, () => SelectedOrder != null)
                .ObservesProperty(() => SelectedOrder);
            DeleteCommand = new DelegateCommand(OnDelete, () => SelectedOrder != null)
                .ObservesProperty(() => SelectedOrder);
            EmailCommand = new DelegateCommand(OnEmail);
            RefreshCommand = new DelegateCommand(() => LoadOrders());
            PropertyChanged += OrderListViewModel_PropertyChanged;
        }

        private async void OrderListViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case nameof(SearchBoxText):
                    SetSuggestItems(SearchBoxText);
                    break;
                case nameof(QueryText):
                    await SetOrdersAsync(QueryText);
                    break;
            }
        }

        private async Task SetOrdersAsync(string queryText)
        {
            Orders.Clear();
            if (string.IsNullOrEmpty(queryText))
            {
                Orders.AddRange(MasterOrdersList);
            }
            else
            {
                await DispatcherHelper.ExecuteOnUIThreadAsync(
                    () =>
                    {
                        List<Order> orders = GetOrders(queryText);
                        Orders.AddRange(orders);
                    });
            }
        }

        private List<Order> GetOrders(string queryText)
        {
            string[] parameters = queryText.Split(new char[] { ' ' },
                StringSplitOptions.RemoveEmptyEntries);

            var orders = MasterOrdersList
                .Where(c => parameters.Any(p =>
                    c.Address.StartsWith(p, StringComparison.OrdinalIgnoreCase) ||
                    c.CustomerName.StartsWith(p, StringComparison.OrdinalIgnoreCase) ||
                    c.InvoiceNumber.ToString().StartsWith(p, StringComparison.OrdinalIgnoreCase)))
                .OrderByDescending(c => parameters.Count(p =>
                    c.Address.StartsWith(p, StringComparison.OrdinalIgnoreCase) ||
                    c.CustomerName.StartsWith(p, StringComparison.OrdinalIgnoreCase) ||
                    c.InvoiceNumber.ToString().StartsWith(p, StringComparison.OrdinalIgnoreCase)))
                .ToList();
            return orders;
        }

        private void SetSuggestItems(string searchBoxText)
        {
            if (string.IsNullOrEmpty(searchBoxText))
            {
                Orders.AddRange(MasterOrdersList);
                SuggestItems = null;
            }
            else
            {
                var orders = GetOrders(searchBoxText);
                SuggestItems = orders
                    .Select(o => $"{o.InvoiceNumber} {o.CustomerName}")
                    .ToList();
            }
        }

        private async void OnEmail()
        {
            var emailMessage = new EmailMessage
            {
                Body = $"Dear {SelectedOrder.CustomerName},",
                Subject = "A message from Contoso about order " +
                    $"#{SelectedOrder.InvoiceNumber} placed on " +
                    $"{SelectedOrder.DatePlaced.ToString("MM/dd/yyyy")}"
            };

            if (!string.IsNullOrEmpty(SelectedCustomer.Email))
            {
                var emailRecipient = new EmailRecipient(SelectedCustomer.Email);
                emailMessage.To.Add(emailRecipient);
            }
            await EmailManager.ShowComposeNewEmailAsync(emailMessage);
        }

        private void OnDelete()
        {
            try
            {
                var deleteOrder = SelectedOrder;
                DialogService.ShowDialog("ConfirmControl",
                    new DialogParameters
                    {
                        {"title", $"Delete to Invoice # {deleteOrder.InvoiceNumber}?" },
                        {"message", $"Are you sure you want to delete Invoice # {deleteOrder.InvoiceNumber}" },
                        {"buttons", "Yes,No"}
                    }, async result => 
                    {
                        if (result.Result != ButtonResult.Yes) return;
                        await _contosoRepository.Orders.DeleteAsync(deleteOrder.Id);
                    });
                //todo : 삭제된 오더를 화면에서 삭제해야하는거 아닌가??
            }
            catch (OrderDeletionException ex)
            {
                DialogService.ShowDialog("MessageControl",
                    new DialogParameters
                    {
                        { "title", "Unable to delete order" },
                        { "message", $"There was an error when we tried to delete " +
                            $"invoice #{SelectedOrder.InvoiceNumber}:\n{ex.Message}"}
                    }, null);
            }

        }

        private void OnEdit()
        {
            NavigationService.RequestNavigate("OrderDetailView", 
                new NavigationParameters 
                {
                    {"OrderId", SelectedOrder.Id }
                });
        }

        private async void LoadOrders()
        {
            await DispatcherHelper.ExecuteOnUIThreadAsync(
                () =>
                {
                    SetBusy("LoadOrders", true);
                    Orders.Clear();
                    MasterOrdersList.Clear();
                    SelectedOrder = null;
                    SelectedCustomer = null;
                });

            var orders = await _contosoRepository.Orders.GetAsync();

            await DispatcherHelper.ExecuteOnUIThreadAsync(
                () =>
                {
                    Orders.AddRange(orders);
                    MasterOrdersList.AddRange(orders);
                    SetBusy("LoadOrders", false);
                });
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            if (Orders.Count < 1)
            {
                LoadOrders();
            }
        }
    }
}
