// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='GeneralMetricRow.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Data
{
    using System;
    using Abc.Azure;
    using Abc.Services.Contracts;
    using Microsoft.WindowsAzure.StorageClient;

    /// <summary>
    /// General Metric Row
    /// </summary>
    [AzureDataStore("GeneralMetricV2")]
    [CLSCompliant(false)]
    public class GeneralMetricRow : TableServiceEntity
    {
        #region Members
        /// <summary>
        /// Message Type
        /// </summary>
        public static readonly Guid Message = new Guid("07269ce0-df4f-464a-ae8a-667d2bd77d57");

        /// <summary>
        /// Error Type
        /// </summary>
        public static readonly Guid Error = new Guid("f78f8b0c-14c2-4c11-ac5c-44c8a79c55c3");

        /// <summary>
        /// Performance Type
        /// </summary>
        public static readonly Guid Performance = new Guid("9DA0C376-6FAF-4EB3-A47D-CA0AAF2FA421");

        /// <summary>
        /// Event Log Type
        /// </summary>
        public static readonly Guid EventLog = new Guid("3CD3BE84-AD2F-4094-8213-9C4C975B943E");

        /// <summary>
        /// Server Statistics
        /// </summary>
        public static readonly Guid ServerStatistics = new Guid("1093AD13-BFF4-4141-975E-5419A12A57C0");
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the GeneralMetricRow class
        /// </summary>
        public GeneralMetricRow()
            : base(Partition(), Row())
        {
            this.Time = DateTime.UtcNow;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets Time, sensitive to the second
        /// </summary>
        public DateTime Time
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Message Count
        /// </summary>
        public int MessageCount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Error Count
        /// </summary>
        public int ErrorCount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Performance Count
        /// </summary>
        public int PerformanceCount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Event Log Count
        /// </summary>
        public int EventLogCount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Server Statistics Count
        /// </summary>
        public int ServerStatisticsCount
        {
            get;
            set;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Creates the Partition Key
        /// </summary>
        /// <returns>Partition Key</returns>
        public static string Partition()
        {
            return "{0:yyyyMMdd}".FormatWithCulture(DateTime.UtcNow);
        }

        /// <summary>
        /// Shortent Date Time for Queries
        /// </summary>
        /// <param name="time">Time</param>
        /// <returns>Time, sensitive to the second</returns>
        public static long Shorten(DateTime time)
        {
            return time.Shorten(TimeSpan.TicksPerSecond).Ticks.RemoveTrailingZeros();
        }

        /// <summary>
        /// Row
        /// </summary>
        /// <returns>Row Key</returns>
        private static string Row()
        {
            return "{0}{1}".FormatWithCulture(AzureEnvironment.ServerName, Shorten(DateTime.UtcNow));
        }
        #endregion
    }
}