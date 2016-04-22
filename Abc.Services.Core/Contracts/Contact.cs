// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='Contact.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Contracts
{
    using System;
    using System.Runtime.Serialization;
    using Abc.Services.Data;

    /// <summary>
    /// Contact
    /// </summary>
    [DataContract]
    [Serializable]
    public class Contact : IConvert<ContactRow>, IIdentifier<Guid>
    {
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
        /// Gets or sets Email
        /// </summary>
        [DataMember]
        public string Email
        {
            get;
            set;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Convert to Contact Row
        /// </summary>
        /// <returns>Contact Row</returns>
        [CLSCompliant(false)]
        public ContactRow Convert()
        {
            return new ContactRow(this.Owner.Identifier, this.Identifier)
            {
                Email = this.Email,
            };
        }
        #endregion
    }
}