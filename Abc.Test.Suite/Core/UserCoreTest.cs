// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='UserCoreTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Core
{
    using Abc.Azure;
    using Abc.Configuration;
    using Abc.Services;
    using Abc.Services.Contracts;
    using Abc.Services.Core;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    [TestClass]
    public class UserCoreTest
    {
        #region Error Cases
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetByIdentifierUserAppNull()
        {
            var core = new UserCore();
            core.GetByIdentifier(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetByIdentifierUserAppApplicationNull()
        {
            var userApp = new UserApplication()
            {
                User = new User(),
            };
            userApp.User.Identifier = Guid.NewGuid();

            var core = new UserCore();
            core.GetByIdentifier(userApp);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetByIdentifierUserAppApplicationIdentifierEmpty()
        {
            var userApp = new UserApplication()
            {
                User = new User(),
                Application = new Application(),
            };
            userApp.User.Identifier = Guid.NewGuid();
            userApp.Application.Identifier = Guid.Empty;

            var core = new UserCore();
            core.GetByIdentifier(userApp);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetByIdentifierUserAppUserNull()
        {
            var userApp = new UserApplication()
            {
                Application = new Application(),
            };
            userApp.Application.Identifier = Guid.NewGuid();

            var core = new UserCore();
            core.GetByIdentifier(userApp);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetByIdentifierUserAppUserIdentifierEmpty()
        {
            var userApp = new UserApplication()
            {
                User = new User(),
                Application = new Application(),
            };
            userApp.User.Identifier = Guid.Empty;
            userApp.Application.Identifier = Guid.NewGuid();

            var core = new UserCore();
            core.GetByIdentifier(userApp);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetProfilePagePageNull()
        {
            var core = new UserCore();
            core.Get((ProfilePage)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetProfilePageApplicationNull()
        {
            var core = new UserCore();
            core.PublicProfiles(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetProfilePagesApplicationIdentifierEmpty()
        {
            var core = new UserCore();
            core.PublicProfiles(new Application());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetProfileFullPageApplicationNull()
        {
            var core = new UserCore();
            core.PublicProfilesFull(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetProfileFullPagesApplicationIdentifierEmpty()
        {
            var core = new UserCore();
            core.PublicProfilesFull(new Application());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetProfilePageApplicationIdentifierEmpty()
        {
            var profile = new ProfilePage()
            {
                ApplicationIdentifier = Guid.Empty,
                Handle = StringHelper.ValidString(),
            };

            var core = new UserCore();
            core.Get(profile);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetProfilePageHandleInvalid()
        {
            var profile = new ProfilePage()
            {
                ApplicationIdentifier = Guid.NewGuid(),
                Handle = StringHelper.NullEmptyWhiteSpace(),
            };

            var core = new UserCore();
            core.Get(profile);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SaveProfilePagePageNull()
        {
            var core = new UserCore();
            core.Save((ProfilePage)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SaveProfilePageApplicationIdentifierEmpty()
        {
            var profile = new ProfilePage()
            {
                ApplicationIdentifier = Guid.Empty,
                Handle = StringHelper.ValidString(),
                OwnerIdentifier = Guid.NewGuid(),
            };

            var core = new UserCore();
            core.Save(profile);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SaveProfilePageOwnerIdentifierEmpty()
        {
            var profile = new ProfilePage()
            {
                ApplicationIdentifier = Guid.NewGuid(),
                Handle = StringHelper.ValidString(),
                OwnerIdentifier = Guid.Empty,
            };

            var core = new UserCore();
            core.Save(profile);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SaveProfilePageHandleInvalid()
        {
            var profile = new ProfilePage()
            {
                ApplicationIdentifier = Guid.NewGuid(),
                Handle = StringHelper.NullEmptyWhiteSpace(),
                OwnerIdentifier = Guid.NewGuid(),
            };

            var core = new UserCore();
            core.Save(profile);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetUserByEmailNull()
        {
            var core = new UserCore();
            core.GetByEmail(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetUserByEmailInvalidEmail()
        {
            var userApp = this.UserApp();
            userApp.User.Email = StringHelper.NullEmptyWhiteSpace();
            var core = new UserCore();
            core.GetByEmail(userApp);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetUserByEmailUserNull()
        {
            var userApp = this.UserApp();
            userApp.User = null;
            var core = new UserCore();
            core.GetByEmail(userApp);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetUserByEmailInvalidApplicationId()
        {
            var userApp = this.UserApp();
            userApp.Application.Identifier = Guid.Empty;
            var core = new UserCore();
            core.GetByEmail(userApp);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetUserByEmailApplicationNull()
        {
            var userApp = this.UserApp();
            userApp.Application = null;
            var core = new UserCore();
            core.GetByEmail(userApp);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetGroupContactsNull()
        {
            var core = new UserCore();
            core.GetGroups(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetGroupContactsEmptyUserIdentifier()
        {
            var core = new UserCore();
            var data = this.UserData();
            data.Identifier = Guid.Empty;
            core.GetGroups(data);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SaveContactGroupNullContactGroup()
        {
            var core = new UserCore();
            core.Save((ContactGroup)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SaveContactGroupNullOwner()
        {
            var core = new UserCore();
            var data = this.Group();
            data.Owner = null;
            core.Save(data);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SaveContactGroupEmptyIdentifier()
        {
            var core = new UserCore();
            var data = this.Group();
            data.Identifier = Guid.Empty;
            core.Save(data);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SaveContactGroupEmptyOwnerIdentifier()
        {
            var core = new UserCore();
            var data = this.Group();
            data.Owner.Identifier = Guid.Empty;
            core.Save(data);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetContactsNull()
        {
            var core = new UserCore();
            core.GetContacts(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetContactsEmptyUserIdentifier()
        {
            var core = new UserCore();
            var data = this.UserData();
            data.Identifier = Guid.Empty;
            core.GetContacts(data);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SaveContactNullContact()
        {
            var core = new UserCore();
            core.Save((Contact)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SaveContactNullOwner()
        {
            var core = new UserCore();
            var data = this.Contact();
            data.Owner = null;
            core.Save(data);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SaveContactEmptyIdentifier()
        {
            var core = new UserCore();
            var data = this.Contact();
            data.Identifier = Guid.Empty;
            core.Save(data);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SaveContactEmptyOwnerIdentifier()
        {
            var core = new UserCore();
            var data = this.Contact();
            data.Owner.Identifier = Guid.Empty;
            core.Save(data);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetUserApplicationNull()
        {
            var core = new UserCore();
            core.Get((UserApplication)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetUserApplicationNullApplication()
        {
            var core = new UserCore();
            var data = this.UserApp();
            data.Application = null;
            core.Get(data);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetUserApplicationNullUser()
        {
            var core = new UserCore();
            var data = this.UserApp();
            data.User = null;
            core.Get(data);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetUserApplicationEmptyApplicationIdentifier()
        {
            var core = new UserCore();
            var data = this.UserApp();
            data.Application.Identifier = Guid.Empty;
            core.Get(data);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetUserApplicationEmptyUserIdentifier()
        {
            var core = new UserCore();
            var data = this.UserApp();
            data.User.Identifier = Guid.Empty;
            core.Get(data);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ValidateNullApplication()
        {
            var core = new UserCore();
            core.SetRoles(null, Guid.NewGuid());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ValidateEmptyApplicationIdentifier()
        {
            var app = new Application()
            {
                Identifier = Guid.Empty,
            };

            var core = new UserCore();
            core.SetRoles(app, Guid.NewGuid());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ValidateEmptyUserId()
        {
            var app = new Application()
            {
                Identifier = Guid.NewGuid(),
            };

            var core = new UserCore();
            core.SetRoles(app, Guid.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetUserPreferenceNull()
        {
            var core = new UserCore();
            core.Get((UserPreference)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetUserPreferenceNullApplication()
        {
            var core = new UserCore();
            var pref = this.Preference();
            pref.Application = null;
            core.Get(pref);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetUserPreferenceNullUser()
        {
            var core = new UserCore();
            var pref = this.Preference();
            pref.User = null;
            core.Get(pref);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetUserPreferenceEmptyApplicationId()
        {
            var core = new UserCore();
            var pref = this.Preference();
            pref.Application.Identifier = Guid.Empty;
            core.Get(pref);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetUserPreferenceEmptyUserId()
        {
            var core = new UserCore();
            var pref = this.Preference();
            pref.User.Identifier = Guid.Empty;
            core.Get(pref);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SaveUserPreferenceNull()
        {
            var core = new UserCore();
            core.Save((UserPreference)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SaveUserPreferenceNullApplication()
        {
            var core = new UserCore();
            var pref = this.Preference();
            pref.Application = null;
            core.Save(pref);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SaveUserPreferenceNullUser()
        {
            var core = new UserCore();
            var pref = this.Preference();
            pref.User = null;
            core.Save(pref);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SaveUserPreferenceEmptyApplicationId()
        {
            var core = new UserCore();
            var pref = this.Preference();
            pref.Application.Identifier = Guid.Empty;
            core.Save(pref);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SaveUserPreferenceEmptyUserId()
        {
            var core = new UserCore();
            var pref = this.Preference();
            pref.User.Identifier = Guid.Empty;
            core.Save(pref);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ProfilePageEditAnothers()
        {
            var page = new ProfilePage()
            {
                ApplicationIdentifier = Guid.NewGuid(),
                Handle = Guid.NewGuid().ToString(),
                OwnerIdentifier = Guid.NewGuid(),
            };

            var core = new UserCore();
            core.Save(page);
            page.OwnerIdentifier = Guid.NewGuid();
            core.Save(page);
        }
        #endregion

        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new UserCore();
        }

        [TestMethod]
        public void IsIUserCore()
        {
            Assert.IsNotNull(new UserCore() as IUserCore);
        }

        [TestMethod]
        public void IsDataCore()
        {
            Assert.IsNotNull(new UserCore() as DataCore);
        }

        [TestMethod]
        public void SaveProfilePage()
        {
            var page = new ProfilePage()
            {
                ApplicationIdentifier = Guid.NewGuid(),
                Handle = Guid.NewGuid().ToString(),
                OwnerIdentifier = Guid.NewGuid(),
            };

            var core = new UserCore();
            var returned = core.Save(page);
            Assert.AreEqual<string>(page.Handle, returned.Handle);
            Assert.AreEqual<Guid>(page.ApplicationIdentifier, returned.ApplicationIdentifier);
            Assert.AreEqual<Guid>(page.OwnerIdentifier, returned.OwnerIdentifier);
        }

        [TestMethod]
        public void RoundTripProfilePage()
        {
            var page = new ProfilePage()
            {
                ApplicationIdentifier = Guid.NewGuid(),
                Handle = Guid.NewGuid().ToString(),
                OwnerIdentifier = Guid.NewGuid(),
            };

            var core = new UserCore();
            core.Save(page);
            var get = core.Get(page);
            Assert.IsNotNull(get);
            Assert.AreEqual<string>(page.Handle, get.Handle);
            Assert.AreEqual<Guid>(page.ApplicationIdentifier, get.ApplicationIdentifier);
            Assert.AreEqual<Guid>(page.OwnerIdentifier, get.OwnerIdentifier);
        }

        [TestMethod]
        public void ProfilePageUpdate()
        {
            var page = new ProfilePage()
            {
                ApplicationIdentifier = Guid.NewGuid(),
                Handle = Guid.NewGuid().ToString(),
                OwnerIdentifier = Guid.NewGuid(),
            };

            var core = new UserCore();
            core.Save(page);
            var newPage = new ProfilePage()
            {
                ExistingHandle = page.Handle,
                Handle = Guid.NewGuid().ToString(),
                ApplicationIdentifier = page.ApplicationIdentifier,
                OwnerIdentifier = page.OwnerIdentifier,
            };
            core.Save(newPage);
            var get = core.Get(page);
            Assert.IsNull(get);
            get = core.Get(newPage);
            Assert.AreEqual<string>(newPage.Handle, get.Handle);
            Assert.AreEqual<Guid>(newPage.ApplicationIdentifier, get.ApplicationIdentifier);
            Assert.AreEqual<Guid>(newPage.OwnerIdentifier, get.OwnerIdentifier);
        }

        [TestMethod]
        public void GetPublicProfiles()
        {
            var appId = Guid.NewGuid();
            var core = new UserCore();
            for (int i = 0; i < 5; i++)
            {
                var page = new ProfilePage()
                {
                    ApplicationIdentifier = appId,
                    Handle = Guid.NewGuid().ToString(),
                    OwnerIdentifier = Guid.NewGuid(),
                };

                core.Save(page);
            }
            var app = new Application()
            {
                Identifier = appId,
            };

            var profiles = core.PublicProfiles(app);
            Assert.AreEqual<int>(5, profiles.Count());
        }

        [TestMethod]
        public void UserPreferenceRoundTrip()
        {
            var core = new UserCore();
            var pref = this.Preference();
            var saved = core.Save(pref);

            Assert.AreEqual<Guid>(pref.User.Identifier, saved.User.Identifier);
            Assert.AreEqual<Guid>(pref.Application.Identifier, saved.Application.Identifier);
            Assert.AreEqual<Guid>(pref.CurrentApplication.Identifier, saved.CurrentApplication.Identifier);
            Assert.AreEqual<string>(pref.TimeZone.Id, saved.TimeZone.Id);
            Assert.AreEqual<string>(pref.TwitterHandle, saved.TwitterHandle);
            Assert.AreEqual<string>(pref.AbcHandle, saved.AbcHandle);
            Assert.AreEqual<string>(pref.GitHubHandle, saved.GitHubHandle);
            Assert.AreEqual<string>(pref.City, saved.City);
            Assert.AreEqual<string>(pref.Country, saved.Country);

            var got = core.Get(pref);
            Assert.AreEqual<Guid>(pref.User.Identifier, got.User.Identifier);
            Assert.AreEqual<Guid>(pref.Application.Identifier, got.Application.Identifier);
            Assert.AreEqual<Guid>(pref.CurrentApplication.Identifier, got.CurrentApplication.Identifier);
            Assert.AreEqual<string>(pref.TimeZone.Id, got.TimeZone.Id);
            Assert.AreEqual<string>(pref.TwitterHandle, got.TwitterHandle);
            Assert.AreEqual<string>(pref.AbcHandle, got.AbcHandle);
            Assert.AreEqual<string>(pref.GitHubHandle, got.GitHubHandle);
            Assert.AreEqual<string>(pref.City, got.City);
            Assert.AreEqual<string>(pref.Country, got.Country);
        }

        [TestMethod]
        public void UserPreferenceDoesntExist()
        {
            var core = new UserCore();
            var pref = this.Preference();

            var got = core.Get(pref);
            Assert.AreEqual<Guid>(pref.User.Identifier, got.User.Identifier);
            Assert.AreEqual<Guid>(pref.Application.Identifier, got.Application.Identifier);
            Assert.AreEqual<Guid>(pref.CurrentApplication.Identifier, got.CurrentApplication.Identifier);
            Assert.AreEqual<string>(pref.TimeZone.Id, got.TimeZone.Id);
        }

        [TestMethod]
        public void Validate()
        {
            var emailValidationKey = StringHelper.ValidString();
            var app = Application.Default;
            var email = StringHelper.ValidString();
            var userData = new UserData(email, StringHelper.ValidString(), StringHelper.ValidString())
            {
                EmailValidationKey = emailValidationKey,
            };

            var table = new AzureTable<UserData>(CloudStorageAccount.DevelopmentStorageAccount);
            table.AddEntity(userData);

            var core = new UserCore();
            var returnedEmail = core.SetRoles(app, userData.Id);
            Assert.AreEqual<string>(email, returnedEmail);
        }

        [TestMethod]
        public void GetUser()
        {
            var data = new UserData(StringHelper.ValidString(), StringHelper.ValidString(), StringHelper.ValidString());
            var table = new AzureTable<UserData>(ServerConfiguration.Default);
            table.AddEntity(data);

            var core = new UserCore();
            var userApp = this.UserApp();
            userApp.User.Identifier = data.Id;
            userApp.Application.Identifier = data.ApplicationId;
            var user = core.Get(userApp);

            Assert.AreEqual<Guid>(data.Id, user.Identifier);
            Assert.AreEqual<string>(data.Email, user.Email);
            Assert.AreEqual<string>(data.UserName, user.UserName);
        }

        [TestMethod]
        public void GetByIdentifier()
        {
            var data = new UserData(StringHelper.ValidString(), StringHelper.ValidString(), StringHelper.ValidString());
            var table = new AzureTable<UserData>(ServerConfiguration.Default);
            table.AddEntity(data);

            var userApp = new UserApplication()
            {
                User = new User(),
                Application = new Application(),
            };
            userApp.User.Identifier = data.Id;
            userApp.Application.Identifier = data.ApplicationId;

            var core = new UserCore();
            var got = core.GetByIdentifier(userApp);
            Assert.IsNotNull(got);
            Assert.AreEqual<Guid>(data.Id, got.Identifier);
        }

        [TestMethod]
        [Ignore]
        public void GetContacts()
        {
            var user = this.UserData();
            var random = new Random();
            var count = random.Next(50);
            var core = new UserCore();
            Parallel.For(
                0,
                count,
                (a, b) =>
                {
                    var c = this.Contact();
                    c.Owner = user;
                    core.Save(c);
                });
            var contacts = core.GetContacts(user);
            Assert.AreEqual<int>(count, contacts.Count());
        }

        [TestMethod]
        public void GetContactGroups()
        {
            var user = this.UserData();
            var random = new Random();
            var count = random.Next(50);
            var core = new UserCore();
            for (int i = 0; i < count; i++)
            {
                var c = this.Group();
                c.Owner = user;
                core.Save(c);
            }

            var groups = core.GetGroups(user);
            Assert.AreEqual<int>(count, groups.Count());
        }

        [TestMethod]
        public void GetUserByEmail()
        {
            var data = new UserData(StringHelper.ValidString(), StringHelper.ValidString(), StringHelper.ValidString());
            var table = new AzureTable<UserData>(ServerConfiguration.Default);
            table.AddEntity(data);

            var userApp = new UserApplication()
            {
                Application = Application.Default,
                User = data.Convert(),
            };

            var core = new UserCore();
            var saved = core.GetByEmail(userApp);
            Assert.IsNotNull(saved);
            Assert.AreEqual<Guid>(data.Id, saved.Identifier);
            Assert.AreEqual<string>(data.Email, saved.Email);
            Assert.AreEqual<string>(data.UserName, saved.UserName);
        }
        #endregion

        #region Helper Methods
        private UserPreference Preference()
        {
            var application = new Application()
            {
                Identifier = Guid.NewGuid(),
            };
            var currentApplication = new Application()
            {
                Identifier = Guid.NewGuid(),
            };
            return new UserPreference()
            {
                User = this.UserData(),
                Application = application,
                CurrentApplication = currentApplication,
                TimeZone = TimeZoneInfo.Local,
                TwitterHandle = StringHelper.ValidString(),
                AbcHandle = StringHelper.ValidString(),
                GitHubHandle = StringHelper.ValidString(),
                City = StringHelper.ValidString(),
                Country = StringHelper.ValidString(),
            };
        }

        private UserApplication UserApp()
        {
            var app = new Application()
            {
                Identifier = Guid.NewGuid(),
            };

            return new UserApplication()
            {
                Active = true,
                Application = app,
                Deleted = true,
                User = this.UserData(),
            };
        }

        private Contact Contact()
        {
            return new Contact()
            {
                Email = StringHelper.ValidString(),
                Identifier = Guid.NewGuid(),
                Owner = this.UserData(),
            };
        }

        private User UserData()
        {
            return new User()
            {
                Identifier = Guid.NewGuid(),
            };
        }

        private ContactGroup Group()
        {
            return new ContactGroup()
            {
                Name = StringHelper.ValidString(),
                Owner = this.UserData(),
                Identifier = Guid.NewGuid(),
            };
        }
        #endregion
    }
}