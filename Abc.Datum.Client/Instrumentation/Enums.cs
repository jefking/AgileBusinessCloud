// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='Enums.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Instrumentation
{
    /// <summary>
    /// Counters
    /// </summary>
    public enum CounterType
    {
        /// <summary>
        /// Unknown Counter
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Memory Usage Percentage Counter
        /// </summary>
        MemoryUsagePercentage = 1,

        /// <summary>
        /// Processor Usage Percentage Counter
        /// </summary>
        ProcessorUsagePercentage = 2,

        /// <summary>
        /// Disk Usage Percentage Counter
        /// </summary>
        DiskUsagePercentage = 3,

        /// <summary>
        /// Network Usage Percentage Counter
        /// </summary>
        NetworkUsagePercentage = 4,
    }
}