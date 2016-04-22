// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='Entity.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite.Data
{
    using Microsoft.WindowsAzure.StorageClient;

    public class Entity : TableServiceEntity
    {
        public int ToTest
        {
            get;
            set;
        }
    }
}