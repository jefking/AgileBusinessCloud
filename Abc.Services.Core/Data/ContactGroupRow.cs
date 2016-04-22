// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='ContactGroupRow.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Data
{
    using Abc.Azure;
    using Abc.Services.Contracts;
    using Microsoft.WindowsAzure.StorageClient;
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Contact Group Row
    /// </summary>
    [AzureDataStore("ContactGroup")]
    [CLSCompliant(false)]
    public class ContactGroupRow : TableServiceEntity, IConvert<ContactGroup>, IIdentifier<Guid>
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the ContactGroupRow class
        /// </summary>
        /// <remarks>
        /// For Deserialization
        /// </remarks>
        public ContactGroupRow()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ContactGroupRow class
        /// </summary>
        /// <param name="ownerIdentifier">Owner Identifier</param>
        /// <param name="identifier">Identifier</param>
        public ContactGroupRow(Guid ownerIdentifier, Guid identifier)
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
        /// Gets or sets Name
        /// </summary>
        public string Name
        {
            get;
            set;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Convert to Contact Group
        /// </summary>
        /// <returns>Contact Group</returns>
        public ContactGroup Convert()
        {
            var owner = new User()
            {
                Identifier = this.OwnerIdentifier,
            };

            return new ContactGroup()
            {
                Owner = owner,
                Name = this.Name,
                Identifier = this.Identifier,
            };
        }
        #endregion
    }
}