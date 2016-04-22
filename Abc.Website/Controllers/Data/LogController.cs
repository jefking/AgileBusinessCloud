// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='LogController.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Website.Controllers.Data
{
    using Abc.Services;
    using Abc.Services.Contracts;
    using Abc.Services.Core;
    using Abc.Website.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    /// <summary>
    /// Management Controller
    /// </summary>
    [Authorize]
    public class LogController : Controller
    {
        #region Members
        /// <summary>
        /// Log Core
        /// </summary>
        private static readonly LogCore logCore = new LogCore();

        /// <summary>
        /// Max Top
        /// </summary>
        private const int MaxTop = 5000;
        #endregion

        #region Methods
        /// <summary>
        /// Index
        /// </summary>
        /// <returns>Action Result</returns>
        public ActionResult Index()
        {
            using (new PerformanceMonitor())
            {
                return this.RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Performance
        /// </summary>
        /// <param name="application">Application Identifier</param>
        /// <param name="from">From</param>
        /// <param name="to">To</param>
        /// <param name="top">Top</param>
        /// <returns>Action Result</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Safety first.")]
        public ActionResult Performance(Guid? application, DateTime? from, DateTime? to, int? top)
        {
            using (new PerformanceMonitor())
            {
                if (Guid.Empty == application || null == application)
                {
                    return this.Json(WebResponse.Bind((int)Fault.InvalidApplicationIdentifier, "Application Identifier not specified."), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    try
                    {
                        var query = new LogQuery()
                        {
                            ApplicationIdentifier = application.Value,
                            Top = top ?? MaxTop,
                            From = from ?? DateTime.UtcNow.AddDays(-21),
                            To = to ?? DateTime.UtcNow,
                        };

                        var results = logCore.SelectOccurrences(query);

                        return this.Json(results, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception ex)
                    {
                        logCore.Log(ex, EventTypes.Error, (int)Fault.Unknown);
                        return this.Json(WebResponse.Bind((int)Fault.Unknown, ex.Message), JsonRequestBehavior.AllowGet);
                    }
                }
            }
        }

        /// <summary>
        /// Errors
        /// </summary>
        /// <param name="application">Application Identifier</param>
        /// <param name="from">From</param>
        /// <param name="to">To</param>
        /// <param name="top">Top</param>
        /// <returns>Action Result</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Safety first.")]
        public ActionResult Error(Guid? application, DateTime? from, DateTime? to, int? top)
        {
            using (new PerformanceMonitor())
            {
                if (Guid.Empty == application || null == application)
                {
                    return this.Json(WebResponse.Bind((int)Fault.InvalidApplicationIdentifier, "Application Identifier not specified."), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var query = new LogQuery()
                    {
                        ApplicationIdentifier = application.Value,
                        From = from ?? DateTime.UtcNow.AddDays(-21),
                        To = to ?? DateTime.UtcNow,
                        Top = top ?? MaxTop,
                    };

                    try
                    {
                        return this.Json(logCore.SelectErrors(query), JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception ex)
                    {
                        logCore.Log(ex, EventTypes.Error, (int)Fault.Unknown);
                        return this.Json(WebResponse.Bind((int)Fault.Unknown, ex.Message), JsonRequestBehavior.AllowGet);
                    }
                }
            }
        }

        /// <summary>
        /// Errors
        /// </summary>
        /// <param name="application">Application Identifier</param>
        /// <returns>Action Result</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Safety first.")]
        public ActionResult ErrorsCompressed(Guid? application)
        {
            using (new PerformanceMonitor())
            {
                if (Guid.Empty == application || null == application)
                {
                    return this.Json(WebResponse.Bind((int)Fault.InvalidApplicationIdentifier, "Application Identifier not specified."), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var query = new LogQuery()
                    {
                        ApplicationIdentifier = application.Value,
                        From = DateTime.UtcNow.AddDays(-1),
                        To = DateTime.UtcNow,
                    };

                    try
                    {
                        var data = logCore.SelectErrors(query);

                        var items = new List<CompressedError>();
                        foreach (var err in (from d in data
                                            select new{Message = d.Message, Class = d.ClassName}).Distinct())
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
                            items.Add(item);
                        }

                        return this.Json(items, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception ex)
                    {
                        logCore.Log(ex, EventTypes.Error, (int)Fault.Unknown);
                        return this.Json(WebResponse.Bind((int)Fault.Unknown, ex.Message), JsonRequestBehavior.AllowGet);
                    }
                }
            }
        }

        /// <summary>
        /// Messages
        /// </summary>
        /// <param name="application">Application Identifier</param>
        /// <param name="from">From</param>
        /// <param name="to">To</param>
        /// <param name="top">Top</param>
        /// <returns>Action Result</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Safety first.")]
        public ActionResult Message(Guid? application, DateTime? from, DateTime? to, int? top)
        {
            using (new PerformanceMonitor())
            {
                if (Guid.Empty == application || null == application)
                {
                    return this.Json(WebResponse.Bind((int)Fault.InvalidApplicationIdentifier, "Application Identifier not specified."), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    try
                    {
                        var query = new LogQuery()
                        {
                            ApplicationIdentifier = application.Value,
                            From = from ?? DateTime.UtcNow.AddDays(-21),
                            To = to ?? DateTime.UtcNow,
                            Top = top ?? MaxTop,
                        };

                        return this.Json(logCore.SelectMessages(query), JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception ex)
                    {
                        logCore.Log(ex, EventTypes.Error, (int)Fault.Unknown);
                        return this.Json(WebResponse.Bind((int)Fault.Unknown, ex.Message), JsonRequestBehavior.AllowGet);
                    }
                }
            }
        }

        /// <summary>
        /// Server Statistics
        /// </summary>
        /// <param name="application">Application Identifier</param>
        /// <param name="from">From</param>
        /// <param name="to">To</param>
        /// <param name="top">Top</param>
        /// <returns>Action Result</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Safety first.")]
        public ActionResult ServerStatistics(Guid? application, DateTime? from, DateTime? to, int? top)
        {
            using (new PerformanceMonitor())
            {
                if (Guid.Empty == application || null == application)
                {
                    return this.Json(WebResponse.Bind((int)Fault.InvalidApplicationIdentifier, "Application Identifier not specified."), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    try
                    {
                        var query = new LogQuery()
                        {
                            ApplicationIdentifier = application.Value,
                            From = from ?? DateTime.UtcNow.AddDays(-21),
                            To = to ?? DateTime.UtcNow,
                            Top = top ?? MaxTop
                        };

                        return this.Json(logCore.SelectServerStatistics(query), JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception ex)
                    {
                        logCore.Log(ex, EventTypes.Error, (int)Fault.Unknown);
                        return this.Json(WebResponse.Bind((int)Fault.Unknown, ex.Message), JsonRequestBehavior.AllowGet);
                    }
                }
            }
        }
        #endregion
    }
}