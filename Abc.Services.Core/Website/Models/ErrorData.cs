// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='ErrorData.cs'>
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
    public class ErrorData
    {
        #region Properties
        public DateTime GeneratedOn
        {
            get;
            set;
        }

        public IEnumerable<ErrorDisplay> Errors
        {
            get;
            set;
        }
        #endregion
    }
}