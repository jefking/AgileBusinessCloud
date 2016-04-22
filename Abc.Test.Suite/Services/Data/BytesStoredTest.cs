// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='BytesStoredTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite
{
    using System;
    using Abc.Services;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Bytes Stored Test Class
    /// </summary>
    [TestClass]
    public class BytesStoredTest
    {
        #region Error Cases
        /// <summary>
        /// Construct with Empty Guid
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ConstructureEmptyGuid()
        {
            new BytesStoredData(Guid.Empty);
        }
        #endregion
    }
}