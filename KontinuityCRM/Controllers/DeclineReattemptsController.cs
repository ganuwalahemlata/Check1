using AutoMapper;
using KontinuityCRM.Filters;
using KontinuityCRM.Helpers;
using KontinuityCRM.Helpers.ScheduledTasks;
using KontinuityCRM.Models;
using KontinuityCRM.Models.APIModels;
using KontinuityCRM.Models.Enums;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace KontinuityCRM.Controllers
{
    /// <summary>
    /// Decline Reattempts Controller.
    /// </summary>
    [CustomAuthorizeAttribute]
    [System.ComponentModel.DisplayName("DeclineReattempts")]
    public class DeclineReattemptsController : BaseController
    {
        private readonly IMappingEngine mapper;

        private readonly IScheduler Scheduler;

        public DeclineReattemptsController(IUnitOfWork uow, IWebSecurityWrapper wsw, IMappingEngine mapper, IScheduler scheduler)
            : base(uow, wsw)
        {
            this.mapper = mapper;
            this.Scheduler = scheduler;
        }

        /// <summary>
        /// GET: Decline Reattempts.
        /// </summary>
        /// <returns></returns>

        public async Task<ActionResult> Index(decimal? discount, string useraction, string SearchString, int? processors, string fOrderId, string oid, int? dprocessors,int? page, int? display)
        {
            var user = uow.UserProfileRepository.Find(wsw.CurrentUserId);
            ViewBag.Processors = uow.ProcessorRepository.Get();
            // remove empty search strings
            var route = GetRouteData();
            
            if (useraction =="reattempt")
            {
                ITrigger trigger = TriggerBuilder.Create().StartNow().Build();
                IJobDetail reattemptJob = new JobDetailImpl("reattemptJob", null, typeof(ReattemptJob));
                reattemptJob.JobDataMap.Put("myKey", oid);
                reattemptJob.JobDataMap.Put("myKey2", dprocessors);
                if (discount.HasValue)
                {
                    reattemptJob.JobDataMap.Put("myKey3", discount);
                }
                else
                {
                    reattemptJob.JobDataMap.Put("myKey3", 0);
                }
                this.Scheduler.ScheduleJob(reattemptJob, trigger);
                this.Scheduler.Start();
                route.Remove("UserAction");
                route.Remove("dprocessors");
                route.Remove("oid");
                route.Remove("discount");
                return RedirectToAction("index", route);
            }
            Func<IQueryable<Transaction>, IOrderedQueryable<Transaction>> orderBy = o =>
            {
                return o.OrderByDescending(c => c.Id);
            };

            var arrOrderIds = (fOrderId != null ? fOrderId.Split(',') : new string[] { "" });

            Expression<Func<Transaction, bool>> filter = f => 
            f.Type == TransactionType.Auth || f.Type == TransactionType.Sale
            && (processors == null || f.Processor.Id == processors)
            && (String.IsNullOrEmpty(fOrderId) || arrOrderIds.Contains(f.OrderId.ToString()))
             && (String.IsNullOrEmpty(SearchString) || f.Message.Contains(SearchString));
           
            int pageNumber = (page ?? 1);
            var model = uow.TransactionRepository
               .GetPage(10, pageNumber,
               //out count,
               filter: filter,
               orderBy: orderBy
               );


            var pageList = new PagedListMapper<Transaction>(model, model.CloneProps().ConvertUtcTime(TimeZoneUtility.GetTimeZone(uow.TimeZoneRepository.Find(user.TimeZoneId).Name)));
            return View(pageList);
        }

        /// <summary>
        /// Get Data from Route in Key, Value pairs
        /// </summary>
        /// <returns></returns>
        private RouteValueDictionary GetRouteData()
        {
            // this could also be done by working with the request url 
            // the way we do in the index[post] action
            // rebill the order and redirect to useraction detail 
            var route = new RouteValueDictionary(Request.RequestContext.RouteData.Values);
            //add the current querystring
            foreach (string key in Request.QueryString.Keys)
            {
                route[key] = Request.QueryString[key];
            }

            return route;
        }
    }
}