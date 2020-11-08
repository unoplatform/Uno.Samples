using Microsoft.Toolkit.Uwp.Helpers;
using Prism.Commands;
using Prism.Ioc;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Uno;
using Uno.Extensions;
using UnoContoso.EventArgs;
using UnoContoso.Events;
using UnoContoso.Helpers;
using UnoContoso.Model;
using UnoContoso.Models;
using UnoContoso.Models.Consts;
using UnoContoso.Repository;
using Windows.UI.Core;

namespace UnoContoso.ViewModels
{
    public class CustomerListViewModel : ViewModelBase
    {
        private readonly IContosoRepository _contosoRepository;

        private IList<CustomerWrapper> _allCustomers;

        private IList<CustomerWrapper> _customers;

        public IList<CustomerWrapper> Customers
        {
            get => _customers;
            set => SetProperty(ref _customers, value);
        }

        private CustomerWrapper _selectedCustomer;

        public CustomerWrapper SelectedCustomer
        {
            get { return _selectedCustomer; }
            set { SetProperty(ref _selectedCustomer, value); }
        }

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

        public ICommand ViewDetailCommand { get; set; }

        public ICommand AddOrderCommand { get; set; }

        public ICommand NewCustomerCommand { get; set; }

        public ICommand SyncCommand { get; set; }

        public CustomerListViewModel()
        {
        }

        public CustomerListViewModel(IContainerProvider containerProvider,
            IContosoRepository contosoRepository)
            : base(containerProvider)
        {
            _contosoRepository = contosoRepository;

            Init();
        }

        private void Init()
        {
            Title = "Customer";
            _allCustomers = new List<CustomerWrapper>();
            Customers = new ObservableCollection<CustomerWrapper>();

            ViewDetailCommand = new DelegateCommand(OnViewDetail,
                () => SelectedCustomer != null)
                .ObservesProperty(() => SelectedCustomer);
            AddOrderCommand = new DelegateCommand(OnAddOrder,
                () => SelectedCustomer != null)
                .ObservesProperty(() => SelectedCustomer);
            NewCustomerCommand = new DelegateCommand(OnNewCustomer);
            SyncCommand = new DelegateCommand(OnSync);

            EventAggregator.GetEvent<CustomerEvent>()
                .Subscribe(ReceivedCustomerEvnet, false);

            PropertyChanged += CustomerListViewModel_PropertyChanged;
        }

        private void ReceivedCustomerEvnet(CustomerEventArgs obj)
        {
            switch (obj.Changes)
            {
                case Enums.EntityChanges.None:
                    break;
                case Enums.EntityChanges.Changed:
                    {
                        var customer = _allCustomers
                            .FirstOrDefault(c => c.Model.Id == obj.Customer.Id);
                        if (customer == null) return;
                        customer.Model = obj.Customer;
                        customer.UpdateProperty();
                    }
                    break;
                case Enums.EntityChanges.Added:
                    {
                        var customer = new CustomerWrapper(_contosoRepository, obj.Customer);
                        _allCustomers.Add(customer);
                    }
                    break;
                case Enums.EntityChanges.Deleted:
                    {
                        var customer = _allCustomers
                            .FirstOrDefault(c => c.Model.Id == obj.Customer.Id);
                        if (customer == null) return;
                        _allCustomers.Remove(customer);
                    }
                    break;
            }
        }

        public override void Destroy()
        {
            _allCustomers?.Clear();
            Customers?.Clear();
        }

        private async void CustomerListViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(SearchBoxText):
                    SetSuggestItems(SearchBoxText);
                    break;
                case nameof(QueryText):
                    await SetCustomersAsync(QueryText);
                    break;
            }
        }

        private async Task SetCustomersAsync(string queryText)
        {
            Customers.Clear();
            if (string.IsNullOrEmpty(queryText))
            {
                Customers.AddRange(_allCustomers);
            }
            else
            {
                await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                {
                    List<CustomerWrapper> customers = GetCustomers(queryText);
                    Customers.AddRange(customers);
                });
            }
        }

        private List<CustomerWrapper> GetCustomers(string queryText)
        {
            Debug.WriteLine($"queryText : {queryText}");

            var customers = _allCustomers
                .Where(c =>
                    c.Address.StartsWith(queryText, StringComparison.OrdinalIgnoreCase) ||
                    c.FirstName.StartsWith(queryText, StringComparison.OrdinalIgnoreCase) ||
                    c.LastName.StartsWith(queryText, StringComparison.OrdinalIgnoreCase) ||
                    c.ToString().StartsWith(queryText, StringComparison.OrdinalIgnoreCase) ||
                    c.Email.StartsWith(queryText, StringComparison.OrdinalIgnoreCase) ||
                    c.Phone.StartsWith(queryText, StringComparison.OrdinalIgnoreCase) ||
                    c.Company.StartsWith(queryText, StringComparison.OrdinalIgnoreCase))
                .ToList();
            return customers;
        }

        private void SetSuggestItems(string searchBoxText)
        {
            if (string.IsNullOrEmpty(searchBoxText))
            {
                Customers.Clear();
                Customers.AddRange(_allCustomers);
                SuggestItems = null;
            }
            else
            {
                var customers = GetCustomers(searchBoxText);
                SuggestItems = customers
                    .Select(c => $"{c.FirstName} {c.LastName}")
                    .ToList();
            }
        }

        private void OnSync()
        {
            Task.Run(async () =>
            {
                SetBusy("Sync", true);
                foreach (var modifiedCustomer in Customers?
                    .Where(c => c.IsModified)
                    .Select(c => c.Model))
                {
                    await _contosoRepository.Customers.UpsertAsync(modifiedCustomer);
                }
                SetBusy("Sync", false);
            });
        }

        private void OnNewCustomer()
        {
            NavigationService.RequestNavigate("CustomerDetailView");
        }

        private void OnAddOrder()
        {
            if (SelectedCustomer == null) return;

            NavigationService.RequestNavigate("OrderDetailView",
                new NavigationParameters
                {
                    {"CustomerId", SelectedCustomer.Model.Id },
                    {"NewOrder", true }
                });
        }

        private void OnViewDetail()
        {
            NavigationService.RequestNavigate("CustomerDetailView",
                new NavigationParameters
                {
                    {"CustomerId", SelectedCustomer?.Model.Id }
                });
        }

        public async Task<IList<CustomerWrapper>> GetCustomerListAsync()
        {
            var customers = await _contosoRepository.Customers.GetAsync();
            if (customers == null
                || customers.Any() == false) return null;

            Customers?.Clear();
            var custs = from c in customers
                        select new CustomerWrapper(_contosoRepository, c);
            return new ObservableCollection<CustomerWrapper>(custs);
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            if (_allCustomers?.Any() == false)
            {
                LoadCustomers();
            }
        }

        private async void LoadCustomers()
        {
            SetBusy("FirstLoading", true);
            _allCustomers?.Clear();
            Customers.Clear();
            SelectedCustomer = null;

            await DispatcherHelper.ExecuteOnUIThreadAsync(
                async () =>
                {
                    _allCustomers = await GetCustomerListAsync();
                    Customers.AddRange(_allCustomers);
                });
            SetBusy("FirstLoading", false);

        }
    }
}
