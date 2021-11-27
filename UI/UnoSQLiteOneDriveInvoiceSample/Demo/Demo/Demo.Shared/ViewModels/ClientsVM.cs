using Demo.Database;
using Demo.Database.Entities;
using Demo.Database.Enums;
using Demo.Database.Services;
using Demo.Helpers;
using Demo.Pages;

using SQLite;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Demo.ViewModels
{
    public class ClientsVM : BaseNotifyClass
    {
        #region Properties

        public ObservableCollection<Client> Clients { get; set; }

        #endregion

        #region DBServices

        public ClientDBService ClientDBService { get; set; }

        #endregion

        #region DB Services        

        private void InitializeServices()
        {
            ClientDBService = new ClientDBService();
        }

        private void LoadEntities()
        {
            
            var fetchClients = ClientDBService.GetEntities();
            Clients = new ObservableCollection<Client>(fetchClients.entities); 
            
        }

        public void DeleteEntity(Client client)
        {
            var result = ClientDBService.DeleteEntity(client);
            if (result.isSuccessful)
            {
                //Clients.Clear()
                Clients.Remove(client);
            }
        }
        #endregion

        public ClientsVM()
        {
            InitializeServices();
            LoadEntities();
        }
    }
}
