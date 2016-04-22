// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='OccurrenceDataValidatorTest.cs'>
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
    public class OccurrenceDataValidatorTest
    {
        #region Error Cases
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Null()
        {
            var validate = new OccurrenceDataValidator();
            validate.ValidateForAdd(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void MessageTooLong()
        {
            var validate = new OccurrenceDataValidator();
            var data = this.Occurrence();
            data.Message = StringHelper.LongerThanMaximumRowLength();
            validate.ValidateForAdd(data);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void MachineNameTooLong()
        {
            var validate = new OccurrenceDataValidator();
            var data = this.Occurrence();
            data.MachineName = StringHelper.LongerThanMaximumRowLength();
            validate.ValidateForAdd(data);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ClassNameTooLong()
        {
            var validate = new OccurrenceDataValidator();
            var data = this.Occurrence();
            data.ClassName = StringHelper.LongerThanMaximumRowLength();
            validate.ValidateForAdd(data);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void MethodNameTooLong()
        {
            var validate = new OccurrenceDataValidator();
            var data = this.Occurrence();
            data.MethodName = StringHelper.LongerThanMaximumRowLength();
            validate.ValidateForAdd(data);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DeploymentIdTooLong()
        {
            var validate = new OccurrenceDataValidator();
            var data = this.Occurrence();
            data.DeploymentId = StringHelper.LongerThanMaximumRowLength();
            validate.ValidateForAdd(data);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DurationNegative()
        {
            var validate = new OccurrenceDataValidator();
            var data = this.Occurrence();
            data.Duration = -1211;
            validate.ValidateForAdd(data);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ThreadIdNegative()
        {
            var validate = new OccurrenceDataValidator();
            var data = this.Occurrence();
            data.ThreadId = -123;
            validate.ValidateForAdd(data);
        }
        #endregion

        #region Valid Cases
        [TestMethod]
        public void Valid()
        {
            var validate = new OccurrenceDataValidator();
            validate.ValidateForAdd(this.Occurrence());
        }
        #endregion

        #region Helper Methods
        private OccurrenceData Occurrence()
        {
            var random = new Random();
            return new OccurrenceData(Guid.NewGuid())
            {
                ClassName = StringHelper.ValidString(),
                DeploymentId = StringHelper.ValidString(),
                Duration = random.Next(),
                MachineName = StringHelper.ValidString(),
                Message = StringHelper.ValidString(),
                MethodName = StringHelper.ValidString(),
                OccurredOn = DateTime.UtcNow,
                ThreadId = random.Next(),
            };
        }
        #endregion
    }
}