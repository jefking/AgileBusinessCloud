// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='ObjectType.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services.Graphics
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Object Type
    /// </summary>
    [DataContract]
    public enum ObjectType
    {
        /// <summary>
        /// Unknown
        /// </summary>
        [EnumMember]
        Unknown = 0,

        /// <summary>
        /// Round Table
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "RoundTable", Justification = "Name of object.")]
        [EnumMember]
        RoundTable = 1,

        /// <summary>
        /// Quadrilateral Table
        /// </summary>
        [EnumMember]
        QuadrilateralTable = 2,

        /// <summary>
        /// Chair
        /// </summary>
        [EnumMember]
        Chair = 3,

        /// <summary>
        /// Wall
        /// </summary>
        [EnumMember]
        Wall = 4,

        /// <summary>
        /// Door
        /// </summary>
        [EnumMember]
        Door = 5,

        /// <summary>
        /// Window
        /// </summary>
        [EnumMember]
        Window = 6,
    }
}