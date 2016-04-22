// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='Emulator.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;

    /// <summary>
    /// Testing Emulator
    /// </summary>
    public class Emulator
    {
        #region Properties
        /// <summary>
        /// Gets Root Directory
        /// </summary>
        /// <remarks>
        /// AgileBusinessCloud: Development Root
        /// Build: Jenkins Root
        /// </remarks>
        protected static string RootDirectory
        {
            get
            {
                string rootDirectory = Environment.CurrentDirectory;
                if (rootDirectory.Contains("AgileBusinessCloud"))
                {
                    return rootDirectory.Substring(0, rootDirectory.IndexOf("AgileBusinessCloud", StringComparison.OrdinalIgnoreCase));
                }
                else if (rootDirectory.Contains("Build"))
                {
                    return rootDirectory.Substring(0, rootDirectory.IndexOf("Build", StringComparison.OrdinalIgnoreCase));
                }
                else
                {
                    throw new InvalidOperationException("Unknown directory: {0}".FormatWithCulture(rootDirectory));
                }
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Run Emulator
        /// </summary>
        public virtual void Run()
        {
        }

        /// <summary>
        /// Stop Emulator
        /// </summary>
        public virtual void Terminate()
        {
        }

        /// <summary>
        /// Kills a process
        /// </summary>
        /// <param name="process">Process to Kill</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Code contracts")]
        protected static void KillProcess(Process process)
        {
            try
            {
                while (!process.HasExited)
                {
                    process.Kill();
                    process.WaitForExit(5);
                }
            }
            catch (Win32Exception)
            {
            }
            catch (InvalidOperationException)
            {
            }
        }
        #endregion
    }
}