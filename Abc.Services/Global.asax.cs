// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='Global.asax.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services
{
    using System;
    using System.Web;
    using Abc.Azure.Configuration;
    using Abc.Configuration;
    using Abc.Services.Contracts;
    using Abc.Services.Core;
    using Abc.Services.Data;

    /// <summary>
    /// Global
    /// </summary>
    public class Global : HttpApplication
    {
        #region Members
        /// <summary>
        /// Log
        /// </summary>
        private readonly LogCore log = new LogCore();
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
        /// Application Error
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event Arguments</param>
        public void Application_Error(object sender, EventArgs e)
        {
            HttpContext context = HttpContext.Current;
            if (null != context
                && null != context.Error)
            {
                this.log.Log(context.Error, EventTypes.Error, (int)ServiceFault.Unknown);
                context.ClearError();
            }
        }

        /// <summary>
        /// Application Start
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Abc.Services.Core.LogCore.Log(System.String)", Justification = "Not a localization issue.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Diagnostics.Contracts.Wrappers.Abc.Services.LogCore.V$Log(Abc.Services.LogCore,System.String)", Justification = "No current need to localize"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Application Start Method")]
        protected void Application_Start()
        {
            using (new PerformanceMonitor())
            {
                TableRegister.Initialize();

                this.log.Log("Application Start Called.");

                var app = new Abc.Underpinning.Administration.Application();
                bool loggedOn = app.LogOn();

                this.log.Log("Logged On: {0}".FormatWithCulture(loggedOn));

                this.log.Log("Application Started.");
            }
        }
        #endregion
    }
}