// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='MessageDataValidator.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Data
{
    using System;
    using Abc.Azure;

    /// <summary>
    /// Message Data Validator
    /// </summary>
    [CLSCompliant(false)]
    public class MessageDataValidator : UnifiedStoreValidator<MessageData>
    {
        #region Methods
        /// <summary>
        /// Validate
        /// </summary>
        /// <param name="entity">Message Data</param>
        /// <returns>Is Valid</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly", Justification = "Information for logging")]
        protected override bool Validate(MessageData entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            else if (Guid.Empty == entity.Id)
            {
                throw new ArgumentOutOfRangeException();
            }
            else if (Guid.Empty == entity.ApplicationId)
            {
                throw new ArgumentOutOfRangeException();
            }
            else if (string.IsNullOrWhiteSpace(entity.Message) || !DataSource.RowIsValid(entity.Message))
            {
                throw new ArgumentOutOfRangeException();
            }
            else if (!DataSource.RowIsValid(entity.MachineName))
            {
                throw new ArgumentOutOfRangeException();
            }
            else if (!DataSource.RowIsValid(entity.DeploymentId))
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