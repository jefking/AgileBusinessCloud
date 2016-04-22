// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='RegexStatementTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite
{
    using System.Text.RegularExpressions;
    using Abc.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class RegexStatementTest
    {
        #region Members
        private const string PayPalResponseValue = @"SUCCESS
first_name=Jane+Doe
last_name=Smith
payment_status=Completed 
payer_email=janiedoesmith@hotmail.com
payment_gross=3.99
mc_currency=USD 
custom=For+the+purchase+of+the+rare+book+Green+Eggs+%26+Ham";

        private const string PayPalResponseFull = @"mc_gross=0.01 
protection_eligibility=Eligible 
address_status=confirmed 
payer_id=LHXGFUDRHPPEL 
tax=0.00 
address_street=546+beatty%0D%0A309 
payment_date=18%3A49%3A02+Jan+30%2C+2012+PST 
payment_status=Completed 
charset=windows-1252 
address_zip=v6b2l3 
first_name=Jeffrey 
mc_fee=0.01 
address_country_code=CA 
address_name=Jeffrey+King 
custom= 
payer_status=unverified 
business=jef%40agilebusinesscloud.com 
address_country=Canada 
address_city=vancouver 
quantity=1 
payer_email=jamieking80%40gmail.com 
txn_id=5JW20211VT0352427 
payment_type=instant 
btn_id=41305525 
last_name=King 
address_state=British+Columbia 
receiver_email=jef%40agilebusinesscloud.com 
payment_fee= 
shipping_discount=0.00 
insurance_amount=0.00 
receiver_id=6ZLN7RZS446C2 
txn_type=web_accept 
item_name=One+Cent 
discount=0.00 
mc_currency=CAD 
item_number=0002 
residence_country=CA 
shipping_method=Default 
handling_amount=0.00 
transaction_subject=One+Cent 
payment_gross= 
shipping=0.00";
        #endregion

        #region Valid Cases
        [TestMethod]
        public void UrlStatement()
        {
            Assert.AreEqual<string>(@"(?<Url>(https?|ftp|gopher|telnet|file|notes|ms-help):((//)|(\\\\))+[\w\d:#@%/;$()~_?\+-=\\\.&]*)", RegexStatement.UrlStatement);
        }

        [TestMethod]
        public void PayPalResponseStatement()
        {
            Assert.AreEqual<string>(@"^(?<key>[a-zA-Z_]+)[=](?<value>.*)\b", RegexStatement.PayPalResponseStatement);
        }

        [TestMethod]
        public void TwitterKeywordStatement()
        {
            Assert.AreEqual<string>(@"[#]{1}(?<Value>\w+)", RegexStatement.TwitterKeywordStatement);
        }

        [TestMethod]
        public void TwitterFollowerStatement()
        {
            Assert.AreEqual<string>(@"[@]{1}(?<Value>\w+)", RegexStatement.TwitterFollowerStatement);
        }

        [TestMethod]
        public void GitHubAuthenticationResponseStatement()
        {
            Assert.AreEqual<string>(@"access_token=(?<accessToken>[\w]*)&token_type=(?<tokenType>[a-zA-Z]*)", RegexStatement.GitHubAuthenticationResponseStatement);
        }

        [TestMethod]
        public void PayPalResponse()
        {
            int i = 0;
            foreach (Match match in RegexStatement.PayPalResponse.Matches(PayPalResponseValue))
            {
                var value = match.Groups["value"].Value;
                switch (match.Groups["key"].Value)
                {
                    case "first_name":
                        i++;
                        Assert.AreEqual<string>("Jane+Doe", value);
                        continue;
                    case "last_name":
                        i++;
                        Assert.AreEqual<string>("Smith", value);
                        continue;
                    case "payment_status":
                        i++;
                        Assert.AreEqual<string>("Completed", value);
                        continue;
                    case "payer_email":
                        i++;
                        Assert.AreEqual<string>("janiedoesmith@hotmail.com", value);
                        continue;
                    case "payment_gross":
                        i++;
                        Assert.AreEqual<string>("3.99", value);
                        continue;
                    case "mc_currency":
                        i++;
                        Assert.AreEqual<string>("USD", value);
                        continue;
                    case "custom":
                        i++;
                        Assert.AreEqual<string>("For+the+purchase+of+the+rare+book+Green+Eggs+%26+Ham", value);
                        continue;
                    default:
                        Assert.Fail("Unknown Key");
                        break;
                }
            }

            Assert.AreEqual<int>(7, i);
        }

        [TestMethod]
        public void PayPalResponseEverything()
        {
            int i = 0;
            foreach (Match match in RegexStatement.PayPalResponse.Matches(PayPalResponseFull))
            {
                var value = match.Groups["value"].Value;
                switch (match.Groups["key"].Value)
                {
                    case "mc_gross":
                        i++;
                        Assert.AreEqual<string>("0.01", value);
                        continue;
                    case "protection_eligibility":
                        i++;
                        Assert.AreEqual<string>("Eligible", value);
                        continue;
                    case "address_status":
                        i++;
                        Assert.AreEqual<string>("confirmed", value);
                        continue;
                    case "payer_id":
                        i++;
                        Assert.AreEqual<string>("LHXGFUDRHPPEL", value);
                        continue;
                    case "tax":
                        i++;
                        Assert.AreEqual<string>("0.00", value);
                        continue;
                    case "address_street":
                        i++;
                        Assert.AreEqual<string>("546+beatty%0D%0A309", value);
                        continue;
                    case "payment_date":
                        i++;
                        Assert.AreEqual<string>("18%3A49%3A02+Jan+30%2C+2012+PST", value);
                        continue;
                    case "payment_status":
                        i++;
                        Assert.AreEqual<string>("Completed", value);
                        continue;
                    case "charset":
                        i++;
                        Assert.AreEqual<string>("windows-1252", value);
                        continue;
                    case "address_zip":
                        i++;
                        Assert.AreEqual<string>("v6b2l3", value);
                        continue;
                    case "first_name":
                        i++;
                        Assert.AreEqual<string>("Jeffrey", value);
                        continue;
                    case "mc_fee":
                        i++;
                        Assert.AreEqual<string>("0.01", value);
                        continue;
                    case "address_country_code":
                        i++;
                        Assert.AreEqual<string>("CA", value);
                        continue;
                    case "address_name":
                        i++;
                        Assert.AreEqual<string>("Jeffrey+King", value);
                        continue;
                    case "payment_fee":
                    case "custom":
                    case "payment_gross":
                        i++;
                        Assert.AreEqual<string>(string.Empty, value);
                        continue;
                    case "payer_status":
                        i++;
                        Assert.AreEqual<string>("unverified", value);
                        continue;
                    case "quantity":
                        i++;
                        Assert.AreEqual<string>("1", value);
                        continue;
                    case "business":
                        i++;
                        Assert.AreEqual<string>("jef%40agilebusinesscloud.com", value);
                        continue;
                    case "address_country":
                        i++;
                        Assert.AreEqual<string>("Canada", value);
                        continue;
                    case "address_city":
                        i++;
                        Assert.AreEqual<string>("vancouver", value);
                        continue;
                    case "payer_email":
                        i++;
                        Assert.AreEqual<string>("jamieking80%40gmail.com", value);
                        continue;
                    case "txn_id":
                        i++;
                        Assert.AreEqual<string>("5JW20211VT0352427", value);
                        continue;
                    case "payment_type":
                        i++;
                        Assert.AreEqual<string>("instant", value);
                        continue;
                    case "btn_id":
                        i++;
                        Assert.AreEqual<string>("41305525", value);
                        continue;
                    case "last_name":
                        i++;
                        Assert.AreEqual<string>("King", value);
                        continue;
                    case "address_state":
                        i++;
                        Assert.AreEqual<string>("British+Columbia", value);
                        continue;
                    case "receiver_email":
                        i++;
                        Assert.AreEqual<string>("jef%40agilebusinesscloud.com", value);
                        continue;
                    case "shipping_discount":
                        i++;
                        Assert.AreEqual<string>("0.00", value);
                        continue;
                    case "insurance_amount":
                        i++;
                        Assert.AreEqual<string>("0.00", value);
                        continue;
                    case "receiver_id":
                        i++;
                        Assert.AreEqual<string>("6ZLN7RZS446C2", value);
                        continue;
                    case "txn_type":
                        i++;
                        Assert.AreEqual<string>("web_accept", value);
                        continue;
                    case "item_name":
                        i++;
                        Assert.AreEqual<string>("One+Cent", value);
                        continue;
                    case "discount":
                        i++;
                        Assert.AreEqual<string>("0.00", value);
                        continue;
                    case "mc_currency":
                        i++;
                        Assert.AreEqual<string>("CAD", value);
                        continue;
                    case "item_number":
                        i++;
                        Assert.AreEqual<string>("0002", value);
                        continue;
                    case "residence_country":
                        i++;
                        Assert.AreEqual<string>("CA", value);
                        continue;
                    case "shipping_method":
                        i++;
                        Assert.AreEqual<string>("Default", value);
                        continue;
                    case "handling_amount":
                        i++;
                        Assert.AreEqual<string>("0.00", value);
                        continue;
                    case "transaction_subject":
                        i++;
                        Assert.AreEqual<string>("One+Cent", value);
                        continue;
                    case "shipping":
                        i++;
                        Assert.AreEqual<string>("0.00", value);
                        continue;
                    default:
                        Assert.Fail("Unknown Key");
                        break;
                }
            }

            Assert.AreEqual<int>(39, i);
        }

        [TestMethod]
        public void UrlValid()
        {
            string url = "http://agilebusinesscloud.com";
            string content = string.Format("{0} {1} {0}", StringHelper.ValidString(), url);
            var match = RegexStatement.Url.Match(content);
            Assert.IsNotNull(match);
            Assert.AreEqual<string>(url, match.Value);
        }

        [TestMethod]
        public void UrlValidGroup()
        {
            string url = "https://www.microsoft.com";
            string content = string.Format("{0} {1} {0}", StringHelper.ValidString(), url);
            var match = RegexStatement.Url.Match(content);
            Assert.IsNotNull(match);
            Assert.AreEqual<string>(url, match.Groups["Url"].Value);
        }

        [TestMethod]
        public void UrlInvalid()
        {
            string url = "http:/www.google";
            string content = string.Format("{0}{1}{0}", StringHelper.ValidString(), url);
            Assert.IsFalse(RegexStatement.Url.IsMatch(content));
        }

        [TestMethod]
        public void TwitterHash()
        {
            var content = "hey there, have you seen #happyland";
            var match = RegexStatement.TwitterKeyword.Match(content);
            Assert.IsNotNull(match);
            Assert.AreEqual<string>("happyland", match.Groups["Value"].Value);
        }

        [TestMethod]
        public void TwitterFollower()
        {
            var content = "hey @HappyLand hows it do?";
            var match = RegexStatement.TwitterFollower.Match(content);
            Assert.IsNotNull(match);
            Assert.AreEqual<string>("HappyLand", match.Groups["Value"].Value);
        }

        [TestMethod]
        public void GitHubAuthenticationResponse()
        {
            var content = "access_token=2287df98079546eb3f71b42e4ba861107f0a732a&token_type=bearer";
            var match = RegexStatement.GitHubAuthenticationResponse.Match(content);
            Assert.IsNotNull(match);
            Assert.AreEqual<string>("2287df98079546eb3f71b42e4ba861107f0a732a", match.Groups["accessToken"].Value);
            Assert.AreEqual<string>("bearer", match.Groups["tokenType"].Value);
        }
        #endregion
    }
}