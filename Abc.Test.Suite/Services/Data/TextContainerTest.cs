// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='TextContainerTest.cs'>
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
    public class TextContainerTest
    {
        #region Error Cases
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SaveInvalidId()
        {
            var container = new JsonContainer<Entity>(CloudStorageAccount.DevelopmentStorageAccount);
            container.Save(StringHelper.NullEmptyWhiteSpace(), new Entity());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SaveNullObj()
        {
            var container = new JsonContainer<Entity>(CloudStorageAccount.DevelopmentStorageAccount);
            container.Save(StringHelper.ValidString(), null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetInvalidId()
        {
            var container = new JsonContainer<Entity>(CloudStorageAccount.DevelopmentStorageAccount);
            container.Get(StringHelper.NullEmptyWhiteSpace());
        }
        #endregion
    }
}