// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='WindowsAzureQueueTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Data
{
    using System;
    using System.Linq;
    using Abc.Azure;
    using Abc.Services.Contracts;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure;

    [TestClass]
    public class WindowsAzureQueueTest
    {
        #region Error Cases
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorNullAccount()
        {
            new WindowsAzureQueue<User>(null, StringHelper.ValidString());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConstructorQueueReferenceInvalid()
        {
            new WindowsAzureQueue<User>(CloudStorageAccount.DevelopmentStorageAccount, StringHelper.NullEmptyWhiteSpace());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void QueueNull()
        {
            var queueName = Guid.NewGuid().ToString();
            var queue = new WindowsAzureQueue<User>(CloudStorageAccount.DevelopmentStorageAccount, queueName);
            queue.Queue(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetMessageCountTooSmall()
        {
            var queueName = Guid.NewGuid().ToString();
            var queue = new WindowsAzureQueue<User>(CloudStorageAccount.DevelopmentStorageAccount, queueName);
            queue.Get(0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetMessageCountTooLarge()
        {
            var queueName = Guid.NewGuid().ToString();
            var queue = new WindowsAzureQueue<User>(CloudStorageAccount.DevelopmentStorageAccount, queueName);
            queue.Get(33);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetMessageCountInvalid()
        {
            var queueName = Guid.NewGuid().ToString();
            var queue = new WindowsAzureQueue<User>(CloudStorageAccount.DevelopmentStorageAccount, queueName);
            queue.Get(0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetTimeSpanZero()
        {
            var queueName = Guid.NewGuid().ToString();
            var queue = new WindowsAzureQueue<User>(CloudStorageAccount.DevelopmentStorageAccount, queueName);
            queue.Get(12, TimeSpan.Zero);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetTimeSpanTooLarge()
        {
            var queueName = Guid.NewGuid().ToString();
            var queue = new WindowsAzureQueue<User>(CloudStorageAccount.DevelopmentStorageAccount, queueName);
            queue.Get(12, new TimeSpan(7, 1, 1, 1));
        }
        #endregion

        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            var queueName = Guid.NewGuid().ToString();
            var queue = new WindowsAzureQueue<User>(CloudStorageAccount.DevelopmentStorageAccount, queueName);
        }

        [TestMethod]
        public void MaximumVisibilityTimeout()
        {
            Assert.AreEqual<TimeSpan>(new TimeSpan(7, 0, 0, 0), WindowsAzureQueue<User>.MaximumVisibilityTimeout);
        }

        [TestMethod]
        public void RetrieveApproximateMessageCount()
        {
            var queueName = Guid.NewGuid().ToString();
            var queue = new WindowsAzureQueue<User>(CloudStorageAccount.DevelopmentStorageAccount, queueName);
            var random = new Random();
            var count = random.Next(1, 100);
            for (int i = 0; i < count; i++)
            {
                queue.Queue(new User());
            }
            Assert.AreEqual<int>(count, queue.RetrieveApproximateMessageCount);
        }

        [TestMethod]
        public void RetrieveApproximateMessageCountZero()
        {
            var queueName = Guid.NewGuid().ToString();
            var queue = new WindowsAzureQueue<User>(CloudStorageAccount.DevelopmentStorageAccount, queueName);
            Assert.AreEqual<int>(0, queue.RetrieveApproximateMessageCount);
        }

        [TestMethod]
        public void ApproximateMessageCountZero()
        {
            var queueName = Guid.NewGuid().ToString();
            var queue = new WindowsAzureQueue<User>(CloudStorageAccount.DevelopmentStorageAccount, queueName);
            Assert.AreEqual<int>(0, queue.ApproximateMessageCount);
        }

        [TestMethod]
        public void Queue()
        {
            var queueName = Guid.NewGuid().ToString();
            var queue = new WindowsAzureQueue<User>(CloudStorageAccount.DevelopmentStorageAccount, queueName);
            queue.Queue(new User());
        }

        [TestMethod]
        public void Peek()
        {
            var queueName = Guid.NewGuid().ToString();
            var queue = new WindowsAzureQueue<User>(CloudStorageAccount.DevelopmentStorageAccount, queueName);
            var data = new User()
            {
                Identifier = Guid.NewGuid(),
            };
            queue.Queue(data);
            var peek = queue.Peek();
            Assert.AreEqual<Guid>(data.Identifier, peek.Identifier);
        }

        [TestMethod]
        public void Get()
        {
            var queueName = Guid.NewGuid().ToString();
            var queue = new WindowsAzureQueue<User>(CloudStorageAccount.DevelopmentStorageAccount, queueName);
            var data = new User()
            {
                Identifier = Guid.NewGuid(),
            };
            queue.Queue(data);
            var peek = queue.Get();
            Assert.AreEqual<Guid>(data.Identifier, peek.Identifier);
        }

        [TestMethod]
        public void GetMany()
        {
            var queueName = Guid.NewGuid().ToString();
            var queue = new WindowsAzureQueue<User>(CloudStorageAccount.DevelopmentStorageAccount, queueName);
            var random = new Random();
            var count = random.Next(2, 32);
            for (int i = 0; i < count; i++)
            {
                queue.Queue(new User());
            }
            var subset = random.Next(1, count - 1);
            var many = queue.Get(subset);
            Assert.AreEqual<int>(subset, many.Count());
        }
        #endregion
    }
}