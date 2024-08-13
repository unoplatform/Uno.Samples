using SQLiteOneDriveInvoiceSample.Database.Entities;
using SQLiteOneDriveInvoiceSample.Database.Services;
using SQLiteOneDriveInvoiceSample.Helpers;

using System.Linq;

namespace SQLiteOneDriveInvoiceSample.Presentation
{
    public class InvoiceVM : ViewModelBase
    {
        #region Properties

        public Invoice Entity { get; set; }
        public InvoiceDBService Service { get; set; }
        public string Header { get; set; }

        private bool isNew;
        public bool IsNew
        {
            get => isNew;
            set => SetProperty(ref isNew, value);
        }

        #endregion

        #region Constructor(s)

        public InvoiceVM(Invoice entity = null, bool _isNew = true)
        {
            Entity = entity != null ? entity : new Invoice();
            isNew = _isNew;
            Service = new InvoiceDBService();
            Header = isNew ? "New Invoice" : "Edit Invoice";
        }

        #endregion

        #region Method(s)

        public void SaveEntity()
        {
            var userAccount = new AccountDBService().GetUserEntities().entities.FirstOrDefault();
            if (userAccount != null)
            {
                Entity.UserBankAccount = userAccount;
                Entity.UserAddress = userAccount.Address;
                Entity.FullName = userAccount.Holder;

                if (isNew)
                {
                    var result = Service.AddEntity(Entity);
                }
                else
                {
                    var result = Service.UpdateEntity(Entity);
                }
            }



        }

        #endregion
    }
}
