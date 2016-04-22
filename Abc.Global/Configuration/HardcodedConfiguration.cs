// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='HardcodedConfiguration.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Configuration
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Hardcoded Configuration
    /// </summary>
    public abstract class HardcodedConfiguration : IConfigurationAdaptor
    {
        #region Members
        /// <summary>
        /// Configuration
        /// </summary>
        private readonly IDictionary<string, string> configuration = null;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the HardcodedConfiguration class
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "By Design")]
        protected HardcodedConfiguration()
        {
            this.configuration = this.DefineConfiguration() ?? new Dictionary<string, string>(0);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets Configuration Dictionary
        /// </summary>
        public IDictionary<string, string> Configuration
        {
            get
            {
                return this.configuration;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Define Configuration
        /// </summary>
        /// <returns>Hard Coded COnfiguartion Values</returns>
        public abstract IDictionary<string, string> DefineConfiguration();
        #endregion
    }
}