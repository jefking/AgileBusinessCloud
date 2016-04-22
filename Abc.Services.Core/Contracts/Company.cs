// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='Company.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Contracts
{
    using System;
    using System.Runtime.Serialization;
    using Abc.Services.Data;

    /// <summary>
    /// Company
    /// </summary>
    [DataContract]
    [Serializable]
    public class Company : IConvert<CompanyRow>, IIdentifier<Guid>, IDeleted
    {
        #region Properties
        /// <summary>
        /// Gets or sets Owner
        /// </summary>
        [DataMember]
        public User Owner
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Identifier
        /// </summary>
        [DataMember]
        public Guid Identifier
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Name
        /// </summary>
        [DataMember]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether Active
        /// </summary>
        [DataMember]
        public bool Active
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether Deleted
        /// </summary>
        [DataMember]
        public bool Deleted
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Last Updated On
        /// </summary>
        [DataMember]
        public DateTime EditedOn
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Last Updated By
        /// </summary>
        [DataMember]
        public User EditedBy
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Created On
        /// </summary>
        [DataMember]
        public DateTime CreatedOn
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Created By
        /// </summary>
        [DataMember]
        public User CreatedBy
        {
            get;
            set;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Convert
        /// </summary>
        /// <returns>Company Row</returns>
        [CLSCompliant(false)]
        public CompanyRow Convert()
        {
            var row = new CompanyRow(this.Owner.Identifier)
            {
                Active = this.Active,
                CreatedByIdentifier = this.CreatedBy.Identifier,
                CreatedOn = this.CreatedOn,
                Deleted = this.Deleted,
                EditedByIdentifier = this.EditedBy.Identifier,
                EditedOn = this.EditedOn,
                Name = this.Name,
            };

            row.RowKey = Guid.Empty == this.Identifier ? row.RowKey : this.Identifier.ToString();

            return row;
        }
        #endregion
    }
}