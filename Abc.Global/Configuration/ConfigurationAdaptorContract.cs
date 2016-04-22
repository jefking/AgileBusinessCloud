// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='ConfigurationAdaptorContract.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Configuration
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// IConfiguration Adaptor Contract
    /// </summary>
    [ContractClassFor(typeof(IConfigurationAdaptor))]
    public abstract class ConfigurationAdaptorContract : IConfigurationAdaptor
    {
        #region Properties
        /// <summary>
        /// Gets Configuration
        /// </summary>
        public IDictionary<string, string> Configuration
        {
            get
            {
                Contract.Ensures(Contract.Result<IDictionary<string, string>>() != null);

                return new Dictionary<string, string>();
            }
        }
        #endregion
    }
}