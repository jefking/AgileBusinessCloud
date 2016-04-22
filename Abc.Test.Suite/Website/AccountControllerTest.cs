// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='AccountControllerTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Website
{
    using System;
    using Abc.Azure;
    using Abc.Services;
    using Abc.Website.Controllers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure;

    [TestClass]
    public class AccountControllerTest
    {
        #region Error Cases
        #endregion

        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new AccountController();
        }

        [TestMethod]
        public void LogOn()
        {
            AccountController target = new AccountController();
            var actual = target.LogOn();
            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void RegisterClient()
        {
            var started = DateTime.UtcNow;
            AccountController target = new AccountController();
            var table = new AzureTable<UserData>(CloudStorageAccount.DevelopmentStorageAccount);
            var user = new UserData(string.Format("{0}@test.com", Guid.NewGuid()), Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            table.AddEntity(user);

            var returned = table.QueryBy(user.PartitionKey, user.RowKey);
            Assert.IsNotNull(returned);
            Assert.AreEqual<DateTime>(returned.CreatedOn, user.CreatedOn);
            Assert.AreEqual<DateTime>(returned.LastLoggedInOn, user.LastLoggedInOn);
            Assert.AreEqual<DateTime>(returned.LastActivityOn.Date, user.LastActivityOn.Date);
            Assert.AreEqual<DateTime>(returned.PasswordLastChangedOn, user.PasswordLastChangedOn);
            Assert.AreEqual<string>(returned.Email, user.Email);
            Assert.AreEqual<string>(returned.OpenId, user.OpenId);
            Assert.AreEqual<string>(returned.UserName, user.UserName);
            Assert.AreEqual<int>(returned.RoleValue, user.RoleValue);
            Assert.AreEqual<bool>(returned.IsApproved, user.IsApproved);
            Assert.AreEqual<bool>(returned.IsLockedOut, user.IsLockedOut);
            Assert.AreEqual<bool>(returned.EmailValidated, user.EmailValidated);
            Assert.AreEqual<string>(returned.EmailValidationKey, user.EmailValidationKey);
            Assert.AreEqual<DateTime>(returned.LastLockedOutOn.Date, user.LastLockedOutOn.Date);

            Assert.AreEqual<DateTime>(returned.LastActivityOn.Date, started.Date);
            Assert.IsTrue(returned.EmailValidated);
        }

        [TestMethod]
        public void RegisterManager()
        {
            var started = DateTime.UtcNow;
            var table = new AzureTable<UserData>(CloudStorageAccount.DevelopmentStorageAccount);
            var user = new UserData(string.Format("{0}@agilebusinesscloud.com", Guid.NewGuid()), Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            table.AddEntity(user);

            var target = new AccountController();

            var returned = table.QueryBy(user.PartitionKey, user.RowKey);
            Assert.IsNotNull(returned);
            Assert.AreEqual<DateTime>(returned.CreatedOn, user.CreatedOn);
            Assert.AreEqual<DateTime>(returned.LastLoggedInOn, user.LastLoggedInOn);
            Assert.AreEqual<DateTime>(returned.LastActivityOn.Date, user.LastActivityOn.Date);
            Assert.AreEqual<DateTime>(returned.PasswordLastChangedOn, user.PasswordLastChangedOn);
            Assert.AreEqual<string>(returned.Email, user.Email);
            Assert.AreEqual<string>(returned.OpenId, user.OpenId);
            Assert.AreEqual<string>(returned.UserName, user.UserName);
            Assert.AreEqual<int>(returned.RoleValue, user.RoleValue);
            Assert.AreEqual<bool>(returned.IsApproved, user.IsApproved);
            Assert.AreEqual<bool>(returned.IsLockedOut, user.IsLockedOut);
            Assert.AreEqual<bool>(returned.EmailValidated, user.EmailValidated);
            Assert.AreEqual<string>(returned.EmailValidationKey, user.EmailValidationKey);
            Assert.AreEqual<DateTime>(returned.LastLockedOutOn.Date, user.LastLockedOutOn.Date);

            Assert.IsTrue(returned.EmailValidated);
            Assert.AreEqual<int>((int)RoleType.Client, returned.RoleValue);
        }
        #endregion
    }
}