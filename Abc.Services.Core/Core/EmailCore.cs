// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='EmailCore.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Core
{
    using System;
    using System.Diagnostics.Contracts;
    using Abc.Azure;
    using Abc.Configuration;
    using Abc.Services.Contracts;
    using Abc.Services.Data;

    /// <summary>
    /// Email Core
    /// </summary>
    public class EmailCore : DataCore
    {
        #region Methods
        /// <summary>
        /// Send Binary Email
        /// </summary>
        /// <param name="email">Email</param>
        /// <returns>Email Id</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "By Design")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Code contracts")]
        public Guid Send(BinaryEmail email)
        {
            Contract.Requires<ArgumentNullException>(null != email);
            
            Contract.Requires<ArgumentOutOfRangeException>(!string.IsNullOrWhiteSpace(email.Sender));
            Contract.Requires<ArgumentOutOfRangeException>(!string.IsNullOrWhiteSpace(email.Recipient));

            Contract.Requires<ArgumentOutOfRangeException>(null != email.RawMessage);

            MailGun.Send(email.Sender, email.Recipient, email.RawMessage);

            var table = new AzureTable<BinaryEmailData>(ServerConfiguration.Default);
            var data = email.Convert();
            table.AddEntity(data);

            return data.Id;
        }

        /// <summary>
        /// Send Plain Text Email
        /// </summary>
        /// <param name="email">Email</param>
        /// <returns>Email Id</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "By Design")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Code contracts")]
        public Guid Send(PlaintextEmail email)
        {
            Contract.Requires<ArgumentNullException>(email != null);

            Contract.Requires<ArgumentOutOfRangeException>(!string.IsNullOrWhiteSpace(email.Sender));
            Contract.Requires<ArgumentOutOfRangeException>(!string.IsNullOrWhiteSpace(email.Recipient));

            Contract.Requires<ArgumentOutOfRangeException>(!string.IsNullOrWhiteSpace(email.Subject));
            Contract.Requires<ArgumentOutOfRangeException>(!string.IsNullOrWhiteSpace(email.Message));

            MailGun.Send(email.Sender, email.Recipient, email.Subject, email.Message);

            var table = new AzureTable<PlaintextEmailData>(ServerConfiguration.Default);
            var data = email.Convert();
            table.AddEntity(data);

            return data.Id;
        }
        #endregion
    }
}