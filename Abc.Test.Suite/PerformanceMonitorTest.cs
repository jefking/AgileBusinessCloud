// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='PerformanceMonitorTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite
{
    using System;
    using System.Linq;
    using System.Threading;
    using Abc.Configuration;
    using Abc.Services;
    using Abc.Services.Core;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Performance Monitor Test Class
    /// </summary>
    [TestClass]
    public class PerformanceMonitorTest
    {
        #region Valid Cases
        [TestMethod]
        public void MinimumDuration()
        {
            Assert.AreEqual<TimeSpan>(new TimeSpan(0, 0, 1), new PerformanceMonitor().MinimumDuration);
        }

        /// <summary>
        /// Less Than Min
        /// </summary>
        [TestMethod]
        public void LessThanMin()
        {
            string message = Guid.NewGuid().ToString();
            using (var perf = new PerformanceMonitor())
            {
                Assert.IsNull(perf.Content, "Content should be null");
                perf.Append(message);
                Assert.AreEqual<string>(message, perf.Content, "Message should match");
            }

            var source = new LogCore();
            var query = new Abc.Services.Contracts.LogQuery()
            {
                ApplicationIdentifier = ConfigurationSettings.ApplicationIdentifier,
            };
            var items = source.SelectOccurrences(query);
            foreach (var item in items)
            {
                if (item.Message == message)
                {
                    Assert.Fail("Perf occurance was saved.");
                }
            }
        }

        /// <summary>
        /// Less Than Min
        /// </summary>
        [TestMethod]
        public void LogOccurrence()
        {
            var message = Guid.NewGuid().ToString();
            using (var perf = new PerformanceMonitor())
            {
                Assert.IsNull(perf.Content, "Content should be null");
                Thread.Sleep(2000);
                perf.Append(message);
            }

            var source = new LogCore();
            var query = new Abc.Services.Contracts.LogQuery()
            {
                ApplicationIdentifier = Settings.ApplicationIdentifier,
            };

            int i = 0;
            Abc.Services.Contracts.OccurrenceDisplay occurance = null;
            while (occurance == null && i < 50)
            {
                Thread.Sleep(50);
                occurance = (from data in source.SelectOccurrences(query)
                             where data.Message == message
                             select data).FirstOrDefault();
                i++;
            }

            Assert.IsNotNull(occurance, "Occurance should not be null");
            Assert.AreEqual<Guid>(ConfigurationSettings.ApplicationIdentifier, occurance.Token.ApplicationId, "Application Id should match");
            Assert.AreEqual<string>(Environment.MachineName, occurance.MachineName, "Machine Name should match");
            Assert.AreEqual<string>(message, occurance.Message, "Message should match");
            Assert.AreEqual<string>("Void LogOccurrence()", occurance.Method, "Method should match");
            Assert.AreEqual<string>(this.GetType().ToString(), occurance.Class, "Type should match");
            Assert.AreEqual<int>(Thread.CurrentThread.ManagedThreadId, occurance.ThreadId, "Thread Id should match");
        }

        /// <summary>
        /// Append Comment
        /// </summary>
        [TestMethod]
        public void AppendComment()
        {
            string comment = "this is my comment";
            using (PerformanceMonitor perf = new PerformanceMonitor())
            {
                Assert.IsNull(perf.Content, "Content should be null");
                perf.Append(comment);
                Assert.AreEqual<string>(comment, perf.Content);
            }
        }

        /// <summary>
        /// Append string Format Comment
        /// </summary>
        [TestMethod]
        public void AppendStringFormatComment()
        {
            using (PerformanceMonitor perf = new PerformanceMonitor())
            {
                string comment = "this is my comment";
                Assert.IsNull(perf.Content, "Content should be null");
                perf.Append("{0}:{0}", comment);
                Assert.AreEqual<string>(comment + ':' + comment, perf.Content);
                Thread.Sleep(5);
                Assert.IsTrue(0 < perf.Duration.Ticks, "Duration In Ticks should be set");
            }
        }
        #endregion
    }
}