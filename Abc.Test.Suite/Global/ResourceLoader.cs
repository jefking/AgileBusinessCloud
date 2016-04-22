// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='ResourceLoader.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test
{
    using System;
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// Loads embedded resources for use in tests.
    /// </summary>
    public class ResourceLoader
    {
        #region Members
        /// <summary>
        /// Name Space Format
        /// </summary>
        protected const string NamespaceFormat = "Abc.Test.{0}.Resources.{1}";
        #endregion

        #region Properties
        /// <summary>
        /// Gets Working Directory
        /// </summary>
        internal static string WorkingDirectory
        {
            get
            {
                return Environment.CurrentDirectory;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Save Resource To Disk
        /// </summary>
        /// <param name="folder">Folder</param>
        /// <param name="fileName">File Name</param>
        /// <returns>File Saved to Disk (Path)</returns>
        public string SaveToDisk(string folder, string fileName)
        {
            return this.SaveToDisk(folder, fileName, @"{0}\{1}".FormatWithCulture(ResourceLoader.WorkingDirectory, folder));
        }

        /// <summary>
        /// Save To Disk
        /// </summary>
        /// <param name="folder">Folder</param>
        /// <param name="fileName">File Name</param>
        /// <param name="path">Path to Save To</param>
        /// <returns>File Saved to Disk (Path)</returns>
        public string SaveToDisk(string folder, string fileName, string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path = @"{0}\{1}".FormatWithCulture(path, fileName);
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                byte[] bytes = this.GetBytes(folder, fileName);
                foreach (byte b in bytes)
                {
                    fs.WriteByte(b);
                }
            }

            return path;
        }

        /// <summary>
        /// Loads an embedded source in string format.
        /// </summary>
        /// <param name="folder">Top-level folder to load resource from.</param>
        /// <param name="fileName">File name of resource.</param>
        /// <returns>Content of resource file.</returns>
        public string GetString(string folder, string fileName)
        {
            using (var resourceStream = this.GetResourceStream(folder, fileName))
            {
                var reader = new StreamReader(resourceStream);
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Loads an embedded resource in binary format.
        /// </summary>
        /// <param name="folder">Top-level folder to load resource from.</param>
        /// <param name="fileName">File name of resource.</param>
        /// <returns>Content of resource file.</returns>
        public byte[] GetBytes(string folder, string fileName)
        {
            using (var resourceStream = this.GetResourceStream(folder, fileName))
            {
                var data = new byte[resourceStream.Length];
                resourceStream.Read(data, 0, data.Length);
                return data;
            }
        }

        /// <summary>
        /// Loads an embedded resource as a stream.
        /// </summary>
        /// <param name="folder">Top-level folder to load resource from.</param>
        /// <param name="fileName">File name of resource.</param>
        /// <returns>Content of resource file.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Handled outside of method")]
        public Stream GetStream(string folder, string fileName)
        {
            var data = this.GetBytes(folder, fileName);
            var memStream = new MemoryStream(data, true);
            memStream.Write(data, 0, data.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            return memStream;
        }

        /// <summary>
        /// Loads an embedded resource.
        /// </summary>
        /// <param name="folder">Top-level folder to load resource from.</param>
        /// <param name="fileName">File name of resource.</param>
        /// <returns>Stream of resource file contents.</returns>
        protected virtual Stream GetResourceStream(string folder, string fileName)
        {
            var nameSpace = NamespaceFormat.FormatWithCulture(folder, fileName);
            var assembly = Assembly.GetExecutingAssembly();
            var resourceStream = assembly.GetManifestResourceStream(nameSpace);
            if (resourceStream == null)
            {
                throw new FileNotFoundException("Embedded resource '{0}' was not found.".FormatWithCulture(nameSpace));
            }

            return resourceStream;
        }
        #endregion
    }
}