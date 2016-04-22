// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='TextContentTest.cs'>
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
    public class TextContentTest
    {
        #region Valid Cases
        [TestMethod]
        public void Content()
        {
            var content = new TextContent();
            var data = StringHelper.ValidString();
            content.Content = data;
            Assert.AreEqual<string>(data, content.Content);
        }

        [TestMethod]
        public void InvalidNoIdNoContent()
        {
            var content = new TextContent()
            {
                Content = StringHelper.NullEmptyWhiteSpace(),
                Id = Guid.Empty,
            };

            var validator = new Validator<TextContent>();
            Assert.IsFalse(validator.IsValid(content));
        }

        [TestMethod]
        public void ValidNoId()
        {
            var content = new TextContent()
            {
                Content = StringHelper.ValidString(),
                Id = Guid.Empty,
            };

            var validator = new Validator<TextContent>();
            Assert.IsTrue(validator.IsValid(content));
        }

        [TestMethod]
        public void ValidNoContent()
        {
            var content = new TextContent()
            {
                Content = null,
                Id = Guid.NewGuid(),
            };

            var validator = new Validator<TextContent>();
            Assert.IsTrue(validator.IsValid(content));
        }

        [TestMethod]
        public void Valid()
        {
            var content = new TextContent()
            {
                Content = StringHelper.ValidString(),
                Id = Guid.NewGuid(),
            };

            var validator = new Validator<TextContent>();
            Assert.IsTrue(validator.IsValid(content));
        }

        [TestMethod]
        public void Convert()
        {
            var token = new Token()
            {
                ApplicationId = Guid.NewGuid(), 
            };

            var content = new TextContent()
            {
                Active = false,
                Deleted = true,
                Token = token,
            };

            var converted = content.Convert();
            Assert.AreEqual<DateTime>(DateTime.UtcNow.Date, converted.CreatedOn.Date);
            Assert.AreEqual<DateTime>(DateTime.UtcNow.Date, converted.UpdatedOn.Date);
            Assert.AreEqual<Guid>(token.ApplicationId, converted.ApplicationId);
            Assert.IsFalse(content.Active);
            Assert.IsTrue(content.Deleted);
        }
        #endregion
    }
}