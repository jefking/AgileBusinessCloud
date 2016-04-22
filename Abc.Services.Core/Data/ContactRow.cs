// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='ContactRow.cs'>
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
    /// Contact Row
    /// </summary>
    [AzureDataStore("UserContact")]
    [CLSCompliant(false)]
    public class ContactRow : TableServiceEntity, IConvert<Contact>, IIdentifier<Guid>
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the ContactRow class
        /// </summary>
        public ContactRow()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ContactRow class
        /// </summary>
        /// <param name="ownerIdentifier">Owner Identifier</param>
        /// <param name="identifier">Identifier</param>
        public ContactRow(Guid ownerIdentifier, Guid identifier)
            : base(ownerIdentifier.ToString(), identifier.ToString())
        {
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != ownerIdentifier);
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != identifier);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets Identifier
        /// </summary>
        public Guid Identifier
        {
            get
            {
                return string.IsNullOrWhiteSpace(this.RowKey) ? Guid.Empty : Guid.Parse(this.RowKey);
            }
        }

        /// <summary>
        /// Gets OwnerIdentifier
        /// </summary>
        public Guid OwnerIdentifier
        {
            get
            {
                return string.IsNullOrWhiteSpace(this.PartitionKey) ? Guid.Empty : Guid.Parse(this.PartitionKey);
            }
        }

        /// <summary>
        /// Gets or sets Email
        /// </summary>
        public string Email
        {
            get;
            set;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Convert to Contact
        /// </summary>
        /// <returns>Contact</returns>
        public Contact Convert()
        {
            var owner = new User()
            {
                Identifier = this.OwnerIdentifier,
            };

            return new Contact()
            {
                Owner = owner,
                Email = this.Email,
                Identifier = this.Identifier,
            };
        }
        #endregion
    }
}
