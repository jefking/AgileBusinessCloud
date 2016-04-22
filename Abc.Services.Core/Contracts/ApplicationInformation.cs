// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='ApplicationInformation.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Contracts
{
    using System;
    using System.Runtime.Serialization;
    using Abc.Underpinning.Administration;

    /// <summary>
    /// Application Information
    /// </summary>
    [DataContract]
    public sealed class ApplicationInformation : Application, ILoad<Details>, IConvert<Details>, IConvert<ApplicationInfoData>, IDeleted
    {
        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether Active
        /// </summary>
        [DataMember]
        public bool Active
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether Is New
        /// </summary>
        [DataMember]
        public bool IsNew
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether Deleted
        /// </summary>
        [DataMember]
        public bool Deleted
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Name
        /// </summary>
        [DataMember]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Environment
        /// </summary>
        [DataMember]
        public string Environment
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Description
        /// </summary>
        [DataMember]
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Owner Id
        /// </summary>
        [DataMember]
        public Guid OwnerId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether Is Valid
        /// </summary>
        [DataMember]
        public bool IsValid
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Valid Until
        /// </summary>
        [DataMember]
        public DateTime ValidUntil
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Public Key
        /// </summary>
        [DataMember]
        public string PublicKey
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the Display Text
        /// </summary>
        public string DisplayText
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.Name) && !string.IsNullOrWhiteSpace(this.Environment))
                {
                    return "{0} in {1}".FormatWithCulture(this.Name, this.Environment);
                }
                else if (!string.IsNullOrWhiteSpace(this.Name))
                {
                    return this.Name;
                }
                else
                {
                    return this.Identifier.ToString();
                }
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Load Details from source
        /// </summary>
        /// <param name="data">Details to Load</param>
        public void Load(Details data)
        {
            if (null != data)
            {
                if (Guid.Empty == this.Identifier)
                {
                    this.Identifier = data.ApplicationId;
                }
                
                if (this.Identifier == data.ApplicationId)
                {
                    this.IsValid = data.IsValid;
                    this.ValidUntil = data.ValidUntil;
                }
            }
        }

        /// <summary>
        /// Convert
        /// </summary>
        /// <returns>Details</returns>
        Details IConvert<Details>.Convert()
        {
            return new Details()
            {
                IsValid = this.IsValid,
                ValidUntil = this.ValidUntil,
                ApplicationId = this.Identifier,
            };
        }

        /// <summary>
        /// Convert
        /// </summary>
        /// <returns>Application Info Data</returns>
        ApplicationInfoData IConvert<ApplicationInfoData>.Convert()
        {
            return new ApplicationInfoData(this.Identifier)
            {
                Active = this.Active,
                Deleted = this.Deleted,
                Description = this.Description,
                Environment = this.Environment,
                Name = this.Name,
                Owner = this.OwnerId,
                PublicKey = this.PublicKey,
            };
        }
        #endregion
    }
}