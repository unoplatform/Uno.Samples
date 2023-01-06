using Prism.Commands;
using Prism.Ioc;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UnoContoso.Enums;
using UnoContoso.Events;
using UnoContoso.Model;
using UnoContoso.Models;
using UnoContoso.Repository;

namespace UnoContoso.ViewModels
{
    public class CustomerDetailViewModel : ViewModelBase
    {
        private CustomerWrapper _customer;
        private readonly IContosoRepository _contosoRepository;

        public CustomerWrapper Customer
        {
            get { return _customer; }
            set { SetProperty(ref _customer, value); }
        }

        public ICommand SaveCommand { get; set; }

        public ICommand CancelCommand { get; set; }

        public ICommand EditCommand { get; set; }

        public ICommand AddOrderCommand { get; set; }

        public ICommand RefreshOrderCommand { get; set; }

        public ICommand ViewInvoiceCommand { get; set; }

        private string _searchBoxText;
        public string SearchBoxText
        {
            get { return _searchBoxText; }
            set { SetProperty(ref _searchBoxText, value); }
        }

        private IList<string> _suggestItems;

        public IList<string> SuggestItems
        {
            get { return _suggestItems; }
            set { SetProperty(ref _suggestItems, value); }
        }

        private string _queryText;
        public string QueryText
        {
            get { return _queryText; }
            set { SetProperty(ref _queryText, value); }
        }

        public CustomerDetailViewModel()
        {
        }

        public CustomerDetailViewModel(IContainerProvider containerProvider,
            IContosoRepository contosoRepository)
            : base(containerProvider)
        {
            Title = "Details";
            _contosoRepository = contosoRepository;
            Init();
        }
        private void Init()
        {
            SaveCommand = new DelegateCommand(OnSave);
            CancelCommand = new DelegateCommand(
                async () => await Customer.CancelEditsAsync());
            EditCommand = new DelegateCommand(
                () => Customer.StartEdit());
            AddOrderCommand = new DelegateCommand(OnAddOrder);
            RefreshOrderCommand = new DelegateCommand(
                () => Customer.RefreshOrders());
            ViewInvoiceCommand = new DelegateCommand<Order>(OnViewInvoice);

            PropertyChanged += CustomerDetailViewModel_PropertyChanged;
        }

        private void OnViewInvoice(Order obj)
        {
            NavigationService.RequestNavigate("OrderDetailView",
                new NavigationParameters
                {
                    {"OrderId", obj.Id },
                });
        }

        private void CustomerDetailViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(SearchBoxText):
                    SetSuggestItems(SearchBoxText);
                    break;
                case nameof(QueryText):
                    SetCustomer(QueryText);
                    break;
            }
        }

        private void SetCustomer(string queryText)
        {
            var part = queryText.Split(',');
            if (part.Length != 2) return;
            NavigationService.RequestNavigate("CustomerDetailView",
                new NavigationParameters
                {
                    {"CustomerId", Guid.Parse(part.Last()) },
                });
            QueryText = SearchBoxText = string.Empty;
        }

        private async void SetSuggestItems(string searchBoxText)
        {
            if(string.IsNullOrEmpty(searchBoxText))
            {
                SuggestItems = null;
            }
            else
            {
                var customers = await _contosoRepository.Customers.GetAsync(searchBoxText);
                if (customers == null) return;
                SuggestItems = customers
                    .Select(c => $"{c.FirstName} {c.LastName}                                                  ,{c.Id}")
                    .ToList();
            }
        }

        private async void OnSave()
        {
            var result = await Customer.SaveAsync();
            if(result.Item1 == true)
            {
                DialogService.ShowDialog("MessageControl",
                    new DialogParameters
                    {
                        { "title", "Work completed" },
                        { "message", $""}
                    }, null);
                var change = (EntityChanges)Enum.Parse(typeof(EntityChanges), result.Item2);
                EventAggregator.GetEvent<CustomerEvent>()
                    .Publish(new EventArgs.CustomerEventArgs 
                    { 
                        Changes = change,
                        Customer = Customer.Model
                    });
            }
            else
            {
                DialogService.ShowDialog("MessageControl",
                    new DialogParameters
                    {
                        { "title", "Unable to save" },
                        { "message", $"There was an error saving your customer:\n{result.Item2}"}
                    }, null);
            }
        }

        private void OnAddOrder()
        {
            NavigationService.RequestNavigate("OrderDetailView",
                new NavigationParameters 
                {
                    { "CustomerId", Customer.Model.Id }
                });
        }

        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            if(navigationContext.Parameters.ContainsKey("CustomerId") == false)
            {
                Customer = new CustomerWrapper(_contosoRepository, new Models.Customer()) 
                {
                    IsNewCustomer = true,
                    IsInEdit = true
                };
            }
            else
            {
                SetBusy("CustomerLoad", true);
                var id = navigationContext.Parameters.GetValue<Guid>("CustomerId");
                //Debug.WriteLine($"{Title} / {id}");
                await DispatcherHelper.ExecuteOnUIThreadAsync(
                    async () =>
                    {
                        var customer = await _contosoRepository.Customers.GetAsync(id);
                        Customer = new CustomerWrapper(_contosoRepository, customer);
                        await Customer.LoadOrdersAsync();
                    });
                SetBusy("CustomerLoad", false);
            }
        }
    }
}
