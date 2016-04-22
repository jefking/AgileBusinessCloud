// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='ConvertContract.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc
{
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Convert Contract
    /// </summary>
    [ContractClassFor(typeof(IConvert<>))]
    public abstract class ConvertContract : IConvert<object>
    {
        #region Methods
        /// <summary>
        /// Convert
        /// </summary>
        /// <returns>New Object</returns>
        public object Convert()
        {
            Contract.Ensures(Contract.Result<object>() != null);

            return new object();
        }
        #endregion
    }
}