// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='CompanyRowTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Contracts
{
    using System;
    using Abc.Services.Data;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class CompanyRowTest
    {
        #region Error Cases
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ConstructorEmptyOwner()
        {
            new CompanyRow(Guid.Empty);
        }
        #endregion

        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new CompanyRow();
        }

        [TestMethod]
        public void ConstructorOwnerIdentifier()
        {
            new CompanyRow(Guid.NewGuid());
        }

        [TestMethod]
        public void OwnerIdentifier()
        {
            var data = Guid.NewGuid();
            var company = new CompanyRow(data);
            Assert.AreEqual<Guid>(data, company.OwnerIdentifier);
        }

        [TestMethod]
        public void Identifier()
        {
            var data = Guid.NewGuid();
            var company = new CompanyRow(Guid.NewGuid());
            company.RowKey = data.ToString();
            Assert.AreEqual<Guid>(data, company.Identifier);
        }

        [TestMethod]
        public void Name()
        {
            var company = new CompanyRow(Guid.NewGuid());
            var data = StringHelper.ValidString();
            company.Name = data;
            Assert.AreEqual<string>(data, company.Name);
        }

        [TestMethod]
        public void Active()
        {
            var company = new CompanyRow(Guid.NewGuid());
            company.Active = true;
            Assert.IsTrue(company.Active);
        }

        [TestMethod]
        public void Deleted()
        {
            var company = new CompanyRow(Guid.NewGuid());
            company.Deleted = true;
            Assert.IsTrue(company.Deleted);
        }

        [TestMethod]
        public void CreatedOn()
        {
            var company = new CompanyRow(Guid.NewGuid());
            var data = DateTime.UtcNow;
            company.CreatedOn = data;
            Assert.AreEqual<DateTime>(data, company.CreatedOn);
        }

        [TestMethod]
        public void CreatedByIdentifier()
        {
            var company = new CompanyRow(Guid.NewGuid());
            var data = Guid.NewGuid();
            company.CreatedByIdentifier = data;
            Assert.AreEqual<Guid>(data, company.CreatedByIdentifier);
        }

        [TestMethod]
        public void EditedOn()
        {
            var company = new CompanyRow(Guid.NewGuid());
            var data = DateTime.UtcNow;
            company.EditedOn = data;
            Assert.AreEqual<DateTime>(data, company.EditedOn);
        }

        [TestMethod]
        public void EditedByIdentifier()
        {
            var company = new CompanyRow(Guid.NewGuid());
            var data = Guid.NewGuid();
            company.EditedByIdentifier = data;
            Assert.AreEqual<Guid>(data, company.EditedByIdentifier);
        }

        [TestMethod]
        public void Convert()
        {
            var company = new CompanyRow(Guid.NewGuid())
            {
                Active = true,
                CreatedByIdentifier = Guid.NewGuid(),
                CreatedOn = DateTime.UtcNow,
                Deleted = true,
                EditedByIdentifier = Guid.NewGuid(),
                EditedOn = DateTime.UtcNow,
                Name = StringHelper.ValidString(),
            };

            var converted = company.Convert();
            Assert.AreEqual<Guid>(company.EditedByIdentifier, converted.EditedBy.Identifier);
            Assert.AreEqual<Guid>(company.CreatedByIdentifier, converted.CreatedBy.Identifier);
            Assert.AreEqual<Guid>(company.Identifier, converted.Identifier);
            Assert.AreEqual<bool>(company.Active, converted.Active);
            Assert.AreEqual<bool>(company.Deleted, converted.Deleted);
            Assert.AreEqual<string>(company.Name, converted.Name);
            Assert.AreEqual<DateTime>(company.CreatedOn, converted.CreatedOn);
            Assert.AreEqual<DateTime>(company.EditedOn, converted.EditedOn);
        }
        #endregion
    }
}