using SQLiteOneDriveInvoiceSample.Database.Entities;
using SQLiteOneDriveInvoiceSample.Database.Exceptions;
using SQLite;

using SQLiteNetExtensions.Extensions;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SQLiteOneDriveInvoiceSample.Database.Services
{
    public abstract class BaseDBService<T>
    {
        #region Properties

        public string TypeName { get; set; }
        protected string databasePath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "SQLiteOneDriveInvoiceSample.db");

        #endregion Properties

        #region Constructor(s)

        public BaseDBService() => TypeName = typeof(T).Name;
        #endregion Constructor(s)

        #region CreateOperation(s)

        public virtual (bool isSuccessful, string operationMessage, object errorObject) AddEntity(T entity)
        {
            using (var connection = new SQLiteConnection(databasePath))
            {
                try
                {
                    connection.InsertWithChildren(entity, recursive: true);
                    return (true, $"Insert of type : {TypeName} with Children was Successful.", null);
                }
                catch (DatabaseException exception)
                {
                    return (false, $"Insert of type : {TypeName} with Children was not Successful. \n Exception Details: {exception} ", entity);
                }
            }
        }

        public virtual (bool isSuccessful, string operationMessage, object errorObject) AddEntities(T[] entities)
        {
            using (var connection = new SQLiteConnection(databasePath))
            {
                try
                {
                    connection.InsertAllWithChildren(entities, recursive: true);
                    return (true, $"Insert of a collection of type : {TypeName} with Children was Successful.", null);
                }
                catch (DatabaseException exception)
                {
                    return (false, $"Insert of a collection of type : {TypeName} with Children was not Successful. \n Exception Details: {exception} ", entities);
                }
            }
        }

        #endregion CreateOperation(s)

        #region UpdateOperation(s)

        public virtual (bool isSuccessful, string operationMessage, object errorObject) UpdateEntity(T entity)
        {
            using (var connection = new SQLiteConnection(databasePath))
            {
                try
                {
                    connection.InsertOrReplaceWithChildren(entity);
                    return (true, $"Insert of a collection of type : {TypeName} with Children was Successful.", null);
                }
                catch (DatabaseException exception)
                {
                    return (false, $"Update of type : {TypeName} with Children was not Successful. \n Exception Details: {exception} ", entity);
                }
            }
        }

        public virtual (bool isSuccessful, string operationMessage, object errorObject) UpdateEntities(T[] entities)
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
                    return (false, $"Update of a collection of type : {TypeName} with Children was not Successful. \n Exception Details: {exception} ", entities);
                }
            }
        }

        #endregion UpdateOperation(s)

        #region ReadOperation(s)

        public abstract (bool isSuccessful, string operationMessage, T entity) GetEntity(Guid entityId);

        public abstract (bool isSuccessful, string operationMessage, List<T> entities) GetEntities(Guid[] entityIds);

        #endregion ReadOperation(s)

        #region DeleteOperation(s)

        public abstract (bool isSuccessful, string operationMessage, object errorObject) DeleteEntity(T entity);

        public abstract (bool isSuccessful, string operationMessage, object errorObject) DeleteEntities(T[] entities);

        #endregion DeleteOperation(s)
    }
}
