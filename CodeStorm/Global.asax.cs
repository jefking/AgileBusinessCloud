// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='Global.asax.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Code
{
    using Abc.Azure.Configuration;
    using Abc.Configuration;
    using Abc.Services;
    using Abc.Services.Contracts;
    using Abc.Services.Core;
    using Abc.Services.Data;
    using System;
    using System.Web;
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;
    using Admin = Abc.Underpinning.Administration;

    /// <summary>
    /// MVC Application
    /// </summary>
    public class MvcApplication : System.Web.HttpApplication
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
        static MvcApplication()
        {
            Settings.Instance.Add(new IConfigurationAdaptor[] { new RoleEnvironmentAdaptor(), new AppSettingsAdaptor() });
        }
        #endregion

        #region Methods
        /// <summary>
        /// Application Start
        /// </summary>
        protected void Application_Start()
        {
            using (new PerformanceMonitor())
            {
                TableRegister.Initialize();

                logger.Log("Application Start Called.");

                var app = new Admin.Application();
                bool loggedOn = app.LogOn();

                logger.Log(string.Format("Logged On: {0}", loggedOn));
                
                AreaRegistration.RegisterAllAreas();

                WebApiConfig.Register(GlobalConfiguration.Configuration);
                FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
                RouteConfig.RegisterRoutes(RouteTable.Routes);
                BundleConfig.RegisterBundles(BundleTable.Bundles);
                AuthConfig.RegisterAuth();

                logger.Log("Application Started.");
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
                    logger.Log(context.Error, EventTypes.Error, 9999);
                    context.ClearError();
                }
            }
        }
        #endregion
    }
}