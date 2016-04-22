// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='UserApplicationValidation.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services
{
    using System;
    using System.Diagnostics.Contracts;
    using Abc.Azure;

    /// <summary>
    /// User Application Validation
    /// </summary>
    public class UserApplicationValidation : IStoreValidator<UserApplicationData>
    {
        #region Methods
        /// <summary>
        /// Validate For Add
        /// </summary>
        /// <param name="entity">Enitity</param>
        /// <returns>Is Valid</returns>
        [CLSCompliant(false)]
        public bool ValidateForAdd(UserApplicationData entity)
        {
            Validate(entity);

            return true;
        }

        /// <summary>
        /// Validate For Add Or Update
        /// </summary>
        /// <param name="entity">Enitity</param>
        /// <returns>Is Valid</returns>
        [CLSCompliant(false)]
        public bool ValidateForAddOrUpdate(UserApplicationData entity)
        {
            Validate(entity);

            return true;
        }

        /// <summary>
        /// Validate
        /// </summary>
        /// <param name="entity">Entity</param>
        private static void Validate(UserApplicationData entity)
        {
            Contract.Requires<ArgumentNullException>(null != entity);
            Contract.Requires<ArgumentOutOfRangeException>(!string.IsNullOrWhiteSpace(entity.PartitionKey));
            Contract.Requires<ArgumentOutOfRangeException>(!string.IsNullOrWhiteSpace(entity.RowKey));
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != entity.ApplicationId);
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != entity.UserId);
        }
        #endregion
    }
}