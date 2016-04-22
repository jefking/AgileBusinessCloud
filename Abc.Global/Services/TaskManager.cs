// <copyright from='2011' to='2012' company='Agile Business Cloud Solutions Ltd.' file='TaskManager.cs'>
// Copyright (c) Agile Business Cloud Solutions Ltd. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential.
// </copyright>
namespace Abc.Services
{
    using System;
    using System.Threading;

    /// <summary>
    /// Task Manager
    /// </summary>
    public abstract class TaskManager : IService, IDisposable
    {
        #region Members
        /// <summary>
        /// Timer
        /// </summary>
        private Timer timer = null;

        /// <summary>
        /// Due Time of Timer
        /// </summary>
        private readonly TimeSpan dueTime;

        /// <summary>
        /// Period of Timer
        /// </summary>
        private readonly TimeSpan period;

        /// <summary>
        /// Disposed
        /// </summary>
        private bool disposed = false;
        #endregion

        #region Constructors
        /// <summary>
        /// Default Constructor
        /// </summary>
        protected TaskManager(int dueInSeconds, int periodInSeconds)
        {
            this.dueTime = TimeSpan.FromSeconds(dueInSeconds);
            this.period = TimeSpan.FromSeconds(periodInSeconds);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Runs Service
        /// </summary>
        /// <returns>Running</returns>
        public bool Run()
        {
            this.timer = new Timer(this.Execute, null, dueTime, period);

            return true;
        }

        /// <summary>
        /// Stops Service
        /// </summary>
        /// <returns>Stopped</returns>
        public bool Stop()
        {
            if (null != this.timer)
            {
                this.timer.Dispose();
                this.timer = null;
            }

            return true;
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="state">State of Timer</param>
        protected virtual void Execute(object state)
        {
            try
            {
                this.Execute();
            }
            catch
            {
                //For Safety. We should be logging!
            }
        }

        /// <summary>
        /// Execute Action
        /// </summary>
        public abstract void Execute();

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing">Disposing</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (null != this.timer)
                {
                    this.timer.Dispose();
                }

                this.timer = null;
                this.disposed = true;
            }
        }
        #endregion
    }
}