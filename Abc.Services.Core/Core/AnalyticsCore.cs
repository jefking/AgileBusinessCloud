// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='AnalyticsCore.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Core
{
    using Abc.Azure;
    using Abc.Configuration;
    using Abc.Services.Contracts;
    using Abc.Threading;
    using System;

    /// <summary>
    /// Analytics Core
    /// </summary>
    public class AnalyticsCore : DataCore
    {
        #region Members
        /// <summary>
        /// Bytes Table
        /// </summary>
        private readonly AzureTable<BytesStoredData> bytesTable = new AzureTable<BytesStoredData>(ServerConfiguration.Default);

        /// <summary>
        /// Throughput Timer
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Thread")]
        private static readonly SafeTimer throughputTimer = new SafeTimer(UpdateThroughput, ThroughputError, throughput, TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(1));

        /// <summary>
        /// Throughput
        /// </summary>
        private static Throughput throughput = new Throughput();

        /// <summary>
        /// Last Request
        /// </summary>
        private static DateTime lastRequest = DateTime.UtcNow;
        #endregion

        #region Methods
        /// <summary>
        /// Gets the Current Throughput
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "By Design")]
        public Throughput Current
        {
            get
            {
                lastRequest = DateTime.UtcNow;
                return throughput;
            }
        }

        /// <summary>
        /// Update Throughput
        /// </summary>
        /// <param name="obj">Data</param>
        private static void UpdateThroughput(object obj)
        {
            const int Frequency = 1;
            if (lastRequest > DateTime.UtcNow.AddSeconds(-30))
            {
                throughput = Logging.SelectSecondsOf(Frequency);
            }
        }

        /// <summary>
        /// Throughput Error
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="args">Arguments</param>
        private static void ThroughputError(object sender, EventArgs<Exception> args)
        {
            Logging.Log(args.Argument, EventTypes.Warning, (int)DatumFault.Analytics);
        }
        #endregion
    }
}