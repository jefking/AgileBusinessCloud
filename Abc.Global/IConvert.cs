// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='IConvert.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc
{
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Convert Interface
    /// </summary>
    /// <typeparam name="T">Type to Convert To</typeparam>
    [ContractClass(typeof(ConvertContract))]
    public interface IConvert<T>
    {
        #region Methods
        /// <summary>
        /// Convert
        /// </summary>
        /// <returns>Return Type</returns>
        T Convert();
        #endregion
    }
}