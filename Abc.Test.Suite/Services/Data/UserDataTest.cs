// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='UserDataTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite
{
    using System;
    using Abc.Services;
    using Abc.Underpinning;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// User Data Test
    /// </summary>
    [TestClass]
    public class UserDataTest
    {
        #region Error Cases
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void InvalidNameIdentifier()
        {
            new UserData("good", StringHelper.NullEmptyWhiteSpace(), "good");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SetKeysApplicationIdentifierInvalid()
        {
            var user = new UserData();
            user.SetKeys(Guid.Empty, Guid.NewGuid());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SetKeysIdentifierInvalid()
        {
            var user = new UserData();
            user.SetKeys(Guid.NewGuid(), Guid.Empty);
        }
        #endregion

        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new UserData();
        }

        [TestMethod]
        public void ConstructorEmailOpenIdUserName()
        {
            new UserData(StringHelper.ValidString(), StringHelper.ValidString(), StringHelper.ValidString());
        }

        [TestMethod]
        public void IdConstructor()
        {
            var user = new UserData();
            Assert.AreNotEqual<Guid>(Guid.Empty, user.Id);
        }

        [TestMethod]
        public void ApplicationIdConstructor()
        {
            var user = new UserData();
            Assert.AreEqual<Guid>(Abc.Services.Contracts.Application.Default.Identifier, user.ApplicationId);
        }

        [TestMethod]
        public void SetKeys()
        {
            var user = new UserData();
            var appId = Guid.NewGuid();
            var id = Guid.NewGuid();
            user.SetKeys(appId, id);
            Assert.AreEqual<Guid>(appId, user.ApplicationId);
            Assert.AreEqual<Guid>(id, user.Id);
        }

        [TestMethod]
        public void UserName()
        {
            var data = StringHelper.ValidString();
            var user = new UserData(StringHelper.ValidString(), StringHelper.ValidString(), data);
            Assert.AreEqual<string>(data, user.UserName);
            data = StringHelper.ValidString();
            user.UserName = data;
            Assert.AreEqual<string>(data, user.UserName);
        }

        [TestMethod]
        public void CreatedOn()
        {
            var user = new UserData(StringHelper.ValidString(), StringHelper.ValidString(), StringHelper.ValidString());
            var data = DateTime.Now;
            user.CreatedOn = data;
            Assert.AreEqual<DateTime>(data, user.CreatedOn);
        }

        [TestMethod]
        public void OpenId()
        {
            var data = StringHelper.ValidString();
            var user = new UserData(StringHelper.ValidString(), data, StringHelper.ValidString());
            data = StringHelper.ValidString();
            user.OpenId = data;
            Assert.AreEqual<string>(data, user.OpenId);
        }

        [TestMethod]
        public void NameIdentifier()
        {
            var data = StringHelper.ValidString();
            var user = new UserData(StringHelper.ValidString(), data, StringHelper.ValidString());
            Assert.AreEqual<string>(data, user.NameIdentifier);
            data = StringHelper.ValidString();
            user.NameIdentifier = data;
            Assert.AreEqual<string>(data, user.NameIdentifier);
        }

        [TestMethod]
        public void Email()
        {
            var data = StringHelper.ValidString();
            var user = new UserData(data, StringHelper.ValidString(), StringHelper.ValidString());
            Assert.AreEqual<string>(data, user.Email);
            data = StringHelper.ValidString();
            user.Email = data;
            Assert.AreEqual<string>(data, user.Email);
        }

        [TestMethod]
        public void EmailValidationKey()
        {
            var data = StringHelper.ValidString();
            var user = new UserData();
            user.EmailValidationKey = data;
            Assert.AreEqual<string>(data, user.EmailValidationKey);
        }

        [TestMethod]
        public void RoleValue()
        {
            var random = new Random();
            var data = random.Next();
            var user = new UserData();
            user.RoleValue = data;
            Assert.AreEqual<int>(data, user.RoleValue);
        }

        [TestMethod]
        public void IsApproved()
        {
            var user = new UserData();
            user.IsApproved = true;
            Assert.IsTrue(user.IsApproved);
        }

        [TestMethod]
        public void IsLockedOut()
        {
            var user = new UserData();
            user.IsLockedOut = true;
            Assert.IsTrue(user.IsLockedOut);
        }

        [TestMethod]
        public void LastLoggedInOn()
        {
            var data = DateTime.UtcNow;
            var user = new UserData();
            user.LastLoggedInOn = data;
            Assert.AreEqual<DateTime>(data, user.LastLoggedInOn);
        }

        [TestMethod]
        public void PasswordLastChangedOn()
        {
            var data = DateTime.UtcNow;
            var user = new UserData();
            user.PasswordLastChangedOn = data;
            Assert.AreEqual<DateTime>(data, user.PasswordLastChangedOn);
        }

        [TestMethod]
        public void LastLockedOutOn()
        {
            var data = DateTime.UtcNow;
            var user = new UserData();
            user.LastLockedOutOn = data;
            Assert.AreEqual<DateTime>(data, user.LastLockedOutOn);
        }

        [TestMethod]
        public void DefaultValues()
        {
            var user = new UserData();
            Assert.AreEqual<Guid>(Abc.Services.Contracts.Application.Default.Identifier, user.ApplicationId);
            Assert.AreNotEqual<Guid>(Guid.Empty, user.Id);
        }

        [TestMethod]
        public void NewClientDefaultValues()
        {
            var user = new UserData("tom@gmail.com", "this is a fake open id", "tom hanks");
            Assert.AreEqual<DateTime>(DateTime.UtcNow.Date, user.CreatedOn.Date);
            Assert.AreEqual<DateTime>(DateTime.UtcNow.Date, user.LastLoggedInOn.Date);
            Assert.AreEqual<DateTime>(DateTime.UtcNow.Date, user.LastActivityOn.Date);
            Assert.AreEqual<DateTime>(DateTime.UtcNow.Date, user.PasswordLastChangedOn.Date);
            Assert.AreEqual<string>("tom@gmail.com", user.Email);
            Assert.IsNull(user.OpenId);
            Assert.AreEqual<string>("this is a fake open id", user.NameIdentifier);
            Assert.AreEqual<string>("tom hanks", user.UserName);
            Assert.AreEqual<int>(0, user.RoleValue);
            Assert.IsTrue(user.IsApproved);
            Assert.IsFalse(user.IsLockedOut);
            Assert.IsTrue(user.EmailValidated);
            Assert.IsFalse(string.IsNullOrWhiteSpace(user.EmailValidationKey));
            Assert.AreEqual<DateTime>(DateTime.UtcNow.AddDays(-1).Date, user.LastLockedOutOn.Date);
        }

        [TestMethod]
        public void EmailValidated()
        {
            var user = new UserData();
            Assert.IsFalse(user.EmailValidated);
            user.EmailValidated = true;
            Assert.IsTrue(user.EmailValidated);
            user.EmailValidated = false;
            Assert.IsFalse(user.EmailValidated);
        }
        #endregion
    }
}