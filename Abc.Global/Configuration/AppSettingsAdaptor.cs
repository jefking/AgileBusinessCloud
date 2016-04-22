// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='AppSettingsAdaptor.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Configuration
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Configuration Adaptor for System.Configuration.ConfigurationManager.AppSettings
    /// </summary>
    public class AppSettingsAdaptor : IConfigurationAdaptor
    {
        #region Members
        /// <summary>
        /// Application Configuration
        /// </summary>
        private static readonly IDictionary<string, string> configuration = Load();
        #endregion

        #region Properties
        /// <summary>
        /// Gets Configuration Items
        /// </summary>
        public IDictionary<string, string> Configuration
        {
            get
            {
                return configuration;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Load Configuration, configuration is loaded statically
        /// </summary>
        /// <returns>Configuration</returns>
        private static IDictionary<string, string> Load()
        {
            var config = new Dictionary<string, string>();
            foreach (string key in ConfigurationManager.AppSettings.Keys)
            {
                if (!config.ContainsKey(key))
                {
                    config.Add(key, ConfigurationManager.AppSettings[key]);
                }
            }

            return config;
        }

        /// <summary>
        /// Invariant Contract
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Contract Invariant")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Invariant Contract")]
        [ContractInvariantMethod]
        private void InvariantContract()
        {
            Contract.Invariant(null != configuration);
        }
        #endregion
    }
}