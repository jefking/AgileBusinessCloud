// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='ManagementModel.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Website.Models
{
    using System.Collections;
    using System.Collections.Generic;
    using Abc.Services.Contracts;

    /// <summary>
    /// Management Model
    /// </summary>
    public class ManagementModel
    {
        #region Members
        /// <summary>
        /// Gets or sets Application
        /// </summary>
        public ApplicationDetailsModel Application { get; set; }

        /// <summary>
        /// Gets or sets Applications
        /// </summary>
        public IEnumerable<ApplicationDetailsModel> Applications
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Data
        /// </summary>
        public IEnumerable Data { get; set; }

        /// <summary>
        /// Gets or sets Error
        /// </summary>
        public WebResponse Error { get; set; }

        /// <summary>
        /// Gets or sets User Preference
        /// </summary>
        public UserPreference Preference
        {
            get;
            set;
        }
        #endregion
    }
}