// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='TextContent.cs'>
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
    /// Text Content
    /// </summary>
    [DataContract]
    public class TextContent : Content, IConvert<TextData>, IValidate<TextContent>
    {
        #region Properties
        /// <summary>
        /// Gets or sets Content
        /// </summary>
        [DataMember]
        public string Content
        {
            get;
            set;
        }

        /// <summary>
        /// Gets Rules
        /// </summary>
        [IgnoreDataMember]
        [ScriptIgnore]
        public IEnumerable<Rule<TextContent>> Rules
        {
            get
            {
                return new Rule<TextContent>[]
                {
                    new Rule<TextContent>(t => Guid.Empty != t.Id || !string.IsNullOrWhiteSpace(t.Content), "Id or Content must be specified."),
                };
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Convert
        /// </summary>
        /// <returns>Text Data</returns>
        [CLSCompliant(false)]
        public TextData Convert()
        {
            return new TextData(this.Token.ApplicationId)
            {
                Active = this.Active,
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow,
                Deleted = this.Deleted
            };
        }
        #endregion
    }
}