// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='TibeToUserRow.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Data
{
    using Abc.Azure;
    using Microsoft.WindowsAzure.StorageClient;
    using System;

    [CLSCompliant(false)]
    [AzureDataStore("UserTribes")]
    public class UserTribesRow : TableServiceEntity
    {
    }
}