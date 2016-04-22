// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='RegexStatement.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Text
{
    using System.Text.RegularExpressions;

    /// <summary>
    /// Regex Statement
    /// </summary>
    public static class RegexStatement
    {
        #region Members
        /// <summary>
        /// URL Regex
        /// </summary>
        public static readonly Regex Url = new Regex(UrlStatement);

        /// <summary>
        /// Twitter Keyword
        /// </summary>
        public static readonly Regex TwitterKeyword = new Regex(TwitterKeywordStatement);

        /// <summary>
        /// Twitter Follower
        /// </summary>
        public static readonly Regex TwitterFollower = new Regex(TwitterFollowerStatement);

        /// <summary>
        /// GitHub Authentication Response
        /// </summary>
        public static readonly Regex GitHubAuthenticationResponse = new Regex(GitHubAuthenticationResponseStatement);

        /// <summary>
        /// PayPal Response Regxe
        /// </summary>
        public static readonly Regex PayPalResponse = new Regex(PayPalResponseStatement, RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

        /// <summary>
        /// URL Statement
        /// </summary>
        public const string UrlStatement = @"(?<Url>(https?|ftp|gopher|telnet|file|notes|ms-help):((//)|(\\\\))+[\w\d:#@%/;$()~_?\+-=\\\.&]*)";

        /// <summary>
        /// PayPay Response Statement
        /// </summary>
        public const string PayPalResponseStatement = @"^(?<key>[a-zA-Z_]+)[=](?<value>.*)\b";

        /// <summary>
        /// Tweet: Keyword Processing Regex Statement
        /// </summary>
        public const string TwitterKeywordStatement = @"[#]{1}(?<Value>\w+)";

        /// <summary>
        /// Tweet: Follower Processing Regex Statement
        /// </summary>
        public const string TwitterFollowerStatement = @"[@]{1}(?<Value>\w+)";

        /// <summary>
        /// GitHub Authentication Response Statement
        /// </summary>
        public const string GitHubAuthenticationResponseStatement = @"access_token=(?<accessToken>[\w]*)&token_type=(?<tokenType>[a-zA-Z]*)";
        #endregion
    }
}