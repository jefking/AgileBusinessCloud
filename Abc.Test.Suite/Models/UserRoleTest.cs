// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='UserRoleTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Models
{
    using System;
    using Abc.Website.Models;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class UserRoleTest
    {
        #region Valid Cases
        /// <summary>
        /// A test for Constructor
        /// </summary>
        [TestMethod]
        public void Constructor()
        {
            new UserRole();
        }

        [TestMethod]
        public void RoleName()
        {
            var userRole = new UserRole();
            var data = StringHelper.ValidString();
            userRole.RoleName = data;
            Assert.AreEqual<string>(data, userRole.RoleName);
        }

        [TestMethod]
        public void UserIdentifier()
        {
            var userRole = new UserRole();
            var data = Guid.NewGuid();
            userRole.UserIdentifier = data;
            Assert.AreEqual<Guid>(data, userRole.UserIdentifier);
        }
        #endregion
    }
}
