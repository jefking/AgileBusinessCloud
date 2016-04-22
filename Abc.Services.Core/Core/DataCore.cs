// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='DataCore.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Core
{
    /// <summary>
    /// Data Core
    /// </summary>
    public abstract class DataCore
    {
        #region Members
        /// <summary>
        /// Log Core
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Protected Readonly")]
        protected static readonly LogCore Logging = new LogCore();
        #endregion
    }
}
