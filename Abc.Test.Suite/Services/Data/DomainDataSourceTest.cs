// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='DomainDataSourceTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite
{
    using System;
    using Abc.Services;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Domain Data Source Test
    /// </summary>
    [TestClass]
    public class DomainDataSourceTest
    {
        #region Error Cases
        #region Insert
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InsertNullUserData()
        {
            var source = new DomainSource();
            source.Insert(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void InsertInvalidNameIdentifier()
        {
            var source = new DomainSource();
            var user = User();
            user.NameIdentifier = StringHelper.NullEmptyWhiteSpace();
            source.Insert(user);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void InsertInvalidRowKey()
        {
            var source = new DomainSource();
            var user = User();
            user.RowKey = StringHelper.NullEmptyWhiteSpace();
            source.Insert(user);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void InsertInvalidPartitionKey()
        {
            var source = new DomainSource();
            var user = User();
            user.PartitionKey = StringHelper.NullEmptyWhiteSpace();
            source.Insert(user);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void InsertInvalidRoleValueNegative()
        {
            var source = new DomainSource();
            var user = User();
            Random random = new Random();
            user.RoleValue = random.Next(-1);
            source.Insert(user);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void InsertInvalidRoleValuePositive()
        {
            var source = new DomainSource();
            var user = User();
            user.RoleValue = 2;
            source.Insert(user);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void InsertInvalidCreatedOn()
        {
            var source = new DomainSource();
            var user = User();
            user.CreatedOn = DateTime.UtcNow.AddDays(1);
            source.Insert(user);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void InsertInvalidLastLoggedInOn()
        {
            var source = new DomainSource();
            var user = User();
            user.LastLoggedInOn = DateTime.UtcNow.AddDays(1);
            source.Insert(user);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void InsertInvalidLastActivityOn()
        {
            var source = new DomainSource();
            var user = User();
            user.LastActivityOn = DateTime.UtcNow.AddDays(1);
            source.Insert(user);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void InsertInvalidPasswordLastChangedOn()
        {
            var source = new DomainSource();
            var user = User();
            user.PasswordLastChangedOn = DateTime.UtcNow.AddDays(1);
            source.Insert(user);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void InsertInvalidLastLockedOutOn()
        {
            var source = new DomainSource();
            var user = User();
            user.LastLockedOutOn = DateTime.UtcNow.AddDays(1);
            source.Insert(user);
        }
        #endregion

        #region Update
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UpdateNullUserData()
        {
            var source = new DomainSource();
            source.Update(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void UpdateInvalidNameIdentifier()
        {
            var source = new DomainSource();
            var user = User();
            user.NameIdentifier = StringHelper.NullEmptyWhiteSpace();
            source.Update(user);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void UpdateInvalidRowKey()
        {
            var source = new DomainSource();
            var user = User();
            user.RowKey = StringHelper.NullEmptyWhiteSpace();
            source.Update(user);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void UpdateInvalidPartitionKey()
        {
            var source = new DomainSource();
            var user = User();
            user.PartitionKey = StringHelper.NullEmptyWhiteSpace();
            source.Update(user);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void UpdateInvalidRoleValueNegative()
        {
            var source = new DomainSource();
            var user = User();
            Random random = new Random();
            user.RoleValue = random.Next(-1);
            source.Update(user);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void UpdateInvalidRoleValuePositive()
        {
            var source = new DomainSource();
            var user = User();
            user.RoleValue = 2;
            source.Update(user);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void UpdateInvalidCreatedOn()
        {
            var source = new DomainSource();
            var user = User();
            user.CreatedOn = DateTime.UtcNow.AddDays(1);
            source.Update(user);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void UpdateInvalidLastLoggedInOn()
        {
            var source = new DomainSource();
            var user = User();
            user.LastLoggedInOn = DateTime.UtcNow.AddDays(1);
            source.Update(user);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void UpdateInvalidLastActivityOn()
        {
            var source = new DomainSource();
            var user = User();
            user.LastActivityOn = DateTime.UtcNow.AddDays(1);
            source.Update(user);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void UpdateInvalidPasswordLastChangedOn()
        {
            var source = new DomainSource();
            var user = User();
            user.PasswordLastChangedOn = DateTime.UtcNow.AddDays(1);
            source.Update(user);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void UpdateInvalidLastLockedOutOn()
        {
            var source = new DomainSource();
            var user = User();
            user.LastLockedOutOn = DateTime.UtcNow.AddDays(1);
            source.Update(user);
        }
        #endregion

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetUserByOpenIdInvalidOpenId()
        {
            var source = new DomainSource();
            source.GetUserByNameIdentifier(Guid.NewGuid(), StringHelper.NullEmptyWhiteSpace());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetUserByOpenIdInvalidApplicationId()
        {
            var source = new DomainSource();
            source.GetUserByNameIdentifier(Guid.Empty, StringHelper.ValidString());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetUserByIdInvalidId()
        {
            var source = new DomainSource();
            source.GetUserById(Guid.Empty, Guid.NewGuid());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetUserByIdInvalidApplicationId()
        {
            var source = new DomainSource();
            source.GetUserById(Guid.NewGuid(), Guid.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetUserByEmailInvalidEmail()
        {
            var source = new DomainSource();
            source.GetUserByEmail(Guid.NewGuid(), StringHelper.NullEmptyWhiteSpace());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetUserByEmailInvalidApplicationId()
        {
            var source = new DomainSource();
            source.GetUserByEmail(Guid.Empty, StringHelper.ValidString());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetUserByUsernameInvalidUserName()
        {
            var source = new DomainSource();
            source.GetUserByUserName(Guid.NewGuid(), StringHelper.NullEmptyWhiteSpace());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetUserByUsernameInvalidApplicationId()
        {
            var source = new DomainSource();
            source.GetUserByUserName(Guid.Empty, StringHelper.ValidString());
        }
        #endregion

        #region Valid Cases
        [TestMethod]
        public void UpdateUser()
        {
            var source = new DomainSource();
            var user = User();
            source.Insert(user);

            var saved = source.GetUserById(user.ApplicationId, user.Id);
            Assert.IsNotNull(saved);
            Assert.AreEqual<DateTime>(saved.CreatedOn, user.CreatedOn);
            Assert.AreEqual<DateTime>(saved.LastLoggedInOn, user.LastLoggedInOn);
            Assert.AreEqual<DateTime>(saved.LastActivityOn, user.LastActivityOn);
            Assert.AreEqual<DateTime>(saved.PasswordLastChangedOn, user.PasswordLastChangedOn);
            Assert.AreEqual<string>(saved.Email, user.Email);
            Assert.AreEqual<string>(saved.OpenId, user.OpenId);
            Assert.AreEqual<string>(saved.UserName, user.UserName);
            Assert.AreEqual<int>(saved.RoleValue, user.RoleValue);
            Assert.AreEqual<bool>(saved.IsApproved, user.IsApproved);
            Assert.AreEqual<bool>(saved.IsLockedOut, user.IsLockedOut);
            Assert.AreEqual<DateTime>(saved.LastLockedOutOn, user.LastLockedOutOn);

            var preUpdate = user;
            preUpdate.LastLoggedInOn = DateTime.UtcNow.AddHours(-34);
            preUpdate.IsLockedOut = true;
            preUpdate.IsApproved = false;
            preUpdate.LastActivityOn = DateTime.UtcNow.AddHours(-88);
            preUpdate.Email = StringHelper.ValidString();
            preUpdate.OpenId = StringHelper.ValidString();
            preUpdate.PasswordLastChangedOn = DateTime.UtcNow.AddHours(-22);

            source.Update(preUpdate);

            var updated = source.GetUserById(user.ApplicationId, user.Id);
            Assert.IsNotNull(preUpdate);
            Assert.AreEqual<DateTime>(preUpdate.CreatedOn, updated.CreatedOn);
            Assert.AreEqual<DateTime>(preUpdate.LastLoggedInOn.Date, updated.LastLoggedInOn.Date);
            Assert.AreEqual<DateTime>(preUpdate.LastActivityOn.Date, updated.LastActivityOn.Date);
            Assert.AreEqual<DateTime>(preUpdate.PasswordLastChangedOn.Date, updated.PasswordLastChangedOn.Date);
            Assert.AreEqual<string>(preUpdate.Email, updated.Email);
            Assert.AreEqual<string>(preUpdate.OpenId, updated.OpenId);
            Assert.AreEqual<string>(preUpdate.UserName, updated.UserName);
            Assert.AreEqual<int>(preUpdate.RoleValue, updated.RoleValue);
            Assert.AreEqual<bool>(preUpdate.IsApproved, updated.IsApproved);
            Assert.AreEqual<bool>(preUpdate.IsLockedOut, updated.IsLockedOut);
            Assert.AreEqual<DateTime>(preUpdate.LastLockedOutOn, updated.LastLockedOutOn);
        }

        [TestMethod]
        public void GetUserById()
        {
            var source = new DomainSource();
            var user = User();
            source.Insert(user);

            var saved = source.GetUserById(user.ApplicationId, user.Id);
            Assert.IsNotNull(saved);
            Assert.AreEqual<DateTime>(saved.CreatedOn, user.CreatedOn);
            Assert.AreEqual<DateTime>(saved.LastLoggedInOn, user.LastLoggedInOn);
            Assert.AreEqual<DateTime>(saved.LastActivityOn, user.LastActivityOn);
            Assert.AreEqual<DateTime>(saved.PasswordLastChangedOn, user.PasswordLastChangedOn);
            Assert.AreEqual<string>(saved.Email, user.Email);
            Assert.AreEqual<string>(saved.OpenId, user.OpenId);
            Assert.AreEqual<string>(saved.UserName, user.UserName);
            Assert.AreEqual<int>(saved.RoleValue, user.RoleValue);
            Assert.AreEqual<bool>(saved.IsApproved, user.IsApproved);
            Assert.AreEqual<bool>(saved.IsLockedOut, user.IsLockedOut);
            Assert.AreEqual<DateTime>(saved.LastLockedOutOn, user.LastLockedOutOn);
        }

        [TestMethod]
        public void GetUserByNameIdentifier()
        {
            var source = new DomainSource();
            var user = User();
            user.NameIdentifier = StringHelper.ValidString();
            source.Insert(user);

            var saved = source.GetUserByNameIdentifier(user.ApplicationId, user.NameIdentifier);
            Assert.IsNotNull(saved);
            Assert.AreEqual<DateTime>(saved.CreatedOn, user.CreatedOn);
            Assert.AreEqual<DateTime>(saved.LastLoggedInOn, user.LastLoggedInOn);
            Assert.AreEqual<DateTime>(saved.LastActivityOn, user.LastActivityOn);
            Assert.AreEqual<DateTime>(saved.PasswordLastChangedOn, user.PasswordLastChangedOn);
            Assert.AreEqual<string>(saved.Email, user.Email);
            Assert.AreEqual<string>(saved.OpenId, user.OpenId);
            Assert.AreEqual<string>(saved.UserName, user.UserName);
            Assert.AreEqual<int>(saved.RoleValue, user.RoleValue);
            Assert.AreEqual<bool>(saved.IsApproved, user.IsApproved);
            Assert.AreEqual<bool>(saved.IsLockedOut, user.IsLockedOut);
            Assert.AreEqual<DateTime>(saved.LastLockedOutOn, user.LastLockedOutOn);
        }

        [TestMethod]
        public void GetUserByEmail()
        {
            var source = new DomainSource();
            var user = User();
            user.Email = StringHelper.ValidString();
            source.Insert(user);

            var saved = source.GetUserByEmail(user.ApplicationId, user.Email);
            Assert.IsNotNull(saved);
            Assert.AreEqual<DateTime>(saved.CreatedOn, user.CreatedOn);
            Assert.AreEqual<DateTime>(saved.LastLoggedInOn, user.LastLoggedInOn);
            Assert.AreEqual<DateTime>(saved.LastActivityOn, user.LastActivityOn);
            Assert.AreEqual<DateTime>(saved.PasswordLastChangedOn, user.PasswordLastChangedOn);
            Assert.AreEqual<string>(saved.Email, user.Email);
            Assert.AreEqual<string>(saved.OpenId, user.OpenId);
            Assert.AreEqual<string>(saved.UserName, user.UserName);
            Assert.AreEqual<int>(saved.RoleValue, user.RoleValue);
            Assert.AreEqual<bool>(saved.IsApproved, user.IsApproved);
            Assert.AreEqual<bool>(saved.IsLockedOut, user.IsLockedOut);
            Assert.AreEqual<DateTime>(saved.LastLockedOutOn, user.LastLockedOutOn);
        }

        [TestMethod]
        public void GetUserByUserName()
        {
            var source = new DomainSource();
            var user = User();
            user.UserName = StringHelper.ValidString();
            source.Insert(user);

            var saved = source.GetUserByUserName(user.ApplicationId, user.UserName);
            Assert.IsNotNull(saved);
            Assert.AreEqual<DateTime>(saved.CreatedOn, user.CreatedOn);
            Assert.AreEqual<DateTime>(saved.LastLoggedInOn, user.LastLoggedInOn);
            Assert.AreEqual<DateTime>(saved.LastActivityOn, user.LastActivityOn);
            Assert.AreEqual<DateTime>(saved.PasswordLastChangedOn, user.PasswordLastChangedOn);
            Assert.AreEqual<string>(saved.Email, user.Email);
            Assert.AreEqual<string>(saved.OpenId, user.OpenId);
            Assert.AreEqual<string>(saved.UserName, user.UserName);
            Assert.AreEqual<int>(saved.RoleValue, user.RoleValue);
            Assert.AreEqual<bool>(saved.IsApproved, user.IsApproved);
            Assert.AreEqual<bool>(saved.IsLockedOut, user.IsLockedOut);
            Assert.AreEqual<DateTime>(saved.LastLockedOutOn, user.LastLockedOutOn);
        }
        #endregion

        #region Helper Methods
        private static UserData User()
        {
            return new UserData("test@abc.com", StringHelper.ValidString(), StringHelper.ValidString());
        }
        #endregion
    }
}