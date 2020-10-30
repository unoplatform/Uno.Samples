using Microsoft.Toolkit.Uwp.Helpers;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnoContoso.Enums;
using UnoContoso.Models;
using UnoContoso.Repository;

namespace UnoContoso.Model
{
    public class CustomerWrapper : BindableBase, IEditableObject
    {
        private readonly IContosoRepository _contosoRepository;
        private Customer _model;

        public CustomerWrapper()
        {
        }
        /// <summary>
        /// Initializes a new instance of the CustomerViewModel class that wraps a Customer object.
        /// </summary>
        public CustomerWrapper(IContosoRepository contosoRepository, Customer model = null)
        {
            _contosoRepository = contosoRepository;
            Model = model ?? new Customer();
        }

        /// <summary>
        /// Gets or sets the underlying Customer object.
        /// </summary>
        public Customer Model
        {
            get => _model;
            set
            {
                if (_model != value)
                {
                    _model = value;
                    //Comment processing due to performance issues
                    //RefreshOrders();
                    // Raise the PropertyChanged event for all properties.
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the customer's first name.
        /// </summary>
        public string FirstName
        {
            get => Model.FirstName;
            set
            {
                if (value != Model.FirstName)
                {
                    Model.FirstName = value;
                    IsModified = true;
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(Name));
                }
            }
        }

        /// <summary>
        /// Gets or sets the customer's last name.
        /// </summary>
        public string LastName
        {
            get => Model.LastName;
            set
            {
                if (value != Model.LastName)
                {
                    Model.LastName = value;
                    IsModified = true;
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(Name));
                }
            }
        }

        /// <summary>
        /// Gets the customers full (first + last) name.
        /// </summary>
        public string Name => IsNewCustomer && string.IsNullOrEmpty(FirstName)
            && string.IsNullOrEmpty(LastName) ? "New customer" : $"{FirstName} {LastName}";

        /// <summary>
        /// Gets or sets the customer's address.
        /// </summary>
        public string Address
        {
            get => Model.Address;
            set
            {
                if (value != Model.Address)
                {
                    Model.Address = value;
                    IsModified = true;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the customer's company.
        /// </summary>
        public string Company
        {
            get => Model.Company;
            set
            {
                if (value != Model.Company)
                {
                    Model.Company = value;
                    IsModified = true;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the customer's phone number. 
        /// </summary>
        public string Phone
        {
            get => Model.Phone;
            set
            {
                if (value != Model.Phone)
                {
                    Model.Phone = value;
                    IsModified = true;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the customer's email. 
        /// </summary>
        public string Email
        {
            get => Model.Email;
            set
            {
                if (value != Model.Email)
                {
                    Model.Email = value;
                    IsModified = true;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the underlying model has been modified. 
        /// </summary>
        /// <remarks>
        /// Used when sync'ing with the server to reduce load and only upload the models that have changed.
        /// </remarks>
        public bool IsModified { get; set; }

        /// <summary>
        /// Gets the collection of the customer's orders.
        /// </summary>
        public ObservableCollection<Order> Orders { get; } 
            = new ObservableCollection<Order>();

        private Order _selectedOrder;

        /// <summary>
        /// Gets or sets the currently selected order.
        /// </summary>
        public Order SelectedOrder
        {
            get => _selectedOrder;
            set => SetProperty(ref _selectedOrder, value);
        }

        private bool _isNewCustomer;

        /// <summary>
        /// Gets or sets a value that indicates whether this is a new customer.
        /// </summary>
        public bool IsNewCustomer
        {
            get => _isNewCustomer;
            set => SetProperty(ref _isNewCustomer, value);
        }

        private bool _isInEdit = false;

        /// <summary>
        /// Gets or sets a value that indicates whether the customer data is being edited.
        /// </summary>
        public bool IsInEdit
        {
            get => _isInEdit;
            set => SetProperty(ref _isInEdit, value);
        }

        /// <summary>
        /// Saves customer data that has been edited.
        /// </summary>
        public async Task<Tuple<bool, string>> SaveAsync()
        {
            EntityChanges change = EntityChanges.Changed;
            IsInEdit = false;
            IsModified = false;
            if (IsNewCustomer)
            {
                IsNewCustomer = false;
                change = EntityChanges.Added;
            }

            try
            {
                var customer = await _contosoRepository.Customers.UpsertAsync(Model);
                RefreshOrders();
                Model = customer;
            }
            catch (CustomerSavingException ex)
            {
                return new Tuple<bool, string>(false, ex.Message);
            }
            return new Tuple<bool, string>(true, change.ToString());
        }

        /// <summary>
        /// Raised when the user cancels the changes they've made to the customer data.
        /// </summary>
        public event EventHandler AddNewCustomerCanceled;

        /// <summary>
        /// Cancels any in progress edits.
        /// </summary>
        public async Task CancelEditsAsync()
        {
            if (IsNewCustomer)
            {
                AddNewCustomerCanceled?.Invoke(this, System.EventArgs.Empty);
            }
            else
            {
                await RevertChangesAsync();
            }
        }

        /// <summary>
        /// Discards any edits that have been made, restoring the original values.
        /// </summary>
        public async Task RevertChangesAsync()
        {
            IsInEdit = false;
            if (IsModified)
            {
                await RefreshCustomerAsync();
                IsModified = false;
            }
        }

        /// <summary>
        /// Enables edit mode.
        /// </summary>
        public void StartEdit() => IsInEdit = true;

        /// <summary>
        /// Reloads all of the customer data.
        /// </summary>
        public async Task RefreshCustomerAsync()
        {
            RefreshOrders();
            Model = await _contosoRepository.Customers.GetAsync(Model.Id);
        }

        /// <summary>
        /// Resets the customer detail fields to the current values.
        /// </summary>
        public void RefreshOrders() => Task.Run(LoadOrdersAsync);

        /// <summary>
        /// Loads the order data for the customer.
        /// </summary>
        public async Task LoadOrdersAsync()
        {
            var orders = await _contosoRepository.Orders.GetForCustomerAsync(Model.Id);

            await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
            {
                Orders.Clear();
                Orders.AddRange(orders);
            });
        }

        /// <summary>
        /// Called when a bound DataGrid control causes the customer to enter edit mode.
        /// </summary>
        public void BeginEdit()
        {
            // Not used.
        }

        /// <summary>
        /// Called when a bound DataGrid control cancels the edits that have been made to a customer.
        /// </summary>
        public async void CancelEdit() => await CancelEditsAsync();

        /// <summary>
        /// Called when a bound DataGrid control commits the edits that have been made to a customer.
        /// </summary>
        public async void EndEdit() => await SaveAsync();

        public void UpdateProperty()
        {
            RaisePropertyChanged(nameof(FirstName));
            RaisePropertyChanged(nameof(LastName));
            RaisePropertyChanged(nameof(Address));
            RaisePropertyChanged(nameof(Company));
            RaisePropertyChanged(nameof(Phone));
            RaisePropertyChanged(nameof(Email));
        }

        public override string ToString()
        {
            return Model.ToString();
        }
    }
}
