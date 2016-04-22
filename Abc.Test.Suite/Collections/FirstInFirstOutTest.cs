// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='FirstInFirstOutTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Collections
{
    using System;
    using Abc.Collections;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class FirstInFirstOutTest
    {
        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new FirstInFirstOut<int>();
        }

        [TestMethod]
        public void Count()
        {
            var queue = new FirstInFirstOut<bool>();
            Assert.AreEqual<int>(0, queue.Count);
            queue.Enqueue(true);
            Assert.AreEqual<int>(1, queue.Count);
            queue.Dequeue();
            Assert.AreEqual<int>(0, queue.Count);
        }

        [TestMethod]
        public void EnqueueDequeue()
        {
            var queue = new FirstInFirstOut<Guid>();
            var g = Guid.NewGuid();
            queue.Enqueue(g);
            var returned = queue.Dequeue();

            Assert.AreEqual<Guid>(g, returned);
        }

        [TestMethod]
        public void EnqueueMultipleDequeue()
        {
            var queue = new FirstInFirstOut<Guid>();
            var a = Guid.NewGuid();
            var b = Guid.NewGuid();
            var c = Guid.NewGuid();
            queue.Enqueue(a);
            queue.Enqueue(b);
            queue.Enqueue(c);

            Assert.AreEqual<Guid>(a, queue.Dequeue());
            Assert.AreEqual<Guid>(b, queue.Dequeue());
            Assert.AreEqual<Guid>(c, queue.Dequeue());
        }

        [TestMethod]
        public void EnqueueTooManyDeque()
        {
            var queue = new FirstInFirstOut<Guid>();
            var a = Guid.NewGuid();
            var b = Guid.NewGuid();
            var c = Guid.NewGuid();
            queue.Enqueue(a);
            queue.Enqueue(b);
            queue.Enqueue(c);

            Assert.AreEqual<Guid>(a, queue.Dequeue());
            Assert.AreEqual<Guid>(b, queue.Dequeue());
            Assert.AreEqual<Guid>(c, queue.Dequeue());
            Assert.AreEqual<Guid>(Guid.Empty, queue.Dequeue());
            Assert.AreEqual<Guid>(Guid.Empty, queue.Dequeue());
            Assert.AreEqual<Guid>(Guid.Empty, queue.Dequeue());
            Assert.AreEqual<Guid>(Guid.Empty, queue.Dequeue());
            Assert.AreEqual<Guid>(Guid.Empty, queue.Dequeue());
            Assert.AreEqual<Guid>(Guid.Empty, queue.Dequeue());
            Assert.AreEqual<Guid>(Guid.Empty, queue.Dequeue());
        }
        #endregion
    }
}