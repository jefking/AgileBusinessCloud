// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='LogQuery.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Runtime.Serialization;
    using Abc.Azure;

    /// <summary>
    /// Log Query
    /// </summary>
    [DataContract]
    public class LogQuery : ApplicationQuery, IIdentifier<Guid?>
    {
        #region Properties
        /// <summary>
        /// Gets or sets Top
        /// </summary>
        [DataMember]
        public int? Top
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets To
        /// </summary>
        [DataMember]
        public DateTime? To
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets From
        /// </summary>
        [DataMember]
        public DateTime? From
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Identifier
        /// </summary>
        [DataMember]
        public Guid? Identifier
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Deep
        /// </summary>
        [DataMember]
        public bool? Deep
        {
            get;
            set;
        }

        /// <summary>
        /// Gets Partition Key
        /// </summary>
        [IgnoreDataMember]
        public string PartitionKey
        {
            get
            {
                Contract.Ensures(!string.IsNullOrWhiteSpace(Contract.Result<string>()));

                return this.ApplicationIdentifier.ToString();
            }
        }

        /// <summary>
        /// Gets Partition Key
        /// </summary>
        [IgnoreDataMember]
        public string RowKey
        {
            get
            {
                return this.Identifier.HasValue ? this.Identifier.Value.ToString() : null;
            }
        }

        /// <summary>
        /// Gets Is Unique
        /// </summary>
        [IgnoreDataMember]
        public bool IsUnique
        {
            get
            {
                return this.Identifier.HasValue;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize Log Query
        /// </summary>
        public void Initialize()
        {
            this.From = this.From.HasValue ? this.From.Value : AzureTable<MessageData>.Minimum;
            this.To = this.To.HasValue ? this.To.Value : DateTime.UtcNow;
            this.Top = this.Top.HasValue ? this.Top.Value : 1000;
        }

        /// <summary>
        /// Filter Data Set based on Query Criteria
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="list">List</param>
        /// <param name="query">Criteria</param>
        /// <returns>Subset</returns>
        public IEnumerable<T> Filter<T>(IEnumerable<T> list)
            where T : LogItem
        {
            Contract.Requires<ArgumentNullException>(null != list);

            Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);

            return (from data in list
                    where data.OccurredOn > this.From.Value
                     && data.OccurredOn < this.To.Value
                    orderby data.OccurredOn descending
                    select data).Take(this.Top.Value).ToList();
        }
        #endregion
    }
}
