// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='IUpdatedOn.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services
{
    using System;

    /// <summary>
    /// Updated On Interface
    /// </summary>
    public interface IUpdatedOn
    {
        #region Properties
        /// <summary>
        /// Gets Updated On
        /// </summary>
        DateTime UpdatedOn
        {
            get;
        }
        #endregion
    }
}