// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='Sampler.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Instrumentation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Abc.Logging;
    using Abc.Logging.Datum;
    using Abc.Underpinning;

    /// <summary>
    /// Sampler
    /// </summary>
    public class Sampler : IDisposable
    {
        #region Members
        /// <summary>
        /// Application
        /// </summary>
        private static readonly Application application = new Application();

        /// <summary>
        /// Logger
        /// </summary>
        private static readonly Logger log = new Logger();

        /// <summary>
        /// Disposed
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Memory Counter
        /// </summary>
        private Counter memory = Counter.Load(CounterType.MemoryUsagePercentage);

        /// <summary>
        /// Disk Counter
        /// </summary>
        private Counter disk = Counter.Load(CounterType.DiskUsagePercentage);

        /// <summary>
        /// Processor Counter
        /// </summary>
        private Counter processor = Counter.Load(CounterType.ProcessorUsagePercentage);

        /// <summary>
        /// Network Counters
        /// </summary>
        private IList<Counter> network = Counter.LoadMultiple(CounterType.NetworkUsagePercentage);
        #endregion

        #region Methods
        /// <summary>
        /// Store Samples
        /// </summary>
        /// <param name="state">State</param>
        public void StoreSamples(object state)
        {
            using (new PerformanceMonitor())
            {
                try
                {
                    var statistics = new ServerStatisticSet()
                    {
                        CpuUsagePercentage = this.processor.SampledPercentage(),
                        MemoryUsagePercentage = this.memory.SampledPercentage(),
                        PhysicalDiskUsagePercentage = this.disk.SampledPercentage(),
                        NetworkPercentages = network.Select(n => n.SampledPercentage()).Where(n => n > 0).ToArray(),
                        OccurredOn = DateTime.UtcNow,
                        MachineName = Environment.MachineName,
                        Token = application.GetToken(),
                        DeploymentId = Abc.Azure.AzureEnvironment.DeploymentId,
                    };

                    MessageHandler.Instance.Queue(statistics);
                }
                catch (Exception ex)
                {
                    log.Log(ex, Abc.Logging.EventTypes.Error);
                }
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing">Disposing</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (null != this.memory)
                    {
                        this.memory.Dispose();
                    }

                    if (null != this.disk)
                    {
                        this.disk.Dispose();
                    }

                    if (null != this.processor)
                    {
                        this.processor.Dispose();
                    }

                    if (null != this.network)
                    {
                        foreach (var nic in network)
                        {
                            nic.Dispose();
                        }
                    }

                    this.disk = null;
                    this.processor = null;
                    this.memory = null;
                    this.network = null;
                }

                this.disposed = true;
            }
        }
        #endregion
    }
}
