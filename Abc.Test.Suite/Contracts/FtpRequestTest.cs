// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='FtpRequestTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Contracts
{
    using Abc.Services.Contracts;
    using Abc.Services.Validation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class FtpRequestTest
    {
        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new FtpRequest();
        }

        [TestMethod]
        public void Token()
        {
            var ftp = new FtpRequest();
            var data = new Token();
            ftp.Token = data;
            Assert.AreEqual<Token>(data, ftp.Token);
        }

        [TestMethod]
        public void UserName()
        {
            var ftp = new FtpRequest();
            var data = StringHelper.ValidString();
            ftp.UserName = data;
            Assert.AreEqual<string>(data, ftp.UserName);
        }

        [TestMethod]
        public void Uri()
        {
            var ftp = new FtpRequest();
            var data = StringHelper.ValidString();
            ftp.Uri = data;
            Assert.AreEqual<string>(data, ftp.Uri);
        }

        [TestMethod]
        public void Valid()
        {
            var ftp = new FtpRequest()
            {
                Uri = StringHelper.ValidString(),
            };

            var validator = new Validator<FtpRequest>();
            Assert.IsTrue(validator.IsValid(ftp));
        }

        [TestMethod]
        public void EmptyUri()
        {
            var ftp = new FtpRequest()
            {
                Uri = StringHelper.NullEmptyWhiteSpace(),
            };

            var validator = new Validator<FtpRequest>();
            Assert.IsFalse(validator.IsValid(ftp));
        }

        [TestMethod]
        public void UriTooLong()
        {
            var ftp = new FtpRequest()
            {
                Uri = StringHelper.LongerThanMaximumRowLength(),
            };

            var validator = new Validator<FtpRequest>();
            Assert.IsFalse(validator.IsValid(ftp));
        }
        #endregion
    }
}
