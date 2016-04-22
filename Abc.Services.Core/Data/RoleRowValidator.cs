// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='RoleRowValidator.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Data
{
    using System;
    using Abc.Azure;

    /// <summary>
    /// Role Row Validator
    /// </summary>
    [CLSCompliant(false)]
    public class RoleRowValidator : UnifiedStoreValidator<RoleRow>
    {
        #region Methods
        /// <summary>
        /// Validate Role Row
        /// </summary>
        /// <param name="entity">Role Row</param>
        /// <returns>Is Valid</returns>
        protected override bool Validate(RoleRow entity)
        {
            return null != entity && Guid.Empty != entity.ApplicationId && Guid.Empty != entity.UserIdentifier && !string.IsNullOrWhiteSpace(entity.Name);
        }
        #endregion
    }
}