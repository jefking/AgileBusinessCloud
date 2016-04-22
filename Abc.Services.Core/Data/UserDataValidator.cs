// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='UserDataValidator.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services
{
    using System;
    using System.Diagnostics.Contracts;
    using Abc.Azure;

    /// <summary>
    /// User Data Validator
    /// </summary>
    public class UserDataValidator : IStoreValidator<UserData>
    {
        #region Methods
        /// <summary>
        /// Validate for Adding to data store
        /// </summary>
        /// <param name="entity">Object</param>
        /// <returns>Is Valid</returns>
        [CLSCompliant(false)]
        public bool ValidateForAdd(UserData entity)
        {
            return Validate(entity);
        }

        /// <summary>
        /// Validate for Adding or Updating in data store
        /// </summary>
        /// <param name="entity">Object</param>
        /// <returns>Is Valid</returns>
        [CLSCompliant(false)]
        public bool ValidateForAddOrUpdate(UserData entity)
        {
            return Validate(entity);
        }

        /// <summary>
        /// Validate
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Is Valid</returns>
        private static bool Validate(UserData entity)
        {
            Contract.Requires<ArgumentOutOfRangeException>(!string.IsNullOrWhiteSpace(entity.NameIdentifier));
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != entity.Id);
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != entity.ApplicationId);
            Contract.Requires<ArgumentOutOfRangeException>(entity.RoleValue == 0 || entity.RoleValue == 1);
            Contract.Requires<ArgumentOutOfRangeException>(entity.CreatedOn <= DateTime.UtcNow);
            Contract.Requires<ArgumentOutOfRangeException>(entity.LastLoggedInOn <= DateTime.UtcNow);
            Contract.Requires<ArgumentOutOfRangeException>(entity.LastActivityOn <= DateTime.UtcNow);
            Contract.Requires<ArgumentOutOfRangeException>(entity.PasswordLastChangedOn <= DateTime.UtcNow);
            Contract.Requires<ArgumentOutOfRangeException>(entity.LastLockedOutOn <= DateTime.UtcNow);

            return true;
        }
        #endregion
    }
}