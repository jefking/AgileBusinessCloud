// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='ServerStatisticSet.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Web.Script.Serialization;
    using Abc.Services.Data;
    using Abc.Services.Validation;

    /// <summary>
    /// Contact Group
    /// </summary>
    [Serializable]
    [DataContract]
    public class ServerStatisticSet : Secured, IConvert<ServerStatisticsRow>, IValidate<ServerStatisticSet>, IConvert<LatestServerStatisticsRow>
    {
        #region Properties
        /// <summary>
        /// Gets or sets Occurred On
        /// </summary>
        [DataMember]
        public DateTime OccurredOn
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Machine Name
        /// </summary>
        [DataMember]
        public string MachineName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Instance Id
        /// </summary>
        [DataMember]
        public string DeploymentId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the CPU Usage Percentage
        /// </summary>
        [DataMember]
        public float CpuUsagePercentage
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Physical Disk Usage Percentage
        /// </summary>
        [DataMember]
        public float PhysicalDiskUsagePercentage
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Memory Usage Percentage
        /// </summary>
        [DataMember]
        public float MemoryUsagePercentage
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Network Percentages
        /// </summary>
        [DataMember]
        public float[] NetworkPercentages
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the Validation Rules
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Abc.Services.Validation.Rule<Abc.Services.Contracts.Email>.#ctor(System.Func<T,System.Boolean>,System.String)", Justification = "Not a localization issue.")]
        [IgnoreDataMember]
        [ScriptIgnore]
        public IEnumerable<Rule<ServerStatisticSet>> Rules
        {
            get
            {
                return new Rule<ServerStatisticSet>[]
                {
                    new Rule<ServerStatisticSet>(s => s.CpuUsagePercentage >= 0 && 100 >= s.CpuUsagePercentage, "Invalid CPU percentage."),
                    new Rule<ServerStatisticSet>(s => s.PhysicalDiskUsagePercentage >= 0 && 100 >= s.PhysicalDiskUsagePercentage, "Invalid Disk percentage."),
                    new Rule<ServerStatisticSet>(s => s.MemoryUsagePercentage >= 0 && 100 >= s.MemoryUsagePercentage, "Invalid Memory percentage."),
                    new Rule<ServerStatisticSet>(s => s.NetworkPercentages == null || (s.NetworkPercentages.All(p => p >= 0) && s.NetworkPercentages.All(p => p <= 100)), "Invalid Network percentage."),
                };
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Convert to Server Statistics Row
        /// </summary>
        /// <returns>Row</returns>
        [CLSCompliant(false)]
        public ServerStatisticsRow Convert()
        {
            var row = new ServerStatisticsRow(this.Token.ApplicationId)
            {
                CpuUsagePercentage = this.CpuUsagePercentage,
                DeploymentId = this.DeploymentId,
                MachineName = this.MachineName,
                MemoryUsagePercentage = this.MemoryUsagePercentage,
                OccurredOn = this.OccurredOn,
                PhysicalDiskUsagePercentage = this.PhysicalDiskUsagePercentage,
            };

            if (null != this.NetworkPercentages)
            {
                int i = 0;
                foreach (var percentage in this.NetworkPercentages)
                {
                    switch (i)
                    {
                        case 0:
                            row.NetworkPercentage1 = this.NetworkPercentages[i];
                            break;
                        case 1:
                            row.NetworkPercentage2 = this.NetworkPercentages[i];
                            break;
                        case 2:
                            row.NetworkPercentage3 = this.NetworkPercentages[i];
                            break;
                        case 3:
                            row.NetworkPercentage4 = this.NetworkPercentages[i];
                            break;
                    }
                    i++;
                }
            }

            return row;
        }

        /// <summary>
        /// Convert to Latest Server Statistics Row
        /// </summary>
        /// <returns>Latest Server Statistics Row</returns>
        LatestServerStatisticsRow IConvert<LatestServerStatisticsRow>.Convert()
        {
            var row = new LatestServerStatisticsRow(this.Token.ApplicationId)
            {
                CpuUsagePercentage = this.CpuUsagePercentage,
                DeploymentId = this.DeploymentId,
                RowKey = this.MachineName,
                MemoryUsagePercentage = this.MemoryUsagePercentage,
                OccurredOn = this.OccurredOn,
                PhysicalDiskUsagePercentage = this.PhysicalDiskUsagePercentage,
            };

            if (null != this.NetworkPercentages)
            {
                int i = 0;
                foreach (var percentage in this.NetworkPercentages)
                {
                    switch (i)
                    {
                        case 0:
                            row.NetworkPercentage1 = this.NetworkPercentages[i];
                            break;
                        case 1:
                            row.NetworkPercentage2 = this.NetworkPercentages[i];
                            break;
                        case 2:
                            row.NetworkPercentage3 = this.NetworkPercentages[i];
                            break;
                        case 3:
                            row.NetworkPercentage4 = this.NetworkPercentages[i];
                            break;
                    }
                    i++;
                }
            }

            return row;
        }
        #endregion
    }
}