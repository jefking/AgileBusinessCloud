// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='TreeTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Global.Collections
{
    using System;
    using Abc.Collections;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tree Test
    /// </summary>
    [TestClass]
    public class TreeTest
    {
        #region Error Cases
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullKeyAdd()
        {
            Tree<string, string> tree = new Tree<string, string>();
            tree.Add(null, "this is a test");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullFind()
        {
            Tree<string, string> tree = new Tree<string, string>();
            tree.Find(null);
        }
        #endregion

        #region Valid Cases
        [TestMethod]
        public void Add()
        {
            Tree<int, string> tree = new Tree<int, string>();
            tree.Add(22, "this is 22");
            Assert.AreEqual<int>(1, tree.Count);
            tree.Add(88, "this is 88");
            Assert.AreEqual<int>(2, tree.Count);
        }

        [TestMethod]
        public void Find()
        {
            Tree<int, string> tree = new Tree<int, string>();
            tree.Add(22, "this is 22");
            Assert.AreEqual<int>(1, tree.Count);
            tree.Add(88, "this is 88");
            Assert.AreEqual<int>(2, tree.Count);

            Assert.AreEqual<string>("this is 22", tree.Find(22), "Values don't match.");
            Assert.AreEqual<string>("this is 88", tree.Find(88), "Values don't match.");
        }
        #endregion
    }
}