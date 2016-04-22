// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='SessionTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Diagnostics
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SessionTest
    {
        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new Abc.Diagnostics.Session();
        }

        [TestMethod]
        public void Identifier()
        {
            var s = new Abc.Diagnostics.Session();
            Assert.AreNotEqual<Guid>(Guid.Empty, s.Identifier);
        }

        [TestMethod]
        public void GetSession()
        {
            Assert.AreNotEqual<Guid>(Guid.Empty, Abc.Diagnostics.Session.GetSession());

            Abc.Diagnostics.Session.ReleaseSession();
        }

        [TestMethod]
        public void InstantSession()
        {
            Assert.AreNotEqual<Guid>(Guid.Empty, Abc.Diagnostics.Session.InstantSession());
        }

        [TestMethod]
        public void GetSameSession()
        {
            var data = Abc.Diagnostics.Session.GetSession();
            Assert.AreNotEqual<Guid>(Guid.Empty, data);
            var same = Abc.Diagnostics.Session.GetSession();
            Assert.AreEqual<Guid>(data, same);

            Abc.Diagnostics.Session.ReleaseSession();
            Abc.Diagnostics.Session.ReleaseSession();
        }

        [TestMethod]
        public void ReleaseSession()
        {
            Abc.Diagnostics.Session.ReleaseSession();
        }
        #endregion
    }
}