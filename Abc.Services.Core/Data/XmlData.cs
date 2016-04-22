// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='XmlData.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services
{
    using System;
    using Abc.Azure;
    using Abc.Services.Contracts;

    /// <summary>
    /// XML Data
    /// </summary>
    [CLSCompliant(false)]
    [AzureDataStore("XmlContent")]
    public sealed class XmlData : ContentData, IConvert<XmlContent>
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the XmlData class
        /// </summary>
        /// <remarks>
        /// For Deserialization
        /// </remarks>
        public XmlData()
        {
        }

        /// <summary>
        /// Initializes a new instance of the XmlData class
        /// </summary>
        /// <param name="applicationId">Application Identifier</param>
        public XmlData(Guid applicationId)
            : base(applicationId)
        {
            this.CreatedOn = DateTime.UtcNow;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets Content
        /// </summary>
        public Guid ContentId
        {
            get;
            set;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Convert
        /// </summary>
        /// <returns>Message Data</returns>
        public XmlContent Convert()
        {
            return new XmlContent()
            {
                Id = this.Id,
                Active = this.Active,
                CreatedOn = this.CreatedOn,
                Deleted = this.Deleted,
                UpdatedOn = this.UpdatedOn,
            };
        }
        #endregion
    }
}