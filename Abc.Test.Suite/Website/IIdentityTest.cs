// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='IIdentityTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Website
{
    using System.Security.Principal;
    using Abc.Website;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class IIdentityTest
    {
        #region Valid Cases
        [TestMethod]
        public void IsManagerNotAuthenticated()
        {
            var identity = new Identity()
            {
                IsAuthenticated = false,
            };

            Assert.IsFalse(identity.IsManager());
        }

        [TestMethod]
        public void UserDataNotAuthenticated()
        {
            var identity = new Identity()
            {
                IsAuthenticated = false,
            };

            Assert.IsNull(identity.Data());
        }
        #endregion

        #region Classes
        private class Identity : IIdentity
        {
            public string AuthenticationType
            {
                get;
                set;
            }

            public bool IsAuthenticated
            {
                get;
                set;
            }

            public string Name
            {
                get;
                set;
            }
        }
        #endregion
    }
}