// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='Global.asax.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Website
{
    using Abc.Azure.Configuration;
    using Abc.Configuration;
    using Abc.Services;
    using Abc.Services.Contracts;
    using Abc.Services.Core;
    using Abc.Services.Data;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.IdentityModel.Web;
    using Microsoft.IdentityModel.Web.Configuration;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Admin = Abc.Underpinning.Administration;

    /// <summary>
    /// MVC Application
    /// </summary>
    public class Global : System.Web.HttpApplication
    {
        #region Members
        /// <summary>
        /// Logger
        /// </summary>
        private readonly LogCore logger = new LogCore();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes static members of the Global class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Lifetime of application.")]
        static Global()
        {
            Settings.Instance.Add(new IConfigurationAdaptor[] { new RoleEnvironmentAdaptor(), new AppSettingsAdaptor() });
        }
        #endregion

        #region Methods
        /// <summary>
        /// Register Global Filters
        /// </summary>
        /// <param name="filters">Filters</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Abc.Services.LogCore.Log(System.String)", Justification = "No need to localize")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Diagnostics.Contracts.Wrappers.Abc.Services.LogCore.V$Log(Abc.Services.LogCore,System.String)", Justification = "No current need to localize")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated")]
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            Contract.Requires<ArgumentNullException>(null != filters);

            using (new PerformanceMonitor())
            {
                var logger = new LogCore();
                logger.Log("Register Global Filters Called.");

                filters.Add(new HandleErrorAttribute
                {
                    ExceptionType = typeof(HttpException),
                    Order = 2,
                });
                filters.Add(new HandleErrorAttribute
                {
                    ExceptionType = typeof(HttpCompileException),
                    Order = 3
                });
                filters.Add(new HandleErrorAttribute());

                logger.Log("Register Global Filters Completed.");
            }
        }

        /// <summary>
        /// Register Routes
        /// </summary>
        /// <param name="routes">Routes</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Abc.Services.LogCore.Log(System.String)", Justification = "Routes shouldn't be localized."), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Diagnostics.Contracts.Wrappers.Abc.Services.LogCore.V$Log(Abc.Services.LogCore,System.String)", Justification = "No current need to localize")]
        public static void RegisterRoutes(RouteCollection routes)
        {
            Contract.Requires<ArgumentNullException>(null != routes);

            using (new PerformanceMonitor())
            {
                var logger = new LogCore();
                logger.Log("Register Routes Called.");

                routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

                routes.MapRoute("Profile", "Profile/{username}", new { controller = "Profile", action = "Index", username = UrlParameter.Optional });
                routes.MapRoute("Default", "{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = UrlParameter.Optional });
                routes.MapRoute("Api", "Data/{controller}/{action}/{id}");
                routes.MapRoute("404-PageNotFound", "{url}", new { controller = "Home", action = "NotFound" });  // 404s

                logger.Log("Register Routes Completed.");
            }
        }

        /// <summary>
        /// Application Error
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event Arguments</param>
        public void Application_Error(object sender, EventArgs e)
        {
            using (new PerformanceMonitor())
            {
                var context = HttpContext.Current;
                if (null != context
                    && null != context.Error)
                {
                    logger.Log(context.Error, EventTypes.Error, (int)Fault.Unknown);
                    context.ClearError();
                }
            }
        }

        /// <summary>
        /// Application Start
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Abc.Services.LogCore.Log(System.String)", Justification = "No need to localize")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Diagnostics.Contracts.Wrappers.Abc.Services.LogCore.V$Log(Abc.Services.LogCore,System.String)", Justification = "No current need to localize")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Application Start Method")]
        protected void Application_Start()
        {
            using (new PerformanceMonitor())
            {
                TableRegister.Initialize();

                logger.Log("Application Start Called.");

                var app = new Admin.Application();
                bool loggedOn = app.LogOn();

                logger.Log("Logged On: {0}".FormatWithCulture(loggedOn));

                FederatedAuthentication.ServiceConfigurationCreated += this.OnServiceConfigurationCreated;

                AreaRegistration.RegisterAllAreas();

                Contract.Assume(null != GlobalFilters.Filters);
                RegisterGlobalFilters(GlobalFilters.Filters);

                Contract.Assume(null != RouteTable.Routes);
                RegisterRoutes(RouteTable.Routes);

                logger.Log("Application Started.");
            }
        }

        /// <summary>
        /// On Service Configuration Created
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Service Configuration Created Event Args</param>
        private void OnServiceConfigurationCreated(object sender, ServiceConfigurationCreatedEventArgs e)
        {
            using (new PerformanceMonitor())
            {
                try
                {
                    var sessionTransforms = new List<CookieTransform>(new CookieTransform[]
                    {
                        new DeflateCookieTransform(),
                        new RsaEncryptionCookieTransform(e.ServiceConfiguration.ServiceCertificate),
                        new RsaSignatureCookieTransform(e.ServiceConfiguration.ServiceCertificate)
                    });

                    var sessionHandler = new SessionSecurityTokenHandler(sessionTransforms.AsReadOnly());
                    e.ServiceConfiguration.SecurityTokenHandlers.AddOrReplace(sessionHandler);
                }
                catch (ArgumentNullException aex)
                {
                    logger.Log(aex, EventTypes.Error, (int)Fault.CertificateNotSpecified);
                }
            }
        }
        #endregion
    }
}