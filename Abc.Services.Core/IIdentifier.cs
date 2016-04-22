// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='IIdentifier.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services
{
    /// <summary>
    /// Identifier Interface
    /// </summary>
    /// <typeparam name="T">Identifier Type</typeparam>
    public interface IIdentifier<T>
    {
        #region Properties
        /// <summary>
        /// Gets the Identifier
        /// </summary>
        T Identifier
        {
            get;
        }
        #endregion
    }
}