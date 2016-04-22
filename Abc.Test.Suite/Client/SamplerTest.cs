// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='SamplerTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Client
{
    using System;
    using System.Linq;
    using System.Threading;
    using Abc.Azure;
    using Abc.Configuration;
    using Abc.Instrumentation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Sampler Test
    /// </summary>
    [TestClass]
    public class SamplerTest
    {
        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new Sampler();
        }

        [TestMethod]
        public void Dispose()
        {
            using (new Sampler())
            {
            }
        }

        [TestMethod]
        [Ignore]
        public void Store()
        {
            using (var sampler = new Sampler())
            {
                Thread.Sleep(1250);

                sampler.StoreSamples(new object());
            }

            Thread.Sleep(1250);

            var table = new AzureTable<Abc.Services.Data.ServerStatisticsRow>(ServerConfiguration.Default);
            var rows = from data in table.QueryByPartition(ServerConfiguration.ApplicationIdentifier.ToString())
                       where data.MachineName == Environment.MachineName
                       select data;
            var list = from data in rows.ToList()
                       where data.OccurredOn.Date == DateTime.UtcNow.Date
                       && data.DeploymentId == Abc.Azure.AzureEnvironment.DeploymentId
                       select data;
            Assert.IsNotNull(list);
            Assert.IsTrue(list.Count() > 0);
        }

        [TestMethod]
        [Ignore]
        public void StoreWithNullState()
        {
            using (var sampler = new Sampler())
            {
                Thread.Sleep(1250);

                sampler.StoreSamples(null);
            }

            Thread.Sleep(1250);

            var table = new AzureTable<Abc.Services.Data.ServerStatisticsRow>(ServerConfiguration.Default);
            var rows = from data in table.QueryByPartition(ServerConfiguration.ApplicationIdentifier.ToString())
                       where data.MachineName == Environment.MachineName
                       select data;
            var list = from data in rows.ToList()
                       where data.OccurredOn.Date == DateTime.UtcNow.Date
                       && data.DeploymentId == Abc.Azure.AzureEnvironment.DeploymentId
                       select data;
            Assert.IsNotNull(list);
            Assert.IsTrue(list.Count() > 0);
        }
        #endregion
    }
}