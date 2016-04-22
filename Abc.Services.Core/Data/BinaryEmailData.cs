// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='BinaryEmailData.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Data
{
    using System;
    using Abc.Azure;

    /// <summary>
    /// Binary Email Data
    /// </summary>
    [AzureDataStore("BinaryEmail")]
    [CLSCompliant(false)]
    public class BinaryEmailData : EmailData
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the BinaryEmailData class
        /// </summary>
        public BinaryEmailData()
        {
        }

        /// <summary>
        /// Initializes a new instance of the BinaryEmailData class
        /// </summary>
        /// <param name="applicationId">Application Id</param>
        public BinaryEmailData(Guid applicationId)
            : base(applicationId)
        {
        }
        #endregion

        #region Members
        /// <summary>
        /// Gets or sets Subject
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Binary data.")]
        public byte[] RawMessage { get; set; }
        #endregion
    }
}