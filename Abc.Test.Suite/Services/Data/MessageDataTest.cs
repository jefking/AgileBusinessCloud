// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='MessageDataTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Data
{
    using System;
    using Abc.Services;
    using Abc.Services.Contracts;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
   
    [TestClass]
    public class MessageDataTest
    {
        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new MessageData();
        }

        [TestMethod]
        public void ConstructorApplicationId()
        {
            new MessageData(Guid.NewGuid());
        }

        [TestMethod]
        public void IsLogData()
        {
            Assert.IsNotNull(new MessageData() as LogData);
        }

        [TestMethod]
        public void IsIConvertMessageDisplay()
        {
            Assert.IsNotNull(new MessageData() as IConvert<MessageDisplay>);
        }

        [TestMethod]
        public void SessionIdentifier()
        {
            var data = new MessageData();
            Assert.IsNull(data.SessionIdentifier);
            var session = Guid.NewGuid();
            data.SessionIdentifier = session;
            Assert.AreEqual<Guid?>(session, data.SessionIdentifier);
        }

        [TestMethod]
        public void Convert()
        {
            var identifier = Guid.NewGuid();
            var random = new Random();
            var data = new MessageData(Guid.NewGuid())
            {
                DeploymentId = StringHelper.ValidString(),
                MachineName = StringHelper.ValidString(),
                Message = StringHelper.ValidString(),
                OccurredOn = DateTime.UtcNow,
                RowKey = identifier.ToString(),
                SessionIdentifier = Guid.NewGuid(),
            };

            var converted = data.Convert();
            Assert.AreEqual<string>(data.DeploymentId, converted.DeploymentId);
            Assert.AreEqual<string>(data.MachineName, converted.MachineName);
            Assert.AreEqual<string>(data.Message, converted.Message);
            Assert.AreEqual<DateTime>(data.OccurredOn, converted.OccurredOn);
            Assert.AreEqual<Guid>(data.ApplicationId, converted.Token.ApplicationId);
            Assert.AreEqual<Guid>(identifier, converted.Identifier);
            Assert.AreEqual<Guid>(Guid.Parse(data.RowKey), converted.Identifier);
            Assert.AreEqual<Guid?>(data.SessionIdentifier, converted.SessionIdentifier);
        }
        #endregion
    }
}
