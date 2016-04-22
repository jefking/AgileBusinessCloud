// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='LoggerTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using Abc.Azure;
    using Abc.Logging;
    using Abc.Services;
    using Abc.Underpinning;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure;

    /// <summary>
    /// Logger Client side test
    /// </summary>
    [TestClass]
    public class LoggerTest
    {
        #region Valid Cases
        [TestMethod]
        public void LogEventItem()
        {
            var logs = EventLog.GetEventLogs();
            var log = logs.FirstOrDefault();
            var entry = log.Entries[0];
            Logger.Log(entry);

            Trace.Flush();
            Thread.Sleep(1250);

            var app = new Application();
            var table = new AzureTable<Abc.Services.Data.EventLogRow>(CloudStorageAccount.DevelopmentStorageAccount);
            var events = table.QueryByPartition(app.Token.ApplicationId.ToString());
            Assert.IsNotNull(events);

            var entryRow = (from data in events
                         where data.InstanceId == entry.InstanceId
                             && data.MachineName == entry.MachineName
                             && data.Source == entry.Source
                         select data).FirstOrDefault();

            Assert.IsNotNull(entryRow);
            Assert.AreEqual<Guid>(Application.Identifier, entryRow.ApplicationId);
            Assert.AreEqual<string>(entry.MachineName, entryRow.MachineName);

            // Can't test due to line breaks.
            ////Assert.AreEqual<string>(entry.Message, entryRow.Message);
            Assert.AreEqual<string>(entry.Source, entryRow.Source);
            Assert.AreEqual<int>((int)entry.EntryType, entryRow.EntryTypeValue);
            Assert.AreEqual<long?>(entry.InstanceId, entryRow.InstanceId);
        }

        [TestMethod]
        public void LogEntryItemNull()
        {
            Logger.Log((EventLogEntry)null);
        }

        [TestMethod]
        [Ignore]
        public void LogError()
        {
            var logger = new Logger();
            var message = Guid.NewGuid().ToString();
            var ex = new Exception(message);
            logger.Log(ex, Logging.EventTypes.Suspend, 9879);

            Trace.Flush();
            Thread.Sleep(1250);

            var app = new Application();
            var table = new AzureTable<ErrorData>(CloudStorageAccount.DevelopmentStorageAccount);
            var errors = table.QueryByPartition(app.Token.ApplicationId.ToString());
            Assert.IsNotNull(errors, "Errors should not be null");
            var error = (from data in errors
                            where ex.Message == data.Message
                            select data).FirstOrDefault();

            Assert.IsNotNull(error, "Error should not be null");
            Assert.AreEqual<Guid>(Application.Identifier, error.ApplicationId, "Application Id should match");
            Assert.AreEqual<string>(System.Environment.MachineName, error.MachineName, "Machine Name should match");
            Assert.AreEqual<string>(ex.Message, error.Message, "Message should match");
            Assert.AreEqual<string>(ex.GetType().ToString(), error.ClassName, "Type should match");
            Assert.AreEqual<int>(9879, error.ErrorCode, "Error Code should match");
            Assert.AreEqual<int>((int)Logging.EventTypes.Suspend, error.EventTypeValue, "Event Type should match");
        }

        [TestMethod]
        [Ignore]
        public void LogErrorWithParents()
        {
            string parentBId = Guid.NewGuid().ToString();
            Exception parentB = new Exception(parentBId);
            string parentAId = Guid.NewGuid().ToString();
            Exception parentA = new Exception(parentAId, parentB);
            string id = Guid.NewGuid().ToString();
            Exception err = new Exception(id, parentA);

            var logger = new Logger();
            logger.Log(err, Logging.EventTypes.Verbose, 324243);

            Trace.Flush();
            Thread.Sleep(1250);

            var app = new Application();
            var table = new AzureTable<ErrorData>(CloudStorageAccount.DevelopmentStorageAccount);
            var errors = table.QueryByPartition(app.Token.ApplicationId.ToString());

            Assert.IsNotNull(errors, "Errors should not be null");

            var data = (from e in errors
                    where e.Message == id
                    select e).FirstOrDefault();
            Assert.IsNotNull(data, "Root Error should not be null");
            Assert.AreEqual<string>(err.Message, data.Message, "Message should match");
            Assert.AreEqual<string>(err.StackTrace, data.StackTrace, "Stack Trace should match");
            Assert.AreEqual<string>(err.Source, data.Source, "Source should match");
            Assert.AreEqual<int>(324243, data.ErrorCode, "Error Code should match");
            Assert.AreEqual<int>((int)Logging.EventTypes.Verbose, data.EventTypeValue, "Event Type should match");

            var pid = data.ParentId.ToString();
            data = (from e in errors
                    where e.RowKey == pid
                    select e).FirstOrDefault();
            Assert.IsNotNull(data, "Parent A Error should not be null");
            Assert.AreEqual<string>(parentA.Message, data.Message, "Message should match");
            Assert.AreEqual<string>(parentA.StackTrace, data.StackTrace, "Stack Trace should match");
            Assert.AreEqual<string>(parentA.Source, data.Source, "Source should match");
            Assert.AreEqual<int>(324243, data.ErrorCode, "Error Code should match");
            Assert.AreEqual<int>((int)Logging.EventTypes.Verbose, data.EventTypeValue, "Event Type should match");

            pid = data.ParentId.ToString();
            data = (from e in errors
                    where e.RowKey == pid
                    select e).FirstOrDefault();
            Assert.IsNotNull(data, "Parent B Error should not be null");
            Assert.AreEqual<string>(parentB.Message, data.Message, "Message should match");
            Assert.AreEqual<string>(parentB.StackTrace, data.StackTrace, "Stack Trace should match");
            Assert.AreEqual<string>(parentB.Source, data.Source, "Source should match");
            Assert.AreEqual<int>(324243, data.ErrorCode, "Error Code should match");
            Assert.AreEqual<int>((int)Logging.EventTypes.Verbose, data.EventTypeValue, "Event Type should match");
        }
        #endregion
    }
}