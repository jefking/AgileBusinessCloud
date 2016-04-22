// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='ContactRowTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Contracts
{
    using System;
    using Abc.Services.Data;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ContactRowTest
    {
        #region Error Cases
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ConstructorEmptyOwnerIdentifier()
        {
            new ContactRow(Guid.Empty, Guid.NewGuid());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ConstructorEmptyIdentifier()
        {
            new ContactRow(Guid.NewGuid(), Guid.Empty);
        }
        #endregion

        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new ContactRow();
        }

        [TestMethod]
        public void ConstructorIdentifiers()
        {
            new ContactRow(Guid.NewGuid(), Guid.NewGuid());
        }

        [TestMethod]
        public void Identifier()
        {
            var data = Guid.NewGuid();
            var contact = new ContactRow(Guid.NewGuid(), data);
            Assert.AreEqual<Guid>(data, contact.Identifier);
        }

        [TestMethod]
        public void OwnerIdentifier()
        {
            var data = Guid.NewGuid();
            var contact = new ContactRow(data, Guid.NewGuid());
            Assert.AreEqual<Guid>(data, contact.OwnerIdentifier);
        }

        [TestMethod]
        public void Email()
        {
            var data = StringHelper.ValidString();
            var contact = new ContactRow(Guid.NewGuid(), Guid.NewGuid());
            contact.Email = data;
            Assert.AreEqual<string>(data, contact.Email);
        }

        [TestMethod]
        public void Convert()
        {
            var contact = new ContactRow(Guid.NewGuid(), Guid.NewGuid())
            {
                Email = StringHelper.ValidString(),
            };

            var converted = contact.Convert();
            Assert.AreEqual<string>(contact.Email, converted.Email);
            Assert.AreEqual<Guid>(contact.Identifier, converted.Identifier);
            Assert.AreEqual<Guid>(contact.OwnerIdentifier, converted.Owner.Identifier);
        }
        #endregion
    }
}