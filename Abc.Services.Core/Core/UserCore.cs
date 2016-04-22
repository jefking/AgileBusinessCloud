// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='UserCore.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Core
{
    using Abc.Azure;
    using Abc.Configuration;
    using Abc.Services.Contracts;
    using Abc.Services.Data;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    /// User Core
    /// </summary>
    public class UserCore : DataCore, IUserCore
    {
        #region Members
        /// <summary>
        /// User Table
        /// </summary>
        private readonly AzureTable<UserData> userTable = new AzureTable<UserData>(ServerConfiguration.Default, new UserDataValidator());
        #endregion

        #region Methods
        /// <summary>
        /// Get Profile Page
        /// </summary>
        /// <param name="page">Page</param>
        /// <returns>Profile Page</returns>
        public ProfilePage Get(ProfilePage page)
        {
            Contract.Requires<ArgumentNullException>(null != page);
            Contract.Requires<ArgumentException>(Guid.Empty != page.ApplicationIdentifier);
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(page.Handle));

            var row = page.Convert();

            var table = new AzureTable<UserProfileRow>(ServerConfiguration.Default);

            var exists = table.QueryBy(row.PartitionKey, row.RowKey);
            return null == exists ? null : exists.Convert();
        }

        /// <summary>
        /// Get Profile Page
        /// </summary>
        /// <param name="application">Page</param>
        /// <returns>Profile Pages</returns>
        public IEnumerable<ProfilePage> PublicProfiles(Application application)
        {
            Contract.Requires<ArgumentNullException>(null != application);
            Contract.Requires<ArgumentException>(Guid.Empty != application.Identifier);

            var table = new AzureTable<UserProfileRow>(ServerConfiguration.Default);

            var rows = from data in table.QueryByPartition(application.Identifier.ToString())
                         select data;

            return null == rows ? null : rows.ToList().Select(r => r.Convert());
        }

        /// <summary>
        /// Public Profiles Full
        /// </summary>
        /// <param name="application">Application</param>
        /// <returns>Full Public Profiles</returns>
        public IEnumerable<UserPublicProfile> PublicProfilesFull(Application application, bool withPreferences = true)
        {
            Contract.Requires<ArgumentNullException>(null != application);
            Contract.Requires<ArgumentException>(Guid.Empty != application.Identifier);

            using (new PerformanceMonitor())
            {
                IList<UserPublicProfile> publicProfiles = null;
                var profiles = this.PublicProfiles(application);
                if (null != profiles)
                {
                    publicProfiles = new List<UserPublicProfile>(profiles.Count());
                    foreach (var profile in profiles)
                    {
                        var user = new User()
                        {
                            Identifier = profile.OwnerIdentifier,
                        };

                        var userApp = new UserApplication()
                        {
                            Application = application,
                            User = user,
                        };

                        user = this.GetByIdentifier(userApp);
                        if (null != user)
                        {
                            var publicProfile = user.Convert();
                            publicProfile.Handle = profile.Handle;
                            publicProfile.PreferedProfile = profile.PreferedProfile;
                            publicProfile.Points = profile.Points;
                            publicProfile.Word = profile.Word;
                            publicProfile.OwnerIdentifier = profile.OwnerIdentifier;

                            var preference = new UserPreference()
                            {
                                Application = application,
                                User = user,
                            };

                            if (withPreferences)
                            {
                                preference = this.Get(preference);

                                if (null != preference)
                                {
                                    publicProfile.TwitterHandle = preference.TwitterHandle;
                                    publicProfile.GitHubHandle = preference.GitHubHandle;
                                }
                            }

                            publicProfiles.Add(publicProfile);
                        }
                    }
                }

                return publicProfiles;
            }
        }

        /// <summary>
        /// Get User
        /// </summary>
        /// <param name="userApp">User Application</param>
        /// <returns>User</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "By Design")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Code contracts")]
        public User Get(UserApplication userApp)
        {
            Contract.Requires<ArgumentNullException>(null != userApp);
            Contract.Requires<ArgumentNullException>(null != userApp.Application);
            Contract.Requires<ArgumentNullException>(null != userApp.User);
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != userApp.Application.Identifier);
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != userApp.User.Identifier);

            var table = new AzureTable<UserData>(ServerConfiguration.Default);
            var userData = table.QueryBy(userApp.Application.Identifier.ToString(), userApp.User.Identifier.ToString());

            return userData == null ? (User)null : userData.Convert();
        }

        /// <summary>
        /// Get User
        /// </summary>
        /// <param name="userApp">User Application</param>
        /// <returns>User</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "By Design")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Code contracts")]
        public User GetByEmail(UserApplication userApp)
        {
            Contract.Requires<ArgumentNullException>(null != userApp);
            Contract.Requires<ArgumentNullException>(null != userApp.Application);
            Contract.Requires<ArgumentNullException>(null != userApp.User);
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != userApp.Application.Identifier);
            Contract.Requires<ArgumentOutOfRangeException>(!string.IsNullOrWhiteSpace(userApp.User.Email));

            var source = new DomainSource();
            var userData = source.GetUserByEmail(userApp.Application.Identifier, userApp.User.Email);

            return userData == null ? (User)null : userData.Convert();
        }

        /// <summary>
        /// Get User
        /// </summary>
        /// <param name="userApp">User Application</param>
        /// <returns>User</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "By Design")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Code contracts")]
        public User GetByNameIdentifier(UserApplication userApp)
        {
            Contract.Requires<ArgumentNullException>(null != userApp);
            Contract.Requires<ArgumentNullException>(null != userApp.Application);
            Contract.Requires<ArgumentNullException>(null != userApp.User);
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != userApp.Application.Identifier);
            Contract.Requires<ArgumentOutOfRangeException>(!string.IsNullOrWhiteSpace(userApp.User.NameIdentifier));

            var source = new DomainSource();
            var userData = source.GetUserByNameIdentifier(userApp.Application.Identifier, userApp.User.NameIdentifier);

            return userData == null ? (User)null : userData.Convert();
        }

        /// <summary>
        /// Get User
        /// </summary>
        /// <param name="userApp">User Application</param>
        /// <returns>User</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "By Design")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Code contracts")]
        public User GetByIdentifier(UserApplication userApp)
        {
            Contract.Requires<ArgumentNullException>(null != userApp);
            Contract.Requires<ArgumentNullException>(null != userApp.Application);
            Contract.Requires<ArgumentNullException>(null != userApp.User);
            Contract.Requires<ArgumentException>(Guid.Empty != userApp.Application.Identifier);
            Contract.Requires<ArgumentException>(Guid.Empty != userApp.User.Identifier);

            var source = new DomainSource();
            var userData = source.GetUserById(userApp.Application.Identifier, userApp.User.Identifier);

            return userData == null ? (User)null : userData.Convert();
        }

        /// <summary>
        /// Get User Preference
        /// </summary>
        /// <param name="userPreference">User Preference</param>
        /// <returns>User Preference Full</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "By Design")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Code contracts")]
        public UserPreference Get(UserPreference userPreference)
        {
            Contract.Requires<ArgumentNullException>(null != userPreference);
            Contract.Requires<ArgumentNullException>(null != userPreference.Application);
            Contract.Requires<ArgumentNullException>(null != userPreference.User);
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != userPreference.Application.Identifier);
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != userPreference.User.Identifier);

            var table = new AzureTable<UserPreferenceRow>(ServerConfiguration.Default);
            var exists = table.QueryBy(userPreference.Application.Identifier.ToString(), userPreference.User.Identifier.ToString());
            if (null == exists)
            {
                userPreference.MaximumAllowedApplications = 1;

                return this.Save(userPreference);
            }
            else
            {
                return exists.Convert();
            }
        }

        /// <summary>
        /// Save User Preference
        /// </summary>
        /// <param name="userPreference">User Preference</param>
        /// <returns>Saved User Preference</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Code contracts")]
        public UserPreference Save(UserPreference userPreference)
        {
            Contract.Requires<ArgumentNullException>(null != userPreference);
            Contract.Requires<ArgumentNullException>(null != userPreference.Application);
            Contract.Requires<ArgumentNullException>(null != userPreference.User);
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != userPreference.Application.Identifier);
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != userPreference.User.Identifier);

            using (new PerformanceMonitor())
            {
                var table = new AzureTable<UserPreferenceRow>(ServerConfiguration.Default);
                var exists = table.QueryBy(userPreference.Application.Identifier.ToString(), userPreference.User.Identifier.ToString());

                if (null == exists)
                {
                    table.AddEntity(userPreference.Convert());
                }
                else
                {
                    if (null != userPreference.CurrentApplication && Guid.Empty != userPreference.CurrentApplication.Identifier)
                    {
                        exists.CurrentApplicationIdentifier = userPreference.CurrentApplication.Identifier;
                    }

                    if (null != userPreference.TimeZone)
                    {
                        exists.TimeZone = userPreference.TimeZone.ToSerializedString();
                    }

                    if (!string.IsNullOrWhiteSpace(userPreference.TwitterHandle))
                    {
                        exists.TwitterHandle = userPreference.TwitterHandle;
                    }

                    if (!string.IsNullOrWhiteSpace(userPreference.GitHubHandle))
                    {
                        exists.GitHubHandle = userPreference.GitHubHandle;
                    }

                    if (!string.IsNullOrWhiteSpace(userPreference.AbcHandle))
                    {
                        userPreference.AbcHandle = Regex.Replace(userPreference.AbcHandle, @"\s", string.Empty);
                        if (!string.IsNullOrWhiteSpace(userPreference.AbcHandle))
                        {
                            if (exists.AbcHandle != userPreference.AbcHandle)
                            {
                                var profile = new ProfilePage()
                                {
                                    ApplicationIdentifier = userPreference.Application.Identifier,
                                    OwnerIdentifier = userPreference.User.Identifier,
                                    ExistingHandle = exists.AbcHandle,
                                    Handle = userPreference.AbcHandle,
                                };

                                try
                                {
                                    profile = this.Save(profile);

                                    exists.AbcHandle = profile.Handle;
                                }
                                catch (Exception ex)
                                {
                                    Logging.Log(ex, EventTypes.Warning, (int)ServiceFault.CannotCreateProfile);
                                }
                            }
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(userPreference.City))
                    {
                        exists.City = userPreference.City;
                    }

                    if (!string.IsNullOrWhiteSpace(userPreference.Country))
                    {
                        exists.Country = userPreference.Country;
                    }

                    exists.MaxiumAllowedApplications = userPreference.MaximumAllowedApplications ?? exists.MaxiumAllowedApplications ?? 1;

                    table.AddOrUpdateEntity(exists);
                }

                return this.Get(userPreference);
            }
        }

        /// <summary>
        /// Get Contacts
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>Contacts</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "By Design")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Code contracts")]
        public IEnumerable<Contact> GetContacts(User user)
        {
            Contract.Requires<ArgumentNullException>(null != user);
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != user.Identifier);

            using (new PerformanceMonitor())
            {
                var table = new AzureTable<ContactRow>(ServerConfiguration.Default);
                var results = table.QueryByPartition(user.Identifier.ToString());
                return results.ToList().Select(u => u.Convert());
            }
        }

        /// <summary>
        /// Get Contact Groups
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>Groups</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "By Design")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Code contracts")]
        public IEnumerable<ContactGroup> GetGroups(User user)
        {
            Contract.Requires<ArgumentNullException>(null != user);
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != user.Identifier);

            using (new PerformanceMonitor())
            {
                var table = new AzureTable<ContactGroupRow>(ServerConfiguration.Default);
                var results = table.QueryByPartition(user.Identifier.ToString());
                return results.ToList().AsParallel().Select(u => u.Convert());
            }
        }

        /// <summary>
        /// Save Contact
        /// </summary>
        /// <param name="contact">Contact to save</param>
        /// <returns>Saved Contact</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "By Design")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Code contracts")]
        public Contact Save(Contact contact)
        {
            Contract.Requires<ArgumentNullException>(null != contact);
            Contract.Requires<ArgumentNullException>(null != contact.Owner);
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != contact.Identifier);
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != contact.Owner.Identifier);

            using (new PerformanceMonitor())
            {
                var row = contact.Convert();
                var table = new AzureTable<ContactRow>(ServerConfiguration.Default, new ContactRowValidator());
                var data = table.QueryBy(row.PartitionKey, row.RowKey);

                if (null == data)
                {
                    table.AddEntity(row);
                }
                else
                {
                    data.Email = row.Email;
                    table.AddOrUpdateEntity(data);

                    row = data;
                }

                return row.Convert();
            }
        }

        /// <summary>
        /// Save Profile Page
        /// </summary>
        /// <param name="page">Page</param>
        /// <returns>Profile Page</returns>
        public ProfilePage Save(ProfilePage page)
        {
            Contract.Requires<ArgumentNullException>(null != page);
            Contract.Requires<ArgumentException>(Guid.Empty != page.ApplicationIdentifier);
            Contract.Requires<ArgumentException>(Guid.Empty != page.OwnerIdentifier);
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(page.Handle));

            var row = page.Convert();

            var table = new AzureTable<UserProfileRow>(ServerConfiguration.Default);
            UserProfileRow oldProfile = null;
            UserProfileRow newProfile = null;

            try
            {
                oldProfile = table.QueryBy(row.PartitionKey, page.ExistingHandle.ToLowerInvariant());
            }
            catch (Exception ex)
            {
                Logging.Log(ex, EventTypes.Warning, (int)ServiceFault.ProfileDoesntExist);
            }

            try
            {
                newProfile = table.QueryBy(row.PartitionKey, page.Handle.ToLowerInvariant());
            }
            catch (Exception ex)
            {
                Logging.Log(ex, EventTypes.Warning, (int)ServiceFault.ProfileDoesntExist);
            }

            if (null == newProfile)
            {
                table.AddEntity(row);
            }
            else if (newProfile.OwnerIdentifier == row.OwnerIdentifier)
            {
                table.AddOrUpdateEntity(row);
            }
            else
            {
                throw new InvalidOperationException("User doesn't own profile");
            }

            if (null != oldProfile)
            {
                if (oldProfile.OwnerIdentifier == row.OwnerIdentifier)
                {
                    table.DeleteBy(oldProfile.PartitionKey, oldProfile.RowKey);
                }
                else
                {
                    throw new InvalidOperationException("User doesn't own profile");
                }
            }

            return page;
        }

        /// <summary>
        /// Save Contact Group
        /// </summary>
        /// <param name="contact">Contact Group to save</param>
        /// <returns>Saved Group</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "By Design")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Code contracts")]
        public ContactGroup Save(ContactGroup contact)
        {
            Contract.Requires<ArgumentNullException>(null != contact);
            Contract.Requires<ArgumentNullException>(null != contact.Owner);
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != contact.Identifier);
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != contact.Owner.Identifier);

            using (new PerformanceMonitor())
            {
                var row = contact.Convert();
                var table = new AzureTable<ContactGroupRow>(ServerConfiguration.Default, new ContactGroupRowValidator());
                var data = table.QueryBy(row.PartitionKey, row.RowKey);

                if (null == data)
                {
                    table.AddEntity(row);
                }
                else
                {
                    data.Name = row.Name;
                    table.AddOrUpdateEntity(data);

                    row = data;
                }

                return row.Convert();
            }
        }

        /// <summary>
        /// Validate Email
        /// </summary>
        /// <param name="app">Application</param>
        /// <param name="userId">User Identifier</param>
        /// <returns>Email Address</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Abc.Services.Core.LogCore.Log(System.String)", Justification = "Not a localization issue.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Code contracts")]
        public string SetRoles(Application app, Guid userId)
        {
            Contract.Requires<ArgumentNullException>(null != app);
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != app.Identifier);
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != userId);

            using (new PerformanceMonitor())
            {
                var userInfo = this.userTable.QueryBy(app.Identifier.ToString(), userId.ToString());

                if (null != userInfo)
                {
                    IList<RoleRow> roles = new List<RoleRow>(2);
                    if (userInfo.Email.ToUpperInvariant().EndsWith("@agilebusinesscloud.com".ToUpperInvariant()))
                    {
                        userInfo.RoleValue = (int)RoleType.Manager;
                        var staff = new RoleRow(app.Identifier)
                        {
                            UserIdentifier = userInfo.Id,
                            Name = "staff",
                        };

                        roles.Add(staff);
                    }

                    var member = new RoleRow(app.Identifier)
                    {
                        UserIdentifier = userInfo.Id,
                        Name = "member",
                    };

                    roles.Add(member);

                    var roleTable = new AzureTable<RoleRow>(ServerConfiguration.Default, new RoleRowValidator());
                    roleTable.AddEntity(roles);

                    var preference = new UserPreference()
                    {
                        Application = app,
                        User = userInfo.Convert(),
                        MaximumAllowedApplications = 1,
                    };

                    var userCore = new UserCore();
                    userCore.Save(preference);

                    userInfo.EmailValidated = true;
                    userInfo.LastActivityOn = DateTime.UtcNow;
                    this.userTable.AddOrUpdateEntity(userInfo);

                    return userInfo.Email;
                }
                else
                {
                    Logging.Log("Email validation key not found.");
                }

                return null;
            }
        }

        public void Save(User user, string tribe)
        {
            if (null == user)
            {
                throw new ArgumentNullException("user");
            }

            if (string.IsNullOrWhiteSpace(tribe))
            {
                throw new ArgumentException("tribe");
            }

            if (Guid.Empty == user.Identifier)
            {
                throw new ArgumentException();
            }

            var table = new AzureTable<UserTribesRow>(ServerConfiguration.Default);
            var tribeRow = new UserTribesRow()
            {
                PartitionKey = tribe,
                RowKey = user.Identifier.ToString(),
            };
            var userRow = new UserTribesRow()
            {
                PartitionKey = user.Identifier.ToString(),
                RowKey = tribe,
            };
        }
        #endregion
    }
}