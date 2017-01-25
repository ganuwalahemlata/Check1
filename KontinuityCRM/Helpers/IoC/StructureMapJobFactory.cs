using Quartz;
using Quartz.Spi;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace KontinuityCRM.Helpers.IoC
{
    /// <summary>
    /// Structure Map Job Factory
    /// </summary>
    public class StructureMapJobFactory : IJobFactory
    {
        private readonly IContainer _container
            ;

            //= new Container();
        /// <summary>
        /// Parameter less Contructor
        /// </summary>
        static StructureMapJobFactory()
        {
        }
        /// <summary>
        /// Constructor with container
        /// </summary>
        /// <param name="container"></param>
        public StructureMapJobFactory(IContainer container)
        {
            _container = container;
        }
        /// <summary>
        /// New job
        /// </summary>
        /// <param name="bundle">TriggerFireBundle</param>
        /// <param name="scheduler">Scedular</param>
        /// <returns></returns>
        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            var jobDetail = bundle.JobDetail;

            try
            {
                return _container.GetInstance(jobDetail.JobType) as IJob;
            }
            catch (Exception ex)
            {
                throw new SchedulerException(string.Format(
                    CultureInfo.InvariantCulture,
                    "Cannot instantiate class '{0}'", new object[] { jobDetail.JobType.FullName }), ex);
            }

           
        }

        /// <summary>
        /// Return Job
        /// </summary>
        /// <param name="job">job</param>
        public void ReturnJob(IJob job)
        {
            // to realize the context / job
            // custom dispose the job

            //@https://github.com/quartznet/quartznet/issues/65


        }
    }
}