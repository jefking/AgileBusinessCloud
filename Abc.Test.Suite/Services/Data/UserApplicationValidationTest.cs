// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='UserApplicationValidationTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Data
{
    using System;
    using Abc.Services;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class UserApplicationValidationTest
    {
        #region Error Cases
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ValidateForAddNull()
        {
            var validate = new UserApplicationValidation();
            validate.ValidateForAdd(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ValidateForAddEmptyUserId()
        {
            var validate = new UserApplicationValidation();
            var user = new UserApplicationData(Guid.Empty, Guid.NewGuid());

            validate.ValidateForAdd(user);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ValidateForAddOrUpdateNull()
        {
            var validate = new UserApplicationValidation();
            validate.ValidateForAddOrUpdate(null);
        }
        #endregion

        #region Valid Cases
        [TestMethod]
        public void ValidateForAdd()
        {
            var validate = new UserApplicationValidation();
            validate.ValidateForAdd(this.UserApp());
        }

        [TestMethod]
        public void ValidateForAddOrUpdate()
        {
            var validate = new UserApplicationValidation();
            validate.ValidateForAddOrUpdate(this.UserApp());
        }
        #endregion

        #region Helper Methods
        private UserApplicationData UserApp()
        {
            return new UserApplicationData(Guid.NewGuid(), Guid.NewGuid());
        }
        #endregion
    }
}