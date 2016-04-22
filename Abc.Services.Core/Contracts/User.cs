// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='User.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using Abc.Services.Data;

    /// <summary>
    /// User
    /// </summary>
    [DataContract]
    [Serializable]
    public class User : Secured, ILoad<IEnumerable<RoleRow>>, IIdentifier<Guid>, IConvert<UserPublicProfile>
    {
        #region Properties
        /// <summary>
        /// Gets or sets Identifier
        /// </summary>
        [DataMember]
        public virtual Guid Identifier { get; set; }

        /// <summary>
        /// Gets or sets Email
        /// </summary>
        [DataMember]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets User Name
        /// </summary>
        [DataMember]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets Roles
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Api")]
        [DataMember]
        public string[] Roles { get; set; }

        /// <summary>
        /// Gets or sets Companies
        /// </summary>
        [DataMember]
        public Company[] Companies { get; set; }

        /// <summary>
        /// Gets or sets Name Identifier
        /// </summary>
        [DataMember]
        public string NameIdentifier { get; set; }

        /// <summary>
        /// Gets or sets Name CreatedOn
        /// </summary>
        [DataMember]
        public DateTime CreatedOn { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Load Role Row Data
        /// </summary>
        /// <param name="data">data</param>
        public void Load(IEnumerable<RoleRow> data)
        {
            var results = from row in data
                          where row.ApplicationId == this.Token.ApplicationId
                             && row.UserIdentifier == this.Identifier
                          select row.Name;

            if (null != results && 0 < results.Count())
            {
                this.Roles = results.ToArray();
            }
        }

        /// <summary>
        /// Convert to User Public Profile
        /// </summary>
        /// <returns></returns>
        public UserPublicProfile Convert()
        {
            return new UserPublicProfile()
            {
                CreatedOn = this.CreatedOn,
                UserName = this.UserName,
                Gravatar = string.IsNullOrWhiteSpace(this.Email) ? null : this.Email.GetHexMD5(),
            };
        }
        #endregion
    }
}