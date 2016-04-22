// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='PerformanceMonitorClientTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite
{
    using System;
    using System.Linq;
    using System.Threading;
    using Abc.Configuration;
    using Abc.Logging;
    using Abc.Underpinning;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Performance Monitor Client Test
    /// </summary>
    [TestClass]
    public class PerformanceMonitorClientTest
    {
        #region Members
        /// <summary>
        /// Application
        /// </summary>
        private readonly Application application = new Application();
        #endregion

        #region Valid Cases
        [TestMethod]
        public void DurationTooSmall()
        {
            var message = Guid.NewGuid().ToString();

            using (var perf = new PerformanceMonitor())
            {
                Assert.IsNull(perf.Content, "Content should be null");
                perf.Append(message);
                Assert.AreEqual<string>(message, perf.Content, "Message should match");
            }

            var source = new Abc.Services.Core.LogCore();
            var query = new Abc.Services.Contracts.LogQuery()
            {
                ApplicationIdentifier = Application.Identifier,
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

        [TestMethod]
        [Ignore]
        public void LogOccurrence()
        {
            var message = Guid.NewGuid().ToString();
            using (var perf = new PerformanceMonitor())
            {
                Assert.IsNull(perf.Content, "Content should be null");
                Thread.Sleep(perf.MinimumDuration.Add(new TimeSpan(0, 0, 2)));
                perf.Append(message);
            }
            
            var source = new Abc.Services.Core.LogCore();
            var query = new Abc.Services.Contracts.LogQuery()
            {
                ApplicationIdentifier = Settings.ApplicationIdentifier,
                From = DateTime.UtcNow.AddMinutes(-5),
            };

            int i = 0;
            Abc.Services.Contracts.OccurrenceDisplay occurance = null;
            while (occurance == null && i < 50)
            {
                Thread.Sleep(100);
                occurance = (from data in source.SelectOccurrences(query)
                           where message == data.Message
                           select data).FirstOrDefault();
                i++;
            }

            Assert.IsNotNull(occurance, "Occurrence should not be null");
            Assert.AreEqual<Guid>(Application.Identifier, occurance.Token.ApplicationId, "Application Id should match");
            Assert.AreEqual<string>(Environment.MachineName, occurance.MachineName, "Machine Name should match");
            Assert.AreEqual<string>(message, occurance.Message, "Message should match");
            Assert.AreEqual<string>("Void LogOccurrence()", occurance.Method, "Method should match");
            Assert.AreEqual<string>(this.GetType().ToString(), occurance.Class, "Type should match");
            Assert.AreEqual<int>(Thread.CurrentThread.ManagedThreadId, occurance.ThreadId, "Thread Id should match");
        }

        /// <summary>
        /// Performance Monitor Too High
        /// </summary>
        [TestMethod]
        public void AppendComment()
        {
            var comment = "this is my comment";
            using (var perf = new PerformanceMonitor())
            {
                Assert.IsNull(perf.Content, "Content should be null");
                perf.Append(comment);
                Assert.AreEqual<string>(comment, perf.Content);
            }
        }

        /// <summary>
        /// Performance Monitor Too High
        /// </summary>
        [TestMethod]
        public void AppendStringFormatComment()
        {
            var comment = "this is my comment";
            using (var perf = new PerformanceMonitor())
            {
                Assert.IsNull(perf.Content, "Content should be null");
                perf.Append("{0}:{0}", comment);
                Assert.AreEqual<string>(comment + ':' + comment, perf.Content);
            }
        }

        [TestMethod]
        public void MinimumDuration()
        {
            Assert.AreEqual<TimeSpan>(ConfigurationSettings.MinimumDuration, new PerformanceMonitor().MinimumDuration);
        }
        #endregion
    }
}