// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='ApplicationSummary.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Website
{
    using Abc.Website.Models;

    /// <summary>
    /// Application Summary
    /// </summary>
    public class ApplicationSummary
    {
        #region Properties
        /// <summary>
        /// Gets or sets Message Count
        /// </summary>
        public int MessageCount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Occurrence Count
        /// </summary>
        public int OccurrenceCount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Error Count
        /// </summary>
        public int ErrorCount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Server Count
        /// </summary>
        public int ServerCount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Application Details Model
        /// </summary>
        public ApplicationDetailsModel Details
        {
            get;
            set;
        }
        #endregion
    }
}