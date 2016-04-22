// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='RoleRow.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Data
{
    using System;
    using System.Diagnostics.Contracts;
    using Abc.Azure;

    /// <summary>
    /// Role Row
    /// </summary>
    [CLSCompliant(false)]
    [AzureDataStore("UserRole")]
    public class RoleRow : ApplicationData
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the RoleRow class
        /// </summary>
        /// <remarks>
        /// For Deserialization
        /// </remarks>
        public RoleRow()
        {
        }

        /// <summary>
        /// Initializes a new instance of the RoleRow class
        /// </summary>
        /// <param name="applicationId">Application Identifier</param>
        public RoleRow(Guid applicationId)
            : base(applicationId)
        {
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != applicationId);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets Identifier
        /// </summary>
        public Guid UserIdentifier
        {
            get;
            set;
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
    }
}