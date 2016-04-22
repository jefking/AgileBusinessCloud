// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='ConvertTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ConvertTest
    {
        #region Valid Cases
        [TestMethod]
        public void FromString()
        {
            var data = StringHelper.ValidString();
            Assert.AreEqual<string>(data, Convert.FromString<string>(data));
        }

        [TestMethod]
        public void FromStringString()
        {
            var data = StringHelper.ValidString();
            Assert.AreEqual<string>(data, Convert.FromString<string>(data, null));
        }

        [TestMethod]
        public void FromStringInt()
        {
            var random = new System.Random();
            var data = random.Next();
            Assert.AreEqual<int>(data, Convert.FromString<int>(data.ToString()));
        }

        [TestMethod]
        public void FromStringGuid()
        {
            var data = System.Guid.NewGuid();
            Assert.AreEqual<System.Guid>(data, Convert.FromString<System.Guid>(data.ToString()));
        }

        [TestMethod]
        public void FromStringDateTime()
        {
            var data = System.DateTime.UtcNow;
            Assert.AreEqual<System.DateTime>(data.Date, Convert.FromString<System.DateTime>(data.ToString()).Date);
        }

        [TestMethod]
        public void FromStringLong()
        {
            var random = new System.Random();
            var data = random.Next();
            Assert.AreEqual<long>(data, Convert.FromString<long>(data.ToString()));
        }

        [TestMethod]
        public void FromStringDecimal()
        {
            var random = new System.Random();
            var data = (decimal)random.NextDouble();
            Assert.AreEqual<decimal>(data, Convert.FromString<decimal>(data.ToString()));
        }

        [TestMethod]
        public void FromStringDouble()
        {
            var random = new System.Random();
            var data = random.NextDouble();
            Assert.AreEqual(data, Convert.FromString<double>(data.ToString()), 1);
        }

        [TestMethod]
        public void FromStringByte()
        {
            var random = new System.Random();
            var data = (byte)random.Next();
            Assert.AreEqual<byte>(data, Convert.FromString<byte>(data.ToString()));
        }

        [TestMethod]
        public void FromStringChar()
        {
            var random = new System.Random();
            var data = (char)random.Next();
            Assert.AreEqual<char>(data, Convert.FromString<char>(data.ToString()));
        }

        [TestMethod]
        public void FromStringShort()
        {
            var random = new System.Random();
            var data = (short)random.Next(short.MaxValue);
            Assert.AreEqual<short>(data, Convert.FromString<short>(data.ToString()));
        }

        [TestMethod]
        public void FromStringUInt()
        {
            var random = new System.Random();
            var data = (uint)random.Next(55444);
            Assert.AreEqual<uint>(data, Convert.FromString<uint>(data.ToString()));
        }

        [TestMethod]
        public void FromStringSingle()
        {
            var random = new System.Random();
            var data = (float)random.Next(9451151);
            Assert.AreEqual<float>(data, Convert.FromString<float>(data.ToString()));
        }

        [TestMethod]
        public void FromStringUShort()
        {
            var random = new System.Random();
            var data = (ushort)random.Next(ushort.MaxValue);
            Assert.AreEqual<ushort>(data, Convert.FromString<ushort>(data.ToString()));
        }

        [TestMethod]
        public void FromStringBool()
        {
            Assert.IsTrue(Convert.FromString<bool>(bool.TrueString));
            Assert.IsFalse(Convert.FromString<bool>(bool.FalseString));
        }

        [TestMethod]
        public void FromStringBoolDefault()
        {
            var invalidConversion = StringHelper.ValidString();
            Assert.IsTrue(Convert.FromString<bool>(invalidConversion, true));
            Assert.IsFalse(Convert.FromString<bool>(invalidConversion, false));
        }

        [TestMethod]
        public void FromStringIntDefault()
        {
            var invalidConversion = StringHelper.ValidString() + "  ";
            var random = new System.Random();
            var data = random.Next();
            Assert.AreEqual<int>(data, Convert.FromString<int>(invalidConversion, data));
        }

        [TestMethod]
        public void FromStringGuidDefault()
        {
            var invalidConversion = StringHelper.ValidString();
            var data = System.Guid.NewGuid();
            Assert.AreEqual<System.Guid>(data, Convert.FromString<System.Guid>(invalidConversion, data));
        }

        [TestMethod]
        public void FromStringDateTimeDefault()
        {
            var invalidConversion = StringHelper.ValidString();
            var data = System.DateTime.UtcNow;
            Assert.AreEqual<System.DateTime>(data, Convert.FromString<System.DateTime>(invalidConversion, data));
        }

        [TestMethod]
        public void FromStringLongDefault()
        {
            var invalidConversion = StringHelper.ValidString();
            var random = new System.Random();
            var data = random.Next();
            Assert.AreEqual<long>(data, Convert.FromString<long>(invalidConversion, data));
        }

        [TestMethod]
        public void FromStringDecimalDefault()
        {
            var invalidConversion = StringHelper.ValidString();
            var random = new System.Random();
            var data = (decimal)random.NextDouble();
            Assert.AreEqual<decimal>(data, Convert.FromString<decimal>(invalidConversion, data));
        }

        [TestMethod]
        public void FromStringDoubleDefault()
        {
            var invalidConversion = StringHelper.ValidString();
            var random = new System.Random();
            var data = random.NextDouble();
            Assert.AreEqual<double>(data, Convert.FromString<double>(invalidConversion, data));
        }

        [TestMethod]
        public void FromStringByteDefault()
        {
            var invalidConversion = StringHelper.ValidString() + "++";
            var random = new System.Random();
            var data = (byte)random.Next();
            Assert.AreEqual<byte>(data, Convert.FromString<byte>(invalidConversion, data));
        }

        [TestMethod]
        public void FromStringCharDefault()
        {
            var invalidConversion = StringHelper.ValidString();
            var random = new System.Random();
            var data = (char)random.Next();
            Assert.AreEqual<char>(data, Convert.FromString<char>(invalidConversion, data));
        }

        [TestMethod]
        public void FromStringShortDefault()
        {
            var invalidConversion = StringHelper.ValidString();
            var random = new System.Random();
            var data = (short)random.Next(short.MaxValue);
            Assert.AreEqual<short>(data, Convert.FromString<short>(invalidConversion, data));
        }

        [TestMethod]
        [Ignore]
        public void FromStringUIntDefault()
        {
            var invalidConversion = StringHelper.ValidString();
            var random = new System.Random();
            var data = (uint)random.Next(55444);
            Assert.AreEqual<uint>(data, Convert.FromString<uint>(invalidConversion, data));
        }

        [TestMethod]
        public void FromStringSingleDefault()
        {
            var invalidConversion = StringHelper.ValidString();
            var random = new System.Random();
            var data = (float)random.Next(9451151);
            Assert.AreEqual<float>(data, Convert.FromString<float>(invalidConversion, data));
        }

        [TestMethod]
        public void FromStringUShortDefault()
        {
            var invalidConversion = StringHelper.ValidString();
            var random = new System.Random();
            var data = (ushort)random.Next(ushort.MaxValue);
            Assert.AreEqual<ushort>(data, Convert.FromString<ushort>(invalidConversion, data));
        }

        [TestMethod]
        public void FromStringToEnum()
        {
            var data = UnitTestOutcome.Inconclusive.ToString();
            Assert.AreEqual<UnitTestOutcome>(UnitTestOutcome.Inconclusive, Convert.FromString<UnitTestOutcome>(data, UnitTestOutcome.Passed));
        }

        [TestMethod]
        public void FromStringToEnumDefault()
        {
            var invalidConversion = StringHelper.ValidString();
            var data = DataAccessMethod.Sequential;
            Assert.AreEqual<DataAccessMethod>(data, Convert.FromString<DataAccessMethod>(invalidConversion, data));
        }
        #endregion
    }
}