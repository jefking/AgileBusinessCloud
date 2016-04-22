// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='AnalyticsCoreTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Core
{
    using Abc.Services;
    using Abc.Services.Contracts;
    using Abc.Services.Core;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    [TestClass]
    public class AnalyticsCoreTest
    {
        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new AnalyticsCore();
        }

        [TestMethod]
        public void Current()
        {
            var core = new AnalyticsCore();
            Assert.IsNotNull(core.Current);
            var throughput = core.Current;
            Assert.IsTrue(0 <= throughput.EventLog);
            Assert.IsTrue(0 <= throughput.Performance);
            Assert.IsTrue(0 <= throughput.Exceptions);
            Assert.IsTrue(0 <= throughput.Messages);
        }
        #endregion

        #region Helper Methods
        /// <summary>
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
        #endregion
    }
}