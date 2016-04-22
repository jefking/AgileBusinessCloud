// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='UserRole.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Website.Models
{
    using System;

    /// <summary>
    /// User Role
    /// </summary>
    public class UserRole
    {
        #region Properties
        /// <summary>
        /// Gets or sets Role Name
        /// </summary>
        public string RoleName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets User Identifier
        /// </summary>
        public Guid UserIdentifier
        {
            get;
            set;
        }
        #endregion
    }
}
