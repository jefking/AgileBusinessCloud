// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='MessageDisplayTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Contracts
{
    using System;
    using Abc.Services.Contracts;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class MessageDisplayTest
    {
        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new MessageDisplay();
        }

        [TestMethod]
        public void Identifier()
        {
            var message = new MessageDisplay();
            var data = Guid.NewGuid();
            message.Identifier = data;
            Assert.AreEqual<Guid>(data, message.Identifier);
        }

        [TestMethod]
        public void AsMessage()
        {
            Assert.IsNotNull(new MessageDisplay() as Message);
        }
        #endregion
    }
}