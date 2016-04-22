// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='ApplicationData.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services
{
    using System;
    using System.Diagnostics.Contracts;
    using Microsoft.WindowsAzure.StorageClient;

    /// <summary>
    /// Application Data, for uniquely identifying data and relating it to an application
    /// </summary>
    [CLSCompliant(false)]
    public class ApplicationData : TableServiceEntity
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the ApplicationData class
        /// </summary>
        /// <remarks>
        /// For Deserialization
        /// </remarks>
        protected ApplicationData()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ApplicationData class
        /// </summary>
        /// <param name="applicationId">Application Identifier</param>
        protected ApplicationData(Guid applicationId)
            : base(applicationId.ToString(), Guid.NewGuid().ToString())
        {
            Contract.Requires<ArgumentException>(Guid.Empty != applicationId);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets Identifier
        /// </summary>
        public Guid Id
        {
            get
            {
                return string.IsNullOrWhiteSpace(this.RowKey) ? Guid.Empty : Guid.Parse(this.RowKey);
            }
        }

        /// <summary>
        /// Gets Application Identifier
        /// </summary>
        public Guid ApplicationId
        {
            get
            {
                return string.IsNullOrWhiteSpace(this.PartitionKey) ? Guid.Empty : Guid.Parse(this.PartitionKey);
            }
        }
        #endregion
    }
}