// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='CounterTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Client
{
    using System;
    using Abc.Instrumentation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class CounterTest
    {
        #region Error Cases
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void LoadUnknown()
        {
            Counter.Load(CounterType.Unknown);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void LoadMultipleUnknown()
        {
            Counter.LoadMultiple(CounterType.Unknown);
        }
        #endregion

        #region Valid Cases
        [TestMethod]
        public void LoadNetwork()
        {
            var counters = Counter.LoadMultiple(CounterType.NetworkUsagePercentage);
            Assert.IsTrue(0 < counters.Count);
            foreach (var counter in counters)
            {
                Assert.AreEqual<CounterType>(CounterType.NetworkUsagePercentage, counter.CounterType);
                var samplePercentage = counter.SampledPercentage();
                Assert.IsTrue(samplePercentage >= 0);
                Assert.IsTrue(samplePercentage <= 100);
            }
        }

        [TestMethod]
        public void Memory()
        {
            var counter = Counter.Load(CounterType.MemoryUsagePercentage);
            Assert.AreEqual<CounterType>(CounterType.MemoryUsagePercentage, counter.CounterType);
            var samplePercentage = counter.SampledPercentage();
            Assert.IsTrue(samplePercentage >= 0);
            Assert.IsTrue(samplePercentage <= 100);
        }

        [TestMethod]
        public void RawValue()
        {
            var counter = Counter.Load(CounterType.MemoryUsagePercentage);
            Assert.AreEqual<CounterType>(CounterType.MemoryUsagePercentage, counter.CounterType);
            var samplePercentage = counter.RawValue;
            Assert.IsTrue(samplePercentage > 0);
        }

        [TestMethod]
        public void Disk()
        {
            var counter = Counter.Load(CounterType.DiskUsagePercentage);
            Assert.AreEqual<CounterType>(CounterType.DiskUsagePercentage, counter.CounterType);
            var samplePercentage = counter.SampledPercentage();
            Assert.IsTrue(samplePercentage >= 0, "zero " + samplePercentage);
            Assert.IsTrue(samplePercentage <= 100, "100? " + samplePercentage);
        }

        [TestMethod]
        public void Processor()
        {
            var counter = Counter.Load(CounterType.ProcessorUsagePercentage);
            Assert.AreEqual<CounterType>(CounterType.ProcessorUsagePercentage, counter.CounterType);
            var samplePercentage = counter.SampledPercentage();
            Assert.IsTrue(samplePercentage >= 0);
            Assert.IsTrue(samplePercentage <= 100);
        }

        [TestMethod]
        public void Dispose()
        {
            using (var counter = Counter.Load(CounterType.ProcessorUsagePercentage))
            {
            }
        }
        #endregion
    }
}