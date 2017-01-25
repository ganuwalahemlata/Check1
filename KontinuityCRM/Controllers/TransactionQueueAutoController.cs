using KontinuityCRM.Models;
using KontinuityCRM.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Expressions;
using KontinuityCRM.Filters;
using KontinuityCRM.Models.Gateways;
using System.Collections.Specialized;
using AuthorizeNet;
using System.Reflection.Emit;
using AutoMapper;
using Quartz;
using Quartz.Impl.Matchers;

namespace KontinuityCRM.Controllers
{
    [CustomAuthorizeAttribute]
    [System.ComponentModel.DisplayName("Prepaid Cards")]
    public class TransactionQueueAutoController : BaseController
    {

        private readonly IMappingEngine mapper;
        private readonly IScheduler scheduler;

        public TransactionQueueAutoController(IUnitOfWork uow, IWebSecurityWrapper wsw, IMappingEngine mapper, IScheduler scheduler)
             : base(uow, wsw)
        {
            this.mapper = mapper;
            this.scheduler = scheduler;
        }
        public ActionResult Index(int? id, string sortOrder, string currentFilter, string searchString, int? page, int? display, string decline, FormCollection collection, string processDecline, string[] checkedItem, string processDeclineSelected)
        {
            //try
            //{
            var user = uow.UserProfileRepository.Find(wsw.CurrentUserId);
            int pageSize = user.PrepaidCardDisplay;

            if (display.HasValue && display != pageSize)
            {
                page = 1;
                pageSize = display.Value;
                user.PrepaidCardDisplay = pageSize;
                uow.UserProfileRepository.Update(user);
                uow.Save(wsw.CurrentUserName);
            }
            if (searchString != null)
                page = 1;
            else
                searchString = currentFilter;

            ViewBag.CurrentSort = sortOrder;
            ViewBag.IdSortParm = String.IsNullOrEmpty(sortOrder) ? "id_asc" : "";
            ViewBag.FirstNameSortParm = sortOrder == "firstname" ? "firstname_desc" : "firstname";
            ViewBag.LastNameSortParm = sortOrder == "lastname" ? "lastname_desc" : "lastname";
            ViewBag.lastnameOrderIcon = "sort";
            ViewBag.firstnameOrderIcon = "sort";

            ViewBag.idOrderIcon = "sort";

            Func<IQueryable<TransactionQueueMaster>, IOrderedQueryable<TransactionQueueMaster>> orderBy = o =>
            {
                switch (sortOrder)
                {
                    case "id_asc":
                        ViewBag.idOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Id);

                    //case "firstname":
                    //    ViewBag.nameOrderIcon = "sorting_asc";
                    //    return o.OrderBy(c => c.First_Name).ThenByDescending(c => c.Id);
                    //case "firstname_desc":
                    //    ViewBag.nameOrderIcon = "sorting_desc";
                    //    return o.OrderByDescending(c => c.First_Name).ThenByDescending(c => c.Id);

                    //case "lastname":
                    //    ViewBag.nameOrderIcon = "sorting_asc";
                    //    return o.OrderBy(c => c.Last_Name).ThenByDescending(c => c.Id);
                    //case "lastname_desc":
                    //    ViewBag.nameOrderIcon = "sorting_desc";
                    //    return o.OrderByDescending(c => c.Last_Name).ThenByDescending(c => c.Id);


                    default:
                        ViewBag.idOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Id);
                }
            };

            ViewBag.CurrentFilter = searchString;
            Expression<Func<TransactionQueueMaster, bool>> filter = f => true;

            if (!String.IsNullOrEmpty(searchString))
            {
                filter = f => f.Processor.Name.Contains(searchString)

                    || f.Amount.ToString().Contains(searchString)

                    //|| f.Address2.Contains(searchString)
                    //|| f.Country.Contains(searchString)
                    //|| f.IPAddress.Contains(searchString)
                    //|| f.Type.Contains(searchString)
                    //|| f.Phone.Contains(searchString)
                    ;
            }


            Expression<Func<TransactionQueueMaster, bool>> filter2 = f => !id.HasValue || id == f.Id;

            var lambda = filter2.AndAlso(filter);
            ViewBag.Display = pageSize;

            int pageNumber = (page ?? 1);

            var model_new = uow.TransactionQueueMasterRepository
                .GetPage(pageSize, pageNumber,
                //out count,
                filter: lambda,
                orderBy: orderBy
                );

            var pageList = new PagedListMapper<TransactionQueueMaster>(model_new, model_new.CloneProps().ConvertUtcTime(TimeZoneUtility.GetTimeZone(uow.TimeZoneRepository.Find(user.TimeZoneId).Name)));
            return View(pageList);
        }

        private void CreateViewBag()
        {
            ViewBag.Providers = uow.ProcessorRepository.Get().ToList();
            List<SelectListItem> ObjListProcessor = new List<SelectListItem>();
            foreach (var item in uow.ProcessorRepository.Get().ToList())
            {
                SelectListItem listitem = new SelectListItem { Text = item.Name, Value = item.Id.ToString() };
                ObjListProcessor.Add(listitem);
            }
            //Assigning generic list to ViewBag
            ViewBag.Processors = ObjListProcessor;



            SelectList ObjListCardType = new SelectList(
       new List<SelectListItem>
       {
            new SelectListItem { Text = "VISA", Value = "VISA"},
            new SelectListItem { Text = "AMEX", Value = "AMEX"},
             new SelectListItem { Text = "DISCOVER", Value = "DISCOVER"},
              new SelectListItem { Text = "MASTERCARD", Value = "MASTERCARD"},
       }, "Value", "Text");

            //Assigning generic list to ViewBag
            ViewBag.CardType = ObjListCardType;

        }


        public ActionResult Create()
        {

            CreateViewBag();

            return View();
        }


        [HttpPost]
        public ActionResult Create(TransactionQueueMaster trans)
        {
            if (ModelState.IsValid)
            {
                // Add min and max time in master table

                trans.RemainingTransactions = trans.NoOfTransactions;
                trans.Date = DateTime.UtcNow;
                uow.TransactionQueueMasterRepository.Add(trans);
                uow.Save();
                for (int i = 0; i < trans.NoOfTransactions;)
                {
                    var prepaidCards = uow.PrepaidCardRepository.Get().Where(a => a.Stop == false && a.declined == false && a.PaymentType == trans.CardType).ToList();
                    if (prepaidCards.Count != 0)
                    {
                        foreach (var cards in prepaidCards)
                        {
                            if (i < trans.NoOfTransactions)
                            {
                                // Add min, max time and attampt column
                                TransactionQueue _queue = new TransactionQueue();
                                _queue.Amount = trans.Amount;
                                _queue.PrepaidCardId = cards.Id;
                                _queue.ProcessorId = trans.ProcessorId;
                                _queue.finished = false;
                                _queue.Date = DateTime.UtcNow;
                                _queue.TimeIntervalMin = ConvertSecondsToMiliseconds(trans.TimeIntervalMin);
                                _queue.TimeIntervalMax = ConvertSecondsToMiliseconds(trans.TimeIntervalMax);
                                _queue.Attempt = 0;
                                _queue.TransactionQueMasterId = uow.TransactionQueueMasterRepository.Get().OrderByDescending(a => a.Id).FirstOrDefault().Id;
                                decimal transactionSumForCard = uow.TransactionQueueRepository.Get().Where(a => a.PrepaidCardId == cards.Id && a.finished==false).ToList().Sum(a => a.Amount);
                                if (Convert.ToDecimal(cards.RemainingAmount) >= (transactionSumForCard + trans.Amount))
                                {
                                    uow.TransactionQueueRepository.Add(_queue);
                                    uow.Save();
                                    i++;
                                }
                            }
                        }
                    }
                    else
                    {
                        break;
                    }

                }

                ModelState.Clear();

                CreateViewBag();


                //var jobKey = scheduler
                //.GetJobKeys(GroupMatcher<JobKey>.AnyGroup())
                //.FirstOrDefault(k => k.Name == "transactionJob");

                //if (jobKey != null)
                //{
                //    // Create listener ??

                //    var jobdetail = scheduler.GetJobDetail(jobKey);
                //    jobdetail.JobDataMap["PreviousFireTime"] = DateTimeOffset.UtcNow; // still doesn't work it need we need to refresh the page to see the changes

                //    scheduler.TriggerJob(jobKey);
                //}


                return View();
            }
            else
            {
                CreateViewBag();
                return View(trans);
            }
        }






        public ActionResult Edit(int id)
        {
            var email = uow.PrepaidCardRepository.Find(id); //repo.FindPrepaidCard(id);
            if (email == null)
            {
                return HttpNotFound();
            }
            return View(email);
        }

        [HttpPost]
        public ActionResult Edit(PrepaidCardTransactionQueue prepaidCardData)
        {
            if (ModelState.IsValid)
            {
                prepaidCardData.Date = DateTime.UtcNow;
                //email.UpdatedUserId = wsw.CurrentUserId;

                uow.PrepaidCardTransactionQueueRepository.Update(prepaidCardData);
                uow.Save(wsw.CurrentUserName);
                return RedirectToAction("Index");
            }
            return View(prepaidCardData);
        }

        public ActionResult Delete(int id)
        {
            var email = uow.PrepaidCardRepository.Find(id);
            if (email == null)
            {
                return HttpNotFound();
            }

            uow.PrepaidCardRepository.Delete(email);
            uow.Save(wsw.CurrentUserName);
            return RedirectToAction("Index");
        }
        /// <summary>
        /// Convert Seconds To Miliseconds.
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static int ConvertSecondsToMiliseconds(int seconds)
        {
            return Convert.ToInt32(TimeSpan.FromSeconds(seconds).TotalMilliseconds);
        }
    }
}
