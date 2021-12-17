using Demo.Database.Entities;
using Demo.Database.Exceptions;

using SQLite;

using SQLiteNetExtensions.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Demo.Database.Services
{
    public class ClientDBService : BaseDBService<Client>
    {
        #region ReadOperation(s)

        public override (bool isSuccessful, string operationMessage, Client entity) GetEntity(Guid clientId)
        {
            using (var connection = new SQLiteConnection(databasePath))
            {
                try
                {
                    var client = connection.GetWithChildren<Client>(clientId, recursive: true);
                    return (true, $"Read of entity of type : {nameof(Client)} was Successful.", client);
                }
                catch (DatabaseException exception)
                {
                    return (false, $"Read of entity of type : {nameof(Client)} was not Successful. \n Exception Details: {exception}.\n CLIENT ID: {clientId}.", null);
                }
            }
        }

        public override (bool isSuccessful, string operationMessage, List<Client> entities) GetEntities(Guid[] clientIds = null)
        {
            using (var connection = new SQLiteConnection(databasePath))
            {
                try
                {
                    var clients = clientIds == null ? connection.GetAllWithChildren<Client>() : connection.GetAllWithChildren<Client>(filter: client => clientIds.Contains(client.Id), recursive: true);
                    return (true, $"Read of collection entity of type : {nameof(Client)} was Successful.", clients);
                }
                catch (DatabaseException exception)
                {
                    return (false, $"Read of collection entity of type : {nameof(Client)} was not Successful. \n Exception Details: {exception}.\n CLIENT IDs: {clientIds}.", null);
                }
            }
        }

        #endregion

        #region DeleteOperation(s)

        public override (bool isSuccessful, string operationMessage, object errorObject) DeleteEntity(Client client)
        {
            using (var connection = new SQLiteConnection(databasePath))
            {
                try
                {
                    connection.Delete(client, recursive: true);
                    return (true, $"Delete of entity of type : {nameof(Client)} was Successful", null);
                }
                catch (DatabaseException exception)
                {
                    return (false, $"Delete of entity of type : {nameof(Client)} was not Successful. \n Exception Details: {exception}.\n CLIENT IDs: {client.Id}.", null);
                }
            }
        }

        public override (bool isSuccessful, string operationMessage, object errorObject) DeleteEntities(Client[] clientIds)
        {
            using (var connection = new SQLiteConnection(databasePath))
            {
                try
                {
                    connection.DeleteAll(clientIds, true);
                    return (true, $"Delete of collection of entity of type : {nameof(Client)} was Successful", null);
                }
                catch (DatabaseException exception)
                {
                    return (false, $"Delete of collection of entity of type : {nameof(Client)} was not Successful. \n Exception Details: {exception}.\n CLIENT IDs: {clientIds}.", null);
                }
            }
        }

        #endregion
    }
}
