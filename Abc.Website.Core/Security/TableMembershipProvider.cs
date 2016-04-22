// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='TableMembershipProvider.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Website.Security
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Web.Security;
    using Abc.Configuration;
    using Abc.Services;
    using Abc.Services.Contracts;
    using Abc.Services.Core;

    /// <summary>
    /// Table Membership Provider
    /// </summary>
    public class TableMembershipProvider : MembershipProvider
    {
        #region Members
        /// <summary>
        /// Provider Name
        /// </summary>
        public const string ProviderName = "ABCAzureMembershipProvider";

        /// <summary>
        /// Domain Data Source
        /// </summary>
        private static readonly DomainSource source = new DomainSource();

        /// <summary>
        /// Logger
        /// </summary>
        private static readonly LogCore logger = new LogCore();
        #endregion

        #region Properties
        /// <summary>
        /// Gets MinRequiredNonAlphanumericCharacters
        /// </summary>
        public override int MinRequiredNonAlphanumericCharacters
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets MinRequiredPasswordLength
        /// </summary>
        public override int MinRequiredPasswordLength
        {
            get
            {
                return 0;
            }
        }

        #region Not Implemented
        /// <summary>
        /// Gets MaxInvalidPasswordAttempts
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "Not Implemented.")]
        public override int MaxInvalidPasswordAttempts
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets or sets ApplicationName
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "Not Implemented.")]
        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets a value indicating whether EnablePasswordReset
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "Not Implemented.")]
        public override bool EnablePasswordReset
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets a value indicating whether EnablePasswordRetrieval
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "Not Implemented.")]
        public override bool EnablePasswordRetrieval
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets PasswordAttemptWindow
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "Not Implemented.")]
        public override int PasswordAttemptWindow
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets PasswordFormat
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "Not Implemented.")]
        public override MembershipPasswordFormat PasswordFormat
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets PasswordStrengthRegularExpression
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "Not Implemented.")]
        public override string PasswordStrengthRegularExpression
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets a value indicating whether RequiresQuestionAndAnswer
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "Not Implemented.")]
        public override bool RequiresQuestionAndAnswer
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets a value indicating whether RequiresUniqueEmail
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "Not Implemented.")]
        public override bool RequiresUniqueEmail
        {
            get { throw new NotImplementedException(); }
        }
        #endregion
        #endregion

        #region Methods
        /// <summary>
        /// Determine Role Type
        /// </summary>
        /// <param name="email">Email</param>
        /// <returns>Role Type</returns>
        public static RoleType DetermineRoleType(string email)
        {
            return !string.IsNullOrWhiteSpace(email) && email.ToUpperInvariant().EndsWith("@agilebusinesscloud.com", System.StringComparison.OrdinalIgnoreCase) ? RoleType.Manager : RoleType.Client;
        }

        /// <summary>
        /// Create User
        /// </summary>
        /// <param name="username">User Name</param>
        /// <param name="password">Password</param>
        /// <param name="email">Email</param>
        /// <param name="passwordQuestion">Password Question</param>
        /// <param name="passwordAnswer">Password Answer</param>
        /// <param name="approved">Is Approved</param>
        /// <param name="providerUserKey">Provider User Key</param>
        /// <param name="status">Status</param>
        /// <returns>Membership User</returns>
        [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", Justification = "StyleCop issue as it thinks parameter is hungarian notation.")]
        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool approved, object providerUserKey, out MembershipCreateStatus status)
        {
            using (new PerformanceMonitor())
            {
                MembershipUser membership = null;
                status = MembershipCreateStatus.UserRejected;

                UserData userValidation = null;
                if (!string.IsNullOrWhiteSpace(email))
                {
                    userValidation = source.GetUserByEmail(Application.Default.Identifier, email);
                    if (null != userValidation)
                    {
                        status = MembershipCreateStatus.DuplicateEmail;
                    }
                }

                if (!string.IsNullOrWhiteSpace(username))
                {
                    userValidation = source.GetUserByEmail(Application.Default.Identifier, username);
                    if (null != userValidation)
                    {
                        status = MembershipCreateStatus.DuplicateUserName;
                    }
                }

                if (null == userValidation)
                {
                    userValidation = source.GetUserByNameIdentifier(Application.Default.Identifier, providerUserKey.ToString());
                    if (null != userValidation)
                    {
                        status = MembershipCreateStatus.DuplicateProviderUserKey;
                    }
                    else
                    {
                        var user = new UserData(email, providerUserKey.ToString(), username)
                        {
                            RoleValue = (int)TableMembershipProvider.DetermineRoleType(email),
                        };

                        source.Insert(user);

                        var returnedUser = source.GetUserById(Application.Default.Identifier, user.Id);
                        if (null == returnedUser)
                        {
                            status = MembershipCreateStatus.ProviderError;
                        }
                        else if (!returnedUser.IsApproved)
                        {
                            status = MembershipCreateStatus.UserRejected;
                        }
                        else if (returnedUser.IsLockedOut)
                        {
                            status = MembershipCreateStatus.UserRejected;
                        }
                        else
                        {
                            status = MembershipCreateStatus.Success;
                            membership = new MembershipUser(ProviderName, returnedUser.UserName, returnedUser.NameIdentifier, returnedUser.Email, passwordQuestion, string.Empty, returnedUser.IsApproved, returnedUser.IsLockedOut, returnedUser.CreatedOn, returnedUser.LastLoggedInOn, returnedUser.LastActivityOn, returnedUser.PasswordLastChangedOn, returnedUser.LastLockedOutOn);

                            logger.Log("New user signed up.");
                        }
                    }
                }

                return membership;
            }
        }

        /// <summary>
        /// Get User
        /// </summary>
        /// <param name="username">Open Id</param>
        /// <param name="userIsOnline">User Is Online</param>
        /// <returns>Membership User</returns>
        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            using (new PerformanceMonitor())
            {
                var user = source.GetUserByUserName(Application.Default.Identifier, username);

                MembershipUser member = null;
                if (null != user)
                {
                    if (!user.IsApproved)
                    {
                        throw new InvalidOperationException("User is not approved.");
                    }
                    else if (user.IsLockedOut)
                    {
                        throw new InvalidOperationException("User is locked out.");
                    }
                    else
                    {
                        member = new MembershipUser(ProviderName, user.UserName, user.NameIdentifier, user.Email, null, null, user.IsApproved, user.IsLockedOut, user.CreatedOn, user.LastLoggedInOn, user.LastActivityOn, user.PasswordLastChangedOn, user.LastLockedOutOn);

                        user.LastLoggedInOn = DateTime.UtcNow;
                    }

                    user.LastActivityOn = DateTime.UtcNow;

                    source.Update(user);
                }
                else
                {
                    logger.Log("User was not found.");
                }

                return member;
            }
        }

        /// <summary>
        /// Get User Name By Email
        /// </summary>
        /// <param name="email">email</param>
        /// <returns>User Name</returns>
        public override string GetUserNameByEmail(string email)
        {
            Contract.Requires<ArgumentOutOfRangeException>(!string.IsNullOrWhiteSpace(email));

            using (new PerformanceMonitor())
            {
                var user = source.GetUserByEmail(Application.Default.Identifier, email);

                MembershipUser member = null;
                if (null != user)
                {
                    if (!user.IsApproved)
                    {
                        throw new InvalidOperationException("User is not approved.");
                    }
                    else if (user.IsLockedOut)
                    {
                        throw new InvalidOperationException("User is locked out.");
                    }
                    else
                    {
                        member = new MembershipUser(ProviderName, user.UserName, user.NameIdentifier, user.Email, null, null, user.IsApproved, user.IsLockedOut, user.CreatedOn, user.LastLoggedInOn, user.LastActivityOn, user.PasswordLastChangedOn, user.LastLockedOutOn);

                        user.LastLoggedInOn = DateTime.UtcNow;
                    }

                    user.LastActivityOn = DateTime.UtcNow;

                    source.Update(user);

                    return member.UserName;
                }
                else
                {
                    logger.Log("User was not found.");

                    throw new InvalidOperationException("Unknown user.");
                }
            }
        }

        #region Not Implemented
        /// <summary>
        /// Get User
        /// </summary>
        /// <param name="providerUserKey">Provider User Key</param>
        /// <param name="userIsOnline">User Is Online</param>
        /// <returns>Membership User</returns>
        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Find Users By Email
        /// </summary>
        /// <param name="emailToMatch">Email To Match</param>
        /// <param name="pageIndex">Page Index</param>
        /// <param name="pageSize">Page Size</param>
        /// <param name="totalRecords">Total Records</param>
        /// <returns>Membership User Collection</returns>
        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Find Users By Name
        /// </summary>
        /// <param name="usernameToMatch">Username To Match</param>
        /// <param name="pageIndex">Page Index</param>
        /// <param name="pageSize">Page Size</param>
        /// <param name="totalRecords">Total Records</param>
        /// <returns>Membership User Collection</returns>
        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get All Users
        /// </summary>
        /// <param name="pageIndex">Page Index</param>
        /// <param name="pageSize">Page Size</param>
        /// <param name="totalRecords">Total Records</param>
        /// <returns>Membership User Collection</returns>
        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get Number of Users Online
        /// </summary>
        /// <returns>Number of Users Online</returns>
        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get Password
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="answer">Answer</param>
        /// <returns>Password</returns>
        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Delete User
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="deleteAllRelatedData">Delete All Related Data</param>
        /// <returns>Deleted</returns>
        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Change Password
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="oldPassword">Old Password</param>
        /// <param name="newPassword">New Password</param>
        /// <returns>Changed</returns>
        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Change Password Question and Answer
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <param name="newPasswordQuestion">New Password Question</param>
        /// <param name="newPasswordAnswer">New Password Answer</param>
        /// <returns>Changed</returns>
        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Reset Password
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="answer">Answer</param>
        /// <returns>Password</returns>
        public override string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Unlock User
        /// </summary>
        /// <param name="userName">Username</param>
        /// <returns>Unlocked</returns>
        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Update User
        /// </summary>
        /// <param name="user">Membership User</param>
        public override void UpdateUser(MembershipUser user)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Validate User
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns>Validated</returns>
        public override bool ValidateUser(string username, string password)
        {
            return false;
        }
        #endregion
        #endregion
    }
}