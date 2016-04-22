// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='ApplicationInfoData.cs'>
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
    [AzureDataStore("ApplicationInformation")]
    public class ApplicationInfoData : TableServiceEntity, IConvert<ApplicationInformation>, ILoad<UserData>, ICreatedOn, IDeleted
    {
        #region Members
        /// <summary>
        /// Partition Key
        /// </summary>
        public static readonly string Key = string.Empty;

        /// <summary>
        /// Public Key
        /// </summary>
        private string publicKey = null;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the ApplicationInfoData class
        /// </summary>
        /// <remarks>
        /// For Deserialization
        /// </remarks>
        public ApplicationInfoData()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ApplicationInfoData class
        /// </summary>
        /// <param name="applicationId">Application Identifier</param>
        public ApplicationInfoData(Guid applicationId)
            : base(ApplicationInfoData.Key, applicationId.ToString())
        {
            Contract.Requires<ArgumentException>(Guid.Empty != applicationId);
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
                return string.IsNullOrWhiteSpace(this.RowKey) ? Guid.Empty : Guid.Parse(this.RowKey);
            }
        }

        /// <summary>
        /// Gets or sets Key
        /// </summary>
        public string PublicKey
        {
            get
            {
                return string.IsNullOrWhiteSpace(this.publicKey) ? this.ApplicationId.ToAscii85().GetHexMD5() : this.publicKey;
            }
            set
            {
                this.publicKey = value;
            }
        }

        /// <summary>
        /// Gets or sets Name
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Environment
        /// </summary>
        public string Environment
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Description
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Created On
        /// </summary>
        public DateTime CreatedOn
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Created By
        /// </summary>
        public Guid CreatedBy
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Last Updated On
        /// </summary>
        public DateTime LastUpdatedOn
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Last Updated By
        /// </summary>
        public Guid LastUpdatedBy
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether Active
        /// </summary>
        public bool Active
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether Deleted
        /// </summary>
        public bool Deleted
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Owner
        /// </summary>
        public Guid Owner
        {
            get;
            set;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Converts Data class to Application Information
        /// </summary>
        /// <returns>Application Information</returns>
        public ApplicationInformation Convert()
        {
            return new ApplicationInformation()
            {
                Name = this.Name,
                Environment = this.Environment,
                Description = this.Description,
                Deleted = this.Deleted,
                Identifier = this.ApplicationId,
                Active = this.Active,
                OwnerId = this.Owner,
                PublicKey = this.PublicKey,
            };
        }

        /// <summary>
        /// Load
        /// </summary>
        /// <param name="data">User</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Code contracts")]
        public void Load(UserData data)
        {
            this.LastUpdatedBy = data.Id;
            this.CreatedBy = data.Id;
        }
        #endregion
    }
}