// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='MailgunTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite
{
    using System;
    using Abc.Services;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class MailgunTest
    {
        #region Error Cases
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SendTextInvalidSender()
        {
            MailGun.Send(StringHelper.NullEmptyWhiteSpace(), StringHelper.ValidString(), string.Empty, string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SendTextInvalidRecipients()
        {
            MailGun.Send(StringHelper.ValidString(), StringHelper.NullEmptyWhiteSpace(), string.Empty, string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SendTextNullSubject()
        {
            MailGun.Send(StringHelper.ValidString(), StringHelper.ValidString(), null, string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SendTextNullText()
        {
            MailGun.Send(StringHelper.ValidString(), StringHelper.ValidString(), string.Empty, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SendDataInvalidSender()
        {
            var random = new Random();
            byte[] bytes = new byte[23];
            random.NextBytes(bytes);
            MailGun.Send(StringHelper.NullEmptyWhiteSpace(), StringHelper.ValidString(), bytes);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SendDataInvalidRecipients()
        {
            var random = new Random();
            byte[] bytes = new byte[23];
            random.NextBytes(bytes);
            MailGun.Send(StringHelper.ValidString(), StringHelper.NullEmptyWhiteSpace(), bytes);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SendDataNullMime()
        {
            MailGun.Send(StringHelper.ValidString(), StringHelper.ValidString(), null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SendDataEmptyMime()
        {
            byte[] bytes = new byte[0];
            MailGun.Send(StringHelper.ValidString(), StringHelper.NullEmptyWhiteSpace(), bytes);
        }
        #endregion

        #region Valid Cases
        [TestMethod]
        public void SendText()
        {
            MailGun.Send("test@agilebusinesscloud.com", "build@agilebusinesscloud.com", "This is a test email message", string.Format("this is a test message{0}; which is suppose to have text in it.", StringHelper.ValidString()));
        }

        [TestMethod]
        public void SendData()
        {
            MailGun.Send("test@agilebusinesscloud.com", "build@agilebusinesscloud.com", Guid.NewGuid().ToByteArray());
        }
        #endregion
    }
}