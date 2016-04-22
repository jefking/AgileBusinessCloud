// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='UserTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Contracts
{
    using Abc.Services;
    using Abc.Services.Contracts;
    using Abc.Services.Data;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// User Test
    /// </summary>
    [TestClass]
    public class UserTest
    {
        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new User();
        }

        [TestMethod]
        public void IsSecured()
        {
            Assert.IsNotNull(new User() as Secured);
        }

        [TestMethod]
        public void IsILoadIEnumerableRoleRow()
        {
            Assert.IsNotNull(new User() as ILoad<IEnumerable<RoleRow>>);
        }

        [TestMethod]
        public void IsIIdentifierGuid()
        {
            Assert.IsNotNull(new User() as IIdentifier<Guid>);
        }

        [TestMethod]
        public void IsIConvertUserPublicProfile()
        {
            Assert.IsNotNull(new User() as IConvert<UserPublicProfile>);
        }

        [TestMethod]
        public void Identifier()
        {
            var id = Guid.NewGuid();
            var user = new User();
            user.Identifier = id;

            Assert.AreEqual<Guid>(id, user.Identifier);
        }

        [TestMethod]
        public void Roles()
        {
            var data = new string[] { Guid.NewGuid().ToAscii85(), Guid.NewGuid().ToAscii85() };
            var user = new User();
            user.Roles = data;

            Assert.AreEqual<string[]>(data, user.Roles);
        }

        [TestMethod]
        public void Companies()
        {
            var data = new Company[] { new Company(), new Company() };
            var user = new User();
            user.Companies = data;

            Assert.AreEqual<Company[]>(data, user.Companies);
        }

        [TestMethod]
        public void Email()
        {
            var email = Guid.NewGuid().ToString();
            var user = new User()
            {
                Email = email,
            };

            Assert.AreEqual<string>(email, user.Email);
        }

        [TestMethod]
        public void CreatedOn()
        {
            var data = DateTime.Now;
            var user = new User()
            {
                CreatedOn = data,
            };

            Assert.AreEqual<DateTime>(data, user.CreatedOn);
        }

        [TestMethod]
        public void UserName()
        {
            var name = StringHelper.ValidString();
            var user = new User()
            {
                UserName = name,
            };

            Assert.AreEqual<string>(name, user.UserName);
        }

        [TestMethod]
        public void LoadRoleRow()
        {
            var token = new Token()
            {
                ApplicationId = Guid.NewGuid(),
            };
            var user = new User()
            {
                Token = token,
                Identifier = Guid.NewGuid(),
            };
            var random = new Random();
            var count = random.Next(50);
            var roles = new List<Abc.Services.Data.RoleRow>(count);
            for (int i = 0; i < count; i++)
            {
                var role = new Abc.Services.Data.RoleRow(Guid.NewGuid())
                {
                    Name = Guid.NewGuid().ToBase64()
                };

                roles.Add(role);
            }

            var userRole = new Abc.Services.Data.RoleRow(token.ApplicationId)
            {
                Name = Guid.NewGuid().ToBase64(),
                UserIdentifier = user.Identifier,
            };
            roles.Add(userRole);

            user.Load(roles);

            Assert.IsNotNull(user.Roles);
            Assert.AreEqual<int>(1, user.Roles.Length);
            var appliedRole = user.Roles[0];
            Assert.AreEqual<string>(userRole.Name, appliedRole);
        }

        [TestMethod]
        public void Convert()
        {
            var item = new User()
            {
                CreatedOn = DateTime.UtcNow,
                UserName = StringHelper.ValidString(),
                Email = StringHelper.ValidString(),
            };

            var converted = item.Convert();
            Assert.IsNotNull(converted);
            Assert.AreEqual<DateTime>(item.CreatedOn, converted.CreatedOn);
            Assert.AreEqual<string>(item.UserName, converted.UserName);
            Assert.IsFalse(string.IsNullOrWhiteSpace(converted.Gravatar));
            Assert.AreEqual<string>(item.Email.GetHexMD5(), converted.Gravatar);
        }
        #endregion
    }
}