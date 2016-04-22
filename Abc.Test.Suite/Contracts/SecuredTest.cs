// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='SecuredTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Contracts
{
    using System;
    using Abc.Services.Contracts;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SecuredTest
    {
        #region Valid Cases
        [TestMethod]
        public void Token()
        {
            var token = new Token()
            {
                ApplicationId = Guid.NewGuid(),
                ValidationKey = StringHelper.ValidString(),
            };

            var secured = (Secured)new Message()
            {
                Token = token,
            };

            Assert.AreEqual<Token>(token, secured.Token);
        }
        #endregion
    }
}