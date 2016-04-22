// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='ContactRowValidator.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Data
{
    using System;
    using Abc.Azure;

    /// <summary>
    /// Contact Row Validator
    /// </summary>
    [CLSCompliant(false)]
    public class ContactRowValidator : UnifiedStoreValidator<ContactRow>
    {
        #region Methods
        /// <summary>
        /// Validate
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Is Valid</returns>
        protected override bool Validate(ContactRow entity)
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
            else if (string.IsNullOrWhiteSpace(entity.Email))
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