// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='ExtensionMethods.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite
{
    using System;
    using Abc.Services.Contracts;
    using Abc.Services;

    public static class ExtensionMethods
    {
        #region Methods
        public static void Fill(this Message msg, Guid appId)
        {
            var token = new Token()
            {
                ApplicationId = appId
            };

            msg.OccurredOn = DateTime.UtcNow;
            msg.Token = token;
            msg.Message = StringHelper.ValidString();
        }

        public static void Fill(this Message msg)
        {
            msg.Fill(Guid.NewGuid());
        }

        public static void Fill(this MessageData msg)
        {
            msg.OccurredOn = DateTime.UtcNow;
            msg.DeploymentId = StringHelper.ValidString();
            msg.Message = StringHelper.ValidString();
            msg.MachineName = Environment.MachineName;
        }

        public static void Fill(this LogQuery query)
        {
            query.From = DateTime.UtcNow.AddYears(-1);
            query.To = DateTime.UtcNow;
            query.Top = 100;
            query.ApplicationIdentifier = Guid.NewGuid();
        }
        #endregion
    }
}