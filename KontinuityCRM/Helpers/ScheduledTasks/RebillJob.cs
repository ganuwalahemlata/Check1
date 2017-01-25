using System;
using System.Collections.Generic;
using KontinuityCRM.Models;
using Quartz;
using System.Linq;
using AutoMapper;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net.Mail;
using System.Web.Security;

namespace KontinuityCRM.Helpers.ScheduledTasks
{
    [DisallowConcurrentExecution]
    [PersistJobDataAfterExecution]
    public class RebillJob : IJob
    {
        private readonly IUnitOfWork uow;
        private readonly IWebSecurityWrapper wsw;
        private readonly IMappingEngine mapper;
        INLogger _logger = new NInLogger();

        public RebillJob(IUnitOfWork uow, IWebSecurityWrapper wsw, IMappingEngine mapper)
        {
            this.uow = uow;
            this.mapper = mapper;
        }
        /// <summary>
        /// RebillJob Execution Process for all the orders that have recurring set to true
        /// </summary>
        /// <param name="context"></param>
        public async void Execute(IJobExecutionContext context)
        {
            try
            {
                var timeUtc = DateTime.UtcNow;
                TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                DateTime easternTime = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, easternZone);

                _logger.LogInfo("Rebill job execution start. UTC Time :" + DateTimeOffset.UtcNow);

                context.JobDetail.JobDataMap["PreviousFireTime"] = DateTimeOffset.UtcNow;
                IList<Models.Order> orders = uow.OrderRepository.Get(o => !o.IsTest && o.OrderProducts.Where(p => p.Recurring == true).Any(p => p.NextDate < easternTime)).ToList();
                Task[] tasks = new Task[orders.Count()];
                foreach (var order in orders)
                {
                    await order.Rebill(uow, wsw, mapper);
                }

                _logger.LogInfo("Rebill job execution ended. UTC Time :" + DateTimeOffset.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.LogInfo("Rebill job failed. Exception :" + ex.Message);

                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();

                var roleProvider = wsw.RoleProvider;
                var userNames = roleProvider.GetUsersInRole("Admin");
                var devuserNames = roleProvider.GetUsersInRole("Technical");
                var users = uow.UserProfileRepository.Get(u => userNames.Contains(u.UserName));
                var to = "";

                MailAddressCollection addressCollection = null;

                // Admin
                if (users.Any())
                {
                    to = users.First().Email;

                    addressCollection = new MailAddressCollection();
                    foreach (var user in users.Skip(1))
                    {
                        addressCollection.Add(user.Email);
                    }
                }

                // Dev group
                if (devuserNames.Any())
                {
                    to = users.First().Email;

                    addressCollection = new MailAddressCollection();
                    foreach (var user in users.Skip(1))
                    {
                        addressCollection.Add(user.Email);
                    }
                }

                // Send email of exception of rebill job
                KontinuityCRMHelper.SendMail("noreply@continuitycrm.com", to, "ContinuityCRM Rebill Job Exception",
                     string.Format("Message: {0}\r\nLine: {1}\r\nStackTrace: {2}", ex.Message, line.ToString(), ex.StackTrace, addressCollection));
            }
         
        }
    }
}