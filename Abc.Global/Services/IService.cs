// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='IService.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc
{
    /// <summary>
    /// Service Interface
    /// </summary>
    public interface IService
    {
        #region Methods
        /// <summary>
        /// Runs Service
        /// </summary>
        /// <returns>Running</returns>
        bool Run();

        /// <summary>
        /// Stops Service
        /// </summary>
        /// <returns>Stopped</returns>
        bool Stop();
        #endregion
    }
}