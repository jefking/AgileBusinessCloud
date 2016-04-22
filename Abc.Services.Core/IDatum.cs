// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='IDatum.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services
{
    using System.Collections.Generic;
    using System.ServiceModel;
    using Abc.Services.Contracts;

    /// <summary>
    /// Interface for Datum Web Service (WCF)
    /// Data Storage for baseline and measurement of application during runtime.
    /// </summary>
    [ServiceContract]// (ProtectionLevel = ProtectionLevel.EncryptAndSign)]
    public interface IDatum
    {
        #region Methods
        /// <summary>
        /// Log Exception
        /// </summary>
        /// <param name="exception">Exception</param>
        [OperationContract]
        void LogException(ErrorItem exception);

        /// <summary>
        /// Log Message
        /// </summary>
        /// <param name="message">Message</param>
        [OperationContract]
        void LogMessage(Message message);

        /// <summary>
        /// Log Server Statistic Set
        /// </summary>
        /// <param name="data">Server Statistic Set</param>
        [OperationContract]
        void LogServerStatisticSet(ServerStatisticSet data);

        /// <summary>
        /// Log Performance
        /// </summary>
        /// <param name="occurrence">Occurrence</param>
        [OperationContract]
        void LogPerformance(Occurrence occurrence);

        /// <summary>
        /// Log Event Item
        /// </summary>
        /// <param name="item">Event Log Item</param>
        [OperationContract]
        void LogEventItem(EventLogItem item);

        /// <summary>
        /// Get Configuration
        /// </summary>
        /// <param name="configuration">Configuration</param>
        /// <returns>Application Configuration</returns>
        [OperationContract]
        IEnumerable<Abc.Services.Contracts.Configuration> GetConfiguration(Abc.Services.Contracts.Configuration configuration);
        #endregion
    }
}