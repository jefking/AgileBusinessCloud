// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='LogData.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Log Data
    /// </summary>
    [CLSCompliant(false)]
    public class LogData : ApplicationData
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the LogData class
        /// </summary>
        /// <remarks>
        /// For Deserialization
        /// </remarks>
        protected LogData()
        {
        }

        /// <summary>
        /// Initializes a new instance of the LogData class
        /// </summary>
        /// <param name="applicationId">Application Identifier</param>
        protected LogData(Guid applicationId)
            : base(applicationId)
        {
            Contract.Requires<ArgumentException>(Guid.Empty != applicationId);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets Occured On
        /// </summary>
        public DateTime OccurredOn { get; set; }

        /// <summary>
        /// Gets or sets Machine Name
        /// </summary>
        public string MachineName { get; set; }

        /// <summary>
        /// Gets or sets Instance Id
        /// </summary>
        public string DeploymentId { get; set; }

        /// <summary>
        /// Gets or sets Message
        /// </summary>
        public string Message { get; set; }
        #endregion
    }
}