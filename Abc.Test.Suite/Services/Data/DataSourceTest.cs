// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='DataSourceTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Domain Data Source Test
    /// </summary>
    [TestClass]
    public class DataSourceTest
    {
        #region Valid Cases
        [TestMethod]
        public void MaximumStringLength()
        {
            Assert.AreEqual<int>(32000, DataSource.MaximumStringLength);
        }
        #endregion
    }
}