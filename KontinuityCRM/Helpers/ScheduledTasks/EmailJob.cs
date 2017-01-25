using System;
using System.Collections.Generic;
using KontinuityCRM.Models;
using Quartz;
using System.Linq;
using AutoMapper;
using System.Threading.Tasks;

namespace KontinuityCRM.Helpers.ScheduledTasks
{
    [DisallowConcurrentExecution]
    [PersistJobDataAfterExecution]
    public class EmailJob : IJob
    {
        private readonly IUnitOfWork uow;
        private readonly IWebSecurityWrapper wsw;
        private readonly IMappingEngine mapper;

        public EmailJob(IUnitOfWork uow, IWebSecurityWrapper wsw, IMappingEngine mapper)
        {
            this.uow = uow;
            this.wsw = wsw;
            this.mapper = mapper;
        }
        /// <summary>
        /// RebillJob Execution Process for all the orders that have recurring set to true
        /// </summary>
        /// <param name="context"></param>
        public async void Execute(IJobExecutionContext context)
        {
            context.JobDetail.JobDataMap["PreviousFireTime"] = DateTimeOffset.UtcNow;
            var orders = uow.TriggerEmailTableRepository.Get();
            foreach (var order in orders)
            {
                if (order.sent != true)
                {
                    var eventdetails = uow.EventRepository.Get().Where(a => a.Id == order.EventId).FirstOrDefault();
                    if (eventdetails.NoOfDays != null && eventdetails.TemplateId != 0)
                    {
                        int noofdays = Convert.ToInt32(eventdetails.NoOfDays);
                        if (DateTime.Compare(DateTime.UtcNow.Date, Convert.ToDateTime(order.TriggerTime).AddDays(noofdays).Date) == 0)
                        {
                            EmailHelper.SendSecondEmailTemplate(uow, mapper, order.OrderId, order.EventId);
                           
                            var modelEmail = uow.TriggerEmailTableRepository.Get().Where(a => a.Id == order.Id).FirstOrDefault();
                            modelEmail.sent = true;
                            uow.TriggerEmailTableRepository.Update(modelEmail);
                            uow.Save();
                        }
                    }
                }
            }
        }
    }
}