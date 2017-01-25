using System;
using System.Collections.Generic;
using KontinuityCRM.Models;
using Quartz;
using System.Linq;
using AutoMapper;
using System.Threading.Tasks;
using KontinuityCRM.Models.Enums;

namespace KontinuityCRM.Helpers.ScheduledTasks
{
    [DisallowConcurrentExecution]
    [PersistJobDataAfterExecution]
    public class ReattemptJob : IJob
    {
        private readonly IUnitOfWork uow;
        private readonly IWebSecurityWrapper wsw;
        private readonly IMappingEngine mapper;

        public ReattemptJob(IUnitOfWork uow, IWebSecurityWrapper wsw, IMappingEngine mapper)
        {
            this.uow = uow;
            this.wsw = wsw;
            this.mapper = mapper;
        }
        /// <summary>
        /// ReattemptJob Execution Process for Decline order reattempt.
        /// </summary>
        /// <param name="context"></param>
        public async void Execute(IJobExecutionContext context)
        {
            var dataMap = context.MergedJobDataMap;
            var oid = (string)dataMap["myKey"];
            var dprocessors = (int?)dataMap["myKey2"];
            var discount = (decimal)dataMap["myKey3"];
            IEnumerable<int> oidList = oid.Split(',').Select(s => int.Parse(s));

            Task[] tasks = new Task[oidList.Count()];
            foreach (var orderid in oidList)
            {
                #region Reattempt (Process order again) call just on Declined new orders (not rebill orders) Manual reattempt

                var order = uow.OrderRepository.Find(orderid);

                //if (order != null && ((!order.IsRebill && order.Status == OrderStatus.Declined && !order.HasRebills) || order.Status == OrderStatus.Unpaid))
                //{

                //    // set the recurring of all products to true to update the nextdate
                foreach (var op in order.OrderProducts)
                {
                    if (op.Recurring == false)
                    {
                        uow.OrderNoteRepository.Add(new OrderNote
                        {
                            OrderId = op.OrderId,
                            NoteDate = DateTime.UtcNow,
                            Note = "Start Recurring",
                        });
                        
                    }
                    op.Recurring = true; // will this be updated ? it not => update every op in another loop 
                    op.RebillDiscount = op.Price!= null && op.Price != 0 ? op.Price.Value * discount / 100 : 0;

                   
                }
                uow.Save();

                var retryProcessor = uow.ProcessorRepository.Find(dprocessors);
                await order.Rebill(uow, wsw, mapper, retryProcessor);

                #endregion
            }
        }
    }
}