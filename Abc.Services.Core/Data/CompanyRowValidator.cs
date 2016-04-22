// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='CompanyRowValidator.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Data
{
    using System;
    using Abc.Azure;

    /// <summary>
    /// Company Row Validator
    /// </summary>
    [CLSCompliant(false)]
    public class CompanyRowValidator : UnifiedStoreValidator<CompanyRow>
    {
        #region Methods
        /// <summary>
        /// Validate
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Is Valid</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly", Justification = "Information for logging")]
        protected override bool Validate(CompanyRow entity)
        {
            if (null == entity)
            {
                throw new ArgumentNullException();
            }
            else if (Guid.Empty == entity.OwnerIdentifier)
            {
                throw new ArgumentOutOfRangeException();
            }
            else if (Guid.Empty == entity.Identifier)
            {
                throw new ArgumentOutOfRangeException();
            }
            else if (Guid.Empty == entity.CreatedByIdentifier)
            {
                throw new ArgumentOutOfRangeException();
            }
            else if (Guid.Empty == entity.EditedByIdentifier)
            {
                throw new ArgumentOutOfRangeException();
            }
            else if (string.IsNullOrWhiteSpace(entity.Name))
            {
                throw new ArgumentOutOfRangeException();
            }
            else
            {
                return true;
            }
        }
        #endregion
    }
}
