// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='TestConfig.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Configuration
{
    using System;
    using System.Collections.Generic;
    using Abc.Configuration;

    /// <summary>
    /// Test Config
    /// </summary>
    public class TestConfig : HardcodedConfiguration
    {
        #region Methods
        /// <summary>
        /// Define Configuration
        /// </summary>
        /// <returns>Configuration</returns>
        public override IDictionary<string, string> DefineConfiguration()
        {
            var config = new Dictionary<string, string>();
            config.Add("AzureEmulator", @"C:\Program Files\Microsoft SDKs\Windows Azure\Emulator\csrun.exe");
            config.Add("ApplicationIdentifier", "481f0065-e83c-4386-bffa-017a3b0db72b");
            config.Add("Abc.LogExceptions", "true");
            config.Add("Abc.LogPerformance", "true");
            config.Add("95FD7F7F-439B-4D97-95C1-2782B9CE1743", "true");
            config.Add("DataConnectionString", "UseDevelopmentStorage=true");
            config.Add("MailGunApi", "https://mailgun.net/api/");
            config.Add("MailGunApiKey", "key-6kv4oyp2e5tw446bz5");
            config.Add("Abc.ServerConfig", "true");
            config.Add("Abc.LogPerformanceMinimumDuration", "200");
            config.Add("Abc.DatumRemoteAddress", "http://localhost:8001/Datum.svc");
            return config;
        }
        #endregion
    }
}