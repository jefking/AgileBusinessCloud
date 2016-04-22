// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='ContentSourceTest.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Test.Suite
{
    using System;
    using Abc.Services;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure.StorageClient;

    /// <summary>
    /// Content Management System Source Test
    /// </summary>
    [TestClass]
    public class ContentSourceTest
    {
        #region Members
        /// <summary>
        /// Datum Data Source
        /// </summary>
        private ContentSource source;
        #endregion

        #region Initialize
        /// <summary>
        /// Initialize Test
        /// </summary>
        [TestInitialize]
        public void Init()
        {
            source = new ContentSource();
        }
        #endregion

        #region Error Cases
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InsertNullTextData()
        {
            source.Insert((TextData)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InsertNullBinaryContent()
        {
            source.InsertBinary((byte[])null, StringHelper.ValidString());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void InsertInvalidContentType()
        {
            Random random = new Random();
            var bytes = new byte[128];
            random.NextBytes(bytes);
            source.InsertBinary(bytes, StringHelper.NullEmptyWhiteSpace());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SelectBinaryContentEmptyGuid()
        {
            source.SelectBinary(Guid.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(StorageClientException))]
        public void SelectUnkownBinaryContent()
        {
            source.SelectBinary(Guid.NewGuid());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InsertEmptyIdTextData()
        {
            source.Insert(new TextData());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UpdateNullTextData()
        {
            source.Update((TextData)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UpdateEmptyIdTextData()
        {
            source.Update(new TextData());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SelectEmptyIdTextData()
        {
            source.SelectText(Guid.NewGuid(), Guid.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SelectEmptyApplicationIdTextData()
        {
            source.SelectText(Guid.Empty, Guid.NewGuid());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InsertNullXmlData()
        {
            source.Insert((XmlData)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InsertEmptyIdXmlData()
        {
            source.Insert(new XmlData());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UpdateNullXmlData()
        {
            source.Update((XmlData)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UpdateEmptyIdXmlData()
        {
            source.Update(new XmlData());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SelectEmptyIdXmlData()
        {
            source.SelectXml(Guid.NewGuid(), Guid.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SelectEmptyApplicationIdXmlData()
        {
            source.SelectXml(Guid.Empty, Guid.NewGuid());
        }
        #endregion

        #region Valid Cases
        [TestMethod]
        public void RoundTripTextContent()
        {
            Guid appId = Guid.NewGuid();
            TextData data = new TextData(appId)
            {
                Active = true,
                ContentId = Guid.NewGuid(),
                CreatedOn = DateTime.UtcNow,
                Deleted = false,
                UpdatedOn = DateTime.UtcNow
            };

            Guid id = source.Insert(data);
            Assert.AreNotEqual<Guid>(Guid.Empty, id);
            Assert.AreEqual<Guid>(data.Id, id);

            TextData getData = source.SelectText(appId, id);
            Assert.IsNotNull(getData);
            Assert.AreEqual<Guid>(data.Id, getData.Id, "Data Mismatch");
            Assert.AreEqual<Guid>(data.ApplicationId, getData.ApplicationId, "Data Mismatch");
            Assert.AreEqual<bool>(data.Active, getData.Active, "Data Mismatch");
            Assert.AreEqual<Guid>(data.ContentId, getData.ContentId, "Data Mismatch");
            Assert.AreEqual<DateTime>(data.CreatedOn.Date, getData.CreatedOn.Date, "Data Mismatch");
            Assert.AreEqual<bool>(data.Deleted, getData.Deleted, "Data Mismatch");
            Assert.AreEqual<DateTime>(data.UpdatedOn.Date, getData.UpdatedOn.Date, "Data Mismatch");

            getData.Active = false;
            getData.ContentId = Guid.NewGuid();
            getData.CreatedOn = DateTime.UtcNow;
            getData.Deleted = true;
            getData.UpdatedOn = DateTime.UtcNow;

            source.Update(getData);

            TextData finalData = source.SelectText(appId, id);
            Assert.IsNotNull(finalData);
            Assert.AreEqual<Guid>(getData.Id, finalData.Id, "Data Mismatch");
            Assert.AreEqual<Guid>(getData.ApplicationId, finalData.ApplicationId, "Data Mismatch");
            Assert.AreEqual<bool>(getData.Active, finalData.Active, "Data Mismatch");
            Assert.AreEqual<Guid>(getData.ContentId, finalData.ContentId, "Data Mismatch");
            Assert.AreEqual<DateTime>(getData.CreatedOn.Date, finalData.CreatedOn.Date, "Data Mismatch");
            Assert.AreEqual<bool>(getData.Deleted, finalData.Deleted, "Data Mismatch");
            Assert.AreEqual<DateTime>(getData.UpdatedOn.Date, finalData.UpdatedOn.Date, "Data Mismatch");
        }

        [TestMethod]
        public void RoundTripText()
        {
            string originalText = Guid.NewGuid().ToString();
            Guid id = source.InsertText(originalText);
            Assert.AreNotEqual<Guid>(Guid.Empty, id);

            var getData = source.SelectText(id);
            Assert.IsNotNull(getData);
            Assert.AreEqual<string>(originalText, getData, "Data Mismatch");

            string update = Guid.NewGuid().ToString();

            source.UpdateText(id, update);

            var finalData = source.SelectText(id);
            Assert.IsNotNull(finalData);
            Assert.AreEqual<string>(update, finalData, "Data Mismatch");
        }

        [TestMethod]
        public void RoundTripXmlContent()
        {
            Guid appId = Guid.NewGuid();
            XmlData data = new XmlData(appId)
            {
                Active = true,
                ContentId = Guid.NewGuid(),
                CreatedOn = DateTime.UtcNow,
                Deleted = false,
                UpdatedOn = DateTime.UtcNow
            };

            Guid id = source.Insert(data);
            Assert.AreNotEqual<Guid>(Guid.Empty, id);
            Assert.AreEqual<Guid>(data.Id, id);

            XmlData getData = source.SelectXml(appId, id);
            Assert.IsNotNull(getData);
            Assert.AreEqual<Guid>(data.Id, getData.Id, "Data Mismatch");
            Assert.AreEqual<Guid>(data.ApplicationId, getData.ApplicationId, "Data Mismatch");
            Assert.AreEqual<bool>(data.Active, getData.Active, "Data Mismatch");
            Assert.AreEqual<Guid>(data.ContentId, getData.ContentId, "Data Mismatch");
            Assert.AreEqual<DateTime>(data.CreatedOn.Date, getData.CreatedOn.Date, "Data Mismatch");
            Assert.AreEqual<bool>(data.Deleted, getData.Deleted, "Data Mismatch");
            Assert.AreEqual<DateTime>(data.UpdatedOn.Date, getData.UpdatedOn.Date, "Data Mismatch");

            getData.Active = false;
            getData.ContentId = Guid.NewGuid();
            getData.CreatedOn = DateTime.UtcNow;
            getData.Deleted = true;
            getData.UpdatedOn = DateTime.UtcNow;

            source.Update(getData);

            XmlData finalData = source.SelectXml(appId, id);
            Assert.IsNotNull(finalData);
            Assert.AreEqual<Guid>(getData.Id, finalData.Id, "Data Mismatch");
            Assert.AreEqual<Guid>(getData.ApplicationId, finalData.ApplicationId, "Data Mismatch");
            Assert.AreEqual<bool>(getData.Active, finalData.Active, "Data Mismatch");
            Assert.AreEqual<Guid>(getData.ContentId, finalData.ContentId, "Data Mismatch");
            Assert.AreEqual<DateTime>(getData.CreatedOn.Date, finalData.CreatedOn.Date, "Data Mismatch");
            Assert.AreEqual<bool>(getData.Deleted, finalData.Deleted, "Data Mismatch");
            Assert.AreEqual<DateTime>(getData.UpdatedOn.Date, finalData.UpdatedOn.Date, "Data Mismatch");
        }

        [TestMethod]
        public void RoundTripXml()
        {
            const string XmlFormat = "<xml>{0}</xml>";
            string original = string.Format(XmlFormat, Guid.NewGuid());
            Guid id = source.InsertXml(original);
            Assert.AreNotEqual<Guid>(Guid.Empty, id);

            var returned = source.SelectXml(id);
            Assert.IsNotNull(returned);
            Assert.AreEqual<string>(original, returned, "Data Mismatch");

            string update = string.Format(XmlFormat, Guid.NewGuid());

            source.UpdateXml(id, update);

            var finalData = source.SelectXml(id);
            Assert.IsNotNull(finalData);
            Assert.AreEqual<string>(update, finalData, "Data Mismatch");
        }

        [TestMethod]
        public void RoundTripBinaryContent()
        {
            byte[] data = new byte[512];
            Random random = new Random();
            random.NextBytes(data);
            
            Guid id = this.source.InsertBinary(data, "na");
            Assert.AreNotEqual<Guid>(Guid.Empty, id, "Identifier not set.");

            byte[] returnedData = this.source.SelectBinary(id);
            Assert.IsNotNull(returnedData);
            Assert.AreEqual<int>(data.Length, returnedData.Length, "Data is inconsistant.");

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] != returnedData[i])
                {
                    Assert.Fail("Data is inconsistant.");
                }
            }
        }
        #endregion
    }
}
