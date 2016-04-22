// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='ContactGroupRowValidator.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Data
{
    using System;
    using Abc.Azure;

    /// <summary>
    /// Contact Group Row Validator
    /// </summary>
    [CLSCompliant(false)]
    public class ContactGroupRowValidator : UnifiedStoreValidator<ContactGroupRow>
    {
        #region Methods
        /// <summary>
        /// Validate Contact Group Row
        /// </summary>
        /// <param name="entity">Contact Group Row</param>
        /// <returns>Is Valid</returns>
        protected override bool Validate(ContactGroupRow entity)
        {
            if (null == entity)
            {
                return false;
            }
            else if (Guid.Empty == entity.Identifier)
            {
                return false;
            }
            else if (Guid.Empty == entity.OwnerIdentifier)
            {
                return false;
            }
            else if (string.IsNullOrWhiteSpace(entity.Name))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion
    }
}