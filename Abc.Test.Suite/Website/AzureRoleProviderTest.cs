// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='AzureRoleProviderTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Website
{
    using Abc.Website.Security;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class AzureRoleProviderTest
    {
        #region Valid Cases
        [TestMethod]
        public void StaffRole()
        {
            Assert.AreEqual<string>("staff", AzureRoleProvider.StaffRole);
        }

        [TestMethod]
        public void MemberRole()
        {
            Assert.AreEqual<string>("member", AzureRoleProvider.MemberRole);
        }
        #endregion
    }
}