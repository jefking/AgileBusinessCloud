// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='TableAdaptor.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using Abc.Azure;
    using Abc.Configuration;
    using Abc.Services.Contracts;
    using Abc.Services.Core;
    using Abc.Threading;

    /// <summary>
    /// Configuration Adaptor, for Application Configuration in Azure Table Storage
    /// </summary>
    public sealed class TableAdaptor : IConfigurationAdaptor, IDisposable
    {
        #region Members
        /// <summary>
        /// Application Identifier
        /// </summary>
        private readonly Guid applicationIdentifier;

        /// <summary>
        /// Logger
        /// </summary>
        private readonly LogCore logger = new LogCore();

        /// <summary>
        /// Configuration Timer
        /// </summary>
        private SafeTimer configurationTimer = null;

        /// <summary>
        /// Configuration
        /// </summary>
        private IDictionary<string, string> configuration = new Dictionary<string, string>();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the TableAdaptor class
        /// </summary>
        /// <param name="applicationIdentifier">Application Identifier</param>
        public TableAdaptor(Guid applicationIdentifier)
        {
            Contract.Requires(Guid.Empty != applicationIdentifier);

            this.applicationIdentifier = applicationIdentifier;

            this.configurationTimer = new SafeTimer(this.Load, this.LoadError, null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(60));
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets Configuration
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
        /// Load Configuration
        /// </summary>
        /// <param name="value">Object</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Abc.Services.Core.LogCore.Log(System.String)", Justification = "Not localized, tracing.")]
        public void Load(object value)
        {
            this.logger.Log("Loading configuration from data store.");

            var table = new AzureTable<ApplicationConfiguration>(ServerConfiguration.Default);

            var data = table.QueryByPartition(this.applicationIdentifier.ToString());
            var settings = data.ToList().AsParallel().Select(d => d.Convert());
            if (0 < settings.Count())
            {
                var config = new Dictionary<string, string>(this.configuration.Count);
                foreach (var setting in settings)
                {
                    if (!config.ContainsKey(setting.Key))
                    {
                        config.Add(setting.Key, setting.Value);
                    }
                    else
                    {
                        this.logger.Log("Key is already present in dictionary: '{0}', first value was used '{1}'.".FormatWithCulture(setting.Key, config[setting.Key]));
                    }
                }

                this.configuration = config;
            }
            else
            {
                this.logger.Log("No settings being loaded from data store.");
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            if (null != this.configurationTimer)
            {
                this.configurationTimer.Dispose();
            }

            this.configurationTimer = null;

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Load Error
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="exception">Exception</param>
        private void LoadError(object sender, EventArgs<Exception> exception)
        {
            this.logger.Log(exception.Argument, EventTypes.Information, (int)DatumFault.DataExistsFailure);
        }
        #endregion
    }
}