// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='CompressedErrors.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Website.Models
{
    using Abc.Azure;
    using System;
    using System.Collections.Generic;

    [AzureDataStore("api")]
    [Serializable]
    public class CompressedErrors
    {
        #region Properties
        public DateTime GeneratedOn
        {
            get;
            set;
        }

        public IEnumerable<CompressedError> Errors
        {
            get;
            set;
        }

        public IEnumerable<ErrorsPerTime> Occurrences
        {
            get;
            set;
        }
        #endregion
    }
}