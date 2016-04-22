// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='LoggingController.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Website.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Abc.Configuration;
    using Abc.Services;
    using Abc.Services.Contracts;
    using Abc.Services.Core;
    using Abc.Website.Models;

    /// <summary>
    /// Management Controller
    /// </summary>
    [Authorize]
    public class LoggingController : Controller
    {
        #region Members
        /// <summary>
        /// Logger
        /// </summary>
        private static readonly LogCore logger = new LogCore();

        /// <summary>
        /// Application Core
        /// </summary>
        private static readonly ApplicationCore appCore = new ApplicationCore();

        /// <summary>
        /// Max Top
        /// </summary>
        private const int MaxTop = 2500;
        #endregion

        #region Methods
        /// <summary>
        /// Default Page
        /// </summary>
        /// <remarks>
        /// GET: /
        /// </remarks>
        /// <returns>Action Result</returns>
        public ActionResult Index()
        {
            using (new PerformanceMonitor())
            {
                return this.RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Performance Data
        /// </summary>
        /// <remarks>
        /// GET: /Logging/Performance
        /// </remarks>
        /// <returns>Action Result</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Safety first.")]
        public ActionResult Performance(Guid? identifier, string key)
        {
            using (new PerformanceMonitor())
            {
                var model = new ManagementModel();

                try
                {
                    model = this.InitializeModel(key);

                    if (null != model.Application && Guid.Empty != model.Application.ApplicationId)
                    {
                        var query = new LogQuery()
                        {
                            ApplicationIdentifier = model.Application.ApplicationId,
                            Top = MaxTop,
                            From = DateTime.UtcNow.AddDays(-7),
                            Identifier = identifier,
                        };

                        var source = new LogCore();
                        model.Data = source.SelectOccurrences(query);
                    }
                }
                catch (Exception ex)
                {
                    logger.Log(ex, EventTypes.Error, (int)Fault.Unknown);

                    model.Error = WebResponse.Bind((int)Fault.Unknown, ex.Message);
                }

                return View(model);
            }
        }

        /// <summary>
        /// Collector Data
        /// </summary>
        /// <remarks>
        /// GET: /Logging/Collector
        /// </remarks>
        /// <returns>Action Result</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Safety first.")]
        public ActionResult Collector(Guid? identifier, string key)
        {
            using (new PerformanceMonitor())
            {
                var model = new ManagementModel();

                try
                {
                    model = this.InitializeModel(key);

                    if (null != model.Application && Guid.Empty != model.Application.ApplicationId)
                    {
                        var query = new LogQuery()
                        {
                            ApplicationIdentifier = model.Application.ApplicationId,
                            Top = MaxTop,
                            From = DateTime.UtcNow.AddDays(-7),
                            Identifier = identifier,
                        };

                        var source = new LogCore();
                        model.Data = source.SelectServerStatistics(query);
                    }
                }
                catch (Exception ex)
                {
                    logger.Log(ex, EventTypes.Error, (int)Fault.Unknown);

                    model.Error = WebResponse.Bind((int)Fault.Unknown, ex.Message);
                }

                return View(model);
            }
        }

        /// <summary>
        /// Collector Data
        /// </summary>
        /// <remarks>
        /// GET: /Logging/CollectorBrief
        /// </remarks>
        /// <returns>Action Result</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Safety first.")]
        public ActionResult CollectorBrief(string key)
        {
            using (new PerformanceMonitor())
            {
                var model = new ManagementModel();

                try
                {
                    model = this.InitializeModel(key);

                    if (null != model.Application && Guid.Empty != model.Application.ApplicationId)
                    {
                        var query = new LogQuery()
                        {
                            ApplicationIdentifier = model.Application.ApplicationId,
                            Deep = false,
                        };

                        var source = new LogCore();
                        var data = source.LatestServerStatistics(query);

                        model.Data = from server in data
                                        orderby server.MemoryUsagePercentage + server.CpuUsagePercentage + server.MemoryUsagePercentage descending
                                        select server;
                    }
                }
                catch (Exception ex)
                {
                    logger.Log(ex, EventTypes.Error, (int)Fault.Unknown);

                    model.Error = WebResponse.Bind((int)Fault.Unknown, ex.Message);
                }

                return View(model);
            }
        }

        /// <summary>
        /// Performance Data
        /// </summary>
        /// <remarks>
        /// GET: /Logging/PerformanceChart
        /// </remarks>
        /// <returns>Action Result</returns>
        public ActionResult PerformanceChart(string key)
        {
            using (new PerformanceMonitor())
            {
                var model = new ManagementModel();

                try
                {
                    model = this.InitializeModel(key);
                }
                catch (Exception ex)
                {
                    logger.Log(ex, EventTypes.Error, (int)Fault.Unknown);

                    model.Error = WebResponse.Bind((int)Fault.Unknown, ex.Message);
                }

                return View(model);
            }
        }

        /// <summary>
        /// Error Data
        /// </summary>
        /// <remarks>
        /// GET: /Logging/ErrorChart
        /// </remarks>
        /// <returns>Action Result</returns>
        public ActionResult ErrorChart(string key)
        {
            using (new PerformanceMonitor())
            {
                var model = new ManagementModel();

                try
                {
                    model = this.InitializeModel(key);
                }
                catch (Exception ex)
                {
                    logger.Log(ex, EventTypes.Error, (int)Fault.Unknown);

                    model.Error = WebResponse.Bind((int)Fault.Unknown, ex.Message);
                }

                return View(model);
            }
        }

        /// <summary>
        /// Dashboard
        /// </summary>
        /// <remarks>
        /// GET: /Logging/Dashboard
        /// </remarks>
        /// <returns>Action Result</returns>
        public ActionResult Dashboard(string key)
        {
            using (new PerformanceMonitor())
            {
                var model = new DashboardModel();

                try
                {
                    var management = this.InitializeModel(key);
                    var source = new LogCore();
                    var query = new LogQuery()
                    {
                        Top = 250,
                        From = DateTime.UtcNow.AddMinutes(-10),
                        ApplicationIdentifier = management.Application.ApplicationId,
                    };

                    model.Messages = source.SelectMessages(query);
                    model.Occurrences = source.SelectOccurrences(query);
                    model.Errors = source.SelectErrors(query);
                    var servers = (from item in model.Messages
                                   select item.MachineName).ToList();
                    servers.AddRange(from item in model.Occurrences
                                     select item.MachineName);
                    servers.AddRange(from item in model.Errors
                                     select item.MachineName);
                    model.Servers = servers.Distinct(StringComparer.CurrentCultureIgnoreCase);
                    model.Summary = new ApplicationSummary()
                    {
                        MessageCount = model.Messages.Count(),
                        OccurrenceCount = model.Occurrences.Count(),
                        ServerCount = model.Servers.Count(),
                        ErrorCount = model.Errors.Count(),
                        Details = management.Application,
                    };
                }
                catch (Exception ex)
                {
                    logger.Log(ex, EventTypes.Error, (int)Fault.Unknown);
                }

                return View(model);
            }
        }

        /// <summary>
        /// Error
        /// </summary>
        /// <remarks>
        /// GET: /Logging/Error
        /// </remarks>
        /// <returns>Action Result</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Safety first.")]
        public ActionResult Error(Guid? identifier, string key)
        {
            using (new PerformanceMonitor())
            {
                var model = new ManagementModel();

                try
                {
                    model = this.InitializeModel(key);

                    if (null != model.Application && Guid.Empty != model.Application.ApplicationId)
                    {
                        var query = new LogQuery()
                        {
                            ApplicationIdentifier = model.Application.ApplicationId,
                            Top = MaxTop,
                            From = DateTime.UtcNow.AddDays(-7),
                            Identifier = identifier,
                            Deep = false,
                        };

                        var source = new LogCore();
                        model.Data = source.SelectErrors(query);
                    }
                }
                catch (Exception ex)
                {
                    logger.Log(ex, EventTypes.Error, (int)Fault.Unknown);

                    model.Error = WebResponse.Bind((int)Fault.Unknown, ex.Message);
                }

                return View(model);
            }
        }

        /// <summary>
        /// Public Performance Publish
        /// </summary>
        /// <param name="identifier">Identifier</param>
        /// <returns>Redirect</returns>
        public ActionResult PerformancePublish(Guid? identifier)
        {
            using (new PerformanceMonitor())
            {
                if (Guid.Empty == identifier || null == identifier)
                {
                    return this.Json(WebResponse.Bind((int)Fault.InvalidIdentifier, "Identifier not specified."), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var model = new ManagementModel();

                    try
                    {
                        model = this.InitializeModel(null);

                        if (null != model.Application && Guid.Empty != model.Application.ApplicationId)
                        {
                            var query = new LogQuery()
                            {
                                ApplicationIdentifier = model.Application.ApplicationId,
                                Identifier = identifier,
                            };

                            var source = new LogCore();
                            source.PublishOccurence(query);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Log(ex, EventTypes.Error, (int)Fault.Unknown);

                        model.Error = WebResponse.Bind((int)Fault.Unknown, ex.Message);
                    }

                    return RedirectToAction("Performance");
                }
            }
        }

        /// <summary>
        /// Message
        /// </summary>
        /// <remarks>
        /// GET: /Logging/Message
        /// </remarks>
        /// <returns>Action Result</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Safety first.")]
        public ActionResult Message(Guid? identifier, string key)
        {
            using (new PerformanceMonitor())
            {
                var model = new ManagementModel();

                try
                {
                    model = this.InitializeModel(key);

                    if (null != model.Application && Guid.Empty != model.Application.ApplicationId)
                    {
                        var query = new LogQuery()
                        {
                            ApplicationIdentifier = model.Application.ApplicationId,
                            Top = MaxTop,
                            From = DateTime.UtcNow.AddDays(-7),
                            Identifier = identifier,
                        };

                        var source = new LogCore();
                        model.Data = source.SelectMessages(query);
                    }
                }
                catch (Exception ex)
                {
                    logger.Log(ex, EventTypes.Error, (int)Fault.Unknown);

                    model.Error = WebResponse.Bind((int)Fault.Unknown, ex.Message);
                }

                return View(model);
            }
        }

        /// <summary>
        /// Initialize Model
        /// </summary>
        /// <returns>Management Model</returns>
        public ManagementModel InitializeModel(string key)
        {
            using (new PerformanceMonitor())
            {
                var model = new ManagementModel();
                model.Applications = this.GetApplications();
                model.Preference = GetPreference(ServerConfiguration.ApplicationIdentifier, User.Identity.Data().Identifier);
                if (!string.IsNullOrWhiteSpace(key))
                {
                    model.Application = (from data in model.Applications
                                         where data.PublicKey == key
                                         select data).FirstOrDefault();
                }
                else if (null == model.Preference || null == model.Preference.CurrentApplication || Guid.Empty == model.Preference.CurrentApplication.Identifier)
                {
                    model.Application = model.Applications.FirstOrDefault();
                }
                else
                {
                    model.Application = (from data in model.Applications
                                         where data.ApplicationId == model.Preference.CurrentApplication.Identifier
                                         select data).FirstOrDefault();
                }

                if (string.IsNullOrWhiteSpace(model.Application.PublicKey))
                {
                    model.Application.PublicKey = model.Application.Id.ToAscii85().GetHexMD5();
                }

                return model;
            }
        }

        /// <summary>
        /// Get Preference
        /// </summary>
        /// <param name="appId">Application Identifier</param>
        /// <param name="userId">User Identifier</param>
        /// <returns>User Preference</returns>
        private static UserPreference GetPreference(Guid appId, Guid userId)
        {
            using (new PerformanceMonitor())
            {
                var app = new Application()
                {
                    Identifier = appId,
                };
                var user = new User()
                {
                    Identifier = userId,
                };
                var pref = new UserPreference()
                {
                    Application = app,
                    User = user,
                };

                var core = new UserCore();
                return core.Get(pref);
            }
        }

        /// <summary>
        /// Get Applications
        /// </summary>
        /// <returns>Application Details Models</returns>
        private IEnumerable<ApplicationDetailsModel> GetApplications()
        {
            using (new PerformanceMonitor())
            {
                var applications = appCore.Get(User.Identity.Data(), ServerConfiguration.ApplicationIdentifier);
                return applications.Convert();
            }
        }

        /// <summary>
        /// Set Preference
        /// </summary>
        /// <param name="appId">Application Identifier</param>
        /// <param name="userId">User Identifier</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Controller")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Controller")]
        private void SetPreference(Guid appId, Guid userId)
        {
            using (new PerformanceMonitor())
            {
                var app = new Application()
                {
                    Identifier = ServerConfiguration.ApplicationIdentifier,
                };
                var currentApp = new Application()
                {
                    Identifier = appId,
                };
                var user = new User()
                {
                    Identifier = userId,
                };
                var pref = new UserPreference()
                {
                    Application = app,
                    CurrentApplication = currentApp,
                    User = user,
                    MaximumAllowedApplications = null,
                };

                var core = new UserCore();
                core.Save(pref);
            }
        }
        #endregion
    }
}