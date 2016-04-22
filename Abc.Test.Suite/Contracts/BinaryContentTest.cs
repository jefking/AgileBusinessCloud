// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='BinaryContentTest.cs'>
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
    public class BinaryContentTest
    {
        #region Valid Cases
        [TestMethod]
        public void Content()
        {
            var random = new Random();
            var content = new BinaryContent();
            var data = new byte[1024];
            random.NextBytes(data);
            content.Content = data;
            Assert.AreEqual<byte[]>(data, content.Content);
        }

        [TestMethod]
        public void InvalidNoIdNoContent()
        {
            var content = new BinaryContent()
            {
                Content = null,
                Id = Guid.Empty,
            };

            var validator = new Validator<BinaryContent>();
            Assert.IsFalse(validator.IsValid(content));
        }

        [TestMethod]
        public void InvalidNoId()
        {
            var random = new Random();
            var data = new byte[1024];
            random.NextBytes(data);
            var content = new BinaryContent()
            {
                Content = data,
                Id = Guid.Empty,
            };

            var validator = new Validator<BinaryContent>();
            Assert.IsTrue(validator.IsValid(content));
        }

        [TestMethod]
        public void InvalidNoContent()
        {
            var content = new BinaryContent()
            {
                Content = null,
                Id = Guid.NewGuid(),
            };

            var validator = new Validator<BinaryContent>();
            Assert.IsTrue(validator.IsValid(content));
        }

        [TestMethod]
        public void Valid()
        {
            var random = new Random();
            var data = new byte[1024];
            random.NextBytes(data);
            var content = new BinaryContent()
            {
                Content = data,
                Id = Guid.NewGuid(),
            };

            var validator = new Validator<BinaryContent>();
            Assert.IsTrue(validator.IsValid(content));
        }
        #endregion
    }
}
