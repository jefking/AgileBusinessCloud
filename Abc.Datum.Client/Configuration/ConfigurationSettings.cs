// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='ConfigurationSettings.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc
{
    using System;
    using System.Collections.Generic;
    using Abc.Azure;
    using Abc.Azure.Configuration;
    using Abc.Configuration;
    using Abc.Underpinning;

    /// <summary>
    /// Configuration Settings
    /// </summary>
    public static class ConfigurationSettings
    {
        #region Members
        /// <summary>
        /// Log Performance Data Key
        /// </summary>
        public const string LogPerformanceKey = "Abc.LogPerformance";

        /// <summary>
        /// Minimum Duration Data Key
        /// </summary>
        public const string MinimumDurationKey = "Abc.LogPerformanceMinimumDuration";

        /// <summary>
        /// Server Statistics Key
        /// </summary>
        public const string ServerStatisticsKey = "Abc.ServerStatistics";

        /// <summary>
        /// Event Log Key
        /// </summary>
        public const string EventLogKey = "Abc.EventLog";

        /// <summary>
        /// Log Exceptions Key
        /// </summary>
        public const string LogExceptionsKey = "Abc.LogExceptions";

        /// <summary>
        /// Server Configuration Key
        /// </summary>
        public const string ServerConfigKey = "Abc.ServerConfig";

        /// <summary>
        /// Datum Remote Address Key
        /// </summary>
        public const string DatumRemoteAddressKey = "Abc.DatumRemoteAddress";

        /// <summary>
        /// Default Duration In Miliseconds
        /// </summary>
        public const int DefaultDurationInMilliseconds = 1000;

        /// <summary>
        /// Minimum Duration In Miliseconds
        /// </summary>
        public const int MinimumDurationInMilliseconds = 200;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes static members of the ConfigurationSettings class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Lifetime of application.")]
        static ConfigurationSettings()
        {
            var adaptors = new List<IConfigurationAdaptor>(3);
            if (LoadConfigurationFromServer)
            {
                var config = new ServerConfigurationAdaptor();
                config.Load(null);
                adaptors.Add(config);
            }

            if (AzureEnvironment.RoleIsAvailable)
            {
                adaptors.Add(new RoleEnvironmentAdaptor());
            }

            Settings.Instance.Add(adaptors);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets Datum Remote Address
        /// </summary>
        public static string DatumRemoteAddress
        {
            get
            {
                return Settings.Instance.Get(DatumRemoteAddressKey, "http://esb1.abcapi.ws/Datum.svc");
            }
        }

        /// <summary>
        /// Gets Minimum Duration
        /// </summary>
        public static TimeSpan MinimumDuration
        {
            get
            {
                var duration = Settings.Instance.Get<int>(MinimumDurationKey, DefaultDurationInMilliseconds);
                return TimeSpan.FromMilliseconds(duration > MinimumDurationInMilliseconds ? duration : MinimumDurationInMilliseconds);
            }
        }

        /// <summary>
        /// Gets a value indicating whether Log Performance
        /// </summary>
        public static bool LogPerformance
        {
            get
            {
                return Settings.Instance.Get<bool>(LogPerformanceKey, true);
            }
        }

        /// <summary>
        /// Gets a value indicating whether Log Exceptions
        /// </summary>
        public static bool LogExceptions
        {
            get
            {
                return Settings.Instance.Get<bool>(LogExceptionsKey, true);
            }
        }

        /// <summary>
        /// Gets a value indicating whether Instrument Server
        /// </summary>
        public static bool InstrumentServer
        {
            get
            {
                return Settings.Instance.Get<bool>(ServerStatisticsKey, true);
            }
        }

        /// <summary>
        /// Gets a value indicating whether to Log Windows Events
        /// </summary>
        public static bool LogWindowsEvents
        {
            get
            {
                return Settings.Instance.Get<bool>(EventLogKey, false);
            }
        }

        /// <summary>
        /// Gets a value indicating whether Load Configuration From Server
        /// </summary>
        public static bool LoadConfigurationFromServer
        {
            get
            {
                return Settings.Instance.Get<bool>(ServerConfigKey, false);
            }
        }

        /// <summary>
        /// Gets Application Identifer
        /// </summary>
        public static Guid ApplicationIdentifier
        {
            get
            {
                return Application.Identifier;
            }
        }

        /// <summary>
        /// Gets Settings
        /// </summary>
        public static Settings Settings
        {
            get
            {
                return Settings.Instance;
            }
        }
        #endregion
    }
}