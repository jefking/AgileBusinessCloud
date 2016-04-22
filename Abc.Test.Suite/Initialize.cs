// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='Initialize.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite
{
    using System;
    using Abc.Configuration;
    using Abc.Services.Data;
    using Abc.Test.Configuration;
    using Abc.Underpinning;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Initialize
    /// </summary>
    [TestClass]
    public static class Initialize
    {
        #region Members
        /// <summary>
        /// Azure Emulator
        /// </summary>
        private static AzureComputeEmulator backend;

        /// <summary>
        /// Azure Emulator
        /// </summary>
        private static AzureComputeEmulator frontend;
        #endregion

        #region Initialize
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="context">TestContext</param>
        [AssemblyInitialize]
        public static void Init(TestContext context)
        {
            Settings.Instance.Add(new TestConfig());

            backend = new AzureComputeEmulator("\\Abc.Cloud.Backend");
            backend.Run();

            frontend = new AzureComputeEmulator("\\Abc.Host.Cloud");
            frontend.Run();

            TableRegister.Initialize();

            var loggedOn = false;
            var i = 0;
            while (i < 10 && !loggedOn)
            {
                loggedOn = Application.LogOn();
                i++;
            }

            if (!loggedOn)
            {
                throw new ApplicationException("Application not validated.");
            }

            var app = new Abc.Underpinning.Administration.Application();
            loggedOn = false;
            i = 0;
            while (i < 10 && !loggedOn)
            {
                loggedOn = app.LogOn();
                i++;
            }

            if (!loggedOn)
            {
                throw new ApplicationException("Application not validated.");
            }

            DeleteData();
        }

        /// <summary>
        /// Clean Up
        /// </summary>
        [AssemblyCleanup]
        public static void CleanUp()
        {
            backend.Terminate();
            frontend.Terminate();
        }

        /// <summary>
        /// Delete Tesing Data
        /// </summary>
        private static void DeleteData()
        {
            var token = new Abc.Services.Contracts.Token()
            {
                ApplicationId = Settings.ApplicationIdentifier,
            };
            var source = new Abc.Services.Core.LogCore();

            var perf = new Abc.Services.Contracts.Occurrence()
            {
                Token = token,
                OccurredOn = DateTime.UtcNow.AddDays(-1),
            };

            source.Delete(perf);

            var err = new Abc.Services.Contracts.ErrorItem()
            {
                Token = token,
                OccurredOn = DateTime.UtcNow.AddDays(-1),
            };

            source.Delete(err);

            var msg = new Abc.Services.Contracts.Message()
            {
                Token = token,
                OccurredOn = DateTime.UtcNow.AddDays(-1),
            };

            source.Delete(msg);
        }
        #endregion
    }
}