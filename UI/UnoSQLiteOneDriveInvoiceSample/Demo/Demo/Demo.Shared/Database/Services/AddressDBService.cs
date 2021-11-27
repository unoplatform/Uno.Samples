using Demo.Database.Entities;
using Demo.Database.Exceptions;
using SQLite;

using SQLiteNetExtensions.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Demo.Database.Services
{
    public class AddressDBService : BaseDBService<Address>
    {
        #region ReadOperation(s)

        public override (bool isSuccessful, string operationMessage, Address entity) GetEntity(Guid addressId)
        {
            using (var connection = new SQLiteConnection(databasePath))
            {
                try
                {
                    var address = connection.GetWithChildren<Address>(addressId, recursive: true);
                    return (true, $"Read of entity of type : {nameof(Address)} was Succcesful.", address);
                }
                catch (DatabaseException exception)
                {
                    return (false, $"Read of entity of type : {nameof(Address)} was not Succcesful. \n Exception Details: {exception}.\n ADDRESS ID: {addressId}.", null);
                }
            }
        }

        public override (bool isSuccessful, string operationMessage, List<Address> entities) GetEntities(Guid[] addressIds = null)
        {
            using (var connection = new SQLiteConnection(databasePath))
            {
                try
                {
                    var addresses = addressIds == null ? connection.GetAllWithChildren<Address>() : connection.GetAllWithChildren<Address>(address => addressIds.Contains(address.Id), true);
                    return (true, $"Read of collection entity of type : {nameof(Address)} was Succcesful.", addresses);
                }
                catch (DatabaseException exception)
                {
                    return (false, $"Read of collection entity of type : {nameof(Address)} was not Succcesful. \n Exception Details: {exception}.\n ADDRESS IDs: {addressIds}.", null);
                }
            }
        }

        public (bool isSuccessful, string operationMessage, List<Address> entities) GetUserEntities()
        {
            using (var connection = new SQLiteConnection(databasePath))
            {
                try
                {
                    var addresses = connection.GetAllWithChildren<Address>(address => address.IsUser, true);
                    return (true, $"Read of collection entity of type : {nameof(Address)} was Successful.", addresses);
                }
                catch (DatabaseException exception)
                {
                    return (false, $"Read of collection entities of type : {nameof(Address)} and is User owned was not Successful. \n Exception Details: {exception}.\n ", null);
                }
            }
        }

        #endregion

        #region DeleteOperation(s)

        public override (bool isSuccessful, string operationMessage, object errorObject) DeleteEntity(Address address)
        {
            using (var connection = new SQLiteConnection(databasePath))
            {
                try
                {
                    connection.Delete(address, recursive: true);
                    return (true, $"Delete of entity of type : {nameof(Address)} was Successful", null);
                }
                catch (DatabaseException exception)
                {
                    return (false, $"Delete of entity of type : {nameof(Address)} was not Succcesful. \n Exception Details: {exception}.\n ADDRESS IDs: {address.Id}.", null);
                }
            }
        }

        public override (bool isSuccessful, string operationMessage, object errorObject) DeleteEntities(Address[] addressIds)
        {
            using (var connection = new SQLiteConnection(databasePath))
            {
                try
                {
                    connection.DeleteAll(addressIds, true);
                    return (true, $"Delete of collection of entity of type : {nameof(Address)} was Successful", null);
                }
                catch (DatabaseException exception)
                {
                    return (false, $"Delete of collection of entity of type : {nameof(Address)} was not Succcesful. \n Exception Details: {exception}.\n ADDRESS IDs: {addressIds}.", null);
                }
            }
        }

        #endregion
    }
}
