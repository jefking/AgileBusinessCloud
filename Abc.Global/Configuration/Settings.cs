// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='Settings.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using Abc.Collections;

    /// <summary>
    /// Settings
    /// </summary>
    public sealed class Settings
    {
        #region Members
        /// <summary>
        /// Adaptors in Priority Order
        /// </summary>
        private readonly PriorityCollection<ConfigurationAdaptorOrder> orderedAdaptors = new PriorityCollection<ConfigurationAdaptorOrder>();

        /// <summary>
        /// Adaptor Lock for thread safety
        /// </summary>
        private readonly object adaptorLock = new object();

        /// <summary>
        /// Settings Instance
        /// </summary>
        private static readonly Settings instance = new Settings();
        #endregion

        #region Constructors
        /// <summary>
        /// Prevents a default instance of the Settings class from being created.
        /// </summary>
        private Settings()
        {
            this.Add(new AppSettingsAdaptor());
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets Settings Instance
        /// </summary>
        public static Settings Instance
        {
            get
            {
                Contract.Ensures(Contract.Result<Settings>() != null);

                return instance;
            }
        }

        /// <summary>
        /// Gets Application Identifier
        /// </summary>
        public static Guid ApplicationIdentifier
        {
            get
            {
                return Settings.Instance.Get<Guid>("ApplicationIdentifier", Guid.Empty);
            }
        }

        /// <summary>
        /// Get Value
        /// </summary>
        /// <param name="key">Setting Key</param>
        /// <returns>Value</returns>
        public string this[string key]
        {
            get
            {
                return this.Get(key);
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Set Adaptors
        /// </summary>
        /// <param name="adaptors">Configuration Adaptors</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Code contracts")]
        public void Add(IEnumerable<IConfigurationAdaptor> adaptors)
        {
            Contract.Requires(null != adaptors);
            Contract.Requires(!adaptors.Any(a => a == null));

            foreach (var adaptor in adaptors)
            {
                this.Add(adaptor);
            }
        }

        /// <summary>
        /// Add Configuration Adaptor
        /// </summary>
        /// <param name="adaptor">Adaptor</param>
        public void Add(IConfigurationAdaptor adaptor)
        {
            Contract.Requires(null != adaptor);

            lock (this.adaptorLock)
            {
                this.orderedAdaptors.Add(new ConfigurationAdaptorOrder(adaptor, this.orderedAdaptors.Count));
            }
        }

        /// <summary>
        /// Get Value
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Value</returns>
        public string Get(string key)
        {
            return this.Get(key, null);
        }

        /// <summary>
        /// Get Value
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">Default Value</param>
        /// <returns>Value</returns>
        public string Get(string key, string defaultValue)
        {
            return this.Get<string>(key, defaultValue);
        }

        /// <summary>
        /// Get Value
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">Key</param>
        /// <returns>Value of Type</returns>
        public T Get<T>(string key)
        {
            return this.Get(key, default(T));
        }

        /// <summary>
        /// Get Value
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">Default Value</param>
        /// <returns>Value of Type</returns>
        public T Get<T>(string key, T defaultValue)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                foreach (var a in this.orderedAdaptors)
                {
                    if (a.Adaptor.Configuration.ContainsKey(key))
                    {
                        return Abc.Convert.FromString(a.Adaptor.Configuration[key], defaultValue);
                    }
                }
            }

            return defaultValue;
        }

        /// <summary>
        /// Invariant Contract
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Contract Invariant")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Invariant Contract")]
        [ContractInvariantMethod]
        private void InvariantContract()
        {
            Contract.Invariant(null != this.orderedAdaptors);
            Contract.Invariant(null != this.adaptorLock);
        }
        #endregion
    }
}