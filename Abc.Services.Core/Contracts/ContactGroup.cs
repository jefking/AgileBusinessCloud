// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='ContactGroup.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Contracts
{
    using System;
    using System.Runtime.Serialization;
    using Abc.Services.Data;

    /// <summary>
    /// Contact Group
    /// </summary>
    [DataContract]
    public class ContactGroup : IConvert<ContactGroupRow>, IIdentifier<Guid>
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the ContactGroup class
        /// </summary>
        /// <remarks>
        /// For Deserialization
        /// </remarks>
        public ContactGroup()
        {
        }
        #endregion

        #region Properties
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
        /// Gets or sets Owner
        /// </summary>
        [DataMember]
        public User Owner
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
        #endregion

        #region Methods
        /// <summary>
        /// Convert to Contact Group Row
        /// </summary>
        /// <returns>Contact Group Row</returns>
        [CLSCompliant(false)]
        public ContactGroupRow Convert()
        {
            return new ContactGroupRow(this.Owner.Identifier, this.Identifier)
            {
                Name = this.Name,
            };
        }
        #endregion
    }
}