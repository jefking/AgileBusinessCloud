// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='AzureEnvironment.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Azure
{
    using System;
    using Microsoft.WindowsAzure.ServiceRuntime;

    /// <summary>
    /// Azure Environment Information
    /// </summary>
    public static class AzureEnvironment
    {
        #region Methods
        /// <summary>
        /// Deployment Id Lock
        /// </summary>
        private static readonly object deploymentIdLock = new object();

        /// <summary>
        /// Deployment Set
        /// </summary>
        private static bool deploymentSet = false;

        /// <summary>
        /// Deployment Identifier
        /// </summary>
        private static string deploymentId = null;
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether Role Is Available (Safely)
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Safety first.")]
        public static bool RoleIsAvailable
        {
            get
            {
                try
                {
                    return RoleEnvironment.IsAvailable;
                }
                catch
                {
                }

                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether Is Compute Emulator
        /// </summary>
        public static bool IsComputeEmulator
        {
            get
            {
                return AzureEnvironment.RoleIsAvailable && RoleEnvironment.IsEmulated;
            }
        }

        /// <summary>
        /// Gets Deployment Identifier for Azure Server
        /// </summary>
        public static string DeploymentId
        {
            get
            {
                lock (deploymentIdLock)
                {
                    if (!deploymentSet)
                    {
                        deploymentId = AzureEnvironment.RoleIsAvailable ? RoleEnvironment.DeploymentId : null;

                        deploymentSet = true;
                    }
                }

                return deploymentId;
            }
        }

        /// <summary>
        /// Gets Azure Server Name
        /// </summary>
        public static string ServerName
        {
            get
            {
                return Environment.MachineName ?? DeploymentId;
            }
        }
        #endregion
    }
}