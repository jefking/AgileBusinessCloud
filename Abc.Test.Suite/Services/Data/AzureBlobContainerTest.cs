// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='AzureBlobContainerTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Data
{
    using System;
    using System.IO;
    using Abc.Azure;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.StorageClient;

    [TestClass]
    public class AzureBlobContainerTest
    {
        #region Error Cases
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullAccount()
        {
            new BinaryContainer(null, StringHelper.ValidString());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ConstructorInvalidContainerName()
        {
            new BinaryContainer(CloudStorageAccount.DevelopmentStorageAccount, StringHelper.NullEmptyWhiteSpace());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetStreamNullId()
        {
            var container = new BinaryContainer(CloudStorageAccount.DevelopmentStorageAccount, "asdas");
            container.GetStream(StringHelper.NullEmptyWhiteSpace());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetBytesNullId()
        {
            var container = new BinaryContainer(CloudStorageAccount.DevelopmentStorageAccount, "asdas");
            container.GetBytes(StringHelper.NullEmptyWhiteSpace());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DeleteNullId()
        {
            var container = new BinaryContainer(CloudStorageAccount.DevelopmentStorageAccount, "asdas");
            container.Delete(StringHelper.NullEmptyWhiteSpace());
        }
        #endregion

        #region Valid Cases
        [TestMethod]
        public void Exists()
        {
            var containerName = Guid.NewGuid().ToString().Replace("-", string.Empty);
            var container = new BinaryContainer(CloudStorageAccount.DevelopmentStorageAccount, containerName);
            Assert.IsFalse(container.Exists());
            container.EnsureExist();
            Assert.IsTrue(container.Exists());
            container.DeleteIfExist();
            Assert.IsFalse(container.Exists());
        }

        [TestMethod]
        public void EnsureExists()
        {
            var containerName = Guid.NewGuid().ToString().Replace("-", string.Empty);
            var container = new BinaryContainer(CloudStorageAccount.DevelopmentStorageAccount, containerName);
            container.EnsureExist();

            var client = CloudStorageAccount.DevelopmentStorageAccount.CreateCloudBlobClient();
            var containerRef = client.GetContainerReference(containerName.ToLowerInvariant());
            Assert.IsNotNull(containerRef);
        }

        [TestMethod]
        public void EnsureExistsPublic()
        {
            var containerName = Guid.NewGuid().ToString().Replace("-", string.Empty);
            var container = new BinaryContainer(CloudStorageAccount.DevelopmentStorageAccount, containerName);
            container.EnsureExist(true);

            var client = CloudStorageAccount.DevelopmentStorageAccount.CreateCloudBlobClient();
            var containerRef = client.GetContainerReference(containerName.ToLowerInvariant());
            Assert.IsNotNull(containerRef);
            var permissions = containerRef.GetPermissions();
            Assert.IsNotNull(permissions);
            Assert.AreEqual<BlobContainerPublicAccessType>(BlobContainerPublicAccessType.Container, permissions.PublicAccess);
        }

        [TestMethod]
        public void DeleteIfExist()
        {
            var containerName = Guid.NewGuid().ToString().Replace("-", string.Empty);
            var container = new BinaryContainer(CloudStorageAccount.DevelopmentStorageAccount, containerName);
            container.EnsureExist();

            var client = CloudStorageAccount.DevelopmentStorageAccount.CreateCloudBlobClient();
            var containerRef = client.GetContainerReference(containerName.ToLowerInvariant());
            Assert.IsTrue(containerRef.Exists());

            container.DeleteIfExist();
            containerRef = client.GetContainerReference(containerName.ToLowerInvariant());
            Assert.IsFalse(containerRef.Exists());
        }

        [TestMethod]
        public void GetStream()
        {
            var bytes = new byte[256];
            Random random = new Random();
            random.NextBytes(bytes);
            var containerName = Guid.NewGuid().ToString().Replace("-", string.Empty);
            var container = new BinaryContainer(CloudStorageAccount.DevelopmentStorageAccount, containerName);
            container.EnsureExist();

            var id = Guid.NewGuid().ToString();
            var uri = container.Save(id, bytes, "na");
            using (var stream = container.GetStream(id) as MemoryStream)
            {
                var returned = stream.ToArray();
                Assert.IsTrue(bytes.ContentEquals(returned));
            }
        }

        [TestMethod]
        public void GetBytes()
        {
            var bytes = new byte[256];
            Random random = new Random();
            random.NextBytes(bytes);
            var containerName = Guid.NewGuid().ToString().Replace("-", string.Empty);
            var container = new BinaryContainer(CloudStorageAccount.DevelopmentStorageAccount, containerName);
            container.EnsureExist();

            var id = Guid.NewGuid().ToString();
            var uri = container.Save(id, bytes, "na");
            var returned = container.GetBytes(id);
            Assert.IsTrue(bytes.ContentEquals(returned));
        }

        [TestMethod]
        public void GetBytesNested()
        {
            var bytes = new byte[256];
            Random random = new Random();
            random.NextBytes(bytes);
            var containerName = Guid.NewGuid().ToString().Replace("-", string.Empty);
            var container = new BinaryContainer(CloudStorageAccount.DevelopmentStorageAccount, containerName);
            container.EnsureExist();

            var id = string.Format("/testingA/{0}/Happy", Guid.NewGuid());
            var uri = container.Save(id, bytes, "na");
            var returned = container.GetBytes(id);
            Assert.IsTrue(bytes.ContentEquals(returned));
        }

        [TestMethod]
        public void GetBytesExtension()
        {
            var bytes = new byte[256];
            Random random = new Random();
            random.NextBytes(bytes);
            var containerName = Guid.NewGuid().ToString().Replace("-", string.Empty);
            var container = new BinaryContainer(CloudStorageAccount.DevelopmentStorageAccount, containerName);
            container.EnsureExist();

            var id = string.Format("{0}.jpeg", Guid.NewGuid());
            var uri = container.Save(id, bytes, "na");
            var returned = container.GetBytes(id);
            Assert.IsTrue(bytes.ContentEquals(returned));
        }

        [TestMethod]
        public void Delete()
        {
            var bytes = new byte[256];
            Random random = new Random();
            random.NextBytes(bytes);
            var containerName = Guid.NewGuid().ToString().Replace("-", string.Empty);
            var container = new BinaryContainer(CloudStorageAccount.DevelopmentStorageAccount, containerName);
            container.EnsureExist();

            var id = Guid.NewGuid().ToString();
            var uri = container.Save(id, bytes, "na");
            var returned = container.GetBytes(id);
            Assert.IsTrue(bytes.ContentEquals(returned));
            container.Delete(id);
        }
        #endregion
    }
}