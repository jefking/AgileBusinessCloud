// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='BytesStoredDataValidator.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Data
{
    using System;
    using System.Linq;
    using Abc.Azure;
    using Abc.Services.Contracts;
    
    /// <summary>
    /// Bytes Stored Data Validator
    /// </summary>
    [CLSCompliant(false)]
    public class BytesStoredDataValidator : UnifiedStoreValidator<BytesStoredData>
    {
        #region Methods
        /// <summary>
        /// Validate
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Is Valid</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly", Justification = "Information for logging")]
        protected override bool Validate(BytesStoredData entity)
        {
            if (null == entity)
            {
                throw new ArgumentNullException();
            }
            else if (Guid.Empty == entity.ApplicationId)
            {
                throw new ArgumentOutOfRangeException();
            }
            else if (0 > entity.Bytes)
            {
                throw new ArgumentOutOfRangeException();
            }
            else if (0 > entity.DataCostType)
            {
                throw new ArgumentOutOfRangeException();
            }
            else if (!DataSource.RowIsValid(entity.ObjectType))
            {
                throw new ArgumentOutOfRangeException();
            }
            else if (Enum.GetValues(typeof(DataCostType)).Cast<int>().Max() < entity.DataCostType)
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