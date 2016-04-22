// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='XmlContent.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Runtime.Serialization;
    using System.Web.Script.Serialization;
    using System.Xml;
    using Abc.Services.Core;
    using Abc.Services.Validation;

    /// <summary>
    /// XML Content
    /// </summary>
    [DataContract]
    public class XmlContent : Content, IConvert<XmlData>, IValidate<XmlContent>
    {
        #region Members
        /// <summary>
        /// Logger
        /// </summary>
        private static readonly LogCore logger = new LogCore();
        #endregion

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
        public IEnumerable<Rule<XmlContent>> Rules
        {
            get
            {
                return new Rule<XmlContent>[]
                {
                    new Rule<XmlContent>(x => Guid.Empty != x.Id || !string.IsNullOrWhiteSpace(x.Content), "Id or Content must be specified."),
                    new Rule<XmlContent>(x => x.IsValid(), "Xml must be valid."),
                };
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Validates Object
        /// </summary>
        /// <remarks>
        /// Validates XML
        /// </remarks>
        /// <returns>Is Valid</returns>
        [Pure]
        public bool IsValid()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(this.Content))
                {
                    var doc = new XmlDocument();
                    doc.LoadXml(this.Content);
                }

                return true;
            }
            catch (XmlException ex)
            {
                logger.Log(ex, EventTypes.Warning, (int)ContentFault.ContentInvalidXml);
                return false;
            }
        }

        /// <summary>
        /// Convert
        /// </summary>
        /// <returns>XML Data</returns>
        [CLSCompliant(false)]
        public XmlData Convert()
        {
            return new XmlData(this.Token.ApplicationId)
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