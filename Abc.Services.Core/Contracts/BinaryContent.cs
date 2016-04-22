// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='BinaryContent.cs'>
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
    /// Binary Content
    /// </summary>
    [DataContract]
    public class BinaryContent : Secured, IValidate<BinaryContent>
    {
        #region Properties
        /// <summary>
        /// Gets or sets Id
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets Content
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Byte[] for transfering data.")]
        [DataMember]
        public byte[] Content { get; set; }

        /// <summary>
        /// Gets or sets Content Type, Mime Type
        /// </summary>
        [DataMember]
        public string ContentType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets Rules
        /// </summary>
        [IgnoreDataMember]
        [ScriptIgnore]
        public IEnumerable<Rule<BinaryContent>> Rules
        {
            get
            {
                return new Rule<BinaryContent>[]
                {
                    new Rule<BinaryContent>(b => Guid.Empty != b.Id || null != b.Content, "Id or Content must be specified."),
                };
            }
        }
        #endregion
    }
}