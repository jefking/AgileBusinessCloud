// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='ServerConfigurationAdaptor.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Configuration
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Abc.Logging;
    using Abc.Logging.Datum;
    using Abc.Underpinning;

    /// <summary>
    /// Server Configuration Adaptor
    /// </summary>
    public sealed class ServerConfigurationAdaptor : IConfigurationAdaptor, IDisposable
    {
        #region Members
        /// <summary>
        /// Application
        /// </summary>
        private static readonly Application application = new Application();

        /// <summary>
        /// Configuration Items
        /// </summary>
        private static readonly IDictionary<string, string> configuration = new ConcurrentDictionary<string, string>();
        
        /// <summary>
        /// Item Lock
        /// </summary>
        private static object itemLock = new object();

        /// <summary>
        /// Loaded
        /// </summary>
        private volatile bool loaded = false;

        /// <summary>
        /// Configuration Timer
        /// </summary>
        private Timer configurationTimer = null;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the ServerConfigurationAdaptor class
        /// </summary>
        public ServerConfigurationAdaptor()
        {
            this.configurationTimer = new Timer(this.Load, null, Timeout.Infinite, (int)TimeSpan.FromMinutes(5).TotalMilliseconds);
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
                if (!this.loaded)
                {
                    this.Load(null);
                    this.loaded = true;
                }

                return configuration;
            }
        }
        #endregion

        #region Methods
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
        /// Gets Items from Proxy
        /// </summary>
        /// <param name="state">Object</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Safety first.")]
        public void Load(object state)
        {
            try
            {
                lock (itemLock)
                {
                    if (null != application.Token)
                    {
                        var token = new Token()
                        {
                            ApplicationId = application.Token.ApplicationId,
                            ValidationKey = application.Token.ValidationKey,
                        };

                        Configuration[] configurations = null;
                        using (var proxy = new Proxy())
                        {
                            configurations = proxy.GetConfiguration(token);
                        }

                        if (null != configurations)
                        {
                            Parallel.ForEach<Configuration>(
                                configurations,
                                (c) =>
                                {
                                    if (!string.IsNullOrWhiteSpace(c.Key) && !configuration.ContainsKey(c.Key))
                                    {
                                        configuration.Add(c.Key, c.Value);
                                    }
                                    else
                                    {
                                        Trace.Write("Item already added to configuration: '{0}'.".FormatWithCulture(c.Key));
                                    }
                                });
                        }
                    }
                }

                this.loaded = true;
            }
            catch
            {
            }
        }
        #endregion
    }
}