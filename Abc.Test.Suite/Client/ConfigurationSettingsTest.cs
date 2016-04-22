// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='ConfigurationSettingsTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Client
{
    using Abc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ConfigurationSettingsTest
    {
        #region Valid Cases
        /// <summary>
        /// Minimum Duration Default
        /// </summary>
        [TestMethod]
        public void MinimumDurationDefault()
        {
            Assert.AreEqual<int>(1000, ConfigurationSettings.DefaultDurationInMilliseconds);
        }

        [TestMethod]
        public void MinimumDurationInMilliseconds()
        {
            Assert.AreEqual<int>(200, ConfigurationSettings.MinimumDurationInMilliseconds);
        }

        [TestMethod]
        public void DatumRemoteAddress()
        {
            Assert.AreEqual<string>("http://localhost:8001/Datum.svc", ConfigurationSettings.DatumRemoteAddress);
        }

        [TestMethod]
        public void LogPerformanceKey()
        {
            Assert.AreEqual<string>("Abc.LogPerformance", ConfigurationSettings.LogPerformanceKey);
        }

        [TestMethod]
        public void DatumRemoteAddressKey()
        {
            Assert.AreEqual<string>("Abc.DatumRemoteAddress", ConfigurationSettings.DatumRemoteAddressKey);
        }

        [TestMethod]
        public void LogPerformanceMinimumDuration()
        {
            Assert.AreEqual<string>("Abc.LogPerformanceMinimumDuration", ConfigurationSettings.MinimumDurationKey);
        }

        [TestMethod]
        public void LogExceptionsKey()
        {
            Assert.AreEqual<string>("Abc.LogExceptions", ConfigurationSettings.LogExceptionsKey);
        }

        [TestMethod]
        public void ServerStatisticsKey()
        {
            Assert.AreEqual<string>("Abc.ServerStatistics", ConfigurationSettings.ServerStatisticsKey);
        }

        [TestMethod]
        public void EventLogKey()
        {
            Assert.AreEqual<string>("Abc.EventLog", ConfigurationSettings.EventLogKey);
        }

        [TestMethod]
        public void DurationInMilliseconds()
        {
            Assert.AreEqual<int>(200, (int)ConfigurationSettings.MinimumDuration.TotalMilliseconds);
        }

        [TestMethod]
        public void LogPerformance()
        {
            Assert.IsTrue(ConfigurationSettings.LogPerformance);
        }

        [TestMethod]
        public void LogExceptions()
        {
            Assert.IsTrue(ConfigurationSettings.LogExceptions);
        }

        [TestMethod]
        public void InstrumentServer()
        {
            Assert.IsTrue(ConfigurationSettings.InstrumentServer);
        }

        [TestMethod]
        public void LogWindowsEvents()
        {
            Assert.IsFalse(ConfigurationSettings.LogWindowsEvents);
        }

        [TestMethod]
        public void LoadConfigurationFromServer()
        {
            Assert.IsTrue(ConfigurationSettings.LoadConfigurationFromServer);
        }

        [TestMethod]
        public void ServerConfigKey()
        {
            Assert.AreEqual<string>("Abc.ServerConfig", ConfigurationSettings.ServerConfigKey);
        }
        #endregion
    }
}