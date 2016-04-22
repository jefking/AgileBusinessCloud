// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='PlaintextEmail.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Web.Script.Serialization;
    using Abc.Services.Data;
    using Abc.Services.Validation;

    /// <summary>
    /// Plain Text Email
    /// </summary>
    public class PlaintextEmail : Email, IConvert<PlaintextEmailData>, IValidate<PlaintextEmail>
    {
        #region Members
        /// <summary>
        /// Gets or sets Subject
        /// </summary>
        [DataMember]
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets Message
        /// </summary>
        [DataMember]
        public string Message { get; set; }

        /// <summary>
        /// Gets the Validation Rules
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Abc.Services.Validation.Rule<Abc.Services.Contracts.Email>.#ctor(System.Func<T,System.Boolean>,System.String)", Justification = "Not a localization issue.")]
        [IgnoreDataMember]
        [ScriptIgnore]
        public IEnumerable<Rule<PlaintextEmail>> Rules
        {
            get
            {
                return new Rule<PlaintextEmail>[]
                {
                    new Rule<PlaintextEmail>(c => DataSource.RowIsValid(c.Subject), "Subject is too long."),
                    new Rule<PlaintextEmail>(c => !string.IsNullOrWhiteSpace(c.Subject), "Subject is not present."),
                    new Rule<PlaintextEmail>(c => DataSource.RowIsValid(c.Message), "Message is too long."),
                    new Rule<PlaintextEmail>(c => !string.IsNullOrWhiteSpace(c.Message), "Message is not present."),
                };
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Convert
        /// </summary>
        /// <returns>Plaintext Email Data</returns>
        [CLSCompliant(false)]
        public PlaintextEmailData Convert()
        {
            if (null == this.Token)
            {
                throw new InvalidOperationException();
            }

            return new PlaintextEmailData(this.Token.ApplicationId)
            {
                Sender = this.Sender,
                Recipient = this.Recipient,
                Subject = this.Subject,
                Message = this.Message
            };
        }
        #endregion
    }
}