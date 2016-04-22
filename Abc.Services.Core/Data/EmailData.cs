// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='EmailData.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Data
{
    using System;
    using System.Diagnostics.Contracts;
    
    /// <summary>
    /// Email Data
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1012:AbstractTypesShouldNotHaveConstructors", Justification = "Default constructor for Serialization.")]
    [CLSCompliant(false)]
    public abstract class EmailData : ApplicationData
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the EmailData class
        /// </summary>
        public EmailData()
        {
        }

        /// <summary>
        /// Initializes a new instance of the EmailData class
        /// </summary>
        /// <param name="applicationId">Application Id</param>
        protected EmailData(Guid applicationId)
            : base(applicationId)
        {
        }
        #endregion

        #region Members
        /// <summary>
        /// Gets or sets Sender
        /// </summary>
        public string Sender { get; set; }

        /// <summary>
        /// Gets or sets Recipient
        /// </summary>
        public string Recipient { get; set; }
        #endregion
    }
}