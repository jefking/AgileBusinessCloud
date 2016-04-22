// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='PriorityCollectionTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Global.Collections
{
    using System;
    using Abc.Collections;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Priority Collection Test
    /// </summary>
    [TestClass]
    public class PriorityCollectionTest
    {
        #region Valid Cases
        [TestMethod]
        public void PriorityCollectionEnumerate()
        {
            PriorityCollection<DateTime> pc = new PriorityCollection<DateTime>();
            pc.Add(DateTime.Now);
            pc.Add(DateTime.MaxValue);
            pc.Add(DateTime.MinValue);

            DateTime? last = null;
            foreach (DateTime dt in pc)
            {
                if (null != last)
                {
                    Assert.IsTrue(dt > last, "Order should be least to greatest.");
                }

                last = dt;
            }
        }

        [TestMethod]
        public void PriorityCollectionPop()
        {
            PriorityCollection<int> pc = new PriorityCollection<int>();
            pc.Add(0);
            pc.Add(100);
            pc.Add(-100);

            Assert.AreEqual<int>(100, pc.Pop());
        }

        [TestMethod]
        public void PriorityCollectionMinMax()
        {
            PriorityCollection<double> pc = new PriorityCollection<double>();
            pc.Add(123.123);
            pc.Add(999.999);
            pc.Add(111.111);

            Assert.AreEqual<double>(111.111, pc.Min);
            Assert.AreEqual<double>(999.999, pc.Max);
        }

        [TestMethod]
        public void PriorityCollectionICollection()
        {
            PriorityCollection<DateTime> pc = new PriorityCollection<DateTime>();
            pc.Add(DateTime.Now);
            pc.Add(DateTime.MaxValue);
            pc.Add(DateTime.MinValue);

            Assert.AreEqual<bool>(false, pc.IsReadOnly);
            Assert.AreEqual<int>(3, pc.Count);
            Assert.AreEqual<bool>(true, pc.Contains(DateTime.MaxValue));
            Assert.AreEqual<bool>(true, pc.Remove(DateTime.MaxValue));
            pc.Clear();
            Assert.AreEqual<int>(0, pc.Count);
        }
        #endregion
    }
}