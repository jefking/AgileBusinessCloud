// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='ExtensionMethods.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Azure
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Reflection;
    using Microsoft.WindowsAzure.StorageClient;

    /// <summary>
    /// Extension Methods
    /// </summary>
    public static class ExtensionMethods
    {
        #region Abc.Azure.AzureTable
        /// <summary>
        /// Get Contract from Azure Table
        /// </summary>
        /// <typeparam name="T">Contract Class</typeparam>
        /// <typeparam name="TEntity">Table Entity</typeparam>
        /// <param name="table">Table</param>
        /// <param name="partitionKey">Partition Key</param>
        /// <param name="rowKey">Row Key</param>
        /// <returns>Contract Class</returns>
        [CLSCompliant(false)]
        public static T Get<T, TEntity>(this AzureTable<TEntity> table, string partitionKey, string rowKey)
            where TEntity : TableServiceEntity, IConvert<T>, new()
        {
            Contract.Requires<ArgumentNullException>(null != partitionKey);
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(rowKey));

            var item = table.QueryBy(partitionKey, rowKey);
            return null == item ? default(T) : item.Convert();
        }
        #endregion

        #region Microsoft.WindowsAzure.StorageClient.CloudBlob
        /// <summary>
        /// Blob Exists
        /// </summary>
        /// <param name="blob">Blob</param>
        /// <returns>Exists</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "CloudBlob cannot be null.")]
        public static bool Exists(this CloudBlob blob)
        {
            try
            {
                blob.FetchAttributes();
                return true;
            }
            catch (StorageClientException e)
            {
                if (e.ErrorCode == StorageErrorCode.ResourceNotFound)
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
        }
        #endregion

        #region Microsoft.WindowsAzure.StorageClient.CloudBlobContainer
        /// <summary>
        /// Container Exists
        /// </summary>
        /// <param name="container">Container</param>
        /// <returns>Exists</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Code Contracts")]
        public static bool Exists(this CloudBlobContainer container)
        {
            Contract.Requires(null != container);
            try
            {
                container.FetchAttributes();
                return true;
            }
            catch (StorageClientException e)
            {
                if (e.ErrorCode == StorageErrorCode.ResourceNotFound)
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
        }
        #endregion

        #region Microsoft.WindowsAzure.StorageClient.CloudTableClient
        /// <summary>
        /// Create Table If Not Exist
        /// </summary>
        /// <typeparam name="T">Table Service Entity</typeparam>
        /// <param name="tableStorage">Table Storage</param>
        /// <param name="entityName">Entity Name</param>
        /// <returns>Created</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Code Contract"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Code Contracts."), CLSCompliant(false)]
        public static bool CreateTableIfNotExist<T>(this CloudTableClient tableStorage, string entityName)
            where T : TableServiceEntity, new()
        {
            Contract.Requires(null != tableStorage);
            Contract.Requires<ArgumentOutOfRangeException>(!string.IsNullOrWhiteSpace(entityName));

            bool result = tableStorage.CreateTableIfNotExist(entityName);

            if (tableStorage.BaseUri.IsLoopback)
            {
                InitializeTableSchemaFromEntity(tableStorage, entityName, new T());
            }

            return result;
        }

        /// <summary>
        /// Initialize Table Schema From Entity
        /// </summary>
        /// <param name="tableStorage">Table Storage</param>
        /// <param name="entityName">Entity Name</param>
        /// <param name="entity">Entity</param>
        private static void InitializeTableSchemaFromEntity(CloudTableClient tableStorage, string entityName, TableServiceEntity entity)
        {
            Contract.Requires(null != tableStorage);

            var context = tableStorage.GetDataServiceContext();
            var now = DateTime.UtcNow;
            entity.PartitionKey = Guid.NewGuid().ToString();
            entity.RowKey = Guid.NewGuid().ToString();
            Array.ForEach(
                entity.GetType().GetProperties(
                    BindingFlags.Public | BindingFlags.Instance),
                    p =>
                    {
                        if ((p.Name != "PartitionKey") && (p.Name != "RowKey") && (p.Name != "Timestamp"))
                        {
                            if (p.PropertyType == typeof(string))
                            {
                                p.SetValue(entity, Guid.NewGuid().ToString(), null);
                            }
                            else if (p.PropertyType == typeof(DateTime))
                            {
                                p.SetValue(entity, now, null);
                            }
                        }
                    });

            context.AddObject(entityName, entity);
            context.SaveChangesWithRetries();
            context.DeleteObject(entity);
            context.SaveChangesWithRetries();
        }
        #endregion
    }
}