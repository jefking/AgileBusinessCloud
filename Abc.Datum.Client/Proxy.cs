// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='Proxy.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Logging
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.ServiceModel;

    /// <summary>
    /// Proxy Class
    /// </summary>
    internal class Proxy : IDisposable
    {
        #region Members
        /// <summary>
        /// Datum Client for accessing the Datum web service
        /// </summary>
        private readonly Datum.DatumClient client;

        /// <summary>
        /// Logger
        /// </summary>
        private static readonly Logger logger = new Logger();

        /// <summary>
        /// Remote Address
        /// </summary>
        private readonly EndpointAddress remoteAddress = new EndpointAddress(ConfigurationSettings.DatumRemoteAddress);

        /// <summary>
        /// Binding
        /// </summary>
        private readonly WSHttpBinding binding = new WSHttpBinding(SecurityMode.None);

        /// <summary>
        /// Track whether Dispose has been called.
        /// </summary>
        private volatile bool disposed = false;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the Proxy class.
        /// </summary>
        public Proxy()
        {
            this.client = new Datum.DatumClient(this.binding, this.remoteAddress);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Dispose of class cleanly
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Log Error
        /// </summary>
        /// <param name="error">Error Item</param>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Safety first.")]
        internal void Log(Datum.ErrorItem error)
        {
            try
            {
                this.client.LogExceptionAsync(error);
            }
            catch
            {
                // Do not log exception, recusive stack overflow could occur
                this.client.Abort();
            }
        }

        /// <summary>
        /// Log Occurance
        /// </summary>
        /// <param name="occurrence">Occurrence</param>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Safety first.")]
        internal void Log(Datum.Occurrence occurrence)
        {
            try
            {
                this.client.LogPerformanceAsync(occurrence);
            }
            catch (Exception ex)
            {
                this.client.Abort();

                logger.Log(ex);
            }
        }

        /// <summary>
        /// Log Occurance
        /// </summary>
        /// <param name="message">Message</param>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Safety first.")]
        internal void Log(Datum.Message message)
        {
            try
            {
                this.client.LogMessageAsync(message);
            }
            catch (Exception ex)
            {
                this.client.Abort();

                logger.Log(ex);
            }
        }

        /// <summary>
        /// Log Server Statistic Set
        /// </summary>
        /// <param name="data">Server Statistic Set</param>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Safety first.")]
        internal void Log(Datum.ServerStatisticSet data)
        {
            try
            {
                this.client.LogServerStatisticSetAsync(data);
            }
            catch (Exception ex)
            {
                this.client.Abort();

                logger.Log(ex);
            }
        }

        /// <summary>
        /// Log Event Log Item
        /// </summary>
        /// <param name="item">Event Log Item</param>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Safety first.")]
        internal void Log(Datum.EventLogItem item)
        {
            try
            {
                this.client.LogEventItemAsync(item);
            }
            catch (Exception ex)
            {
                this.client.Abort();

                logger.Log(ex);
            }
        }

        /// <summary>
        /// Get Configuration
        /// </summary>
        /// <param name="token">Token</param>
        /// <returns>Configuration Items</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Safety first.")]
        internal Datum.Configuration[] GetConfiguration(Datum.Token token)
        {
            Datum.Configuration[] configuration = null;
            try
            {
                var config = new Datum.Configuration()
                {
                    Token = token,
                };

                configuration = this.client.GetConfiguration(config);
            }
            catch (Exception ex)
            {
                this.client.Abort();

                logger.Log(ex);
            }

            return configuration;
        }

        /// <summary>
        /// Dispose in a certian context
        /// </summary>
        /// <seealso cref="http://msdn.microsoft.com/en-us/library/system.idisposable.dispose.aspx"/>
        /// <param name="disposing">Disposing</param>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Safety first.")]
        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    try
                    {
                        this.client.Close();
                    }
                    catch
                    {
                        this.client.Abort();
                    }
                }

                this.disposed = true;
            }
        }
        #endregion
    }
}