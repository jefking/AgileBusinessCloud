// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='IApplicationIdentifier.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services
{
    using System;

    /// <summary>
    /// Application Identifier Interface
    /// </summary>
    public interface IApplicationIdentifier
    {
        #region Properties
        /// <summary>
        /// Gets the Application Identifier
        /// </summary>
        Guid ApplicationIdentifier
        {
            get;
        }
        #endregion
    }
}