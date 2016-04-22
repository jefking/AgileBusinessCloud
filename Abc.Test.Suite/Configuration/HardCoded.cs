// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='HardCoded.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Configuration
{
    using System;
    using System.Collections.Generic;
    using Abc.Configuration;

    public class HardCoded : HardcodedConfiguration
    {
        private string initialKey = Guid.NewGuid().ToString();
        private string initialValue = Guid.NewGuid().ToString();

        public string InitialKey
        {
            get
            {
                return this.initialKey;
            }
        }

        public string InitialValue
        {
            get
            {
                return this.initialValue;
            }
        }

        public override IDictionary<string, string> DefineConfiguration()
        {
            var config = new Dictionary<string, string>();
            config.Add(InitialKey, InitialValue); 
            return config;
        }
    }
}