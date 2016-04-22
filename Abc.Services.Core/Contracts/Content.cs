// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='Content.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Contracts
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Content
    /// </summary>
    [DataContract]
    public class Content : Secured, ICreatedOn, IUpdatedOn, IDeleted
    {
        #region Properties
        /// <summary>
        /// Gets or sets Created On
        /// </summary>
        [DataMember]
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets Updated On
        /// </summary>
        [DataMember]
        public DateTime UpdatedOn { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the item is Deleted
        /// </summary>
        [DataMember]
        public bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the item is Active
        /// </summary>
        [DataMember]
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets Identifier
        /// </summary>
        [DataMember]
        public Guid Id
        {
            get;
            set;
        }
        #endregion
    }
}