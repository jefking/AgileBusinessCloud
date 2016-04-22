// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='Ascii85Test.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite
{
    using System;
    using System.IO;
    using Abc.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    
    /// <summary>
    /// This is a test class for Ascii85Test and is intended to contain all Ascii85Test Unit Tests
    /// </summary>
    [TestClass]
    public class Ascii85Test
    {
        #region Error Cases
        /// <summary>
        /// Decode Invalid String
        /// </summary>
        [ExpectedException(typeof(FormatException))]
        [TestMethod]
        public void DecodeInvalidStringZ()
        {
            Ascii85 target = new Ascii85();
            target.Decode("asdzas");
        }

        /// <summary>
        /// Decode Invalid String
        /// </summary>
        [ExpectedException(typeof(FormatException))]
        [TestMethod]
        public void DecodeInvalidStringU()
        {
            Ascii85 target = new Ascii85();
            target.Decode("v");
        }

        /// <summary>
        /// Decode Invalid String
        /// </summary>
        [ExpectedException(typeof(InvalidDataException))]
        [TestMethod]
        public void DecodeInvalidLastBlock()
        {
            Ascii85 target = new Ascii85();
            target.Decode("as!das");
        }
        #endregion

        #region Valid Cases
        /// <summary>
        /// Decode Invalid String
        /// </summary>
        [TestMethod]
        public void DecodeInvalidString()
        {
            Ascii85 target = new Ascii85();
            Assert.IsNull(target.Decode(StringHelper.NullEmptyWhiteSpace()));
        }

        /// <summary>
        /// A test for Decode
        /// </summary>
        [TestMethod]
        public void DecodeTest()
        {
            Ascii85 target = new Ascii85();
            Guid original = Guid.NewGuid();
            string encoded = target.Encode(original.ToByteArray());
            var actual = target.Decode(encoded);
            Assert.AreEqual<Guid>(original, new Guid(actual));
        }

        /// <summary>
        /// A test for Encode
        /// </summary>
        [TestMethod]
        public void EncodeTest()
        {
            Ascii85 target = new Ascii85();
            Guid val = "462b9417-77e5-4681-aafc-4b40529362d6".ToGuid();
            byte[] binary = val.ToByteArray();
            string expected = "(R-F>j`c8FWr,LT;NkS@";
            string actual = target.Encode(binary);
            Assert.AreEqual(expected, actual);
        }
        #endregion
    }
}