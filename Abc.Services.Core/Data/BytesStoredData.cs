// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='BytesStoredData.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services
{
    using System;
    using System.Diagnostics.Contracts;
    using Abc.Azure;
    using Abc.Services.Contracts;
    using Microsoft.WindowsAzure.StorageClient;

    /// <summary>
    /// Bytes Stored Data
    /// </summary>
    [CLSCompliant(false)]
    [AzureDataStore("ApplicationDataAccount")]
    public class BytesStoredData : TableServiceEntity, IConvert<BytesStored>
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the BytesStoredData class
        /// </summary>
        /// <remarks>
        /// For deserialization
        /// </remarks>
        public BytesStoredData()
        {
        }

        /// <summary>
        /// Initializes a new instance of the BytesStoredData class
        /// </summary>
        /// <param name="applicationId">Application Identifier</param>
        public BytesStoredData(Guid applicationId)
            : base(applicationId.ToString(), Guid.NewGuid().ToString())
        {
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != applicationId);

            this.OccurredOn = DateTime.UtcNow;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets Application Identifier
        /// </summary>
        public Guid ApplicationId
        {
            get
            {
                return string.IsNullOrWhiteSpace(this.PartitionKey) ? Guid.Empty : Guid.Parse(PartitionKey);
            }
        }

        /// <summary>
        /// Gets or sets Data Cost Type
        /// </summary>
        public int DataCostType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Occurred On
        /// </summary>
        public DateTime OccurredOn
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Bytes
        /// </summary>
        public int Bytes
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Object Type
        /// </summary>
        public string ObjectType
        {
            get;
            set;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Convert
        /// </summary>
        /// <returns>Bytes Stored</returns>
        public BytesStored Convert()
        {
            return new BytesStored()
            {
                ApplicationId = this.ApplicationId,
                Bytes = this.Bytes,
                DataCostType = this.DataCostType,
                ObjectType = this.ObjectType,
                OccurredOn = this.OccurredOn,
            };
        }
        #endregion
    }
}