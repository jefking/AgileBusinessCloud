// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='ServerStatisticSetTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Contracts
{
    using System;
    using System.Collections.Generic;
    using Abc.Services.Contracts;
    using Abc.Services.Validation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Abc.Services.Data;

    [TestClass]
    public class ServerStatisticSetTest
    {
        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new ServerStatisticSet();
        }

        [TestMethod]
        public void IsSecured()
        {
            Assert.IsNotNull(new ServerStatisticSet() as Secured);
        }

        [TestMethod]
        public void IsIConvertServerStatisticsRow()
        {
            Assert.IsNotNull(new ServerStatisticSet() as IConvert<ServerStatisticsRow>);
        }

        [TestMethod]
        public void IsIConvertLatestServerStatisticsRow()
        {
            Assert.IsNotNull(new ServerStatisticSet() as IConvert<LatestServerStatisticsRow>);
        }

        [TestMethod]
        public void IsIValidateServerStatisticSet()
        {
            Assert.IsNotNull(new ServerStatisticSet() as IValidate<ServerStatisticSet>);
        }

        [TestMethod]
        public void OccurredOn()
        {
            var data = DateTime.UtcNow;
            var set = new ServerStatisticSet();
            set.OccurredOn = data;
            Assert.AreEqual<DateTime>(data, set.OccurredOn);
        }

        [TestMethod]
        public void MachineName()
        {
            var data = StringHelper.ValidString();
            var set = new ServerStatisticSet();
            set.MachineName = data;
            Assert.AreEqual<string>(data, set.MachineName);
        }

        [TestMethod]
        public void DeploymentId()
        {
            var data = StringHelper.ValidString();
            var set = new ServerStatisticSet();
            set.DeploymentId = data;
            Assert.AreEqual<string>(data, set.DeploymentId);
        }

        [TestMethod]
        public void CpuUsagePercentage()
        {
            var random = new Random();
            var data = random.Next();
            var set = new ServerStatisticSet();
            set.CpuUsagePercentage = data;
            Assert.AreEqual<float>(data, set.CpuUsagePercentage);
        }

        [TestMethod]
        public void PhysicalDiskUsagePercentage()
        {
            var random = new Random();
            var data = random.Next();
            var set = new ServerStatisticSet();
            set.PhysicalDiskUsagePercentage = data;
            Assert.AreEqual<float>(data, set.PhysicalDiskUsagePercentage);
        }

        [TestMethod]
        public void NetworkPercentages()
        {
            var random = new Random();
            var list = new List<float>();
            for (int i = 0; i < random.Next(1, 100); i++)
            {
                list.Add((float)random.Next());
            }
            var data = list.ToArray();
            var set = new ServerStatisticSet();
            Assert.IsNull(set.NetworkPercentages);
            set.NetworkPercentages = data;
            Assert.AreEqual<int>(data.Length, set.NetworkPercentages.Length);
        }

        [TestMethod]
        public void MemoryUsagePercentage()
        {
            var random = new Random();
            var data = random.Next();
            var set = new ServerStatisticSet();
            set.MemoryUsagePercentage = data;
            Assert.AreEqual<float>(data, set.MemoryUsagePercentage);
        }

        [TestMethod]
        public void InvalidDiskPercentageLow()
        {
            var random = new Random();
            var data = random.Next() * -1;
            var set = new ServerStatisticSet();
            set.PhysicalDiskUsagePercentage = data;
            var validator = new Validator<ServerStatisticSet>();
            Assert.IsFalse(validator.IsValid(set));
        }

        [TestMethod]
        public void InvalidDiskPercentageHigh()
        {
            var random = new Random();
            var data = random.Next(101, 1000);
            var set = new ServerStatisticSet();
            set.PhysicalDiskUsagePercentage = data;
            var validator = new Validator<ServerStatisticSet>();
            Assert.IsFalse(validator.IsValid(set));
        }

        [TestMethod]
        public void InvalidCpuUsagePercentage()
        {
            var random = new Random();
            var data = random.Next() * -1;
            var set = new ServerStatisticSet();
            set.CpuUsagePercentage = data;
            var validator = new Validator<ServerStatisticSet>();
            Assert.IsFalse(validator.IsValid(set));
        }

        [TestMethod]
        public void InvalidMemoryUsagePercentage()
        {
            var random = new Random();
            var data = random.Next() + 101;
            var set = new ServerStatisticSet();
            set.MemoryUsagePercentage = data;
            var validator = new Validator<ServerStatisticSet>();
            Assert.IsFalse(validator.IsValid(set));
        }

        [TestMethod]
        public void InvalidWithNetwork()
        {
            var random = new Random();
            var data = new List<float>();
            for (int i = 0; i < random.Next(1, 5); i++)
            {
                data.Add(random.Next(101, 500));
            }
            var set = new ServerStatisticSet()
            {
                CpuUsagePercentage = random.Next(100),
                MemoryUsagePercentage = random.Next(100),
                PhysicalDiskUsagePercentage = random.Next(100),
                DeploymentId = StringHelper.ValidString(),
                MachineName = StringHelper.ValidString(),
                OccurredOn = DateTime.UtcNow,
                NetworkPercentages = data.ToArray(),
            };

            var validator = new Validator<ServerStatisticSet>();
            Assert.IsFalse(validator.IsValid(set));
        }

        [TestMethod]
        public void Valid()
        {
            var random = new Random();
            var set = new ServerStatisticSet()
            {
                CpuUsagePercentage = random.Next(100),
                MemoryUsagePercentage = random.Next(100),
                PhysicalDiskUsagePercentage = random.Next(100),
                DeploymentId = StringHelper.ValidString(),
                MachineName = StringHelper.ValidString(),
                OccurredOn = DateTime.UtcNow,
                NetworkPercentages = null,
            };

            var validator = new Validator<ServerStatisticSet>();
            Assert.IsTrue(validator.IsValid(set));
        }

        [TestMethod]
        public void ValidBoundaryHigh()
        {
            var random = new Random();
            var set = new ServerStatisticSet()
            {
                CpuUsagePercentage = 100,
                MemoryUsagePercentage = 100,
                PhysicalDiskUsagePercentage = 100,
                DeploymentId = StringHelper.ValidString(),
                MachineName = StringHelper.ValidString(),
                OccurredOn = DateTime.UtcNow,
                NetworkPercentages = null,
            };

            var validator = new Validator<ServerStatisticSet>();
            Assert.IsTrue(validator.IsValid(set));
        }

        [TestMethod]
        public void ValidBoundaryLow()
        {
            var random = new Random();
            var set = new ServerStatisticSet()
            {
                CpuUsagePercentage = 0,
                MemoryUsagePercentage = 0,
                PhysicalDiskUsagePercentage = 0,
                DeploymentId = StringHelper.ValidString(),
                MachineName = StringHelper.ValidString(),
                OccurredOn = DateTime.UtcNow,
                NetworkPercentages = null,
            };

            var validator = new Validator<ServerStatisticSet>();
            Assert.IsTrue(validator.IsValid(set));
        }

        [TestMethod]
        public void ValidWithNetwork()
        {
            var random = new Random();
            var data = new List<float>();
            for (int i = 0; i < random.Next(1, 100); i++)
            {
                data.Add(random.Next(1, 100));
            }
            var set = new ServerStatisticSet()
            {
                CpuUsagePercentage = random.Next(100),
                MemoryUsagePercentage = random.Next(100),
                PhysicalDiskUsagePercentage = random.Next(100),
                DeploymentId = StringHelper.ValidString(),
                MachineName = StringHelper.ValidString(),
                OccurredOn = DateTime.UtcNow,
                NetworkPercentages = data.ToArray(),
            };

            var validator = new Validator<ServerStatisticSet>();
            Assert.IsTrue(validator.IsValid(set));
        }

        [TestMethod]
        public void Convert()
        {
            var random = new Random();
            var set = new ServerStatisticSet()
            {
                CpuUsagePercentage = random.Next(100),
                MemoryUsagePercentage = random.Next(100),
                PhysicalDiskUsagePercentage = random.Next(100),
                DeploymentId = StringHelper.ValidString(),
                MachineName = StringHelper.ValidString(),
                OccurredOn = DateTime.UtcNow,
                Token = new Token()
                {
                    ApplicationId = Guid.NewGuid()
                },
                NetworkPercentages = null,
            };

            var row = set.Convert();
            Assert.AreEqual<double>(set.CpuUsagePercentage, row.CpuUsagePercentage);
            Assert.AreEqual<double>(set.MemoryUsagePercentage, row.MemoryUsagePercentage);
            Assert.AreEqual<double>(set.PhysicalDiskUsagePercentage, row.PhysicalDiskUsagePercentage);
            Assert.AreEqual<DateTime>(set.OccurredOn, row.OccurredOn);
            Assert.AreEqual<string>(set.DeploymentId, row.DeploymentId);
            Assert.AreEqual<string>(set.MachineName, row.MachineName);
            Assert.AreEqual<Guid>(set.Token.ApplicationId, row.ApplicationId);
            Assert.IsNull(row.NetworkPercentage1);
            Assert.IsNull(row.NetworkPercentage2);
            Assert.IsNull(row.NetworkPercentage3);
            Assert.IsNull(row.NetworkPercentage4);
        }

        [TestMethod]
        public void ConvertLatest()
        {
            var random = new Random();
            var set = new ServerStatisticSet()
            {
                CpuUsagePercentage = random.Next(100),
                MemoryUsagePercentage = random.Next(100),
                PhysicalDiskUsagePercentage = random.Next(100),
                DeploymentId = StringHelper.ValidString(),
                MachineName = StringHelper.ValidString(),
                OccurredOn = DateTime.UtcNow,
                Token = new Token()
                {
                    ApplicationId = Guid.NewGuid()
                },
                NetworkPercentages = null,
            };

            var row = ((IConvert<LatestServerStatisticsRow>)set).Convert();
            Assert.AreEqual<double>(set.CpuUsagePercentage, row.CpuUsagePercentage);
            Assert.AreEqual<double>(set.MemoryUsagePercentage, row.MemoryUsagePercentage);
            Assert.AreEqual<double>(set.PhysicalDiskUsagePercentage, row.PhysicalDiskUsagePercentage);
            Assert.AreEqual<DateTime>(set.OccurredOn, row.OccurredOn);
            Assert.AreEqual<string>(set.DeploymentId, row.DeploymentId);
            Assert.AreEqual<string>(set.MachineName, row.RowKey);
            Assert.AreEqual<Guid>(set.Token.ApplicationId, row.ApplicationId);
            Assert.IsNull(row.NetworkPercentage1);
            Assert.IsNull(row.NetworkPercentage2);
            Assert.IsNull(row.NetworkPercentage3);
            Assert.IsNull(row.NetworkPercentage4);
        }

        [TestMethod]
        public void ConvertNetworkPercentage4()
        {
            var random = new Random();
            var network = new float[] { random.Next(), random.Next(), random.Next(), random.Next() };
            var set = new ServerStatisticSet()
            {
                CpuUsagePercentage = random.Next(100),
                MemoryUsagePercentage = random.Next(100),
                PhysicalDiskUsagePercentage = random.Next(100),
                DeploymentId = StringHelper.ValidString(),
                MachineName = StringHelper.ValidString(),
                OccurredOn = DateTime.UtcNow,
                Token = new Token()
                {
                    ApplicationId = Guid.NewGuid()
                },
                NetworkPercentages = network,
            };

            var row = set.Convert();
            Assert.AreEqual<double>(set.CpuUsagePercentage, row.CpuUsagePercentage);
            Assert.AreEqual<double>(set.MemoryUsagePercentage, row.MemoryUsagePercentage);
            Assert.AreEqual<double>(set.PhysicalDiskUsagePercentage, row.PhysicalDiskUsagePercentage);
            Assert.AreEqual<DateTime>(set.OccurredOn, row.OccurredOn);
            Assert.AreEqual<string>(set.DeploymentId, row.DeploymentId);
            Assert.AreEqual<string>(set.MachineName, row.MachineName);
            Assert.AreEqual<Guid>(set.Token.ApplicationId, row.ApplicationId);

            Assert.AreEqual<double?>(network[0], row.NetworkPercentage1);
            Assert.AreEqual<double?>(network[1], row.NetworkPercentage2);
            Assert.AreEqual<double?>(network[2], row.NetworkPercentage3);
            Assert.AreEqual<double?>(network[3], row.NetworkPercentage4);
        }

        [TestMethod]
        public void ConvertNetworkPercentage4Latest()
        {
            var random = new Random();
            var network = new float[] { random.Next(), random.Next(), random.Next(), random.Next() };
            var set = new ServerStatisticSet()
            {
                CpuUsagePercentage = random.Next(100),
                MemoryUsagePercentage = random.Next(100),
                PhysicalDiskUsagePercentage = random.Next(100),
                DeploymentId = StringHelper.ValidString(),
                MachineName = StringHelper.ValidString(),
                OccurredOn = DateTime.UtcNow,
                Token = new Token()
                {
                    ApplicationId = Guid.NewGuid()
                },
                NetworkPercentages = network,
            };

            var row = ((IConvert<LatestServerStatisticsRow>)set).Convert();
            Assert.AreEqual<double>(set.CpuUsagePercentage, row.CpuUsagePercentage);
            Assert.AreEqual<double>(set.MemoryUsagePercentage, row.MemoryUsagePercentage);
            Assert.AreEqual<double>(set.PhysicalDiskUsagePercentage, row.PhysicalDiskUsagePercentage);
            Assert.AreEqual<DateTime>(set.OccurredOn, row.OccurredOn);
            Assert.AreEqual<string>(set.DeploymentId, row.DeploymentId);
            Assert.AreEqual<string>(set.MachineName, row.RowKey);
            Assert.AreEqual<Guid>(set.Token.ApplicationId, row.ApplicationId);

            Assert.AreEqual<double?>(network[0], row.NetworkPercentage1);
            Assert.AreEqual<double?>(network[1], row.NetworkPercentage2);
            Assert.AreEqual<double?>(network[2], row.NetworkPercentage3);
            Assert.AreEqual<double?>(network[3], row.NetworkPercentage4);
        }

        [TestMethod]
        public void ConvertNetworkPercentage2()
        {
            var random = new Random();
            var network = new float[] { random.Next(), random.Next() };
            var set = new ServerStatisticSet()
            {
                CpuUsagePercentage = random.Next(100),
                MemoryUsagePercentage = random.Next(100),
                PhysicalDiskUsagePercentage = random.Next(100),
                DeploymentId = StringHelper.ValidString(),
                MachineName = StringHelper.ValidString(),
                OccurredOn = DateTime.UtcNow,
                Token = new Token()
                {
                    ApplicationId = Guid.NewGuid()
                },
                NetworkPercentages = network,
            };

            var row = set.Convert();
            Assert.AreEqual<double>(set.CpuUsagePercentage, row.CpuUsagePercentage);
            Assert.AreEqual<double>(set.MemoryUsagePercentage, row.MemoryUsagePercentage);
            Assert.AreEqual<double>(set.PhysicalDiskUsagePercentage, row.PhysicalDiskUsagePercentage);
            Assert.AreEqual<DateTime>(set.OccurredOn, row.OccurredOn);
            Assert.AreEqual<string>(set.DeploymentId, row.DeploymentId);
            Assert.AreEqual<string>(set.MachineName, row.MachineName);
            Assert.AreEqual<Guid>(set.Token.ApplicationId, row.ApplicationId);

            Assert.AreEqual<double?>(network[0], row.NetworkPercentage1);
            Assert.AreEqual<double?>(network[1], row.NetworkPercentage2);
            Assert.IsNull(row.NetworkPercentage3);
            Assert.IsNull(row.NetworkPercentage4);
        }

        [TestMethod]
        public void ConvertNetworkPercentage2Latest()
        {
            var random = new Random();
            var network = new float[] { random.Next(), random.Next() };
            var set = new ServerStatisticSet()
            {
                CpuUsagePercentage = random.Next(100),
                MemoryUsagePercentage = random.Next(100),
                PhysicalDiskUsagePercentage = random.Next(100),
                DeploymentId = StringHelper.ValidString(),
                MachineName = StringHelper.ValidString(),
                OccurredOn = DateTime.UtcNow,
                Token = new Token()
                {
                    ApplicationId = Guid.NewGuid()
                },
                NetworkPercentages = network,
            };

            var row = ((IConvert<LatestServerStatisticsRow>)set).Convert();
            Assert.AreEqual<double>(set.CpuUsagePercentage, row.CpuUsagePercentage);
            Assert.AreEqual<double>(set.MemoryUsagePercentage, row.MemoryUsagePercentage);
            Assert.AreEqual<double>(set.PhysicalDiskUsagePercentage, row.PhysicalDiskUsagePercentage);
            Assert.AreEqual<DateTime>(set.OccurredOn, row.OccurredOn);
            Assert.AreEqual<string>(set.DeploymentId, row.DeploymentId);
            Assert.AreEqual<string>(set.MachineName, row.RowKey);
            Assert.AreEqual<Guid>(set.Token.ApplicationId, row.ApplicationId);

            Assert.AreEqual<double?>(network[0], row.NetworkPercentage1);
            Assert.AreEqual<double?>(network[1], row.NetworkPercentage2);
            Assert.IsNull(row.NetworkPercentage3);
            Assert.IsNull(row.NetworkPercentage4);
        }
        #endregion
    }
}