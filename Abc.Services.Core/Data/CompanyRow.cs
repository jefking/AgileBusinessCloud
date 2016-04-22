// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='CompanyRow.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Data
{
    using System;
    using System.Diagnostics.Contracts;
    using Abc.Azure;
    using Abc.Services.Contracts;
    using Microsoft.WindowsAzure.StorageClient;

    /// <summary>
    /// Company Rows
    /// </summary>
    [AzureDataStore("Company")]
    [CLSCompliant(false)]
    public class CompanyRow : TableServiceEntity, IConvert<Company>, IIdentifier<Guid>, IDeleted, ICreatedOn
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the CompanyRow class
        /// </summary>
        public CompanyRow()
        {
        }

        /// <summary>
        /// Initializes a new instance of the CompanyRow class
        /// </summary>
        /// <param name="ownerIdentifier">Owner Identifier</param>
        public CompanyRow(Guid ownerIdentifier)
            : base(ownerIdentifier.ToString(), Guid.NewGuid().ToString())
        {
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != ownerIdentifier);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets OwnerIdentifier
        /// </summary>
        public Guid OwnerIdentifier
        {
            get
            {
                return Guid.Parse(this.PartitionKey);
            }
        }

        /// <summary>
        /// Gets Identifier
        /// </summary>
        public Guid Identifier
        {
            get
            {
                return Guid.Parse(this.RowKey);
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
        /// Gets or sets Last Updated On
        /// </summary>
        public DateTime EditedOn
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Last Updated By Identifier
        /// </summary>
        public Guid EditedByIdentifier
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
        /// Gets or sets Created By Identifier
        /// </summary>
        public Guid CreatedByIdentifier
        {
            get;
            set;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Convert To Company
        /// </summary>
        /// <returns>Company</returns>
        public Company Convert()
        {
            var owner = new User()
            {
                Identifier = this.OwnerIdentifier,
            };

            var creator = new User()
            {
                Identifier = this.CreatedByIdentifier,
            };

            var editor = new User()
            {
                Identifier = this.EditedByIdentifier,
            };

            return new Company()
            {
                Active = this.Active,
                CreatedBy = creator,
                CreatedOn = this.CreatedOn,
                Deleted = this.Deleted,
                EditedBy = editor,
                EditedOn = this.EditedOn,
                Name = this.Name,
                Owner = owner,
                Identifier = this.Identifier,
            };
        }
        #endregion
    }
}