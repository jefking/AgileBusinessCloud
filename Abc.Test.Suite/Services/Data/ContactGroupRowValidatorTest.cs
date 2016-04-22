// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='ContactGroupRowValidatorTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Data
{
    using System;
    using Abc.Services.Data;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ContactGroupRowValidatorTest
    {
        #region Valid Cases
        [TestMethod]
        public void Valid()
        {
            var validator = new ContactGroupRowValidator();
            Assert.IsTrue(validator.ValidateForAddOrUpdate(this.Group()));
        }

        [TestMethod]
        public void NullEntity()
        {
            var validator = new ContactRowValidator();
            Assert.IsFalse(validator.ValidateForAddOrUpdate(null));
        }

        public void InvalidIdentifier()
        {
            var validator = new ContactGroupRowValidator();
            var data = this.Group();
            data.RowKey = StringHelper.NullEmptyWhiteSpace();
            Assert.IsFalse(validator.ValidateForAddOrUpdate(data));
        }

        [TestMethod]
        public void InvalidOwnerIdentifier()
        {
            var validator = new ContactGroupRowValidator();
            var data = this.Group();
            data.PartitionKey = StringHelper.NullEmptyWhiteSpace();
            Assert.IsFalse(validator.ValidateForAddOrUpdate(data));
        }

        [TestMethod]
        public void InvalidName()
        {
            var validator = new ContactGroupRowValidator();
            var data = this.Group();
            data.Name = StringHelper.NullEmptyWhiteSpace();
            Assert.IsFalse(validator.ValidateForAdd(data));
        }
        #endregion

        #region Helper Methods
        private ContactGroupRow Group()
        {
            return new ContactGroupRow(Guid.NewGuid(), Guid.NewGuid())
            {
                Name = StringHelper.ValidString(),
            };
        }
        #endregion
    }
}