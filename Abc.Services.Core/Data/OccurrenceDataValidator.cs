﻿// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='OccurrenceDataValidator.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Data
{
    using System;
    using Abc.Azure;

    /// <summary>
    /// Occurrence Data Validator
    /// </summary>
    [CLSCompliant(false)]
    public class OccurrenceDataValidator : UnifiedStoreValidator<OccurrenceData>
    {
        #region Methods
        /// <summary>
        /// Validate
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Is Valid</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly", Justification = "Information for logging")]
        protected override bool Validate(OccurrenceData entity)
        {
            if (null == entity)
            {
                throw new ArgumentNullException();
            }
            else if (Guid.Empty == entity.Id)
            {
                throw new ArgumentOutOfRangeException();
            }
            else if (Guid.Empty == entity.ApplicationId)
            {
                throw new ArgumentOutOfRangeException();
            }
            else if (!DataSource.RowIsValid(entity.Message))
            {
                throw new ArgumentOutOfRangeException();
            }
            else if (!DataSource.RowIsValid(entity.MachineName))
            {
                throw new ArgumentOutOfRangeException();
            }
            else if (!DataSource.RowIsValid(entity.ClassName))
            {
                throw new ArgumentOutOfRangeException();
            }
            else if (!DataSource.RowIsValid(entity.MethodName))
            {
                throw new ArgumentOutOfRangeException();
            }
            else if (!DataSource.RowIsValid(entity.DeploymentId))
            {
                throw new ArgumentOutOfRangeException();
            }
            else if (0 > entity.Duration)
            {
                throw new ArgumentOutOfRangeException();
            }
            else if (0 > entity.ThreadId)
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