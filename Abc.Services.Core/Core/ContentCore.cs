// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='ContentCore.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Core
{
    using Abc.Azure;
    using Abc.Configuration;
    using Abc.Services.Contracts;
    using Abc.Services.Data;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    /// <summary>
    /// Content Management System Core
    /// </summary>
    public class ContentCore : DataCore
    {
        #region Members
        /// <summary>
        /// Content Management System Source
        /// </summary>
        private readonly ContentSource source = new ContentSource();
        #endregion

        #region Methods
        /// <summary>
        /// Store Binary Content
        /// </summary>
        /// <param name="content">Content</param>
        /// <returns>Binary Content</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "It is being validated upstream.")]
        public BinaryContent Store(BinaryContent content)
        {
            Contract.Requires<ArgumentNullException>(null != content);
            Contract.Requires<ArgumentNullException>(null != content.Content);
            Contract.Requires<ArgumentOutOfRangeException>(!string.IsNullOrEmpty(content.ContentType));
            Contract.Ensures(Contract.Result<BinaryContent>() != null);

            using (new PerformanceMonitor())
            {
                return new BinaryContent()
                {
                    Id = this.source.InsertBinary(content.Content, content.ContentType)
                };
            }
        }

        /// <summary>
        /// Get Binary Content
        /// </summary>
        /// <param name="content">Binary Content</param>
        /// <returns>Content</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "It is being validated upstream.")]
        public BinaryContent Get(BinaryContent content)
        {
            Contract.Requires<ArgumentNullException>(null != content);
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != content.Id);
            Contract.Ensures(Contract.Result<BinaryContent>() != null);

            using (new PerformanceMonitor())
            {
                return new BinaryContent()
                {
                    Id = content.Id,
                    Content = this.source.SelectBinary(content.Id)
                };
            }
        }

        /// <summary>
        /// Store Text
        /// </summary>
        /// <param name="text">Text</param>
        /// <returns>Text Id</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated upstream.")]
        public TextContent Store(TextContent text)
        {
            Contract.Requires<ArgumentNullException>(null != text);
            Contract.Requires<ArgumentNullException>(null != text.Token);
            Contract.Requires<ArgumentOutOfRangeException>(Guid.Empty != text.Token.ApplicationId);
            Contract.Ensures(Contract.Result<TextContent>() != null);

            using (new PerformanceMonitor())
            {
                var id = Guid.Empty;

                if (Guid.Empty == text.Id)
                {
                    var contentId = this.source.InsertText(text.Content);
                    var data = text.Convert();
                    data.ContentId = contentId;

                    id = this.source.Insert(data);
                }
                else
                {
                    var data = this.source.SelectText(text.Token.ApplicationId, text.Id);

                    data.Active = text.Active;
                    data.Deleted = text.Deleted;
                    data.UpdatedOn = DateTime.UtcNow;

                    this.source.UpdateText(data.ContentId, text.Content);
                    id = this.source.Update(data);
                }

                return new TextContent()
                {
                    Id = id
                };
            }
        }

        /// <summary>
        /// Get Text
        /// </summary>
        /// <param name="text">Text</param>
        /// <returns>Text Filled</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "By design."), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated upstream.")]
        public TextContent Get(TextContent text)
        {
            Contract.Requires<ArgumentNullException>(null != text);
            Contract.Ensures(Contract.Result<TextContent>() != null);

            using (new PerformanceMonitor())
            {
                var data = this.source.SelectText(text.Token.ApplicationId, text.Id);
                var content = data.Convert();
                content.Content = this.source.SelectText(data.ContentId);
                content.Token = text.Token;
                return content;
            }
        }

        /// <summary>
        /// Store XML
        /// </summary>
        /// <param name="xml">XML</param>
        /// <returns>XML Id</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated upstream.")]
        public XmlContent Store(XmlContent xml)
        {
            Contract.Requires<ArgumentNullException>(null != xml);
            Contract.Requires<ArgumentNullException>(null != xml.Token);
            Contract.Ensures(Contract.Result<XmlContent>() != null);

            using (new PerformanceMonitor())
            {
                var id = Guid.Empty;

                if (Guid.Empty == xml.Id)
                {
                    var contentId = this.source.InsertXml(xml.Content);

                    var data = xml.Convert();
                    data.ContentId = contentId;

                    id = this.source.Insert(data);
                }
                else
                {
                    var data = this.source.SelectXml(xml.Token.ApplicationId, xml.Id);

                    data.Active = xml.Active;
                    data.Deleted = xml.Deleted;
                    data.UpdatedOn = DateTime.UtcNow;

                    this.source.UpdateXml(data.ContentId, xml.Content);

                    id = this.source.Update(data);
                }

                return new XmlContent()
                {
                    Id = id
                };
            }
        }

        /// <summary>
        /// Get XML Content
        /// </summary>
        /// <param name="xml">XML</param>
        /// <returns>XML Filled</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "By design.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated upstream.")]
        public XmlContent Get(XmlContent xml)
        {
            Contract.Requires<ArgumentNullException>(null != xml);
            Contract.Ensures(Contract.Result<XmlContent>() != null);

            using (new PerformanceMonitor())
            {
                var data = this.source.SelectXml(xml.Token.ApplicationId, xml.Id);
                var content = data.Convert();
                content.Content = this.source.SelectXml(data.ContentId);
                content.Token = xml.Token;
                return content;
            }
        }

        /// <summary>
        /// Blog Entry
        /// </summary>
        /// <param name="entry">Blog Entry</param>
        /// <returns>Blog Entry</returns>
        public BlogEntry Store(BlogEntry entry)
        {
            Contract.Requires<ArgumentNullException>(null != entry);
            Contract.Requires<ArgumentException>(Guid.Empty != entry.SectionIdentifier);
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(entry.Title));
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(entry.Content));

            Contract.Ensures(Contract.Result<BlogEntry>() != null);

            using (new PerformanceMonitor())
            {
                entry.Identifier = this.source.InsertText(entry.Content);

                var table = new AzureTable<BlogRow>(ServerConfiguration.Default);
                table.AddEntity(entry.Convert());
            }

            return entry;
        }

        /// <summary>
        /// Blog Entry
        /// </summary>
        /// <param name="entry">Blog Entry</param>
        /// <returns>Blog Entries</returns>
        public IEnumerable<BlogEntry> Get(BlogEntry entry)
        {
            Contract.Requires<ArgumentNullException>(null != entry);
            Contract.Requires<ArgumentException>(Guid.Empty != entry.SectionIdentifier);

            Contract.Ensures(Contract.Result<IEnumerable<BlogEntry>>() != null);

            using (new PerformanceMonitor())
            {
                var table = new AzureTable<BlogRow>(ServerConfiguration.Default);
                var items = table.QueryByPartition(entry.SectionIdentifier.ToString());

                var data = (from item in items.ToList()
                       orderby item.PostedOn descending
                       select item.Convert()).ToList();

                BlogEntry content;
                if (Guid.Empty != entry.Identifier)
                {
                    content = (from item in data
                                   where item.Identifier == entry.Identifier
                                   select item).FirstOrDefault();
                }
                else
                {
                    content = data.FirstOrDefault();
                }

                if (null != content)
                {
                    data.Remove(content);
                    content.Content = this.source.SelectText(content.Identifier);
                    data.Add(content);
                }

                return data;
            }
        }
        #endregion
    }
}