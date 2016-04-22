// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='Occurrence.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Web.Script.Serialization;
    using Abc.Services.Validation;

    /// <summary>
    /// Occurrence, of code execution
    /// </summary>
    [DataContract]
    [Serializable]
    public class Occurrence : LogItem, IConvert<OccurrenceData>, IValidate<Occurrence>
    {
        #region Properties
        /// <summary>
        /// Gets or sets Class
        /// </summary>
        [DataMember]
        public string Class { get; set; }

        /// <summary>
        /// Gets or sets Execution Time
        /// </summary>
        [DataMember]
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// Gets or sets Method
        /// </summary>
        [DataMember]
        public string Method { get; set; }

        /// <summary>
        /// Gets or sets Thread Id
        /// </summary>
        [DataMember]
        public int ThreadId { get; set; }

        /// <summary>
        /// Gets or sets Session Identifier
        /// </summary>
        [DataMember]
        public Guid? SessionIdentifier { get; set; }

        /// <summary>
        /// Gets Rules
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Abc.Services.Validation.Rule<Abc.Services.Contracts.Email>.#ctor(System.Func<T,System.Boolean>,System.String)", Justification = "Not a localization issue.")]
        [IgnoreDataMember]
        [ScriptIgnore]
        public IEnumerable<Rule<Occurrence>> Rules
        {
            get
            {
                return new Rule<Occurrence>[]
                {
                    new Rule<Occurrence>(o => !string.IsNullOrWhiteSpace(o.Method), "Method is not specified."),
                    new Rule<Occurrence>(o => DataSource.RowIsValid(o.Method), "Method is too long."),
                    new Rule<Occurrence>(o => !string.IsNullOrWhiteSpace(o.Class), "Class is not specified."),
                    new Rule<Occurrence>(o => DataSource.RowIsValid(o.Class), "Class is too long."),
                    new Rule<Occurrence>(o => TimeSpan.Zero < o.Duration, "Duration too short."),
                    new Rule<Occurrence>(o => 0 < o.ThreadId, "Thread Id invalid."),
                    new Rule<Occurrence>(o => o.SessionIdentifier == null || Guid.Empty != o.SessionIdentifier, "Session Identifier invalid."),
                };
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Convert
        /// </summary>
        /// <returns>Occurrence Data</returns>
        [CLSCompliant(false)]
        public OccurrenceData Convert()
        {
            return new OccurrenceData(this.Token.ApplicationId)
            {
                OccurredOn = this.OccurredOn,
                MachineName = this.MachineName,
                DeploymentId = this.DeploymentId,
                Message = this.Message,
                ClassName = this.Class,
                Duration = this.Duration.Ticks,
                MethodName = this.Method,
                ThreadId = this.ThreadId,
                SessionIdentifier = this.SessionIdentifier,
            };
        }
        #endregion
    }
}