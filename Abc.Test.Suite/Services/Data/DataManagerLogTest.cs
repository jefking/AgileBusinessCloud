namespace Abc.Test.Suite.Data
{
    using Abc.Services.Data;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    [TestClass]
    public class DataManagerLogTest
    {
        #region Error Cases
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorCallerNull()
        {
            new DataManagerLog(null);
        }
        #endregion

        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new DataManagerLog();
        }

        [TestMethod]
        public void PartitionKey()
        {
            var startDate = DateTime.UtcNow;
            var item = new DataManagerLog(this.GetType());
            Assert.AreEqual<string>(string.Format("{0}{1}{2}", this.GetType(), startDate.Year, startDate.Month), item.PartitionKey);
        }

        [TestMethod]
        public void RowKey()
        {
            var item = new DataManagerLog(this.GetType());
            Assert.IsFalse(string.IsNullOrWhiteSpace(item.RowKey));
            Assert.AreNotEqual<Guid>(Guid.Empty, Guid.Parse(item.RowKey));
        }

        [TestMethod]
        public void StartTime()
        {
            var item = new DataManagerLog(this.GetType());
            var data = DateTime.UtcNow;
            item.StartTime = data;
            Assert.AreEqual<DateTime>(data, item.StartTime);
        }

        [TestMethod]
        public void CompletionTime()
        {
            var item = new DataManagerLog(this.GetType());
            Assert.IsNull(item.CompletionTime);
            var data = DateTime.UtcNow;
            item.CompletionTime = data;
            Assert.AreEqual<DateTime?>(data, item.CompletionTime);
        }

        [TestMethod]
        public void Successful()
        {
            var item = new DataManagerLog(this.GetType());
            Assert.IsFalse(item.Successful);
            item.Successful = true;
            Assert.IsTrue(item.Successful);
        }
        #endregion
    }
}