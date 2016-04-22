// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='RoleRowValidatorTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Data
{
    using System;
    using Abc.Services.Data;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class RoleRowValidatorTest
    {
        #region Invalid Cases
        [TestMethod]
        public void ValidateNullEntity()
        {
            var validator = new RoleRowValidator();
            Assert.IsFalse(validator.ValidateForAdd(null));
        }

        [TestMethod]
        public void ValidateEmptyUserId()
        {
            var validator = new RoleRowValidator();
            var role = Role();
            role.UserIdentifier = Guid.Empty;
            Assert.IsFalse(validator.ValidateForAdd(role));
        }

        [TestMethod]
        public void ValidateInvalidName()
        {
            var validator = new RoleRowValidator();
            var role = Role();
            role.Name = StringHelper.NullEmptyWhiteSpace();
            Assert.IsFalse(validator.ValidateForAdd(role));
        }
        #endregion

        #region Valid Cases
        [TestMethod]
        public void Validate()
        {
            var validator = new RoleRowValidator();
            Assert.IsTrue(validator.ValidateForAdd(Role()));
        }
        #endregion

        #region Helper Methods
        private RoleRow Role()
        {
            return new RoleRow(Guid.NewGuid())
            {
                Name = StringHelper.ValidString(),
                UserIdentifier = Guid.NewGuid(),
            };
        }
        #endregion
    }
}