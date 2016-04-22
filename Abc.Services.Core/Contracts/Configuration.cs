// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='Configuration.cs'>
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
    /// Configuration
    /// </summary>
    [DataContract]
    public class Configuration : Secured, IConvert<ApplicationConfiguration>, IValidate<Configuration>
    {
        #region Properties
        /// <summary>
        /// Gets or sets Key
        /// </summary>
        [DataMember]
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets Value
        /// </summary>
        [DataMember]
        public string Value { get; set; }

        /// <summary>
        /// Gets the Validation Rules
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Abc.Services.Validation.Rule<Abc.Services.Contracts.Configuration>.#ctor(System.Func<T,System.Boolean>,System.String)", Justification = "Not a localization issue.")]
        [IgnoreDataMember]
        [ScriptIgnore]
        public IEnumerable<Rule<Configuration>> Rules
        {
            get
            {
                return new Rule<Configuration>[]
                {
                    new Rule<Configuration>(c => DataSource.RowIsValid(c.Key), "Key is too long."),
                };
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Convert to Application Configuration
        /// </summary>
        /// <returns>Application Configuration</returns>
        [CLSCompliant(false)]
        public ApplicationConfiguration Convert()
        {
            if (null == this.Token)
            {
                throw new InvalidOperationException();
            }

            return new ApplicationConfiguration(this.Token.ApplicationId, this.Key)
            {
                Value = this.Value,
            };
        }
        #endregion
    }
}