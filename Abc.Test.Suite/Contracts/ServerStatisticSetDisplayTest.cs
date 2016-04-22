// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='ServerStatisticSetDisplayTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Contracts
{
    using System;
    using System.Collections.Generic;
    using Abc.Services;
    using Abc.Services.Contracts;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ServerStatisticSetDisplayTest
    {
        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new ServerStatisticSetDisplay();
        }

        [TestMethod]
        public void Identifier()
        {
            var message = new ServerStatisticSetDisplay();
            var data = Guid.NewGuid();
            message.Identifier = data;
            Assert.AreEqual<Guid>(data, message.Identifier);
        }

        [TestMethod]
        public void Range()
        {
            var message = new ServerStatisticSetDisplay();
            var data = new List<ServerStatisticSetDisplay>();
            message.Range = data;
            Assert.AreEqual<IEnumerable<ServerStatisticSetDisplay>>(data, message.Range);
        }

        [TestMethod]
        public void AsMessage()
        {
            Assert.IsNotNull(new ServerStatisticSetDisplay() as ServerStatisticSet);
        }

        [TestMethod]
        public void AsIIdentifierGuid()
        {
            Assert.IsNotNull(new ServerStatisticSetDisplay() as IIdentifier<Guid>);
        }
        #endregion
    }
}