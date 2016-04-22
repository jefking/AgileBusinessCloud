// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='BlogModel.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Website.Models
{
    using System.Collections.Generic;
    using Abc.Services.Contracts;

    /// <summary>
    /// Blog Model
    /// </summary>
    public class BlogModel
    {
        #region Properties
        /// <summary>
        /// Gets or sets Current Post
        /// </summary>
        public BlogEntry Post
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Posts
        /// </summary>
        public IEnumerable<BlogEntry> Posts
        {
            get;
            set;
        }
        #endregion
    }
}