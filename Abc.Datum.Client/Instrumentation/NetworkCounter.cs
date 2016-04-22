// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='NetworkCounter.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Instrumentation
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Network Counter
    /// </summary>
    public class NetworkCounter : Counter
    {
        #region Members
        /// <summary>
        /// Maximum Throughput
        /// </summary>
        private readonly long maxThroughput;

        /// <summary>
        /// Last Value
        /// </summary>
        private float lastValue = float.NaN;
        #endregion

        #region Constructors
        /// <summary>
        /// Network Counter
        /// </summary>
        /// <param name="instance">Instance</param>
        /// <param name="bandwidth">Bandwidth</param>
        public NetworkCounter(string instance, long bandwidth)
            : base(CounterType.NetworkUsagePercentage, PerformanceCounterNames.NetworkCategory, PerformanceCounterNames.NetworkBytesTotalPerSec, instance)
        {
            if (string.IsNullOrWhiteSpace(instance))
            {
                throw new ArgumentException("instance");
            }
            else if (0 >= bandwidth)
            {
                throw new ArgumentException("bandwidth");
            }
            else
            {
                this.maxThroughput = bandwidth;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Next Value
        /// </summary>
        /// <returns>Next Value</returns>
        protected override float NextValue()
        {
            var value = this.counter.RawValue * 100 * 8;
            var returnValue = (value - lastValue) / this.maxThroughput;
            lastValue = value;
            return returnValue;
        }
        #endregion
    }
}