// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='LoggingCleanerTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Services.Process
{
    using System;
    using Abc.Services.Process;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Abc.Services.Contracts;

    [TestClass]
    public class LoggingCleanerTest
    {
        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new LoggingCleaner();
        }

        [TestMethod]
        public void IsScheduledManager()
        {
            Assert.IsNotNull(new LoggingCleaner() as ScheduledManager);
        }

        [TestMethod]
        public void Expiration()
        {
            Assert.AreEqual<TimeSpan>(new TimeSpan(22, 0, 0, 0), LoggingCleaner.Expiration);
        }

        [TestMethod]
        public void Messages()
        {
            var log = new LoggingCleaner();
            var token = new Token()
            {
                ApplicationId = Guid.NewGuid(),
            };
            log.Messages(token);
        }

        [TestMethod]
        public void Performance()
        {
            var log = new LoggingCleaner();
            var token = new Token()
            {
                ApplicationId = Guid.NewGuid(),
            };
            log.Performance(token);
        }

        public void Errors()
        {
            var log = new LoggingCleaner();
            var token = new Token()
            {
                ApplicationId = Guid.NewGuid(),
            };
            log.Errors(token);
        }
        #endregion
    }
}