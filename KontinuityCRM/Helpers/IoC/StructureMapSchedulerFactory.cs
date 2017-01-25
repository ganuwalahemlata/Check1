using Quartz;
using Quartz.Core;
using Quartz.Impl;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KontinuityCRM.Helpers.IoC
{
    /// <summary>
    /// Structure Map Schedule Factory
    /// </summary>
    public class StructureMapSchedulerFactory : StdSchedulerFactory
    {
        /// <summary>
        /// structure Map job factory
        /// </summary>
        private readonly StructureMapJobFactory jobFactory;
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="jobFactory"></param>
        public StructureMapSchedulerFactory(StructureMapJobFactory jobFactory)
        {
            this.jobFactory = jobFactory;
        }
        /// <summary>
        /// Instantiate
        /// </summary>
        /// <param name="rsrcs">Quartz Schedular Resources</param>
        /// <param name="qs">Quartz Schedular</param>
        /// <returns></returns>
        protected override IScheduler Instantiate(QuartzSchedulerResources rsrcs, QuartzScheduler qs)
        {
            qs.JobFactory = this.jobFactory;
            return base.Instantiate(rsrcs, qs);
        }
    }
}