// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='AzureRoleProvider.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Website.Security
{
    using System.Configuration.Provider;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Web.Security;
    using Abc.Azure;
    using Abc.Configuration;
    using Abc.Services;
    using Abc.Services.Core;
    using Abc.Services.Data;

    /// <summary>
    /// Azure Role Provider
    /// </summary>
    public class AzureRoleProvider : RoleProvider
    {
        #region Members
        /// <summary>
        /// Staff Role
        /// </summary>
        public const string StaffRole = "staff";

        /// <summary>
        /// Member Role
        /// </summary>
        public const string MemberRole = "member";

        /// <summary>
        /// Role Table
        /// </summary>
        private static readonly AzureTable<RoleRow> roleTable = new AzureTable<RoleRow>(ServerConfiguration.Default, new RoleRowValidator());

        /// <summary>
        /// User Table
        /// </summary>
        private static readonly AzureTable<UserData> userTable = new AzureTable<UserData>(ServerConfiguration.Default);

        /// <summary>
        /// Log Core
        /// </summary>
        private static readonly LogCore logger = new LogCore();
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets ApplicationName
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Unsure of affects")]
        public override string ApplicationName
        {
            get
            {
                return ServerConfiguration.ApplicationIdentifier.ToString().ToLowerInvariant();
            }

            set
            {
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Get All Roles
        /// </summary>
        /// <returns>All Roles</returns>
        public override string[] GetAllRoles()
        {
            using (new PerformanceMonitor())
            {
                return new string[] { StaffRole, MemberRole };
            }
        }

        /// <summary>
        /// Get Roles For User
        /// </summary>
        /// <param name="email">Email</param>
        /// <returns>User's Roles</returns>
        [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", Justification = "StyleCop issue as it thinks parameter is hungarian notation.")]
        public override string[] GetRolesForUser(string email)
        {
            using (new PerformanceMonitor())
            {
                var user = (from data in userTable.QueryByPartition(this.ApplicationName)
                            where data.Email == email
                            select data).FirstOrDefault();

                string[] roles = null;
                if (null != user)
                {
                    var userRoles = from data in roleTable.QueryByPartition(this.ApplicationName)
                                    where data.UserIdentifier == user.Id
                                    select data;

                    return userRoles.ToList().AsParallel().Select(u => u.Name).ToArray();
                }
                else
                {
                    logger.Log("User not found when Get Roles for User was called.".FormatWithCulture(email));
                }

                return roles;
            }
        }

        /// <summary>
        /// Is User In Role
        /// </summary>
        /// <param name="email">Email</param>
        /// <param name="roleName">Role Name</param>
        /// <returns>User is in Role</returns>
        [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", Justification = "StyleCop issue as it thinks parameter is hungarian notation.")]
        public override bool IsUserInRole(string email, string roleName)
        {
            using (new PerformanceMonitor())
            {
                var user = (from data in userTable.QueryByPartition(this.ApplicationName)
                            where data.Email == email
                            select data).FirstOrDefault();

                if (null != user)
                {
                    var role = (from data in roleTable.QueryByPartition(this.ApplicationName)
                                where data.UserIdentifier == user.Id
                                && data.Name == roleName
                                select data).FirstOrDefault();

                    return null != role;
                }
                else
                {
                    logger.Log("User not found when Is User In Role was called.".FormatWithCulture(email));
                }

                return false;
            }
        }

        /// <summary>
        /// Role Exists
        /// </summary>
        /// <param name="roleName">Role Name</param>
        /// <returns>Exists</returns>
        public override bool RoleExists(string roleName)
        {
            using (new PerformanceMonitor())
            {
                foreach (var role in this.GetAllRoles())
                {
                    if (0 == string.CompareOrdinal(role, roleName))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Add User To Role
        /// </summary>
        /// <param name="email">Email</param>
        /// <param name="roleName">Role Name</param>
        public void AddUserToRole(string email, string roleName)
        {
            using (new PerformanceMonitor())
            {
                this.AddUsersToRoles(new string[] { email }, new string[] { roleName });
            }
        }

        /// <summary>
        /// Add Users To Roles
        /// </summary>
        /// <param name="emails">Email</param>
        /// <param name="roleNames">Role Names</param>
        [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", Justification = "StyleCop issue as it thinks parameter is hungarian notation.")]
        public override void AddUsersToRoles(string[] emails, string[] roleNames)
        {
            using (new PerformanceMonitor())
            {
                if (roleNames.Any(rolename => !this.RoleExists(rolename)))
                {
                    throw new ProviderException("Role name not found.");
                }
                else if (emails.Any(email => roleNames.Any(rolename => this.IsUserInRole(email, rolename))))
                {
                    throw new ProviderException("User is already in role.");
                }
                else
                {
                    foreach (var email in emails)
                    {
                        var user = (from data in userTable.QueryByPartition(this.ApplicationName)
                                    where data.Email == email
                                    select data).FirstOrDefault();

                        foreach (var roleName in roleNames)
                        {
                            if (!this.IsUserInRole(email, roleName))
                            {
                                var role = new RoleRow(ServerConfiguration.ApplicationIdentifier)
                                {
                                    Name = roleName,
                                    UserIdentifier = user.Id,
                                };

                                roleTable.AddEntity(role);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Remove Users From Roles
        /// </summary>
        /// <param name="userName">User Name</param>
        /// <param name="roleName">Role Name</param>
        public void RemoveUserFromRole(string userName, string roleName)
        {
            using (new PerformanceMonitor())
            {
                this.RemoveUsersFromRoles(new string[] { userName }, new string[] { roleName });
            }
        }

        /// <summary>
        /// Remove Users From Roles
        /// </summary>
        /// <param name="usernames">User Names</param>
        /// <param name="roleNames">Role Names</param>
        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            using (new PerformanceMonitor())
            {
                if (roleNames.Any(rolename => !this.RoleExists(rolename)))
                {
                    throw new ProviderException("Role name not found.");
                }
                else if (usernames.Any(username => roleNames.Any(rolename => !this.IsUserInRole(username, rolename))))
                {
                    throw new ProviderException("User is not already in role.");
                }
                else
                {
                    foreach (var userName in usernames)
                    {
                        var user = (from data in userTable.QueryByPartition(this.ApplicationName)
                                    where data.UserName == userName
                                    select data).FirstOrDefault();

                        foreach (var roleName in roleNames)
                        {
                            if (!this.IsUserInRole(userName, roleName))
                            {
                                var roles = from data in roleTable.QueryByPartition(ServerConfiguration.ApplicationIdentifier.ToString())
                                            where data.Name == roleName
                                            && data.UserIdentifier == user.Id
                                            select data;

                                roleTable.DeleteEntity(roles);
                            }
                        }
                    }
                }
            }
        }

        #region Not Implemented
        /// <summary>
        /// Create Role
        /// </summary>
        /// <param name="roleName">Role Name</param>
        public override void CreateRole(string roleName)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Delete Role
        /// </summary>
        /// <param name="roleName">Role Name</param>
        /// <param name="throwOnPopulatedRole">Throw On Populated Role</param>
        /// <returns>Deleted Role</returns>
        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Find Users In Role
        /// </summary>
        /// <param name="roleName">Role Name</param>
        /// <param name="usernameToMatch">User Name To Match</param>
        /// <returns>User's in Role</returns>
        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Get Users In Role
        /// </summary>
        /// <param name="roleName">Role Name</param>
        /// <returns>User's In Role</returns>
        public override string[] GetUsersInRole(string roleName)
        {
            throw new System.NotImplementedException();
        }
        #endregion
        #endregion
    }
}