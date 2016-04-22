// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='ApplicationCoreTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite
{
    using System;
    using System.Linq;
    using System.Security;
    using Abc.Azure;
    using Abc.Configuration;
    using Abc.Services;
    using Abc.Services.Contracts;
    using Abc.Services.Core;
    using Abc.Services.Data;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure;

    [TestClass]
    public class ApplicationCoreTest
    {
        #region Error Cases
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UserIsAssociatedNullEditor()
        {
            var core = new ApplicationCore();
            core.UserIsAssociated(null, this.App(Guid.NewGuid()));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UserIsAssociatedNullEditorApplication()
        {
            var core = new ApplicationCore();
            var editor = this.UserApplication(Guid.NewGuid());
            editor.Application = null;
            core.UserIsAssociated(editor, this.App(Guid.NewGuid()));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UserIsAssociatedNullEditorUser()
        {
            var core = new ApplicationCore();
            var editor = this.UserApplication(Guid.NewGuid());
            editor.User = null;
            core.UserIsAssociated(editor, this.App(Guid.NewGuid()));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UserIsAssociatedNullApplication()
        {
            var core = new ApplicationCore();
            core.UserIsAssociated(this.UserApplication(Guid.NewGuid()), null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void UserIsAssociatedEmptyEditorUserIdentifier()
        {
            var core = new ApplicationCore();
            var editor = this.UserApplication(Guid.NewGuid());
            editor.User.Identifier = Guid.Empty;
            core.UserIsAssociated(editor, this.App(Guid.NewGuid()));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void UserIsAssociatedEmptyEditorApplicationIdentifier()
        {
            var core = new ApplicationCore();
            core.UserIsAssociated(this.UserApplication(Guid.Empty), this.App(Guid.NewGuid()));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void UserIsAssociatedEmptyApplicationIdentifier()
        {
            var core = new ApplicationCore();
            core.UserIsAssociated(this.UserApplication(Guid.NewGuid()), this.App(Guid.Empty));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SaveApplicationInformationNullEditorApplication()
        {
            var core = new ApplicationCore();
            var editor = this.UserApplication(Guid.NewGuid());
            editor.Application = null;
            core.Save(this.ApplicationInfo().Convert(), editor, this.ApplicationInfo().Convert());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SaveApplicationInformationNullEditorUser()
        {
            var core = new ApplicationCore();
            var editor = this.UserApplication(Guid.NewGuid());
            editor.User = null;
            core.Save(this.ApplicationInfo().Convert(), editor, this.ApplicationInfo().Convert());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SaveApplicationInformationEmptyEditorApplicationIdentifier()
        {
            var core = new ApplicationCore();
            var editor = this.UserApplication(Guid.NewGuid());
            editor.Application.Identifier = Guid.Empty;
            core.Save(this.ApplicationInfo().Convert(), editor, this.ApplicationInfo().Convert());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SaveApplicationInformationEmptyEditorUserIdentifier()
        {
            var core = new ApplicationCore();
            var editor = this.UserApplication(Guid.NewGuid());
            editor.User.Identifier = Guid.Empty;
            core.Save(this.ApplicationInfo().Convert(), editor, this.ApplicationInfo().Convert());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SaveUserApplicationNullEditorApplication()
        {
            var core = new ApplicationCore();
            var editor = this.UserApplication(Guid.NewGuid());
            editor.Application = null;
            core.Save(this.UserApplication(Guid.NewGuid()), editor);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SaveUserApplicationNullEditorUser()
        {
            var core = new ApplicationCore();
            var editor = this.UserApplication(Guid.NewGuid());
            editor.User = null;
            core.Save(this.UserApplication(Guid.NewGuid()), editor);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SaveUserApplicationEmptyEditorApplicationIdentifier()
        {
            var core = new ApplicationCore();
            var editor = this.UserApplication(Guid.NewGuid());
            editor.Application.Identifier = Guid.Empty;
            core.Save(this.UserApplication(Guid.NewGuid()), editor);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SaveUserApplicationEmptyEditorUserIdentifier()
        {
            var core = new ApplicationCore();
            var editor = this.UserApplication(Guid.NewGuid());
            editor.User.Identifier = Guid.Empty;
            core.Save(this.UserApplication(Guid.NewGuid()), editor);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ApplicationCountNullUser()
        {
            var core = new ApplicationCore();
            core.ApplicationCount(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ApplicationCountEmptyUserId()
        {
            var u = new User()
            {
                Identifier = Guid.Empty,
            };
            var core = new ApplicationCore();
            core.ApplicationCount(u);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SaveApplicationInformationNullData()
        {
            var core = new ApplicationCore();
            var u = new User()
            {
                Identifier = Guid.NewGuid(),
            };
            var app = new Application()
            {
                Identifier = Guid.NewGuid(),
            };
            var editor = new UserApplication()
            {
                User = u,
                Application = app,
            };
            var userApp = new ApplicationInformation()
            {
                Identifier = Guid.NewGuid(),
            };
            core.Save((ApplicationInformation)null, editor, userApp);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SaveApplicationInformationNullUser()
        {
            var core = new ApplicationCore();
            var u = new User()
            {
                Identifier = Guid.NewGuid(),
            };
            var userApp = new ApplicationInformation()
            {
                Identifier = Guid.NewGuid(),
            };
            var info = this.Information();
            core.Save(info, null, userApp);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SaveApplicationInformationEmptyInfoAppId()
        {
            var core = new ApplicationCore();
            var u = new User()
            {
                Identifier = Guid.NewGuid(),
            };
            var app = new Application()
            {
                Identifier = Guid.NewGuid(),
            };
            var editor = new UserApplication()
            {
                User = u,
                Application = app,
            };
            var userApp = new ApplicationInformation()
            {
                Identifier = Guid.NewGuid(),
            };
            var info = this.Information();
            info.Identifier = Guid.Empty;
            core.Save(info, editor, userApp);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SaveApplicationInformationEmptyUserId()
        {
            var core = new ApplicationCore();
            var editor = this.UserApplication(Guid.NewGuid());
            editor.User.Identifier = Guid.Empty;
            var userApp = new ApplicationInformation()
            {
                Identifier = Guid.NewGuid(),
            };
            var info = this.Information();
            core.Save(info, editor, userApp);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SaveApplicationInformationEmptyAppId()
        {
            var core = new ApplicationCore();
            var u = new User()
            {
                Identifier = Guid.NewGuid(),
            };
            var app = new Application()
            {
                Identifier = Guid.NewGuid(),
            };
            var editor = new UserApplication()
            {
                User = u,
                Application = app,
            };
            var userApp = new ApplicationInformation()
            {
                Identifier = Guid.Empty,
            };
            var info = this.Information();
            core.Save(info, editor, userApp);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetApplicationInformationNull()
        {
            var core = new ApplicationCore();
            core.Get((ApplicationInformation)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetApplicationInformationEmptyInfoAppId()
        {
            var core = new ApplicationCore();
            var info = this.Information();
            info.Identifier = Guid.Empty;
            core.Get(info);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SaveUserApplicationNullUserApplication()
        {
            var core = new ApplicationCore();
            core.Save((UserApplication)null, this.UserApplication(Guid.NewGuid()));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SaveUserApplicationDataEmptyAppId()
        {
            var core = new ApplicationCore();
            var user = new User()
            {
                Identifier = Guid.NewGuid(),
            };
            var u = new User()
            {
                Identifier = Guid.NewGuid(),
            };
            var app = new Application()
            {
                Identifier = Guid.NewGuid(),
            };
            var editor = new UserApplication()
            {
                User = u,
                Application = app,
            };
            var application = new Abc.Services.Contracts.Application()
            {
                Identifier = Guid.Empty,
            };

            var ua = new UserApplication()
            {
                Application = application,
                User = user,
            };
            core.Save(ua, editor);
        }

        [TestMethod]
        [ExpectedException(typeof(SecurityException))]
        public void SaveUserApplicationInvalidEditor()
        {
            var core = new ApplicationCore();
            var user = new User()
            {
                Identifier = Guid.NewGuid(),
            };
            var u = new User()
            {
                Identifier = Guid.NewGuid(),
            };
            var app = new Application()
            {
                Identifier = Guid.NewGuid(),
            };
            var editor = new UserApplication()
            {
                User = u,
                Application = app,
            };
            var application = new Abc.Services.Contracts.Application()
            {
                Identifier = Guid.NewGuid(),
            };

            var ua = new UserApplication()
            {
                Application = application,
                User = user,
            };
            core.Save(ua, editor);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetUserApplicationDataNull()
        {
            var core = new ApplicationCore();
            core.Get((UserApplication)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetUserApplicationNull()
        {
            var core = new ApplicationCore();
            core.Get((Application)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetUserApplicationEmptyIdentifier()
        {
            var core = new ApplicationCore();
            core.Get(new Application());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetApplicationInformationNullUser()
        {
            var core = new ApplicationCore();
            core.Get(null, Guid.NewGuid());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetApplicationInformationEmptyUserId()
        {
            var core = new ApplicationCore();
            core.Get(new User(), Guid.NewGuid());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetApplicationInformationEmptyApplicationId()
        {
            var core = new ApplicationCore();
            var user = new User()
            {
                Identifier = Guid.NewGuid()
            };

            core.Get(user, Guid.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(SecurityException))]
        public void SaveConfiguration()
        {
            var token = new Token()
            {
                ApplicationId = Guid.NewGuid(),
                ValidationKey = StringHelper.ValidString(),
            };

            var config = new Abc.Services.Contracts.Configuration()
            {
                Key = StringHelper.ValidString(256),
                Token = token,
                Value = StringHelper.ValidString(256),
            };

            var u = new Abc.Services.Contracts.User()
            {
                Identifier = Guid.NewGuid(),
            };
            var a = new Abc.Services.Contracts.Application()
            {
                Identifier = token.ApplicationId,
            };
            var editor = new Abc.Services.Contracts.UserApplication()
            {
                User = u,
                Application = a,
            };

            var core = new ApplicationCore();
            core.Save(config, editor);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PermitApplicationCreationNullApplication()
        {
            var user = new User()
            {
                Identifier = Guid.NewGuid(),
            };

            var core = new ApplicationCore();
            core.PermitApplicationCreation(null, user);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void PermitApplicationCreationEmptyApplicationIdentifier()
        {
            var user = new User()
            {
                Identifier = Guid.NewGuid(),
            };

            var app = new Application()
            {
                Identifier = Guid.Empty,
            };

            var core = new ApplicationCore();
            core.PermitApplicationCreation(app, user);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PermitApplicationCreationNullUser()
        {
            var app = new Application()
            {
                Identifier = Guid.NewGuid(),
            };

            var core = new ApplicationCore();
            core.PermitApplicationCreation(app, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void PermitApplicationCreationEmptyUserIdentifier()
        {
            var user = new User()
            {
                Identifier = Guid.Empty,
            };

            var app = new Application()
            {
                Identifier = Guid.NewGuid(),
            };

            var core = new ApplicationCore();
            core.PermitApplicationCreation(app, user);
        }
        #endregion

        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new ApplicationCore();
        }

        [TestMethod]
        public void Applications()
        {
            var appCore = new ApplicationCore();
            var apps = appCore.Applications();

            var table = new AzureTable<ApplicationInfoData>(ServerConfiguration.Default);
            var applications = (from data in table.QueryByPartition(ApplicationInfoData.Key)
                               select data).ToList();

            Assert.AreEqual<int>(applications.Count, apps.Count());
            var app = apps.First();
            var exists = (from data in applications
                          where data.ApplicationId == app.Identifier
                          select data).FirstOrDefault();

            Assert.IsNotNull(exists);
        }

        [TestMethod]
        public void GetUsers()
        {
            var table = new AzureTable<UserData>(CloudStorageAccount.DevelopmentStorageAccount);
            var user = table.QueryByPartition(Settings.ApplicationIdentifier.ToString()).FirstOrDefault();
            if (null == user)
            {
                user = new UserData(string.Format("{0}@temp.com", Guid.NewGuid()), "na", Guid.NewGuid().ToBase64());
                table.AddEntity(user);
            }

            var companyTable = new AzureTable<CompanyRow>(CloudStorageAccount.DevelopmentStorageAccount);
            var companyRow = new CompanyRow(user.Id)
            {
                Name = Guid.NewGuid().ToBase64(),   
            };

            companyTable.AddEntity(companyRow);
            var roleTable = new AzureTable<RoleRow>(CloudStorageAccount.DevelopmentStorageAccount);
            var roleRow = new RoleRow(Settings.ApplicationIdentifier)
            {
                UserIdentifier = user.Id,
                Name = Guid.NewGuid().ToBase64(),
            };
            roleTable.AddEntity(roleRow);

            var core = new ApplicationCore();
            var users = core.GetUsers(Application.Current, false);
            Assert.IsNotNull(users);
            Assert.IsTrue(0 < users.Count());
            var returned = (from data in users
                           where data.Identifier == user.Id
                           select data).FirstOrDefault();

            Assert.IsNotNull(returned);
            Assert.AreEqual<Guid>(user.Id, returned.Identifier);
            Assert.AreEqual<string>(user.UserName, returned.UserName);
            Assert.IsNull(returned.Companies);
            Assert.IsNull(returned.Roles);
        }

        [TestMethod]
        public void GetUsersDeepLoad()
        {
            var table = new AzureTable<UserData>(CloudStorageAccount.DevelopmentStorageAccount);
            var user = table.QueryByPartition(Settings.ApplicationIdentifier.ToString()).FirstOrDefault();
            if (null == user)
            {
                user = new UserData(string.Format("{0}@temp.com", Guid.NewGuid()), "na", Guid.NewGuid().ToBase64());
                table.AddEntity(user);
            }

            var companyTable = new AzureTable<CompanyRow>(CloudStorageAccount.DevelopmentStorageAccount);
            var companyRow = new CompanyRow(user.Id)
            {
                Name = Guid.NewGuid().ToBase64(),
            };

            companyTable.AddEntity(companyRow);
            var roleTable = new AzureTable<RoleRow>(CloudStorageAccount.DevelopmentStorageAccount);
            var roleRow = new RoleRow(Settings.ApplicationIdentifier)
            {
                UserIdentifier = user.Id,
                Name = Guid.NewGuid().ToBase64(),
            };
            roleTable.AddEntity(roleRow);

            var core = new ApplicationCore();
            var users = core.GetUsers(Application.Current, true);
            Assert.IsNotNull(users);
            Assert.IsTrue(0 < users.Count());
            var returned = (from data in users
                            where data.Identifier == user.Id
                                && data.Companies != null
                                && data.Roles != null
                            select data).FirstOrDefault();

            Assert.IsNotNull(returned);
            Assert.AreEqual<Guid>(user.Id, returned.Identifier);
            Assert.AreEqual<string>(user.UserName, returned.UserName);
            Assert.IsNotNull(returned.Companies);
            var company = (from data in returned.Companies
                          where data.Identifier == companyRow.Identifier
                          select data).First();
            Assert.AreEqual<Guid>(companyRow.Identifier, company.Identifier);
            Assert.AreEqual<string>(companyRow.Name, company.Name);
            Assert.IsNotNull(returned.Roles);

            var role = (from data in returned.Roles
                           where data == roleRow.Name
                           select data).First();
            Assert.AreEqual<string>(roleRow.Name, role);
        }

        [TestMethod]
        public void ApplicationInfoRoundTrip()
        {
            var table = new AzureTable<UserData>(CloudStorageAccount.DevelopmentStorageAccount);
            var user = new UserData(string.Format("{0}@temp.com", Guid.NewGuid()), "na", Guid.NewGuid().ToString());
            table.AddEntity(user);

            var core = new ApplicationCore();
            var data = this.Information();

            data.Identifier = Abc.Underpinning.Application.Identifier;
            var u = new User()
            {
                Identifier = user.Id,
            };
            var app = new Application()
            {
                Identifier = Guid.NewGuid(),
            };
            var editor = new UserApplication()
            {
                User = u,
                Application = app,
            };
            var userApp = new ApplicationInformation()
            {
                Identifier = Application.Default.Identifier,
            };
            var saved = core.Save(data, editor, userApp);
            Assert.IsNotNull(saved);
            Assert.AreEqual<string>(data.Description, saved.Description);
            Assert.AreEqual<string>(data.Name, saved.Name);
            Assert.AreEqual<Guid>(data.Identifier, saved.Identifier);
        }

        [TestMethod]
        public void UserApplicationRoundTrip()
        {
            var user = new UserData(StringHelper.ValidString(), StringHelper.ValidString(), StringHelper.ValidString());
            var e = new UserData(StringHelper.ValidString(), StringHelper.ValidString(), StringHelper.ValidString());
            e.EmailValidated = true;
            e.RoleValue = (int)RoleType.Manager;
            var userTable = new AzureTable<UserData>(CloudStorageAccount.DevelopmentStorageAccount);
            userTable.AddEntity(new[] { e, user });
            var userApp = new UserApplicationData(user.Id, user.ApplicationId)
            {
                Active = true
            };
            var table = new AzureTable<UserApplicationData>(CloudStorageAccount.DevelopmentStorageAccount);
            table.AddEntity(userApp);

            var core = new ApplicationCore();
            var data = this.UserApplication(userApp.ApplicationId);
            var editor = this.UserApplication(userApp.ApplicationId);
            editor.User = e.Convert();
            
            var saved = core.Save(data, editor);

            Assert.AreEqual<Guid>(data.Application.Identifier, saved.Application.Identifier);
            Assert.AreEqual<Guid>(data.User.Identifier, saved.User.Identifier);

            var get = core.Get(data);
            Assert.AreEqual<Guid>(data.Application.Identifier, get.Application.Identifier);
            Assert.AreEqual<Guid>(data.User.Identifier, get.User.Identifier);
        }

        [TestMethod]
        public void GetConfigurationItem()
        {
            var user = new UserData(StringHelper.ValidString(), StringHelper.ValidString(), StringHelper.ValidString());
            var userTable = new AzureTable<UserData>(CloudStorageAccount.DevelopmentStorageAccount);
            userTable.AddEntity(user);
            var userApp = new UserApplicationData(user.Id, user.ApplicationId)
            {
                Active = true
            };
            var table = new AzureTable<UserApplicationData>(CloudStorageAccount.DevelopmentStorageAccount);
            table.AddEntity(userApp);

            var core = new ApplicationCore();
            var config = this.Config(userApp.ApplicationId);
            var u = new Abc.Services.Contracts.User()
            {
                Identifier = userApp.UserId,
            };
            var a = new Abc.Services.Contracts.Application()
            {
                Identifier = user.ApplicationId,
            };
            var editor = new Abc.Services.Contracts.UserApplication()
            {
                User = u,
                Application = a,
            };
            core.Save(config, editor);

            var query = new Abc.Services.Contracts.Configuration()
            {
                Token = config.Token,
                Key = config.Key,
            };

            var returned = core.Get(query);
            Assert.IsNotNull(returned);
            Assert.AreEqual<int>(1, returned.Count());
            var item = returned.ToArray()[0];
            Assert.AreEqual<string>(config.Key, item.Key);
            Assert.AreEqual<string>(config.Value, item.Value);
        }

        [TestMethod]
        public void GetConfigurationItems()
        {
            var user = new UserData(StringHelper.ValidString(), StringHelper.ValidString(), StringHelper.ValidString());
            var userTable = new AzureTable<UserData>(CloudStorageAccount.DevelopmentStorageAccount);
            userTable.AddEntity(user);
            var userApp = new UserApplicationData(user.Id, user.ApplicationId)
            {
                Active = true
            };
            var table = new AzureTable<UserApplicationData>(CloudStorageAccount.DevelopmentStorageAccount);
            table.AddEntity(userApp);

            var core = new ApplicationCore();
            var config1 = this.Config(userApp.ApplicationId);

            var config2 = this.Config(userApp.ApplicationId);
            var u = new Abc.Services.Contracts.User()
            {
                Identifier = userApp.UserId,
            };
            var a = new Abc.Services.Contracts.Application()
            {
                Identifier = user.ApplicationId,
            };
            var editor = new Abc.Services.Contracts.UserApplication()
            {
                User = u,
                Application = a,
            };

            core.Save(config1, editor);
            core.Save(config2, editor);

            var query = new Abc.Services.Contracts.Configuration()
            {
                Token = config1.Token,
            };

            var returned = core.Get(query);
            Assert.IsNotNull(returned);
            var item1 = (from data in returned
                         where data.Key == config1.Key
                         && data.Value == config1.Value
                         select data).First();
            Assert.AreEqual<string>(config1.Key, item1.Key);
            Assert.AreEqual<string>(config1.Value, item1.Value);

            var item2 = (from data in returned
                         where data.Key == config2.Key
                         && data.Value == config2.Value
                         select data).First();
            Assert.AreEqual<string>(config2.Key, item2.Key);
            Assert.AreEqual<string>(config2.Value, item2.Value);
        }

        [TestMethod]
        public void SaveConfigurationNullsUserAndToken()
        {
            var user = new UserData(StringHelper.ValidString(), StringHelper.ValidString(), StringHelper.ValidString());
            var userTable = new AzureTable<UserData>(CloudStorageAccount.DevelopmentStorageAccount);
            userTable.AddEntity(user);
            var userApp = new UserApplicationData(user.Id, user.ApplicationId)
            {
                Active = true
            };
            var table = new AzureTable<UserApplicationData>(CloudStorageAccount.DevelopmentStorageAccount);
            table.AddEntity(userApp);

            var core = new ApplicationCore();
            var config = this.Config(userApp.ApplicationId);

            var u = new User()
            {
                Identifier = userApp.UserId,
            };
            var a = new Application()
            {
                Identifier = user.ApplicationId,
            };
            var editor = new UserApplication()
            {
                User = u,
                Application = a,
            };
            var returned = core.Save(config, editor);
            Assert.IsNotNull(returned);
            Assert.IsNull(returned.Token);
        }

        [TestMethod]
        public void ApplicationCountZero()
        {
            var user = new User()
            {
                Identifier = Guid.NewGuid(),
            };

            var appCore = new ApplicationCore();
            var count = appCore.ApplicationCount(user);
            Assert.AreEqual<int>(0, count);
        }

        [TestMethod]
        public void ApplicationCount()
        {
            var userId = Guid.NewGuid();
            var table = new AzureTable<ApplicationInfoData>(CloudStorageAccount.DevelopmentStorageAccount);
            var random = new Random();
            var expectedCount = random.Next(15);
            for (int i = 0; i < expectedCount; i++)
            {
                var ua = new ApplicationInfoData(Guid.NewGuid())
                {
                    Owner = userId,
                };

                table.AddOrUpdateEntity(ua);
            }

            var user = new User()
            {
                Identifier = userId,
            };

            var appCore = new ApplicationCore();
            var count = appCore.ApplicationCount(user);
            Assert.AreEqual<int>(expectedCount, count);
        }

        [TestMethod]
        public void PermitApplicationCreation()
        {
            var application = Application.Default;

            var userData = new UserData(StringHelper.ValidString(), StringHelper.ValidString(), StringHelper.ValidString());
            var tbl = new AzureTable<UserData>(CloudStorageAccount.DevelopmentStorageAccount);
            tbl.AddEntity(userData);

            var user = new User()
            {
                Identifier = userData.Id,
            };

            var row = new UserPreferenceRow(application.Identifier, user.Identifier);
            var table = new AzureTable<UserPreferenceRow>(CloudStorageAccount.DevelopmentStorageAccount);
            table.AddEntity(row);
            var appCore = new ApplicationCore();
            appCore.PermitApplicationCreation(application, user);
        }
        #endregion

        #region Helper Methods
        private ApplicationInformation Information()
        {
            return new ApplicationInformation()
            {
                Active = true,
                Identifier = Guid.NewGuid(),
                Deleted = true,
                Description = StringHelper.ValidString(),
                Environment = StringHelper.ValidString(),
                IsNew = false,
                IsValid = true,
                Name = StringHelper.ValidString(),
                ValidUntil = DateTime.UtcNow.AddDays(1),
            };
        }

        private ApplicationInfoData ApplicationInfo()
        {
            return new ApplicationInfoData(Guid.NewGuid())
            {
                CreatedBy = Guid.NewGuid(),
                CreatedOn = DateTime.UtcNow,
                Description = StringHelper.ValidString(),
                LastUpdatedBy = Guid.NewGuid(),
                LastUpdatedOn = DateTime.UtcNow,
                Name = StringHelper.ValidString(),
                Environment = StringHelper.ValidString(),
            };
        }

        private Abc.Services.Contracts.Application App(Guid appId)
        {
            return new Abc.Services.Contracts.Application()
            {
                Identifier = appId,
            };
        }

        private UserApplication UserApplication(Guid appId)
        {
            var user = new User()
            {
                Identifier = Guid.NewGuid(),
            };

            return new UserApplication()
            {
                Application = this.App(appId),
                User = user,
            };
        }

        private Abc.Services.Contracts.Configuration Config(Guid appId)
        {
            var token = new Token()
            {
                ApplicationId = appId,
                ValidationKey = StringHelper.ValidString(),
            };

            return new Abc.Services.Contracts.Configuration()
            {
                Key = StringHelper.ValidString(63),
                Value = StringHelper.ValidString(),
                Token = token,
            };
        }
        #endregion
    }
}