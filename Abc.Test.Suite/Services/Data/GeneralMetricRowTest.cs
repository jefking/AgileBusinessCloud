// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='GeneralMetricRowTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Data
{
    using System;
    using Abc.Services.Data;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class GeneralMetricRowTest
    {
        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new GeneralMetricRow();
        }

        [TestMethod]
        public void Time()
        {
            var data = new GeneralMetricRow();
            var item = DateTime.UtcNow;
            data.Time = item;
            Assert.AreEqual<DateTime>(item, data.Time);
        }

        [TestMethod]
        public void ServerStatisticsCount()
        {
            var random = new Random();
            var data = new GeneralMetricRow();
            var item = random.Next();
            data.ServerStatisticsCount = item;
            Assert.AreEqual<long>(item, data.ServerStatisticsCount);
        }

        [TestMethod]
        public void EventLogCount()
        {
            var random = new Random();
            var data = new GeneralMetricRow();
            var item = random.Next();
            data.EventLogCount = item;
            Assert.AreEqual<long>(item, data.EventLogCount);
        }

        [TestMethod]
        public void MessageCount()
        {
            var random = new Random();
            var data = new GeneralMetricRow();
            var item = random.Next();
            data.MessageCount = item;
            Assert.AreEqual<long>(item, data.MessageCount);
        }

        [TestMethod]
        public void PerformanceCount()
        {
            var random = new Random();
            var data = new GeneralMetricRow();
            var item = random.Next();
            data.PerformanceCount = item;
            Assert.AreEqual<long>(item, data.PerformanceCount);
        }

        [TestMethod]
        public void ErrorCount()
        {
            var random = new Random();
            var data = new GeneralMetricRow();
            var item = random.Next();
            data.ErrorCount = item;
            Assert.AreEqual<long>(item, data.ErrorCount);
        }

        [TestMethod]
        public void PartitionKey()
        {
            Assert.AreEqual<string>(GeneralMetricRow.Partition(), new GeneralMetricRow().PartitionKey);
        }

        [TestMethod]
        public void Message()
        {
            Assert.AreEqual<Guid>(new Guid("07269ce0-df4f-464a-ae8a-667d2bd77d57"), GeneralMetricRow.Message);
        }

        [TestMethod]
        public void ServerStatistics()
        {
            Assert.AreEqual<Guid>(new Guid("1093AD13-BFF4-4141-975E-5419A12A57C0"), GeneralMetricRow.ServerStatistics);
        }

        [TestMethod]
        public void Error()
        {
            Assert.AreEqual<Guid>(new Guid("f78f8b0c-14c2-4c11-ac5c-44c8a79c55c3"), GeneralMetricRow.Error);
        }

        [TestMethod]
        public void Performance()
        {
            Assert.AreEqual<Guid>(new Guid("9DA0C376-6FAF-4EB3-A47D-CA0AAF2FA421"), GeneralMetricRow.Performance);
        }

        [TestMethod]
        public void EventLog()
        {
            Assert.AreEqual<Guid>(new Guid("3CD3BE84-AD2F-4094-8213-9C4C975B943E"), GeneralMetricRow.EventLog);
        }
        #endregion
    }
}
