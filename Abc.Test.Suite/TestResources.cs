// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='TestResources.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test
{
    using System;
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// Test Resources
    /// </summary>
    public class TestResources : ResourceLoader
    {
        #region Methods
        /// <summary>
        /// Loads an embedded resource.
        /// </summary>
        /// <param name="folder">Top-level folder to load resource from.</param>
        /// <param name="fileName">File name of resource.</param>
        /// <returns>Stream of resource file contents.</returns>
        protected override Stream GetResourceStream(string folder, string fileName)
        {
            var nameSpace = NamespaceFormat.FormatWithCulture(folder, fileName);
            var assembly = Assembly.GetExecutingAssembly();
            var resourceStream = assembly.GetManifestResourceStream(nameSpace);
            if (resourceStream == null)
            {
                throw new FileNotFoundException(string.Format("Embedded resource '{0}' was not found.", nameSpace));
            }

            return resourceStream;
        }
        #endregion
    }
}