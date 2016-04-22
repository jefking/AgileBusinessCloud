﻿// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='ServerStatisticsRow.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using Abc.Azure;
    using Abc.Services.Contracts;

    /// <summary>
    /// Server Statistics Row
    /// </summary>
    [AzureDataStore("ServerStatistics")]
    [CLSCompliant(false)]
    public class ServerStatisticsRow : ApplicationData, IConvert<ServerStatisticSetDisplay>
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the ServerStatisticsRow class
        /// </summary>
        public ServerStatisticsRow()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ServerStatisticsRow class
        /// </summary>
        /// <param name="applicationId">Application Identifier</param>
        public ServerStatisticsRow(Guid applicationId)
            : base(applicationId)
        {
            Contract.Requires(Guid.Empty != applicationId);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets Occured On
        /// </summary>
        public DateTime OccurredOn
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Machine Name
        /// </summary>
        public string MachineName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Instance Id
        /// </summary>
        public string DeploymentId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the CPU Usage Percentage
        /// </summary>
        public double CpuUsagePercentage
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Physical Disk Usage Percentage
        /// </summary>
        public double PhysicalDiskUsagePercentage
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Memory Usage Percentage
        /// </summary>
        public double MemoryUsagePercentage
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Network Percentage 1
        /// </summary>
        public double? NetworkPercentage1
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Network Percentage 2
        /// </summary>
        public double? NetworkPercentage2
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Network Percentage 3
        /// </summary>
        public double? NetworkPercentage3
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Network Percentage 4
        /// </summary>
        public double? NetworkPercentage4
        {
            get;
            set;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Convert Server Statistic Set
        /// </summary>
        /// <returns>Server Statistic Set</returns>
        public ServerStatisticSetDisplay Convert()
        {
            var token = new Token()
            {
                ApplicationId = this.ApplicationId,
            };

            var display = new ServerStatisticSetDisplay()
            {
                CpuUsagePercentage = (float)this.CpuUsagePercentage,
                DeploymentId = this.DeploymentId,
                MachineName = this.MachineName,
                MemoryUsagePercentage = (float)this.MemoryUsagePercentage,
                OccurredOn = this.OccurredOn,
                PhysicalDiskUsagePercentage = (float)this.PhysicalDiskUsagePercentage,
                Token = token,
                Identifier = Guid.Parse(this.RowKey),
            };

            if (null != this.NetworkPercentage1 || null != this.NetworkPercentage2 || null != this.NetworkPercentage3 || null != this.NetworkPercentage4)
            {
                var network = new List<float>(4);
                if (null != this.NetworkPercentage1)
                {
                    network.Add((float)this.NetworkPercentage1);
                }
                if (null != this.NetworkPercentage2)
                {
                    network.Add((float)this.NetworkPercentage2);
                }
                if (null != this.NetworkPercentage3)
                {
                    network.Add((float)this.NetworkPercentage3);
                }
                if (null != this.NetworkPercentage4)
                {
                    network.Add((float)this.NetworkPercentage4);
                }

                display.NetworkPercentages = network.ToArray();
            }

            return display;
        }
        #endregion
    }
}