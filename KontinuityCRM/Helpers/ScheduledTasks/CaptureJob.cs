using AutoMapper;
using KontinuityCRM.Models;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KontinuityCRM.Helpers.ScheduledTasks
{
    [PersistJobDataAfterExecution]
    public class CaptureJob : IJob
    {
        private readonly IUnitOfWork uow;
        private readonly IMappingEngine mapper;

        public CaptureJob(IUnitOfWork uow, IMappingEngine mapper)
        {
            this.uow = uow;
            this.mapper = mapper;
        }

        //public CaptureJob() : this(new UnitOfWork(), Mapper.Engine) { }

        /// <summary>
        /// Execute Capture Job
        /// </summary>
        /// <param name="context">job execution context</param>
        public void Execute(IJobExecutionContext context)
        {

            context.JobDetail.JobDataMap["PreviousFireTime"] = DateTimeOffset.UtcNow;
            // get all schedule capture and make the transaction
            var authOrders = uow.OrderRepository.Get(o => o.CaptureDate < DateTime.Now); // false if null

            if (authOrders.Any())
            {
                foreach (var order in authOrders)
                {
                    var captureTransaction = order.Capture(uow, mapper);

                    /* if success and capture on shipment */
                    if (captureTransaction.Success && captureTransaction.Processor.ShipmentOnCapture)
                    {
                        var task = order.Fulfill(mapper);

                        var result = task.Result;
                        //task.RunSynchronously();

                        //task.Start();
                        //task.Wait();
                    }
                        
                    uow.OrderRepository.Update(order); // will this save all references ? notes, transactions, orderproducts ?? YES
                }

                uow.Save();
            }
            
            //context.Trigger.JobDataMap.Add("PreviousFireTime", DateTimeOffset.UtcNow);

        }
    }
}