// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='ContentBlobContextTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite
{
    using Abc.Services;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for ContentManagementSystemBlobContextTest and is intended
    /// to contain all ContentManagementSystemBlobContextTest Unit Tests
    /// </summary>
    [TestClass]
    public class ContentBlobContextTest
    {
        #region Error Cases
        #endregion

        #region Valid Cases
        [TestMethod]
        public void BinaryBlobContainerName()
        {
            Assert.AreEqual<string>("binaryblobcontainer", ContentBlobContext.BinaryBlobContainer);
        }

        [TestMethod]
        public void UserLoginTableIsValid()
        {
            Assert.IsTrue(ContentBlobContext.BinaryBlobContainer.IsValidBlobContainer());
        }

        [TestMethod]
        public void XmlBlobContainerName()
        {
            Assert.AreEqual<string>("xmlblobcontainer", ContentBlobContext.XmlBlobContainer);
        }

        [TestMethod]
        public void XmlBlobContainerIsValid()
        {
            Assert.IsTrue(ContentBlobContext.XmlBlobContainer.IsValidBlobContainer());
        }

        [TestMethod]
        public void TextBlobContainerName()
        {
            Assert.AreEqual<string>("textblobcontainer", ContentBlobContext.TextBlobContainer);
        }

        [TestMethod]
        public void TextBlobContainerIsValid()
        {
            Assert.IsTrue(ContentBlobContext.TextBlobContainer.IsValidBlobContainer());
        }

        [TestMethod]
        public void ValidateTables()
        {
            Assert.IsNotNull(ContentBlobContext.Containers);
            foreach (string table in ContentBlobContext.Containers)
            {
                Assert.IsFalse(string.IsNullOrWhiteSpace(table));
                Assert.IsTrue(table.IsValidBlobContainer());
            }
        }
        #endregion
    }
}