// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='ExtensionMethodsTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Website
{
    using System;
    using Abc.Services;
    using Abc.Website;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Extension Methods Test
    /// </summary>
    [TestClass]
    public class ExtensionMethodsTest
    {
        #region Error Cases
        #endregion

        #region Valid Cases
        [TestMethod]
        public void OccurrenceDataExecutionTime()
        {
            var random = new Random();
            var ts = new TimeSpan(random.Next());
            var od = new OccurrenceData()
            {
                Duration = ts.Ticks
            };
            Assert.AreEqual<TimeSpan>(ts, od.ExecutionTime());
        }

        [TestMethod]
        public void ConvertPublicProfile()
        {
            var user = new Abc.Services.Data.UserPublicProfile()
            {
                CreatedOn = DateTime.UtcNow,
                UserName = StringHelper.ValidString(),
                Gravatar = StringHelper.ValidString(),
                Handle = StringHelper.ValidString(),
                PreferedProfile = true,
            };

            var profile = user.Convert();
            Assert.IsNotNull(profile);
            Assert.AreEqual<DateTime>(user.CreatedOn, profile.CreatedOn);
            Assert.AreEqual<string>(user.UserName, profile.UserName);
            Assert.AreEqual<string>(user.Handle, profile.AbcHandle);
            Assert.AreEqual<string>(user.Gravatar, profile.Gravatar);
            Assert.IsTrue(profile.PreferedProfile);
        }
        #endregion
    }
}