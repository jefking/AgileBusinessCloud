// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='XmlContentTest.cs'>
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
    public class XmlContentTest
    {
        #region Valid Cases
        [TestMethod]
        public void Content()
        {
            var xml = new XmlContent();
            var data = StringHelper.ValidString();
            xml.Content = data;
            Assert.AreEqual<string>(data, xml.Content);
        }

        [TestMethod]
        public void InvalidNoIdNoContent()
        {
            var xml = new XmlContent()
            {
                Content = StringHelper.NullEmptyWhiteSpace(),
                Id = Guid.Empty,
            };

            var validator = new Validator<XmlContent>();
            Assert.IsFalse(validator.IsValid(xml));
        }

        [TestMethod]
        public void InvalidNoId()
        {
            var xml = new XmlContent()
            {
                Content = "<testnode />",
                Id = Guid.Empty,
            };

            var validator = new Validator<XmlContent>();
            Assert.IsTrue(validator.IsValid(xml));
        }

        [TestMethod]
        public void InvalidNoContent()
        {
            var xml = new XmlContent()
            {
                Content = null,
                Id = Guid.NewGuid(),
            };

            var validator = new Validator<XmlContent>();
            Assert.IsTrue(validator.IsValid(xml));
        }

        [TestMethod]
        public void InvalidXml()
        {
            var xml = new XmlContent()
            {
                Content = StringHelper.ValidString(),
                Id = Guid.Empty,
            };

            var validator = new Validator<XmlContent>();
            Assert.IsFalse(validator.IsValid(xml));
        }

        [TestMethod]
        public void Valid()
        {
            var xml = new XmlContent()
            {
                Content = "<testnode>this is an xml node.</testnode>",
                Id = Guid.NewGuid(),
            };

            var validator = new Validator<XmlContent>();
            Assert.IsTrue(validator.IsValid(xml));
        }

        [TestMethod]
        public void IsValid()
        {
            var xml = new XmlContent()
            {
                Content = "<testnode />",
            };

            Assert.IsTrue(xml.IsValid());
        }

        [TestMethod]
        public void Convert()
        {
            var token = new Token()
            {
                ApplicationId = Guid.NewGuid(),
            };

            var xml = new XmlContent()
            {
                Active = false,
                Deleted = true,
                Token = token,
            };

            var converted = xml.Convert();
            Assert.AreEqual<DateTime>(DateTime.UtcNow.Date, converted.CreatedOn.Date);
            Assert.AreEqual<DateTime>(DateTime.UtcNow.Date, converted.UpdatedOn.Date);
            Assert.AreEqual<Guid>(token.ApplicationId, converted.ApplicationId);
            Assert.IsFalse(xml.Active);
            Assert.IsTrue(xml.Deleted);
        }
        #endregion
    }
}