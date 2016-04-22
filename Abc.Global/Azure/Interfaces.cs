// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='Interfaces.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Azure
{
    using System;
    using Microsoft.WindowsAzure.StorageClient;

    /// <summary>
    /// Store Validation
    /// </summary>
    /// <typeparam name="TEntity">Table Entity</typeparam>
    [CLSCompliant(false)]
    public interface IStoreValidator<TEntity>
        where TEntity : TableServiceEntity
    {
        #region Methods
        /// <summary>
        /// Validate for Adding to data store
        /// </summary>
        /// <param name="entity">Object</param>
        /// <returns>Is Valid</returns>
        bool ValidateForAdd(TEntity entity);

        /// <summary>
        /// Validate for Adding or Updating in data store
        /// </summary>
        /// <param name="entity">Object</param>
        /// <returns>Is Valid</returns>
        bool ValidateForAddOrUpdate(TEntity entity);
        #endregion
    }

    /// <summary>
    /// Azure Storage Interface, Tables and Blobs
    /// </summary>
    public interface IAzureStorage
    {
        #region Methods
        /// <summary>
        /// Create If Not Exists
        /// </summary>
        void EnsureExist();

        /// <summary>
        /// Delete If Exists
        /// </summary>
        void DeleteIfExist();
        #endregion
    }

    /// <summary>
    /// Text Container for Mocking
    /// </summary>
    /// <typeparam name="T">Type to Serialize</typeparam>
    public interface ITextContainer<T>
    {
        #region Methods
        /// <summary>
        /// Save an Object
        /// </summary>
        /// <param name="objId">Object Identifier</param>
        /// <param name="obj">Object</param>
        void Save(string objId, T obj);
        #endregion
    }
}