// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='PerformanceData.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Website.Models
{
    using Abc.Azure;
    using Abc.Services.Contracts;
    using System;
    using System.Collections.Generic;

    [AzureDataStore("api")]
    [Serializable]
    public class PerformanceData
    {
        #region Properties
        public DateTime GeneratedOn
        {
            get;
            set;
        }

        public IEnumerable<OccurrenceDisplay> Occurrences
        {
            get;
            set;
        }
        #endregion
    }
}