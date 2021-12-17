using Demo.Database.Entities;
using Demo.Database.Exceptions;

using SQLite;

using SQLiteNetExtensions.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Demo.Database.Services
{
    public class AccountDBService : BaseDBService<Account>
    {
        #region ReadOperation(s)

        public override (bool isSuccessful, string operationMessage, Account entity) GetEntity(Guid accountId)
        {
            using (var connection = new SQLiteConnection(databasePath))
            {
                try
                {
                    var account = connection.GetWithChildren<Account>(accountId, recursive: true);
                    return (true, $"Read of entity of type : {nameof(Account)} was Successful.", account);
                }
                catch (DatabaseException exception)
                {
                    return (false, $"Read of entity of type : {nameof(Account)} was not Successful. \n Exception Details: {exception}.\n ACCOUNT ID: {accountId}.", null);
                }
            }
        }

        public override (bool isSuccessful, string operationMessage, List<Account> entities) GetEntities(Guid[] accountIds = null)
        {
            using (var connection = new SQLiteConnection(databasePath))
            {
                try
                {
                    var accounts = accountIds == null ? connection.GetAllWithChildren<Account>() : connection.GetAllWithChildren<Account>(account => accountIds.Contains(account.Id), true);
                    return (true, $"Read of collection entity of type : {nameof(Account)} was Successful.", accounts);
                }
                catch (DatabaseException exception)
                {
                    return (false, $"Read of collection entity of type : {nameof(Account)} was not Successful. \n Exception Details: {exception}.\n ACCOUNT IDs: {accountIds}.", null);
                }
            }
        }

        public (bool isSuccessful, string operationMessage, List<Account> entities) GetUserEntities()
        {
            using (var connection = new SQLiteConnection(databasePath))
            {
                try
                {
                    var accounts = connection.GetAllWithChildren<Account>(account => account.IsUser, true);
                    return (true, $"Read of collection entity of type : {nameof(Account)} was Successful.", accounts);
                }
                catch (DatabaseException exception)
                {
                    return (false, $"Read of collection entities of type : {nameof(Account)} and is User owned was not Successful. \n Exception Details: {exception}.\n ", null);
                }
            }
        }

        #endregion

        #region DeleteOperation(s)

        public override (bool isSuccessful, string operationMessage, object errorObject) DeleteEntity(Account account)
        {
            using (var connection = new SQLiteConnection(databasePath))
            {
                try
                {
                    connection.Delete(account, recursive: true);
                    return (true, $"Delete of entity of type : {nameof(Account)} was Successful", null);
                }
                catch (DatabaseException exception)
                {
                    return (false, $"Delete of entity of type : {nameof(Account)} was not Successful. \n Exception Details: {exception}.\n ACCOUNT IDs: {account.Id}.", null);
                }
            }
        }

        public override (bool isSuccessful, string operationMessage, object errorObject) DeleteEntities(Account[] accounts)
        {
            using (var connection = new SQLiteConnection(databasePath))
            {
                try
                {
                    connection.DeleteAll(accounts, true);
                    return (true, $"Delete of collection of entity of type : {nameof(Account)} was Successful", null);
                }
                catch (DatabaseException exception)
                {
                    return (false, $"Delete of collection of entity of type : {nameof(Account)} was not Successful. \n Exception Details: {exception}.\n ACCOUNT IDs: {accounts}.", null);
                }
            }
        }

        #endregion
    }
}
