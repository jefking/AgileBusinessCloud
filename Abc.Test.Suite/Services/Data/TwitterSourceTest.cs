// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='TwitterSourceTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Data
{
    using Abc.Services.Data;
    using Abc.Text;
    using LinqToTwitter;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Linq;

    /// <summary>
    /// Twitter Source Test
    /// </summary>
    [TestClass]
    public class TwitterSourceTest
    {
        #region Valid Cases
        [TestMethod]
        public void Constructor()
        {
            new TwitterSource();
        }

        [TestMethod]
        public void IsITwitterSource()
        {
            Assert.IsNotNull(new TwitterSource() as ITwitterSource);
        }

        [TestMethod]
        public void JefKingAbc()
        {
            Assert.AreEqual<string>("jefkingabc".ToLowerInvariant(), TwitterSource.JefKing.ToLowerInvariant());
        }

        [TestMethod]
        public void GeorgeDanes()
        {
            Assert.AreEqual<string>("georgedanesabc".ToLowerInvariant(), TwitterSource.GeorgeDanes.ToLowerInvariant());
        }

        [TestMethod]
        public void Tweets()
        {
            var source = new TwitterSource();
            var data = source.ByUser("weareabc", 5).ToList();
            Assert.IsNotNull(data);
            Assert.AreEqual<int>(5, data.Count());
        }

        [TestMethod]
        public void Employees()
        {
            var source = new TwitterSource();
            var data = source.Employees(5);
            Assert.IsNotNull(data);
            Assert.IsTrue(5 * 3 >= data.Count());
            foreach (var item in data)
            {
                if (item.ScreenName.ToLowerInvariant() == TwitterSource.JefKing.ToLowerInvariant()
                    || item.ScreenName.ToLowerInvariant() == TwitterSource.GeorgeDanes.ToLowerInvariant()
                    || item.ScreenName.ToLowerInvariant() == TwitterSource.JaimeBueza.ToLowerInvariant())
                {
                    continue;
                }
                else
                {
                    Assert.Fail(string.Format("Unknown User Name: {0}", item.User.ScreenName));
                }

                if (item.Text.Contains('@') || item.Text.Contains('#'))
                {
                    if (!RegexStatement.Url.IsMatch(item.Text))
                    {
                        Assert.Fail(string.Format("Item Contains @ | # yet doesn't have link to twitter: '{0}'", item.Text));
                    }
                }
            }
        }

        [TestMethod]
        public void RenderHtmlLink()
        {
            var status = new Status()
            {
                Text = "hey link land: http://t.com/happy click it",
            };
            
            var source = new TwitterSource();

            var html = source.RenderHtml(status);
            Assert.AreEqual<string>("hey link land: <a href=\"http://t.com/happy\" target=\"_blank\">http://t.com/happy</a> click it", html);
        }

        [TestMethod]
        public void RenderHtmlMention()
        {
            var status = new Status()
            {
                Text = "Hey @someone check this out!",
            };

            var source = new TwitterSource();

            var html = source.RenderHtml(status);
            Assert.AreEqual<string>("Hey @<a href=\"http://www.twitter.com/#!/someone\" target=\"_blank\">someone</a> check this out!", html);
        }

        [TestMethod]
        public void RenderHtmlKeyword()
        {
            var status = new Status()
            {
                Text = "This is so cool #Dude check yourself.",
            };

            var source = new TwitterSource();

            var html = source.RenderHtml(status);
            Assert.AreEqual<string>("This is so cool <a href=\"http://www.twitter.com/#!/search?q=Dude\" target=\"_blank\">#Dude</a> check yourself.", html);
        }

        [TestMethod]
        public void RenderHtml()
        {
            var status = new Status()
            {
                Text = "Hey @someone check #this out! http://happy.com/land",
            };

            var source = new TwitterSource();

            var html = source.RenderHtml(status);
            Assert.AreEqual<string>("Hey @<a href=\"http://www.twitter.com/#!/someone\" target=\"_blank\">someone</a> check <a href=\"http://www.twitter.com/#!/search?q=this\" target=\"_blank\">#this</a> out! <a href=\"http://happy.com/land\" target=\"_blank\">http://happy.com/land</a>", html);
        }
        #endregion
    }
}