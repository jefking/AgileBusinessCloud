// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='AppsController.cs'>
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
    using Abc.Services.Validation;
    using Abc.Website.Models;

    /// <summary>
    /// Apps Controller
    /// </summary>
    public class AppsController : Controller
    {
        #region Members
        /// <summary>
        /// Application Core
        /// </summary>
        private static readonly ApplicationCore appCore = new ApplicationCore();

        /// <summary>
        /// Logging
        /// </summary>
        private static readonly LogCore log = new LogCore();

        /// <summary>
        /// Payment Core
        /// </summary>
        private static readonly PaymentCore paymentCore = new PaymentCore();
        #endregion

        #region Methods
        /// <summary>
        /// Default
        /// </summary>
        /// <remarks>
        /// GET: /Apps
        /// </remarks>
        /// <returns>Action Result</returns>
        public ActionResult Index()
        {
            using (new PerformanceMonitor())
            {
                ViewBag.CanCreateAnApplication = false;

                try
                {
                    ViewBag.CanCreateAnApplication = this.User.Identity.IsAuthenticated && this.CanCreateAnApplication();
                }
                catch (Exception ex)
                {
                    log.Log(ex, EventTypes.Error, (int)Fault.Unknown);
                }

                return View();
            }
        }

        /// <summary>
        /// Purchase Complete
        /// </summary>
        /// <remarks>
        /// GET: /Apps/PurchaseComplete
        /// </remarks>
        /// <param name="tx">Transaction ID</param>
        /// <param name="st">Payment status</param>
        /// <param name="amt">Payment amount</param>
        /// <param name="cc">Currency code</param>
        /// <returns>Action Result</returns>
        [Authorize]
        public ActionResult PurchaseComplete(string tx, string st, string amt, string cc)
        {
            using (new PerformanceMonitor())
            {
                if (string.IsNullOrWhiteSpace(tx))
                {
                    return this.RedirectToAction("Index", "Home");
                }
                else
                {
                    var payment = new PayPalPaymentConfirmation()
                    {
                        TransactionId = tx,
                        PaymentStatus = st,
                        Amount = amt,
                        CurrencyCode = cc,
                        Application = Abc.Services.Contracts.Application.Current,
                        User = this.User.Identity.Data(),
                    };

                    try
                    {
                        ViewBag.CanCreateAnApplication = this.CanCreateAnApplication();

                        var validator = new Validator<PayPalPaymentConfirmation>();
                        if (!validator.IsValid(payment))
                        {
                            var messages = validator.AllMessages(payment);
                            log.Log(messages);
                            return this.View(messages);
                        }
                        else
                        {
                            paymentCore.ValidatePurchase(payment);

                            ViewBag.CanCreateAnApplication = this.CanCreateAnApplication();

                            return View(payment);
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Log(ex, EventTypes.Error, (int)Fault.Unknown);
                        return this.View(ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Application
        /// </summary>
        /// <remarks>
        /// GET: /Apps/Application
        /// </remarks>
        /// <returns>Action Result</returns>
        [Authorize]
        public ActionResult Application()
        {
            using (new PerformanceMonitor())
            {
                ViewBag.CanCreateAnApplication = false;

                try
                {
                    var applications = this.GetApplications();
                    if (0 < applications.Count())
                    {
                        ViewBag.Data = applications;
                    }

                    ViewBag.CanCreateAnApplication = this.CanCreateAnApplication();
                    return View();
                }
                catch (Exception ex)
                {
                    log.Log(ex, EventTypes.Error, (int)Fault.Unknown);
                    return View();
                }
            }
        }

        /// <summary>
        /// Application Details
        /// </summary>
        /// <remarks>
        /// GET: /Apps/Details
        /// </remarks>
        /// <param name="appId">Application Identifier</param>
        /// <returns>Action Result</returns>
        [Authorize]
        public ActionResult  Details(Guid appId)
        {
            using (new PerformanceMonitor())
            {
                var app = new Application()
                {
                    Identifier = appId,
                };

                try
                {
                    var canCreateApplication = this.CanCreateAnApplication();
                    if (canCreateApplication && Guid.Empty == appId)
                    {
                        var newApp = new ApplicationDetailsModel()
                        {
                            ApplicationId = Guid.NewGuid(),
                            IsValid = true,
                            New = true,
                            Deleted = false,
                            Active = true,
                            ValidUntil = DateTime.UtcNow.AddYears(1),
                        };

                        newApp.PublicKey = newApp.ApplicationId.ToAscii85().GetHexMD5();
                        this.ViewData.Model = newApp;
                    }
                    else if (Guid.Empty != appId && appCore.UserIsAssociated(User.Identity.Editor(), app))
                    {
                        var info = new ApplicationInformation()
                        {
                            Identifier = appId,
                        };
                        var appInfo = appCore.Get(info);
                        if (null != appInfo)
                        {
                            var model = appInfo.Convert();

                            try
                            {
                                if (Guid.Empty != appInfo.OwnerId)
                                {
                                    var userCore = new UserCore();
                                    var application = new Application()
                                    {
                                        Identifier = appId,
                                    };
                                    var user = new User()
                                    {
                                        Identifier = appInfo.OwnerId,
                                    };
                                    var userApp = new UserApplication()
                                    {
                                        Application = application,
                                        User = user,
                                    };
                                    var userLoaded = userCore.Get(userApp);
                                    model.User = userLoaded.Convert().Convert();
                                }
                            }
                            catch (Exception ex)
                            {
                                log.Log(ex, EventTypes.Error, (int)Fault.Unknown);
                            }

                            this.ViewData.Model = model;
                        }
                    }
                    else
                    {
                        return this.RedirectToAction("Application");
                    }

                    ViewBag.CanCreateAnApplication = canCreateApplication;
                    return this.View();
                }
                catch (Exception ex)
                {
                    log.Log(ex, EventTypes.Error, (int)Fault.Unknown);
                    return View();
                }
            }
        }

        /// <summary>
        /// Application Details
        /// </summary>
        /// <remarks>
        /// GET: /Apps/Details
        /// </remarks>
        /// <param name="value">Application Details Model</param>
        /// <returns>Action Result</returns>
        [Authorize]
        [HttpPost]
        public ActionResult Details(ApplicationDetailsModel value)
        {
            using (new PerformanceMonitor())
            {
                var userApplication = new ApplicationInformation()
                {
                    Identifier = ServerConfiguration.ApplicationIdentifier,
                };

                try
                {
                    appCore.Save(value.Convert(), User.Identity.Editor(), userApplication);

                    ViewBag.CanCreateAnApplication = this.CanCreateAnApplication();
                    return this.RedirectToAction("Application");
                }
                catch (Exception ex)
                {
                    log.Log(ex, EventTypes.Error, (int)Fault.Unknown);
                    return View();
                }
            }
        }

        /// <summary>
        /// Get Applications
        /// </summary>
        /// <returns>Application Details Models</returns>
        private IEnumerable<ApplicationDetailsModel> GetApplications()
        {
            var applications = appCore.Get(User.Identity.Data(), ServerConfiguration.ApplicationIdentifier);
            return applications.Convert(User.Identity.Data());
        }

        /// <summary>
        /// Check if the current user Can Create An Application
        /// </summary>
        /// <returns>Can Create An Application</returns>
        private bool CanCreateAnApplication()
        {
            using (new PerformanceMonitor())
            {
                return appCore.PermitApplicationCreation(Abc.Services.Contracts.Application.Current, User.Identity.Data());
            }
        }
        #endregion
    }
}