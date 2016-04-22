// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='ErrorItem.cs'>
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
    /// Error Item
    /// </summary>
    [DataContract]
    [Serializable]
    public class ErrorItem : LogItem, IConvert<ErrorData>, IValidate<ErrorItem>
    {
        #region Properties
        /// <summary>
        /// Gets or sets Parent
        /// </summary>
        [DataMember]
        public ErrorItem Parent { get; set; }
        
        /// <summary>
        /// Gets or sets Source
        /// </summary>
        [DataMember]
        public string Source { get; set; }

        /// <summary>
        /// Gets or sets Stack Trace
        /// </summary>
        [DataMember]
        public string StackTrace { get; set; }

        /// <summary>
        /// Gets or sets ClassName
        /// </summary>
        [DataMember]
        public string ClassName { get; set; }

        /// <summary>
        /// Gets or sets Error Code
        /// </summary>
        [DataMember]
        public int ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets Error Type
        /// </summary>
        [DataMember]
        public EventTypes EventType { get; set; }

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
        public IEnumerable<Rule<ErrorItem>> Rules
        {
            get
            {
                return new Rule<ErrorItem>[]
                {
                    new Rule<ErrorItem>(e => !string.IsNullOrWhiteSpace(e.Message), "Message is not specified."),
                    new Rule<ErrorItem>(e => DataSource.RowIsValid(e.Message), "Message is too long."),
                    new Rule<ErrorItem>(e => !string.IsNullOrWhiteSpace(e.ClassName), "Class Name is not specified."),
                    new Rule<ErrorItem>(e => DataSource.RowIsValid(e.ClassName), "Class Name is too long."),
                    new Rule<ErrorItem>(e => e.SessionIdentifier == null || Guid.Empty != e.SessionIdentifier, "Session Identifier invalid."),
                };
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Convert
        /// </summary>
        /// <returns>Error Data</returns>
        [CLSCompliant(false)]
        public ErrorData Convert()
        {
            return new ErrorData(this.Token.ApplicationId)
            {
                OccurredOn = this.OccurredOn,
                MachineName = this.MachineName,
                DeploymentId = this.DeploymentId,
                Message = this.Message,
                Source = this.Source,
                StackTrace = this.StackTrace,
                ErrorCode = this.ErrorCode,
                ClassName = this.ClassName,
                EventTypeValue = (int)this.EventType,
                SessionIdentifier = this.SessionIdentifier,
            };
        }
        #endregion
    }
}