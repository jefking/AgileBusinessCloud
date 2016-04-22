// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='BytesStoredDataValidatorTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Data
{
    using System;
    using Abc.Services;
    using Abc.Services.Data;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class BytesStoredDataValidatorTest
    {
        #region Error Cases
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Null()
        {
            var validate = new BytesStoredDataValidator();
            validate.ValidateForAdd(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void BytesNegative()
        {
            var validate = new BytesStoredDataValidator();
            var data = this.Bytes();
            data.Bytes = -948398;
            validate.ValidateForAdd(data);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DataCostTypeNegative()
        {
            var validate = new BytesStoredDataValidator();
            var data = this.Bytes();
            data.DataCostType = -123;
            validate.ValidateForAdd(data);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DataCostTypeTooLarge()
        {
            var validate = new BytesStoredDataValidator();
            var data = this.Bytes();
            data.DataCostType = 123123123;
            validate.ValidateForAdd(data);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ObjectTypeTooLong()
        {
            var validate = new BytesStoredDataValidator();
            var data = this.Bytes();
            data.ObjectType = StringHelper.LongerThanMaximumRowLength();
            validate.ValidateForAdd(data);
        }
        #endregion

        #region Valid Cases
        [TestMethod]
        public void Valid()
        {
            var validate = new BytesStoredDataValidator();
            validate.ValidateForAdd(this.Bytes());
        }
        #endregion

        #region Helper Methods
        private BytesStoredData Bytes()
        {
            var random = new Random();
            return new BytesStoredData(Guid.NewGuid())
            {
                Bytes = random.Next(),
                DataCostType = 1,
                ObjectType = StringHelper.ValidString(),
                OccurredOn = DateTime.UtcNow,
            };
        }
        #endregion
    }
}