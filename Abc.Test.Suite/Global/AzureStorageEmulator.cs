// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='AzureStorageEmulator.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test
{
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using Abc.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Azure Storage Emulator
    /// </summary>
    public class AzureStorageEmulator : Emulator
    {
        #region Members
        /// <summary>
        /// Azure Emulator
        /// </summary>
        private readonly string EmulatorFileName = Settings.Instance.Get("AzureEmulator");
        #endregion

        #region Methods
        /// <summary>
        /// Run Emulator
        /// </summary>
        [AssemblyInitialize]
        public override void Run()
        {
            var count = Process.GetProcessesByName("DSService").Length;
            if (count == 0)
            {
                var storage = new ProcessStartInfo();
                storage.Arguments = "/devstore:start";
                storage.FileName = this.EmulatorFileName;

                using (var proc = new Process())
                {
                    proc.StartInfo = storage;
                    proc.Start();
                    proc.WaitForExit();
                }
            }

            base.Run();
        }

        /// <summary>
        /// Stop
        /// </summary>
        public override void Terminate()
        {
            var storage = new ProcessStartInfo();
            storage.Arguments = "/devstore:shutdown";
            storage.FileName = this.EmulatorFileName;

            using (var proc = new Process())
            {
                proc.StartInfo = storage;
                proc.Start();
                proc.WaitForExit();
            }

            base.Terminate();
        }

        /// <summary>
        /// Invariant Contract
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Invariant Contract")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Invariant Contract")]
        [ContractInvariantMethod]
        private void InvariantContract()
        {
            Contract.Invariant(!string.IsNullOrWhiteSpace(this.EmulatorFileName));
        }
        #endregion
    }
}