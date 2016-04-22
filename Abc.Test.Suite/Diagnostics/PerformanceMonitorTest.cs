// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='PerformanceMonitorTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Diagnostics
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Abc.Diagnostics;
    using System;

    [TestClass]
    public class PerformanceMonitorTest
    {
        #region Helper Classes
        private class Perf : PerformanceMonitor
        {
            protected override void LogOccurrence(System.TimeSpan duration)
            {

            }
        }
        #endregion

        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new Perf();
        }

        [TestMethod]
        public void IsIDisposable()
        {
            Assert.IsNotNull(new Perf() as IDisposable);
        }

        [TestMethod]
        public void Dispose()
        {
            using (var perf = new Perf())
            {
            }
        }

        [TestMethod]
        public void AppendFormatNull()
        {
            using (var perf = new Perf())
            {
                perf.Append(null, StringHelper.ValidString(), StringHelper.NullEmptyWhiteSpace());
                Assert.IsNull(perf.Content);
            }
        }

        [TestMethod]
        public void AppendFormatDataNull()
        {
            using (var perf = new Perf())
            {
                perf.Append(StringHelper.ValidString(), null);
                Assert.IsNull(perf.Content);
            }
        }

        [TestMethod]
        public void AppendMessageNull()
        {
            using (var perf = new Perf())
            {
                perf.Append(null);
                Assert.IsNull(perf.Content);
            }
        }

        [TestMethod]
        public void AppendMesssage()
        {
            using (var perf = new Perf())
            {
                var data = StringHelper.ValidString();
                perf.Append(data);
                Assert.AreEqual<string>(data, perf.Content);
            }
        }

        [TestMethod]
        public void AppendFormat()
        {
            using (var perf = new Perf())
            {
                var format = "{0}{1}";
                var first = StringHelper.ValidString();
                var second = StringHelper.ValidString();
                var output = format.FormatWithCulture(first, second);
                perf.Append(format, first, second);
                Assert.AreEqual<string>(output, perf.Content);
            }
        }

        [TestMethod]
        public void MinimumDuration()
        {
            using (var perf = new Perf())
            {
                Assert.AreEqual<TimeSpan>(new TimeSpan(0, 0, 0, 1), perf.MinimumDuration);
            }
        }

        [TestMethod]
        public void Duration()
        {
            using (var perf = new Perf())
            {
                Assert.IsTrue(perf.Duration > TimeSpan.Zero);
            }
        }
        #endregion
    }
}