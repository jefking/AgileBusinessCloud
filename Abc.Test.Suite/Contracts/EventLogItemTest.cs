// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='EventLogItemTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Contracts
{
    using System;
    using Abc.Services.Contracts;
    using Abc.Services.Validation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class EventLogItemTest
    {
        #region Helpers
        private EventLogItem Item
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

        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new EventLogItem();
        }

        [TestMethod]
        public void Source()
        {
            var item = new EventLogItem();
            var data = StringHelper.ValidString();
            item.Source = data;
            Assert.AreEqual<string>(data, item.Source);
        }

        [TestMethod]
        public void User()
        {
            var item = new EventLogItem();
            var data = StringHelper.ValidString();
            item.User = data;
            Assert.AreEqual<string>(data, item.User);
        }

        [TestMethod]
        public void EventId()
        {
            var random = new Random();
            var item = new EventLogItem();
            var data = random.Next();
            item.EventId = data;
            Assert.AreEqual<int>(data, item.EventId);
        }

        [TestMethod]
        public void InstanceId()
        {
            var random = new Random();
            var item = new EventLogItem();
            var data = random.Next();
            item.InstanceId = data;
            Assert.AreEqual<long>(data, item.InstanceId);
        }

        [TestMethod]
        public void EntryType()
        {
            var random = new Random();
            var item = new EventLogItem();
            Assert.AreEqual<EventLogEntryType>(EventLogEntryType.Unknown, item.EntryType);
            var data = EventLogEntryType.Information;
            item.EntryType = data;
            Assert.AreEqual<EventLogEntryType>(data, item.EntryType);
        }

        [TestMethod]
        public void Convert()
        {
            var item = this.Item;

            var data = item.Convert();

            Assert.AreEqual<string>(item.Source, data.Source);
            Assert.AreEqual<string>(item.MachineName, data.MachineName);
            Assert.AreEqual<string>(item.Message, data.Message);
            Assert.AreEqual<DateTime>(item.OccurredOn, data.OccurredOn);
            Assert.AreEqual<Guid>(item.Token.ApplicationId, data.ApplicationId);
            Assert.AreEqual<string>(item.DeploymentId, data.DeploymentId);
            Assert.AreEqual<string>(item.User, data.User);
            Assert.AreEqual<int?>(item.EventId, data.EventId);
            Assert.AreEqual<long?>(item.InstanceId, data.InstanceId);
            Assert.AreEqual<int>((int)item.EntryType, data.EntryTypeValue);
        }
        #endregion

        #region Validation
        [TestMethod]
        public void Valid()
        {
            var item = this.Item;
            var validator = new Validator<EventLogItem>();
            Assert.IsTrue(validator.IsValid(item));
        }

        [TestMethod]
        public void RulesMessageIsNotValid()
        {
            var validator = new Validator<EventLogItem>();
            var item = this.Item;
            item.Message = StringHelper.LongerThanMaximumRowLength();
            Assert.IsFalse(validator.IsValid(item));
        }

        [TestMethod]
        public void RulesMachineNameIsNotValid()
        {
            var validator = new Validator<EventLogItem>();
            var item = this.Item;
            item.MachineName = StringHelper.NullEmptyWhiteSpace();
            Assert.IsFalse(validator.IsValid(item));
        }

        [TestMethod]
        public void RulesMachineNameTooLong()
        {
            var validator = new Validator<EventLogItem>();
            var item = this.Item;
            item.MachineName = StringHelper.LongerThanMaximumRowLength();
            Assert.IsFalse(validator.IsValid(item));
        }

        [TestMethod]
        public void RulesDeploymentIdIsNotValid()
        {
            var validator = new Validator<EventLogItem>();
            var item = this.Item;
            item.DeploymentId = StringHelper.LongerThanMaximumRowLength();
            Assert.IsFalse(validator.IsValid(item));
        }

        [TestMethod]
        public void RulesUserIsTooLong()
        {
            var validator = new Validator<EventLogItem>();
            var item = this.Item;
            item.User = StringHelper.LongerThanMaximumRowLength();
            Assert.IsFalse(validator.IsValid(item));
        }

        [TestMethod]
        public void RulesEventIdIsNotValid()
        {
            var random = new Random();
            var validator = new Validator<EventLogItem>();
            var item = this.Item;
            item.EventId = random.Next() * -1;
            Assert.IsFalse(validator.IsValid(item));
        }

        [TestMethod]
        public void RulesInstanceIdIsNotValid()
        {
            var random = new Random();
            var validator = new Validator<EventLogItem>();
            var item = this.Item;
            item.InstanceId = random.Next() * -1;
            Assert.IsFalse(validator.IsValid(item));
        }

        [TestMethod]
        public void RulesEntryTypeIsNotValid()
        {
            var random = new Random();
            var validator = new Validator<EventLogItem>();
            var item = this.Item;
            item.EntryType = (EventLogEntryType)(random.Next() * -1);
            Assert.IsFalse(validator.IsValid(item));
        }
        #endregion
    }
}