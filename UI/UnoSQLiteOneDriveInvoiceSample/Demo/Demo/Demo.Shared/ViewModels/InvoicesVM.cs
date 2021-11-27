using Demo.Database;
using Demo.Database.Entities;
using Demo.Database.Services;
using Demo.Helpers;

using System;
using System.Collections.ObjectModel;
using System.Linq;

using Uno.Extensions;

namespace Demo.ViewModels
{
    public class InvoicesVM : BaseNotifyClass
    {
        #region Properties

        public ObservableCollection<Invoice> Invoices { get; set; }
        public Account UserAccount { get; set; }
        public Address UserAddress { get; set; }

        #endregion     

        #region DBServices

        public InvoiceDBService InvoiceDBService { get; set; }
        public AccountDBService AccountDBService { get; set; }
        public AddressDBService AddressDBService { get; set; }

        #endregion

        #region DB Services

        private void InitializeServices()
        {
            InvoiceDBService = new InvoiceDBService();
            AccountDBService = new AccountDBService();
            AddressDBService = new AddressDBService();
        }

        private void LoadEntities()
        {
            
            var fetchInvoices = InvoiceDBService.GetEntities();
            Invoices = new ObservableCollection<Invoice>(fetchInvoices.entities);

        }

        public void DeleteEntity(Invoice invoice )
        {
            var result = InvoiceDBService.DeleteEntity(invoice);
            if (result.isSuccessful)
            {
                Invoices.Remove(invoice);
            }
        }

        #endregion

        public InvoicesVM()
        {
            InitializeServices();
            LoadEntities();
        }
    }
}
