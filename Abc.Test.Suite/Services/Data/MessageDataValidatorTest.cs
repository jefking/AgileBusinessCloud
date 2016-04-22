// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='MessageDataValidatorTest.cs'>
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
    public class MessageDataValidatorTest
    {
        #region Error Cases
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Null()
        {
            var validator = new MessageDataValidator();
            validator.ValidateForAdd(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void MessageTooLong()
        {
            var validator = new MessageDataValidator();
            var data = this.Message();
            data.Message = StringHelper.LongerThanMaximumRowLength();
            validator.ValidateForAdd(data);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void MachineNameTooLong()
        {
            var validator = new MessageDataValidator();
            var data = this.Message();
            data.MachineName = StringHelper.LongerThanMaximumRowLength();
            validator.ValidateForAdd(data);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DeploymentIdTooLong()
        {
            var validator = new MessageDataValidator();
            var data = this.Message();
            data.DeploymentId = StringHelper.LongerThanMaximumRowLength();
            validator.ValidateForAdd(data);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void MessageInvalid()
        {
            var validator = new MessageDataValidator();
            var data = this.Message();
            data.Message = StringHelper.NullEmptyWhiteSpace();
            validator.ValidateForAdd(data);
        }
        #endregion

        #region Valid Cases
        [TestMethod]
        public void Valid()
        {
            var validator = new MessageDataValidator();
            validator.ValidateForAdd(this.Message());
        }
        #endregion

        #region Helper Methods
        public MessageData Message()
        {
            return new MessageData(Guid.NewGuid())
            {
                DeploymentId = StringHelper.ValidString(),
                MachineName = StringHelper.ValidString(),
                Message = StringHelper.ValidString(),
                OccurredOn = DateTime.UtcNow,
            };
        }
        #endregion
    }
}