// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='BinaryEmail.cs'>
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
    /// Binary Email
    /// </summary>
    [DataContract]
    public class BinaryEmail : Email, IConvert<BinaryEmailData>, IValidate<BinaryEmail>
    {
        #region Properties
        /// <summary>
        /// Gets or sets Subject
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Public Api.")]
        [DataMember]
        public byte[] RawMessage { get; set; }

        /// <summary>
        /// Gets the Validation Rules
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Abc.Services.Validation.Rule<Abc.Services.Contracts.BinaryEmail>.#ctor(System.Func<T,System.Boolean>,System.String)", Justification = "Not for localization.")]
        [IgnoreDataMember]
        [ScriptIgnore]
        public IEnumerable<Rule<BinaryEmail>> Rules
        {
            get
            {
                return new Rule<BinaryEmail>[]
                {
                    new Rule<BinaryEmail>(b => null != b.RawMessage, "Raw Message is empty."),
                };
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Convert
        /// </summary>
        /// <returns>Binary Email Data</returns>
        [CLSCompliant(false)]
        public BinaryEmailData Convert()
        {
            if (null == this.Token)
            {
                throw new InvalidOperationException();
            }

            return new BinaryEmailData(this.Token.ApplicationId)
            {
                Sender = this.Sender,
                Recipient = this.Recipient,
                RawMessage = this.RawMessage,
            };
        }
        #endregion
    }
}