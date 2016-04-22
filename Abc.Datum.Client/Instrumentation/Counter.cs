// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='Counter.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Instrumentation
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using Abc.Logging;

    /// <summary>
    /// Counter
    /// </summary>
    public class Counter : IDisposable
    {
        #region Members
        /// <summary>
        /// Lockers
        /// </summary>
        private readonly object locker = new object();

        /// <summary>
        /// Logger
        /// </summary>
        private static readonly Logger log = new Logger();

        /// <summary>
        /// Performance Counter
        /// </summary>
        protected PerformanceCounter counter;

        /// <summary>
        /// Samples
        /// </summary>
        private IList<float> samples = new List<float>(350);

        /// <summary>
        /// Logging Timer
        /// </summary>
        private Timer timer;

        /// <summary>
        /// Disposed
        /// </summary>
        private bool disposed = false;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the Counter class.
        /// </summary>
        /// <param name="counterType">Counter Type</param>
        /// <param name="category">Category</param>
        /// <param name="counter">Counter</param>
        /// <param name="instance">Instance</param>
        protected Counter(CounterType counterType, string category, string counter, string instance = null)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                throw new ArgumentException("Invalid Cateogry", "category");
            }
            else if (string.IsNullOrWhiteSpace(counter))
            {
                throw new ArgumentException("Invalid Counter", "counter");
            }
            else if (string.IsNullOrWhiteSpace(instance))
            {
                this.counter = new PerformanceCounter(category, counter);
            }
            else
            {
                this.counter = new PerformanceCounter(category, counter, instance);
            }

            this.Sample(null);

            this.CounterType = counterType;
            this.timer = new Timer(this.Sample, this.samples, 0, 1000);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the Current Raw Value of the Counter
        /// </summary>
        public long RawValue
        {
            get
            {
                return this.counter.RawValue;
            }
        }

        /// <summary>
        /// Gets the CounterType
        /// </summary>
        public CounterType CounterType
        {
            get;
            private set;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Load Counter
        /// </summary>
        /// <param name="counter">Counter</param>
        /// <returns>Loaded Counter</returns>
        public static Counter Load(CounterType counter)
        {
            using (new PerformanceMonitor())
            {
                switch (counter)
                {
                    case CounterType.DiskUsagePercentage:
                        return new Counter(counter, PerformanceCounterNames.PhysicalDiskCategory, PerformanceCounterNames.DiskTime, PerformanceCounterNames.Total);
                    case CounterType.MemoryUsagePercentage:
                        return new Counter(counter, PerformanceCounterNames.MemoryCategory, PerformanceCounterNames.CommittedBytes);
                    case CounterType.ProcessorUsagePercentage:
                        return new Counter(counter, PerformanceCounterNames.ProcessorCategory, PerformanceCounterNames.ProcessorTime, PerformanceCounterNames.Total);
                    default:
                        throw new ArgumentException("Unknown Counter", "counter");
                }
            }
        }

        /// <summary>
        /// Load Multiple
        /// </summary>
        /// <param name="counter"></param>
        /// <returns></returns>
        public static IList<Counter> LoadMultiple(CounterType counter)
        {
            switch (counter)
            {
                case CounterType.NetworkUsagePercentage:
                    var counters = new List<Counter>();
                    try
                    {
                        var instances = new PerformanceCounterCategory(PerformanceCounterNames.NetworkCategory).GetInstanceNames();

                        foreach (var instance in instances)
                        {
                            try
                            {
                                var check = new Counter(counter, PerformanceCounterNames.NetworkCategory, PerformanceCounterNames.NetworkCurrentBandwidth, instance);
                                var sample = check.RawValue;
                                if (0 < sample)
                                {
                                    counters.Add(new NetworkCounter(instance, sample));
                                }
                                else
                                {
                                    Trace.WriteLine("Instance bandwidth is 0: '{0}', not collecting data from NIC.".FormatWithCulture(instance));
                                }
                            }
                            catch (Exception ex)
                            {
                                log.Log(ex);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Log(ex);
                    }

                    return counters;
                default:
                    throw new ArgumentException("Unknown Counter", "counter");
            }
        }

        /// <summary>
        /// Sampled Percentage, clears existing samples.
        /// </summary>
        /// <returns>Sampled Percentage</returns>
        public float SampledPercentage()
        {
            float percentage;
            using (new PerformanceMonitor())
            {
                lock (this.locker)
                {
                    percentage = this.samples.Count > 0 ? this.samples.Sum() / this.samples.Count : 0;
                    this.samples.Clear();
                }
            }

            return percentage;
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
                    if (null != this.timer)
                    {
                        this.timer.Dispose();
                    }

                    if (null != this.counter)
                    {
                        this.counter.Dispose();
                    }

                    this.counter = null;
                    this.timer = null;
                }

                this.disposed = true;
            }
        }

        /// <summary>
        /// Sample
        /// </summary>
        /// <param name="state">State</param>
        private void Sample(object state)
        {
            using (new PerformanceMonitor())
            {
                this.Sample(state as IList<float>);
            }
        }

        /// <summary>
        /// Sample
        /// </summary>
        /// <param name="samplesState">Samples State</param>
        /// <returns>Sample</returns>
        private float Sample(IList<float> samplesState)
        {
            using (new PerformanceMonitor())
            {
                float sample = 0;
                if (null != this.counter)
                {
                    try
                    {
                        sample = this.NextValue();

                        if (Single.NaN != sample)
                        {
                            lock (this.locker)
                            {
                                if (null != samplesState)
                                {
                                    samplesState.Add(sample);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Log(ex, EventTypes.Error);
                    }
                }

                return sample;
            }
        }

        /// <summary>
        /// Next Value from Counter
        /// </summary>
        /// <returns>Sampled Value</returns>
        protected virtual float NextValue()
        {
            return this.counter.NextValue();
        }
        #endregion
    }
}