// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='BinaryContainerTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Data
{
    using System;
    using Abc.Azure;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure;

    [TestClass]
    public class BinaryContainerTest
    {
        #region Error Cases
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SaveInvalidId()
        {
            var container = new BinaryContainer(CloudStorageAccount.DevelopmentStorageAccount, "asdasdfa");
            container.Save(StringHelper.NullEmptyWhiteSpace(), null, StringHelper.ValidString());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SaveInvalidContentType()
        {
            var container = new BinaryContainer(CloudStorageAccount.DevelopmentStorageAccount, "asdasdfa");
            container.Save(StringHelper.ValidString(), null, StringHelper.NullEmptyWhiteSpace());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SaveWithTimeSpanInvalidId()
        {
            var container = new BinaryContainer(CloudStorageAccount.DevelopmentStorageAccount, "asdasdfa");
            container.Save(StringHelper.NullEmptyWhiteSpace(), null, StringHelper.ValidString(), TimeSpan.MaxValue);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SaveWithTimeSpanInvalidContentType()
        {
            var container = new BinaryContainer(CloudStorageAccount.DevelopmentStorageAccount, "asdasdfa");
            container.Save(StringHelper.ValidString(), null, StringHelper.NullEmptyWhiteSpace(), new TimeSpan(0, 0, 45));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SaveWithTimeSpanNegativeTimeSpan()
        {
            var container = new BinaryContainer(CloudStorageAccount.DevelopmentStorageAccount, "asdasdfa");
            container.Save(StringHelper.ValidString(), null, StringHelper.NullEmptyWhiteSpace(), TimeSpan.MinValue);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SaveWithTimeSpanMaxTimeSpan()
        {
            var container = new BinaryContainer(CloudStorageAccount.DevelopmentStorageAccount, "asdasdfa");
            container.Save(StringHelper.ValidString(), null, StringHelper.NullEmptyWhiteSpace(), TimeSpan.MaxValue);
        }
        #endregion

        #region Valid Cases
        [TestMethod]
        public void Save()
        {
            var bytes = new byte[256];
            Random random = new Random();
            random.NextBytes(bytes);
            var containerName = "aslkdjhasasd";
            var container = new BinaryContainer(CloudStorageAccount.DevelopmentStorageAccount, containerName);
            container.EnsureExist();

            var id = Guid.NewGuid().ToString();
            var uri = container.Save(id, bytes, "na");
            var returned = container.GetBytes(id);
            Assert.IsTrue(bytes.ContentEquals(returned));
        }

        [TestMethod]
        public void SaveWithTimeOut()
        {
            var bytes = new byte[256];
            Random random = new Random();
            random.NextBytes(bytes);
            var containerName = "aslasdasdkdjh";
            var container = new BinaryContainer(CloudStorageAccount.DevelopmentStorageAccount, containerName);
            container.EnsureExist();

            var id = Guid.NewGuid().ToString();
            var uri = container.Save(id, bytes, "na", new TimeSpan(0, 1, 0));
            var returned = container.GetBytes(id);
            Assert.IsTrue(bytes.ContentEquals(returned));
        }
        #endregion
    }
}