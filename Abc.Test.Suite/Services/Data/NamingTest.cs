// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='NamingTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Data
{
    using Abc.Azure;
    using Abc.Services;
    using Abc.Services.Contracts;
    using Abc.Services.Data;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class NamingTest
    {
        #region Valid Cases
        [TestMethod]
        public void MessageHistory()
        {
            var table = typeof(LogHistory<LogItem>).GetCustomAttribute<AzureDataStoreAttribute>(false);
            Assert.AreEqual<string>("logging", table.Name);
        }

        [TestMethod]
        public void CodeStormSocial()
        {
            var table = typeof(CodeStormSocial).GetCustomAttribute<AzureDataStoreAttribute>(false);
            Assert.AreEqual<string>("social", table.Name);
        }

        [TestMethod]
        public void ContactGroupRow()
        {
            var table = typeof(ContactGroupRow).GetCustomAttribute<AzureDataStoreAttribute>(false);
            Assert.AreEqual<string>("ContactGroup", table.Name);
        }

        [TestMethod]
        public void PayPalPaymentConfirmationRow()
        {
            var table = typeof(PayPalPaymentConfirmationRow).GetCustomAttribute<AzureDataStoreAttribute>(false);
            Assert.AreEqual<string>("PayPalPaymentConfirmation", table.Name);
        }

        [TestMethod]
        public void ServerStatisticsRow()
        {
            var table = typeof(ServerStatisticsRow).GetCustomAttribute<AzureDataStoreAttribute>(false);
            Assert.AreEqual<string>("ServerStatistics", table.Name);
        }

        [TestMethod]
        public void LatestServerStatisticsRow()
        {
            var table = typeof(LatestServerStatisticsRow).GetCustomAttribute<AzureDataStoreAttribute>(false);
            Assert.AreEqual<string>("LatestServerStatistics", table.Name);
        }

        [TestMethod]
        public void BlogEntry()
        {
            var table = typeof(BlogRow).GetCustomAttribute<AzureDataStoreAttribute>(false);
            Assert.AreEqual<string>("BlogEntries", table.Name);
        }

        [TestMethod]
        public void EventLogRow()
        {
            var table = typeof(EventLogRow).GetCustomAttribute<AzureDataStoreAttribute>(false);
            Assert.AreEqual<string>("EventLog", table.Name);
        }
        
        [TestMethod]
        public void ContactRow()
        {
            var table = typeof(ContactRow).GetCustomAttribute<AzureDataStoreAttribute>(false);
            Assert.AreEqual<string>("UserContact", table.Name);
        }

        [TestMethod]
        public void CompanyRow()
        {
            var table = typeof(CompanyRow).GetCustomAttribute<AzureDataStoreAttribute>(false);
            Assert.AreEqual<string>("Company", table.Name);
        }

        [TestMethod]
        public void UserData()
        {
            var table = typeof(UserData).GetCustomAttribute<AzureDataStoreAttribute>(false);
            Assert.AreEqual<string>("UserLogin", table.Name);
        }

        [TestMethod]
        public void GeneralMetricRow()
        {
            var table = typeof(GeneralMetricRow).GetCustomAttribute<AzureDataStoreAttribute>(false);
            Assert.AreEqual<string>("GeneralMetricV2", table.Name);
        }

        [TestMethod]
        public void BytesStoredData()
        {
            var table = typeof(BytesStoredData).GetCustomAttribute<AzureDataStoreAttribute>(false);
            Assert.AreEqual<string>("ApplicationDataAccount", table.Name);
        }

        [TestMethod]
        public void ErrorData()
        {
            var table = typeof(ErrorData).GetCustomAttribute<AzureDataStoreAttribute>(false);
            Assert.AreEqual<string>("ApplicationError", table.Name);
        }

        [TestMethod]
        public void MessageData()
        {
            var table = typeof(MessageData).GetCustomAttribute<AzureDataStoreAttribute>(false);
            Assert.AreEqual<string>("ApplicationMessage", table.Name);
        }

        [TestMethod]
        public void OccurrenceData()
        {
            var table = typeof(OccurrenceData).GetCustomAttribute<AzureDataStoreAttribute>(false);
            Assert.AreEqual<string>("ApplicationOccurrence", table.Name);
        }

        [TestMethod]
        public void UserProfileRow()
        {
            var table = typeof(UserProfileRow).GetCustomAttribute<AzureDataStoreAttribute>(false);
            Assert.AreEqual<string>("UserProfile", table.Name);
        }

        [TestMethod]
        public void TextData()
        {
            var table = typeof(TextData).GetCustomAttribute<AzureDataStoreAttribute>(false);
            Assert.AreEqual<string>("TextContent", table.Name);
        }

        [TestMethod]
        public void XmlData()
        {
            var table = typeof(XmlData).GetCustomAttribute<AzureDataStoreAttribute>(false);
            Assert.AreEqual<string>("XmlContent", table.Name);
        }

        [TestMethod]
        public void ApplicationInfoData()
        {
            var table = typeof(ApplicationInfoData).GetCustomAttribute<AzureDataStoreAttribute>(false);
            Assert.AreEqual<string>("ApplicationInformation", table.Name);
        }

        [TestMethod]
        public void UserApplicationData()
        {
            var table = typeof(UserApplicationData).GetCustomAttribute<AzureDataStoreAttribute>(false);
            Assert.AreEqual<string>("ApplicationUser", table.Name);
        }

        [TestMethod]
        public void BinaryEmailData()
        {
            var table = typeof(BinaryEmailData).GetCustomAttribute<AzureDataStoreAttribute>(false);
            Assert.AreEqual<string>("BinaryEmail", table.Name);
        }

        [TestMethod]
        public void PlaintextEmailData()
        {
            var table = typeof(PlaintextEmailData).GetCustomAttribute<AzureDataStoreAttribute>(false);
            Assert.AreEqual<string>("PlaintextEmail", table.Name);
        }

        [TestMethod]
        public void UserPreferenceRow()
        {
            var table = typeof(UserPreferenceRow).GetCustomAttribute<AzureDataStoreAttribute>(false);
            Assert.AreEqual<string>("ApplicationUserPreference", table.Name);
        }

        [TestMethod]
        public void RoleRow()
        {
            var table = typeof(RoleRow).GetCustomAttribute<AzureDataStoreAttribute>(false);
            Assert.AreEqual<string>("UserRole", table.Name);
        }
        #endregion
    }
}