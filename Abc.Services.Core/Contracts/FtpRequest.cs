// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='FtpRequest.cs'>
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
    /// FTP Request
    /// </summary>
    public class FtpRequest : IToken, IValidate<FtpRequest>
    {
        #region Properties
        /// <summary>
        /// Gets or sets Token
        /// </summary>
        [DataMember]
        public Token Token { get; set; }

        /// <summary>
        /// Gets or sets UserName
        /// </summary>
        [DataMember]
        public string UserName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Password
        /// </summary>
        [DataMember]
        public string Password
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Uri
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Simplified API"), DataMember]
        public string Uri
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the Validation Rules
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Abc.Services.Validation.Rule<Abc.Services.Contracts.Email>.#ctor(System.Func<T,System.Boolean>,System.String)", Justification = "Not a localization issue.")]
        [IgnoreDataMember]
        [ScriptIgnore]
        public IEnumerable<Rule<FtpRequest>> Rules
        {
            get
            {
                return new Rule<FtpRequest>[]
                {
                    new Rule<FtpRequest>(f => !string.IsNullOrWhiteSpace(f.Uri), "Uri is empty."),
                    new Rule<FtpRequest>(f => DataSource.RowIsValid(f.Uri), "Uri is too long."),
                };
            }
        }
        #endregion
    }
}