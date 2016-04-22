// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='ErrorDataValidatorTest.cs'>
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
    public class ErrorDataValidatorTest
    {
        #region Error Cases
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Null()
        {
            var validator = new ErrorDataValidator();
            validator.ValidateForAddOrUpdate(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void MessageTooLong()
        {
            var validator = new ErrorDataValidator();
            var error = this.Error();
            error.Message = StringHelper.LongerThanMaximumRowLength();
            validator.ValidateForAddOrUpdate(error);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void MachineNameTooLong()
        {
            var validator = new ErrorDataValidator();
            var error = this.Error();
            error.MachineName = StringHelper.LongerThanMaximumRowLength();
            validator.ValidateForAddOrUpdate(error);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ClassNameTooLong()
        {
            var validator = new ErrorDataValidator();
            var error = this.Error();
            error.ClassName = StringHelper.LongerThanMaximumRowLength();
            validator.ValidateForAddOrUpdate(error);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SourceTooLong()
        {
            var validator = new ErrorDataValidator();
            var error = this.Error();
            error.Source = StringHelper.LongerThanMaximumRowLength();
            validator.ValidateForAddOrUpdate(error);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void StackTraceTooLong()
        {
            var validator = new ErrorDataValidator();
            var error = this.Error();
            error.StackTrace = StringHelper.LongerThanMaximumRowLength();
            validator.ValidateForAddOrUpdate(error);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DeploymentIdTooLong()
        {
            var validator = new ErrorDataValidator();
            var error = this.Error();
            error.DeploymentId = StringHelper.LongerThanMaximumRowLength();
            validator.ValidateForAddOrUpdate(error);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ErrorCodeNegative()
        {
            var validator = new ErrorDataValidator();
            var error = this.Error();
            error.ErrorCode = -123847;
            validator.ValidateForAddOrUpdate(error);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void EventTypeValueNegative()
        {
            var validator = new ErrorDataValidator();
            var error = this.Error();
            error.EventTypeValue = -847373;
            validator.ValidateForAddOrUpdate(error);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void EventTypeValueTooHigh()
        {
            var validator = new ErrorDataValidator();
            var error = this.Error();
            error.EventTypeValue = 9412;
            validator.ValidateForAddOrUpdate(error);
        }
        #endregion

        #region Valid Cases
        [TestMethod]
        public void Valid()
        {
            var validator = new ErrorDataValidator();
            validator.ValidateForAddOrUpdate(this.Error());
        }
        #endregion

        #region Helper Methods
        private ErrorData Error()
        {
            var random = new Random();
            return new ErrorData(Guid.NewGuid())
            {
                ClassName = StringHelper.ValidString(),
                DeploymentId = StringHelper.ValidString(),
                ErrorCode = random.Next(),
                EventTypeValue = 2,
                MachineName = StringHelper.ValidString(),
                Message = StringHelper.ValidString(),
                OccurredOn = DateTime.UtcNow,
                ParentId = Guid.NewGuid(),
                Source = StringHelper.ValidString(),
                StackTrace = StringHelper.ValidString(),
            };
        }
        #endregion
    }
}