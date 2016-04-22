// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='RegisterModel.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Website.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    /// <summary>
    /// Register Model
    /// </summary>
    public class RegisterModel
    {
        #region Properties
        /// <summary>
        /// Gets or sets Name Identifier
        /// </summary>
        [Required]
        public string NameIdentifier { get; set; }

        /// <summary>
        /// Gets or sets UserName
        /// </summary>
        [Required]
        [Display(Name = "Name")]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets Email
        /// </summary>
        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        [RegularExpression(@"^(?("")("".+?""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,6}))$", ErrorMessage = "Not a valid email.")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether RememberMe
        /// </summary>
        [Required]
        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
        #endregion
    }
}