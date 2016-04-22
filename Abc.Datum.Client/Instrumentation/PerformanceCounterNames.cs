// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='PerformanceCounterNames.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Instrumentation
{
    /// <summary>
    /// Performance Counter Names
    /// </summary>
    public struct PerformanceCounterNames
    {
        #region Members
        /// <summary>
        /// Physical Disk Category
        /// </summary>
        public const string PhysicalDiskCategory = "PhysicalDisk";
        
        /// <summary>
        /// Processor Category
        /// </summary>
        public const string ProcessorCategory = "Processor";

        /// <summary>
        /// Memory Category
        /// </summary>
        public const string MemoryCategory = "Memory";

        /// <summary>
        /// Network Category
        /// </summary>
        public const string NetworkCategory = "Network Interface";

        /// <summary>
        /// Disk Time
        /// </summary>
        public const string DiskTime = "% Disk Time";

        /// <summary>
        /// Processor Time
        /// </summary>
        public const string ProcessorTime = "% Processor Time";

        /// <summary>
        /// Total
        /// </summary>
        public const string Total = "_Total";

        /// <summary>
        /// Committed Bytes
        /// </summary>
        public const string CommittedBytes = "% Committed Bytes In Use";

        /// <summary>
        /// Network Bytes Total/Sec
        /// </summary>
        public const string NetworkBytesTotalPerSec = "Bytes Total/sec";

        /// <summary>
        /// Network Current Bandwidth
        /// </summary>
        public const string NetworkCurrentBandwidth = "Current Bandwidth";
        #endregion
    }
}