// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='TestStoreValidator.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Data
{
    using System;
    using System.Diagnostics.Contracts;
    using Abc.Azure;

    internal class TestStoreValidator : IStoreValidator<EntityWithDataStore>
    {
        #region Methods
        public bool ValidateForAdd(EntityWithDataStore entity)
        {
            if (0 > entity.ToTest)
            {
                throw new ArgumentOutOfRangeException();
            }

            return true;
        }

        public bool ValidateForAddOrUpdate(EntityWithDataStore entity)
        {
            return 0 <= entity.ToTest;
        }
        #endregion
    }
}