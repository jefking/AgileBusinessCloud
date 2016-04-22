// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='ApplicationCore.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Core
{
    using System;
    using System.Collections.Generic;
    using System.Data.Services.Client;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Security;
    using Abc.Azure;
    using Abc.Configuration;
    using Abc.Services.Contracts;
    using Abc.Services.Data;
    using Abc.Underpinning.Administration;

    /// <summary>
    /// Application Core
    /// </summary>
    public class ApplicationCore : DataCore
    {
        #region Members
        /// <summary>
        /// Application
        /// </summary>
        private static readonly Abc.Underpinning.Administration.Application app = new Abc.Underpinning.Administration.Application();

        /// <summary>
        /// User Application Cache
        /// </summary>
        private static readonly IDictionary<Guid, IList<ApplicationInformation>> userApplicationCache = new Dictionary<Guid, IList<ApplicationInformation>>();
        #endregion

        #region Methods
        /// <summary>
        /// Save
        /// </summary>
        /// <param name="data">Application Information</param>
        /// <param name="editor">User</param>
        /// <param name="application">Application</param>
        /// <returns>Saved Application Information</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Abc.Services.Core.LogCore.Log(System.String)", Justification = "Not localizing.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Code Contracts")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Code Contracts")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "2", Justification = "Code Contracts")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Part of object model")]
        [CLSCompliant(false)]
        public ApplicationInformation Save(ApplicationInformation data, UserApplication editor, Abc.Services.Contracts.Application application)
        {
            Contract.Requires<ArgumentNullException>(null != data);
            Contract.Requires<ArgumentNullException>(null != editor);
            Contract.Requires<ArgumentNullException>(null != editor.Application);
            Contract.Requires<ArgumentNullException>(null != editor.User);
            Contract.Requires<ArgumentNullException>(null != application);
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != data.Identifier);
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != editor.Application.Identifier);
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != editor.User.Identifier);
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != application.Identifier);

            Contract.Ensures(Contract.Result<ApplicationInformation>() != null);

            using (new PerformanceMonitor())
            {
                var source = new DomainSource();
                var editorData = source.GetUserById(application.Identifier, editor.User.Identifier);
                var serverDetails = ((IConvert<Details>)data).Convert();
                if (data.IsNew)
                {
                    if (this.PermitApplicationCreation(application, editor.User))
                    {
                        if (data.IsNew)
                        {
                            serverDetails.IsValid = true;
                            app.Create(serverDetails);

                            if (userApplicationCache.ContainsKey(editor.User.Identifier))
                            {
                                userApplicationCache.Remove(editor.User.Identifier);
                            }
                        }
                    }
                    else
                    {
                        Logging.Log("User tried to create an application without authorization.");
                    }
                }
                else
                {
                    if (editorData.RoleValue == (int)RoleType.Manager)
                    {
                        app.Update(serverDetails);

                        if (userApplicationCache.ContainsKey(editor.User.Identifier))
                        {
                            userApplicationCache.Remove(editor.User.Identifier);
                        }
                    }
                    else
                    {
                        Logging.Log("User tried to update an application without authorization.");
                    }
                }

                var appInfoData = ((IConvert<ApplicationInfoData>)data).Convert();
                appInfoData.Load(editorData);

                var table = new AzureTable<ApplicationInfoData>(ServerConfiguration.Default, new ApplicationInfoValidator());

                var existing = this.Get(appInfoData);

                if (null != existing)
                {
                    existing.Environment = appInfoData.Environment;
                    existing.Name = appInfoData.Name;
                    existing.Description = appInfoData.Description;
                    existing.Deleted = appInfoData.Deleted;
                    existing.Active = appInfoData.Active;
                    appInfoData = existing;
                }
                else
                {
                    appInfoData.CreatedBy = editor.User.Identifier;
                    appInfoData.Owner = editor.User.Identifier;
                    appInfoData.CreatedOn = DateTime.UtcNow;
                }

                appInfoData.LastUpdatedBy = editor.User.Identifier;
                appInfoData.LastUpdatedOn = DateTime.UtcNow;

                table.AddOrUpdateEntity(appInfoData);

                if (data.IsNew && editorData.RoleValue == (int)RoleType.Client)
                {
                    var save = new Abc.Services.Contracts.Application()
                    {
                        Identifier = serverDetails.ApplicationId,
                    };

                    var ua = new UserApplication()
                    {
                        Application = save,
                        User = editor.User,
                        Deleted = false,
                        Active = true,
                    };

                    this.Save(ua, editor);
                }

                return data;
            }
        }

        /// <summary>
        /// Save User Application Data
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="editor">Editor</param>
        /// <returns>User Application</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Code contracts")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Code contracts")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Part of object model")]
        public UserApplication Save(UserApplication data, UserApplication editor)
        {
            Contract.Requires<ArgumentNullException>(null != data);
            Contract.Requires<ArgumentNullException>(null != editor);
            Contract.Requires<ArgumentNullException>(null != editor.Application);
            Contract.Requires<ArgumentNullException>(null != editor.User);
            Contract.Requires<ArgumentNullException>(null != data.Application);
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != data.Application.Identifier);
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != editor.User.Identifier);
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != editor.Application.Identifier);

            Contract.Ensures(Contract.Result<UserApplication>() != null);

            using (new PerformanceMonitor())
            {
                if (!this.UserIsAssociated(editor, data.Application))
                {
                    throw new SecurityException("Editor is not associated with Application.");
                }

                var table = new AzureTable<UserApplicationData>(ServerConfiguration.Default, new UserApplicationValidation());

                var existing = table.QueryBy(data.User.Identifier.ToString(), data.Application.Identifier.ToString());

                if (null == existing)
                {
                    existing = data.Convert();
                    existing.CreatedOn = DateTime.UtcNow;
                    existing.LastUpdatedOn = DateTime.UtcNow;
                    existing.CreatedBy = editor.User.Identifier;
                    existing.LastUpdatedBy = editor.User.Identifier;
                    table.AddEntity(existing);
                }
                else
                {
                    var createdBy = existing.CreatedBy;
                    var createdOn = existing.CreatedOn;
                    existing = data.Convert();
                    existing.CreatedBy = createdBy;
                    existing.CreatedOn = createdOn;
                    existing.LastUpdatedOn = DateTime.UtcNow;
                    existing.LastUpdatedBy = editor.User.Identifier;

                    table.AddOrUpdateEntity(existing);
                }

                return data;
            }
        }

        /// <summary>
        /// User Is Associated to Application
        /// </summary>
        /// <param name="editor">User Application</param>
        /// <param name="application">Application</param>
        /// <returns>Already Associated</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Code contracts")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Code contracts")]
        public bool UserIsAssociated(UserApplication editor, Abc.Services.Contracts.Application application)
        {
            Contract.Requires<ArgumentNullException>(null != editor);
            Contract.Requires<ArgumentNullException>(null != application);
            Contract.Requires<ArgumentNullException>(null != editor.User);
            Contract.Requires<ArgumentNullException>(null != editor.Application);
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != editor.User.Identifier);
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != editor.Application.Identifier);
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != application.Identifier);

            using (new PerformanceMonitor())
            {
                var source = new DomainSource();
                var user = source.GetUserById(editor.Application.Identifier, editor.User.Identifier);
                if (null != user)
                {
                    if (user.RoleValue == (int)RoleType.Manager)
                    {
                        return true;
                    }
                }

                var table = new AzureTable<ApplicationInfoData>(ServerConfiguration.Default);
                var data = table.QueryBy(string.Empty, application.Identifier.ToString());
                if (null != data)
                {
                    if (data.Owner == editor.User.Identifier)
                    {
                        return true;
                    }
                }

                return this.Get(editor) != null;
            }
        }

        /// <summary>
        /// Get User Application Data
        /// </summary>
        /// <param name="data">Data</param>
        /// <returns>User Application</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Code contracts")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Part of object model")]
        public UserApplication Get(UserApplication data)
        {
            Contract.Requires<ArgumentNullException>(null != data);
            Contract.Requires<ArgumentNullException>(null != data.User);
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != data.Application.Identifier);
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != data.User.Identifier);

            using (new PerformanceMonitor())
            {
                var table = new AzureTable<UserApplicationData>(ServerConfiguration.Default);
                var entity = table.QueryBy(data.User.Identifier.ToString(), data.Application.Identifier.ToString());
                return entity == null ? null : entity.Convert();
            }
        }

        /// <summary>
        /// Get Application Information
        /// </summary>
        /// <param name="data">Application Information (Application Identifier)</param>
        /// <returns>Application Information</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "By Design")]
        public ApplicationInformation Get(Abc.Services.Contracts.Application data)
        {
            Contract.Requires<ArgumentNullException>(null != data);
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != data.Identifier);

            using (new PerformanceMonitor())
            {
                ApplicationInformation applicationInformation = null;
                var table = new AzureTable<ApplicationInfoData>(ServerConfiguration.Default);
                var info = new ApplicationInfoData(data.Identifier);
                var savedInfo = table.QueryBy(info.PartitionKey, info.RowKey);

                applicationInformation = (null == savedInfo) ? new ApplicationInformation() : savedInfo.Convert();

                var allApps = app.Search();
                var details = from item in allApps
                              where item.ApplicationId == data.Identifier
                              select item;

                applicationInformation.Load(details.FirstOrDefault());

                return applicationInformation;
            }
        }

        /// <summary>
        /// Save Configuration
        /// </summary>
        /// <param name="configuration">Configuration to save</param>
        /// <param name="editor">Editor</param>
        /// <returns>Saved Configuration</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Code contracts")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Code contracts")]
        public Configuration Save(Configuration configuration, UserApplication editor)
        {
            Contract.Requires(null != configuration);
            Contract.Requires(null != configuration.Token);
            Contract.Requires(null != editor);
            Contract.Requires(Guid.Empty != configuration.Token.ApplicationId);
            Contract.Requires(Guid.Empty != editor.User.Identifier);
            Contract.Requires(Guid.Empty != editor.Application.Identifier);
            Contract.Requires(!string.IsNullOrWhiteSpace(configuration.Value));
            Contract.Requires(!string.IsNullOrWhiteSpace(configuration.Key));

            using (new PerformanceMonitor())
            {
                var app = new Abc.Services.Contracts.Application()
                {
                    Identifier = configuration.Token.ApplicationId,
                };

                if (!this.UserIsAssociated(editor, app))
                {
                    throw new SecurityException("Editor is not associated with Application.");
                }

                var table = new AzureTable<ApplicationConfiguration>(ServerConfiguration.Default);
                var save = configuration.Convert();
                var exists = table.QueryBy(save.PartitionKey, save.RowKey);

                save.LastUpdatedBy = editor.User.Identifier;
                save.LastUpdatedOn = DateTime.UtcNow;
                if (null == exists)
                {
                    save.CreatedBy = editor.User.Identifier;
                    save.CreatedOn = DateTime.UtcNow;
                    table.AddEntity(save);
                }
                else
                {
                    save.CreatedOn = exists.CreatedOn;
                    save.CreatedBy = exists.CreatedBy;
                    table.AddOrUpdateEntity(save);
                }

                return new Configuration()
                {
                    Key = configuration.Key,
                    Value = configuration.Value,
                };
            }
        }

        /// <summary>
        /// Get Configuration
        /// </summary>
        /// <remarks>
        /// Get can get you:
        /// -A single Configuration Item
        /// -A listing of Configuration Items based off of Application Identifier
        /// </remarks>
        /// <param name="configuration">Configuration</param>
        /// <returns>Configuration Items</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "By Design")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Code contracts")]
        public ICollection<Configuration> Get(Configuration configuration)
        {
            Contract.Requires(null != configuration);
            Contract.Requires(null != configuration.Token);
            Contract.Requires(Guid.Empty != configuration.Token.ApplicationId);

            using (new PerformanceMonitor())
            {
                var partitionKey = configuration.Token.ApplicationId.ToString();
                var configurations = new List<Configuration>();
                var table = new AzureTable<ApplicationConfiguration>(ServerConfiguration.Default);
                if (string.IsNullOrWhiteSpace(configuration.Key))
                {
                    var results = table.QueryByPartition(partitionKey);
                    var items = results.ToList();
                    configurations.AddRange(items.Select(r => r.Convert()));
                }
                else
                {
                    var config = table.QueryBy(partitionKey, configuration.Key);
                    configurations.Add(config.Convert());
                }

                return configurations;
            }
        }

        /// <summary>
        /// Get Application Information for a User; Application
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="applicationId">Application Id</param>
        /// <returns>Application Information</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Complexity.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Abc.Services.Core.LogCore.Log(System.String)", Justification = "Logging")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Code contracts")]
        public IList<ApplicationInformation> Get(User user, Guid applicationId)
        {
            Contract.Requires<ArgumentNullException>(null != user);
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != user.Identifier);
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != applicationId);

            using (new PerformanceMonitor())
            {
                if (userApplicationCache.ContainsKey(user.Identifier))
                {
                    return userApplicationCache[user.Identifier];
                }
                else
                {
                    var source = new DomainSource();
                    var webUser = source.GetUserById(applicationId, user.Identifier);

                    var allAppDetails = app.Search();
                    if (null != allAppDetails)
                    {
                        IList<Details> usersAppDetails = null;
                        var role = (RoleType)webUser.RoleValue;
                        switch (role)
                        {
                            case RoleType.Client:
                                var appTable = new AzureTable<ApplicationInfoData>(ServerConfiguration.Default);
                                var userApplications = (from data in appTable.QueryByPartition(ApplicationInfoData.Key)
                                                        where data.Owner == webUser.Id
                                                        select data).ToList();

                                var table = new AzureTable<UserApplicationData>(ServerConfiguration.Default);
                                var assignedUsers = table.QueryByPartition(webUser.Id.ToString()).ToList();

                                usersAppDetails = (from all in allAppDetails
                                                   where userApplications.Any(ua => ua.ApplicationId == all.ApplicationId)
                                                      || assignedUsers.Any(ua => ua.ApplicationId == all.ApplicationId)
                                                   select all).ToList();

                                break;
                            case RoleType.Manager:
                                usersAppDetails = allAppDetails.ToList();
                                break;
                            default:
                                Logging.Log("Unknown Role Type.");
                                break;
                        }

                        if (null != usersAppDetails)
                        {
                            var infos = new List<ApplicationInformation>(usersAppDetails.Count);
                            foreach (var usersApp in usersAppDetails)
                            {
                                var info = this.Get(new ApplicationInfoData(usersApp.ApplicationId));

                                ApplicationInformation appInfo;
                                if (null == info)
                                {
                                    appInfo = new ApplicationInformation()
                                    {
                                        Identifier = usersApp.ApplicationId,
                                    };
                                }
                                else
                                {
                                    appInfo = info.Convert();
                                }

                                appInfo.Load(usersApp);
                                infos.Add(appInfo);
                            }

                            if (!userApplicationCache.ContainsKey(user.Identifier) && null != infos && 0 < infos.Count)
                            {
                                userApplicationCache.Add(user.Identifier, infos);
                            }

                            return infos;
                        }
                    }

                    return null;
                }
            }
        }

        /// <summary>
        /// Get Users, for a given application
        /// </summary>
        /// <param name="application">Application Information (Id)</param>
        /// <param name="deepLoad">Deep Load</param>
        /// <returns>Users</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Only called by C#.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Code Contracts")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "By Design")]
        public IEnumerable<User> GetUsers(Abc.Services.Contracts.Application application, bool deepLoad = false)
        {
            Contract.Requires<ArgumentNullException>(null != application);
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != application.Identifier);

            using (new PerformanceMonitor())
            {
                var table = new AzureTable<UserData>(ServerConfiguration.Default);
                var results = table.QueryByPartition(application.Identifier.ToString());
                var users = results.ToList().Select(d => d.Convert()).ToList();

                if (deepLoad)
                {
                    var companyTable = new AzureTable<CompanyRow>(ServerConfiguration.Default);
                    var roleTable = new AzureTable<RoleRow>(ServerConfiguration.Default);
                    var roles = roleTable.QueryByPartition(application.Identifier.ToString()).ToList();

                    var token = new Token()
                    {
                        ApplicationId = application.Identifier,
                    };

                    foreach (var user in users)
                    {
                        user.Token = token;

                        user.Load(roles);
                        var companies = companyTable.QueryByPartition(user.Identifier.ToString()).ToList();
                        if (null != companies && 0 < companies.Count())
                        {
                            user.Companies = companies.Select(c => c.Convert()).ToArray();
                        }
                    }
                }

                return users;
            }
        }

        /// <summary>
        /// Application Count
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>Applications User Owns</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "By Design")]
        public int ApplicationCount(User user)
        {
            Contract.Requires<ArgumentNullException>(null != user);
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != user.Identifier);

            using (new PerformanceMonitor())
            {
                var table = new AzureTable<ApplicationInfoData>(ServerConfiguration.Default);
                var results = from data in table.QueryByPartition(string.Empty)
                              where data.Owner == user.Identifier
                              select data;

                return results.ToList().Count();
            }
        }

        /// <summary>
        /// Permit Application Creation
        /// </summary>
        /// <param name="application">Application</param>
        /// <param name="user">User</param>
        /// <returns>Permitted</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Code contracts")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Code contracts")]
        public bool PermitApplicationCreation(Abc.Services.Contracts.Application application, User user)
        {
            Contract.Requires<ArgumentNullException>(null != user);
            Contract.Requires<ArgumentNullException>(null != application);
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != application.Identifier);
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != user.Identifier);

            var source = new DomainSource();
            var editorData = source.GetUserById(application.Identifier, user.Identifier);
            if (editorData.RoleValue == (int)RoleType.Manager)
            {
                return true;
            }
            else
            {
                var editorPreference = new UserPreference()
                {
                    User = user,
                    Application = application,
                };

                var userCore = new UserCore();
                editorPreference = userCore.Get(editorPreference);
 
                return editorPreference.MaximumAllowedApplications.HasValue && editorPreference.MaximumAllowedApplications.Value > this.ApplicationCount(user);
            }
        }

        /// <summary>
        /// Get all Applications
        /// </summary>
        /// <remarks>
        /// Background processing method
        /// </remarks>
        /// <returns>Application Information</returns>
        public IEnumerable<ApplicationInformation> Applications()
        {
            using (new PerformanceMonitor())
            {
                var table = new AzureTable<ApplicationInfoData>(ServerConfiguration.Default);
                var applications = from data in table.QueryByPartition(ApplicationInfoData.Key)
                                    select data;

                return applications.ToList().AsParallel().Select(a => a.Convert());
            }
        }

        /// <summary>
        /// Get Application Information Data
        /// </summary>
        /// <param name="data">Data</param>
        /// <returns>Application Information Data</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Part of object model")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Code Contracts")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Intended use")]
        private ApplicationInfoData Get(ApplicationInfoData data)
        {
            Contract.Requires<ArgumentNullException>(null != data);
            Contract.Requires<ArgumentOutOfRangeException>(!string.IsNullOrWhiteSpace(data.RowKey));
            Contract.Requires<ArgumentOutOfRangeException>(null != data.PartitionKey);
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != data.ApplicationId);

            using (new PerformanceMonitor())
            {
                var table = new AzureTable<ApplicationInfoData>(ServerConfiguration.Default);
                try
                {
                    return table.QueryBy(data.PartitionKey, data.RowKey);
                }
                catch (DataServiceQueryException)
                {
                    //Way too much logging going on.
                }

                return null;
            }
        }
        #endregion
    }
}