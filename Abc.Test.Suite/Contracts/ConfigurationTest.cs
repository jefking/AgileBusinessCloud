// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='ConfigurationTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Contracts
{
    using System;
    using Abc.Services.Contracts;
    using Abc.Services.Validation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Configuration Test
    /// </summary>
    [TestClass]
    public class ConfigurationTest
    {
        #region Error Cases
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ConvertNullToken()
        {
            var c = new Configuration();
            c.Convert();
        }
        #endregion

        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new Configuration();
        }

        [TestMethod]
        public void Key()
        {
            var key = StringHelper.ValidString();
            var c = new Configuration();
            c.Key = key;

            Assert.AreEqual<string>(key, c.Key);
        }

        [TestMethod]
        public void Value()
        {
            var value = StringHelper.ValidString();
            var c = new Configuration();
            c.Value = value;

            Assert.AreEqual<string>(value, c.Value);
        }

        [TestMethod]
        public void Convert()
        {
            var token = new Token()
            {
                ApplicationId = Guid.NewGuid(),
                ValidationKey = StringHelper.ValidString(),
            };
            var c = new Configuration()
            {
                Token = token,
                Key = StringHelper.ValidString(),
                Value = StringHelper.ValidString(),
            };
            var ac = c.Convert();

            Assert.AreEqual<Guid>(token.ApplicationId, ac.ApplicationId);
            Assert.AreEqual<string>(c.Value, ac.Value);
            Assert.AreEqual<string>(c.Key, ac.RowKey);
        }

        [TestMethod]
        public void Valid()
        {
            var config = new Configuration()
            {
                Key = StringHelper.ValidString(),
                Value = StringHelper.ValidString(),
            };

            var validator = new Validator<Configuration>();
            Assert.IsTrue(validator.IsValid(config));
        }

        [TestMethod]
        public void KeyTooLong()
        {
            var config = new Configuration()
            {
                Key = StringHelper.LongerThanMaximumRowLength(),
                Value = StringHelper.ValidString(),
            };

            var validator = new Validator<Configuration>();
            Assert.IsFalse(validator.IsValid(config));
        }

        [TestMethod]
        public void KeyNotSpecified()
        {
            var config = new Configuration()
            {
                Key = StringHelper.NullEmptyWhiteSpace(),
                Value = StringHelper.ValidString(),
            };

            var validator = new Validator<Configuration>();
            Assert.IsTrue(validator.IsValid(config));
        }
        #endregion
    }
}