// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='EventArgsTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class EventArgsTest
    {
        #region Valid Cases
        /// <summary>
        /// Constructor
        /// </summary>
        public void Constructor()
        {
            new EventArgs<object>(null);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public void Argument()
        {
            var guid = Guid.NewGuid();
            var args = new EventArgs<Guid>(guid);

            Assert.AreEqual<Guid>(guid, args.Argument);
        }
        #endregion
    }
}