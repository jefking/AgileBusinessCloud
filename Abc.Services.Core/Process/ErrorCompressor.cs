// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='ErrorCompressor.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Process
{
    using Abc.Azure;
    using Abc.Configuration;
    using Abc.Services.Contracts;
    using Abc.Services.Core;
    using Abc.Website.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Error Compressor
    /// </summary>
    public class ErrorCompressor : ApplicationScheduleManager
    {
        #region Members
        /// <summary>
        /// Blob
        /// </summary>
        private readonly JsonPContainer<CompressedErrors> blob = new JsonPContainer<CompressedErrors>(ServerConfiguration.Default, "renderErrorsCompressed");
        #endregion

        #region Constructors
        /// <summary>
        /// Default Constructor
        /// </summary>
        public ErrorCompressor()
            : base(12 * 60)
        {
        }
        #endregion

        #region Methods
        public override void Execute(Guid application)
        {
            var objectId = LogCore.ErrorsCompressed1DaysFormat.FormatWithCulture(application.ToAscii85().GetHexMD5());

            blob.Save(objectId, this.Compress(application));
        }

        private CompressedErrors Compress(Guid applicationIdentifier)
        {
            var compressed = new CompressedErrors()
            {
                GeneratedOn = DateTime.UtcNow,
            };

            var query = new LogQuery()
            {
                ApplicationIdentifier = applicationIdentifier,
                From = DateTime.UtcNow.AddDays(-1),
                To = DateTime.UtcNow,
            };

            try
            {
                var data = logCore.SelectErrors(query);

                compressed.Errors = this.Compressed(data);
                compressed.Errors = this.ErrorRate(data, compressed.Errors);
                compressed.Occurrences = this.Occurrences(data, compressed.Errors);
            }
            catch (Exception ex)
            {
                logCore.Log(ex, EventTypes.Critical, 99999);
            }

            return compressed;
        }

        private IEnumerable<ErrorsPerTime> Occurrences(IEnumerable<ErrorDisplay> data, IEnumerable<CompressedError> errors)
        {
            try
            {
                var temp = new List<ErrorsPerTime>();
                foreach (var error in errors)
                {
                    temp.AddRange(error.Occurrences);
                }

                return from x in temp
                        group x by x.Time into x
                        select new ErrorsPerTime()
                        {
                            Count = x.Sum(y => y.Count),
                            Time = x.First().Time,
                        };
            }
            catch (Exception ex)
            {
                logCore.Log(ex, EventTypes.Critical, 99999);
            }

            return new List<ErrorsPerTime>();
        }

        private IEnumerable<CompressedError> ErrorRate(IEnumerable<ErrorDisplay> data, IEnumerable<CompressedError> errors)
        {
            try
            {
                foreach (var err in errors)
                {
                    err.Occurrences = (from d in data
                                       where d.ClassName == err.Class && d.Message == err.Message
                                       group d by d.OccurredOn.Hour into x
                                       select new ErrorsPerTime()
                                       {
                                           Count = x.Count(),
                                           Time = new DateTime(x.First().OccurredOn.Year, x.First().OccurredOn.Month, x.First().OccurredOn.Day, x.First().OccurredOn.Hour, 0, 0),
                                       });
                }

            }
            catch (Exception ex)
            {
                logCore.Log(ex, EventTypes.Critical, 99999);
            }

            return errors;
        }

        private IEnumerable<CompressedError> Compressed(IEnumerable<ErrorDisplay> data)
        {
            var errors = new List<CompressedError>();
            try
            {
                foreach (var err in (from d in data
                                     select new { Message = d.Message, Class = d.ClassName }).Distinct())
                {
                    var item = new CompressedError()
                    {
                        Class = err.Class,
                        Message = err.Message,
                        Count = (from d in data
                                 where d.Message == err.Message
                                    && d.ClassName == err.Class
                                 select d).Count(),
                    };
                    errors.Add(item);
                }
            }
            catch (Exception ex)
            {
                logCore.Log(ex, EventTypes.Critical, 99999);
            }

            return errors;
        }
        #endregion
    }
}