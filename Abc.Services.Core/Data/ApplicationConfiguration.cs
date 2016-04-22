// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='ApplicationConfiguration.cs'>
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
    /// Application Configuration
    /// </summary>
    [AzureDataStore("ApplicationConfiguration")]
    [CLSCompliant(false)]
    public class ApplicationConfiguration : TableServiceEntity, IConvert<Abc.Services.Contracts.Configuration>, ICreatedOn
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the ApplicationConfiguration class
        /// </summary>
        /// <remarks>
        /// For Deserialization
        /// </remarks>
        public ApplicationConfiguration()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ApplicationConfiguration class
        /// </summary>
        /// <param name="applicationId">Application Identifier</param>
        /// <param name="key">Key</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Normal pattern.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Code contracts")]
        public ApplicationConfiguration(Guid applicationId, string key)
        {
            Contract.Requires<ArgumentException>(Guid.Empty != applicationId);
            Contract.Requires<ArgumentOutOfRangeException>(!string.IsNullOrWhiteSpace(key));
            Contract.Requires<ArgumentException>(key.Trim() == key);

            this.PartitionKey = applicationId.ToString();
            this.RowKey = key;
            this.CreatedOn = DateTime.UtcNow;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets Value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets Created On
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets Created By
        /// </summary>
        public Guid CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets Last Updated On
        /// </summary>
        public DateTime LastUpdatedOn { get; set; }

        /// <summary>
        /// Gets or sets Last Updated By
        /// </summary>
        public Guid LastUpdatedBy { get; set; }

        /// <summary>
        /// Gets Application Identifier
        /// </summary>
        public Guid ApplicationId
        {
            get
            {
                return string.IsNullOrWhiteSpace(this.PartitionKey) ? Guid.Empty : Guid.Parse(this.PartitionKey);
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Convert to Configuration
        /// </summary>
        /// <returns>Configuration</returns>
        public Abc.Services.Contracts.Configuration Convert()
        {
            return new Abc.Services.Contracts.Configuration()
            {
                Key = this.RowKey,
                Value = this.Value
            };
        }
        #endregion
    }
}