// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='WebResponseTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Website
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Abc.Web;
    using Abc.Website;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class WebResponseTest
    {
        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            var wr = new WebResponse();
            Assert.IsTrue(wr.Successful);
            Assert.IsNull(wr.Errors);
        }

        [TestMethod]
        public void ConstructorNullErrors()
        {
            var wr = new WebResponse(null);
            Assert.IsTrue(wr.Successful);
            Assert.IsNull(wr.Errors);
        }

        [TestMethod]
        public void ConstructorEmptyErrors()
        {
            var wr = new WebResponse(new List<Error>(0));
            Assert.IsTrue(wr.Successful);
            Assert.IsNotNull(wr.Errors);
        }

        [TestMethod]
        public void ConstructorErrors()
        {
            var random = new Random();
            var count = random.Next(50);
            var errors = new List<Error>(count);
            for (int i = 0; i < count; i++)
            {
                errors.Add(this.Err());
            }

            var wr = new WebResponse(errors);
            Assert.IsFalse(wr.Successful);
            Assert.AreEqual<int>(count, wr.Errors.Count());
        }

        [TestMethod]
        public void Successful()
        {
            var wr = new WebResponse(new List<Error>(0));
            Assert.IsTrue(wr.Successful);
            wr.Successful = false;
            Assert.IsFalse(wr.Successful);
        }

        [TestMethod]
        public void Errors()
        {
            var random = new Random();
            var wr = new WebResponse();
            var count = random.Next(50);
            var errors = new List<Error>(count);
            Parallel.For(0, count, (a, b) => errors.Add(this.Err()));
            wr.Errors = errors;
            Assert.AreEqual<int>(errors.Count, wr.Errors.Count());
        }

        [TestMethod]
        public void Bind()
        {
            var random = new Random();
            var data = StringHelper.ValidString();
            var code = random.Next();
            var response = WebResponse.Bind(code, data);
            Assert.IsFalse(response.Successful);
            Assert.AreEqual<int>(1,  response.Errors.Count());
            var error = response.Errors.First();
            Assert.AreEqual<string>(data, error.Message);
            Assert.AreEqual<int>(code, error.Code);
        }
        #endregion

        #region Helper Methods
        private Error Err()
        {
            var random = new Random();
            return new Error()
            {
                Code = random.Next(),
                Message = StringHelper.ValidString(),
            };
        }
        #endregion
    }
}