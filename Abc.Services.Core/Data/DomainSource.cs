// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='DomainSource.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using Abc.Azure;
    using Abc.Configuration;

    /// <summary>
    /// Domain Source
    /// </summary>
    public sealed class DomainSource : DataSource
    {
        #region Members
        /// <summary>
        /// User Table
        /// </summary>
        private readonly AzureTable<UserData> userTable;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the DomainSource class.
        /// </summary>
        public DomainSource()
        {
            this.userTable = new AzureTable<UserData>(ServerConfiguration.Default, new UserDataValidator());
        }
        #endregion

        #region Methods
        /// <summary>
        /// Insert User Data
        /// </summary>
        /// <param name="data">New User</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Code Contracts"), CLSCompliant(false)]
        public void Insert(UserData data)
        {
            Contract.Requires<ArgumentNullException>(null != data);
            Contract.Requires<ArgumentOutOfRangeException>(!string.IsNullOrWhiteSpace(data.RowKey));
            Contract.Requires<ArgumentOutOfRangeException>(!string.IsNullOrWhiteSpace(data.PartitionKey));

            this.userTable.AddEntity(data);
        }

        /// <summary>
        /// Update User
        /// </summary>
        /// <param name="data">Data</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Code Contracts"), CLSCompliant(false)]
        public void Update(UserData data)
        {
            Contract.Requires<ArgumentNullException>(null != data);
            Contract.Requires<ArgumentOutOfRangeException>(!string.IsNullOrWhiteSpace(data.RowKey));
            Contract.Requires<ArgumentOutOfRangeException>(!string.IsNullOrWhiteSpace(data.PartitionKey));

            this.userTable.AddOrUpdateEntity(data);
        }

        /// <summary>
        /// Get User By Open Id
        /// </summary>
        /// <param name="applicationId">Application Identifier</param>
        /// <param name="nameIdentifier">Open Id</param>
        /// <returns>User Data</returns>
        [CLSCompliant(false)]
        public UserData GetUserByNameIdentifier(Guid applicationId, string nameIdentifier)
        {
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != applicationId);
            Contract.Requires<ArgumentOutOfRangeException>(!string.IsNullOrWhiteSpace(nameIdentifier));

            var appId = applicationId.ToString();
            var results = from data in this.userTable.QueryByPartition(appId)
                          where data.NameIdentifier == nameIdentifier
                          select data;

            return results.FirstOrDefault();
        }

        /// <summary>
        /// Get User By Identifier
        /// </summary>
        /// <param name="applicationId">Application Identifier</param>
        /// <param name="id">Identifier</param>
        /// <returns>User Data</returns>
        [CLSCompliant(false)]
        public UserData GetUserById(Guid applicationId, Guid id)
        {
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != applicationId);
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != id);

            var appId = applicationId.ToString();
            var identifier = id.ToString();
            return this.userTable.QueryBy(appId, identifier);
        }

        /// <summary>
        /// Get User By Email
        /// </summary>
        /// <param name="applicationId">Application Identifier</param>
        /// <param name="email">Email</param>
        /// <returns>User Data</returns>
        [CLSCompliant(false)]
        public UserData GetUserByEmail(Guid applicationId, string email)
        {
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != applicationId);
            Contract.Requires<ArgumentOutOfRangeException>(!string.IsNullOrWhiteSpace(email));

            string appId = applicationId.ToString();
            var results = from data in this.userTable.QueryByPartition(appId)
                          where data.Email == email
                          select data;

            return results.FirstOrDefault();
        }

        /// <summary>
        /// Get User By User Name
        /// </summary>
        /// <param name="applicationId">Application Identifier</param>
        /// <param name="userName">User Name</param>
        /// <returns>User Data</returns>
        [CLSCompliant(false)]
        public UserData GetUserByUserName(Guid applicationId, string userName)
        {
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != applicationId);
            Contract.Requires<ArgumentOutOfRangeException>(!string.IsNullOrWhiteSpace(userName));

            string appId = applicationId.ToString();
            var results = from data in this.userTable.QueryByPartition(appId)
                          where data.UserName == userName
                          select data;

            return results.FirstOrDefault();
        }
        #endregion
    }
}