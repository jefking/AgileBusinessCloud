// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='AzureComputeEmulator.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test
{
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using Abc;
    using Abc.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Azure Compute Emulator
    /// </summary>
    public class AzureComputeEmulator : AzureStorageEmulator
    {
        #region Members
        /// <summary>
        /// CSX File
        /// </summary>
        private readonly string csx;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the AzureComputeEmulator class.
        /// </summary>
        /// <param name="csx">CSX File</param>
        public AzureComputeEmulator(string csx)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(csx));

            this.csx = csx;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether Is Debug
        /// </summary>
        public bool IsDebug
        {
            get
            {
#if (DEBUG)
                return true;
#else
                return false;
#endif
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Run Emulator
        /// </summary>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Test Code")]
        [AssemblyInitialize]
        public override void Run()
        {
            // Storage Emulator must be running to deploy to Azure.
            base.Run();

            var compute = new ProcessStartInfo();
            var build = this.IsDebug ? "Debug" : "Release";
            compute.Arguments = "/run:\"{0}AgileBusinessCloud\\Application{1}\\csx\\{2}\";\"{0}AgileBusinessCloud\\Application{1}\\ServiceConfiguration.cscfg\"".FormatWithCulture(RootDirectory, this.csx, build);

            compute.FileName = Settings.Instance.Get("AzureEmulator");
            
            var proc = new Process();
            proc.StartInfo = compute;
            proc.Start();
            proc.WaitForExit();
        }

        /// <summary>
        /// Stop
        /// </summary>
        public override void Terminate()
        {
            foreach (var process in Process.GetProcessesByName("DFService"))
            {
                Emulator.KillProcess(process);
            }

            base.Terminate();
        }

        /// <summary>
        /// Invariant Contract
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Contract Invariant")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Invariant Contract")]
        [ContractInvariantMethod]
        private void InvariantContract()
        {
            Contract.Invariant(!string.IsNullOrWhiteSpace(this.csx));
        }
        #endregion
    }
}