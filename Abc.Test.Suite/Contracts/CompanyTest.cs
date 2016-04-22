// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='CompanyTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Contracts
{
    using System;
    using Abc.Services.Contracts;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class CompanyTest
    {
        #region Methods
        [TestMethod]
        public void Constructor()
        {
            new Company();
        }

        [TestMethod]
        public void Owner()
        {
            var data = new User()
            {
                Identifier = Guid.NewGuid(),
            };
            var company = new Company();
            company.Owner = data;
            Assert.AreEqual<User>(data, company.Owner);
        }

        [TestMethod]
        public void Name()
        {
            var data = StringHelper.ValidString();
            var company = new Company();
            company.Name = data;
            Assert.AreEqual<string>(data, company.Name);
        }

        [TestMethod]
        public void Active()
        {
            var data = true;
            var company = new Company();
            company.Active = data;
            Assert.AreEqual<bool>(data, company.Active);
        }

        [TestMethod]
        public void Deleted()
        {
            var data = true;
            var company = new Company();
            company.Deleted = data;
            Assert.AreEqual<bool>(data, company.Deleted);
        }

        [TestMethod]
        public void EditedOn()
        {
            var data = DateTime.UtcNow;
            var company = new Company();
            company.EditedOn = data;
            Assert.AreEqual<DateTime>(data, company.EditedOn);
        }

        [TestMethod]
        public void EditedBy()
        {
            var data = new User()
            {
                Identifier = Guid.NewGuid(),
            };
            var company = new Company();
            company.EditedBy = data;
            Assert.AreEqual<User>(data, company.EditedBy);
        }

        [TestMethod]
        public void Identifier()
        {
            var data = Guid.NewGuid();
            var company = new Company();
            company.Identifier = data;
            Assert.AreEqual<Guid>(data, company.Identifier);
        }

        [TestMethod]
        public void CreatedOn()
        {
            var data = DateTime.UtcNow;
            var company = new Company();
            company.CreatedOn = data;
            Assert.AreEqual<DateTime>(data, company.CreatedOn);
        }

        [TestMethod]
        public void CreatedBy()
        {
            var data = new User()
            {
                Identifier = Guid.NewGuid(),
            };
            var company = new Company();
            company.CreatedBy = data;
            Assert.AreEqual<User>(data, company.CreatedBy);
        }

        [TestMethod]
        public void Convert()
        {
            var creator = new User()
            {
                Identifier = Guid.NewGuid(),
            };
            var editor = new User()
            {
                Identifier = Guid.NewGuid(),
            };
            var owner = new User()
            {
                Identifier = Guid.NewGuid(),
            };
            var company = new Company()
            {
                Active = true,
                CreatedBy = creator,
                CreatedOn = DateTime.UtcNow,
                Deleted = true,
                EditedBy = editor,
                EditedOn = DateTime.UtcNow,
                Name = StringHelper.ValidString(),
                Owner = owner,
                Identifier = Guid.NewGuid(),
            };

            var data = company.Convert();
            Assert.AreEqual<bool>(company.Active, data.Active);
            Assert.AreEqual<Guid>(company.CreatedBy.Identifier, data.CreatedByIdentifier);
            Assert.AreEqual<DateTime>(company.CreatedOn, data.CreatedOn);
            Assert.AreEqual<bool>(company.Deleted, data.Deleted);
            Assert.AreEqual<Guid>(company.EditedBy.Identifier, data.EditedByIdentifier);
            Assert.AreEqual<DateTime>(company.EditedOn, data.EditedOn);
            Assert.AreEqual<string>(company.Name, data.Name);
            Assert.AreEqual<Guid>(company.Owner.Identifier, data.OwnerIdentifier);
            Assert.AreEqual<Guid>(company.Identifier, data.Identifier);
        }
        #endregion
    }
}