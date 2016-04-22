// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='PerformanceOccurances.cs'>
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

    /// <summary>
    /// Performance Occurences
    /// </summary>
    public class PerformanceOccurrences : ApplicationScheduleManager
    {
        #region Members
        /// <summary>
        /// Blob
        /// </summary>
        private readonly JsonPContainer<PerformanceData> blob = new JsonPContainer<PerformanceData>(ServerConfiguration.Default, "renderOccurrences");
        #endregion

        #region Constructors
        /// <summary>
        /// Default Constructor
        /// </summary>
        public PerformanceOccurrences()
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

                var data = new PerformanceData()
                {
                    Occurrences = logCore.SelectOccurrences(query),
                    GeneratedOn = DateTime.UtcNow,
                };

                foreach (var occurrence in data.Occurrences)
                {
                    occurrence.Token = null;
                }

                var objectId = LogCore.Performance1DaysFormat.FormatWithCulture(application.ToAscii85().GetHexMD5());
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