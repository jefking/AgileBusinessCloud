// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='DashboardModel.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Website.Models
{
    using System.Collections.Generic;
    using Abc.Services.Contracts;

    /// <summary>
    /// Dashboard Model
    /// </summary>
    public class DashboardModel
    {
        #region Properties
        /// <summary>
        /// Gets or sets Summary
        /// </summary>
        public ApplicationSummary Summary
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Messages
        /// </summary>
        public IEnumerable<MessageDisplay> Messages
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Occurrences
        /// </summary>
        public IEnumerable<OccurrenceDisplay> Occurrences
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Errors
        /// </summary>
        public IEnumerable<ErrorDisplay> Errors
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Servers
        /// </summary>
        public IEnumerable<string> Servers
        {
            get;
            set;
        }
        #endregion
    }
}