namespace Abc.Test.Suite.Threading
{
    using System;
    using System.Threading;
    using Abc.Threading;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SafeTimerTest
    {
        #region Members
        private volatile int errorCount = 0;
        private volatile int validCount = 0;
        #endregion

        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new SafeTimer(null, null, TimeSpan.Zero, TimeSpan.Zero);
        }

        [TestMethod]
        public void ConstructorWithOnError()
        {
            new SafeTimer(null, null, null, TimeSpan.Zero, TimeSpan.Zero);
        }

        [TestMethod]
        public void Dispose()
        {
            using (new SafeTimer(null, null, TimeSpan.Zero, TimeSpan.Zero))
            {
            }
        }

        [TestMethod]
        [Ignore]
        public void ErrorMethod()
        {
            this.errorCount = 0;
            using (new SafeTimer(this.Error, this.OnError, null, TimeSpan.Zero, TimeSpan.FromSeconds(1)))
            {
                var i = 0;
                while (0 == this.errorCount && 3 > i)
                {
                    Thread.Sleep(10);
                    i++;
                }

                Assert.AreEqual<int>(1, this.errorCount);
                Assert.AreNotEqual<int>(3, i);
            }
        }

        [TestMethod]
        [Ignore]
        public void ValidMethod()
        {
            this.validCount = 0;
            using (new SafeTimer(this.Valid, null, null, TimeSpan.Zero, TimeSpan.FromSeconds(1)))
            {
                var i = 0;
                while (0 == this.validCount && 3 > i)
                {
                    Thread.Sleep(10);
                    i++;
                }

                Assert.AreEqual<int>(1, this.validCount);
                Assert.AreNotEqual<int>(3, i);
            }
        }
        #endregion

        #region Test Methods
        private void Error(object state)
        {
            throw new ApplicationException();
        }

        private void Valid(object state)
        {
            this.validCount = 1;
        }

        private void OnError(object sender, EventArgs<Exception> args)
        {
            this.errorCount = 1;
        }
        #endregion
    }
}