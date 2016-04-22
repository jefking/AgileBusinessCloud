// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='Message.cs'>
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
    /// Message, for Log
    /// </summary>
    [DataContract]
    [Serializable]
    public class Message : LogItem, IConvert<MessageData>, IValidate<Message>
    {
        #region Properties
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
        public IEnumerable<Rule<Message>> Rules
        {
            get
            {
                return new Rule<Message>[]
                {
                    new Rule<Message>(m => !string.IsNullOrWhiteSpace(m.Message), "Message is not specified."),
                    new Rule<Message>(m => DataSource.RowIsValid(m.Message), "Message is too long."),
                    new Rule<Message>(m => m.SessionIdentifier == null || Guid.Empty != m.SessionIdentifier, "Session Identifier invalid."),
                };
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Convert
        /// </summary>
        /// <returns>Message Data</returns>
        [CLSCompliant(false)]
        public MessageData Convert()
        {
            return new MessageData(this.Token.ApplicationId)
            {
                OccurredOn = this.OccurredOn,
                MachineName = this.MachineName,
                DeploymentId = this.DeploymentId,
                Message = this.Message,
                SessionIdentifier = this.SessionIdentifier,
            };
        }
        #endregion
    }
}