// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='Messages.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Process
{
    using Abc.Azure;
    using Abc.Configuration;
    using Abc.Services.Contracts;
    using Abc.Services.Core;
    using Abc.Services.Website.Models;
    using System;

    public class Messages : ApplicationScheduleManager
    {
        #region Members
        /// <summary>
        /// Blob
        /// </summary>
        private readonly JsonPContainer<MessageData> blob = new JsonPContainer<MessageData>(ServerConfiguration.Default, "renderMessages");
        #endregion

        #region Constructors
        /// <summary>
        /// Default Constructor
        /// </summary>
        public Messages()
            : base(12 * 60)
        {
        }
        #endregion

        #region Methods
        public override void Execute(Guid application)
        {
            try
            {
                var query = new LogQuery()
                {
                    ApplicationIdentifier = application,
                    From = DateTime.UtcNow.AddHours(-24),
                    To = DateTime.UtcNow,
                    Top = 5000
                };

                var data = new MessageData()
                {
                    Messages = logCore.SelectMessages(query),
                    GeneratedOn = DateTime.UtcNow,
                };

                foreach (var message in data.Messages)
                {
                    message.Token = null;
                }

                var objectId = LogCore.Message1DaysFormat.FormatWithCulture(application.ToAscii85().GetHexMD5());
                blob.Save(objectId, data);
            }
            catch (Exception ex)
            {
                logCore.Log(ex, EventTypes.Critical, 99999);
            }
        }
        #endregion
    }
}