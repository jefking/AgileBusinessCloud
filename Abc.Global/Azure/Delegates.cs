// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='Delegates.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Azure
{
    #region Delegates
    /// <summary>
    /// Recieve
    /// </summary>
    /// <typeparam name="T">Type of Object</typeparam>
    /// <param name="type">Type</param>
    /// <returns>Completed</returns>
    public delegate bool Recieve<T>(T type);
    #endregion
}