// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='ApplicationInfoValidatorTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Data
{
    using System;
    using Abc.Services;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ApplicationInfoValidatorTest
    {
        #region Error Cases
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ValidateForAddOrUpdateNull()
        {
            var validate = new ApplicationInfoValidator();
            validate.ValidateForAddOrUpdate(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ValidateForAddOrUpdateEmptyCreatedBy()
        {
            var validate = new ApplicationInfoValidator();
            var data = this.ApplicationInfo();
            data.CreatedBy = Guid.Empty;
            validate.ValidateForAddOrUpdate(data);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ValidateForAddOrUpdateEmptyLastUpdatedBy()
        {
            var validate = new ApplicationInfoValidator();
            var data = this.ApplicationInfo();
            data.LastUpdatedBy = Guid.Empty;
            validate.ValidateForAddOrUpdate(data);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void 
        ValidateForAddOrUpdateOldCreatedOn()
        {
            var validate = new ApplicationInfoValidator();
            var data = this.ApplicationInfo();
            data.CreatedOn = new DateTime(2011, 07, 22);
            validate.ValidateForAddOrUpdate(data);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ValidateForAddOrUpdateOldLastUpdatedOn()
        {
            var validate = new ApplicationInfoValidator();
            var data = this.ApplicationInfo();
            data.LastUpdatedOn = new DateTime(2001, 07, 22);
            validate.ValidateForAddOrUpdate(data);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ValidateForAddNull()
        {
            var validate = new ApplicationInfoValidator();
            validate.ValidateForAdd(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ValidateForAddEmptyCreatedBy()
        {
            var validate = new ApplicationInfoValidator();
            var data = this.ApplicationInfo();
            data.CreatedBy = Guid.Empty;
            validate.ValidateForAdd(data);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ValidateForAddEmptyLastUpdatedBy()
        {
            var validate = new ApplicationInfoValidator();
            var data = this.ApplicationInfo();
            data.LastUpdatedBy = Guid.Empty;
            validate.ValidateForAdd(data);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void
        ValidateForAddOldCreatedOn()
        {
            var validate = new ApplicationInfoValidator();
            var data = this.ApplicationInfo();
            data.CreatedOn = new DateTime(2011, 07, 22);
            validate.ValidateForAdd(data);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ValidateForAddOldLastUpdatedOn()
        {
            var validate = new ApplicationInfoValidator();
            var data = this.ApplicationInfo();
            data.LastUpdatedOn = new DateTime(2001, 07, 22);
            validate.ValidateForAdd(data);
        }
        #endregion

        #region Valid Cases
        [TestMethod]
        public void ValidateForAddOrUpdate()
        {
            var validate = new ApplicationInfoValidator();
            validate.ValidateForAddOrUpdate(this.ApplicationInfo());
        }

        [TestMethod]
        public void ValidateForAdd()
        {
            var validate = new ApplicationInfoValidator();
            validate.ValidateForAdd(this.ApplicationInfo());
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Application Info
        /// </summary>
        /// <returns></returns>
        private ApplicationInfoData ApplicationInfo()
        {
            return new ApplicationInfoData(Guid.NewGuid())
            {
                CreatedBy = Guid.NewGuid(),
                CreatedOn = DateTime.UtcNow,
                Description = StringHelper.ValidString(),
                LastUpdatedBy = Guid.NewGuid(),
                LastUpdatedOn = DateTime.UtcNow,
                Name = StringHelper.ValidString()
            };
        }
        #endregion
    }
}