// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='IConfigurationAdaptor.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Configuration
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Configuration Adaptor
    /// </summary>
    [ContractClass(typeof(ConfigurationAdaptorContract))]
    public interface IConfigurationAdaptor
    {
        #region Properties
        /// <summary>
        /// Gets Configuration Dictionary
        /// </summary>
        IDictionary<string, string> Configuration
        {
            get;
        }
        #endregion
    }
}