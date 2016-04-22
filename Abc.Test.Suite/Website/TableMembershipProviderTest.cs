// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='TableMembershipProviderTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite
{
    using System;
    using System.Web.Security;
    using Abc.Services;
    using Abc.Website.Security;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    
    /// <summary>
    /// This is a test class for TableMembershipProviderTest and is intended to contain all TableMembershipProviderTest Unit Tests
    /// </summary>
    [TestClass]
    public class TableMembershipProviderTest
    {
        #region Error Cases
        /// <summary>
        /// A test for GetUserNameByEmailInvalidEmail
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetUserNameByEmailInvalidEmail()
        {
            var target = new TableMembershipProvider();
            target.GetUserNameByEmail(StringHelper.NullEmptyWhiteSpace());
        }

        /// <summary>
        /// A test for EnablePasswordRetrieval
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void EnablePasswordRetrieval()
        {
            var target = new TableMembershipProvider();
            var actual = target.EnablePasswordRetrieval;
        }

        /// <summary>
        /// A test for ApplicationName
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void ApplicationNameTestGet()
        {
            var target = new TableMembershipProvider();
            var actual = target.ApplicationName;
        }

        /// <summary>
        /// A test for ApplicationName
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void ApplicationNameTestSet()
        {
            var target = new TableMembershipProvider();
            target.ApplicationName = StringHelper.ValidString();
        }

        /// <summary>
        /// A test for EnablePasswordReset
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void EnablePasswordReset()
        {
            var target = new TableMembershipProvider();
            var actual = target.EnablePasswordReset;
        }

        /// <summary>
        /// A test for PasswordAttemptWindow
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void PasswordAttemptWindow()
        {
            var target = new TableMembershipProvider();
            var actual = target.PasswordAttemptWindow;
        }

        /// <summary>
        /// A test for PasswordFormat
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void PasswordFormat()
        {
            var target = new TableMembershipProvider();
            var actual = target.PasswordFormat;
        }

        /// <summary>
        /// A test for PasswordStrengthRegularExpression
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void PasswordStrengthRegularExpression()
        {
            var target = new TableMembershipProvider();
            var actual = target.PasswordStrengthRegularExpression;
        }

        /// <summary>
        /// A test for RequiresQuestionAndAnswer
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void RequiresQuestionAndAnswer()
        {
            var target = new TableMembershipProvider();
            var actual = target.RequiresQuestionAndAnswer;
        }

        /// <summary>
        /// A test for RequiresUniqueEmail
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void RequiresUniqueEmail()
        {
            var target = new TableMembershipProvider();
            var actual = target.RequiresUniqueEmail;
        }

        /// <summary>
        /// A test for GetUser
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void GetUserTestUserKey()
        {
            var target = new TableMembershipProvider();
            object providerUserKey = new object();
            bool userIsOnline = false;
            var actual = target.GetUser(providerUserKey, userIsOnline);
        }
        
        /// <summary>
        /// A test for FindUsersByEmail
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void FindUsersByEmail()
        {
            var random = new Random();
            var target = new TableMembershipProvider();
            string emailToMatch = StringHelper.ValidString();
            int pageIndex = random.Next();
            int pageSize = random.Next();
            int totalRecords = random.Next();
            var actual = target.FindUsersByEmail(emailToMatch, pageIndex, pageSize, out totalRecords);
        }

        /// <summary>
        /// A test for FindUsersByName
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void FindUsersByName()
        {
            var random = new Random();
            var target = new TableMembershipProvider();
            string usernameToMatch = StringHelper.ValidString();
            int pageIndex = random.Next();
            int pageSize = random.Next();
            int totalRecords = random.Next();
            var actual = target.FindUsersByName(usernameToMatch, pageIndex, pageSize, out totalRecords);
        }

        /// <summary>
        /// A test for GetAllUsers
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void GetAllUsers()
        {
            var random = new Random();
            var target = new TableMembershipProvider();
            int pageIndex = random.Next();
            int pageSize = random.Next();
            int totalRecords = random.Next();
            var actual = target.GetAllUsers(pageIndex, pageSize, out totalRecords);
        }

        /// <summary>
        /// A test for GetNumberOfUsersOnline
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void GetNumberOfUsersOnline()
        {
            var target = new TableMembershipProvider();
            var actual = target.GetNumberOfUsersOnline();
        }

        /// <summary>
        /// A test for GetPassword
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void GetPassword()
        {
            var target = new TableMembershipProvider();
            string username = StringHelper.ValidString();
            string answer = StringHelper.ValidString();
            var actual = target.GetPassword(username, answer);
        }

        /// <summary>
        /// A test for DeleteUser
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void DeleteUser()
        {
            var target = new TableMembershipProvider();
            string username = StringHelper.ValidString();
            bool deleteAllRelatedData = false;
            var actual = target.DeleteUser(username, deleteAllRelatedData);
        }

        /// <summary>
        /// A test for ChangePassword
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void ChangePassword()
        {
            var target = new TableMembershipProvider();
            string username = StringHelper.ValidString();
            string oldPassword = StringHelper.ValidString();
            string newPassword = StringHelper.ValidString();
            var actual = target.ChangePassword(username, oldPassword, newPassword);
        }

        /// <summary>
        /// A test for ChangePasswordQuestionAndAnswer
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void ChangePasswordQuestionAndAnswer()
        {
            var target = new TableMembershipProvider();
            string username = StringHelper.ValidString();
            string password = StringHelper.ValidString();
            string newPasswordQuestion = StringHelper.ValidString();
            string newPasswordAnswer = StringHelper.ValidString();
            var actual = target.ChangePasswordQuestionAndAnswer(username, password, newPasswordQuestion, newPasswordAnswer);
        }

        /// <summary>
        /// A test for ResetPassword
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void ResetPassword()
        {
            var target = new TableMembershipProvider();
            string username = StringHelper.ValidString();
            string answer = StringHelper.ValidString();
            var actual = target.ResetPassword(username, answer);
        }

        /// <summary>
        /// A test for UnlockUser
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void UnlockUser()
        {
            var target = new TableMembershipProvider();
            string userName = StringHelper.ValidString();
            var actual = target.UnlockUser(userName);
        }

        /// <summary>
        /// A test for UpdateUser
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void UpdateUser()
        {
            var target = new TableMembershipProvider();
            MembershipUser user = null;
            target.UpdateUser(user);
        }

        /// <summary>
        /// A test for MaxInvalidPasswordAttempts
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void MaxInvalidPasswordAttempts()
        {
            var target = new TableMembershipProvider();
            var actual = target.MaxInvalidPasswordAttempts;
        }

        /// <summary>
        /// A test for GetUser
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetUserInvalid()
        {
            var target = new TableMembershipProvider();
            string username = StringHelper.NullEmptyWhiteSpace();
            bool userIsOnline = false;
            var actual = target.GetUser(username, userIsOnline);
            Assert.IsNull(actual);
        }
        #endregion

        #region Valid Cases
        [TestMethod]
        public void DetermineRoleTypeInvalidEmail()
        {
            Assert.AreEqual<RoleType>(RoleType.Client, TableMembershipProvider.DetermineRoleType(StringHelper.NullEmptyWhiteSpace()));
        }

        [TestMethod]
        public void DetermineRoleTypeClient()
        {
            Assert.AreEqual<RoleType>(RoleType.Client, TableMembershipProvider.DetermineRoleType(StringHelper.ValidString()));
        }

        [TestMethod]
        public void DetermineRoleTypeManager()
        {
            Assert.AreEqual<RoleType>(RoleType.Manager, TableMembershipProvider.DetermineRoleType("john.doe@agilebusinesscloud.com"));
        }

        /// <summary>
        /// A test for TableMembershipProvider Constructor
        /// </summary>
        [TestMethod]
        public void TableMembershipProviderConstructor()
        {
            var target = new TableMembershipProvider();
            Assert.IsNotNull(target);
        }

        /// <summary>
        /// A test for TableMembershipProvider Constructor
        /// </summary>
        [TestMethod]
        public void ProviderName()
        {
            Assert.AreEqual<string>("ABCAzureMembershipProvider", TableMembershipProvider.ProviderName);
        }

        /// <summary>
        /// A test for GetUser
        /// </summary>
        [TestMethod]
        public void GetUserUnknown()
        {
            var target = new TableMembershipProvider();
            string username = StringHelper.ValidString();
            bool userIsOnline = false;
            var actual = target.GetUser(username, userIsOnline);
            Assert.IsNull(actual);
        }

        /// <summary>
        /// A test for ValidateUser
        /// </summary>
        [TestMethod]
        public void ValidateUser()
        {
            var target = new TableMembershipProvider();
            string username = StringHelper.ValidString();
            string password = StringHelper.ValidString();
            bool expected = false;
            var actual = target.ValidateUser(username, password);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for MinRequiredNonAlphanumericCharacters
        /// </summary>
        [TestMethod]
        public void MinRequiredNonAlphanumericCharacters()
        {
            var target = new TableMembershipProvider();
            int actual = target.MinRequiredNonAlphanumericCharacters;
            Assert.AreEqual<int>(0, actual);
        }

        /// <summary>
        /// A test for MinRequiredPasswordLength
        /// </summary>
        [TestMethod]
        public void MinRequiredPasswordLength()
        {
            var target = new TableMembershipProvider();
            int actual = target.MinRequiredPasswordLength;
            Assert.AreEqual<int>(0, actual);
        }
        #endregion
    }
}