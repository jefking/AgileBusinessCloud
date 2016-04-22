// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='RoleRowTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Data
{
    using System;
    using Abc.Services.Data;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class RoleRowTest
    {
        #region Error Cases
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ConstructorEmptyApplicationIdentifer()
        {
            new RoleRow(Guid.Empty);
        }
        #endregion

        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new RoleRow();
        }

        [TestMethod]
        public void ConstructorApplicationIdentifier()
        {
            new RoleRow(Guid.NewGuid());
        }

        [TestMethod]
        public void UserIdentifier()
        {
            var data = Guid.NewGuid();
            var role = new RoleRow(Guid.NewGuid());
            role.UserIdentifier = data;
            Assert.AreEqual<Guid>(data, role.UserIdentifier);
        }

        [TestMethod]
        public void Name()
        {
            var data = StringHelper.ValidString();
            var role = new RoleRow(Guid.NewGuid());
            role.Name = data;
            Assert.AreEqual<string>(data, role.Name);
        }
        #endregion
    }
}