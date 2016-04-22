// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='Email.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Contracts
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Web.Script.Serialization;
    using Abc.Services.Validation;

    /// <summary>
    /// Email Class
    /// </summary>
    [DataContract]
    public abstract class Email : IToken, IValidate<Email>
    {
        #region Properties
        /// <summary>
        /// Gets or sets Token
        /// </summary>
        [DataMember]
        public Token Token { get; set; }

        /// <summary>
        /// Gets or sets Sender
        /// </summary>
        [DataMember]
        public string Sender { get; set; }

        /// <summary>
        /// Gets or sets Recipient
        /// </summary>
        [DataMember]
        public string Recipient { get; set; }

        /// <summary>
        /// Gets the Validation Rules
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Abc.Services.Validation.Rule<Abc.Services.Contracts.Email>.#ctor(System.Func<T,System.Boolean>,System.String)", Justification = "Not a localization issue.")]
        [IgnoreDataMember]
        [ScriptIgnore]
        IEnumerable<Rule<Email>> IValidate<Email>.Rules
        {
            get
            {
                return new Rule<Email>[]
                {
                    new Rule<Email>(c => DataSource.RowIsValid(c.Sender), "Sender is too long."),
                    new Rule<Email>(c => !string.IsNullOrWhiteSpace(c.Sender), "Sender is not present."),
                    new Rule<Email>(c => DataSource.RowIsValid(c.Recipient), "Recipient is too long."),
                    new Rule<Email>(c => !string.IsNullOrWhiteSpace(c.Recipient), "Recipient is not present."),
                };
            }
        }
        #endregion
    }
}