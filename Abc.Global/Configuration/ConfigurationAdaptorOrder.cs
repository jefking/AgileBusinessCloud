// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='ConfigurationAdaptorOrder.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Configuration
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Configuration Adaptor Order
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1036:OverrideMethodsOnComparableTypes", Justification = "Simple implementation")]
    public class ConfigurationAdaptorOrder : IComparable<ConfigurationAdaptorOrder>
    {
        #region Members
        /// <summary>
        /// Adaptror
        /// </summary>
        private readonly IConfigurationAdaptor adaptor = null;
        
        /// <summary>
        /// Order
        /// </summary>
        private int order = 0;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the ConfigurationAdaptorOrder class
        /// </summary>
        /// <param name="adaptor">Adaptor</param>
        /// <param name="order">Order</param>
        public ConfigurationAdaptorOrder(IConfigurationAdaptor adaptor, int order)
        {
            if (null == adaptor)
            {
                throw new ArgumentNullException("adaptor");
            }
            else if (0 > order)
            {
                throw new ArgumentOutOfRangeException("order");
            }
            else
            {
                this.adaptor = adaptor;
                this.order = order;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the Configuration Adaptor
        /// </summary>
        public IConfigurationAdaptor Adaptor
        {
            get
            {
                return this.adaptor;
            }
        }

        /// <summary>
        /// Gets the Order
        /// </summary>
        public int Order
        {
            get
            {
                return this.order;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Compare To
        /// </summary>
        /// <param name="other">Configuration Adaptor Order</param>
        /// <returns>Comparison of Order</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Code contracts")]
        public int CompareTo(ConfigurationAdaptorOrder other)
        {
            if (null == other)
            {
                throw new ArgumentNullException("other");
            }
            else
            {
                return other.Order.CompareTo(this.Order);
            }
        }

        /// <summary>
        /// Invariant Contract
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Contract Invariant")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Invariant Contract")]
        [ContractInvariantMethod]
        private void InvariantContract()
        {
            Contract.Invariant(null != this.adaptor);
            Contract.Invariant(0 <= this.order);
        }
        #endregion
    }
}