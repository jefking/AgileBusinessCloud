// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='BlogRow.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Data
{
    using System;
    using System.Diagnostics.Contracts;
    using Abc.Azure;
    using Abc.Services.Contracts;
    using Microsoft.WindowsAzure.StorageClient;

    [CLSCompliant(false)]
    [AzureDataStore("BlogEntries")]
    public class BlogRow : TableServiceEntity, IConvert<BlogEntry>
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the BlogRow class
        /// </summary>
        /// <remarks>
        /// For deserialization
        /// </remarks>
        public BlogRow()
        {
        }

        /// <summary>
        /// Initializes a new instance of the BlogRow class
        /// </summary>
        /// <param name="poster">Poster</param>
        /// <param name="identifier">Identifier</param>
        public BlogRow(Guid poster, Guid identifier)
        {
            Contract.Requires<ArgumentException>(Guid.Empty != identifier);
            Contract.Requires<ArgumentException>(Guid.Empty != poster);

            this.PartitionKey = poster.ToString();
            this.RowKey = identifier.ToString();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the Title
        /// </summary>
        public string Title
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Posted On Date
        /// </summary>
        public DateTime PostedOn
        {
            get;
            set;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Convert to Blog Entry
        /// </summary>
        /// <returns>Blog Entry</returns>
        public BlogEntry Convert()
        {
            return new BlogEntry()
            {
                Identifier = Guid.Parse(this.RowKey),
                Title = this.Title,
                PostedOn = this.PostedOn,
                SectionIdentifier = Guid.Parse(this.PartitionKey),
            };
        }
        #endregion
    }
}
