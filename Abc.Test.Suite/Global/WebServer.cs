// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='WebServer.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test
{
    using System.Diagnostics;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Web Server
    /// </summary>
    public class WebServer : Emulator
    {
        #region Members
        /// <summary>
        /// Web Server Path
        /// </summary>
        private const string WebserverPath = @"C:\Program Files (x86)\Common Files\microsoft shared\DevServer\10.0\WebDev.WebServer40.exe";
        
        /// <summary>
        /// Web Application
        /// </summary>
        private readonly string webApplication;

        /// <summary>
        /// Port
        /// </summary>
        private readonly int port = 80;

        /// <summary>
        /// Server Process
        /// </summary>
        private Process serverProcess;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the WebServer class.
        /// </summary>
        /// <param name="path">Path</param>
        /// <param name="port">Port</param>
        public WebServer(string path, int port)
        {
            Contract.Requires(!string.IsNullOrEmpty(path));
            Contract.Requires(0 < port && 66000 > port);

            this.webApplication = @"{0}{1}".FormatWithCulture(RootDirectory, path);
            this.port = port;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Run Emulator
        /// </summary>
        public override void Run()
        {
            this.serverProcess = Process.Start(WebserverPath, "/port:{0} /path:\"{1}\"".FormatWithCulture(this.port, this.webApplication));
            
            base.Run();
        }

        /// <summary>
        /// Stop
        /// </summary>
        public override void Terminate()
        {
            Emulator.KillProcess(this.serverProcess);

            base.Terminate();
        }

        /// <summary>
        /// Contract Invariant
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Invariant Contract"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Invariant Contract"), ContractInvariantMethod]
        private void ContractInvariant()
        {
            Contract.Requires(!string.IsNullOrEmpty(this.webApplication));
            Contract.Requires(0 < this.port && 66000 > this.port);
        }
        #endregion
    }
}