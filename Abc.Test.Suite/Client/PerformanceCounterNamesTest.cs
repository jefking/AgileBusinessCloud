// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='PerformanceCounterNamesTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Client
{
    using Abc.Instrumentation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PerformanceCounterNamesTest
    {
        #region Valid Cases
        [TestMethod]
        public void PhysicalDiskCategory()
        {
            Assert.AreEqual<string>("PhysicalDisk", PerformanceCounterNames.PhysicalDiskCategory);
        }

        [TestMethod]
        public void ProcessorCategory()
        {
            Assert.AreEqual<string>("Processor", PerformanceCounterNames.ProcessorCategory);
        }

        [TestMethod]
        public void MemoryCategory()
        {
            Assert.AreEqual<string>("Memory", PerformanceCounterNames.MemoryCategory);
        }

        [TestMethod]
        public void DiskTime()
        {
            Assert.AreEqual<string>("% Disk Time", PerformanceCounterNames.DiskTime);
        }

        [TestMethod]
        public void ProcessorTime()
        {
            Assert.AreEqual<string>("% Processor Time", PerformanceCounterNames.ProcessorTime);
        }

        [TestMethod]
        public void Total()
        {
            Assert.AreEqual<string>("_Total", PerformanceCounterNames.Total);
        }

        [TestMethod]
        public void CommittedBytes()
        {
            Assert.AreEqual<string>("% Committed Bytes In Use", PerformanceCounterNames.CommittedBytes);
        }

        [TestMethod]
        public void NetworkCategory()
        {
            Assert.AreEqual<string>("Network Interface", PerformanceCounterNames.NetworkCategory);
        }

        [TestMethod]
        public void NetworkBytesTotalPerSec()
        {
            Assert.AreEqual<string>("Bytes Total/sec", PerformanceCounterNames.NetworkBytesTotalPerSec);
        }

        [TestMethod]
        public void NetworkCurrentBandwidth()
        {
            Assert.AreEqual<string>("Current Bandwidth", PerformanceCounterNames.NetworkCurrentBandwidth);
        }
        #endregion
    }
}