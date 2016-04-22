// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='UserData.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services
{
    using System;
    using System.Diagnostics.Contracts;
    using Abc.Azure;
    using Abc.Configuration;
    using Abc.Services.Contracts;
    using Microsoft.WindowsAzure.StorageClient;

    /// <summary>
    /// User Data
    /// </summary>
    [CLSCompliant(false)]
    [AzureDataStore("UserLogin")]
    [Serializable]
    public class UserData : TableServiceEntity, IConvert<User>
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the UserData class
        /// </summary>
        public UserData()
            : base(Application.Default.Identifier.ToString(), Guid.NewGuid().ToString())
        {
        }

        /// <summary>
        /// Initializes a new instance of the UserData class
        /// </summary>
        /// <param name="email">Email</param>
        /// <param name="nameIdentifier">Open Id</param>
        /// <param name="userName">User Name</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Code Contracts")]
        public UserData(string email, string nameIdentifier, string userName)
            : this()
        {
            Contract.Requires<ArgumentOutOfRangeException>(!string.IsNullOrWhiteSpace(nameIdentifier));

            this.Email = email;
            this.EmailValidated = true;
            this.EmailValidationKey = Guid.NewGuid().ToString();
            this.RoleValue = (int)RoleType.Client;
            this.NameIdentifier = nameIdentifier;
            this.UserName = userName;
            this.IsApproved = true;
            this.IsLockedOut = false;
            this.CreatedOn = DateTime.UtcNow;
            this.LastLoggedInOn = DateTime.UtcNow;
            this.LastActivityOn = DateTime.UtcNow;
            this.PasswordLastChangedOn = DateTime.UtcNow;
            this.LastLockedOutOn = DateTime.UtcNow.AddDays(-1);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets Identifier
        /// </summary>
        public Guid Id
        {
            get
            {
                return string.IsNullOrWhiteSpace(this.RowKey) ? Guid.Empty : Guid.Parse(this.RowKey);
            }
        }

        /// <summary>
        /// Gets Application Identifier
        /// </summary>
        public Guid ApplicationId
        {
            get
            {
                return string.IsNullOrWhiteSpace(this.PartitionKey) ? Guid.Empty : Guid.Parse(this.PartitionKey);
            }
        }

        /// <summary>
        /// Gets or sets User Name
        /// </summary>
        public string UserName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Open Id
        /// </summary>
        [Obsolete("NameIdentifier")]
        public string OpenId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Name Identifier
        /// </summary>
        public string NameIdentifier
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Email
        /// </summary>
        public string Email
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether EmailValidated
        /// </summary>
        public bool EmailValidated
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets EmailValidationKey
        /// </summary>
        public string EmailValidationKey
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Role Value
        /// </summary>
        public int RoleValue
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether Is Approved
        /// </summary>
        public bool IsApproved
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether Is Locked Out
        /// </summary>
        public bool IsLockedOut
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Created On
        /// </summary>
        public DateTime CreatedOn
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Last Logged In On
        /// </summary>
        public DateTime LastLoggedInOn
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Last Activity On
        /// </summary>
        public DateTime LastActivityOn
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Password Last Changed On
        /// </summary>
        public DateTime PasswordLastChangedOn
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Last Locked Out On
        /// </summary>
        public DateTime LastLockedOutOn
        {
            get;
            set;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Set Keys
        /// </summary>
        /// <param name="applicationIdentifier">Application Identifier</param>
        /// <param name="identifier">Identifier</param>
        public void SetKeys(Guid applicationIdentifier, Guid identifier)
        {
            Contract.Requires<ArgumentException>(Guid.Empty != applicationIdentifier);
            Contract.Requires<ArgumentException>(Guid.Empty != identifier);

            this.PartitionKey = applicationIdentifier.ToString();
            this.RowKey = identifier.ToString();
        }

        /// <summary>
        /// Convert
        /// </summary>
        /// <returns>User</returns>
        public User Convert()
        {
            return new User()
            {
                Email = this.Email,
                Identifier = this.Id,
                UserName = this.UserName,
                CreatedOn = this.CreatedOn,
            };
        }
        #endregion
    }
}