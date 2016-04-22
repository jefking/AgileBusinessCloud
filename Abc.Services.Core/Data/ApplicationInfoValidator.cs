// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='ApplicationInfoValidator.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services
{
    using System;
    using System.Diagnostics.Contracts;
    using Abc.Azure;

    /// <summary>
    /// Application Info Validator
    /// </summary>
    [CLSCompliant(false)]
    public class ApplicationInfoValidator : UnifiedStoreValidator<ApplicationInfoData>
    {
        #region Methods
        /// <summary>
        /// Validate
        /// </summary>
        /// <param name="entity">Application Info Data</param>
        /// <returns>Is Valid</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly", Justification = "Pass information back for logging")]
        protected override bool Validate(ApplicationInfoData entity)
        {
            if (null == entity)
            {
                throw new ArgumentNullException("entity");
            }
            else if (Guid.Empty == entity.ApplicationId)
            {
                throw new ArgumentOutOfRangeException();
            }
            else if (Guid.Empty == entity.CreatedBy)
            {
                throw new ArgumentOutOfRangeException();
            }
            else if (Guid.Empty == entity.LastUpdatedBy)
            {
                throw new ArgumentOutOfRangeException();
            }
            else if (new DateTime(2011, 08, 15) > entity.CreatedOn)
            {
                throw new ArgumentOutOfRangeException();
            }
            else if (new DateTime(2011, 08, 15) > entity.LastUpdatedOn)
            {
                throw new ArgumentOutOfRangeException();
            }
            else if (null == entity.PartitionKey)
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