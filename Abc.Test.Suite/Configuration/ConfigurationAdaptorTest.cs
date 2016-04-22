// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='ConfigurationAdaptorTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Configuration
{
    using System.Collections.Generic;
    using Abc.Configuration;

    public class ConfigurationAdaptorTest : IConfigurationAdaptor
    {
        private IDictionary<string, string> config = new Dictionary<string, string>();
        
        public IDictionary<string, string> Configuration
        {
            get
            {
                return config;
            }
        }
    }
}