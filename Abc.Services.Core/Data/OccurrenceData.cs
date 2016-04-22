// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='OccurrenceData.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services
{
    using System;
    using Abc.Azure;
    using Abc.Services.Contracts;

    /// <summary>
    /// Occurrence Data
    /// </summary>
    [CLSCompliant(false)]
    [AzureDataStore("ApplicationOccurrence")]
    public class OccurrenceData : LogData, IConvert<OccurrenceDisplay>
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the OccurrenceData class
        /// </summary>
        /// <remarks>
        /// For Deserialization
        /// </remarks>
        public OccurrenceData()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the OccurrenceData class
        /// </summary>
        /// <param name="applicationId">Application Identifier</param>
        public OccurrenceData(Guid applicationId)
            : base(applicationId)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets ClassName
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// Gets or sets Duration in Ticks
        /// </summary>
        public long Duration { get; set; }

        /// <summary>
        /// Gets or sets Method
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// Gets or sets Thread Id
        /// </summary>
        public int ThreadId { get; set; }

        /// <summary>
        /// Gets or sets Session Identifier
        /// </summary>
        public Guid? SessionIdentifier { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Convert
        /// </summary>
        /// <returns>Occurence</returns>
        public OccurrenceDisplay Convert()
        {
            var token = new Token()
            {
                ApplicationId = this.ApplicationId,
            };

            return new OccurrenceDisplay()
            {
                Class = this.ClassName,
                DeploymentId = this.DeploymentId,
                Duration = TimeSpan.FromTicks(this.Duration),
                MachineName = this.MachineName,
                Message = this.Message,
                Method = this.MethodName,
                OccurredOn = this.OccurredOn,
                ThreadId = this.ThreadId,
                Token = token,
                Identifier = Guid.Parse(this.RowKey),
                SessionIdentifier = this.SessionIdentifier,
            };
        }
        #endregion
    }
}