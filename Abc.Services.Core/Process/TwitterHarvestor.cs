// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='TwitterHarvestor.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Process
{
    using Abc.Azure;
    using Abc.Configuration;
    using Abc.Services.Contracts;
    using Abc.Services.Core;
    using Abc.Services.Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Twitter Harvestor
    /// </summary>
    public class TwitterHarvestor : ScheduledManager
    {
        #region Members
        /// <summary>
        /// User Core
        /// </summary>
        private readonly IUserCore userCore;

        /// <summary>
        /// Container
        /// </summary>
        private readonly ITextContainer<CodeStormSocial> container;

        /// <summary>
        /// Twitter Source
        /// </summary>
        private readonly ITwitterSource twitter;
        #endregion

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        public TwitterHarvestor()
            : this(new UserCore(), new JsonPContainer<CodeStormSocial>(ServerConfiguration.CodeStormData, "CodeStormRenderProfile"), new TwitterSource())
        {
        }

        /// <summary>
        /// Fully Loaded.
        /// </summary>
        /// <param name="userCore">User Core</param>
        /// <param name="container">Container</param>
        /// <param name="twitter">Twitter Source</param>
        public TwitterHarvestor(IUserCore userCore, ITextContainer<CodeStormSocial> container, ITwitterSource twitter)
            : base(30, (int)TimeSpan.FromHours(12).TotalSeconds)
        {
            if (null == userCore)
            {
                throw new ArgumentNullException("userCore");
            }
            else if (null == container)
            {
                throw new ArgumentNullException("container");
            }
            else if (null == twitter)
            {
                throw new ArgumentNullException("twitter");
            }
            else
            {
                this.userCore = userCore;
                this.container = container;
                this.twitter = twitter;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Execute
        /// </summary>
        public override void Execute()
        {
            using (new PerformanceMonitor())
            {
                var profiles = userCore.PublicProfilesFull(Application.Default);
                foreach (var profile in profiles)
                {
                    var social = new CodeStormSocial()
                    {
                        AbcHandle = profile.Handle,
                        TwitterHandle = profile.TwitterHandle,
                        GitHubHandle = profile.GitHubHandle,
                    };

                    try
                    {
                        if (!string.IsNullOrWhiteSpace(social.TwitterHandle))
                        {
                            var tweets = twitter.ByUser(social.TwitterHandle, 5);
                            var links = new List<string>();
                            var mentions = new List<string>();
                            foreach (var tweet in tweets)
                            {
                                links.AddRange(tweet.ParseLinks());
                                mentions.AddRange(tweet.ParseMentions().Distinct());
                            }

                            var twitterMentions = new List<Mention>();
                            foreach (var mention in mentions.Distinct())
                            {
                                var userMention = new Mention()
                                {
                                    TwitterHandle = mention,
                                };

                                userMention.AbcHandle = (from data in profiles
                                                         where data.TwitterHandle == mention
                                                         select data.Handle).FirstOrDefault();

                                twitterMentions.Add(userMention);

                            }

                            social.TwitterLinks = links;

                            social.TwitterMentions = twitterMentions;
                        }
                        else
                        {
                            log.Log("Twitter Handle is empty.");
                        }
                    }
                    catch (Exception ex)
                    {
                        base.log.Log(ex, EventTypes.Error, (int)ServiceFault.Unknown);
                    }

                    container.Save(social.AbcHandle, social);
                }
            }
        }
        #endregion
    }
}