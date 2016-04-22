// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='OccurrenceTest.cs'>
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
    public class OccurrenceTest
    {
        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new Occurrence();
        }

        [TestMethod]
        public void Class()
        {
            var occurrence = new Occurrence();
            var data = StringHelper.ValidString();
            occurrence.Class = data;
            Assert.AreEqual<string>(data, occurrence.Class);
        }

        [TestMethod]
        public void Duration()
        {
            var random = new Random();
            var occurrence = new Occurrence();
            var data = TimeSpan.FromMilliseconds(random.NextDouble());
            occurrence.Duration = data;
            Assert.AreEqual<TimeSpan>(data, occurrence.Duration);
        }

        [TestMethod]
        public void SessionIdentifier()
        {
            var occurrence = new Occurrence();
            Assert.IsNull(occurrence.SessionIdentifier);
            var data = Guid.NewGuid();
            occurrence.SessionIdentifier = data;
            Assert.AreEqual<Guid?>(data, occurrence.SessionIdentifier);
        }

        [TestMethod]
        public void Method()
        {
            var occurrence = new Occurrence();
            var data = StringHelper.ValidString();
            occurrence.Method = data;
            Assert.AreEqual<string>(data, occurrence.Method);
        }

        [TestMethod]
        public void ThreadId()
        {
            var random = new Random();
            var occurrence = new Occurrence();
            var data = random.Next();
            occurrence.ThreadId = data;
            Assert.AreEqual<int>(data, occurrence.ThreadId);
        }

        [TestMethod]
        public void Convert()
        {
            var random = new Random();
            var token = new Token()
            {
                ApplicationId = Guid.NewGuid(),
            };
            var occurrence = new Occurrence()
            {
                Token = token,
                OccurredOn = DateTime.UtcNow,
                MachineName = Environment.MachineName,
                DeploymentId = StringHelper.ValidString(),
                Message = StringHelper.ValidString(),
                Class = StringHelper.ValidString(),
                Duration = TimeSpan.FromMilliseconds(random.NextDouble()),
                Method = StringHelper.ValidString(),
                ThreadId = random.Next(),
                SessionIdentifier = Guid.NewGuid(),
            };
            var converted = occurrence.Convert();
            Assert.AreEqual<Guid>(token.ApplicationId, converted.ApplicationId);
            Assert.AreEqual<DateTime>(occurrence.OccurredOn, converted.OccurredOn);
            Assert.AreEqual<string>(occurrence.MachineName, converted.MachineName);
            Assert.AreEqual<string>(occurrence.DeploymentId, converted.DeploymentId);
            Assert.AreEqual<string>(occurrence.Message, converted.Message);
            Assert.AreEqual<string>(occurrence.Class, converted.ClassName);
            Assert.AreEqual<string>(occurrence.Method, converted.MethodName);
            Assert.AreEqual<long>(occurrence.Duration.Ticks, converted.Duration);
            Assert.AreEqual<int>(occurrence.ThreadId, converted.ThreadId);
            Assert.AreEqual<Guid?>(occurrence.SessionIdentifier, converted.SessionIdentifier);
        }

        [TestMethod]
        public void Valid()
        {
            var random = new Random();
            var occurrence = new Occurrence()
            {
                Method = StringHelper.ValidString(),
                Class = StringHelper.ValidString(),
                Duration = TimeSpan.FromMilliseconds(random.Next()),
                ThreadId = random.Next(),
                SessionIdentifier = null,
            };

            var validator = new Validator<Occurrence>();
            Assert.IsTrue(validator.IsValid(occurrence));
        }

        [TestMethod]
        public void ValidSessionIdentifier()
        {
            var random = new Random();
            var occurrence = new Occurrence()
            {
                Method = StringHelper.ValidString(),
                Class = StringHelper.ValidString(),
                Duration = TimeSpan.FromMilliseconds(random.Next()),
                ThreadId = random.Next(),
                SessionIdentifier = Guid.NewGuid(),
            };

            var validator = new Validator<Occurrence>();
            Assert.IsTrue(validator.IsValid(occurrence));
        }

        [TestMethod]
        public void InvalidSessionIdentifier()
        {
            var random = new Random();
            var occurrence = new Occurrence()
            {
                Method = StringHelper.ValidString(),
                Class = StringHelper.ValidString(),
                Duration = TimeSpan.FromMilliseconds(random.Next()),
                ThreadId = random.Next(),
                SessionIdentifier = Guid.Empty,
            };

            var validator = new Validator<Occurrence>();
            Assert.IsFalse(validator.IsValid(occurrence));
        }

        [TestMethod]
        public void MessageNotSpecified()
        {
            var random = new Random();
            var occurrence = new Occurrence()
            {
                Method = StringHelper.NullEmptyWhiteSpace(),
                Class = StringHelper.ValidString(),
                Duration = TimeSpan.FromMilliseconds(random.Next()),
                ThreadId = random.Next(),
            };

            var validator = new Validator<Occurrence>();
            Assert.IsFalse(validator.IsValid(occurrence));
        }

        [TestMethod]
        public void MethodTooLong()
        {
            var random = new Random();
            var occurrence = new Occurrence()
            {
                Method = StringHelper.LongerThanMaximumRowLength(),
                Class = StringHelper.ValidString(),
                Duration = TimeSpan.FromMilliseconds(random.Next()),
                ThreadId = random.Next(),
            };

            var validator = new Validator<Occurrence>();
            Assert.IsFalse(validator.IsValid(occurrence));
        }

        [TestMethod]
        public void ClassNotSpecified()
        {
            var random = new Random();
            var occurrence = new Occurrence()
            {
                Method = StringHelper.ValidString(),
                Class = StringHelper.NullEmptyWhiteSpace(),
                Duration = TimeSpan.FromMilliseconds(random.Next()),
                ThreadId = random.Next(),
            };

            var validator = new Validator<Occurrence>();
            Assert.IsFalse(validator.IsValid(occurrence));
        }

        [TestMethod]
        public void ClassTooLong()
        {
            var random = new Random();
            var occurrence = new Occurrence()
            {
                Method = StringHelper.ValidString(),
                Class = StringHelper.LongerThanMaximumRowLength(),
                Duration = TimeSpan.FromMilliseconds(random.Next()),
                ThreadId = random.Next(),
            };

            var validator = new Validator<Occurrence>();
            Assert.IsFalse(validator.IsValid(occurrence));
        }

        [TestMethod]
        public void DurationTooShort()
        {
            var random = new Random();
            var occurrence = new Occurrence()
            {
                Method = StringHelper.ValidString(),
                Class = StringHelper.ValidString(),
                Duration = TimeSpan.Zero,
                ThreadId = random.Next(),
            };

            var validator = new Validator<Occurrence>();
            Assert.IsFalse(validator.IsValid(occurrence));
        }

        [TestMethod]
        public void ThreadIdInvalid()
        {
            var random = new Random();
            var occurrence = new Occurrence()
            {
                Method = StringHelper.ValidString(),
                Class = StringHelper.ValidString(),
                Duration = TimeSpan.FromMilliseconds(random.Next()),
                ThreadId = random.Next() * -1,
            };

            var validator = new Validator<Occurrence>();
            Assert.IsFalse(validator.IsValid(occurrence));
        }
        #endregion
    }
}
