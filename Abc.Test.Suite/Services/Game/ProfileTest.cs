// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='Profile.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Services.Game
{
    using Abc.Services.Data;
    using Abc.Services.Game;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ProfileTest
    {
        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new Profile();
        }

        [TestMethod]
        public void IsIProfile()
        {
            Assert.IsNotNull(new Profile() as IProfile);
        }

        [TestMethod]
        public void EvaluateNull()
        {
            var item = new Profile();
            Assert.AreEqual<int>(0, item.Evaluate(null));
        }

        [TestMethod]
        public void EvaluateValid()
        {
            var item = new Profile();
            Assert.AreEqual<int>(0, item.Evaluate(new UserPublicProfile()));
        }

        [TestMethod]
        public void EvaluatePrefered()
        {
            var item = new Profile();
            var data = new UserPublicProfile()
            {
                PreferedProfile = true,
            };

            Assert.AreEqual<int>(500, item.Evaluate(data));
        }

        [TestMethod]
        public void EvaluateWord()
        {
            var item = new Profile();
            var data = new UserPublicProfile()
            {
                Word = StringHelper.ValidString(),
            };

            Assert.AreEqual<int>(200, item.Evaluate(data));
        }

        [TestMethod]
        public void EvaluateGithubHandle()
        {
            var item = new Profile();
            var data = new UserPublicProfile()
            {
                GitHubHandle = StringHelper.ValidString(),
            };

            Assert.AreEqual<int>(100, item.Evaluate(data));
        }

        [TestMethod]
        public void EvaluateTwitterHandle()
        {
            var item = new Profile();
            var data = new UserPublicProfile()
            {
                TwitterHandle = StringHelper.ValidString(),
            };

            Assert.AreEqual<int>(100, item.Evaluate(data));
        }

        [TestMethod]
        public void EvaluateAbcHandle()
        {
            var item = new Profile();
            var data = new UserPublicProfile()
            {
                Handle = StringHelper.ValidString(),
            };

            Assert.AreEqual<int>(100, item.Evaluate(data));
        }
        #endregion
    }
}