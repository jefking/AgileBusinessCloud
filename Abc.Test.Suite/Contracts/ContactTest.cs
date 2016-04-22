// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='ContactTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Contracts
{
    using System;
    using Abc.Services.Contracts;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ContactTest
    {
        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new Contact();
        }

        [TestMethod]
        public void Identifier()
        {
            var contact = new Contact();
            var data = Guid.NewGuid();
            contact.Identifier = data;
            Assert.AreEqual<Guid>(data, contact.Identifier);
        }

        [TestMethod]
        public void Owner()
        {
            var contact = new Contact();
            var data = new User();
            contact.Owner = data;
            Assert.AreEqual<User>(data, contact.Owner);
        }

        [TestMethod]
        public void Email()
        {
            var contact = new Contact();
            var data = StringHelper.ValidString();
            contact.Email = data;
            Assert.AreEqual<string>(data, contact.Email);
        }

        [TestMethod]
        public void Convert()
        {
            var owner = new User()
            {
                Identifier = Guid.NewGuid(),
            };

            var contact = new Contact()
            {
                Identifier = Guid.NewGuid(),
                Email = StringHelper.ValidString(),
                Owner = owner,
            };

            var converted = contact.Convert();
            Assert.AreEqual<string>(contact.Email, converted.Email);
            Assert.AreEqual<Guid>(contact.Owner.Identifier, converted.OwnerIdentifier);
            Assert.AreEqual<Guid>(contact.Identifier, converted.Identifier);
        }
        #endregion
    }
}