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
    public class InvoiceDBService : BaseDBService<Invoice>
    {
        #region CreateOperation(s)

        public override (bool isSuccessful, string operationMessage, object errorObject) AddEntity(Invoice entity)
        {
            using (var connection = new SQLiteConnection(databasePath))
            {
                try
                {
                    connection.InsertWithChildren(entity);
                    //var itemsInsert = AddInvoiceItem(connection, entity, entity.Items);
                    connection.GetChildren(entity);

                    return (true, $"Insert of type : {nameof(Invoice)} with Children was Successful.", null);
                }
                catch (DatabaseException exception)
                {
                    return (false, $"Insert of type : {nameof(Invoice)} with Children was not Successful. \n Exception Details: {exception} ", entity);
                }
            }
        }

        public override (bool isSuccessful, string operationMessage, object errorObject) AddEntities(Invoice[] entities)
        {
            using (var connection = new SQLiteConnection(databasePath))
            {
                try
                {
                    connection.InsertAllWithChildren(entities);
                    return (true, $"Insert of a collection of type : {nameof(Invoice)} with Children was Successful.", null);
                }
                catch (DatabaseException exception)
                {
                    return (false, $"Insert of a collection of type : {nameof(Invoice)} with Children was not Successful. \n Exception Details: {exception} ", entities);
                }
            }
        }

        #endregion

        #region UpdateOperation(s)

        public override (bool isSuccessful, string operationMessage, object errorObject) UpdateEntity(Invoice entity)
        {
            using (var connection = new SQLiteConnection(databasePath))
            {
                try
                {
                    connection.InsertOrReplaceWithChildren(entity);
                    return (true, $"Insert of a collection of type : {nameof(Invoice)} with Children was Successful.", null);
                }
                catch (DatabaseException exception)
                {
                    return (false, $"Update of type : {nameof(Invoice)} with Children was not Successful. \n Exception Details: {exception} ", entity);
                }
            }
        }

        public override (bool isSuccessful, string operationMessage, object errorObject) UpdateEntities(Invoice[] entities)
        {
            using (var connection = new SQLiteConnection(databasePath))
            {
                try
                {
                    (bool isSuccessful, string operationMessage, object errorObject) result = (false, string.Empty, null);
                    foreach (var entity in entities)
                    {
                        result = UpdateEntity(entity);
                        //LogInfo
                        //if(result.isSuccessful == false) { return result;}
                    }
                    return result;
                }
                catch (DatabaseException exception)
                {
                    return (false, $"Update of a collection of type : {nameof(Invoice)} with Children was not Successful. \n Exception Details: {exception} ", entities);
                }
            }
        }

        #endregion

        #region ReadOperation(s)

        public override (bool isSuccessful, string operationMessage, Invoice entity) GetEntity(Guid invoiceId)
        {
            using (var connection = new SQLiteConnection(databasePath))
            {
                try
                {
                    var invoice = connection.GetWithChildren<Invoice>(invoiceId, recursive: true);
                    return (true, $"Read of entity of type : {nameof(Invoice)} was Successful.", invoice);
                }
                catch (DatabaseException exception)
                {
                    return (false, $"Read of entity of type : {nameof(Invoice)} was not Successful. \n Exception Details: {exception}.\n INVOICE ID: {invoiceId}.", null);
                }
            }
        }

        public override (bool isSuccessful, string operationMessage, List<Invoice> entities) GetEntities(Guid[] invoiceIds = null)
        {
            using (var connection = new SQLiteConnection(databasePath))
            {
                try
                {
                    var quotes = invoiceIds == null ? connection.GetAllWithChildren<Invoice>() : connection.GetAllWithChildren<Invoice>(invoice => invoiceIds.Contains(invoice.Id));
                    return (true, $"Read of collection entity of type : {nameof(Invoice)} was Successful.", quotes);
                }
                catch (DatabaseException exception)
                {
                    return (false, $"Read of collection entity of type : {nameof(Invoice)} was not Successful. \n Exception Details: {exception}.\n INVOICE IDs: {invoiceIds}.", null);
                }
            }
        }

        #endregion

        #region DeleteOperation(s)

        public override (bool isSuccessful, string operationMessage, object errorObject) DeleteEntity(Invoice invoice)
        {
            using (var connection = new SQLiteConnection(databasePath))
            {
                try
                {
                    connection.Delete(invoice);
                    return (true, $"Delete of entity of type : {nameof(Invoice)} was Successful", null);
                }
                catch (DatabaseException exception)
                {
                    return (false, $"Delete of entity of type : {nameof(Invoice)} was not Successful. \n Exception Details: {exception}.\n INVOICE IDs: {invoice.Id}.", null);
                }
            }
        }

        public override (bool isSuccessful, string operationMessage, object errorObject) DeleteEntities(Invoice[] invoices)
        {
            using (var connection = new SQLiteConnection(databasePath))
            {
                try
                {
                    connection.DeleteAllIds<Invoice>(invoices.Select(invoice => invoice.Id).Cast<object>());
                    return (true, $"Delete of collection of entity of type : {nameof(Invoice)} was Successful", null);
                }
                catch (DatabaseException exception)
                {
                    return (false, $"Delete of collection of entity of type : {nameof(Invoice)} was not Successful. \n Exception Details: {exception}.\n INVOICE IDs: {invoices}.", null);
                }
            }
        }

        #endregion
    }
}
