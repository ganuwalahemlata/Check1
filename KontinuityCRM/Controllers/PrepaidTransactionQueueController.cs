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

namespace KontinuityCRM.Controllers
{
    [CustomAuthorizeAttribute]
    [System.ComponentModel.DisplayName("Prepaid Cards")]
    public class PrepaidTransactionQueueController : BaseController
    {

        private readonly IMappingEngine mapper;

        public PrepaidTransactionQueueController(IUnitOfWork uow, IWebSecurityWrapper wsw, IMappingEngine mapper)
             : base(uow, wsw)
        {
            this.mapper = mapper;
        }
        public ActionResult Index(int? id, string sortOrder, string currentFilter, string searchString, int? page, int? display, string decline, FormCollection collection, string processDecline, string[] checkedItem, string processDeclineSelected)
        {
            //try
            //{
            var user = uow.UserProfileRepository.Find(wsw.CurrentUserId);
            int pageSize = user.PrepaidCardDisplay;
            int j = pageSize * (Convert.ToInt32(page) - 1);
            if (page == null)
            {
                j = 0;
            }
            if (processDeclineSelected != null)
            {
                if (checkedItem != null)
                {
                    var modelPrepaidTransaction = uow.TransactionViaPrepaidCardQueueRepository.Get().Where(a => a.Success == false).OrderByDescending(a => a.Id).ToList();
                    for (int i = 0; i < checkedItem.Length; i++)
                    {
                        if (checkedItem[i] == "true")
                        {
                            int TransactionId = modelPrepaidTransaction[j - 1].Id;
                            ExecuteSelectedFailedTransactions(TransactionId);
                            i++;
                        }
                        j++;
                    }
                }
                ViewBag.Decline = "checked";
                return RedirectToAction("Index");
            }

            if (processDecline != null)
            {
                ExecuteAllFailedTransactions();
                ViewBag.Decline = "checked";
                return RedirectToAction("Index");
            }

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

            Func<IQueryable<TransactionViaPrepaidCardQueue>, IOrderedQueryable<TransactionViaPrepaidCardQueue>> orderBy = o =>
            {
                switch (sortOrder)
                {
                    case "id_asc":
                        ViewBag.idOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Id);
                    default:
                        ViewBag.idOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Id);
                }
            };

            ViewBag.CurrentFilter = searchString;
            Expression<Func<TransactionViaPrepaidCardQueue, bool>> filter = f => true;
            Expression<Func<TransactionViaPrepaidCardQueue, bool>> filterForSuccessDecline = f => true;

            if (!String.IsNullOrEmpty(searchString))
            {
                filter = f => f.Processor.Name.Contains(searchString)
                    || f.PrepaidCard.Number.Contains(searchString)
                    || f.Amount.ToString().Contains(searchString)
                    ;
            }
            
            if (Session["Decline"] != null && page != 0)
            {
                if (decline == null)
                {
                    decline = Session["Decline"].ToString();
                }
            }

            if (decline == "1")
            {
                filterForSuccessDecline = f => f.Success == false;
                ViewBag.Decline = "checked";
                ViewBag.Success = "";
                Session["Decline"] = 1;
            }
            else if (decline == "2")
            {
                filterForSuccessDecline = f => f.Success == true;
                ViewBag.Success = "checked";
                ViewBag.Decline = "";
                Session["Decline"] = 2;
            }

            Expression<Func<TransactionViaPrepaidCardQueue, bool>> filter2 = f => !id.HasValue || id == f.Id;

            var lambda = filter2.AndAlso(filter).AndAlso(filterForSuccessDecline);
            ViewBag.Display = pageSize;

            int pageNumber = (page ?? 1);

            var model_new = uow.TransactionViaPrepaidCardQueueRepository
                .GetPage(pageSize, pageNumber,
                //out count,
                filter: lambda,
                orderBy: orderBy
                );

            var pageList = new PagedListMapper<TransactionViaPrepaidCardQueue>(model_new, model_new.CloneProps().ConvertUtcTime(TimeZoneUtility.GetTimeZone(uow.TimeZoneRepository.Find(user.TimeZoneId).Name)));
            return View(pageList);
        }

        private void ExecuteAllFailedTransactions()
        {
            var modelPrepaidTransaction = uow.TransactionViaPrepaidCardQueueRepository.Get().Where(a => a.Success == false).ToList();
            foreach (var itemForTransaction in modelPrepaidTransaction)
            {
                TransactionSingle(itemForTransaction.Processor, itemForTransaction.Amount, itemForTransaction.PrepaidCard, itemForTransaction.Id, mapper);
            }
        }

        private void ExecuteSelectedFailedTransactions(int id)
        {
            var modelPrepaidTransaction = uow.TransactionViaPrepaidCardQueueRepository.Get().Where(a => a.Success == false && a.Id == id).FirstOrDefault();
            // modelPrepaidTransaction[count].Id;
            //foreach (var itemForTransaction in modelPrepaidTransaction)
            //{
            TransactionSingle(modelPrepaidTransaction.Processor, modelPrepaidTransaction.Amount, modelPrepaidTransaction.PrepaidCard, modelPrepaidTransaction.Id, mapper);
            //}
        }


        private void CreateViewBag()
        {
            ViewBag.Providers = uow.ProcessorRepository.Get().ToList();
            List<SelectListItem> ObjList = new List<SelectListItem>();
            foreach (var item in uow.ProcessorRepository.Get().ToList())
            {
                SelectListItem listitem = new SelectListItem { Text = item.Name, Value = item.Name };
                ObjList.Add(listitem);
            }
            //Assigning generic list to ViewBag
            ViewBag.Processors = ObjList;
        }

        public ActionResult Create()
        {

            CreateViewBag();

            return View();
        }



        [HttpPost]
        public ActionResult Create(PrepaidCardTransactionQueue prepaidCardData)
        {
            prepaidCardData.Date = DateTime.UtcNow;
            int countT = 0;
            string processorname = prepaidCardData.ProcessorID;
            var processorDetails = uow.ProcessorRepository.Get().Where(a => a.Name == processorname).FirstOrDefault();

            if (ModelState.IsValid)
            {
                int no_of_transactions = Convert.ToInt32(prepaidCardData.no_of_transactions);
                for (int i = 0; i < no_of_transactions;)
                {
                    decimal amount = Convert.ToDecimal(prepaidCardData.amount);

                    i = MerchantTransaction(no_of_transactions, processorDetails, amount, i, mapper);

                    var prepaidCardsNew = uow.PrepaidCardRepository.Get().ToList().Where(a => a.Stop == false && a.declined == false);
                    if (prepaidCardsNew.Count() == 0)
                    {
                        ViewBag.ProcessMsg = "All Cards Declined";
                        break;
                    }
                }
            }
            //email.CreatedUserId = wsw.CurrentUserId;

            uow.PrepaidCardTransactionQueueRepository.Add(prepaidCardData);
            uow.Save(wsw.CurrentUserName);
            //  return RedirectToAction("Index");
            ModelState.Clear();

            CreateViewBag();

            return View();
        }

        private int MerchantTransaction(int no_of_transactions, Processor processorDetails, decimal amount, int i, IMappingEngine mapper)
        {
            var prepaidCards = uow.PrepaidCardRepository.Get().Where(a => a.Stop == false && a.declined == false).ToList();
            foreach (var item in prepaidCards)
            {
                if (i < no_of_transactions)
                {
                    GatewayModel gatewayModel = null;
                    gatewayModel = processorDetails.GatewayModel(mapper);
                    TransactionViaPrepaidCardQueue tran = gatewayModel.SalePrepaidCard(item, processorDetails, amount);
                    if (tran.Success == true)
                    {
                        item.Date = DateTime.UtcNow;
                        item.RemainingAmount = (Convert.ToDecimal(item.RemainingAmount) - Convert.ToDecimal(amount)).ToString();
                        if (Convert.ToDecimal(item.RemainingAmount) == 0)
                        {
                            item.Stop = true;
                        }
                        uow.PrepaidCardRepository.Update(item);
                        uow.Save(wsw.CurrentUserName);
                        i++;
                    }
                    if (tran.Status == Models.TransactionStatus.Declined)
                    {
                        item.Date = DateTime.UtcNow;
                        item.declined = true;
                        uow.PrepaidCardRepository.Update(item);
                        uow.Save(wsw.CurrentUserName);
                    }
                    uow.TransactionViaPrepaidCardQueueRepository.Add(tran);
                    uow.Save();
                }
            }
            return i;
        }
        //  private readonly IMappingEngine mapper;

        private void TransactionSingle(Processor processorDetails, decimal amount, PrepaidCard item, int tranid, IMappingEngine mapper)
        {
            GatewayModel gatewayModel = null;
            gatewayModel = processorDetails.GatewayModel(mapper);
            TransactionViaPrepaidCardQueue tran = gatewayModel.SalePrepaidCard(item, processorDetails, amount);
            if (tran.Success == true)
            {
                item.Date = DateTime.UtcNow;
                item.RemainingAmount = (Convert.ToDecimal(item.RemainingAmount) - Convert.ToDecimal(amount)).ToString();
                if (Convert.ToDecimal(item.RemainingAmount) == 0)
                {
                    item.Stop = true;
                }
                uow.PrepaidCardRepository.Update(item);
                uow.Save(wsw.CurrentUserName);
                uow.TransactionViaPrepaidCardQueueRepository.Delete(tranid);
                uow.Save(wsw.CurrentUserName);
            }
            if (tran.Status == Models.TransactionStatus.Declined)
            {
                item.Date = DateTime.UtcNow;
                item.declined = true;
                uow.PrepaidCardRepository.Update(item);
                uow.Save(wsw.CurrentUserName);
            }
            uow.TransactionViaPrepaidCardQueueRepository.Add(tran);
            uow.Save();
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
    }
}
