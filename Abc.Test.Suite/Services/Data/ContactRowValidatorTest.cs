// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='ContactRowValidatorTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Data
{
    using System;
    using Abc.Services.Data;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ContactRowValidatorTest
    {
        #region Valid Cases
        [TestMethod]
        public void Valid()
        {
            var validator = new ContactRowValidator();
            Assert.IsTrue(validator.ValidateForAddOrUpdate(this.Contact()));
        }

        [TestMethod]
        public void NullEntity()
        {
            var validator = new ContactRowValidator();
            Assert.IsFalse(validator.ValidateForAddOrUpdate(null));
        }

        public void InvalidIdentifier()
        {
            var validator = new ContactRowValidator();
            var data = this.Contact();
            data.RowKey = StringHelper.NullEmptyWhiteSpace();
            Assert.IsFalse(validator.ValidateForAddOrUpdate(data));
        }

        [TestMethod]
        public void InvalidOwnerIdentifier()
        {
            var validator = new ContactRowValidator();
            var data = this.Contact();
            data.PartitionKey = StringHelper.NullEmptyWhiteSpace();
            Assert.IsFalse(validator.ValidateForAddOrUpdate(data));
        }

        [TestMethod]
        public void InvalidEmail()
        {
            var validator = new ContactRowValidator();
            var data = this.Contact();
            data.Email = StringHelper.NullEmptyWhiteSpace();
            Assert.IsFalse(validator.ValidateForAddOrUpdate(data));
        }
        #endregion

        #region Helper Methods
        private ContactRow Contact()
        {
            return new ContactRow(Guid.NewGuid(), Guid.NewGuid())
            {
                Email = StringHelper.ValidString(),
            };
        }
        #endregion
    }
}