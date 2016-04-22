// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='TwitterSource.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Data
{
    using Abc.Services.Contracts;
    using Abc.Services.Core;
    using Abc.Text;
    using LinqToTwitter;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    
    /// <summary>
    /// Twitter Source
    /// </summary>
    public class TwitterSource : ITwitterSource
    {
        #region Members
        /// <summary>
        /// Jef King
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Jef", Justification = "Name")]
        public const string JefKing = "jefkingabc";

        /// <summary>
        /// George Danes
        /// </summary>
        public const string GeorgeDanes = "georgedanesabc";

        /// <summary>
        /// Jaime Bueza
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Bueza", Justification = "Persons name.")]
        public const string JaimeBueza = "jaimebuezaabc";

        /// <summary>
        /// Twitter Url
        /// </summary>
        private const string TwitterUrl = "http://www.twitter.com/#!/";

        /// <summary>
        /// Logging
        /// </summary>
        private static readonly LogCore log = new LogCore();

        /// <summary>
        /// Twitter Authorization
        /// </summary>
        private readonly SingleUserAuthorizer authorization = new SingleUserAuthorizer
        {
            Credentials = new InMemoryCredentials
            {
                ConsumerKey = "u3dWsVzakfMTABPhbcjAfQ",
                ConsumerSecret = "n12w973GoJNBIb5FwtR0iph7DFbxkatWjbS6Zt8",
                OAuthToken = "455360778-QVe1zmyhyewW4qrzkJOJFyuAaiIHiukQ5iHZhp4U",
                AccessToken = "APNAIjowEwDNUIZoL4SxxLfal2dJuZTdNyrt2abrY",
            }
        };
        #endregion

        #region Methods
        /// <summary>
        /// Tweets By User
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="max">Maximum</param>
        /// <returns>Tweets</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Not meant to be static.")]
        public IQueryable<Status> ByUser(string user, int max)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(user));
            Contract.Requires(0 < max);
            
            using (var context = new TwitterContext(authorization))
            {
                return (from tweet in context.Status
                        where tweet.Type == StatusType.User
                        && tweet.ScreenName == user
                        select tweet).Take(max);
            }
        }

        /// <summary>
        /// Employees Tweets
        /// </summary>
        /// <param name="max">Maximum</param>
        /// <returns>Tweets</returns>
        [CLSCompliant(false)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Undetermined errors, will bug.")]
        public IOrderedEnumerable<Status> Employees(int max)
        {
            Contract.Requires(0 < max);
            Contract.Ensures(Contract.Result<IOrderedEnumerable<Status>>() != null);

            var tweets = new List<Status>(max * 3);

            try
            {
                tweets.AddRange(this.ByUser(JefKing, max));
                tweets.AddRange(this.ByUser(GeorgeDanes, max));
                tweets.AddRange(this.ByUser(JaimeBueza, max));
            }
            catch (Exception ex)
            {
                log.Log(ex, EventTypes.Error, (int)DatumFault.GeneralTwitterFailure);
            }

            return from tweet in tweets
                   orderby tweet.CreatedAt descending
                   select tweet;
        }

        /// <summary>
        /// Render HTML
        /// </summary>
        /// <param name="status">Status</param>
        /// <returns>Render HTML</returns>
        [CLSCompliant(false)]
        public string RenderHtml(Status status)
        {
            var sb = new StringBuilder(status.Text);
            foreach (var match in status.ParseLinks())
            {
                sb.Replace(match, "<a href=\"{0}\" target=\"_blank\">{0}</a>".FormatWithCulture(match));
            }

            foreach (var match in status.ParseMentions())
            {
                var url = "<a href=\"{0}{1}\" target=\"_blank\">{1}</a>".FormatWithCulture(TwitterUrl, match);
                sb.Replace(match, url);
            }

            foreach (Match match in RegexStatement.TwitterKeyword.Matches(sb.ToString()))
            {
                var value = match.Groups["Value"].Value;
                var url = "<a href=\"{0}search?q={1}\" target=\"_blank\">#{1}</a>".FormatWithCulture(TwitterUrl, value);
                sb.Replace(match.Value, url);
            }

            return sb.ToString();
        }
        #endregion
    }
}