// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='TokenTest.cs'>
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
    public class TokenTest
    {
        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new Token();
        }

        [TestMethod]
        public void ApplicationId()
        {
            var token = new Token();
            var data = Guid.NewGuid();
            token.ApplicationId = data;
            Assert.AreEqual<Guid>(data, token.ApplicationId);
        }

        [TestMethod]
        public void ValidationKey()
        {
            var token = new Token();
            var data = StringHelper.ValidString();
            token.ValidationKey = data;
            Assert.AreEqual<string>(data, token.ValidationKey);
        }

        [TestMethod]
        public void Valid()
        {
            var token = new Token()
            {
                ValidationKey = StringHelper.ValidString(),
                ApplicationId = Guid.NewGuid(),
            };

            var validator = new Validator<Token>();
            Assert.IsTrue(validator.IsValid(token));
        }

        [TestMethod]
        public void InvalidApplicationId()
        {
            var token = new Token()
            {
                ValidationKey = StringHelper.ValidString(),
                ApplicationId = Guid.Empty,
            };

            var validator = new Validator<Token>();
            Assert.IsFalse(validator.IsValid(token));
        }

        [TestMethod]
        public void InvalidValidationKey()
        {
            var token = new Token()
            {
                ValidationKey = StringHelper.NullEmptyWhiteSpace(),
                ApplicationId = Guid.NewGuid(),
            };

            var validator = new Validator<Token>();
            Assert.IsFalse(validator.IsValid(token));
        }
        #endregion
    }
}