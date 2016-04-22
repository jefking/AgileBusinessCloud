// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='PlaintextEmailData.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Data
{
    using System;
    using Abc.Azure;

    /// <summary>
    /// Plaintext Email Data
    /// </summary>
    [AzureDataStore("PlaintextEmail")]
    [CLSCompliant(false)]
    public class PlaintextEmailData : EmailData
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the PlaintextEmailData class
        /// </summary>
        public PlaintextEmailData()
        {
        }

        /// <summary>
        /// Initializes a new instance of the PlaintextEmailData class
        /// </summary>
        /// <param name="applicationId">Application Id</param>
        public PlaintextEmailData(Guid applicationId)
            : base(applicationId)
        {
        }
        #endregion

        #region Members
        /// <summary>
        /// Gets or sets Subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets Message
        /// </summary>
        public string Message { get; set; }
        #endregion
    }
}