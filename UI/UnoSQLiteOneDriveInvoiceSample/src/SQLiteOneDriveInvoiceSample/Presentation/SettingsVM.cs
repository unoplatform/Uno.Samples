using SQLiteOneDriveInvoiceSample.CloudProvider;
using SQLiteOneDriveInvoiceSample.Database.Entities;
using SQLiteOneDriveInvoiceSample.Database.Enums;
using SQLiteOneDriveInvoiceSample.Database.Services;
using SQLiteOneDriveInvoiceSample.Helpers;
using SQLiteOneDriveInvoiceSample.Views;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteOneDriveInvoiceSample.Presentation
{
    public class SettingsVM : ViewModelBase
    {
        #region OneDrive Properties

        private bool isSignedIn;
        public bool IsSignedIn
        {
            get => isSignedIn;
            set => SetProperty(ref isSignedIn, value);
        }

        private bool isBackedUp;
        public bool IsBackedUp
        {
            get => isBackedUp;
            set => SetProperty(ref isBackedUp, value);
        }

        // The user's display name
        private string userName;
        public string Username
        {
            get => userName;
            set => SetProperty(ref userName, value);
        }

        // The user's email
        private string userEmail;
        public string UserEmail
        {
            get => userEmail;
            set => SetProperty(ref userEmail, value);
        }

        #endregion

        #region Properties

        public Account UserAccount { get; set; }
        public Address UserAddress { get; set; }

        #endregion

        #region DBServices

        public AccountDBService AccountDBService { get; set; }
        public AddressDBService AddressDBService { get; set; }

        #endregion

        #region DB Services

        private void InitializeServices()
        {
            AccountDBService = new AccountDBService();
            AddressDBService = new AddressDBService();
        }

        private void LoadEntities()
        {
            var fetchAccount = AccountDBService.GetUserEntities();
            var fetchAddress = AddressDBService.GetUserEntities();

            UserAccount = fetchAccount.entities.FirstOrDefault();
            UserAddress = fetchAddress.entities.FirstOrDefault();

            if (UserAccount == null)
            {
                UserAccount = new Account();
                UserAccount.IsUser = true;
            }
            if (UserAddress == null)
            {
                UserAddress = new Address();
                UserAddress.IsUser = true;
            }

        }

        public async Task SaveAccount(Account account)
        {
            if (account.BankName != null && account.Holder != null && account.Currency != null)
            {
                var result = AccountDBService.UpdateEntity(account);
                if (!result.isSuccessful)
                {
                    await Dialogs.GenericDialogAsync("Account Save Failed", result.operationMessage, "OK");
                }
                await Dialogs.GenericDialogAsync($"Account Saved", result.operationMessage, "OK");
            }
        }

        public async Task SaveAddress(Address address)
        {
            if (address.AddressOne != null && address.City != null && address.Country != null && address.PostalCode != null)
            {
                var result = AddressDBService.UpdateEntity(address);
                if (!result.isSuccessful)
                {
                    await Dialogs.GenericDialogAsync("Account Save Failed", result.operationMessage, "OK");
                }
                await Dialogs.GenericDialogAsync($"Account Saved", result.operationMessage, "OK");
            }
        }

        #region OneDrive Methods

        public async Task OneDriveSetupAsync()
        {
            try
            {
                OneDrive.BuildPublicClientApplication();
                var result = await OneDrive.InitializeWithSilentProviderAsync();
                if (result == null)
                {
                    result = await OneDrive.InitializeWithInteractiveProviderAsync();
                }
                await OneDrive.InitializeGraphClientAsync(result);
                var user = await OneDrive.AccountInfo();
                Username = user.Value.username;
                UserEmail = user.Value.email;
                IsSignedIn = user.Value.isSignedIn;
            }
            catch (Exception exception)
            {
                await Dialogs.ExceptionDialogAsync(exception);
            }
        }

        public async Task LogOutAsync() => await OneDrive.RemoveAccountsAsync();

        public async Task Restore()
        {
            try
            {
                await OneDrive.Restore("SQLiteOneDriveInvoiceSample.db");
                LoadEntities();
                await Dialogs.GenericDialogAsync($"Databased Restored", "Database was successfully restored.", "OK");
            }
            catch (Exception exception)
            {
                await Dialogs.ExceptionDialogAsync(exception);
            }
        }

        public async Task BackUp()
        {
            try
            {
                await OneDrive.BackUp("SQLiteOneDriveInvoiceSample.db");
                await Dialogs.GenericDialogAsync($"Databased Backed up", "Database was successfully backed up.", "OK");
            }
            catch (Exception exception)
            {
                await Dialogs.ExceptionDialogAsync(exception);
            }
        }
        #endregion

        #endregion

        public SettingsVM()
        {
            InitializeServices();
            LoadEntities();

        }
    }
}
