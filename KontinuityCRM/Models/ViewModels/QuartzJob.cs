using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KontinuityCRM.Models.ViewModels
{
    /// <summary>
    /// Custome Job Class for Job Schedular
    /// </summary>
    public class QuartzJob
    {
        /// <summary>
        /// Indicates the JobName
        /// </summary>
        public string JobName { get; set; }
        /// <summary>
        /// Indicates the JobGroup to which Job Belongs
        /// </summary>
        public string JobGroup { get; set; }
        /// <summary>
        /// Indicates date for the nextFireTime
        /// </summary>
        public DateTimeOffset? NextFireTime { get; set; }
        /// <summary>
        /// Indicates date for the previousFireTime
        /// </summary>
        public DateTimeOffset? PreviousFireTime { get; set; }
        /// <summary>
        /// Indicates if the PreviousFireTime Persists or not
        /// </summary>
        public string PreviousFireTimePersist { get; set; }
    }
}