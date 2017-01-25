using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KontinuityCRM.Helpers.ScheduledTasks
{

    public interface IQuartzScheduler
    {
        /// <summary>
        /// Start Job 
        /// </summary>
        void Start();
        /// <summary>
        /// Stop Job
        /// </summary>
        void Stop();
    }
}
