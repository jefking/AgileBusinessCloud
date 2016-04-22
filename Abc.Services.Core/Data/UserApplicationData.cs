// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='UserApplicationData.cs'>
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
    /// Application Information Data
    /// </summary>
    [CLSCompliant(false)]
    [AzureDataStore("ApplicationUser")]
    public class UserApplicationData : TableServiceEntity, IConvert<UserApplication>, IDeleted, ICreatedOn
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the UserApplicationData class
        /// </summary>
        /// <remarks>
        /// For Deserialization
        /// </remarks>
        public UserApplicationData()
        {
        }

        /// <summary>
        /// Initializes a new instance of the UserApplicationData class
        /// </summary>
        /// <param name="userId">User Identifier</param>
        /// <param name="applicationId">Application Id</param>
        public UserApplicationData(Guid userId, Guid applicationId)
        {
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != userId);

            this.PartitionKey = userId.ToString();
            this.RowKey = applicationId.ToString();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets User Id
        /// </summary>
        public Guid UserId
        {
            get
            {
                return string.IsNullOrWhiteSpace(this.PartitionKey) ? Guid.Empty : Guid.Parse(this.PartitionKey);
            }
        }

        /// <summary>
        /// Gets Application Identifier
        /// </summary>
        public Guid ApplicationId
        {
            get
            {
                return string.IsNullOrWhiteSpace(this.RowKey) ? Guid.Empty : Guid.Parse(this.RowKey);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Active
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Deleted
        /// </summary>
        public bool Deleted { get; set; }

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
        #endregion

        #region Methods
        /// <summary>
        /// Convert to User Application
        /// </summary>
        /// <returns>User Application</returns>
        public UserApplication Convert()
        {
            var user = new User()
            {
                Identifier = this.UserId,
            };
            var application = new Application()
            {
                Identifier = this.ApplicationId,
            };

            return new UserApplication()
            {
                Application = application,
                Active = this.Active,
                Deleted = this.Deleted,
                User = user,
            };
        }
        #endregion
    }
}