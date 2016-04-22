// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='WebRole.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Website
{
    using Abc.Azure.Configuration;
    using Abc.Configuration;
    using Microsoft.WindowsAzure.ServiceRuntime;

    /// <summary>
    /// Web Role
    /// </summary>
    public class WebRole : RoleEntryPoint
    {
        #region Constructors
        /// <summary>
        /// Initializes static members of the WebRole class.
        /// </summary>
        static WebRole()
        {
            Settings.Instance.Add(new RoleEnvironmentAdaptor());
        }
        #endregion
    }
}