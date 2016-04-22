// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='Token.cs'>
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
    /// Token class for further authentication from the client
    /// </summary>
    [DataContract]
    [Serializable]
    public class Token : IValidate<Token>
    {
        #region Properties
        /// <summary>
        /// Gets or sets Application Id
        /// </summary>
        [DataMember]
        public Guid ApplicationId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Validation Key
        /// </summary>
        [DataMember]
        public string ValidationKey
        {
            get;
            set;
        }

        /// <summary>
        /// Gets Rules
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Abc.Services.Validation.Rule<Abc.Services.Contracts.Token>.#ctor(System.Func<T,System.Boolean>,System.String)", Justification = "Currently not localizing.")]
        [IgnoreDataMember]
        [ScriptIgnore]
        public IEnumerable<Rule<Token>> Rules
        {
            get
            {
                return new Rule<Token>[]
                {
                    new Rule<Token>(t => t != null && t.ApplicationId != Guid.Empty, "Application identifier not specified."),
                    new Rule<Token>(t => t != null && !string.IsNullOrWhiteSpace(t.ValidationKey), "Validation Key not specified."),
                };
            }
        }
        #endregion
    }
}