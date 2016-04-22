// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='RoleEnvironmentConfiguration.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Azure.Configuration
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using Microsoft.WindowsAzure.ServiceRuntime;

    /// <summary>
    /// Role Environment Configuration
    /// </summary>
    public class RoleEnvironmentConfigurationDictionary : IDictionary<string, string>
    {
        #region Members
        /// <summary>
        /// Cached Values
        /// </summary>
        private IDictionary<string, string> cachedValues = new Dictionary<string, string>();
        #endregion

        #region Properties
        /// <summary>
        /// Gets Known Configuration Keys
        /// </summary>
        public ICollection<string> Keys
        {
            get
            {
                return this.cachedValues.Keys;
            }
        }

        /// <summary>
        /// Gets Values
        /// </summary>
        public ICollection<string> Values
        {
            get
            {
                return this.cachedValues.Values;
            }
        }

        /// <summary>
        /// Gets the Count of Known Configuration Items
        /// </summary>
        public int Count
        {
            get
            {
                return this.cachedValues.Count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether Is Read Only
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Get Value based on Key
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Value</returns>
        public string this[string key]
        {
            get
            {
                return this.ContainsKey(key) ? this.cachedValues[key] : null;
            }

            set
            {
                this.Add(new KeyValuePair<string, string>(key, value));
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Add Configuration Item
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        public void Add(string key, string value)
        {
            this.Add(new KeyValuePair<string, string>(key, value));
        }

        /// <summary>
        /// Contains Key
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Contains</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Safety First")]
        public bool ContainsKey(string key)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                if (this.cachedValues.ContainsKey(key))
                {
                    return true;
                }
                else if (AzureEnvironment.RoleIsAvailable)
                {
                    try
                    {
                        var value = RoleEnvironment.GetConfigurationSettingValue(key);
                        if (null != value)
                        {
                            if (!this.cachedValues.ContainsKey(key))
                            {
                                this.cachedValues.Add(key, value);
                            }

                            return true;
                        }
                    }
                    catch
                    {
                        // For Safety
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Try Get Value
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <returns>Success</returns>
        public bool TryGetValue(string key, out string value)
        {
            value = null;
            if (!string.IsNullOrWhiteSpace(key) && this.ContainsKey(key))
            {
                value = this.cachedValues[key];
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Key
        /// </summary>
        /// <param name="item">Item</param>
        public void Add(KeyValuePair<string, string> item)
        {
            this.cachedValues.Add(item);
        }

        /// <summary>
        /// Contains Item
        /// </summary>
        /// <param name="item">Item</param>
        /// <returns>Contains</returns>
        public bool Contains(KeyValuePair<string, string> item)
        {
            return this.cachedValues.Contains(item);
        }

        /// <summary>
        /// Remove Item
        /// </summary>
        /// <param name="item">Item</param>
        /// <returns>Removed</returns>
        public bool Remove(KeyValuePair<string, string> item)
        {
            return this.cachedValues.Remove(item);
        }

        /// <summary>
        /// Get Enumerator
        /// </summary>
        /// <returns>Key Value Pairs</returns>
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return this.cachedValues.GetEnumerator();
        }

        /// <summary>
        /// Get Enumerator
        /// </summary>
        /// <returns>Key Value Pairs</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.cachedValues.GetEnumerator();
        }

        /// <summary>
        /// Remove Key
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Revmoved</returns>
        public bool Remove(string key)
        {
            return this.cachedValues.Remove(key);
        }

        /// <summary>
        /// Clear
        /// </summary>
        public void Clear()
        {
            this.cachedValues.Clear();
        }

        /// <summary>
        /// Copy To
        /// </summary>
        /// <param name="array">Array</param>
        /// <param name="arrayIndex">Array Index</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Code contracts")]
        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            this.cachedValues.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Contract Invariant
        /// </summary>
        [ContractInvariantMethod]
        private void ContractInvariant()
        {
            Contract.Invariant(null != this.cachedValues);
        }
        #endregion
    }
}