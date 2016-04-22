// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='ILoad.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc
{
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Load Interface
    /// </summary>
    /// <typeparam name="T">Type to load data from</typeparam>
    [ContractClass(typeof(LoadContract))]
    public interface ILoad<T>
    {
        #region Methods
        /// <summary>
        /// Load data from source
        /// </summary>
        /// <param name="data">To Load</param>
        void Load(T data);
        #endregion
    }
}