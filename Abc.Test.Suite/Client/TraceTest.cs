// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='TraceTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using Abc.Configuration;
    using Abc.Services.Core;
    using Abc.Underpinning;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Client Side Tracing Test
    /// </summary>
    [TestClass]
    public class TraceTest
    {
        #region Valid Cases
        [TestMethod]
        public void IsThreadSafe()
        {
            var trace = new Abc.Diagnostics.TraceListener();
            Assert.IsTrue(trace.IsThreadSafe);
        }

        [TestMethod]
        public void Write()
        {
            var text = Guid.NewGuid().ToString();
            Trace.Write(text);

            Trace.Flush();

            var message = this.GetMessage(text);
            Assert.IsNotNull(message, "Message should not be null");
            Assert.AreEqual<Guid>(Application.Identifier, message.Token.ApplicationId, "Application Id should match");
            Assert.AreEqual<DateTime>(DateTime.UtcNow.Date, message.OccurredOn.Date, "Occured On should match");
            Assert.AreEqual<string>(Environment.MachineName, message.MachineName, "Machine Name should match");
            Assert.AreEqual<string>(text, message.Message, "Message should match");
        }
        
        [TestMethod]
        public void WriteInvalidMessage()
        {
            this.CleanUp();

            var now = DateTime.UtcNow;

            Trace.Write(StringHelper.NullEmptyWhiteSpace());

            Trace.Flush();

            var source = new LogCore();
            var query = new Abc.Services.Contracts.LogQuery()
            {
                ApplicationIdentifier = Settings.ApplicationIdentifier,
                From = now,
            };
            var messages = source.SelectMessages(query);
            Assert.IsNotNull(messages, "Messages should not be null");
            Assert.AreEqual<int>(0, messages.Count());
        }

        [TestMethod]
        public void WriteLine()
        {
            var text = Guid.NewGuid().ToString();
            Trace.WriteLine(text);

            Trace.Flush();

            var message = this.GetMessage(text);
            Assert.IsNotNull(message, "Message should not be null");
            Assert.AreEqual<Guid>(Application.Identifier, message.Token.ApplicationId, "Application Id should match");
            Assert.AreEqual<DateTime>(DateTime.UtcNow.Date, message.OccurredOn.Date, "Occured On should match");
            Assert.AreEqual<string>(Environment.MachineName, message.MachineName, "Machine Name should match");
            Assert.AreEqual<string>(text, message.Message, "Message should match");
        }

        [TestMethod]
        public void WriteLineInvalidMessage()
        {
            this.CleanUp();

            var now = DateTime.UtcNow;

            Trace.WriteLine(StringHelper.NullEmptyWhiteSpace());

            Trace.Flush();

            var source = new LogCore();
            var query = new Abc.Services.Contracts.LogQuery()
            {
                ApplicationIdentifier = Application.Identifier,
                From = now,
            };
            var messages = source.SelectMessages(query);
            Assert.IsNotNull(messages, "Messages should not be null");
            Assert.AreEqual<int>(0, messages.Count());
        }

        [TestMethod]
        public void Flush()
        {
            Trace.Flush();

            string text = Guid.NewGuid().ToString();
            Trace.Write(text);

            Trace.Flush();

            var message = this.GetMessage(text);
            Assert.IsNotNull(message, "Message should not be null");
            Assert.AreEqual<Guid>(Application.Identifier, message.Token.ApplicationId, "Application Id should match");
            Assert.AreEqual<DateTime>(DateTime.UtcNow.Date, message.OccurredOn.Date, "Occured On should match");
            Assert.AreEqual<string>(Environment.MachineName, message.MachineName, "Machine Name should match");
            Assert.AreEqual<string>(text, message.Message, "Message should match");
        }

        [TestMethod]
        public void WriteMessageCategory()
        {
            string msg = Guid.NewGuid().ToString();
            string category = Guid.NewGuid().ToString();
            Trace.Write(msg, category);

            Trace.Flush();

            string text = "{1}: {0}".FormatWithCulture(msg, category);

            var message = this.GetMessage(text);
            Assert.IsNotNull(message, "Message should not be null");
            Assert.AreEqual<Guid>(Application.Identifier, message.Token.ApplicationId, "Application Id should match");
            Assert.AreEqual<DateTime>(DateTime.UtcNow.Date, message.OccurredOn.Date, "Occured On should match");
            Assert.AreEqual<string>(Environment.MachineName, message.MachineName, "Machine Name should match");
            Assert.AreEqual<string>(text, message.Message, "Message should match");
        }

        [TestMethod]
        public void WriteMessageCategoryInvalidMessage()
        {
            string msg = StringHelper.NullEmptyWhiteSpace();
            string category = Guid.NewGuid().ToString();
            Trace.Write(msg, category);

            Trace.Flush();

            string text = "{0}: {1}".FormatWithCulture(category, msg ?? "null");

            var message = this.GetMessage(text);
            Assert.IsNotNull(message, "Message should not be null");
            Assert.AreEqual<Guid>(Application.Identifier, message.Token.ApplicationId, "Application Id should match");
            Assert.AreEqual<DateTime>(DateTime.UtcNow.Date, message.OccurredOn.Date, "Occured On should match");
            Assert.AreEqual<string>(Environment.MachineName, message.MachineName, "Machine Name should match");
            Assert.AreEqual<string>(text, message.Message, "Message should match");
        }

        [TestMethod]
        public void WriteMessageCategoryInvalidCategory()
        {
            string msg = Guid.NewGuid().ToString();
            string category = StringHelper.NullEmptyWhiteSpace();
            Trace.Write(msg, category);

            Trace.Flush();

            string text = "{1}: {0}".FormatWithCulture(msg, category ?? "null");

            var message = this.GetMessage(text);
            Assert.IsNotNull(message, "Message should not be null");
            Assert.AreEqual<Guid>(Application.Identifier, message.Token.ApplicationId, "Application Id should match");
            Assert.AreEqual<DateTime>(DateTime.UtcNow.Date, message.OccurredOn.Date, "Occured On should match");
            Assert.AreEqual<string>(Environment.MachineName, message.MachineName, "Machine Name should match");
            Assert.AreEqual<string>(text, message.Message, "Message should match");
        }

        [TestMethod]
        public void WriteMessageCategoryInvalidMessageAndCategory()
        {
            this.CleanUp();

            var now = DateTime.UtcNow;

            Trace.Write(StringHelper.NullEmptyWhiteSpace(), StringHelper.NullEmptyWhiteSpace());

            Trace.Flush();

            var source = new LogCore();
            var query = new Abc.Services.Contracts.LogQuery()
            {
                ApplicationIdentifier = Settings.ApplicationIdentifier,
                From = now,
            };
            var messages = source.SelectMessages(query);
            Assert.IsNotNull(messages, "Messages should not be null");
            Assert.AreEqual<int>(0, messages.Count());
        }

        [TestMethod]
        public void WriteObject()
        {
            object obj = Guid.NewGuid();
            Trace.Write(obj);

            Trace.Flush();

            string text = obj.ToString();

            var message = this.GetMessage(text);
            Assert.IsNotNull(message, "Message should not be null");
            Assert.AreEqual<Guid>(Application.Identifier, message.Token.ApplicationId, "Application Id should match");
            Assert.AreEqual<DateTime>(DateTime.UtcNow.Date, message.OccurredOn.Date, "Occured On should match");
            Assert.AreEqual<string>(Environment.MachineName, message.MachineName, "Machine Name should match");
            Assert.AreEqual<string>(text, message.Message, "Message should match");
        }

        [TestMethod]
        public void WriteObjectNull()
        {
            this.CleanUp();

            var now = DateTime.UtcNow;

            Trace.Write((object)null);

            Trace.Flush();

            var source = new LogCore();
            var query = new Abc.Services.Contracts.LogQuery()
            {
                ApplicationIdentifier = Settings.ApplicationIdentifier,
                From = now,
            };
            var messages = source.SelectMessages(query);
            Assert.IsNotNull(messages, "Messages should not be null");
            Assert.AreEqual<int>(0, messages.Count());
        }

        [TestMethod]
        public void WriteObjectCategory()
        {
            object obj = Guid.NewGuid();
            string category = Guid.NewGuid().ToString();
            Trace.Write(obj, category);

            Trace.Flush();

            string text = "{1}: {0}".FormatWithCulture(obj, category);

            var message = this.GetMessage(text);
            Assert.IsNotNull(message, "Message should not be null");
            Assert.AreEqual<Guid>(Application.Identifier, message.Token.ApplicationId, "Application Id should match");
            Assert.AreEqual<DateTime>(DateTime.UtcNow.Date, message.OccurredOn.Date, "Occured On should match");
            Assert.AreEqual<string>(Environment.MachineName, message.MachineName, "Machine Name should match");
            Assert.AreEqual<string>(text, message.Message, "Message should match");
        }

        [TestMethod]
        public void WriteObjectCategoryNullObject()
        {
            object obj = null;
            string category = Guid.NewGuid().ToString();
            Trace.Write(obj, category);

            Trace.Flush();

            string text = "{0}: null".FormatWithCulture(category);

            var message = this.GetMessage(text);
            Assert.IsNotNull(message, "Message should not be null");
            Assert.AreEqual<Guid>(Application.Identifier, message.Token.ApplicationId, "Application Id should match");
            Assert.AreEqual<DateTime>(DateTime.UtcNow.Date, message.OccurredOn.Date, "Occured On should match");
            Assert.AreEqual<string>(Environment.MachineName, message.MachineName, "Machine Name should match");
            Assert.AreEqual<string>(text, message.Message, "Message should match");
        }

        [TestMethod]
        public void WriteObjectCategoryInvalidCategory()
        {
            object obj = Guid.NewGuid();
            string category = StringHelper.NullEmptyWhiteSpace();
            Trace.Write(obj, category);

            Trace.Flush();

            string text = "{1}: {0}".FormatWithCulture(obj, category ?? "null");

            var message = this.GetMessage(text);
            Assert.IsNotNull(message, "Message should not be null");
            Assert.AreEqual<Guid>(Application.Identifier, message.Token.ApplicationId, "Application Id should match");
            Assert.AreEqual<DateTime>(DateTime.UtcNow.Date, message.OccurredOn.Date, "Occured On should match");
            Assert.AreEqual<string>(Environment.MachineName, message.MachineName, "Machine Name should match");
            Assert.AreEqual<string>(text, message.Message, "Message should match");
        }

        [TestMethod]
        public void WriteObjectCategoryNullObjectInvalidCategory()
        {
            this.CleanUp();

            var now = DateTime.UtcNow;

            Trace.Write((object)null, StringHelper.NullEmptyWhiteSpace());

            Trace.Flush();

            var source = new LogCore();
            var query = new Abc.Services.Contracts.LogQuery()
            {
                ApplicationIdentifier = Application.Identifier,
                From = now,
            };
            var messages = source.SelectMessages(query);
            Assert.IsNotNull(messages, "Messages should not be null");
            Assert.AreEqual<int>(0, messages.Count());
        }

        [TestMethod]
        public void WriteLineObject()
        {
            object obj = Guid.NewGuid();
            Trace.WriteLine(obj);

            Trace.Flush();

            string text = obj.ToString();

            var message = this.GetMessage(text);
            Assert.IsNotNull(message, "Message should not be null");
            Assert.AreEqual<Guid>(Application.Identifier, message.Token.ApplicationId, "Application Id should match");
            Assert.AreEqual<DateTime>(DateTime.UtcNow.Date, message.OccurredOn.Date, "Occured On should match");
            Assert.AreEqual<string>(Environment.MachineName, message.MachineName, "Machine Name should match");
            Assert.AreEqual<string>(text, message.Message, "Message should match");
        }

        [TestMethod]
        public void WriteLineObjectNull()
        {
            this.CleanUp();

            var now = DateTime.UtcNow;

            Trace.WriteLine((object)null);

            Trace.Flush();

            var source = new LogCore();
            var query = new Abc.Services.Contracts.LogQuery()
            {
                ApplicationIdentifier = Application.Identifier,
                From = now,
            };
            var messages = source.SelectMessages(query);
            Assert.IsNotNull(messages, "Messages should not be null");
            Assert.AreEqual<int>(0, messages.Count());
        }

        [TestMethod]
        public void WriteLineMessageCategory()
        {
            string msg = Guid.NewGuid().ToString();
            string category = Guid.NewGuid().ToString();
            Trace.WriteLine(msg, category);

            Trace.Flush();

            string text = "{1}: {0}".FormatWithCulture(msg, category);

            var message = this.GetMessage(text);
            Assert.IsNotNull(message, "Message should not be null");
            Assert.AreEqual<Guid>(Application.Identifier, message.Token.ApplicationId, "Application Id should match");
            Assert.AreEqual<DateTime>(DateTime.UtcNow.Date, message.OccurredOn.Date, "Occured On should match");
            Assert.AreEqual<string>(Environment.MachineName, message.MachineName, "Machine Name should match");
            Assert.AreEqual<string>(text, message.Message, "Message should match");
        }

        [TestMethod]
        public void WriteLineMessageCategoryInvalidMessage()
        {
            string msg = StringHelper.NullEmptyWhiteSpace();
            string category = Guid.NewGuid().ToString();
            Trace.WriteLine(msg, category);

            Trace.Flush();

            string text = "{0}: {1}".FormatWithCulture(category, msg ?? "null");

            var message = this.GetMessage(text);
            Assert.IsNotNull(message, "Message should not be null");
            Assert.AreEqual<Guid>(Application.Identifier, message.Token.ApplicationId, "Application Id should match");
            Assert.AreEqual<DateTime>(DateTime.UtcNow.Date, message.OccurredOn.Date, "Occured On should match");
            Assert.AreEqual<string>(Environment.MachineName, message.MachineName, "Machine Name should match");
            Assert.AreEqual<string>(text, message.Message, "Message should match");
        }

        [TestMethod]
        public void WriteLineMessageCategoryInvalidCategory()
        {
            string msg = Guid.NewGuid().ToString();
            string category = StringHelper.NullEmptyWhiteSpace();
            Trace.WriteLine(msg, category);

            Trace.Flush();

            string text = "{1}: {0}".FormatWithCulture(msg, category ?? "null");

            var message = this.GetMessage(text);
            Assert.IsNotNull(message, "Message should not be null");
            Assert.AreEqual<Guid>(Application.Identifier, message.Token.ApplicationId, "Application Id should match");
            Assert.AreEqual<DateTime>(DateTime.UtcNow.Date, message.OccurredOn.Date, "Occured On should match");
            Assert.AreEqual<string>(Environment.MachineName, message.MachineName, "Machine Name should match");
            Assert.AreEqual<string>(text, message.Message, "Message should match");
        }

        [TestMethod]
        public void WriteLineMessageCategoryInvalidMessageAndCategory()
        {
            this.CleanUp();

            var now = DateTime.UtcNow;

            Trace.WriteLine(StringHelper.NullEmptyWhiteSpace(), StringHelper.NullEmptyWhiteSpace());

            Trace.Flush();

            var source = new LogCore();
            var query = new Abc.Services.Contracts.LogQuery()
            {
                ApplicationIdentifier = Application.Identifier,
                From = now,
            };
            var messages = source.SelectMessages(query);
            Assert.IsNotNull(messages, "Messages should not be null");
            Assert.AreEqual<int>(0, messages.Count());
        }

        [TestMethod]
        public void WriteLineObjectCategory()
        {
            object obj = Guid.NewGuid();
            string category = Guid.NewGuid().ToString();
            Trace.WriteLine(obj, category);

            Trace.Flush();

            string text = "{1}: {0}".FormatWithCulture(obj, category);

            var message = this.GetMessage(text);
            Assert.IsNotNull(message, "Message should not be null");
            Assert.AreEqual<Guid>(Application.Identifier, message.Token.ApplicationId, "Application Id should match");
            Assert.AreEqual<DateTime>(DateTime.UtcNow.Date, message.OccurredOn.Date, "Occured On should match");
            Assert.AreEqual<string>(Environment.MachineName, message.MachineName, "Machine Name should match");
            Assert.AreEqual<string>(text, message.Message, "Message should match");
        }

        [TestMethod]
        public void WriteLineObjectCategoryNullObject()
        {
            object obj = null;
            string category = Guid.NewGuid().ToString();
            Trace.WriteLine(obj, category);

            Trace.Flush();

            string text = "{0}: null".FormatWithCulture(category);

            var message = this.GetMessage(text);
            Assert.IsNotNull(message, "Message should not be null");
            Assert.AreEqual<Guid>(Application.Identifier, message.Token.ApplicationId, "Application Id should match");
            Assert.AreEqual<DateTime>(DateTime.UtcNow.Date, message.OccurredOn.Date, "Occured On should match");
            Assert.AreEqual<string>(Environment.MachineName, message.MachineName, "Machine Name should match");
            Assert.AreEqual<string>(text, message.Message, "Message should match");
        }

        [TestMethod]
        public void WriteLineObjectCategoryInvalidCategory()
        {
            object obj = Guid.NewGuid();
            string category = StringHelper.NullEmptyWhiteSpace();
            Trace.WriteLine(obj, category);

            Trace.Flush();

            string text = "{1}: {0}".FormatWithCulture(obj, category ?? "null");

            var message = this.GetMessage(text);
            Assert.IsNotNull(message, "Message should not be null");
            Assert.AreEqual<Guid>(Application.Identifier, message.Token.ApplicationId, "Application Id should match");
            Assert.AreEqual<DateTime>(DateTime.UtcNow.Date, message.OccurredOn.Date, "Occured On should match");
            Assert.AreEqual<string>(Environment.MachineName, message.MachineName, "Machine Name should match");
            Assert.AreEqual<string>(text, message.Message, "Message should match");
        }

        [TestMethod]
        public void WriteLineObjectCategoryNullObjectInvalidCategory()
        {
            this.CleanUp();

            var now = DateTime.UtcNow;

            Trace.WriteLine((object)null, StringHelper.NullEmptyWhiteSpace());

            Trace.Flush();

            var source = new LogCore();
            var query = new Abc.Services.Contracts.LogQuery()
            {
                ApplicationIdentifier = Application.Identifier,
                From = now,
            };
            var messages = source.SelectMessages(query);
            Assert.IsNotNull(messages, "Messages should not be null");
            Assert.AreEqual<int>(0, messages.Count());
        }
        #endregion

        #region Helper Methods
        public Abc.Services.Contracts.Message GetMessage(string text)
        {
            var source = new LogCore();
            Abc.Services.Contracts.Message message = null;
            int i = 0;
            while (i < 100 && null == message)
            {
                var query = new Abc.Services.Contracts.LogQuery()
                {
                    ApplicationIdentifier = Application.Identifier,
                };
                var messages = source.SelectMessages(query);
                if (null != messages)
                {
                    message = (from data in messages
                               where text == data.Message
                               select data).FirstOrDefault();
                }

                i++;
                Thread.Sleep(75);
            }

            return message;
        }

        public void CleanUp()
        {
            var source = new LogCore();
            var token = new Abc.Services.Contracts.Token()
            {
                ApplicationId = Settings.ApplicationIdentifier,
            };
            var message = new Abc.Services.Contracts.Message()
            {
                Token = token,
                OccurredOn = DateTime.UtcNow,
            };
            source.Delete(message);
        }
        #endregion
    }
}