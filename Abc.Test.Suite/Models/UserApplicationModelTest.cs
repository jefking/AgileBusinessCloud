// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='UserApplicationModelTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Abc.Services.Contracts;
    using Abc.Website.Models;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class UserApplicationModelTest
    {
        #region Valid Cases
        /// <summary>
        /// A test for Constructor
        /// </summary>
        [TestMethod]
        public void Constructor()
        {
            new UserApplicationModel();
        }

        [TestMethod]
        public void Users()
        {
            var binding = new UserApplicationModel();
            var data = new List<User>();
            var random = new Random();
            var count = random.Next(200);
            for (int i = 0; i < count; i++)
            {
                data.Add(new User());
            }

            binding.Users = data;
            Assert.AreEqual<int>(data.Count, binding.Users.Count());
        }

        [TestMethod]
        public void Applications()
        {
            var binding = new UserApplicationModel();
            var data = new List<ApplicationDetailsModel>();
            var random = new Random();
            var count = random.Next(200);
            for (int i = 0; i < count; i++)
            {
                data.Add(new ApplicationDetailsModel());
            }

            binding.Applications = data;
            Assert.AreEqual<int>(data.Count, binding.Applications.Count());
        }
        #endregion
    }
}