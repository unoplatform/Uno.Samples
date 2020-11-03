using Microsoft.Toolkit.Uwp.Helpers;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnoContoso.Models;
using UnoContoso.Repository;

namespace UnoContoso.Model
{
    public class OrderWrapper : BindableBase
    {
        private readonly IContosoRepository _contosoRepository;

        public OrderWrapper()
        {
        }

        public OrderWrapper(IContosoRepository contosoRepository, Order model = null)
        {
            Model = model;
            _contosoRepository = contosoRepository;
            // Create an ObservableCollection to wrap Order.LineItems so we can track
            // product additions and deletions.
            LineItems = new ObservableCollection<LineItem>(Model.LineItems);
            LineItems.CollectionChanged += LineItems_Changed;

            NewLineItem = new LineItemWrapper();

            if (model.Customer == null)
            {
                Task.Run(() => LoadCustomer(Model.CustomerId));
            }
        }

        /// <summary>
        /// Creates an OrderViewModel that wraps an Order object created from the specified ID.
        /// </summary>
        public async Task<OrderWrapper> CreateFromGuid(Guid orderId) =>
            new OrderWrapper(_contosoRepository, await GetOrder(orderId));

        /// <summary>
        /// Gets the underlying Order object. 
        /// </summary>
        public Order Model { get; }

        /// <summary>
        /// Loads the customer with the specified ID. 
        /// </summary>
        private async void LoadCustomer(Guid customerId)
        {
            var customer = await _contosoRepository.Customers.GetAsync(customerId);
            await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
            {
                Customer = customer;
            });
        }

        /// <summary>
        /// Returns the order with the specified ID.
        /// </summary>
        private async Task<Order> GetOrder(Guid orderId) =>
            await _contosoRepository.Orders.GetAsync(orderId);

        /// <summary>
        /// Gets a value that specifies whether the user can refresh the page.
        /// </summary>
        public bool CanRefresh => Model != null && !IsModified && IsExistingOrder;

        /// <summary>
        /// Gets a value that specifies whether the user can revert changes. 
        /// </summary>
        public bool CanRevert => Model != null && IsModified && IsExistingOrder;

        /// <summary>
        /// Gets or sets the order's ID.
        /// </summary>
        public Guid Id
        {
            get => Model.Id;
            set
            {
                if (Model.Id != value)
                {
                    Model.Id = value;
                    RaisePropertyChanged();
                    IsModified = true;
                }
            }
        }

        bool _IsModified = false;

        /// <summary>
        /// Gets or sets a value that indicates whether the underlying model has been modified. 
        /// </summary>
        public bool IsModified
        {
            get => _IsModified;
            set
            {
                if (value != _IsModified)
                {
                    // Only record changes after the order has loaded. 
                    if (IsLoaded)
                    {
                        _IsModified = value;
                        RaisePropertyChanged();
                    }
                }
            }
        }

        /// <summary>
        /// Gets a value that indicates whether this is an existing order.
        /// </summary>
        public bool IsExistingOrder => !IsNewOrder;

        /// <summary>
        /// Gets a value that indicates whether there is a backing order.
        /// </summary>
        public bool IsLoaded => Model != null && (IsNewOrder || Model.Customer != null);

        /// <summary>
        /// Gets a value that indicates whether there is not a backing order.
        /// </summary>
        public bool IsNotLoaded => !IsLoaded;

        /// <summary>
        /// Gets a value that indicates whether this is a newly-created order.
        /// </summary>
        public bool IsNewOrder => Model.InvoiceNumber == 0;

        /// <summary>
        /// Gets or sets the invoice number for this order. 
        /// </summary>
        public int InvoiceNumber
        {
            get => Model.InvoiceNumber;
            set
            {
                if (Model.InvoiceNumber != value)
                {
                    Model.InvoiceNumber = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(IsNewOrder));
                    RaisePropertyChanged(nameof(IsLoaded));
                    RaisePropertyChanged(nameof(IsNotLoaded));
                    RaisePropertyChanged(nameof(IsNewOrder));
                    RaisePropertyChanged(nameof(IsExistingOrder));
                    IsModified = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets the customer for this order. This value is null
        /// unless you manually retrieve the customer (using CustomerId) and 
        /// set it. 
        /// </summary>
        public Customer Customer
        {
            get => Model.Customer;
            set
            {
                if (Model.Customer != value)
                {
                    var isLoadingOperation = Model.Customer == null &&
                        value != null && !IsNewOrder;
                    Model.Customer = value;
                    RaisePropertyChanged();
                    if (isLoadingOperation)
                    {
                        RaisePropertyChanged(nameof(IsLoaded));
                        RaisePropertyChanged(nameof(IsNotLoaded));
                    }
                    else
                    {
                        IsModified = true;
                    }
                }
            }
        }

        private ObservableCollection<LineItem> _lineItems;

        /// <summary>
        /// Gets the line items in this invoice. 
        /// </summary>
        public ObservableCollection<LineItem> LineItems
        {
            get => _lineItems;
            set
            {
                if (_lineItems != value)
                {
                    if (value != null)
                    {
                        value.CollectionChanged += LineItems_Changed;
                    }

                    if (_lineItems != null)
                    {
                        _lineItems.CollectionChanged -= LineItems_Changed;
                    }
                    _lineItems = value;
                    RaisePropertyChanged();
                    IsModified = true;
                }
            }
        }

        /// <summary>
        /// Notifies anyone listening to this object that a line item changed. 
        /// </summary>
        private void LineItems_Changed(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (LineItems != null)
            {
                Model.LineItems = LineItems.ToList();
            }

            RaisePropertyChanged(nameof(LineItems));
            RaisePropertyChanged(nameof(Subtotal));
            RaisePropertyChanged(nameof(Tax));
            RaisePropertyChanged(nameof(GrandTotal));
            IsModified = true;
        }

        private LineItemWrapper _newLineItem;

        /// <summary>
        /// Gets or sets the line item that the user is currently working on.
        /// </summary>
        public LineItemWrapper NewLineItem
        {
            get => _newLineItem;
            set
            {
                if (value != _newLineItem)
                {
                    if (value != null)
                    {
                        value.PropertyChanged += NewLineItem_PropertyChanged;
                    }

                    if (_newLineItem != null)
                    {
                        _newLineItem.PropertyChanged -= NewLineItem_PropertyChanged;
                    }

                    _newLineItem = value;
                    UpdateNewLineItemBindings();
                }
            }
        }

        private void NewLineItem_PropertyChanged(object sender, PropertyChangedEventArgs e) => UpdateNewLineItemBindings();

        private void UpdateNewLineItemBindings()
        {
            RaisePropertyChanged(nameof(NewLineItem));
            RaisePropertyChanged(nameof(HasNewLineItem));
            RaisePropertyChanged(nameof(NewLineItemProductListPriceFormatted));
        }

        /// <summary>
        /// Gets or sets whether there is a new line item in progress.
        /// </summary>
        public bool HasNewLineItem => NewLineItem != null && NewLineItem.Product != null;

        /// <summary>
        /// Gets the product list price of the new line item, formatted for display.
        /// </summary>
        public string NewLineItemProductListPriceFormatted => (NewLineItem?.Product?.ListPrice ?? 0).ToString("n");

        /// <summary>
        /// Gets or sets the date this order was placed. 
        /// </summary>
        public DateTime DatePlaced
        {
            get => Model.DatePlaced;
            set
            {
                if (Model.DatePlaced != value)
                {
                    Model.DatePlaced = value;
                    RaisePropertyChanged();
                    IsModified = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets the date this order was filled. 
        /// This value is automatically updated when the 
        /// OrderStatus changes. 
        /// </summary>
        public DateTime? DateFilled
        {
            get => Model.DateFilled;
            set
            {
                if (value != Model.DateFilled)
                {
                    Model.DateFilled = value;
                    RaisePropertyChanged();
                    IsModified = true;
                }
            }
        }

        /// <summary>
        /// Gets the subtotal. This value is calculated automatically. 
        /// </summary>
        public decimal Subtotal => Model.Subtotal;

        /// <summary>
        /// Gets the tax. This value is calculated automatically. 
        /// </summary>
        public decimal Tax => Model.Tax;

        /// <summary>
        /// Gets the total. This value is calculated automatically. 
        /// </summary>
        public decimal GrandTotal => Model.GrandTotal;

        /// <summary>
        /// Gets or sets the shipping address, which may be different
        /// from the customer's primary address. 
        /// </summary>
        public string Address
        {
            get => Model.Address;
            set
            {
                if (Model.Address != value)
                {
                    Model.Address = value;
                    RaisePropertyChanged();
                    IsModified = true;
                }
            }
        }

        /// <summary>
        /// Sets the PaymentStatus property by parsing a string representation of the enum value.
        /// </summary>
        public void SetPaymentStatus(object value) => PaymentStatus = value == null ?
            PaymentStatus.Unpaid : (PaymentStatus)Enum.Parse(typeof(PaymentStatus), value as string);

        /// <summary>
        /// Gets or sets the payment status.
        /// </summary>
        public PaymentStatus PaymentStatus
        {
            get => Model.PaymentStatus;
            set
            {
                if (Model.PaymentStatus != value)
                {
                    Model.PaymentStatus = value;
                    RaisePropertyChanged();
                    IsModified = true;
                }
            }
        }

        /// <summary>
        /// Sets the Term property by parsing a string representation of the enum value.
        /// </summary>
        public Term SetTerm(object value) => Term = value == null ?
            Term.Net1 : (Term)Enum.Parse(typeof(Term), value as string);

        /// <summary>
        /// Gets or sets the payment term.
        /// </summary>
        public Term Term
        {
            get => Model.Term;
            set
            {
                if (Model.Term != value)
                {
                    Model.Term = value;
                    RaisePropertyChanged();
                    IsModified = true;
                }
            }
        }

        /// <summary>
        /// Sets the Status property by parsing a string representation of the enum value.
        /// </summary>
        public OrderStatus SetOrderStatus(object value) => OrderStatus = value == null ?
            OrderStatus.Open : (OrderStatus)Enum.Parse(typeof(OrderStatus), value as string);

        /// <summary>
        /// Gets or sets the order status.
        /// </summary>
        public OrderStatus OrderStatus
        {
            get => Model.Status;
            set
            {
                if (Model.Status != value)
                {
                    Model.Status = value;
                    RaisePropertyChanged();

                    // Update the DateFilled value.
                    DateFilled = Model.Status == OrderStatus.Filled ? (DateTime?)DateTime.Now : null;
                    IsModified = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of the order's customer. 
        /// </summary>
        public string CustomerName
        {
            get => Model.CustomerName;
            set
            {
                if (Model.CustomerName != value)
                {
                    Model.CustomerName = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Saves the current order to the database. 
        /// </summary>
        public async Task SaveOrderAsync()
        {
            Order result = null;
            try
            {
                result = await _contosoRepository.Orders.UpsertAsync(Model);
            }
            catch (Exception ex)
            {
                throw new OrderSavingException("Unable to save. There might have been a problem " +
                    "connecting to the database. Please try again.", ex);
            }

            if (result != null)
            {
                await DispatcherHelper.ExecuteOnUIThreadAsync(() => IsModified = false);
            }
            else
            {
                await DispatcherHelper.ExecuteOnUIThreadAsync(() => new OrderSavingException(
                    "Unable to save. There might have been a problem " +
                    "connecting to the database. Please try again."));
            }
        }
    }
}
