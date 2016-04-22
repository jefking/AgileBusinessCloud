// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='UnifiedStoreValidator.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Azure
{
    using System;
    using Microsoft.WindowsAzure.StorageClient;

    /// <summary>
    /// Unified Store Validator
    /// </summary>
    /// <typeparam name="TEntity">Table Entity</typeparam>
    [CLSCompliant(false)]
    public abstract class UnifiedStoreValidator<TEntity> : IStoreValidator<TEntity>
        where TEntity : TableServiceEntity
    {
        #region Methods
        /// <summary>
        /// Validate for Adding to data store
        /// </summary>
        /// <param name="entity">Object</param>
        /// <returns>Is Valid</returns>
        public bool ValidateForAdd(TEntity entity)
        {
            return this.Validate(entity);
        }

        /// <summary>
        /// Validate for Adding or Updating in data store
        /// </summary>
        /// <param name="entity">Object</param>
        /// <returns>Is Valid</returns>
        public bool ValidateForAddOrUpdate(TEntity entity)
        {
            return this.Validate(entity);
        }

        /// <summary>
        /// Validate
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Is Valid</returns>
        protected abstract bool Validate(TEntity entity);
        #endregion
    }
}