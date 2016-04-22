// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='UserApplication.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Contracts
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Runtime.Serialization;

    /// <summary>
    /// User Application
    /// </summary>
    [DataContract]
    public class UserApplication : IConvert<UserApplicationData>, IDeleted, IUser, IApplication
    {
        #region Properties
        /// <summary>
        /// Gets or sets Application
        /// </summary>
        [DataMember]
        public Application Application
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets User
        /// </summary>
        [DataMember]
        public User User
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether Active
        /// </summary>
        [DataMember]
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Deleted
        /// </summary>
        [DataMember]
        public bool Deleted { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Converts Application User to User Application Data
        /// </summary>
        /// <returns>User Application Data</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly", Justification = "Information for logging")]
        [CLSCompliant(false)]
        public UserApplicationData Convert()
        {
            if (null == this.User)
            {
                throw new ArgumentNullException();
            }
            else if (null == this.Application)
            {
                throw new ArgumentNullException();
            }
            else
            {
                return new UserApplicationData(this.User.Identifier, this.Application.Identifier)
                {
                    Active = this.Active,
                    Deleted = this.Deleted,
                };
            }
        }
        #endregion
    }
}