// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='ContactGroupRowTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Contracts
{
    using System;
    using Abc.Services.Data;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ContactGroupRowTest
    {
        #region Error Cases
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ConstructorEmptyOwnerIdentifier()
        {
            new ContactGroupRow(Guid.Empty, Guid.NewGuid());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ConstructorEmptyIdentifier()
        {
            new ContactGroupRow(Guid.NewGuid(), Guid.Empty);
        }
        #endregion

        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new ContactGroupRow();
        }

        [TestMethod]
        public void ConstructorIdentifiers()
        {
            new ContactGroupRow(Guid.NewGuid(), Guid.NewGuid());
        }

        [TestMethod]
        public void Identifier()
        {
            var data = Guid.NewGuid();
            var contact = new ContactGroupRow(Guid.NewGuid(), data);
            Assert.AreEqual<Guid>(data, contact.Identifier);
        }

        [TestMethod]
        public void OwnerIdentifier()
        {
            var data = Guid.NewGuid();
            var contact = new ContactGroupRow(data, Guid.NewGuid());
            Assert.AreEqual<Guid>(data, contact.OwnerIdentifier);
        }

        [TestMethod]
        public void Name()
        {
            var data = StringHelper.ValidString();
            var contact = new ContactGroupRow(Guid.NewGuid(), Guid.NewGuid());
            contact.Name = data;
            Assert.AreEqual<string>(data, contact.Name);
        }

        [TestMethod]
        public void Convert()
        {
            var contact = new ContactGroupRow(Guid.NewGuid(), Guid.NewGuid())
            {
                Name = StringHelper.ValidString(),
            };

            var converted = contact.Convert();
            Assert.AreEqual<string>(contact.Name, converted.Name);
            Assert.AreEqual<Guid>(contact.Identifier, converted.Identifier);
            Assert.AreEqual<Guid>(contact.OwnerIdentifier, converted.Owner.Identifier);
        }
        #endregion
    }
}