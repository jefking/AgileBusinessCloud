// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='LogCoreTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Abc.Azure;
    using Abc.Services;
    using Abc.Services.Contracts;
    using Abc.Services.Core;
    using Abc.Services.Data;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure;

    /// <summary>
    /// Log Core Test
    /// </summary>
    [TestClass]
    public class LogCoreTest
    {
        #region Properties
        public string TestProperty
        {
            get;
            set;
        }

        private EventLogItem EventLog
        {
            get
            {
                var random = new Random();
                var token = new Token()
                {
                    ApplicationId = Guid.NewGuid(),
                };

                return new EventLogItem()
                {
                    DeploymentId = StringHelper.ValidString(),
                    EntryType = EventLogEntryType.Warning,
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
            var table = new AzureTable<GeneralMetricRow>(CloudStorageAccount.DevelopmentStorageAccount);
            table.EnsureExist();
        }
        #endregion

        #region Error Cases
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LogEventLogItemNull()
        {
            var core = new LogCore();
            core.Log((EventLogItem)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DigestObjectIdInvalid()
        {
            var core = new LogCore();
            core.Digest<MessageDisplay>(new List<MessageDisplay>(), StringHelper.NullEmptyWhiteSpace());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DigestMessagesApplicationIdentifierEmpty()
        {
            var core = new LogCore();
            core.DigestMessages(Guid.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DigestErrorsApplicationIdentifierEmpty()
        {
            var core = new LogCore();
            core.DigestErrors(Guid.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DigestOccurrencesApplicationIdentifierEmpty()
        {
            var core = new LogCore();
            core.DigestOccurrences(Guid.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LogEventLogItemTokenNull()
        {
            var core = new LogCore();
            core.Log(new EventLogItem());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void LogEventLogItemTokenApplicationIdEmpty()
        {
            var item = new EventLogItem()
            {
                Token = new Token()
            };
            var core = new LogCore();
            core.Log(item);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LogServerStatisticSetNull()
        {
            var core = new LogCore();
            core.Log((ServerStatisticSet)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LogServerStatisticSetTokenNull()
        {
            var core = new LogCore();
            core.Log(new ServerStatisticSet());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void LogServerStatisticSetTokenApplicationIdEmpty()
        {
            var item = new ServerStatisticSet()
            {
                Token = new Token()
            };
            var core = new LogCore();
            core.Log(item);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void LogMetricEmptyType()
        {
            var core = new LogCore();
            core.LogMetric(Guid.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DeleteErrorsEmptyAppId()
        {
            LogCore core = new LogCore();
            var token = new Token()
            {
                ApplicationId = Guid.Empty,
            };

            var data = new ErrorItem()
            {
                Token = token,
            };
            core.Delete(data);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DeleteMessagesEmptyAppId()
        {
            LogCore core = new LogCore();
            var token = new Token()
            {
                ApplicationId = Guid.Empty,
            };

            var data = new Message()
            {
                Token = token,
            };
            core.Delete(data);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DeleteOccurancesEmptyAppId()
        {
            LogCore core = new LogCore();
            var token = new Token()
            {
                ApplicationId = Guid.Empty,
            };

            var data = new Occurrence()
            {
                Token = token,
            };
            core.Delete(data);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SelectBytesStoredEmptyAppId()
        {
            LogCore core = new LogCore();
            core.SelectBytesStored(Guid.Empty);
        }

        /// <summary>
        /// Null Error Item
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullDeleteErrorItem()
        {
            LogCore core = new LogCore();
            core.Delete((ErrorItem)null);
        }

        /// <summary>
        /// Null Message
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullDeleteMessage()
        {
            LogCore core = new LogCore();
            core.Delete((Message)null);
        }

        /// <summary>
        /// Null Occurrence
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullDeleteOccurrence()
        {
            LogCore core = new LogCore();
            core.Delete((Occurrence)null);
        }

        /// <summary>
        /// Null Error Item Token
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullDeleteErrorItemToken()
        {
            LogCore core = new LogCore();
            var item = new ErrorItem();
            core.Delete(item);
        }

        /// <summary>
        /// Null Message Token
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullDeleteMessageToken()
        {
            LogCore core = new LogCore();
            var item = new Message();
            core.Delete(item);
        }

        /// <summary>
        /// Null Occurrence Token
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullDeleteOccurrenceToken()
        {
            LogCore core = new LogCore();
            var item = new Occurrence();
            core.Delete(item);
        }

        /// <summary>
        /// Error Item Token Empty Guid
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void EmptyGuidDeleteErrorItem()
        {
            LogCore core = new LogCore();
            var item = new ErrorItem();
            item.Token = new Token();
            core.Delete(item);
        }

        /// <summary>
        /// Message Token Empty Guid
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void EmptyGuidDeleteMessage()
        {
            LogCore core = new LogCore();
            var item = new Message();
            item.Token = new Token();
            core.Delete(item);
        }

        /// <summary>
        /// Occurrence Token Empty Guid
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void EmptyGuidDeleteOccurrence()
        {
            LogCore core = new LogCore();
            var item = new Occurrence();
            item.Token = new Token();
            core.Delete(item);
        }

        /// <summary>
        /// Null Exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullException()
        {
            LogCore core = new LogCore();
            core.Log(null, EventTypes.Error, 1);
        }

        /// <summary>
        /// Negative Error Code
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void NegativeErrorCode()
        {
            LogCore core = new LogCore();
            core.Log(new Exception(), EventTypes.Error, -123);
        }

        /// <summary>
        /// Empty String in Log Message
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void LogMessageInvalidString()
        {
            LogCore core = new LogCore();
            core.Log(StringHelper.NullEmptyWhiteSpace());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void StoreTransferredByteCountEmptyGuid()
        {
            LogCore core = new LogCore();
            core.StoreByteCount(Guid.Empty, DataCostType.Egress, this.GetType());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void StoreTransferredByteCountCollectionEmptyGuid()
        {
            LogCore core = new LogCore();
            core.StoreByteCount(Guid.Empty, DataCostType.Egress, new List<object>());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void StoreTransferredByteCountNullCollection()
        {
            LogCore core = new LogCore();
            core.StoreByteCount(Guid.NewGuid(), DataCostType.Egress, (ICollection)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void StoreTransferredByteCountNullType()
        {
            LogCore core = new LogCore();
            core.StoreByteCount(Guid.NewGuid(), DataCostType.Egress, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SelectSecondsOfZero()
        {
            var core = new LogCore();
            core.SelectSecondsOf(0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SelectServerStatisticsNull()
        {
            var core = new LogCore();
            core.SelectServerStatistics(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SelectServerStatisticsAppIdEmpty()
        {
            var core = new LogCore();
            core.SelectServerStatistics(new LogQuery());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PublishOccurenceQueryNull()
        {
            var core = new LogCore();
            core.PublishOccurence(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void PublishOccurenceApplicationIdentifierEmpty()
        {
            var query = new LogQuery()
            {
                ApplicationIdentifier = Guid.Empty,
                Identifier = Guid.NewGuid(),
            };

            var core = new LogCore();
            core.PublishOccurence(query);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void PublishOccurenceIdentifierEmpty()
        {
            var query = new LogQuery()
            {
                ApplicationIdentifier = Guid.NewGuid(),
                Identifier = Guid.Empty,
            };

            var core = new LogCore();
            core.PublishOccurence(query);
        }
        #endregion

        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new LogCore();
        }

        [TestMethod]
        public void PublicApplicationIdentifier()
        {
            Assert.AreEqual<Guid>(Guid.Parse("897F3AA2-7DD2-4B4B-BC6C-51B1EF65CC4B"), LogCore.PublicApplicationIdentifier);
        }

        [TestMethod]
        public void Message21DaysFormat()
        {
            Assert.AreEqual<string>("{0}/Messages/21-days", LogCore.Message21DaysFormat);
        }

        [TestMethod]
        public void Error21DaysFormat()
        {
            Assert.AreEqual<string>("{0}/Errors/21-days", LogCore.Error21DaysFormat);
        }

        [TestMethod]
        public void Performance21DaysFormat()
        {
            Assert.AreEqual<string>("{0}/Occurrences/21-days", LogCore.Performance21DaysFormat);
        }

        [TestMethod]
        [Ignore]
        public void LogMetric()
        {
            var id = GeneralMetricRow.ServerStatistics;
            var log = new LogCore();
            log.LogMetric(id);

            var row = new GeneralMetricRow();
            var table = new AzureTable<GeneralMetricRow>(CloudStorageAccount.DevelopmentStorageAccount);
            var item = table.QueryBy(row.PartitionKey, row.RowKey);
            Assert.IsTrue(1 <= item.ServerStatisticsCount, item.ServerStatisticsCount.ToString());
        }

        [TestMethod]
        public void DigestMessages()
        {
            var appId = Guid.NewGuid();
            var log = new LogCore();
            for (int i = 0; i < 9; i++)
            {
                var msg = this.Message(appId);
                log.Log(msg);
            }

            var digest = log.DigestMessages(appId);
            Assert.AreEqual<int>(9, digest.Count);
            Assert.IsNotNull(digest.Items);
            Assert.AreEqual<int>(digest.Count, digest.Items.Count());
            Assert.AreEqual<DateTime?>(DateTime.UtcNow.Date, digest.MaximumDate.Value.Date);
            Assert.AreEqual<DateTime?>(DateTime.UtcNow.Date, digest.MinimumDate.Value.Date);
        }

        [TestMethod]
        public void Digest()
        {
            var appId = Guid.NewGuid();
            var items = new List<EventLogItem>();
            var random = new Random();
            var log = new LogCore();
            for (int i = 0; i < random.Next(10000); i++)
            {
                items.Add(this.EventLogItem(Guid.NewGuid()));
            }

            var digest = log.Digest<EventLogItem>(items, "happy place");
            Assert.AreEqual<int>(items.Count, digest.Count);
            Assert.IsNotNull(digest.Items);
            Assert.AreEqual<int>(digest.Count, digest.Items.Count());
            Assert.AreEqual<DateTime?>(DateTime.UtcNow.Date, digest.MaximumDate.Value.Date);
            Assert.AreEqual<DateTime?>(DateTime.UtcNow.Date, digest.MinimumDate.Value.Date);
        }

        [TestMethod]
        public void DigestErrors()
        {
            var appId = Guid.NewGuid();
            var log = new LogCore();
            for (int i = 0; i < 9; i++)
            {
                var item = this.Error(appId);
                log.Log(item);
            }

            var digest = log.DigestErrors(appId);
            Assert.AreEqual<int>(9, digest.Count);
            Assert.IsNotNull(digest.Items);
            Assert.AreEqual<int>(digest.Count, digest.Items.Count());
            Assert.AreEqual<DateTime?>(DateTime.UtcNow.Date, digest.MaximumDate.Value.Date);
            Assert.AreEqual<DateTime?>(DateTime.UtcNow.Date, digest.MinimumDate.Value.Date);
        }

        [TestMethod]
        public void DigestOccurrences()
        {
            var appId = Guid.NewGuid();
            var log = new LogCore();
            for (int i = 0; i < 9; i++)
            {
                var item = this.Occurrence(appId);
                log.Log(item);
            }

            var digest = log.DigestOccurrences(appId);
            Assert.AreEqual<int>(9, digest.Count);
            Assert.IsNotNull(digest.Items);
            Assert.AreEqual<int>(digest.Count, digest.Items.Count());
            Assert.AreEqual<DateTime?>(DateTime.UtcNow.Date, digest.MaximumDate.Value.Date);
            Assert.AreEqual<DateTime?>(DateTime.UtcNow.Date, digest.MinimumDate.Value.Date);
        }

        [TestMethod]
        [Ignore]
        public void LogError()
        {
            var message = Guid.NewGuid().ToString();
            var err = new Exception(message);

            var random = new Random();
            var errorCode = random.Next();
            var log = new LogCore();
            log.Log(err, EventTypes.Error, errorCode);

            var source = new AzureTable<ErrorData>(CloudStorageAccount.DevelopmentStorageAccount);
            var error = (from e in source.QueryByPartition(ConfigurationSettings.ApplicationIdentifier.ToString())
                         where e.ErrorCode == errorCode
                         && e.Message == message
                         select e).FirstOrDefault();

            Assert.IsNotNull(error, "Error should not be null");
            Assert.AreEqual<string>(err.Message, error.Message, "Message should match");
            Assert.AreEqual<string>(err.StackTrace, error.StackTrace, "Stack Trace should match");
            Assert.AreEqual<string>(err.Source, error.Source, "Source should match");
        }

        [TestMethod]
        public void LogServerStatisticSet()
        {
            var set = this.ServerStat(Guid.NewGuid());

            var log = new LogCore();
            log.Log(set);

            var source = new AzureTable<ServerStatisticsRow>(CloudStorageAccount.DevelopmentStorageAccount);
            var row = (from e in source.QueryByPartition(set.Token.ApplicationId.ToString())
                       select e).First();

            Assert.IsNotNull(row, "Row should not be null");
            Assert.AreEqual<string>(set.MachineName, row.MachineName);
            Assert.AreEqual<string>(set.DeploymentId, row.DeploymentId);
            Assert.AreEqual<double>(set.PhysicalDiskUsagePercentage, row.PhysicalDiskUsagePercentage);
            Assert.AreEqual<double>(set.CpuUsagePercentage, row.CpuUsagePercentage);
            Assert.AreEqual<double>(set.MemoryUsagePercentage, row.MemoryUsagePercentage);
        }

        [TestMethod]
        public void LogServerStatisticSetLatest()
        {
            var set = this.ServerStat(Guid.NewGuid());

            var log = new LogCore();
            log.Log(set);

            var source = new AzureTable<ServerStatisticsRow>(CloudStorageAccount.DevelopmentStorageAccount);
            var rows = source.QueryByPartition(set.Token.ApplicationId.ToString()).ToList();

            Assert.IsNotNull(rows, "Row should not be null");
            Assert.AreEqual<int>(1, rows.Count());
            var row = rows.FirstOrDefault();
            Assert.AreEqual<string>(set.MachineName, row.MachineName);
            Assert.AreEqual<string>(set.DeploymentId, row.DeploymentId);
            Assert.AreEqual<double>(set.PhysicalDiskUsagePercentage, row.PhysicalDiskUsagePercentage);
            Assert.AreEqual<double>(set.CpuUsagePercentage, row.CpuUsagePercentage);
            Assert.AreEqual<double>(set.MemoryUsagePercentage, row.MemoryUsagePercentage);
        }

        [TestMethod]
        [Ignore]
        public void LogErrorWithParents()
        {
            var parentBId = Guid.NewGuid().ToString();
            var parentB = new Exception(parentBId);
            var parentAId = Guid.NewGuid().ToString();
            var parentA = new Exception(parentAId, parentB);
            var id = Guid.NewGuid().ToString();
            var err = new Exception(id, parentA);

            var random = new Random();
            var errorCode = random.Next();
            var log = new LogCore();
            log.Log(err, EventTypes.Error, errorCode);
            var source = new AzureTable<ErrorData>(CloudStorageAccount.DevelopmentStorageAccount);
            var errors = (from e in source.QueryByPartition(ConfigurationSettings.ApplicationIdentifier.ToString())
                              where e.ErrorCode == errorCode
                              select e).ToList();
            Assert.IsNotNull(errors, "Errors should not be null");

            var data = (from e in errors
                    where e.Message == id
                    select e).FirstOrDefault();
            Assert.IsNotNull(data, "Root Error should not be null");
            Assert.AreEqual<string>(err.Message, data.Message, "Message should match");
            Assert.AreEqual<string>(err.StackTrace, data.StackTrace, "Stack Trace should match");
            Assert.AreEqual<string>(err.Source, data.Source, "Source should match");

            data = (from e in errors
                    where e.Id == data.ParentId
                    select e).FirstOrDefault();
            Assert.IsNotNull(data, "Parent A Error should not be null");
            Assert.AreEqual<string>(parentA.Message, data.Message, "Message should match");
            Assert.AreEqual<string>(parentA.StackTrace, data.StackTrace, "Stack Trace should match");
            Assert.AreEqual<string>(parentA.Source, data.Source, "Source should match");

            data = (from e in errors
                    where e.Id == data.ParentId
                        select e).FirstOrDefault();
            Assert.IsNotNull(data, "Parent B Error should not be null");
            Assert.AreEqual<string>(parentB.Message, data.Message, "Message should match");
            Assert.AreEqual<string>(parentB.StackTrace, data.StackTrace, "Stack Trace should match");
            Assert.AreEqual<string>(parentB.Source, data.Source, "Source should match");
        }

        [TestMethod]
        public void StoreTransferredByteCount()
        {
            this.TestProperty = "This is a test property...............";
            Guid appId = Guid.NewGuid();
            LogCore core = new LogCore();
            core.StoreByteCount(appId, DataCostType.Egress, this);

            var source = new LogCore();
            var stored = source.SelectBytesStored(appId);
            Assert.IsNotNull(stored, "Should have 1 stored item.");
            Assert.AreEqual<int>(1, stored.Count(), "Should only have 1 stored item matching uniqu app id.");
            var item = stored.FirstOrDefault();
            Assert.AreEqual<Guid>(appId, item.ApplicationId, "Application Id should be what was sent in.");
            Assert.AreEqual<string>(this.GetType().ToString(), item.ObjectType, "Object type should be same as this object.");
            Assert.AreEqual<int>(100, item.Bytes, "Bytes should be 100");
        }

        [TestMethod]
        public void StoreTransferredByteCountCollection()
        {
            var appId = Guid.NewGuid();
            var core = new LogCore();

            var random = new Random();
            var count = random.Next(15);
            var data = new TestData()
            {
                BoolTest = true,
                DateTimeTest = DateTime.UtcNow,
                IntTest = random.Next()
            };
            int dataCost = DataCostCalculator.Calculate(data);

            var items = new List<TestData>(count);
            for (int i = 0; i < count; i++)
            {
                items.Add(data);
            }

            core.StoreByteCount(appId, DataCostType.Ingress, items);

            var source = new LogCore();
            var stored = source.SelectBytesStored(appId);
            Assert.IsNotNull(stored, "Should have 1 stored item.");
            Assert.AreEqual<int>(1, stored.Count(), "Should only have 1 stored item matching uniqu app id.");
            var item = stored.FirstOrDefault();
            Assert.AreEqual<Guid>(appId, item.ApplicationId, "Application Id should be what was sent in.");
            Assert.AreEqual<string>(items.GetType().ToString(), item.ObjectType, "Object type should be same as this object.");
            Assert.AreEqual<int>(dataCost * count, item.Bytes);
        }

        [TestMethod]
        public void SearchErrors()
        {
            var source = new LogCore();
            Guid appId = Guid.NewGuid();
            for (int i = 0; i < 4; i++)
            {
                var data = this.Error(appId);

                source.Log(data);
            }

            var query = new LogQuery()
            {
                ApplicationIdentifier = appId,
                Top = 1,
            };

            var errors = source.SelectErrors(query);
            Assert.IsNotNull(errors, "Error data should not be null.");
            Assert.AreEqual<int>(1, errors.Count(), "1 Error Should be contained.");

            query.Top = 3;

            errors = source.SelectErrors(query);
            Assert.IsNotNull(errors, "Error data should not be null.");
            Assert.AreEqual<int>(3, errors.Count(), "3 Error Should be contained.");
        }

        [TestMethod]
        public void SearchOccurrences()
        {
            var source = new LogCore();
            Guid appId = Guid.NewGuid();
            for (int i = 0; i < 4; i++)
            {
                var data = this.Occurrence(appId);

                source.Log(data);
            }

            var query = new LogQuery()
            {
                ApplicationIdentifier = appId,
                Top = 1,
            };

            var occurrences = source.SelectOccurrences(query);
            Assert.IsNotNull(occurrences, "Error data should not be null.");
            Assert.AreEqual<int>(1, occurrences.Count(), "1 Error Should be contained.");

            query.Top = 3;

            occurrences = source.SelectOccurrences(query);
            Assert.IsNotNull(occurrences, "Error data should not be null.");
            Assert.AreEqual<int>(3, occurrences.Count(), "3 Error Should be contained.");
        }

        [TestMethod]
        public void SearchMessages()
        {
            var source = new LogCore();
            Guid appId = Guid.NewGuid();
            for (int i = 0; i < 4; i++)
            {
                var data = this.Message(appId);

                source.Log(data);
            }

            var query = new LogQuery()
            {
                ApplicationIdentifier = appId,
                Top = 1,
            };

            var messages = source.SelectMessages(query);
            Assert.IsNotNull(messages, "Message data should not be null.");
            Assert.AreEqual<int>(1, messages.Count(), "1 Message Should be contained.");

            query.Top = 3;

            messages = source.SelectMessages(query);
            Assert.IsNotNull(messages, "Message data should not be null.");
            Assert.AreEqual<int>(3, messages.Count(), "3 Message Should be contained.");
        }

        [TestMethod]
        public void SearchServerStatistics()
        {
            var source = new LogCore();
            var appId = Guid.NewGuid();
            for (var i = 0; i < 4; i++)
            {
                var data = this.ServerStat(appId);

                source.Log(data);
            }

            var query = new LogQuery()
            {
                ApplicationIdentifier = appId,
                Top = 1,
            };

            var datas = source.SelectServerStatistics(query);
            Assert.IsNotNull(datas, "data should not be null.");
            Assert.AreEqual<int>(1, datas.Count(), "1 Should be contained.");

            query.Top = 3;

            datas = source.SelectServerStatistics(query);
            Assert.IsNotNull(datas, "data should not be null.");
            Assert.AreEqual<int>(3, datas.Count(), "3 Should be contained.");
        }

        [TestMethod]
        public void LatestServerStatistics()
        {
            var source = new LogCore();
            var appId = Guid.NewGuid();
            var server = Guid.NewGuid().ToString();
            ServerStatisticSet data = null;
            for (var i = 0; i < 4; i++)
            {
                data = this.ServerStat(appId);
                data.MachineName = server;
                source.Log(data);
            }

            Assert.IsNotNull(data);

            var query = new LogQuery()
            {
                ApplicationIdentifier = appId,
            };

            var results = source.LatestServerStatistics(query);
            Assert.IsNotNull(results, "Result should not be null.");
            Assert.AreEqual<int>(1, results.Count());
            var result = results.FirstOrDefault();
            Assert.IsNotNull(result, "Result should not be null.");
            Assert.AreEqual<double>(data.CpuUsagePercentage, result.CpuUsagePercentage);
            Assert.AreEqual<double>(data.MemoryUsagePercentage, result.MemoryUsagePercentage);
            Assert.AreEqual<double>(data.PhysicalDiskUsagePercentage, result.PhysicalDiskUsagePercentage);
            Assert.AreEqual<string>(data.MachineName, result.MachineName);
        }

        [TestMethod]
        public void LatestServerStatisticsDeep()
        {
            var source = new LogCore();
            var appId = Guid.NewGuid();
            var server = Guid.NewGuid().ToString();
            ServerStatisticSet data = null;
            for (var i = 0; i < 4; i++)
            {
                data = this.ServerStat(appId);
                data.MachineName = server;
                source.Log(data);
            }

            Assert.IsNotNull(data);

            var query = new LogQuery()
            {
                ApplicationIdentifier = appId,
                Deep = true,
            };

            var results = source.LatestServerStatistics(query);
            Assert.IsNotNull(results, "Result should not be null.");
            Assert.AreEqual<int>(1, results.Count());
            var result = results.FirstOrDefault();
            Assert.IsNotNull(result, "Result should not be null.");
            Assert.AreEqual<double>(data.CpuUsagePercentage, result.CpuUsagePercentage);
            Assert.AreEqual<double>(data.MemoryUsagePercentage, result.MemoryUsagePercentage);
            Assert.AreEqual<double>(data.PhysicalDiskUsagePercentage, result.PhysicalDiskUsagePercentage);
            Assert.AreEqual<string>(data.MachineName, result.MachineName);
            Assert.AreEqual<int>(4, result.Range.Count());
        }

        [TestMethod]
        public void LatestServerStatisticsMultiple()
        {
            var source = new LogCore();
            var appId = Guid.NewGuid();
            ServerStatisticSet data = null;
            for (var i = 0; i < 4; i++)
            {
                data = this.ServerStat(appId);
                source.Log(data);
            }

            Assert.IsNotNull(data);

            var query = new LogQuery()
            {
                ApplicationIdentifier = appId,
            };

            var results = source.LatestServerStatistics(query);
            Assert.IsNotNull(results, "Result should not be null.");
            Assert.AreEqual<int>(4, results.Count());
        }

        [TestMethod]
        public void RoundTripOccurance()
        {
            var data = this.Occurrence(Guid.NewGuid());

            var source = new LogCore();
            source.Log(data);

            var query = new Abc.Services.Contracts.LogQuery()
            {
                ApplicationIdentifier = data.Token.ApplicationId,
            };
            var occurances = source.SelectOccurrences(query);
            Assert.IsNotNull(occurances, "Occurances should not be null");
            Assert.AreEqual<int>(1, occurances.Count(), "There should only be one occurance.");
            var occurance = occurances.FirstOrDefault();
            Assert.IsNotNull(occurance, "Occurance should not be null");
            Assert.AreEqual<Guid>(data.Token.ApplicationId, occurance.Token.ApplicationId, "Application Id should match");
            Assert.AreEqual<DateTime>(data.OccurredOn.Date, occurance.OccurredOn.Date, "Occured On should match");
            Assert.AreEqual<string>(data.MachineName, occurance.MachineName, "Machine Name should match");
            Assert.AreEqual<string>(data.Message, occurance.Message, "Message should match");
            Assert.AreEqual<string>(data.Class, occurance.Class, "Type should match");
            Assert.AreEqual<TimeSpan>(data.Duration, occurance.Duration, "Duration should match");
            Assert.AreEqual<string>(data.Method, occurance.Method, "Method should match");
            Assert.AreEqual<int>(data.ThreadId, occurance.ThreadId, "Thread Id should match");
        }

        [TestMethod]
        public void DeleteOccurrence()
        {
            LogCore core = new LogCore();
            Guid appId = Guid.NewGuid();
            for (int i = -4; i < 0; i++)
            {
                var data = this.Occurrence(appId);
                data.OccurredOn = data.OccurredOn.AddDays(i);
                core.Log(data);
            }

            for (int i = -4; i < 0; i++)
            {
                var data = this.Occurrence(appId);
                data.OccurredOn = DateTime.UtcNow.AddDays(i);
                int count = core.Delete(data);
                Assert.AreNotEqual<int>(0, count, "No items were deleted.");
            }
        }

        [TestMethod]
        public void DeleteMessages()
        {
            LogCore core = new LogCore();
            Guid appId = Guid.NewGuid();
            for (int i = -4; i < 0; i++)
            {
                var data = this.Message(appId);
                data.OccurredOn = data.OccurredOn.AddDays(i);
                core.Log(data);
            }

            for (int i = -4; i < 0; i++)
            {
                var data = this.Message(appId);
                data.OccurredOn = DateTime.UtcNow.AddDays(i);
                int count = core.Delete(data);
                Assert.AreNotEqual<int>(0, count, "No items were deleted.");
            }
        }

        [TestMethod]
        public void DeleteErrors()
        {
            var core = new LogCore();
            var appId = Guid.NewGuid();
            for (int i = -4; i < 0; i++)
            {
                var data = this.Error(appId);
                data.OccurredOn = data.OccurredOn.AddDays(i);
                core.Log(data);
            }

            for (int i = -4; i < 0; i++)
            {
                var data = this.Error(appId);
                data.OccurredOn = DateTime.UtcNow.AddDays(i);
                int count = core.Delete(data);
                Assert.AreNotEqual<int>(0, count, "No items were deleted.");
            }
        }

        [TestMethod]
        public void SelectSecondsOfExceptions()
        {
            var core = new LogCore();
            var appId = Guid.NewGuid();
            for (int i = 0; i < 4; i++)
            {
                var data = this.Error(appId);
                core.Log(data);
            }

            Thread.Sleep(250);

            var exceptions = core.SelectSecondsOf(3);
            Assert.IsTrue(3 <= exceptions.Exceptions, exceptions.Exceptions.ToString());
        }

        [TestMethod]
        [Ignore]
        public void SelectSecondsOfMessages()
        {
            var core = new LogCore();
            var appId = Guid.NewGuid();
            for (int i = 0; i < 4; i++)
            {
                var data = this.Message(appId);
                core.Log(data);
            }

            Thread.Sleep(250);

            var count = core.SelectSecondsOf(3);
            Assert.IsTrue(3 <= count.Messages, count.Messages.ToString());
        }

        [TestMethod]
        [Ignore]
        public void SelectSecondsOfPerformance()
        {
            var core = new LogCore();
            var appId = Guid.NewGuid();
            for (int i = 0; i < 4; i++)
            {
                core.Log(this.Occurrence(appId));
            }

            Thread.Sleep(250);

            var count = core.SelectSecondsOf(3);
            Assert.IsTrue(3 <= count.Performance, count.Performance.ToString());
        }

        [TestMethod]
        public void LogEventLogItem()
        {
            var core = new LogCore();
            var item = this.EventLog;

            core.Log(item);
            var table = new AzureTable<EventLogRow>(CloudStorageAccount.DevelopmentStorageAccount);
            var returned = (from data in table.QueryByPartition(item.Token.ApplicationId.ToString())
                         select data).FirstOrDefault();

            Assert.IsNotNull(returned);
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
        public void GetError()
        {
            var source = new LogCore();
            Guid appId = Guid.NewGuid();
            for (int i = 0; i < 4; i++)
            {
                var data = this.Error(appId);

                source.Log(data);
            }

            var query = new LogQuery()
            {
                ApplicationIdentifier = appId,
                Top = 1,
            };

            var errors = source.SelectErrors(query);

            var item = errors.First();
            query.Identifier = item.Identifier;

            var error = source.SelectErrors(query);
            Assert.AreEqual<int>(1, error.Count());
            var returned = error.First();
            Assert.AreEqual<Guid>(item.Identifier, returned.Identifier);
        }

        [TestMethod]
        public void GetOccurrence()
        {
            var source = new LogCore();
            var appId = Guid.NewGuid();
            for (int i = 0; i < 4; i++)
            {
                var data = this.Occurrence(appId);

                source.Log(data);
            }

            var query = new LogQuery()
            {
                ApplicationIdentifier = appId,
                Top = 1,
            };

            var items = source.SelectOccurrences(query);

            var item = items.First();
            query.Identifier = item.Identifier;

            var returnedItems = source.SelectOccurrences(query);
            Assert.AreEqual<int>(1, returnedItems.Count());
            var returned = returnedItems.First();
            Assert.AreEqual<Guid>(item.Identifier, returned.Identifier);
        }

        [TestMethod]
        public void GetMessage()
        {
            var source = new LogCore();
            var appId = Guid.NewGuid();
            for (int i = 0; i < 4; i++)
            {
                var data = this.Message(appId);

                source.Log(data);
            }

            var query = new LogQuery()
            {
                ApplicationIdentifier = appId,
                Top = 1,
            };

            var items = source.SelectMessages(query);

            var item = items.First();
            query.Identifier = item.Identifier;

            var returnedItems = source.SelectMessages(query);
            Assert.AreEqual<int>(1, returnedItems.Count());
            var returned = returnedItems.First();
            Assert.AreEqual<Guid>(item.Identifier, returned.Identifier);
        }

        [TestMethod]
        public void GetServerStatistic()
        {
            var source = new LogCore();
            var appId = Guid.NewGuid();
            for (var i = 0; i < 4; i++)
            {
                var data = this.ServerStat(appId);

                source.Log(data);
            }

            var query = new LogQuery()
            {
                ApplicationIdentifier = appId,
                Top = 1,
            };

            var items = source.SelectServerStatistics(query);

            var item = items.First();
            query.Identifier = item.Identifier;

            var returnedItems = source.SelectServerStatistics(query);
            Assert.AreEqual<int>(1, returnedItems.Count());
            var returned = returnedItems.First();
            Assert.AreEqual<Guid>(item.Identifier, returned.Identifier);
        }

        [TestMethod]
        public void PublishOccurence()
        {
            var source = new LogCore();
            var appId = Guid.NewGuid();
            var data = this.Occurrence(appId);
            source.Log(data);

            var query = new LogQuery()
            {
                ApplicationIdentifier = appId,
            };

            var item = source.SelectOccurrences(query).FirstOrDefault();
            Assert.IsNotNull(item);

            query.Identifier = item.Identifier;

            source.PublishOccurence(query);

            query.ApplicationIdentifier = LogCore.PublicApplicationIdentifier;
            var published = source.SelectOccurrences(query).FirstOrDefault();
            Assert.IsNotNull(published);
            Assert.AreEqual<Guid>(item.Identifier, published.Identifier);
            Assert.AreEqual<string>(item.Class, published.Class);
            Assert.AreEqual<string>(item.DeploymentId, published.DeploymentId);
            Assert.AreEqual<TimeSpan>(item.Duration, published.Duration);
            Assert.AreEqual<string>(item.MachineName, published.MachineName);
            Assert.AreEqual<string>(item.Message, published.Message);
            Assert.AreEqual<string>(item.Method, published.Method);
            Assert.AreEqual<DateTime>(item.OccurredOn, published.OccurredOn);
            Assert.AreEqual<Guid?>(item.SessionIdentifier, published.SessionIdentifier);
            Assert.AreEqual<int>(item.ThreadId, published.ThreadId);
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Occurrence
        /// </summary>
        /// <param name="appId">Application Identifier</param>
        /// <returns>Occurrence Data</returns>
        private Occurrence Occurrence(Guid appId)
        {
            var token = new Token()
            {
                ApplicationId = appId
            };

            return new Occurrence()
            {
                OccurredOn = DateTime.UtcNow,
                Token = token
            };
        }

        /// <summary>
        /// Message
        /// </summary>
        /// <param name="appId">Application Identifier</param>
        /// <returns>Message Data</returns>
        private Message Message(Guid appId)
        {
            var token = new Token()
            {
                ApplicationId = appId
            };

            return new Message()
            {
                OccurredOn = DateTime.UtcNow,
                Token = token,
                Message = StringHelper.ValidString(),
            };
        }

        /// <summary>
        /// Error
        /// </summary>
        /// <param name="appId">Application Identifier</param>
        /// <returns>Error Data</returns>
        private ErrorItem Error(Guid appId)
        {
            var token = new Token()
            {
                ApplicationId = appId
            };

            return new ErrorItem()
            {
                OccurredOn = DateTime.UtcNow,
                Token = token
            };
        }

        private ServerStatisticSet ServerStat(Guid appId)
        {
            var random = new Random();
            return new ServerStatisticSet()
            {
                CpuUsagePercentage = random.Next(),
                MemoryUsagePercentage = random.Next(),
                PhysicalDiskUsagePercentage = random.Next(),
                DeploymentId = StringHelper.ValidString(),
                MachineName = Guid.NewGuid().ToString(),
                OccurredOn = DateTime.UtcNow,
                Token = new Token()
                {
                    ApplicationId = appId,
                },
            };
        }

        private EventLogItem EventLogItem(Guid appId)
        {
            var random = new Random();
            var token = new Token()
            {
                ApplicationId = appId
            };

            return new EventLogItem()
            {
                Token = token,
                MachineName = Environment.MachineName,
                OccurredOn = DateTime.UtcNow,
                EventId = random.Next(),
            };
        }
        #endregion

        #region Classes
        /// <summary>
        /// Test Data
        /// </summary>
        private class TestData
        {
            public bool BoolTest
            {
                get;
                set;
            }

            public int IntTest
            {
                get;
                set;
            }

            public DateTime DateTimeTest
            {
                get;
                set;
            }
        }
        #endregion
    }
}