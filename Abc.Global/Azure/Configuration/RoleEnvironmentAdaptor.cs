// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='RoleEnvironmentAdaptor.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Azure.Configuration
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using Abc.Configuration;
    using Microsoft.WindowsAzure.ServiceRuntime;

    /// <summary>
    /// Role Environment Settings
    /// </summary>
    public class RoleEnvironmentAdaptor : IConfigurationAdaptor
    {
        #region Members
        /// <summary>
        /// Role Environment Configuration
        /// </summary>
        private readonly RoleEnvironmentConfigurationDictionary config = new RoleEnvironmentConfigurationDictionary();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the RoleEnvironmentAdaptor class.
        /// </summary>
        public RoleEnvironmentAdaptor()
        {
            RoleEnvironment.Changing += this.RoleEnvironmentChanging;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets Configuration Items
        /// </summary>
        public IDictionary<string, string> Configuration
        {
            get
            {
                return this.config;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Role Environment Changing
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Role Environment Changing Event Args</param>
        private void RoleEnvironmentChanging(object sender, RoleEnvironmentChangingEventArgs e)
        {
            foreach (RoleEnvironmentConfigurationSettingChange change in e.Changes)
            {
                if (null != change && !string.IsNullOrWhiteSpace(change.ConfigurationSettingName) && this.config.ContainsKey(change.ConfigurationSettingName))
                {
                    this.config.Remove(change.ConfigurationSettingName);
                }
            }
        }

        /// <summary>
        /// Contract Invariant
        /// </summary>
        [ContractInvariantMethod]
        private void ContractInvariant()
        {
            Contract.Invariant(null != this.config);
        }
        #endregion
    }
}