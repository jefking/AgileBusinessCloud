// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='ApplicationDetailsModel.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Website.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Abc.Services;
    using Abc.Services.Contracts;

    /// <summary>
    /// Application Details Model
    /// </summary>
    public class ApplicationDetailsModel : IConvert<ApplicationInformation>, IDeleted
    {
        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether Is Valid
        /// </summary>
        [Required]
        [Display(Name = "Is Valid")]
        public bool IsValid { get; set; }

        /// <summary>
        /// Gets or sets Valid Until
        /// </summary>
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Valid Until")]
        public DateTime ValidUntil { get; set; }

        /// <summary>
        /// Gets or sets a value Application Id
        /// </summary>
        [Required]
        [Display(Name = "Application Id")]
        public Guid ApplicationId { get; set; }

        /// <summary>
        /// Gets or sets Identifier
        /// </summary>
        [Display(Name = "Id")]
        public Guid Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether Active
        /// </summary>
        [Required]
        [Display(Name = "Is Active")]
        public bool Active
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether Deleted
        /// </summary>
        [Required]
        [Display(Name = "Is Deleted")]
        public bool Deleted
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether Is New
        /// </summary>
        [Display(Name = "Is New")]
        public bool New
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Name
        /// </summary>
        [Display(Name = "Name")]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Public Key
        /// </summary>
        public string PublicKey
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Environment
        /// </summary>
        [DataType(DataType.MultilineText)]
        [Display(Name = "Environment")]
        public string Environment
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Description
        /// </summary>
        [DataType(DataType.MultilineText)]
        [Display(Name = "Description")]
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether Is Owner
        /// </summary>
        public bool IsOwner
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
                    return this.ApplicationId.ToString();
                }
            }
        }

        /// <summary>
        /// User Public Profile
        /// </summary>
        public UserPublicProfile User
        {
            get;
            set;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Convert
        /// </summary>
        /// <returns>Application Information</returns>
        public ApplicationInformation Convert()
        {
            return new ApplicationInformation()
            {
                Active = this.Active,
                Identifier = this.ApplicationId,
                Deleted = this.Deleted,
                Description = this.Description,
                Environment = this.Environment,
                IsValid = this.IsValid,
                Name = this.Name,
                IsNew = this.New,
                ValidUntil = this.ValidUntil,
                PublicKey = this.ApplicationId.ToAscii85().GetHexMD5(),
            };
        }
        #endregion
    }
}