// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='TextData.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services
{
    using System;
    using Abc.Azure;
    using Abc.Services.Contracts;

    /// <summary>
    /// Text Data
    /// </summary>
    [CLSCompliant(false)]
    [AzureDataStore("TextContent")]
    public sealed class TextData : ContentData, IConvert<TextContent>
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the TextData class
        /// </summary>
        /// <remarks>
        /// For Deserialization
        /// </remarks>
        public TextData()
        {
        }

        /// <summary>
        /// Initializes a new instance of the TextData class
        /// </summary>
        /// <param name="applicationId">Application Identifier</param>
        public TextData(Guid applicationId)
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
        /// <returns>Text Content</returns>
        public TextContent Convert()
        {
            return new TextContent()
            {
                Id = this.Id,
                Active = this.Active,
                CreatedOn = this.CreatedOn,
                Deleted = this.Deleted,
                UpdatedOn = this.UpdatedOn
            };
        }
        #endregion
    }
}