// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='ErrorData.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services
{
    using System;
    using System.Diagnostics.Contracts;
    using Abc.Azure;
    using Abc.Services.Contracts;

    /// <summary>
    /// Error Data
    /// </summary>
    [CLSCompliant(false)]
    [AzureDataStore("ApplicationError")]
    public class ErrorData : LogData, IConvert<ErrorDisplay>
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the ErrorData class
        /// </summary>
        /// <remarks>
        /// For Deserialization
        /// </remarks>
        public ErrorData()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ErrorData class
        /// </summary>
        /// <param name="applicationId">Application Identifier</param>
        public ErrorData(Guid applicationId)
            : base(applicationId)
        {
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != applicationId);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets Parent Id
        /// </summary>
        public Guid ParentId { get; set; }

        /// <summary>
        /// Gets or sets Source
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Gets or sets Stack Trace
        /// </summary>
        public string StackTrace { get; set; }

        /// <summary>
        /// Gets or sets ClassName
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// Gets or sets Error Code
        /// </summary>
        public int ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets Error Type
        /// </summary>
        public int EventTypeValue { get; set; }

        /// <summary>
        /// Gets or sets Session Identifier
        /// </summary>
        public Guid? SessionIdentifier { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Convert
        /// </summary>
        /// <returns>Error Item</returns>
        public ErrorDisplay Convert()
        {
            var token = new Token()
            {
                ApplicationId = this.ApplicationId,
            };

            return new ErrorDisplay()
            {
                ClassName = this.ClassName,
                DeploymentId = this.DeploymentId,
                ErrorCode = this.ErrorCode,
                EventType = (EventTypes)this.EventTypeValue,
                MachineName = this.MachineName,
                Message = this.Message,
                OccurredOn = this.OccurredOn,
                Source = this.Source,
                StackTrace = this.StackTrace,
                Token = token,
                Identifier = Guid.Parse(this.RowKey),
                ParentIdentifier = this.ParentId,
                SessionIdentifier = this.SessionIdentifier,
            };
        }
        #endregion
    }
}