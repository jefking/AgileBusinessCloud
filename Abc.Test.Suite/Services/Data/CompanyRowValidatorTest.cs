// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='CompanyRowValidatorTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Data
{
    using System;
    using Abc.Services.Data;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class CompanyRowValidatorTest
    {
        #region Error Cases
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullEntity()
        {
            var validator = new CompanyRowValidator();
            validator.ValidateForAddOrUpdate(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CreatedByIdentifierEmpty()
        {
            var data = this.Company();
            data.CreatedByIdentifier = Guid.Empty;
            var validator = new CompanyRowValidator();
            validator.ValidateForAddOrUpdate(data);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void EditedByIdentifierEmpty()
        {
            var data = this.Company();
            data.EditedByIdentifier = Guid.Empty;
            var validator = new CompanyRowValidator();
            validator.ValidateForAddOrUpdate(data);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void NameInvalid()
        {
            var data = this.Company();
            data.Name = StringHelper.NullEmptyWhiteSpace();
            var validator = new CompanyRowValidator();
            validator.ValidateForAddOrUpdate(data);
        }
        #endregion

        #region Valid Cases
        public void Valid()
        {
            var validator = new CompanyRowValidator();
            Assert.IsTrue(validator.ValidateForAdd(this.Company()));
        }
        #endregion

        #region Valid Cases
        public CompanyRow Company()
        {
            return new CompanyRow(Guid.NewGuid())
            {
                EditedByIdentifier = Guid.NewGuid(),
                Name = StringHelper.ValidString(),
                CreatedByIdentifier = Guid.NewGuid(),
            };
        }
        #endregion
    }
}
