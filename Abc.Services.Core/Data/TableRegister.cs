// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='TableRegister.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Data
{
    using Abc.Azure;
    using Abc.Configuration;
    using Abc.Services.Contracts;
    using Abc.Services.Graphics;

    /// <summary>
    /// Table Regiser
    /// </summary>
    public static class TableRegister
    {
        #region Methods
        /// <summary>
        /// Initialize
        /// </summary>
        public static void Initialize()
        {
            var account = ServerConfiguration.Default;

            var appInfo = new AzureTable<ApplicationInfoData>(account);
            appInfo.EnsureExist();

            var userApp = new AzureTable<UserApplicationData>(account);
            userApp.EnsureExist();

            var binaryEmail = new AzureTable<BinaryEmailData>(account);
            binaryEmail.EnsureExist();

            var plaintextEmail = new AzureTable<PlaintextEmailData>(account);
            plaintextEmail.EnsureExist();

            var appConfig = new AzureTable<ApplicationConfiguration>(account);
            appConfig.EnsureExist();

            var occuranceTable = new AzureTable<OccurrenceData>(account);
            occuranceTable.EnsureExist();

            var errorTable = new AzureTable<ErrorData>(account);
            errorTable.EnsureExist();

            var bytesTable = new AzureTable<BytesStoredData>(account);
            bytesTable.EnsureExist();

            var messageTable = new AzureTable<MessageData>(account);
            messageTable.EnsureExist();

            var generalMetricTable = new AzureTable<GeneralMetricRow>(account);
            generalMetricTable.EnsureExist();

            var userpreference = new AzureTable<UserPreferenceRow>(account);
            userpreference.EnsureExist();

            var company = new AzureTable<CompanyRow>(account);
            company.EnsureExist();

            var contact = new AzureTable<ContactRow>(account);
            contact.EnsureExist();

            var contactGroup = new AzureTable<ContactGroupRow>(account);
            contactGroup.EnsureExist();

            var userTable = new AzureTable<UserData>(account);
            userTable.EnsureExist();

            var roleTable = new AzureTable<RoleRow>(account);
            roleTable.EnsureExist();

            var eventLog = new AzureTable<EventLogRow>(account);
            eventLog.EnsureExist();

            var paypal = new AzureTable<PayPalPaymentConfirmationRow>(account);
            paypal.EnsureExist();

            var serverStats = new AzureTable<ServerStatisticsRow>(account);
            serverStats.EnsureExist();

            var blog = new AzureTable<BlogRow>(account);
            blog.EnsureExist();

            var latestServerStats = new AzureTable<LatestServerStatisticsRow>(account);
            latestServerStats.EnsureExist();

            var logHistory = new JsonContainer<LogHistory<LogItem>>(account);
            logHistory.EnsureExist();

            var userProfile = new AzureTable<UserProfileRow>(account);
            userProfile.EnsureExist();
            
            var dataManagerLog = new AzureTable<DataManagerLog>(account);
            dataManagerLog.EnsureExist();

            var userTribesRow = new AzureTable<UserTribesRow>(account);
            userTribesRow.EnsureExist();
        }
        #endregion
    }
}