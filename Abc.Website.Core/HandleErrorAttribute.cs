// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='HandleErrorAttribute.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Website
{
    using System;
    using System.Web.Mvc;
    using Abc.Services.Contracts;
    using Abc.Services.Core;

    /// <summary>
    /// Handle Error Attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class HandleErrorAttribute : System.Web.Mvc.HandleErrorAttribute
    {
        #region Members
        /// <summary>
        /// Log Core
        /// </summary>
        private static readonly LogCore logger = new LogCore();
        #endregion

        #region Methods
        /// <summary>
        /// On Exception
        /// </summary>
        /// <param name="filterContext">Filter Context</param>
        public override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);

            if (null != filterContext && null != filterContext.Exception && !filterContext.ExceptionHandled)
            {
                logger.Log(filterContext.Exception, EventTypes.Error, (int)Fault.Unknown);
            }
        }
        #endregion
    }
}