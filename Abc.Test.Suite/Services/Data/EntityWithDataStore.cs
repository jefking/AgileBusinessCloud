// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='EntityWithDataStore.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Data
{
    using Abc.Azure;

    [AzureDataStore("testtablename")]
    public class EntityWithDataStore : Entity
    {
        public string Hex
        {
            get;
            set;
        }

        public int? Index
        {
            get;
            set;
        }
    }
}
