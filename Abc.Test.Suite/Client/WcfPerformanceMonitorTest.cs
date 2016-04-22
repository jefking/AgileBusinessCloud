// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='WcfPerformanceMonitorTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Client
{
    using Abc.Configuration;
    using Abc.Web;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Linq;
    using System.ServiceModel.Dispatcher;
    using System.Threading;

    [TestClass]
    public class WcfPerformanceMonitorTest
    {
        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new WcfPerformanceMonitor(typeof(object));
        }

        [TestMethod]
        public void IsIParameterInspector()
        {
            Assert.IsNotNull(new WcfPerformanceMonitor(typeof(object)) as IParameterInspector);
        }

        [TestMethod]
        public void DurationTooSmall()
        {
            var operationName = Guid.NewGuid().ToString();
            var perf = new WcfPerformanceMonitor(typeof(WcfPerformanceMonitorTest));
            perf.BeforeCall(null, null);
            perf.AfterCall(operationName, null, null, null);

            var source = new Abc.Services.Core.LogCore();
            var query = new Abc.Services.Contracts.LogQuery()
            {
                ApplicationIdentifier = Settings.ApplicationIdentifier,
            };

            var className = typeof(WcfPerformanceMonitorTest).ToString();
            var item = (from data in source.SelectOccurrences(query)
                        where data.Class == className
                            && data.Method == operationName
                        select data).FirstOrDefault();

            Assert.IsNull(item);
        }

        [TestMethod]
        public void LogOccurrence()
        {
            var operationName = Guid.NewGuid().ToString();
            var perf = new WcfPerformanceMonitor(typeof(WcfPerformanceMonitorTest));
            perf.BeforeCall(null, null);

            Thread.Sleep(3000);

            perf.AfterCall(operationName, null, null, null);

            Thread.Sleep(3000);

            operationName = ' ' + operationName;

            var source = new Abc.Services.Core.LogCore();
            var query = new Abc.Services.Contracts.LogQuery()
            {
                ApplicationIdentifier = Settings.ApplicationIdentifier,
            };

            var className = typeof(WcfPerformanceMonitorTest).ToString();
            int i = 0;
            Abc.Services.Contracts.OccurrenceDisplay occurance = null;
            while (occurance == null && i < 50)
            {
                Thread.Sleep(50);
                occurance = (from data in source.SelectOccurrences(query)
                             where data.Class == className
                             && data.Method == operationName
                             select data).FirstOrDefault();
                i++;
            }

            Assert.IsNotNull(occurance, "Occurance should not be null");
            Assert.AreEqual<Guid>(Settings.ApplicationIdentifier, occurance.Token.ApplicationId, "Application Id should match");
            Assert.AreEqual<string>(Environment.MachineName, occurance.MachineName, "Machine Name should match");
            Assert.AreEqual<string>(operationName, occurance.Method, "Method should match");
            Assert.AreEqual<string>(this.GetType().ToString(), occurance.Class, "Type should match");
            Assert.AreEqual<int>(Thread.CurrentThread.ManagedThreadId, occurance.ThreadId, "Thread Id should match");
        }
        #endregion

        #region Helper Methods
        [TestInitialize]
        public void Initialize()
        {
            try
            {
                var token = new Abc.Services.Contracts.Token()
                {
                    ApplicationId = Settings.ApplicationIdentifier,
                };
                var perf = new Abc.Services.Contracts.Occurrence()
                {
                    Token = token,
                    OccurredOn = DateTime.UtcNow,
                };

                var source = new Abc.Services.Core.LogCore();
                source.Delete(perf);
            }
            catch
            {
            }
        }
        #endregion
    }
}