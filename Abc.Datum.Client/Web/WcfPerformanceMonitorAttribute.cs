// <copyright from='2011' to='2011' company='Agile Business Cloud Solutions Ltd.' file='WcfPerformanceMonitorAttribute.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Web
{
    using System;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.ServiceModel.Dispatcher;

    /// <summary>
    /// WCF Performance Monitor
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1019:DefineAccessorsForAttributeArguments", Justification = "Not meant to be set at runtime.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1018:MarkAttributesWithAttributeUsage", Justification = "Let developers do what they do.")]
    public sealed class WcfPerformanceMonitorAttribute : Attribute, IOperationBehavior
    {
        #region Members
        /// <summary>
        /// Class Name
        /// </summary>
        private readonly Type callingClass;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the WcfPerformanceMonitorAttribute class
        /// </summary>
        /// <param name="callingClass">Calling Class</param>
        public WcfPerformanceMonitorAttribute(Type callingClass)
        {
            this.callingClass = callingClass;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Add Binding Parameters
        /// </summary>
        /// <param name="operationDescription">Operation Description</param>
        /// <param name="bindingParameters">Binding Parameters</param>
        public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
        {
        }

        /// <summary>
        /// Apply Client Behavior
        /// </summary>
        /// <param name="operationDescription">Operation Description</param>
        /// <param name="clientOperation">Client Operation</param>
        public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
        {
        }

        /// <summary>
        /// Apply Dispatch Behavior
        /// </summary>
        /// <param name="operationDescription">Operation Description</param>
        /// <param name="dispatchOperation">Dispatch Operation</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Not validating")]
        public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {
            if (null != dispatchOperation)
            {
                dispatchOperation.ParameterInspectors.Add(new WcfPerformanceMonitor(this.callingClass));
            }
        }

        /// <summary>
        /// Validate
        /// </summary>
        /// <param name="operationDescription">Operation Description</param>
        public void Validate(OperationDescription operationDescription)
        {
        }
        #endregion
    }
}