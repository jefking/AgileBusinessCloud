// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='DatumWebServiceTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite
{
    using System;
    using System.Linq;
    using System.ServiceModel;
    using System.Threading;
    using Abc.Azure;
    using Abc.Configuration;
    using Abc.Services;
    using Abc.Test.Services.Client.Datum.Client;
    using Abc.Underpinning;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure;
    using Server = Abc.Services;

    /// <summary>
    /// Datum Web Service Test
    /// </summary>
    [TestClass]
    public class DatumWebServiceTest
    {
        #region Members
        /// <summary>
        /// Application
        /// </summary>
        private readonly Application application = new Application();
        #endregion

        #region Properties
        private EventLogItem EventLog
        {
            get
            {
                var random = new Random();

                var token = new Token()
                {
                    ApplicationId = application.Token.ApplicationId,
                    ValidationKey = application.Token.ValidationKey
                };

                return new EventLogItem()
                {
                    DeploymentId = StringHelper.ValidString(),
                    EntryType = EventLogEntryType.Error,
                    EventId = random.Next(),
                    InstanceId = random.Next(),
                    MachineName = StringHelper.ValidString(),
                    Message = StringHelper.ValidString(),
                    OccurredOn = DateTime.UtcNow,
                    Source = StringHelper.ValidString(),
                    User = StringHelper.ValidString(),
                    Token = token,
                };
            }
        }
        #endregion

        #region Initialize
        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            var items = new AzureTable<ApplicationConfiguration>(CloudStorageAccount.DevelopmentStorageAccount);
            items.DeleteByPartition(ConfigurationSettings.ApplicationIdentifier.ToString());
        }
        #endregion

        #region Error Cases
        #region Exception
        [TestMethod]
        [ExpectedException(typeof(FaultException))]
        public void NullException()
        {
            using (var client = new DatumClient())
            {
                client.LogException(null);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(FaultException))]
        public void NullExceptionToken()
        {
            using (var client = new DatumClient())
            {
                var err = this.Error();
                err.Token = null;
                client.LogException(err);
            }
        }

        [TestMethod]
        public void ExceptionEmptyAppId()
        {
            using (var client = new DatumClient())
            {
                var err = this.Error();
                err.Token.ApplicationId = Guid.Empty;
                client.LogException(err);
            }
        }

        [TestMethod]
        public void ExceptionInvalidMessage()
        {
            using (var client = new DatumClient())
            {
                var err = this.Error();
                err.Message = StringHelper.NullEmptyWhiteSpace();
                client.LogException(err);
            }
        }

        [TestMethod]
        public void ExceptionInvalidType()
        {
            using (var client = new DatumClient())
            {
                var err = this.Error();
                err.ClassName = StringHelper.NullEmptyWhiteSpace();
                client.LogException(err);
            }
        }
        #endregion

        #region Message
        [TestMethod]
        [ExpectedException(typeof(FaultException))]
        public void NullMessage()
        {
            using (var client = new DatumClient())
            {
                client.LogMessage(null);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(FaultException))]
        public void NullMessageToken()
        {
            using (var client = new DatumClient())
            {
                var msg = this.Message();
                msg.Token = null;
                client.LogMessage(msg);
            }
        }

        [TestMethod]
        public void MessageEmptyAppId()
        {
            using (var client = new DatumClient())
            {
                Message msg = this.Message();
                msg.Token.ApplicationId = Guid.Empty;
                client.LogMessage(msg);
            }
        }

        [TestMethod]
        public void MessageInvalidMessage()
        {
            using (var client = new DatumClient())
            {
                var msg = this.Message();
                msg.Message = StringHelper.NullEmptyWhiteSpace();
                client.LogMessage(msg);
            }
        }
        #endregion

        #region Performance
        [TestMethod]
        [ExpectedException(typeof(FaultException))]
        public void NullPerformance()
        {
            using (var client = new DatumClient())
            {
                client.LogPerformance(null);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(FaultException))]
        public void NullPerformanceToken()
        {
            using (var client = new DatumClient())
            {
                var perf = this.Occurrence();
                perf.Token = null;
                client.LogPerformance(perf);
            }
        }

        [TestMethod]
        public void PerformanceEmptyAppId()
        {
            using (var client = new DatumClient())
            {
                var perf = this.Occurrence();
                perf.Token.ApplicationId = Guid.Empty;
                client.LogPerformance(perf);
            }
        }

        [TestMethod]
        public void PerformanceZeroThreadId()
        {
            using (var client = new DatumClient())
            {
                var perf = this.Occurrence();
                perf.ThreadId = 0;
                client.LogPerformance(perf);
            }
        }

        [TestMethod]
        public void PerformanceZeroExecutionTime()
        {
            using (var client = new DatumClient())
            {
                var perf = this.Occurrence();
                perf.Duration = TimeSpan.Zero;
                client.LogPerformance(perf);
            }
        }

        [TestMethod]
        public void PerformanceInvalidClass()
        {
            using (var client = new DatumClient())
            {
                var perf = this.Occurrence();
                perf.Class = StringHelper.NullEmptyWhiteSpace();
                client.LogPerformance(perf);
            }
        }

        [TestMethod]
        public void PerformanceInvalidMethod()
        {
            using (var client = new DatumClient())
            {
                var perf = this.Occurrence();
                perf.Method = StringHelper.NullEmptyWhiteSpace();
                client.LogPerformance(perf);
            }
        }
        #endregion

        #region Configuration
        [TestMethod]
        [ExpectedException(typeof(FaultException))]
        public void GetConfigurationNull()
        {
            using (var client = new DatumClient())
            {
                client.GetConfiguration(null);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(FaultException))]
        public void GetConfigurationNullToken()
        {
            using (var client = new DatumClient())
            {
                var config = this.Config();
                config.Token = null;
                client.GetConfiguration(config);
            }
        }

        [TestMethod]
        public void GetConfigurationEmptyApplicationId()
        {
            using (var client = new DatumClient())
            {
                var config = this.Config();
                config.Token.ApplicationId = Guid.Empty;
                var returnedData = client.GetConfiguration(config);
                Assert.IsNull(returnedData);
            }
        }

        [TestMethod]
        public void GetConfigurationInvalidValidationKey()
        {
            using (var client = new DatumClient())
            {
                var config = this.Config();
                config.Token.ValidationKey = StringHelper.NullEmptyWhiteSpace();
                var returnedData = client.GetConfiguration(config);
                Assert.IsNull(returnedData);
            }
        }
        #endregion

        #region Server Statistics Set
        [TestMethod]
        [ExpectedException(typeof(FaultException))]
        public void LogServerStatisticSetNull()
        {
            using (var client = new DatumClient())
            {
                client.LogServerStatisticSet(null);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(FaultException))]
        public void LogServerStatisticSetNullToken()
        {
            using (var client = new DatumClient())
            {
                var stats = this.ServerStats();
                stats.Token = null;
                client.LogServerStatisticSet(stats);
            }
        }

        [TestMethod]
        public void LogServerStatisticSetEmptyApplicationId()
        {
            using (var client = new DatumClient())
            {
                var stats = this.ServerStats();
                stats.Token.ApplicationId = Guid.Empty;
                client.LogServerStatisticSet(stats);
            }
        }

        [TestMethod]
        public void LogServerStatisticSetInvalidValidationKey()
        {
            using (var client = new DatumClient())
            {
                var stats = this.ServerStats();
                stats.Token.ValidationKey = StringHelper.NullEmptyWhiteSpace();
                client.LogServerStatisticSet(stats);
            }
        }
        #endregion
        #endregion

        #region Valid Cases
        [TestMethod]
        [Ignore]
        public void LogError()
        {
            var err = this.Error();
            using (var client = new DatumClient())
            {
                client.LogException(err);
            }

            var app = new Application();
            var table = new AzureTable<Server.ErrorData>(CloudStorageAccount.DevelopmentStorageAccount);
            var errors = table.QueryByPartition(err.Token.ApplicationId.ToString()).ToList();

            Assert.IsNotNull(errors, "Errors should not be null");
            var error = (from data in errors
                         where err.Message == data.Message
                         select data).FirstOrDefault();
            Assert.IsNotNull(error, "Error should not be null");
            Assert.AreEqual<Guid>(err.Token.ApplicationId, error.ApplicationId, "Application Id should match");
            Assert.AreEqual<DateTime>(err.OccurredOn.Date, error.OccurredOn.Date, "Occured On should match");
            Assert.AreEqual<string>(err.MachineName, error.MachineName, "Machine Name should match");
            Assert.AreEqual<string>(err.Message, error.Message, "Message should match");
            Assert.AreEqual<string>(err.ClassName, error.ClassName, "Type should match");
            Assert.AreEqual<int>(err.ErrorCode, error.ErrorCode, "Error Code should match");
            Assert.AreEqual<int>((int)err.EventType, error.EventTypeValue, "Event Type should match");
        }

        [TestMethod]
        public void LogEventItem()
        {
            var item = this.EventLog;
            using (var client = new DatumClient())
            {
                client.LogEventItem(item);
            }
            
            var table = new AzureTable<Server.Data.EventLogRow>(CloudStorageAccount.DevelopmentStorageAccount);

            var returned = (from data in table.QueryByPartition(item.Token.ApplicationId.ToString())
                         where item.Message == data.Message
                         select data).FirstOrDefault();
            Assert.IsNotNull(returned, "Event Log Item should not be null");
            Assert.AreEqual<string>(item.Source, returned.Source);
            Assert.AreEqual<string>(item.MachineName, returned.MachineName);
            Assert.AreEqual<string>(item.Message, returned.Message);
            Assert.AreEqual<DateTime>(item.OccurredOn.Date, returned.OccurredOn.Date);
            Assert.AreEqual<Guid>(item.Token.ApplicationId, returned.ApplicationId);
            Assert.AreEqual<string>(item.DeploymentId, returned.DeploymentId);
            Assert.AreEqual<string>(item.User, returned.User);
            Assert.AreEqual<int?>(item.EventId, returned.EventId);
            Assert.AreEqual<long?>(item.InstanceId, returned.InstanceId);
            Assert.AreEqual<int>((int)item.EntryType, returned.EntryTypeValue);
        }

        [TestMethod]
        [Ignore]
        public void LogErrorWithParents()
        {
            var err = this.Error();
            var parentA = this.Error();
            parentA.Token.ApplicationId = err.Token.ApplicationId;
            var parentB = this.Error();
            parentB.Token.ApplicationId = err.Token.ApplicationId;
            err.Parent = parentA;
            parentA.Parent = parentB;
            using (var client = new DatumClient())
            {
                client.LogException(err);
            }

            var source = new Server.Core.LogCore();
            var query = new Server.Contracts.LogQuery()
            {
                ApplicationIdentifier = err.Token.ApplicationId,
            };
            var errors = source.SelectErrors(query);
            Assert.IsNotNull(errors, "Errors should not be null");
            ErrorItem data;
            foreach (var error in errors)
            {
                if (error.Message == err.Message)
                {
                    data = err;
                }
                else if (error.Message == parentB.Message)
                {
                    data = parentB;
                }
                else if (error.Message == parentA.Message)
                {
                    data = parentA;
                }
                else
                {
                    continue;
                }

                Assert.IsNotNull(error, "Error should not be null");
                Assert.AreEqual<Guid>(data.Token.ApplicationId, error.Token.ApplicationId, "Application Id should match");
                Assert.AreEqual<string>(data.MachineName, error.MachineName, "Machine Name should match");
                Assert.AreEqual<string>(data.Message, error.Message, "Message should match");
                Assert.AreEqual<string>(data.ClassName, error.ClassName, "Type should match");
                Assert.AreEqual<int>(data.ErrorCode, error.ErrorCode, "Error Code should match");
                Assert.AreEqual<DateTime>(data.OccurredOn.Date, error.OccurredOn.Date, "Occured On should match");
            }
        }

        [TestMethod]
        public void LogMessage()
        {
            var msg = this.Message();
            var query = new Server.Contracts.LogQuery()
            {
                ApplicationIdentifier = msg.Token.ApplicationId,
            };

            using (var client = new DatumClient())
            {
                client.LogMessage(msg);
            }

            var source = new Server.Core.LogCore();
            int i = 0;
            Abc.Services.Contracts.MessageDisplay message = null;
            while (message == null && i < 50)
            {
                Thread.Sleep(10);
                message = (from data in source.SelectMessages(query)
                           where msg.Message == data.Message
                           select data).FirstOrDefault();
                i++;
            }
            Assert.IsNotNull(message, "Message should not be null");
            Assert.AreEqual<Guid>(msg.Token.ApplicationId, message.Token.ApplicationId, "Application Id should match");
            Assert.AreEqual<DateTime>(msg.OccurredOn.Date, message.OccurredOn.Date, "Occured On should match");
            Assert.AreEqual<string>(msg.MachineName, message.MachineName, "Machine Name should match");
            Assert.AreEqual<string>(msg.Message, message.Message, "Message should match");
        }

        [TestMethod]
        public void LogServerStatisticSet()
        {
            var stat = this.ServerStats();
            using (var client = new DatumClient())
            {
                client.LogServerStatisticSet(stat);
            }

            var table = new AzureTable<Abc.Services.Data.ServerStatisticsRow>(ServerConfiguration.Default);
            var row = (from data in table.QueryByPartition(stat.Token.ApplicationId.ToString())
                       where data.MachineName == stat.MachineName
                       select data).FirstOrDefault();

            Assert.IsNotNull(row);
            Assert.AreEqual<Guid>(stat.Token.ApplicationId, row.ApplicationId, "Application Id should match");
            Assert.AreEqual<DateTime>(stat.OccurredOn.Date, row.OccurredOn.Date, "Occured On should match");
            Assert.AreEqual<string>(stat.MachineName, row.MachineName, "Machine Name should match");
            Assert.AreEqual<double>(stat.CpuUsagePercentage, row.CpuUsagePercentage);
            Assert.AreEqual<double>(stat.PhysicalDiskUsagePercentage, row.PhysicalDiskUsagePercentage);
            Assert.AreEqual<double>(stat.MemoryUsagePercentage, row.MemoryUsagePercentage);
            Assert.AreEqual<double?>(stat.NetworkPercentages[0], row.NetworkPercentage1);
            Assert.AreEqual<double?>(stat.NetworkPercentages[1], row.NetworkPercentage2);
            Assert.AreEqual<double?>(stat.NetworkPercentages[2], row.NetworkPercentage3);
            Assert.AreEqual<double?>(stat.NetworkPercentages[3], row.NetworkPercentage4);
        }

        [TestMethod]
        public void LogServerStatisticSetNullNetwork()
        {
            var stat = this.ServerStats();
            stat.NetworkPercentages = null;
            using (var client = new DatumClient())
            {
                client.LogServerStatisticSet(stat);
            }

            var table = new AzureTable<Abc.Services.Data.ServerStatisticsRow>(ServerConfiguration.Default);
            var row = (from data in table.QueryByPartition(stat.Token.ApplicationId.ToString())
                       where data.MachineName == stat.MachineName
                       select data).First();

            Assert.IsNotNull(row);
            Assert.AreEqual<Guid>(stat.Token.ApplicationId, row.ApplicationId, "Application Id should match");
            Assert.AreEqual<DateTime>(stat.OccurredOn.Date, row.OccurredOn.Date, "Occured On should match");
            Assert.AreEqual<string>(stat.MachineName, row.MachineName, "Machine Name should match");
            Assert.AreEqual<double>(stat.CpuUsagePercentage, row.CpuUsagePercentage);
            Assert.AreEqual<double>(stat.PhysicalDiskUsagePercentage, row.PhysicalDiskUsagePercentage);
            Assert.AreEqual<double>(stat.MemoryUsagePercentage, row.MemoryUsagePercentage);
            Assert.IsNull(stat.NetworkPercentages);
            Assert.IsNull(row.NetworkPercentage1);
            Assert.IsNull(row.NetworkPercentage2);
            Assert.IsNull(row.NetworkPercentage3);
            Assert.IsNull(row.NetworkPercentage4);
        }

        [TestMethod]
        public void LogOccurrence()
        {
            var perf = this.Occurrence();
            using (var client = new DatumClient())
            {
                client.LogPerformance(perf);
            }

            var source = new Server.Core.LogCore();
            var query = new Server.Contracts.LogQuery()
            {
                ApplicationIdentifier = Settings.ApplicationIdentifier,
            };

            var occurance = (from data in source.SelectOccurrences(query)
                             where perf.Message == data.Message
                             select data).FirstOrDefault();

            Assert.IsNotNull(occurance, "Occurrence should not be null");
            Assert.AreEqual<Guid>(perf.Token.ApplicationId, occurance.Token.ApplicationId, "Application Id should match");
            Assert.AreEqual<DateTime>(perf.OccurredOn.Date, occurance.OccurredOn.Date, "Occurred On should match");
            Assert.AreEqual<string>(perf.MachineName, occurance.MachineName, "Machine Name should match");
            Assert.AreEqual<string>(perf.Message, occurance.Message, "Message should match");
            Assert.AreEqual<string>(perf.Class, occurance.Class, "Type should match");
            Assert.AreEqual<TimeSpan>(perf.Duration, occurance.Duration, "Duration should match");
            Assert.AreEqual<string>(perf.Method, occurance.Method, "Method should match");
            Assert.AreEqual<int>(perf.ThreadId, occurance.ThreadId, "Thread Id should match");
            Assert.AreEqual<Guid?>(perf.SessionIdentifier, occurance.SessionIdentifier, "Session Identifier should match");
        }

        [TestMethod]
        public void LogOccurrenceSessionNull()
        {
            var perf = this.Occurrence();
            perf.SessionIdentifier = null;
            using (var client = new DatumClient())
            {
                client.LogPerformance(perf);
            }

            var source = new Server.Core.LogCore();
            var query = new Server.Contracts.LogQuery()
            {
                ApplicationIdentifier = Settings.ApplicationIdentifier,
            };

            var occurance = (from data in source.SelectOccurrences(query)
                             where perf.Message == data.Message
                             select data).FirstOrDefault();

            Assert.IsNotNull(occurance, "Occurrence should not be null");
            Assert.AreEqual<Guid>(perf.Token.ApplicationId, occurance.Token.ApplicationId, "Application Id should match");
            Assert.AreEqual<DateTime>(perf.OccurredOn.Date, occurance.OccurredOn.Date, "Occurred On should match");
            Assert.AreEqual<string>(perf.MachineName, occurance.MachineName, "Machine Name should match");
            Assert.AreEqual<string>(perf.Message, occurance.Message, "Message should match");
            Assert.AreEqual<string>(perf.Class, occurance.Class, "Type should match");
            Assert.AreEqual<TimeSpan>(perf.Duration, occurance.Duration, "Duration should match");
            Assert.AreEqual<string>(perf.Method, occurance.Method, "Method should match");
            Assert.AreEqual<int>(perf.ThreadId, occurance.ThreadId, "Thread Id should match");
            Assert.AreEqual<Guid?>(perf.SessionIdentifier, occurance.SessionIdentifier, "Session Identifier should match");
        }

        [TestMethod]
        [Ignore]
        public void GetConfigurationItem()
        {
            var user = new UserData(StringHelper.ValidString(), StringHelper.ValidString(), StringHelper.ValidString());
            var userTable = new AzureTable<UserData>(CloudStorageAccount.DevelopmentStorageAccount);
            userTable.AddEntity(user);
            var userApp = new UserApplicationData(user.Id, user.ApplicationId)
            {
                Active = true
            };
            var table = new AzureTable<UserApplicationData>(CloudStorageAccount.DevelopmentStorageAccount);
            table.AddEntity(userApp);

            var core = new Abc.Services.Core.ApplicationCore();
            var token = new Abc.Services.Contracts.Token()
            {
                ApplicationId = userApp.ApplicationId,
                ValidationKey = application.Token.ValidationKey
            };
            var config = new Abc.Services.Contracts.Configuration()
            {
                Key = StringHelper.ValidString(63),
                Value = StringHelper.ValidString(),
                Token = token,
            };

            var u = new Abc.Services.Contracts.User()
            {
                Identifier = userApp.UserId,
            };
            var a = new Abc.Services.Contracts.Application()
            {
                Identifier = token.ApplicationId,
            };
            var editor = new Abc.Services.Contracts.UserApplication()
            {
                User = u,
                Application = a,
            };
            core.Save(config, editor);

            var query = new Abc.Test.Services.Client.Datum.Client.Configuration()
            {
                Token = new Token(),
                Key = config.Key,
            };
            query.Token.ApplicationId = application.Token.ApplicationId;
            query.Token.ValidationKey = application.Token.ValidationKey;

            using (var client = new DatumClient())
            {
                var returned = client.GetConfiguration(query);
                Assert.IsNotNull(returned);
                Assert.AreEqual<int>(1, returned.Count());
                var item = returned[0];
                Assert.AreEqual<string>(config.Key, item.Key);
                Assert.AreEqual<string>(config.Value, item.Value);
            }
        }

        [TestMethod]
        [Ignore]
        public void GetConfigurationItems()
        {
            var user = new UserData(StringHelper.ValidString(), StringHelper.ValidString(), StringHelper.ValidString());
            var userTable = new AzureTable<UserData>(CloudStorageAccount.DevelopmentStorageAccount);
            userTable.AddEntity(user);
            var userApp = new UserApplicationData(user.Id, user.ApplicationId)
            {
                Active = true
            };
            var table = new AzureTable<UserApplicationData>(CloudStorageAccount.DevelopmentStorageAccount);
            table.AddEntity(userApp);

            var core = new Abc.Services.Core.ApplicationCore();
            var token = new Abc.Services.Contracts.Token()
            {
                ApplicationId = userApp.ApplicationId,
                ValidationKey = application.Token.ValidationKey
            };
            var u = new Abc.Services.Contracts.User()
            {
                Identifier = userApp.UserId,
            };
            var a = new Abc.Services.Contracts.Application()
            {
                Identifier = token.ApplicationId,
            };
            var editor = new Abc.Services.Contracts.UserApplication()
            {
                User = u,
                Application = a,
            };
            var config1 = new Abc.Services.Contracts.Configuration()
            {
                Key = StringHelper.ValidString(63),
                Value = StringHelper.ValidString(),
                Token = token,
            };

            var config2 = new Abc.Services.Contracts.Configuration()
            {
                Key = StringHelper.ValidString(63),
                Value = StringHelper.ValidString(),
                Token = token,
            };

            core.Save(config1, editor);
            core.Save(config2, editor);

            var query = new Abc.Test.Services.Client.Datum.Client.Configuration()
            {
                Token = new Token(),
            };
            query.Token.ApplicationId = application.Token.ApplicationId;
            query.Token.ValidationKey = application.Token.ValidationKey;

            using (var client = new DatumClient())
            {
                var returned = client.GetConfiguration(query);
                Assert.IsNotNull(returned);
                var trimmed = from item in returned
                              where (item.Key == config1.Key && item.Value == config1.Value)
                              || (item.Key == config2.Key && item.Value == config2.Value)
                              select item;
                Assert.AreEqual<int>(2, trimmed.Count());
            }
        }
        #endregion

        #region Helper Methods
        private Message Message()
        {
            var random = new Random();
            var token = new Token()
            {
                ApplicationId = application.Token.ApplicationId,
                ValidationKey = application.Token.ValidationKey
            };
            return new Message()
            {
                MachineName = System.Environment.MachineName,
                Message = Guid.NewGuid().ToString(),
                OccurredOn = DateTime.UtcNow,
                Token = token
            };
        }

        private ErrorItem Error()
        {
            Random random = new Random();
            var token = new Token()
            {
                ApplicationId = application.Token.ApplicationId,
                ValidationKey = application.Token.ValidationKey
            };
            return new ErrorItem()
            {
                ErrorCode = random.Next(),
                MachineName = System.Environment.MachineName,
                Message = Guid.NewGuid().ToString(),
                OccurredOn = new DateTime(DateTime.Now.Ticks),
                Source = "Empty Source",
                StackTrace = "Empty Stack",
                ClassName = this.GetType().ToString(),
                Token = token,
                EventType = EventTypes.Start
            };
        }

        private Occurrence Occurrence()
        {
            Random random = new Random();
            var token = new Token()
            {
                ApplicationId = application.Token.ApplicationId,
                ValidationKey = application.Token.ValidationKey
            };
            return new Occurrence()
            {
                MachineName = System.Environment.MachineName,
                Message = Guid.NewGuid().ToString(),
                OccurredOn = DateTime.UtcNow,
                Class = this.GetType().ToString(),
                ThreadId = random.Next(),
                Duration = new TimeSpan(123412),
                Method = "this is my method",
                Token = token,
                SessionIdentifier = Guid.NewGuid(),
            };
        }

        private Abc.Test.Services.Client.Datum.Client.Configuration Config()
        {
            var token = new Token()
            {
                ApplicationId = application.Token.ApplicationId,
                ValidationKey = application.Token.ValidationKey
            };

            return new Abc.Test.Services.Client.Datum.Client.Configuration()
            {
                Key = StringHelper.ValidString(63),
                Value = StringHelper.ValidString(),
                Token = token,
            };
        }

        private Abc.Test.Services.Client.Datum.Client.ServerStatisticSet ServerStats()
        {
            var random = new Random();
            var token = new Token()
            {
                ApplicationId = application.Token.ApplicationId,
                ValidationKey = application.Token.ValidationKey
            };

            return new Abc.Test.Services.Client.Datum.Client.ServerStatisticSet()
            {
                Token = token,
                CpuUsagePercentage = random.Next(100),
                DeploymentId = StringHelper.ValidString(),
                MachineName = Guid.NewGuid().ToString(),
                MemoryUsagePercentage = random.Next(100),
                OccurredOn = DateTime.UtcNow,
                PhysicalDiskUsagePercentage = random.Next(100),
                NetworkPercentages = new float[] { random.Next(1, 100), random.Next(1, 100), random.Next(1, 100), random.Next(1, 100) },
            };
        }
        #endregion
    }
}