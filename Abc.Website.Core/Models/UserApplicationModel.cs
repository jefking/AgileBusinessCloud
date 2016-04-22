// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='UserApplicationModel.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Website.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Abc.Services.Contracts;

    /// <summary>
    /// User Application Model
    /// </summary>
    public class UserApplicationModel
    {
        #region Properties
        /// <summary>
        /// Gets or sets Users
        /// </summary>
        [Display(Name = "Users")]
        public IEnumerable<User> Users { get; set; }

        /// <summary>
        /// Gets or sets Applications
        /// </summary>
        [Display(Name = "Applications")]
        public IEnumerable<ApplicationDetailsModel> Applications { get; set; }
        #endregion
    }
}