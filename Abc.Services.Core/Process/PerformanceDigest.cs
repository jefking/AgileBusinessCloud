// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='ErrorDigest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Process
{
    using Abc.Services.Contracts;
    using System;
    using System.Linq;

    /// <summary>
    /// Performance Digest
    /// </summary>
    public class PerformanceDigest : LoggingDigest
    {
        #region Methods
        /// <summary>
        /// Digest Application Messages
        /// </summary>
        /// <param name="applicationIdentifier">Application Identifier</param>
        protected override void Digest(Guid applicationIdentifier)
        {
            using (new PerformanceMonitor())
            {
                try
                {
                    logCore.DigestOccurrences(applicationIdentifier);
                }
                catch (Exception ex)
                {
                    logCore.Log(ex, EventTypes.Critical, 99999);
                }
            }
        }
        #endregion
    }
}