// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='ContentData.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Content Data
    /// </summary>
    [CLSCompliant(false)]
    public abstract class ContentData : ApplicationData, ICreatedOn, IDeleted, IUpdatedOn
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the ContentData class
        /// </summary>
        /// <remarks>
        /// For Deserialization
        /// </remarks>
        protected ContentData()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ContentData class
        /// </summary>
        /// <param name="applicationId">Application Identifier</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Calling base constructor")]
        protected ContentData(Guid applicationId)
            : base(applicationId)
        {
            Contract.Requires<ArgumentException>(Guid.Empty != applicationId);

            this.CreatedOn = DateTime.UtcNow;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets Created On
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets Updated On
        /// </summary>
        public DateTime UpdatedOn { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the item is Deleted
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the item is Active
        /// </summary>
        public bool Active { get; set; }
        #endregion
    }
}