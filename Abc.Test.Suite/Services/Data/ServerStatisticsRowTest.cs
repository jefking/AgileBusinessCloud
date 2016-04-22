// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='ServerStatisticsRowTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Data
{
    using System;
    using Abc.Services;
    using Abc.Services.Contracts;
    using Abc.Services.Data;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ServerStatisticsRowTest
    {
        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new ServerStatisticsRow();
        }

        [TestMethod]
        public void IsApplicationData()
        {
            Assert.IsNotNull(new ServerStatisticsRow() as ApplicationData);
        }

        [TestMethod]
        public void IsIConvertServerStatisticSetDisplay()
        {
            Assert.IsNotNull(new ServerStatisticsRow() as IConvert<ServerStatisticSetDisplay>);
        }

        [TestMethod]
        public void ApplicationId()
        {
            var data = Guid.NewGuid();
            var row = new ServerStatisticsRow(data);
            Assert.AreEqual<Guid>(data, row.ApplicationId);
        }

        [TestMethod]
        public void CpuUsagePercentage()
        {
            var random = new Random();
            var data = random.Next();
            var row = new ServerStatisticsRow(Guid.NewGuid());
            row.CpuUsagePercentage = data;
            Assert.AreEqual<double>(data, row.CpuUsagePercentage);
        }

        [TestMethod]
        public void PhysicalDiskUsagePercentage()
        {
            var random = new Random();
            var data = random.Next();
            var row = new ServerStatisticsRow(Guid.NewGuid());
            row.PhysicalDiskUsagePercentage = data;
            Assert.AreEqual<double>(data, row.PhysicalDiskUsagePercentage);
        }

        [TestMethod]
        public void OccurredOn()
        {
            var data = DateTime.UtcNow;
            var row = new ServerStatisticsRow(Guid.NewGuid());
            row.OccurredOn = data;
            Assert.AreEqual<DateTime>(data, row.OccurredOn);
        }

        [TestMethod]
        public void MachineName()
        {
            var data = StringHelper.ValidString();
            var row = new ServerStatisticsRow(Guid.NewGuid());
            row.MachineName = data;
            Assert.AreEqual<string>(data, row.MachineName);
        }

        [TestMethod]
        public void NetworkPercentage1()
        {
            var random = new Random();
            var data = random.NextDouble();
            var row = new ServerStatisticsRow(Guid.NewGuid());
            Assert.IsNull(row.NetworkPercentage1);
            row.NetworkPercentage1 = data;
            Assert.AreEqual<double?>(data, row.NetworkPercentage1);
        }

        [TestMethod]
        public void NetworkPercentage2()
        {
            var random = new Random();
            var data = random.NextDouble();
            var row = new ServerStatisticsRow(Guid.NewGuid());
            Assert.IsNull(row.NetworkPercentage2);
            row.NetworkPercentage2 = data;
            Assert.AreEqual<double?>(data, row.NetworkPercentage2);
        }

        [TestMethod]
        public void NetworkPercentage3()
        {
            var random = new Random();
            var data = random.NextDouble();
            var row = new ServerStatisticsRow(Guid.NewGuid());
            Assert.IsNull(row.NetworkPercentage3);
            row.NetworkPercentage3 = data;
            Assert.AreEqual<double?>(data, row.NetworkPercentage3);
        }

        [TestMethod]
        public void NetworkPercentage4()
        {
            var random = new Random();
            var data = random.NextDouble();
            var row = new ServerStatisticsRow(Guid.NewGuid());
            Assert.IsNull(row.NetworkPercentage4);
            row.NetworkPercentage4 = data;
            Assert.AreEqual<double?>(data, row.NetworkPercentage4);
        }

        [TestMethod]
        public void DeploymentId()
        {
            var data = StringHelper.ValidString();
            var row = new ServerStatisticsRow(Guid.NewGuid());
            row.DeploymentId = data;
            Assert.AreEqual<string>(data, row.DeploymentId);
        }

        [TestMethod]
        public void MemoryUsagePercentage()
        {
            var random = new Random();
            var data = random.Next();
            var row = new ServerStatisticsRow(Guid.NewGuid());
            row.MemoryUsagePercentage = data;
            Assert.AreEqual<double>(data, row.MemoryUsagePercentage);
        }

        [TestMethod]
        public void Convert()
        {
            var random = new Random();
            var row = new ServerStatisticsRow(Guid.NewGuid())
            {
                CpuUsagePercentage = random.Next(100),
                MemoryUsagePercentage = random.Next(100),
                PhysicalDiskUsagePercentage = random.Next(100),
                DeploymentId = StringHelper.ValidString(),
                MachineName = StringHelper.ValidString(),
                OccurredOn = DateTime.UtcNow,
                NetworkPercentage1 = null,
                NetworkPercentage2 = null,
                NetworkPercentage3 = null,
                NetworkPercentage4 = null,
            };

            var set = row.Convert();
            Assert.AreEqual<double>(row.CpuUsagePercentage, set.CpuUsagePercentage);
            Assert.AreEqual<double>(row.MemoryUsagePercentage, set.MemoryUsagePercentage);
            Assert.AreEqual<double>(row.PhysicalDiskUsagePercentage, set.PhysicalDiskUsagePercentage);
            Assert.AreEqual<DateTime>(row.OccurredOn, set.OccurredOn);
            Assert.AreEqual<string>(row.DeploymentId, set.DeploymentId);
            Assert.AreEqual<string>(row.MachineName, set.MachineName);
            Assert.AreEqual<Guid>(row.ApplicationId, set.Token.ApplicationId);
            Assert.AreEqual<Guid>(Guid.Parse(row.RowKey), set.Identifier);
            Assert.IsNull(set.NetworkPercentages);
        }

        [TestMethod]
        public void ConvertNetwork2()
        {
            var random = new Random();
            var row = new ServerStatisticsRow(Guid.NewGuid())
            {
                CpuUsagePercentage = random.Next(100),
                MemoryUsagePercentage = random.Next(100),
                PhysicalDiskUsagePercentage = random.Next(100),
                DeploymentId = StringHelper.ValidString(),
                MachineName = StringHelper.ValidString(),
                OccurredOn = DateTime.UtcNow,
                NetworkPercentage1 = null,
                NetworkPercentage2 = random.Next(1, 100),
                NetworkPercentage3 = null,
                NetworkPercentage4 = null,
            };

            var set = row.Convert();
            Assert.AreEqual<double>(row.CpuUsagePercentage, set.CpuUsagePercentage);
            Assert.AreEqual<double>(row.MemoryUsagePercentage, set.MemoryUsagePercentage);
            Assert.AreEqual<double>(row.PhysicalDiskUsagePercentage, set.PhysicalDiskUsagePercentage);
            Assert.AreEqual<DateTime>(row.OccurredOn, set.OccurredOn);
            Assert.AreEqual<string>(row.DeploymentId, set.DeploymentId);
            Assert.AreEqual<string>(row.MachineName, set.MachineName);
            Assert.AreEqual<Guid>(row.ApplicationId, set.Token.ApplicationId);
            Assert.AreEqual<Guid>(Guid.Parse(row.RowKey), set.Identifier);
            Assert.AreEqual<int>(1, set.NetworkPercentages.Length);
            Assert.AreEqual<double?>(row.NetworkPercentage2, set.NetworkPercentages[0]);
        }

        [TestMethod]
        public void ConvertNetwork()
        {
            var random = new Random();
            var row = new ServerStatisticsRow(Guid.NewGuid())
            {
                CpuUsagePercentage = random.Next(100),
                MemoryUsagePercentage = random.Next(100),
                PhysicalDiskUsagePercentage = random.Next(100),
                DeploymentId = StringHelper.ValidString(),
                MachineName = StringHelper.ValidString(),
                OccurredOn = DateTime.UtcNow,
                NetworkPercentage1 = random.Next(1, 100),
                NetworkPercentage2 = random.Next(1, 100),
                NetworkPercentage3 = random.Next(1, 100),
                NetworkPercentage4 = random.Next(1, 100),
            };

            var set = row.Convert();
            Assert.AreEqual<double>(row.CpuUsagePercentage, set.CpuUsagePercentage);
            Assert.AreEqual<double>(row.MemoryUsagePercentage, set.MemoryUsagePercentage);
            Assert.AreEqual<double>(row.PhysicalDiskUsagePercentage, set.PhysicalDiskUsagePercentage);
            Assert.AreEqual<DateTime>(row.OccurredOn, set.OccurredOn);
            Assert.AreEqual<string>(row.DeploymentId, set.DeploymentId);
            Assert.AreEqual<string>(row.MachineName, set.MachineName);
            Assert.AreEqual<Guid>(row.ApplicationId, set.Token.ApplicationId);
            Assert.AreEqual<Guid>(Guid.Parse(row.RowKey), set.Identifier);
            Assert.AreEqual<int>(4, set.NetworkPercentages.Length);
            Assert.AreEqual<double?>(row.NetworkPercentage1, set.NetworkPercentages[0]);
            Assert.AreEqual<double?>(row.NetworkPercentage2, set.NetworkPercentages[1]);
            Assert.AreEqual<double?>(row.NetworkPercentage3, set.NetworkPercentages[2]);
            Assert.AreEqual<double?>(row.NetworkPercentage4, set.NetworkPercentages[3]);
        }
        #endregion
    }
}