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
    public class FulfillmentJob : IJob
    {
        private readonly IUnitOfWork uow;
                   // = new UnitOfWork();
        private readonly IMappingEngine mapper;

        public FulfillmentJob(
            IUnitOfWork uow,
            IMappingEngine mapper)
        {
            this.uow = uow;
            this.mapper = mapper;
        }

        //public FulfillmetJob() : this(new UnitOfWork(), Mapper.Engine) { }
        /// <summary>
        /// Execute Fulfillment Job
        /// </summary>
        /// <param name="context">Provided context</param>
        public void Execute(IJobExecutionContext context)
        {
            context.JobDetail.JobDataMap["PreviousFireTime"] = DateTimeOffset.UtcNow;

            var orders = uow.OrderRepository
                .Get(o => //o.Shipped == false && // this condition is for speed up? this query
                    o.OrderProducts.Any(p => /*p.FulfillmentDate.HasValue &&*/ p.FulfillmentDate < DateTime.Now));

            if (orders.Any())
            {
                foreach (var order in orders)
                {
                    //var ftask = order.Fulfill(mapper).ContinueWith(task =>
                    //{
                    //    var success = task.Result;

                    //    if (success)
                    //    {
                    //        // get the transaction Auth transaction. It was sucessfully because otherwise we were not here
                    //        var authtransaction = order.Transactions
                    //            .FirstOrDefault(t => t.Type == Models.TransactionType.Auth && t.Success && t.Processor.CaptureOnShipment);

                    //        // if there is an auth transaction
                    //        if (authtransaction != null)
                    //        {
                    //            var captureTransaction = order.Capture(uow, authtransaction);
                    //        }
                    //    }
                    //});

                    //ftask.Start(); // don't work
                    //ftask.Wait();

                    // ftask.RunSynchronously(); // don't work

                    var ftask = order.Fulfill(mapper);
                    
                    var success = ftask.Result;

                    if (success)
                    {
                        // dont take into account the capture delay

                        // get the transaction Auth transaction. It was sucessfully because otherwise we were not here
                        var authtransaction = order.Transactions
                            .FirstOrDefault(t => t.Type == Models.TransactionType.Auth && t.Success && t.Processor.CaptureOnShipment);

                        // if there is an auth transaction
                        if (authtransaction != null)
                        {
                            var captureTransaction = order.Capture(uow, authtransaction, mapper);
                        }
                    }

                    uow.OrderRepository.Update(order); // will this update all references ? orderproducts, trasactions, notes, ?? YES
                }
                uow.Save(); // no currentusername 'system'
            }

            
            
            //context.Trigger.JobDataMap.Add("PreviousFireTime", DateTimeOffset.UtcNow);
        }
    }
}