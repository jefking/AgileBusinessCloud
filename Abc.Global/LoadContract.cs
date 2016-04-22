// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='LoadContract.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc
{
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Load Contract
    /// </summary>
    [ContractClassFor(typeof(ILoad<>))]
    public abstract class LoadContract : ILoad<object>
    {
        #region Methods
        /// <summary>
        /// Load data from source
        /// </summary>
        /// <param name="data">To Load</param>
        public void Load(object data)
        {
            Contract.Requires(null != data);
        }
        #endregion
    }
}