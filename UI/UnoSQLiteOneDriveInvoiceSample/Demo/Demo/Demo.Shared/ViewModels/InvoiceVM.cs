using Demo.Database.Entities;
using Demo.Database.Services;
using Demo.Helpers;

using System.Linq;

namespace Demo.ViewModels
{
    public class InvoiceVM : BaseNotifyClass
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
