// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='BlogEntry.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Contracts
{
    using System;
    using Abc.Services.Data;

    /// <summary>
    /// Blog Entry
    /// </summary>
    public class BlogEntry : IIdentifier<Guid>, IConvert<BlogRow>
    {
        #region Members
        /// <summary>
        /// Mark Woodward
        /// </summary>
        public static readonly Guid MarkWoodward = Guid.Parse("6EC0D941-E6F6-4DCA-8B11-D2C648853583");

        /// <summary>
        /// Company
        /// </summary>
        public static readonly Guid Company = Guid.Parse("2CAC744E-3544-4DBA-9167-FB75EA9516DA");

        /// <summary>
        /// Jef King
        /// </summary>
        public static readonly Guid JefKing = Guid.Parse("42F64C8D-28AC-4306-B699-CBAA11854B55");

        /// <summary>
        /// Jaime Bueza
        /// </summary>
        public static readonly Guid JaimeBueza = Guid.Parse("076793A3-9FCF-4DBD-8121-5CC05ADC476B");
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the Identifier
        /// </summary>
        public Guid Identifier
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Blog Section
        /// </summary>
        public Guid SectionIdentifier
        {
            get;
            set;
        }

        /// <summary>
        /// Content
        /// </summary>
        public string Content
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Title
        /// </summary>
        public string Title
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Posted On Date
        /// </summary>
        public DateTime PostedOn
        {
            get;
            set;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Convert to Blog Row
        /// </summary>
        /// <returns>Blog Row</returns>
        [CLSCompliant(false)]
        public BlogRow Convert()
        {
            return new BlogRow(this.SectionIdentifier, this.Identifier)
            {
                PostedOn = this.PostedOn,
                Title = this.Title,
            };
        }
        #endregion
    }
}