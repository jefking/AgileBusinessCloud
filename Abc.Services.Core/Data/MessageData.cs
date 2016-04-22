// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='MessageData.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services
{
    using System;
    using Abc.Azure;
    using Abc.Services.Contracts;

    /// <summary>
    /// Message Data
    /// </summary>
    [CLSCompliant(false)]
    [AzureDataStore("ApplicationMessage")]
    public class MessageData : LogData, IConvert<MessageDisplay>
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the MessageData class
        /// </summary>
        /// <remarks>
        /// For Deserialization
        /// </remarks>
        public MessageData()
        {
        }

        /// <summary>
        /// Initializes a new instance of the MessageData class
        /// </summary>
        /// <param name="applicationId">Application Identifier</param>
        public MessageData(Guid applicationId)
            : base(applicationId)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets Session Identifier
        /// </summary>
        public Guid? SessionIdentifier { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Convert
        /// </summary>
        /// <returns>Message</returns>
        public MessageDisplay Convert()
        {
            var token = new Token()
            {
                ApplicationId = this.ApplicationId,
            };

            return new MessageDisplay()
            {
                DeploymentId = this.DeploymentId,
                MachineName = this.MachineName,
                Message = this.Message,
                OccurredOn = this.OccurredOn,
                Token = token,
                Identifier = Guid.Parse(this.RowKey),
                SessionIdentifier = this.SessionIdentifier,
            };
        }
        #endregion
    }
}