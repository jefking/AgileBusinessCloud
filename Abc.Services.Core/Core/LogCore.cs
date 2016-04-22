// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='LogCore.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Core
{
    using Abc.Azure;
    using Abc.Configuration;
    using Abc.Services.Contracts;
    using Abc.Services.Data;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;

    /// <summary>
    /// Log Core
    /// </summary>
    public class LogCore
    {
        #region Members
        /// <summary>
        /// Message 21 Days Format
        /// </summary>
        public const string Message21DaysFormat = "{0}/Messages/21-days";

        /// <summary>
        /// Message 1 Days Format
        /// </summary>
        public const string Message1DaysFormat = "{0}/Messages/1-days";

        /// <summary>
        /// Error 21 Days format
        /// </summary>
        public const string Error21DaysFormat = "{0}/Errors/21-days";

        /// <summary>
        /// Error 1 Day Format
        /// </summary>
        public const string Error1DaysFormat = "{0}/Errors/1-days";

        /// <summary>
        /// Error 1 Days Format
        /// </summary>
        public const string ErrorsCompressed1DaysFormat = "{0}/ErrorsCompressed/1-days";

        /// <summary>
        /// Collector Brief 1 Days Format
        /// </summary>
        public const string CollectorBrief1DaysFormat = "{0}/CollectorBrief/1-days";

        /// <summary>
        /// Performance 21 Days Format
        /// </summary>
        public const string Performance21DaysFormat = "{0}/Occurrences/21-days";

        /// <summary>
        /// Performance 1 Days Format
        /// </summary>
        public const string Performance1DaysFormat = "{0}/Occurrences/1-days";

        /// <summary>
        /// Metrics Lock
        /// </summary>
        private static readonly object metricsLock = new object();

        /// <summary>
        /// Occurance Data Table
        /// </summary>
        private readonly AzureTable<OccurrenceData> occuranceTable = new AzureTable<OccurrenceData>(ServerConfiguration.Default, new OccurrenceDataValidator());

        /// <summary>
        /// Error Data Table
        /// </summary>
        private readonly AzureTable<ErrorData> errorTable = new AzureTable<ErrorData>(ServerConfiguration.Default, new ErrorDataValidator());

        /// <summary>
        /// Bytes Data Table
        /// </summary>
        private readonly AzureTable<BytesStoredData> bytesTable = new AzureTable<BytesStoredData>(ServerConfiguration.Default, new BytesStoredDataValidator());

        /// <summary>
        /// Message Data Table
        /// </summary>
        private readonly AzureTable<MessageData> messageTable = new AzureTable<MessageData>(ServerConfiguration.Default, new MessageDataValidator());

        /// <summary>
        /// Event Log Data Table
        /// </summary>
        private readonly AzureTable<EventLogRow> eventLogTable = new AzureTable<EventLogRow>(ServerConfiguration.Default);

        /// <summary>
        /// General Metric Table
        /// </summary>
        private readonly AzureTable<GeneralMetricRow> generalMetricTable = new AzureTable<GeneralMetricRow>(ServerConfiguration.Default);

        /// <summary>
        /// Server Statistics Row Table
        /// </summary>
        private readonly AzureTable<ServerStatisticsRow> serverStatisticsTable = new AzureTable<ServerStatisticsRow>(ServerConfiguration.Default);

        /// <summary>
        /// Latest Server Statistics Row Table
        /// </summary>
        private readonly AzureTable<LatestServerStatisticsRow> latestServerStatisticsTable = new AzureTable<LatestServerStatisticsRow>(ServerConfiguration.Default);

        /// <summary>
        /// Public Application Identifier
        /// </summary>
        public static readonly Guid PublicApplicationIdentifier = Guid.Parse("897F3AA2-7DD2-4B4B-BC6C-51B1EF65CC4B");
        #endregion

        #region Methods
        #region Web Methods
        /// <summary>
        /// Log Exception
        /// </summary>
        /// <param name="exception">Exception</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "It is validated in Web Method.")]
        public void Log(ErrorItem exception)
        {
            Contract.Requires(null != exception);
            Contract.Requires(null != exception.Token);
            Contract.Requires(Guid.Empty != exception.Token.ApplicationId);

            var errors = new Stack<ErrorItem>();
            var item = exception;
            while (null != item)
            {
                item.OccurredOn = exception.OccurredOn;
                item.Token.ApplicationId = exception.Token.ApplicationId;

                errors.Push(item);

                item = item.Parent;
            }

            var parentId = Guid.Empty;
            ErrorData data;
            while (0 < errors.Count)
            {
                item = errors.Pop();

                data = item.Convert();
                data.ParentId = parentId;

                this.errorTable.AddEntity(data);
                parentId = data.Id;

                this.LogMetric(GeneralMetricRow.Error);
            }
        }

        /// <summary>
        /// Log Message
        /// </summary>
        /// <param name="message">Message</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validation is occuring")]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "As message is meant to scale out, this is the logging method.")]
        public void Log(Message message)
        {
            Contract.Requires(null != message);
            Contract.Requires(null != message.Token);
            Contract.Requires(Guid.Empty != message.Token.ApplicationId);

            using (new PerformanceMonitor())
            {
                var data = message.Convert();

                this.messageTable.AddEntity(data);

                this.StoreByteCount(message.Token.ApplicationId, DataCostType.Stored, data);
            }
        }

        /// <summary>
        /// Log Server Statistic Set
        /// </summary>
        /// <param name="data">Server Statistic Set</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validation is occuring")]
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "As message is meant to scale out, this is the logging method.")]
        public void Log(ServerStatisticSet data)
        {
            Contract.Requires<ArgumentNullException>(null != data);
            Contract.Requires<ArgumentNullException>(null != data.Token);
            Contract.Requires<ArgumentException>(Guid.Empty != data.Token.ApplicationId);

            using (new PerformanceMonitor())
            {
                this.serverStatisticsTable.AddEntity(data.Convert());
                this.latestServerStatisticsTable.AddOrUpdateEntity(((IConvert<LatestServerStatisticsRow>)data).Convert());

                this.StoreByteCount(data.Token.ApplicationId, DataCostType.Stored, data);
            }
        }

        /// <summary>
        /// Log Performance
        /// </summary>
        /// <param name="occurrence">Occurrence</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validation is occuring")]
        public void Log(Occurrence occurrence)
        {
            Contract.Requires(null != occurrence);
            Contract.Requires(null != occurrence.Token);
            Contract.Requires(Guid.Empty != occurrence.Token.ApplicationId);

            var data = occurrence.Convert();

            this.occuranceTable.AddEntity(data);

            this.StoreByteCount(occurrence.Token.ApplicationId, DataCostType.Stored, data);
        }

        /// <summary>
        /// Publish Occurance
        /// </summary>
        /// <param name="query">Log Query</param>
        public void PublishOccurence(LogQuery query)
        {
            Contract.Requires<ArgumentNullException>(null != query);
            Contract.Requires<ArgumentException>(Guid.Empty != query.ApplicationIdentifier);
            Contract.Requires<ArgumentException>(Guid.Empty != query.Identifier);

            using (new PerformanceMonitor())
            {
                var item = this.SelectOccurrences(query).FirstOrDefault();
                if (null != item)
                {
                    var data = item.Convert();
                    data.RowKey = item.Identifier.ToString();
                    data.PartitionKey = PublicApplicationIdentifier.ToString();
                    this.occuranceTable.AddOrUpdateEntity(data);
                }
            }
        }

        /// <summary>
        /// Log Event Log Item
        /// </summary>
        /// <param name="item">Item</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Code Contracts")]
        public void Log(EventLogItem item)
        {
            Contract.Requires<ArgumentNullException>(null != item);
            Contract.Requires<ArgumentNullException>(null != item.Token);
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != item.Token.ApplicationId);

            var data = item.Convert();

            this.eventLogTable.AddEntity(data);

            this.StoreByteCount(item.Token.ApplicationId, DataCostType.Stored, data);

            this.LogMetric(GeneralMetricRow.EventLog);
        }
        #endregion

        /// <summary>
        /// Log Exception
        /// </summary>
        /// <param name="ex">Exception</param>
        /// <param name="eventType">Event Type</param>
        /// <param name="errorCode">Error Code</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Safety first.")]
        public void Log(Exception ex, EventTypes eventType, int errorCode)
        {
            Contract.Requires<ArgumentNullException>(null != ex);
            Contract.Requires<ArgumentException>(0 <= errorCode);

            var exceptions = new Stack<Exception>();
            while (null != ex)
            {
                exceptions.Push(ex);

                ex = ex.InnerException;
            }

            var occurredOn = DateTime.UtcNow;
            var parentId = Guid.Empty;
            ErrorData err;
            while (0 < exceptions.Count)
            {
                ex = exceptions.Pop();

                err = new ErrorData(ServerConfiguration.ApplicationIdentifier)
                {
                    OccurredOn = occurredOn,
                    MachineName = Environment.MachineName,
                    DeploymentId = Abc.Azure.AzureEnvironment.DeploymentId,
                    Message = ex.Message,
                    ParentId = parentId,
                    Source = ex.Source,
                    StackTrace = ex.StackTrace,
                    ErrorCode = errorCode,
                    ClassName = ex.GetType().ToString(),
                    EventTypeValue = (int)eventType
                };

                try
                {
                    this.errorTable.AddEntity(err);
                }
                catch
                {
                    // Safety
                }

                parentId = err.Id;
            }
        }

        /// <summary>
        /// Log Message
        /// </summary>
        /// <param name="message">Message</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Code Contracts")]
        public void Log(string message)
        {
            Contract.Requires<ArgumentOutOfRangeException>(!string.IsNullOrWhiteSpace(message));
            Contract.Requires(message.Length < DataSource.MaximumStringLength);

            var data = new MessageData(ServerConfiguration.ApplicationIdentifier)
            {
                OccurredOn = DateTime.UtcNow,
                MachineName = Environment.MachineName,
                Message = message,
                DeploymentId = AzureEnvironment.DeploymentId,
            };

            this.messageTable.AddEntity(data);
        }

        /// <summary>
        /// Store Byte Count
        /// </summary>
        /// <param name="applicationId">Application Identifier</param>
        /// <param name="costType">Data Cost Type</param>
        /// <param name="data">Data</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "2", Justification = "It is validated.")]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "It is validated.")]
        public void StoreByteCount(Guid applicationId, DataCostType costType, object data)
        {
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != applicationId);

            Contract.Requires<ArgumentNullException>(null != data);

            var stored = new BytesStoredData(applicationId)
            {
                ObjectType = data.GetType().ToString(),
                Bytes = DataCostCalculator.Calculate(data),
                DataCostType = (int)costType
            };

            this.bytesTable.AddEntity(stored);
        }

        /// <summary>
        /// Store Byte Count
        /// </summary>
        /// <param name="applicationId">Application Identifier</param>
        /// <param name="costType">Data Cost Type</param>
        /// <param name="data">Data</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "2", Justification = "It is validated.")]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "It is validated.")]
        public void StoreByteCount(Guid applicationId, DataCostType costType, ICollection data)
        {
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != applicationId);

            Contract.Requires<ArgumentNullException>(null != data);

            var stored = new BytesStoredData(applicationId)
            {
                ObjectType = data.GetType().ToString(),
                Bytes = DataCostCalculator.Calculate(data),
                DataCostType = (int)costType
            };

            this.bytesTable.AddEntity(stored);
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="item">Message</param>
        /// <returns>Number of Items Deleted</returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Deleting based on type")]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "It is being validated")]
        public int Delete(Message item)
        {
            Contract.Requires<ArgumentNullException>(null != item);

            Contract.Requires<ArgumentNullException>(null != item.Token);
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != item.Token.ApplicationId);

            Contract.Ensures(Contract.Result<int>() >= 0);

            var count = 0;
            if (DateTime.UtcNow >= item.OccurredOn)
            {
                var partitionKey = item.Token.ApplicationId.ToString();
                var results = from data in this.messageTable.QueryByPartition(partitionKey)
                              where data.OccurredOn < item.OccurredOn
                              select data;

                var items = results.ToList();
                if (null != items)
                {
                    this.messageTable.DeleteEntity(items);
                    count = items.Count;
                }
            }

            return count;
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="item">Error Item</param>
        /// <returns>Number of Items Deleted</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Deleting based on type"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "It is being validated")]
        public int Delete(ErrorItem item)
        {
            Contract.Requires<ArgumentNullException>(null != item);

            Contract.Requires<ArgumentNullException>(null != item.Token);
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != item.Token.ApplicationId);

            Contract.Ensures(Contract.Result<int>() >= 0);

            var count = 0;
            if (DateTime.UtcNow >= item.OccurredOn)
            {
                var partitionKey = item.Token.ApplicationId.ToString();
                var results = from data in this.errorTable.QueryByPartition(partitionKey)
                              where data.OccurredOn < item.OccurredOn
                              select data;

                var items = results.ToList();
                if (null != items)
                {
                    this.errorTable.DeleteEntity(items);
                    count = items.Count;
                }
            }

            return count;
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="item">Occurance</param>
        /// <returns>Number of Items Deleted</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Deleting based on type"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "It is being validated")]
        public int Delete(Occurrence item)
        {
            Contract.Requires<ArgumentNullException>(null != item);

            Contract.Requires<ArgumentNullException>(null != item.Token);
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != item.Token.ApplicationId);

            Contract.Ensures(Contract.Result<int>() >= 0);

            var count = 0;
            if (DateTime.UtcNow >= item.OccurredOn)
            {
                var partitionKey = item.Token.ApplicationId.ToString();
                var results = from data in this.occuranceTable.QueryByPartition(partitionKey)
                              where data.OccurredOn < item.OccurredOn
                              select data;

                var items = results.ToList();
                if (null != items)
                {
                    this.occuranceTable.DeleteEntity(items);
                    count = items.Count;
                }
            }

            return count;
        }

        /// <summary>
        /// Select Occurrences
        /// </summary>
        /// <param name="query">Query</param>
        /// <returns>Occurrences Data</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Code contracts")]
        public IEnumerable<OccurrenceDisplay> SelectOccurrences(LogQuery query)
        {
            Contract.Requires<ArgumentNullException>(null != query);
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != query.ApplicationIdentifier, "Application Identifier is empty.");

            Contract.Ensures(Contract.Result<IEnumerable<OccurrenceDisplay>>() != null);

            using (new PerformanceMonitor())
            {
                query.Initialize();
                var start = query.From.Value;
                var end = query.To.Value;

                var list = new List<OccurrenceDisplay>();
                if (query.IsUnique)
                {
                    var item = this.occuranceTable.Get<OccurrenceDisplay, OccurrenceData>(query.PartitionKey, query.RowKey);
                    if (null != item)
                    {
                        list.Add(item);
                    }

                    return list;
                }
                else if (start < DateTime.UtcNow.AddHours(-6))
                {
                    var history = this.Get<OccurrenceDisplay>(Performance21DaysFormat.FormatWithCulture(query.PartitionKey));

                    if (null != history)
                    {
                        list.AddRange(history.Items);
                        start = history.MaximumDate.HasValue ? history.MaximumDate.Value : start;
                    }
                }

                if (start < end)
                {
                    using (var perf = new PerformanceMonitor())
                    {
                        perf.Append("Pulling from table storage.");
                        var results = this.occuranceTable.Query<OccurrenceData>(query);
                        var temp = results.AsParallel().ToList().Select(d => d.Convert());
                        perf.Append("{0} pulled from table.", temp.Count());
                        list.AddRange(temp);
                    }
                }

                return query.Filter<OccurrenceDisplay>(list);
            }
        }

        /// <summary>
        /// Get Object from Blob
        /// </summary>
        /// <typeparam name="T">Log Item</typeparam>
        /// <param name="objectId">Object Identifier</param>
        /// <returns>Log History</returns>
        private LogHistory<T> Get<T>(string objectId)
            where T : LogItem
        {
            using (var perf = new PerformanceMonitor())
            {
                perf.Append("Pulling from blob storage.");
                var blob = new BinaryBlob<LogHistory<T>>(ServerConfiguration.Default);
                return blob.GetDigest<LogHistory<T>>(objectId);
            }
        }

        /// <summary>
        /// Select Errors
        /// </summary>
        /// <param name="query">Query</param>
        /// <returns>Errors</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Code contracts")]
        public IEnumerable<ErrorDisplay> SelectErrors(LogQuery query)
        {
            Contract.Requires<ArgumentNullException>(null != query);
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != query.ApplicationIdentifier, "Application Identifier is empty.");

            Contract.Ensures(Contract.Result<IEnumerable<ErrorDisplay>>() != null);

            using (new PerformanceMonitor())
            {
                query.Initialize();
                var start = query.From.Value;
                var end = query.To.Value;

                var list = new List<ErrorDisplay>();
                if (query.IsUnique)
                {
                    var item = this.errorTable.Get<ErrorDisplay, ErrorData>(query.PartitionKey, query.RowKey);
                    if (null != item)
                    {
                        list.Add(item);
                    }

                    return list;
                }
                else if (start < DateTime.UtcNow.AddHours(-6))
                {
                    var history = this.Get<ErrorDisplay>(Error21DaysFormat.FormatWithCulture(query.PartitionKey));

                    if (null != history)
                    {
                        list.AddRange(history.Items);
                        start = history.MaximumDate.HasValue ? history.MaximumDate.Value : start;
                    }
                }

                if (start < end)
                {
                    using (var perf = new PerformanceMonitor())
                    {
                        perf.Append("Pulling from table storage.");
                        var results = this.errorTable.Query<ErrorData>(query);
                        var temp = results.AsParallel().ToList().Select(d => d.Convert());
                        perf.Append("{0} pulled from table.", temp.Count());
                        list.AddRange(temp);
                    }
                }

                var items = query.Filter<ErrorDisplay>(list);
                if (query.Deep.HasValue && !query.Deep.Value && 1 < items.Count())
                {
                    foreach (var error in items)
                    {
                        error.StackTrace = null;
                    }
                }

                return items;
            }
        }

        /// <summary>
        /// Select Messages
        /// </summary>
        /// <param name="query">Query</param>
        /// <returns>Messages</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Code contracts")]
        public IEnumerable<MessageDisplay> SelectMessages(LogQuery query)
        {
            Contract.Requires<ArgumentNullException>(null != query);
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != query.ApplicationIdentifier, "Application Identifier is empty.");

            Contract.Ensures(Contract.Result<IEnumerable<MessageDisplay>>() != null);

            using (new PerformanceMonitor())
            {
                query.Initialize();
                var start = query.From.Value;
                var end = query.To.Value;

                var list = new List<MessageDisplay>();

                if (query.IsUnique)
                {
                    var item = this.messageTable.Get<MessageDisplay, MessageData>(query.PartitionKey, query.RowKey);
                    if (null != item)
                    {
                        list.Add(item);
                    }

                    return list;
                }
                else if (start < DateTime.UtcNow.AddHours(-6))
                {
                    var history = this.Get<MessageDisplay>(Message21DaysFormat.FormatWithCulture(query.PartitionKey));

                    if (null != history)
                    {
                        list.AddRange(history.Items);
                        start = history.MaximumDate.HasValue ? history.MaximumDate.Value : start;
                    }
                }

                if (start < end)
                {
                    using (var perf = new PerformanceMonitor())
                    {
                        perf.Append("Pulling from table storage.");
                        var results = this.messageTable.Query<MessageData>(query);
                        var temp = results.AsParallel().ToList().Select(d => d.Convert());
                        perf.Append(" {0} pulled from table.", temp.Count());
                        list.AddRange(temp);
                    }
                }

                return query.Filter<MessageDisplay>(list);
            }
        }

        /// <summary>
        /// Select Server Statistics
        /// </summary>
        /// <param name="query">Query</param>
        /// <returns>Server Statistics</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Code contracts")]
        public IEnumerable<ServerStatisticSetDisplay> SelectServerStatistics(LogQuery query)
        {
            Contract.Requires<ArgumentNullException>(null != query);
            Contract.Requires<ArgumentException>(Guid.Empty != query.ApplicationIdentifier, "Application Identifier is empty.");

            Contract.Ensures(Contract.Result<IEnumerable<ServerStatisticSet>>() != null);

            using (new PerformanceMonitor())
            {
                query.Initialize();
                var start = query.From.Value;
                var end = query.To.Value;

                if (query.IsUnique)
                {
                    var list = new List<ServerStatisticSetDisplay>();
                    var item = this.serverStatisticsTable.Get<ServerStatisticSetDisplay, ServerStatisticsRow>(query.PartitionKey, query.RowKey);
                    if (null != item)
                    {
                        list.Add(item);
                    }

                    return list;
                }
                else
                {
                    var results = this.serverStatisticsTable.QueryBy(d => d.PartitionKey == query.PartitionKey && d.Timestamp > start && d.Timestamp < end, query.Top.Value);
                    return results.ToList().AsParallel().Select(d => d.Convert());
                }
            }
        }

        /// <summary>
        /// Select Latest Server Statistic Set
        /// </summary>
        /// <param name="query">Query</param>
        /// <returns>Server Statistics Set</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Code  contracts")]
        public IEnumerable<ServerStatisticSetDisplay> LatestServerStatistics(LogQuery query)
        {
            Contract.Requires<ArgumentNullException>(null != query);
            Contract.Requires<ArgumentException>(Guid.Empty != query.ApplicationIdentifier, "Application Identifier is empty.");

            Contract.Ensures(Contract.Result<IEnumerable<ServerStatisticSet>>() != null);

            using (new PerformanceMonitor())
            {
                var results = (from d in this.latestServerStatisticsTable.QueryByPartition(query.ApplicationIdentifier.ToString())
                               where d.OccurredOn > DateTime.UtcNow.AddMinutes(-10)
                               select d).ToList().Select(d => d.Convert());
                var items = new List<ServerStatisticSetDisplay>(results);

                if (query.Deep.HasValue && query.Deep.Value)
                {
                    var start = query.From.HasValue ? query.From.Value : DateTime.UtcNow.AddDays(-1);
                    var dailyResults = (from d in this.serverStatisticsTable.QueryByPartition(query.ApplicationIdentifier.ToString())
                                        where d.OccurredOn > start
                                        select d).ToList();

                    for (int i = 0; i < items.Count(); i++)
                    {
                        items[i].Range = (from d in dailyResults
                                          where string.Equals(d.MachineName, items[i].MachineName, StringComparison.OrdinalIgnoreCase)
                                          select d.Convert()).ToList();
                    }
                }

                return items;
            }
        }

        /// <summary>
        /// Select Seconds Of Type
        /// </summary>
        /// <param name="seconds">Seconds</param>
        /// <returns>Throughput</returns>
        public Throughput SelectSecondsOf(int seconds)
        {
            Contract.Requires<ArgumentOutOfRangeException>(0 < seconds);

            ////Don't Perf; too much logging
            var time = DateTime.UtcNow.AddSeconds(-1 * seconds);
            Throughput metrics = null;
            try
            {
                var results = from data in this.generalMetricTable.QueryByPartition(GeneralMetricRow.Partition())
                              where data.Time >= time
                              select data;

                var list = results.ToList();
                metrics = new Throughput()
                {
                    ServerStatistics = list.Sum(m => m.ServerStatisticsCount),
                    Messages = list.Sum(m => m.MessageCount),
                    EventLog = list.Sum(m => m.EventLogCount),
                    Exceptions = list.Sum(m => m.ErrorCount),
                    Performance = list.Sum(m => m.PerformanceCount),
                };
            }
            catch
            {
                ////Don't log, table misses are frequent
            }

            return metrics ?? new Throughput();
        }

        /// <summary>
        /// Select Errors
        /// </summary>
        /// <param name="applicationId">Application Id</param>
        /// <returns>Errors</returns>
        [CLSCompliant(false)]
        public IEnumerable<BytesStored> SelectBytesStored(Guid applicationId)
        {
            Contract.Requires<ArgumentException>(Guid.Empty != applicationId, "Application Identifier is empty.");

            Contract.Ensures(Contract.Result<IEnumerable<BytesStored>>() != null);

            using (new PerformanceMonitor())
            {
                var partitionKey = applicationId.ToString();
                var results = this.bytesTable.QueryByPartition(partitionKey);

                return results.ToList().Select(d => d.Convert());
            }
        }

        /// <summary>
        /// Log Metric
        /// </summary>
        /// <param name="type">Type</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Logging metrics should bring down caller.")]
        public void LogMetric(Guid type)
        {
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != type);

            //// Do not instrument Performance Metrics, causes recusion

            var metric = new GeneralMetricRow();
            GeneralMetricRow exists = null;
            lock (metricsLock)
            {
                try
                {
                    exists = this.generalMetricTable.QueryBy(metric.PartitionKey, metric.RowKey);
                }
                catch
                {
                    // Do not log, causes recursion
                }

                exists = exists ?? metric;

                if (type == GeneralMetricRow.Error)
                {
                    exists.ErrorCount++;
                }
                else if (type == GeneralMetricRow.EventLog)
                {
                    exists.EventLogCount++;
                }
                else if (type == GeneralMetricRow.Message)
                {
                    exists.MessageCount++;
                }
                else if (type == GeneralMetricRow.Performance)
                {
                    exists.PerformanceCount++;
                }
                else if (type == GeneralMetricRow.ServerStatistics)
                {
                    exists.ServerStatisticsCount++;
                }

                try
                {
                    this.generalMetricTable.AddOrUpdateEntity(exists);
                }
                catch
                {
                    // Do not log, causes recursion
                }
            }
        }

        /// <summary>
        /// Digest Messages
        /// </summary>
        /// <param name="applicationIdentifier">Application Identifier</param>
        /// <returns>History</returns>
        public LogHistory<MessageDisplay> DigestMessages(Guid applicationIdentifier)
        {
            Contract.Requires<ArgumentException>(Guid.Empty != applicationIdentifier);
            Contract.Ensures(Contract.Result<LogHistory<MessageDisplay>>() != null);

            var query = new LogQuery()
            {
                From = DateTime.UtcNow.AddDays(-7),
                Top = 50000,
                ApplicationIdentifier = applicationIdentifier,
            };

            var history = new LogHistory<MessageDisplay>();

            try
            {
                history = this.Digest<MessageDisplay>(this.SelectMessages(query), Message21DaysFormat.FormatWithCulture(applicationIdentifier));
            }
            catch (Exception ex)
            {
                this.Log(ex, EventTypes.Error, (int)ServiceFault.MessageDigest);
            }

            return history;
        }

        /// <summary>
        /// Digest Occurrences
        /// </summary>
        /// <param name="applicationIdentifier">Application Identifier</param>
        /// <returns>History</returns>
        public LogHistory<OccurrenceDisplay> DigestOccurrences(Guid applicationIdentifier)
        {
            Contract.Requires<ArgumentException>(Guid.Empty != applicationIdentifier);
            Contract.Ensures(Contract.Result<LogHistory<OccurrenceDisplay>>() != null);

            var query = new LogQuery()
            {
                From = DateTime.UtcNow.AddDays(-7),
                Top = 50000,
                ApplicationIdentifier = applicationIdentifier,
            };

            var history = new LogHistory<OccurrenceDisplay>();

            try
            {
                history = this.Digest<OccurrenceDisplay>(this.SelectOccurrences(query), Performance21DaysFormat.FormatWithCulture(applicationIdentifier));
            }
            catch (Exception ex)
            {
                this.Log(ex, EventTypes.Error, (int)ServiceFault.PerformanceDigest);
            }

            return history;
        }

        /// <summary>
        /// Digest Errors
        /// </summary>
        /// <param name="applicationIdentifier">Application Identifier</param>
        /// <returns>History</returns>
        public LogHistory<ErrorDisplay> DigestErrors(Guid applicationIdentifier)
        {
            Contract.Requires<ArgumentException>(Guid.Empty != applicationIdentifier);
            Contract.Ensures(Contract.Result<LogHistory<ErrorDisplay>>() != null);

            var query = new LogQuery()
            {
                From = DateTime.UtcNow.AddDays(-7),
                Top = 50000,
                ApplicationIdentifier = applicationIdentifier,
                Deep = true,
            };

            var history = new LogHistory<ErrorDisplay>();

            try
            {
                history = this.Digest<ErrorDisplay>(this.SelectErrors(query), Error21DaysFormat.FormatWithCulture(applicationIdentifier));
            }
            catch (Exception ex)
            {
                this.Log(ex, EventTypes.Error, (int)ServiceFault.PerformanceDigest);
            }

            return history;
        }

        /// <summary>
        /// Digest 
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="items">Items</param>
        /// <param name="objectId">Object Id</param>
        /// <returns>Log History</returns>
        public LogHistory<T> Digest<T>(IEnumerable<T> items, string objectId)
            where T : LogItem
        {
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(objectId));

            Contract.Ensures(Contract.Result<LogHistory<T>>() != null);

            using (new PerformanceMonitor())
            {
                var history = new LogHistory<T>();

                try
                {
                    if (null != items && 0 < items.Count())
                    {
                        history.GeneratedOn = DateTime.UtcNow;
                        history.Count = items.Count();
                        history.Items = items.ToArray();
                        history.MinimumDate = items.Select(m => m.OccurredOn).Min();
                        history.MaximumDate = items.Select(m => m.OccurredOn).Max();

                        var blob = new BinaryBlob<LogHistory<T>>(ServerConfiguration.Default);
                        blob.Save(objectId, history);
                    }
                }
                catch (Exception ex)
                {
                    this.Log(ex, EventTypes.Error, (int)ServiceFault.BlobDigest);
                }

                return history;
            }
        }
        #endregion
    }
}