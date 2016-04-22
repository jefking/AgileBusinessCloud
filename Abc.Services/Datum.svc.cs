// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='Datum.svc.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Abc.Services.Contracts;
    using Abc.Services.Core;
    using Abc.Services.Validation;

    /// <summary>
    /// Datum Web Service (WCF), Data Storage for baseline and measurement of application during runtime.
    /// </summary>
    public class Datum : IDatum
    {
        #region Members
        /// <summary>
        /// Log Core
        /// </summary>
        private static readonly LogCore logCore = new LogCore();

        /// <summary>
        /// Application
        /// </summary>
        private static readonly Abc.Underpinning.Administration.Application application = new Abc.Underpinning.Administration.Application();

        /// <summary>
        /// Application Core
        /// </summary>
        private static readonly ApplicationCore applicationCore = new ApplicationCore();

        /// <summary>
        /// Token Validator
        /// </summary>
        private Validator<Token> tokenValidator = new Validator<Token>();
        #endregion

        #region Methods
        /// <summary>
        /// Log Exception
        /// </summary>
        /// <param name="exception">Exception</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validation is occuring")]
        public void LogException(ErrorItem exception)
        {
            try
            {
                using (new PerformanceMonitor())
                {
                    var validator = new Validator<ErrorItem>();
                    if (!validator.IsValid(exception, true))
                    {
                        logCore.Log(validator.AllMessages(exception));
                    }
                    else if (!this.tokenValidator.IsValid(exception.Token, true))
                    {
                        logCore.Log(this.tokenValidator.AllMessages(exception.Token));
                    }
                    else
                    {
                        application.Validate(exception.Token.ApplicationId, exception.Token.ValidationKey);

                        logCore.StoreByteCount(exception.Token.ApplicationId, DataCostType.Ingress, exception);

                        logCore.Log(exception);
                    }
                }
            }
            catch (Exception ex)
            {
                logCore.Log(ex, EventTypes.Error, (int)DatumFault.LogException);
                throw;
            }
        }

        /// <summary>
        /// Log Server Statistic Set
        /// </summary>
        /// <param name="data">Server Statistic Set</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validation is occuring.")]
        public void LogServerStatisticSet(ServerStatisticSet data)
        {
            try
            {
                using (new PerformanceMonitor())
                {
                    var validator = new Validator<ServerStatisticSet>();
                    if (!validator.IsValid(data))
                    {
                        logCore.Log(validator.AllMessages(data));
                    }
                    else if (!this.tokenValidator.IsValid(data.Token))
                    {
                        logCore.Log(this.tokenValidator.AllMessages(data.Token));
                    }
                    else
                    {
                        application.Validate(data.Token.ApplicationId, data.Token.ValidationKey);

                        logCore.StoreByteCount(data.Token.ApplicationId, DataCostType.Ingress, data);

                        logCore.Log(data);
                    }
                }
            }
            catch (Exception ex)
            {
                logCore.Log(ex, EventTypes.Error, (int)DatumFault.LogServerStatisticSet);
                throw;
            }
        }

        /// <summary>
        /// Log Message
        /// </summary>
        /// <param name="message">Message</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validation is occuring")]
        public void LogMessage(Message message)
        {
            try
            {
                using (new PerformanceMonitor())
                {
                    var validator = new Validator<Message>();
                    if (!validator.IsValid(message, true))
                    {
                        logCore.Log(validator.AllMessages(message));
                    }
                    else if (!this.tokenValidator.IsValid(message.Token, true))
                    {
                        logCore.Log(this.tokenValidator.AllMessages(message.Token));
                    }
                    else
                    {
                        application.Validate(message.Token.ApplicationId, message.Token.ValidationKey);

                        logCore.StoreByteCount(message.Token.ApplicationId, DataCostType.Ingress, message);

                        logCore.Log(message);
                    }
                }
            }
            catch (Exception ex)
            {
                logCore.Log(ex, EventTypes.Error, (int)DatumFault.LogMessage);
                throw;
            }
        }

        /// <summary>
        /// Log Performance
        /// </summary>
        /// <param name="occurrence">Occurrence</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validation is occuring")]
        public void LogPerformance(Occurrence occurrence)
        {
            try
            {
                using (new PerformanceMonitor())
                {
                    var validator = new Validator<Occurrence>();
                    if (!validator.IsValid(occurrence, true))
                    {
                        logCore.Log(validator.AllMessages(occurrence));
                    }
                    else if (!this.tokenValidator.IsValid(occurrence.Token, true))
                    {
                        logCore.Log(this.tokenValidator.AllMessages(occurrence.Token));
                    }
                    else
                    {
                        application.Validate(occurrence.Token.ApplicationId, occurrence.Token.ValidationKey);

                        logCore.StoreByteCount(occurrence.Token.ApplicationId, DataCostType.Ingress, occurrence);

                        logCore.Log(occurrence);
                    }
                }
            }
            catch (Exception ex)
            {
                logCore.Log(ex, EventTypes.Error, (int)DatumFault.LogPerformance);
                throw;
            }
        }

        /// <summary>
        /// Log Event Item
        /// </summary>
        /// <param name="item">Item</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validation is occuring")]
        public void LogEventItem(EventLogItem item)
        {
            try
            {
                using (new PerformanceMonitor())
                {
                    var validator = new Validator<EventLogItem>();
                    if (!validator.IsValid(item, true))
                    {
                        logCore.Log(validator.AllMessages(item));
                    }
                    else if (!this.tokenValidator.IsValid(item.Token, true))
                    {
                        logCore.Log(this.tokenValidator.AllMessages(item.Token));
                    }
                    else
                    {
                        application.Validate(item.Token.ApplicationId, item.Token.ValidationKey);

                        logCore.StoreByteCount(item.Token.ApplicationId, DataCostType.Ingress, item);

                        logCore.Log(item);
                    }
                }
            }
            catch (Exception ex)
            {
                logCore.Log(ex, EventTypes.Error, (int)DatumFault.LogEventItem);
                throw;
            }
        }

        /// <summary>
        /// Get Configuration
        /// </summary>
        /// <param name="configuration">Configuration</param>
        /// <returns>Application Configuration</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Code contracts")]
        public IEnumerable<Abc.Services.Contracts.Configuration> GetConfiguration(Abc.Services.Contracts.Configuration configuration)
        {
            IEnumerable<Abc.Services.Contracts.Configuration> returnData = null;

            try
            {
                using (new PerformanceMonitor())
                {
                    var validator = new Validator<Abc.Services.Contracts.Configuration>();
                    if (!validator.IsValid(configuration, true))
                    {
                        logCore.Log(validator.AllMessages(configuration));
                    }
                    else if (!this.tokenValidator.IsValid(configuration.Token, true))
                    {
                        logCore.Log(this.tokenValidator.AllMessages(configuration.Token));
                    }
                    else
                    {
                        application.Validate(configuration.Token.ApplicationId, configuration.Token.ValidationKey);

                        logCore.StoreByteCount(configuration.Token.ApplicationId, DataCostType.Ingress, configuration);

                        returnData = applicationCore.Get(configuration);

                        logCore.StoreByteCount(configuration.Token.ApplicationId, DataCostType.Egress, returnData);
                    }
                }
            }
            catch (Exception ex)
            {
                logCore.Log(ex, EventTypes.Error, (int)DatumFault.GetConfigurationUnknownError);
                throw;
            }

            return returnData;
        }
        #endregion
    }
}