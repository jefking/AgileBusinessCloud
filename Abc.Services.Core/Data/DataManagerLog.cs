namespace Abc.Services.Data
{
    using Abc.Azure;
    using Microsoft.WindowsAzure.StorageClient;
    using System;

    [AzureDataStore("DataManagerLog")]
    [CLSCompliant(false)]
    public class DataManagerLog : TableServiceEntity
    {
        #region Constructors
        /// <summary>
        /// Default Constructor
        /// </summary>
        public DataManagerLog()
        {
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public DataManagerLog(Type caller)
        {
            if (null == caller)
            {
                throw new ArgumentNullException("caller");
            }
            else
            {
                var startDate = DateTime.UtcNow;
                this.PartitionKey = string.Format("{0}{1}{2}", caller, startDate.Year, startDate.Month);

                this.RowKey = Guid.NewGuid().ToString();
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets and sets Start Time
        /// </summary>
        public DateTime StartTime
        {
            get;
            set;
        }

        /// <summary>
        /// Gets and sets Completion Time
        /// </summary>
        public DateTime? CompletionTime
        {
            get;
            set;
        }

        /// <summary>
        /// Gets and sets Successful
        /// </summary>
        public bool Successful
        {
            get;
            set;
        }
        #endregion
    }
}