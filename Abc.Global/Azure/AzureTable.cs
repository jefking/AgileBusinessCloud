// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='AzureTable.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Azure
{
    using System;
    using System.Collections.Generic;
    using System.Data.Services.Client;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Linq.Expressions;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.StorageClient;

    /// <summary>
    /// Azure Table
    /// </summary>
    /// <typeparam name="TEntity">Table Service Entity</typeparam>
    [CLSCompliant(false)]
    public class AzureTable<TEntity>
        where TEntity : TableServiceEntity, new()
    {
        #region Members
        /// <summary>
        /// Azure doesn't permit DateTime Queries to be less than January 1st, 1753.
        /// </summary>
        private static readonly DateTime minimumDate = new DateTime(1800, 1, 1);

        /// <summary>
        /// Table Name
        /// </summary>
        private readonly string tableName;

        /// <summary>
        /// Account
        /// </summary>
        private readonly CloudStorageAccount account;

        /// <summary>
        /// Validator
        /// </summary>
        private readonly IStoreValidator<TEntity> validator = null;

        /// <summary>
        /// Table Query Limit
        /// </summary>
        private const int TableQueryLimit = 1000;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the AzureTable class
        /// </summary>
        /// <param name="account">Account</param>
        public AzureTable(CloudStorageAccount account)
            : this(account, null)
        {
            Contract.Requires<ArgumentNullException>(null != account);
        }

        /// <summary>
        /// Initializes a new instance of the AzureTable class
        /// </summary>
        /// <param name="account">Account</param>
        /// <param name="validator">Validator</param>
        public AzureTable(CloudStorageAccount account, IStoreValidator<TEntity> validator)
        {
            Contract.Requires<ArgumentNullException>(null != account);

            var table = typeof(TEntity).GetCustomAttribute<AzureDataStoreAttribute>(false);
            this.tableName = null == table ? typeof(TEntity).Name : table.Name;

            this.account = account;
            this.validator = validator;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets Azure doesn't permit DateTime Queries to be less than January 1st, 1753.
        /// </summary>
        public static DateTime Minimum
        {
            get
            {
                return minimumDate;
            }
        }

        /// <summary>
        /// Gets Azure doesn't permit DateTime Queries to be less than January 1st, 1753.
        /// </summary>
        public DateTime MinimumDate
        {
            get
            {
                return minimumDate;
            }
        }

        /// <summary>
        /// Gets Query
        /// </summary>
        public IQueryable<TEntity> Query
        {
            get
            {
                return this.TableQuery;
            }
        }

        /// <summary>
        /// Gets Query
        /// </summary>
        private CloudTableQuery<TEntity> TableQuery
        {
            get
            {
                var context = this.CreateContext();
                return context.CreateQuery<TEntity>(this.tableName).AsTableServiceQuery();
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Create Table If Not Exists
        /// </summary>
        public void EnsureExist()
        {
            var cloudTableClient = new CloudTableClient(this.account.TableEndpoint.ToString(), this.account.Credentials);
            cloudTableClient.CreateTableIfNotExist<TEntity>(this.tableName);
        }

        /// <summary>
        /// Delete Table If Exists
        /// </summary>
        public void DeleteIfExist()
        {
            var cloudTableClient = new CloudTableClient(this.account.TableEndpoint.ToString(), this.account.Credentials);
            cloudTableClient.DeleteTableIfExist(this.tableName);
        }

        /// <summary>
        /// Add Entity
        /// </summary>
        /// <param name="entity">Object</param>
        public void AddEntity(TEntity entity)
        {
            Contract.Requires<ArgumentNullException>(null != entity);
            Contract.Requires<ArgumentOutOfRangeException>(!string.IsNullOrWhiteSpace(entity.RowKey));
            Contract.Requires<ArgumentOutOfRangeException>(null != entity.PartitionKey);

            if (null == this.validator || this.validator.ValidateForAdd(entity))
            {
                this.AddEntity(new[] { entity });
            }
            else
            {
                throw new ArgumentException("Validation failed.");
            }
        }

        /// <summary>
        /// Add Entity
        /// </summary>
        /// <param name="entities">Objects</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Code Contracts")]
        public void AddEntity(IEnumerable<TEntity> entities)
        {
            Contract.Requires<ArgumentNullException>(null != entities);
            Contract.Requires<ArgumentNullException>(Contract.ForAll<TEntity>(entities, obj => null != obj));
            Contract.Requires<ArgumentOutOfRangeException>(Contract.ForAll<TEntity>(entities, obj => !string.IsNullOrWhiteSpace(obj.RowKey)));
            Contract.Requires<ArgumentOutOfRangeException>(Contract.ForAll<TEntity>(entities, obj => null != obj.PartitionKey));

            if (null != this.validator)
            {
                foreach (var entity in entities)
                {
                    if (!this.validator.ValidateForAdd(entity))
                    {
                        throw new ArgumentException("Validation failed.");
                    }
                }
            }

            var context = this.CreateContext();

            foreach (var obj in entities)
            {
                context.AddObject(this.tableName, obj);
            }

            var saveChangesOptions = entities.Distinct(new PartitionKeyComparer()).Count() == 1 ? SaveChangesOptions.Batch : SaveChangesOptions.None;
            context.SaveChanges(saveChangesOptions);
        }

        /// <summary>
        /// Add or Update Entity
        /// </summary>
        /// <param name="entity">Object</param>
        public void AddOrUpdateEntity(TEntity entity)
        {
            Contract.Requires<ArgumentNullException>(null != entity);
            Contract.Requires<ArgumentOutOfRangeException>(!string.IsNullOrWhiteSpace(entity.RowKey));
            Contract.Requires<ArgumentOutOfRangeException>(null != entity.PartitionKey);

            if (null == this.validator || this.validator.ValidateForAddOrUpdate(entity))
            {
                this.AddOrUpdateEntity(new[] { entity });
            }
            else
            {
                throw new ArgumentException("Validation failed.");
            }
        }

        /// <summary>
        /// Add or Update Entities
        /// </summary>
        /// <param name="entities">Objects</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Code Contracts")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Safety")]
        public void AddOrUpdateEntity(IEnumerable<TEntity> entities)
        {
            Contract.Requires<ArgumentNullException>(null != entities);
            Contract.Requires<ArgumentNullException>(Contract.ForAll<TEntity>(entities, obj => null != obj));
            Contract.Requires<ArgumentOutOfRangeException>(Contract.ForAll<TEntity>(entities, obj => !string.IsNullOrWhiteSpace(obj.RowKey)));
            Contract.Requires<ArgumentOutOfRangeException>(Contract.ForAll<TEntity>(entities, obj => null != obj.PartitionKey));

            if (null != this.validator)
            {
                foreach (var entity in entities)
                {
                    if (!this.validator.ValidateForAddOrUpdate(entity))
                    {
                        throw new ArgumentException("Validation failed.");
                    }
                }
            }

            var context = this.CreateContext();
            foreach (var obj in entities)
            {
                //This is temporary, while the emulator doesn't support the merge functionality.
                if (account == CloudStorageAccount.DevelopmentStorageAccount)
                {
                    var entityCheck = this.QueryBy(obj.PartitionKey, obj.RowKey);

                    if (entityCheck == null)
                    {
                        context.AddObject(this.tableName, obj);
                    }
                    else
                    {
                        context.Detach(entityCheck);
                        context.AttachTo(this.tableName, obj, "*");
                        context.UpdateObject(obj);
                    }
                }
                else
                {
                    context.AttachTo(this.tableName, obj, null);
                    context.UpdateObject(obj);
                }

                context.SaveChanges();
            }
        }

        /// <summary>
        /// Delete Entity
        /// </summary>
        /// <param name="entity">Object</param>
        public void DeleteEntity(TEntity entity)
        {
            Contract.Requires<ArgumentNullException>(null != entity);
            Contract.Requires<ArgumentOutOfRangeException>(!string.IsNullOrWhiteSpace(entity.RowKey));
            Contract.Requires<ArgumentOutOfRangeException>(null != entity.PartitionKey);
            
            this.DeleteEntity(new[] { entity });
        }

        /// <summary>
        /// Delete Entities
        /// </summary>
        /// <param name="entities">Objects</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Code Contracts")]
        public void DeleteEntity(IEnumerable<TEntity> entities)
        {
            Contract.Requires<ArgumentNullException>(null != entities);
            Contract.Requires<ArgumentNullException>(Contract.ForAll<TEntity>(entities, obj => null != obj));
            Contract.Requires<ArgumentOutOfRangeException>(Contract.ForAll<TEntity>(entities, obj => !string.IsNullOrWhiteSpace(obj.RowKey)));
            Contract.Requires<ArgumentOutOfRangeException>(Contract.ForAll<TEntity>(entities, obj => null != obj.PartitionKey));

            var context = this.CreateContext();

            foreach (var obj in entities)
            {
                context.AttachTo(this.tableName, obj, "*");
                context.DeleteObject(obj);
            }

            var options = entities.Count() > 1 ? SaveChangesOptions.Batch : SaveChangesOptions.None;
            context.SaveChanges();
        }

        /// <summary>
        /// Query By, PartitionKey and RowKey
        /// </summary>
        /// <param name="partitionKey">Partition Key</param>
        /// <param name="rowKey">Row Key</param>
        /// <returns>Entities</returns>
        public TEntity QueryBy(string partitionKey, string rowKey)
        {
            Contract.Requires<ArgumentOutOfRangeException>(!string.IsNullOrWhiteSpace(rowKey));
            Contract.Requires<ArgumentOutOfRangeException>(null != partitionKey);

            return (from data in this.Query
                    where data.PartitionKey == partitionKey
                        && data.RowKey == rowKey
                    select data).FirstOrDefault();
        }

        /// <summary>
        /// Query By Partition
        /// </summary>
        /// <param name="partitionKey">Partition Key</param>
        /// <returns>Entities</returns>
        public IQueryable<TEntity> QueryByPartition(string partitionKey)
        {
            Contract.Requires<ArgumentOutOfRangeException>(null != partitionKey);

            return from data in this.Query
                   where data.PartitionKey == partitionKey
                   select data;
        }

        /// <summary>
        /// Query By
        /// </summary>
        /// <param name="expression">Expression (Where Clause)</param>
        /// <param name="take">Take</param>
        /// <returns>Entities</returns>
        public IQueryable<TEntity> QueryBy(Expression<Func<TEntity, bool>> expression, int take = TableQueryLimit)
        {
            Contract.Requires<ArgumentNullException>(null != expression);
            Contract.Requires<ArgumentOutOfRangeException>(0 < take);

            var query = this.TableQuery.Where(expression).AsTableServiceQuery();

            if (take != TableQueryLimit)
            {
                query = query.Take(take).AsTableServiceQuery();
            }

            return query;
        }

        /// <summary>
        /// Query By Row
        /// </summary>
        /// <remarks>
        /// Expensive call, Partition Key not taken into account. Prefer Query By Partition and Row
        /// </remarks>
        /// <param name="rowKey">Row Key</param>
        /// <param name="take">Take</param>
        /// <returns>Entities</returns>
        public IQueryable<TEntity> QueryByRow(string rowKey, int take = TableQueryLimit)
        {
            Contract.Requires<ArgumentOutOfRangeException>(!string.IsNullOrWhiteSpace(rowKey));
            Contract.Requires<ArgumentOutOfRangeException>(0 < take);

            return this.QueryBy(d => d.RowKey == rowKey, take);
        }

        /// <summary>
        /// Delete Entity By Partion Key and Row Key
        /// </summary>
        /// <param name="partitionKey">Partition Key</param>
        /// <param name="rowKey">Row Key</param>
        public void DeleteBy(string partitionKey, string rowKey)
        {
            Contract.Requires<ArgumentOutOfRangeException>(null != partitionKey);
            Contract.Requires<ArgumentOutOfRangeException>(!string.IsNullOrWhiteSpace(rowKey));

            var entity = this.QueryBy(partitionKey, rowKey);
            if (null != entity)
            {
                this.DeleteEntity(entity);
            }
        }

        /// <summary>
        /// Delete By Partition
        /// </summary>
        /// <param name="partitionKey">Partition Key</param>
        public void DeleteByPartition(string partitionKey)
        {
            Contract.Requires<ArgumentOutOfRangeException>(null != partitionKey);

            var entities = this.QueryByPartition(partitionKey);
            if (null != entities)
            {
                this.DeleteEntity(entities);
            }
        }

        /// <summary>
        /// Delete By Row
        /// </summary>
        /// <remarks>
        /// Expensive call, Partition Key not taken into account. Prefer DeleteBy
        /// </remarks>
        /// <param name="rowKey">Row Key</param>
        public void DeleteByRow(string rowKey)
        {
            Contract.Requires<ArgumentOutOfRangeException>(!string.IsNullOrWhiteSpace(rowKey));

            var entities = this.QueryByRow(rowKey);
            if (null != entities)
            {
                this.DeleteEntity(entities);
            }
        }

        /// <summary>
        /// Create Context
        /// </summary>
        /// <returns>Table Service Context</returns>
        private TableServiceContext CreateContext()
        {
            var context = new TableServiceContext(this.account.TableEndpoint.ToString(), this.account.Credentials)
            {
                ResolveType = t => typeof(TEntity),
                RetryPolicy = RetryPolicies.RetryExponential(RetryPolicies.DefaultClientRetryCount, RetryPolicies.DefaultClientBackoff),
                IgnoreResourceNotFoundException = true,
            };

            return context;
        }

        /// <summary>
        /// Contract Invariant
        /// </summary>
        [ContractInvariantMethod]
        private void ContractInvariant()
        {
            Contract.Invariant(!string.IsNullOrWhiteSpace(this.tableName));
        }
        #endregion
    }
}