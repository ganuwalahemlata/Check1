using KontinuityCRM.Helpers;
using KontinuityCRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Text.RegularExpressions;
using AutoMapper;
using System.Linq.Expressions;
using KontinuityCRM.Models.Enums;
using Newtonsoft.Json;
using KontinuityCRM.Filters;
using Quartz.Util;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Diagnostics;

namespace KontinuityCRM.Controllers
{
    [CustomAuthorizeAttribute]
    [System.ComponentModel.DisplayName("Orders")]
    public class orderController : BaseController
    {
        private readonly IMappingEngine mapper;
        public orderController(IUnitOfWork uow, IWebSecurityWrapper wsw, IMappingEngine mapper)
            : base(uow, wsw)
        {
            this.mapper = mapper;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="os"></param>
        /// <param name="sortOrder"></param>
        /// <param name="page"></param>
        /// <param name="display"></param>
        /// <param name="pid"></param>
        /// <returns></returns>
        public async Task<ActionResult> Index(OrderSearch os, string sortOrder, int? page, int? display, int? pid = null)
        {
            var defaultFromDate = new DateTime(DateTime.Today.AddDays(-365).Year, 1, 1);
            var defaultToDate = DateTime.Today.AddDays(1);

            // remove empty search strings
            var route = GetRouteData();

            var redirect = false;
            var emptyFilters = route.Keys.Where(k => route[k] == null || string.IsNullOrEmpty(route[k].ToString()));


            if (emptyFilters.Any())
            {
                emptyFilters.ToList().ForEach(a => route.Remove(a));
                redirect = true;
            }
            if (os.fFromDate == defaultFromDate)
            {
                route.Remove("fFromDate");
                redirect = true;
            }
            if (os.fToDate == defaultToDate)
            {
                route.Remove("fToDate");
                redirect = true;
            }

            if (redirect)
            {
                return RedirectToAction("index", route);
            }

            IEnumerable<int> dt = route["oid"] == null ? null : route["oid"]
                    .ToString().Split(',').Select(s => int.Parse(s)); // os.oid;


            var hasdetails = dt != null && dt.Count() > 0;

            // if there is no orders then it could not be detail or batch
            if (!hasdetails && (os.UserAction != UserAction.filter))
            {
                route.Remove("UserAction");
                return RedirectToAction("Index", route);
            }

            var orderids = dt == null ? new int[] { } : from i in dt where i > 0 select i;

            #region delete

            // delete action 
            if (os.UserAction == UserAction.delete)
            {
                // delete the orders and display the current search
                foreach (var item in orderids)
                {
                    var order = uow.OrderRepository.Find(item);

                    if (order.Status != OrderStatus.Deleted)
                    {
                        order.Status = OrderStatus.Deleted;

                        var recurringProducts = order.OrderProducts.Where(o => o.NextDate != null);
                        if (recurringProducts.Any()) // if there is any subscription cancel
                        {
                            KontinuityCRMHelper.CancelSubscription(recurringProducts, uow, wsw);
                        }

                        order.Notes.Add(new OrderNote
                        {
                            Note = "Order Deleted",
                            NoteDate = DateTime.UtcNow,
                        });

                        uow.OrderRepository.Update(order); // no problem 
                    }
                }

                uow.Save(wsw.CurrentUserName); // will this update the products

                route.Remove("UserAction"); // filer is the default action
                route.Remove("oid");
                route["fStatus"] = OrderStatus.Deleted;

                return RedirectToAction("index", route);
            }

            #endregion

            #region Reattempt (Process order again) call just on Declined new orders (not rebill orders) Manual reattempt

            if (os.UserAction == UserAction.reattempt)
            {
                // check if this is allowed

                var _oid = orderids.First();

                var order = uow.OrderRepository.Find(_oid);

                if (order != null && ((!order.IsRebill && order.Status == OrderStatus.Declined && !order.HasRebills) || order.Status == OrderStatus.Unpaid))
                {

                    // set the recurring of all products to true to update the nextdate
                    foreach (var op in order.OrderProducts)
                    {
                        op.Recurring = true; // will this be updated ? it not => update every op in another loop 
                    }

                    // reattempt the order this will start the recurring system
                    await order.Process(uow, wsw, mapper);

                    //// updates the order and each product of course
                    //foreach (var op in order.OrderProducts)
                    //{
                    //    uow.OrderProductRepository.Update(op); // no problem!!
                    //}

                    uow.OrderRepository.Update(order); // no problem!! will this update the order products ??

                    foreach (var oid in order.OrderProducts.Select(p => p.OrderId).Distinct())
                    {
                        uow.OrderNoteRepository.Add(new OrderNote
                        {
                            OrderId = oid,
                            NoteDate = DateTime.UtcNow,
                            Note = "Start Recurring",
                        });
                    }

                    uow.Save(wsw.CurrentUserName);
                }

                //var route = GetRouteData();
                route["UserAction"] = UserAction.detail;
                return RedirectToAction("index", route);
            }
            #endregion

            #region Start Recurring

            // recurring
            if (os.UserAction == UserAction.start)
            {
                //var _oid = orderids.First();
                //var order = uow.OrderRepository.Find(_oid);

                // start recurring not declined orders
                var orderProducts = uow.OrderRepository
                    .Get(o => orderids.Contains(o.OrderId) && o.Status != OrderStatus.Declined && o.Status != OrderStatus.Deleted && !o.IsTest)
                    .SelectMany(o => o.OrderProducts.Where(p => (!pid.HasValue || p.ProductId == pid) && !p.ChildOrderId.HasValue && p.NextDate == null))
                    .ToList();

                if (orderProducts.Any())
                {
                    foreach (var op in orderProducts)
                    {
                        // set the recurring of all products to true to update the nextdate

                        op.Recurring = true; // will this be updated ? it not => update every op in another loop 

                        // if no subscription setting start recurring on itself
                        op.NextProductId = op.NextProductId ?? (op.Product.IsSubscription ? op.Product.RecurringProductId : op.ProductId);
                        //op.NextDate = DateTime.Today.AddDays(1);
                        op.NextDate = DateTime.Today;

                        uow.OrderProductRepository.Update(op);
                    }

                    foreach (var _oid in orderProducts.Select(p => p.OrderId).Distinct())
                    {
                        uow.OrderNoteRepository.Add(new OrderNote
                        {
                            OrderId = _oid,
                            NoteDate = DateTime.UtcNow,
                            Note = "Start Recurring",
                        });
                    }
                    uow.Save(wsw.CurrentUserName);
                }


                //var route = GetRouteData();
                //route["UserAction"] = UserAction.detail;
                //if (orderids.Count() > 1) // if was a batch rebill then go to filter
                route.Remove("UserAction");
                route.Remove("pid");
                route.Remove("oid");

                return RedirectToAction("index", route);
            }

            #endregion

            #region Stop Recurring

            // recurring
            if (os.UserAction == UserAction.stop)
            {
                //var _oid = orderids.First();
                //var order = uow.OrderRepository.Find(_oid);

                // start recurring not declined orders
                var orderProducts = uow.OrderRepository.Get(o => orderids.Contains(o.OrderId))
                            .SelectMany(o => o.OrderProducts.Where(p => (!pid.HasValue || p.ProductId == pid)
                                && p.NextDate != null)).ToList();

                if (orderProducts.Any())
                {
                    //foreach (var op in orderProducts)
                    //{
                    //    op.NextDate = null; // will this be enough?


                    //    // cancel subscription
                    //    uow.OrderTimeEventRepository.Add(new OrderTimeEvent
                    //    {
                    //        OrderId = op.OrderId,
                    //        ProductId = op.ProductId,
                    //        Time = DateTimeOffset.UtcNow,
                    //        Event = OrderEvent.Canceled,
                    //        Action = op.Order.ParentId.HasValue ? op.ReAttempts > 0 : (bool?)null,  // re
                    //    });

                    //    op.ReAttempts = 0;

                    //    uow.OrderProductRepository.Update(op);
                    //}

                    KontinuityCRMHelper.CancelSubscription(orderProducts, uow, wsw);

                }


                //var route = GetRouteData();
                //route["UserAction"] = UserAction.detail;
                //if (orderids.Count() > 1) // if was a batch rebill then go to filter
                route.Remove("UserAction");
                route.Remove("pid");
                route.Remove("oid");

                return RedirectToAction("index", route);
            }

            #endregion

            #region Export

            // recurring
            if (os.UserAction == UserAction.export)
            {
                //var _oid = orderids.First();
                //var order = uow.OrderRepository.Find(_oid);

                // start recurring not declined orders
                var orders = uow.OrderRepository.Get(o => orderids.Contains(o.OrderId) && !o.IsTest);
                var sb = new StringBuilder();
                var pifields = typeof(KontinuityCRM.Models.APIModels.OrderAPIModel).GetProperties();

                foreach (var pi in pifields)
                    sb.AppendFormat("{0},", pi.Name);

                sb.Length--;
                sb.AppendLine();

                foreach (var order in orders)
                {
                    var ordermap = mapper.Map<KontinuityCRM.Models.Order, KontinuityCRM.Models.APIModels.OrderAPIModel>(order);

                    foreach (var pi in pifields)
                    {
                        try
                        {
                            var objvalue = ordermap.GetType().GetProperty(pi.Name).GetValue(ordermap);
                            if (objvalue == null)
                            {
                                sb.Append(",");
                            }
                            else
                            {
                                if (pi.Name == "OrderProducts")
                                {
                                    var varsb = new StringBuilder();
                                    foreach (var op in order.OrderProducts)
                                    {
                                        varsb.AppendFormat("{0} ({1}),", op.Product.Name, op.ProductId);
                                    }
                                    varsb.Length--;
                                    sb.AppendFormat("{0},", varsb.ToString());
                                }
                                else
                                {
                                    sb.AppendFormat("{0},", objvalue.ToString());
                                }
                            }



                        }
                        catch (Exception ex)
                        {
                            var s = pi.Name;
                            throw ex;
                        }

                    }
                    sb.Length--;
                    sb.AppendLine();
                }

                return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", string.Format("export_CSV_at_{0}.csv", DateTime.Now.ToString("dd_MMM_yy_HH_mm_ss")));
            }

            #endregion

            #region Rebill (Force Rebill Now)

            // force rebill
            if (os.UserAction == UserAction.rebill)
            {
                // just rebill those orders that are recurring 
                var rebillorders = uow.OrderRepository.Get(o => !o.IsTest && orderids.Contains(o.OrderId) && o.OrderProducts.Any(p => p.NextDate != null));
                foreach (var order in rebillorders) //(var _oid in orderids)
                {
                    //var _oid = orderids.First();
                    //var order = uow.OrderRepository.Find(_oid);

                    // get the products to rebill (all the products or a specific product)
                    // if no product is given then rebill the entire order (all products)
                    //var rebillProducts = pid.HasValue ? order.OrderProducts.Where(o => o.ProductId == pid) : order.OrderProducts;

                    //// if we rebill an order then rebill all those products with an active recurring
                    //foreach (var orderProduct in rebillProducts.Where(o => o.NextDate != null)) //.ToList()
                    //{
                    //    orderProduct.NextDate = DateTime.Today;
                    //}

                    await order.Rebill(uow, wsw, mapper);
                }

                //route["UserAction"] = UserAction.detail;

                //if (orderids.Count() > 1) // if was a batch rebill then go to filter
                route.Remove("UserAction");

                return RedirectToAction("index", route);

            }

            #endregion

            #region void is never used

            /*
                 * Void (void)
        Transaction voids will cancel an existing sale or captured authorization. In addition, non-captured authorizations
        can be voided to prevent any future capture. Voids can only occur if the transaction has not been settled.
                 * 
                 Refund (refund)
        Transaction refunds will reverse a previously settled transaction. If the transaction has not been settled, it must
        be voided instead of refunded.
                 */

            // void
            if (os.UserAction == UserAction.Void)
            {
                // void the order 
                var _oid = orderids.First();
                var order = uow.OrderRepository.Find(_oid);

                var transaction = order.Transactions.Where(t =>
                    (t.Type == TransactionType.Sale || t.Type == TransactionType.Auth)
                    && t.Status == Models.TransactionStatus.Approved
                    && t.Success).FirstOrDefault(); // do  we need to mark this transaction as voided ???

                if (transaction != null)
                {
                    var voidTransaction = transaction.Processor.GatewayModel(mapper).Void(transaction); //transaction.Gateway.Void(transaction);
                    //voidTransaction.Date = DateTime.UtcNow;

                    if (voidTransaction.Success)//(voidTransaction.Status == Models.TransactionStatus.Approved)
                    {
                        // edit the sale / auth transaction and add this new transaction
                        // marl also the order as void
                        order.Status = OrderStatus.Void;


                        order.Notes.Add(new OrderNote
                        {
                            NoteDate = DateTime.UtcNow,
                            Note = "Order Void"
                        });

                        // update the transaction status
                        transaction.Status = Models.TransactionStatus.Void;
                        uow.TransactionRepository.Update(transaction);
                    }
                    else
                    {
                        order.Notes.Add(new OrderNote
                        {
                            NoteDate = DateTime.UtcNow,
                            Note = "Transaction Void decline: " + voidTransaction.Message,
                        });
                    }

                    uow.OrderRepository.Update(order); // no problem !!
                    uow.TransactionRepository.Add(voidTransaction);
                    uow.Save(wsw.CurrentUserName);
                }

                //var route = GetRouteData();
                route["UserAction"] = UserAction.detail;
                route["fStatus"] = OrderStatus.Void;
                return RedirectToAction("index", route);

            }
            #endregion

            // RMA # filter
            int? frmaOrderId = null;
            int? frmaProductId = null;
            if (!string.IsNullOrEmpty(os.fRMA))
            {
                if (Regex.IsMatch(os.fRMA, @"^\d+-\d+$"))
                {
                    string[] rma = string.IsNullOrEmpty(os.fRMA) ? null : os.fRMA.Split('-');
                    frmaOrderId = int.Parse(rma[0]);
                    frmaProductId = int.Parse(rma[1]);
                }
                else
                {
                    frmaOrderId = 0;
                }
            }

            ViewBag.CurrentSort = sortOrder;
            ViewBag.IdSortParm = String.IsNullOrEmpty(sortOrder) ? "id_asc" : "";
            ViewBag.NameSortParm = sortOrder == "name" ? "name_desc" : "name";
            ViewBag.DateSortParm = sortOrder == "date" ? "date_desc" : "date";
            ViewBag.StatusSortParm = sortOrder == "status" ? "status_desc" : "status";
            ViewBag.QtySortParm = sortOrder == "qty" ? "qty_desc" : "qty";
            ViewBag.EmailSortParm = sortOrder == "email" ? "email_desc" : "email";
            ViewBag.TotalSortParm = sortOrder == "total" ? "total_desc" : "total";
            ViewBag.ShippingMethodSortParm = sortOrder == "shippingmethod" ? "shippingmethod_desc" : "shippingmethod";

            ViewBag.nameOrderIcon = "sort";
            ViewBag.dateOrderIcon = "sort";
            ViewBag.qtyOrderIcon = "sort";
            ViewBag.totalOrderIcon = "sort";
            ViewBag.shippingmethodOrderIcon = "sort";
            ViewBag.countryOrderIcon = "sort";
            ViewBag.emailOrderIcon = "sort";
            ViewBag.idOrderIcon = "sort";
            ViewBag.statusOrderIcon = "sort";

            Func<IQueryable<Order>, IOrderedQueryable<Order>> orderBy = o =>
            {
                switch (sortOrder)
                {
                    case "id_asc":
                        ViewBag.idOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.OrderId);

                    case "qty":
                        ViewBag.qtyOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.OrderProducts.Sum(s => s.Quantity)).ThenByDescending(c => c.OrderId);
                    case "qty_desc":
                        ViewBag.qtyOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.OrderProducts.Sum(s => s.Quantity)).ThenByDescending(c => c.OrderId);

                    case "email":
                        ViewBag.emailOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Email).ThenByDescending(c => c.OrderId);
                    case "email_desc":
                        ViewBag.emailOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Email).ThenByDescending(c => c.OrderId);

                    case "name":
                        ViewBag.nameOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.BillingFirstName).ThenByDescending(c => c.OrderId);
                    case "name_desc":
                        ViewBag.nameOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.BillingFirstName).ThenByDescending(c => c.OrderId);
                    case "date":
                        ViewBag.dateOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Created).ThenByDescending(c => c.OrderId);
                    case "date_desc":
                        ViewBag.dateOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Created).ThenByDescending(c => c.OrderId);
                    case "total":
                        ViewBag.totalOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Total).ThenByDescending(c => c.OrderId);
                    case "total_desc":
                        ViewBag.totalOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Total).ThenByDescending(c => c.OrderId);

                    case "shippingmethod":
                        ViewBag.shippingmethodOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.ShippingMethod.Name).ThenByDescending(c => c.OrderId);
                    case "shippingmethod_desc":
                        ViewBag.shippingmethodOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.ShippingMethod.Name).ThenByDescending(c => c.OrderId);

                    case "status":
                        ViewBag.statusOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Status).ThenByDescending(c => c.OrderId);
                    case "status_desc":
                        ViewBag.statusOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Status).ThenByDescending(c => c.OrderId);

                    default:
                        ViewBag.idOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.OrderId);
                }
            };

            os.fFromDate = os.fFromDate ?? defaultFromDate;
            os.fToDate = os.fToDate ?? defaultToDate;
            //if (os.fCreditCardNumber != null)
            //{

            //    if (os.fCreditCardNumber != "")
            //    {
            //        if (os.fCreditCardNumber.Count() == 6)
            //        {
            //            os.fBIN = os.fCreditCardNumber;
            //           // os.fCreditCardNumber = "";
            //        }
            //        if (os.fCreditCardNumber.Count() == 4)
            //        {
            //            os.fLastFour = os.fCreditCardNumber;
            //            //os.fCreditCardNumber = "";
            //        }
            //    }
            //}
            //user timezone to utc
            TimeZoneInfo userTimezoneInfo = TimeZoneUtility.GetTimeZone(uow.TimeZoneRepository.Find(uow.UserProfileRepository.Find(wsw.CurrentUserId).TimeZoneId).Name);
            var ufdate = TimeZoneUtility.ToUtc((DateTime)os.fFromDate, userTimezoneInfo);
            var utdate = TimeZoneUtility.ToUtc((DateTime)os.fToDate, userTimezoneInfo);
            var arrOrderIds = (os.fOrderId != null ? os.fOrderId.Split(',') : new string[] { "" });
            Expression<Func<Order, bool>> filter = o =>
                    ufdate <= o.Created && o.Created <= utdate
                    && (((int)os.fStatus) == 0 || os.fStatus == OrderStatus.IsTest || o.Status == os.fStatus)
                    && (os.fStatus == OrderStatus.IsTest ? o.IsTest == true : true)
                    && (os.fOrderId == null || hasdetails || arrOrderIds.Contains(o.OrderId.ToString()))
                    && (os.fAffiliateId == null || o.AffiliateId == os.fAffiliateId)
                    && (!os.fCustomerId.HasValue || o.CustomerId == os.fCustomerId)
                    && (!os.fProductId.HasValue || o.OrderProducts.Select(p => p.ProductId).Contains(os.fProductId.Value))
                    && (os.fIP == null || o.IPAddress == os.fIP)
                    && (os.fShipped == null || o.Shipped == os.fShipped)
                    && (os.fAddress == null || o.Customer.Address1 == os.fAddress)
                    && (os.fAddress2 == null || o.Customer.Address2 == os.fAddress2)
                    && (os.fFirstname == null || o.Customer.FirstName == os.fFirstname)
                    && (os.fLastname == null || o.Customer.LastName == os.fLastname)
                    && (os.fSubId == null || o.SubId == os.fSubId)
                    && (os.fEmail == null || o.Customer.Email == os.fEmail)
                    && (os.fCity == null || o.Customer.City == os.fCity)
                    && (os.fZIP == null || o.Customer.PostalCode == os.fZIP)
                    && (os.fPhone == null || o.Customer.Phone == os.fPhone)
                    && (os.fState == null || o.Customer.Province == os.fState)
                    && (os.fCountry == null || o.ShippingCountry == os.fCountry)
                    && (os.fTransactionId == null || o.Transactions.Select(t => t.TransactionId).Contains(os.fTransactionId))
                    && (os.fBIN == null || o.BIN == os.fBIN)
                    //&& (os.fCreditCardNumber == null || o.CreditCardNumber.Contains(os.fCreditCardNumber))
                    && (os.fLastFour == null || o.LastFour == os.fLastFour)
                    && (!hasdetails || orderids.Contains(o.OrderId)) // show only the detailed orderss
                    && (!frmaOrderId.HasValue || o.OrderProducts.Any(p => p.OrderId == frmaOrderId && p.ProductId == frmaProductId && p.RMAReasonId.HasValue))
                   && (os.Recurring.ToLower() == "active" ? o.OrderProducts.Any(x=>x.Recurring==true) : os.Recurring.ToLower() == "notactive" ? o.OrderProducts.Any(x => x.Recurring == false ): o.OrderProducts.Any(x => x.Recurring==true || x.Recurring==false ))
                   ;

            

            var user = uow.UserProfileRepository.Find(wsw.CurrentUserId);
            int pageSize = user.OrderDisplay;
            if (display.HasValue && display != pageSize)
            {
                page = 1;
                pageSize = display.Value;


                user.OrderDisplay = pageSize;
                uow.UserProfileRepository.Update(user);
                uow.Save(wsw.CurrentUserName);

            }

            ViewBag.Display = pageSize;

            int pageNumber = (page ?? 1);

            var model = uow.OrderRepository
                .GetPage(pageSize, pageNumber,
                //out count,
                filter: filter,
                orderBy: orderBy
                );

            os.Orders = model;
            //    uow.OrderRepository.Get(o => 
            //    //repo.Orders().Where(o =>
            //        ufdate <= o.Created && o.Created <= utdate

            //        && (((int)os.fStatus) == 0 || o.Status == os.fStatus)
            //        && (!os.fOrderId.HasValue || hasdetails || o.OrderId == os.fOrderId)
            //        && (!os.fAffiliateId.HasValue || o.AffiliateId == os.fAffiliateId)
            //        && (!os.fCustomerId.HasValue || o.CustomerId == os.fCustomerId)
            //        && (!os.fProductId.HasValue || o.OrderProducts.Select(p => p.ProductId).Contains(os.fProductId.Value))
            //        && (os.fIP == null || o.IPAddress == os.fIP)
            //        && (os.fShipped == null || o.Shipped == os.fShipped)
            //        && (os.fAddress == null || o.Customer.Address1 == os.fAddress)
            //        && (os.fAddress2 == null || o.Customer.Address2 == os.fAddress2)
            //        && (os.fFirstname == null || o.Customer.FirstName == os.fFirstname)
            //        && (os.fLastname == null || o.Customer.LastName == os.fLastname)
            //        && (os.fSubId == null || o.SubId == os.fSubId)
            //        && (os.fEmail == null || o.Customer.Email == os.fEmail)
            //        && (os.fCity == null || o.Customer.City == os.fCity)
            //        && (os.fZIP == null || o.Customer.PostalCode == os.fZIP)
            //        && (os.fPhone == null || o.Customer.Phone == os.fPhone)
            //        && (os.fState == null || o.Customer.Province == os.fState)
            //        && (os.fCountry == null || o.ShippingCountry == os.fCountry)                   
            //        && (os.fTransactionId == null || o.Transactions.Select(t => t.TransactionId).Contains(os.fTransactionId))
            //        && (!hasdetails || orderids.Contains(o.OrderId)) // show only the detailed orders

            //        && (!frmaOrderId.HasValue || o.OrderProducts.Any(p => p.OrderId == frmaOrderId && p.ProductId == frmaProductId && p.RMAReasonId.HasValue))

            //        , orderBy: o => o.OrderByDescending(od => od.OrderId)); //.ToList();

            ////debug
            //var count = os.Orders.Count();

            // calculate amounts
            //foreach (var o in os.Orders)
            //{
            //    o.CalculateTotalAmount();
            //}

            if (os.UserAction == UserAction.detail || os.UserAction == UserAction.batch)
            {
                // fill some viewbag variables
                CreateViewBag();
            }
            else
            {
                ViewBag.Countries = uow.CountryRepository.Get(orderBy: c => c.OrderBy(t => t.Name));
                //repo.Countries().OrderBy(c => c.Name);
            }

            #region Fill ViewBag
             
            ViewBag.ShippingOptions = new List<SelectListItem>()
            {  
                new SelectListItem { Text = "Shipped", Value = "True" },
                new SelectListItem { Text = "Not shipped", Value = "False" },
            };
              
            #endregion
               
            os.Orders = new PagedListMapper<Order>(os.Orders, os.Orders.CloneProps().ConvertUtcTime(TimeZoneUtility.GetTimeZone(uow.TimeZoneRepository.Find(user.TimeZoneId).Name)));
            return View("index", os);
        }
           
        /// <summary>
        ///   
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(int? id, string sortOrder, string currentFilter, string searchString, int? page, int? display)
        {

            var user = uow.UserProfileRepository.Find(wsw.CurrentUserId);
            int pageSize = user.OrderDisplay;

            if (display.HasValue && display != pageSize)
            {
                page = 1;
                pageSize = display.Value;

                user.OrderDisplay = pageSize;

                uow.UserProfileRepository.Update(user);
                uow.Save(wsw.CurrentUserName);
            }
            if (searchString != null)
                page = 1;
            else
                searchString = currentFilter;

            ViewBag.CurrentSort = sortOrder;
            ViewBag.IdSortParm = String.IsNullOrEmpty(sortOrder) ? "id_asc" : "";
            ViewBag.AmountSortParm = sortOrder == "amount" ? "amount_desc" : "amount";
            ViewBag.TypeSortParm = sortOrder == "type" ? "type_desc" : "type";
            ViewBag.ProcessorSortParm = sortOrder == "processor" ? "processor_desc" : "processor";
            ViewBag.ResponseSortParm = sortOrder == "response" ? "response_desc" : "response";
            ViewBag.DateSortParm = sortOrder == "date" ? "date_desc" : "amount";
            ViewBag.StatusSortParm = sortOrder == "status" ? "type_desc" : "type";

            ViewBag.responseIcon = "sort";
            ViewBag.processorIcon = "sort";
            ViewBag.typeIcon = "sort";
            ViewBag.amountIcon = "sort";
            ViewBag.idOrderIcon = "sort";
            ViewBag.statusIcon = "sort";
            ViewBag.dateIcon = "sort";

            Func<IQueryable<Models.Transaction>, IOrderedQueryable<Models.Transaction>> orderBy = o =>
            {
                switch (sortOrder)
                {
                    case "id_asc":
                        ViewBag.idOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Id);

                    case "amount":
                        ViewBag.amountIcon = "sorting_asc";
                        return o.OrderBy(c => c.Amount).ThenByDescending(c => c.Id);
                    case "amount_desc":
                        ViewBag.amountIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Amount).ThenByDescending(c => c.Id);

                    case "type":
                        ViewBag.typeIcon = "sorting_asc";
                        return o.OrderBy(c => c.Type).ThenByDescending(c => c.Id);
                    case "type_desc":
                        ViewBag.typeIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Type).ThenByDescending(c => c.Id);

                    case "processor":
                        ViewBag.processorIcon = "sorting_asc";
                        return o.OrderBy(c => c.Processor.Name).ThenByDescending(c => c.Id);
                    case "processor_desc":
                        ViewBag.processorIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Processor.Name).ThenByDescending(c => c.Id);

                    case "response":
                        ViewBag.responseIcon = "sorting_asc";
                        return o.OrderBy(c => c.Response).ThenByDescending(c => c.Id);
                    case "response_desc":
                        ViewBag.responseIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Response).ThenByDescending(c => c.Id);

                 

                    default:
                        ViewBag.idOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Id);
                }
            };


            ViewBag.CurrentFilter = searchString;

            Expression<Func<Models.Transaction, bool>> filter = f => true;

            if (!String.IsNullOrEmpty(searchString))
            {
                filter = f =>  f.Type.ToString().Contains(searchString)
                    || f.Status.ToString().Contains(searchString)
                    || f.Amount.ToString().Contains(searchString)
                    || f.Processor.Name.ToString().Contains(searchString)
                    || f.Response.Contains(searchString)
                    ;
            }


            Expression<Func<Models.Transaction, bool>> filter2 = f => f.OrderId == id;

            var lambda = filter2.AndAlso(filter);
            
            ViewBag.Display = pageSize;

            int pageNumber = (page ?? 1);

            var model = uow.TransactionRepository
                .GetPage(10, pageNumber,
                //out count,
               filter: lambda,
                orderBy: orderBy
                );

            var pageList = new PagedListMapper<Models.Transaction>(model, model.CloneProps().ConvertUtcTime(TimeZoneUtility.GetTimeZone(uow.TimeZoneRepository.Find(user.TimeZoneId).Name)));
           
            var order = uow.OrderRepository.Find(id);
            order.TransactionsNew = pageList;
            CreateViewBag();
            return View(order);
        }
           
        /// <summary>
        /// Start recurring against orders
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pid"></param>
        /// <returns></returns>
        public ActionResult Start(int id, int? pid = null)
        {
            #region Start Recurring

            // recurring  

            // start recurring not declined orders
            var orderProducts = uow.OrderRepository
                .Get(o => id == o.OrderId && o.Status != OrderStatus.Declined && o.Status != OrderStatus.Fraud && o.Status != OrderStatus.Deleted && !o.IsTest)
                .SelectMany(o => o.OrderProducts.Where(p => (!pid.HasValue || p.ProductId == pid) && !p.ChildOrderId.HasValue && p.NextDate == null))
                .ToList();
            
            if (orderProducts.Any())
            {
                foreach (var op in orderProducts) 
                {
                    // set the recurring of all products to true to update the nextdate

                    op.Recurring = true; // will this be updated ? it not => update every op in another loop 

                    // if no subscription setting start recurring on itself
                    op.NextProductId = op.NextProductId ?? (op.Product.IsSubscription ? op.Product.RecurringProductId : op.ProductId);
                    //op.NextDate = DateTime.Today.AddDays(1);
                    op.NextDate = DateTime.Today; 
                       
                    if (!op.BillType.HasValue && op.Product.IsSubscription)
                    {  
                        op.BillType = op.Product.BillType;
                    }

                    op.BillValue = op.BillValue ?? (op.Product.IsSubscription ? op.Product.BillValue : null);

                    if (!op.BillType.HasValue || !op.BillValue.HasValue)
                    {
                        op.BillType = null;
                        op.BillValue = null;
                    }
                      
                    uow.OrderProductRepository.Update(op);

                    uow.OrderNoteRepository.Add(new OrderNote
                    {
                        OrderId = op.OrderId,
                        NoteDate = DateTime.UtcNow,
                        Note = "Start Recurring",
                    });
                }  
                  
               uow.Save(wsw.CurrentUserName);
            }
              
            //var route = GetRouteData();
            //route["UserAction"] = UserAction.detail;
            //if (orderids.Count() > 1) // if was a batch rebill then go to filter
            //route.Remove("UserAction");
            //route.Remove("pid");
            //route.Remove("oid");

            return RedirectToAction("details", new { id = id });


            #endregion
        }

        /// <summary>
        /// Stop Recurring against orders
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pid"></param>
        /// <returns></returns>
        public ActionResult Stop(int id, int? pid = null)
        {
            #region Stop Recurring
            //var _oid = orderids.First();
            //var order = uow.OrderRepository.Find(_oid);

            // start recurring not declined orders
            var orderProducts = uow.OrderRepository.Get(o => id == o.OrderId)
                        .SelectMany(o => o.OrderProducts.Where(p => (!pid.HasValue || p.ProductId == pid)
                            && p.NextDate != null)).ToList();

            if (orderProducts.Any())
            {
                KontinuityCRMHelper.CancelSubscription(orderProducts, uow, wsw);
            }
            return RedirectToAction("details", new { id = id });
            #endregion
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pid"></param>
        /// <returns></returns>
        public async Task<ActionResult> Rebill(int id, int? pid = null)
        {
            #region Rebill (Force Rebill Now)

            var order = uow.OrderRepository.Find(id);

            if (!order.IsTest && order.OrderProducts.Any(p => p.NextDate != null))
            {
                var rebillProducts = pid.HasValue ? order.OrderProducts.Where(o => o.ProductId == pid) : order.OrderProducts;

                // if we rebill an order then rebill all those products with an active recurring
                foreach (var orderProduct in rebillProducts.Where(o => o.NextDate != null)) //.ToList()
                {
                    orderProduct.NextDate = DateTime.Today;
                }

                await order.Rebill(uow, wsw, mapper);
            }

            #endregion

            return RedirectToAction("details", new { id = id });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ActionResult> Reattempt(int id)
        {
            #region Reattempt (Process order again) call just on Declined new orders (not rebill orders) Manual reattempt

            //if (os.UserAction == UserAction.reattempt)
            //{
            // check if this is allowed

            //var _oid = orderids.First();

            var order = uow.OrderRepository.Find(id);

            if (order != null && ((!order.IsRebill && order.Status == OrderStatus.Declined && !order.HasRebills) || order.Status == OrderStatus.Unpaid))
            {

                // set the recurring of all products to true to update the nextdate
                foreach (var op in order.OrderProducts)
                {
                    op.Recurring = true; // will this be updated ? it not => update every op in another loop 

                }

                // reattempt the order this will start the recurring system
                await order.Process(uow, wsw, mapper);

                //// updates the order and each product of course
                //foreach (var op in order.OrderProducts)
                //{
                //    uow.OrderProductRepository.Update(op); // no problem!!
                //}

                uow.OrderRepository.Update(order); // no problem!! will this update the order products ??
                
                foreach (var _oid in order.OrderProducts.Select(p => p.OrderId).Distinct())
                {
                    uow.OrderNoteRepository.Add(new OrderNote
                    {
                        OrderId = _oid,
                        NoteDate = DateTime.UtcNow,
                        Note = "Start Recurring",
                    });
                }

                uow.Save(wsw.CurrentUserName);
            }

            //var route = GetRouteData();
            //route["UserAction"] = UserAction.detail;
            //return RedirectToAction("index", route);
            return RedirectToAction("details", new { id = id });
            //}
            #endregion


        }
        /// <summary>
        /// Create ViewBag Products List, Credit Card Expiry Dates i.e. Months, Years
        /// </summary>
        /// <param name="ord"></param>
        private void CreateViewBag(Order ord = null)
        {
            ViewBag.ShippingMethods = uow.ShippingMethodRepository.Get(); // repo.ShippingMethods();
            ViewBag.Processors = uow.ProcessorRepository.Get(); // repo.Processors();
            var products = uow.ProductRepository.Get(); // repo.Products();
            ViewBag.Countries = uow.CountryRepository.Get(orderBy: c => c.OrderBy(t => t.Name)); //repo.Countries().OrderBy(c => c.Name);
            ViewBag.RMAReasons = uow.RMAReasonRepository.Get();

            //string productsJson = JsonConvert.SerializeObject(products.Select(p => new { Id = p.ProductId, Name = p.Name, Price = p.Price }));
            //ViewBag.ProductsJson = productsJson;
            ViewBag.ProductArray = products.Select(p => new { Id = p.ProductId, Name = p.Name, Price = p.Price }).ToList();
            ViewBag.Products = products;

            ViewBag.RMAReasons = new SelectList(uow.RMAReasonRepository.GetSet(), "Id", "Description");

            var yarr = new List<SelectListItem>();

            if (ord != null)
            {
                var yearCard = int.Parse(DateTime.Today.Year.ToString().Substring(0, 2) + (ord.CreditCardExpirationYear.ToString().Length > 2 ? ord.CreditCardExpirationYear.ToString().Substring(2).PadLeft(2, '0') : ord.CreditCardExpirationYear.ToString().PadLeft(2, '0')));

                if (yearCard < DateTime.Today.Year)
                {
                    for (int i = yearCard; i < DateTime.Today.Year; i++)
                    {
                        var y = new SelectListItem();
                        y.Text = i.ToString();
                        y.Value = i.ToString().Substring(2);
                        yarr.Add(y);
                    }
                }
            }

            for (int i = 0; i < 20; i++)
            {
                var year = DateTime.Today.Year + i;
                var y = new SelectListItem();
                y.Text = year.ToString();
                y.Value = year.ToString().Substring(2);
                yarr.Add(y);
            }

            ViewBag.CreditCardExpirationYears = yarr;

        }

        /// <summary>
        /// Create New Order from Existing Order with id and order object
        /// </summary>
        /// <param name="id"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public ActionResult New(int id)
        {
            var originalOrder = uow.OrderRepository.Find(id);
            if (originalOrder == null)
            {
                return HttpNotFound();
            }

            var products = uow.ProductRepository.Get();
            ViewBag.ProductArray = products.Select(p => new { Id = p.ProductId, Name = p.Name, Price = p.Price }).ToList();
            CreateViewBag();
            return View(GetOrderInfo(id));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Email(int id)
        {
            var order = uow.OrderRepository.Find(id);
            bool success = true;
            EmailHelper.SendOrderEmail(uow, mapper, NotificationType.OrderConfirmation, order.OrderId);
            ViewBag.Success = success;
            return View(order);
        }

        [HttpPost]
        public async Task<ActionResult> New(int id, Order order)
        {
            // remove errors from other field but the orderproducts
            var otherFields = ModelState.Keys.Where(s => !s.StartsWith("OrderProduct"));
            foreach (var field in otherFields)
            {
                ModelState[field].Errors.Clear();
            }

            if (order.OrderProducts == null || order.OrderProducts.Count == 0)
            {
                ModelState.AddModelError("", "At least one product is required");
            }

            if (ModelState.IsValid)
            {
                var originalOrder = uow.OrderRepository.Find(id);

                order.AffiliateId = originalOrder.AffiliateId;
                order.CustomerId = originalOrder.CustomerId;
                order.SubId = originalOrder.SubId;
                order.ShippingMethodId = originalOrder.ShippingMethodId;

                order.CreatedUserId = wsw.CurrentUserId;
                order.IPAddress = Request.UserHostAddress;
                await order.Create(uow, wsw, mapper);

                //order.RMANumber = string.Format("{0}-{1}", order.OrderId, KontinuityCRMHelper.GetRandomNumber());
                //uow.OrderRepository.Update(order);
                //uow.Save();

                return RedirectToAction("index");
            }

            var products = uow.ProductRepository.Get();
            ViewBag.ProductArray = products.Select(p => new { Id = p.ProductId, Name = p.Name, Price = p.Price }).ToList();
            CreateViewBag();
            return View(order);
        }

        /// <summary>
        /// Redirect ro create view
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()//int? id = null
        {
            //var newcustomer = new List<Customer> { new Customer { FirstName = "New", CustomerId = 0 } };
            //ViewBag.Customers = newcustomer.Concat(uow.CustomerRepository.Get());//repo.Customers()
            CreateViewBag();
            return View();
        }

        /// <summary>
        /// Create order post Action
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Create(Order order)
        {
            if (order.OrderProducts == null || order.OrderProducts.Count == 0)
            {
                ModelState.AddModelError("", "At least one product is required");
            }
            else
            {
                var IsTestCardNumber = uow.TestCardNumberRepository.Get(g => g.Number == order.CreditCardNumber);
                string cc = order.CreditCardNumber.ToString().Trim().Replace("-", "");

                if (uow.BlackListRepository.Get(b => b.CreditCard.Equals(cc)).ToList().Count > 0)
                {
                    ModelState.AddModelError("", "Credit Card Number is in the blacklist");
                }

                Customer customer = uow.CustomerRepository.Find(order.CustomerId);

                foreach (var op in order.OrderProducts)
                {
                    Product product = uow.ProductRepository.Find(op.ProductId);
                    op.Product = product;

                    if (IsTestCardNumber.Count() == 0 && product.IsSinglePurchaseLimit)
                    {
                        var email = "";
                        var phone = "";

                        if (order.CustomerId > 0)
                        {
                            email = customer.Email;
                            phone = customer.Phone;

                        }
                        else
                        {
                            email = order.Email;
                            phone = order.Phone;
                        }

                        var creditCardNumber = order.CreditCardNumber;

                        var orders = uow.OrderRepository.Get(o =>
                            o.OrderProducts.Select(p => p.ProductId).Contains(product.ProductId)
                            && (int)o.Status == 1
                            && ((o.Email == email) || (o.Phone == phone) || (o.CreditCardNumber == creditCardNumber))
                            ).ToList();

                        if (orders.Count > 0)
                        {
                            ModelState.AddModelError("", string.Format("Single Purchase Limit Error on Product: {0} ({1})", product.Name, product.ProductId));
                        }
                    }
                }

                if (ModelState.IsValid)
                {
                    order.CreatedUserId = wsw.CurrentUserId;
                    order.IPAddress = Request.UserHostAddress;

                    await order.Create(uow, wsw, mapper);
                    // here log that an order has been created
                    //order.RMANumber = string.Format("{0}-{1}", order.OrderId, KontinuityCRMHelper.GetRandomNumber());
                    //uow.OrderRepository.Update(order);
                    //uow.Save();

                    //order.TopParentId = uow.SqlQuery<int>("select dbo.GetParentID(@OrderId)", new SqlParameter("OrderId", order.OrderId)).SingleOrDefault();
                    uow.OrderRepository.Update(order);
                    uow.Save();
                    return RedirectToAction("Index", routeValues: new { fStatus = order.Status });
                }
            }
            CreateViewBag();
            return View(order);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderBatch"></param>
        /// <param name="os"></param>
        /// <param name="oid"></param>
        /// <param name="sortOrder"></param>
        /// <param name="page"></param>
        /// <param name="display"></param>
        /// <returns></returns>
        [HttpPost]
        [MultipleButton(Name = "action", Argument = "BatchSave")]
        public async Task<ActionResult> BatchSave(
            [System.Web.Http.FromBody] OrderBatch orderBatch,
            [System.Web.Http.FromUri] OrderSearch os,
            [System.Web.Http.FromBody] int[] oid
             , [System.Web.Http.FromUri] string sortOrder, [System.Web.Http.FromUri] int? page, [System.Web.Http.FromUri] int? display
            )
        {
            if (ModelState.IsValid)
            {
                var pifields = orderBatch.GetType().GetProperties().Where(p => p.GetValue(orderBatch) != null);

                if (pifields.Any())
                {
                    var orders = uow.OrderRepository.Get(o => oid.Contains(o.OrderId));

                    foreach (var order in orders)
                    {
                        foreach (var pi in pifields)
                        {
                            order.GetType().GetProperty(pi.Name).SetValue(order, pi.GetValue(orderBatch));
                        }
                        uow.OrderRepository.Update(order);
                    }
                    uow.Save(wsw.CurrentUserName);
                }

                var route = GetRouteData();
                //route["UserAction"] = UserAction.filter;
                route.Remove("UserAction"); // defaults to the useraction filter
                //route.Remove("oid"); // show all orders under the current status (approved, deleted, ...)
                return RedirectToAction("index", route);
            }

            return await Index(os, sortOrder, page, display);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        [HttpPost]
        [MultipleButton(Name = "action", Argument = "Save")]
        public ActionResult Save([System.Web.Http.FromBody] Order order)
        {
            //TryValidateModel(order);

            if (order.Status == OrderStatus.Approved)
            {
                string cc = order.CreditCardNumber.ToString().Trim().Replace("-", "");

                if (uow.BlackListRepository.Get(b => b.CreditCard.Equals(cc)).ToList().Count > 0)
                {
                    ModelState.AddModelError("", "Credit Card Number is in the blacklist");
                }
            }

            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(order.OrderNote))
                {
                    uow.OrderNoteRepository.Add(new OrderNote()
                    {
                        OrderId = order.OrderId,
                        Note = string.Format(order.OrderNote + "({0})", wsw.CurrentUserName),
                        NoteDate = DateTime.UtcNow
                    });
                }

                #region RMA Generation

                var rmaProducts = order.OrderProducts.Where(o => o.RMAReasonId != null).ToList();

                if (rmaProducts.Any())
                {
                    var time = DateTimeOffset.UtcNow;
                    foreach (var rmaProd in rmaProducts)
                    {
                        try
                        {
                            var keepRecurring = Convert.ToBoolean(Request.Form["rmarecurring_" + rmaProd.ProductId].ToString());
                            if (!keepRecurring)
                            {
                                //rmaProd.NextDate = null;
                                //rmaProd.Recurring = false;

                                KontinuityCRMHelper.CancelSubscription(rmaProd, uow, wsw, time);

                            }
                        }
                        catch { }
                    }

                    order.Status = OrderStatus.RMAIssued;

                    // add a note for this order
                    uow.OrderNoteRepository.Add(new OrderNote
                    {
                        OrderId = order.OrderId,
                        NoteDate = time.DateTime,
                        Note = "RMA Issue"
                    });
                }

                #endregion

                foreach (var op in order.OrderProducts)
                {
                    uow.OrderProductRepository.Update(op);
                }

                order.LastUpdate = DateTime.UtcNow;

                /*
                 * OJO: should we update the the customer with the order data????
                 */

                uow.OrderRepository.Update(order);
                uow.Save(wsw.CurrentUserName);

                return RedirectToAction("details", new { id = order.OrderId });

            }

            foreach (var op in order.OrderProducts)
            {
                op.Order = order;
            }

            CreateViewBag(order);

            var currentOrder = uow.OrderRepository.Find(order.OrderId);

            order.Transactions = currentOrder.Transactions;
            order.Notes = currentOrder.Notes;
            order.Customer = currentOrder.Customer;

            return View("details", order);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="refund_amount"></param>
        /// <param name="OrderId"></param>
        /// <param name="refund_keep_recurring"></param>
        /// <returns></returns>
        [HttpPost]
        [MultipleButton(Name = "action", Argument = "Refund")]
        public ActionResult Refund(decimal refund_amount, int OrderId, bool refund_keep_recurring = false)
        {
            var order = uow.OrderRepository.Find(OrderId);

            if (refund_amount > order.Total)
                ModelState.AddModelError("refund_amount", "The amount field must not exceed the order amount");

            #region Transactions with Refund, Capture, Sale
            //Transactions with Refund
            var refundTransactions = order.Transactions.Where(o =>
                                            o.Type == TransactionType.Refund
                                            && o.Status == Models.TransactionStatus.Approved
                                            && o.Success).ToList();

            //Transactions with Sale/Capture
            var saleCaptureTransactions = order.Transactions.Where(o =>
                                            (o.Type == TransactionType.Sale || o.Type == TransactionType.Capture)
                                            && o.Status == Models.TransactionStatus.Approved
                                            && o.Success).ToList();

            #endregion

            if ((refundTransactions.Sum(o => o.Amount)) + refund_amount > saleCaptureTransactions.Sum(o => o.Amount))
                ModelState.AddModelError("refund_amount", string.Format("Order is already partially refunded, total amount that can be refunded is: {0}", saleCaptureTransactions.Sum(o => o.Amount) - (refundTransactions.Sum(o => o.Amount))));

            if (ModelState.IsValid)
            {
                //First Transaction
                var transaction = order.Transactions.Where(t =>  // do we refund authorization transactions ??
                                                (t.Type == TransactionType.Sale || t.Type == TransactionType.Capture)
                                                && t.Status == Models.TransactionStatus.Approved
                                                && t.Success).FirstOrDefault(); // do  we need to mark this transaction as refunded ???


                if (transaction != null) // this transaction is the current approved transaction the one that would be refund on a new transaction
                {
                    var refundTransaction = transaction.Processor.GatewayModel(mapper).Refund(uow, mapper, NotificationType.OrderRefund, order.OrderId, transaction, refund_amount);

                    if (refundTransaction.Success)
                    {
                        // edit the sale / auth transaction and add this new transaction
                        if ((refundTransactions.Sum(o => o.Amount)) + refund_amount == saleCaptureTransactions.Sum(o => o.Amount)) // Determines whether order is fully refunded
                            order.Status = OrderStatus.Refunded;
                        else
                            order.Status = OrderStatus.PartiallyRefunded;

                        var time = DateTimeOffset.UtcNow;
                        refundTransaction.Amount = refund_amount;
                        order.Notes.Add(new OrderNote
                        {
                            NoteDate = time.DateTime,
                            Note = "Order Refund for " + refundTransaction.Amount + " on " + refundTransaction.Processor.Name
                        });

                        foreach (var op in order.OrderProducts)
                        {
                            order.TimeEvents.Add(new OrderTimeEvent
                            {
                                OrderId = order.OrderId,
                                ProductId = op.ProductId,
                                Time = time,
                                Event = OrderEvent.Refunded,
                                Action = op.Order.ParentId.HasValue ? op.ReAttempts > 0 : (bool?)null,
                                AffiliateId = op.Order.AffiliateId,
                                SubId = op.Order.SubId,
                            });
                        }

                        // if stop recurring after refund cancel subscription and all nextdate
                        if (refund_keep_recurring == false)
                        {
                            KontinuityCRMHelper.CancelSubscription(
                                order.OrderProducts.Where(op => op.NextDate != null)
                                , uow, wsw, time);
                        }

                        //  transaction.Status = Models.TransactionStatus.Refund;
                        //Do not need to update this transaction
                        // uow.TransactionRepository.Update(transaction);
                    }
                    else
                    {
                        order.Notes.Add(new OrderNote
                        {
                            NoteDate = DateTime.UtcNow,
                            Note = "Transaction Refund declined: " + refundTransaction.Message
                        });
                    }
                    uow.OrderRepository.Update(order);
                    uow.TransactionRepository.Add(refundTransaction);
                    uow.Save(wsw.CurrentUserName);

                    return RedirectToAction("details", new { id = OrderId });
                }
                else
                {
                    ModelState.AddModelError("", "No approved transaction was found");
                }
            }
            CreateViewBag();
            return View("details", order);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Void(int id)
        {
            #region void

            /*
                 * Void (void)
        Transaction voids will cancel an existing sale or captured authorization. In addition, non-captured authorizations
        can be voided to prevent any future capture. Voids can only occur if the transaction has not been settled.
                 * 
                 Refund (refund)
        Transaction refunds will reverse a previously settled transaction. If the transaction has not been settled, it must
        be voided instead of refunded.
                 */

            var order = uow.OrderRepository.Find(id);

            var transaction = order.Transactions.Where(t =>
                (t.Type == TransactionType.Sale || t.Type == TransactionType.Auth)
                && t.Status == Models.TransactionStatus.Approved
                && t.Success).FirstOrDefault(); // do  we need to mark this transaction as voided ???

            if (transaction != null)
            {
                var voidTransaction = transaction.Processor.GatewayModel(mapper).Void(transaction); //transaction.Gateway.Void(transaction);
                                                                                                    //voidTransaction.Date = DateTime.UtcNow;

                if (voidTransaction.Success)//(voidTransaction.Status == Models.TransactionStatus.Approved)
                {
                    // edit the sale / auth transaction and add this new transaction
                    // marl also the order as void
                    order.Status = OrderStatus.Void;


                    order.Notes.Add(new OrderNote
                    {
                        NoteDate = DateTime.UtcNow,
                        Note = "Order Void"
                    });

                    // update the transaction status
                    transaction.Status = Models.TransactionStatus.Void;
                    uow.TransactionRepository.Update(transaction);
                }
                else
                {
                    order.Notes.Add(new OrderNote
                    {
                        NoteDate = DateTime.UtcNow,
                        Note = "Transaction Void decline: " + voidTransaction.Message,
                    });
                }

                uow.OrderRepository.Update(order); // no problem !!
                uow.TransactionRepository.Add(voidTransaction);
                uow.Save(wsw.CurrentUserName);
            }


            #endregion

            return RedirectToAction("details", new { id = id });
        }

        /// <summary>
        /// Get Tax Info 
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="country"></param>
        /// <param name="city"></param>
        /// <param name="province"></param>
        /// <param name="postalCode"></param>
        /// <param name="prices"></param>
        /// <param name="shippingMethod"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetTaxInfo(string ids, string country, string city, string province, string postalCode, string prices, string shippingMethod)
        {
            var productId = ids.Split(',');
            var pr = prices.Split(',');
            var res = new Dictionary<string, string>();
            decimal shipValueVariant = 0;
            int index = 0;

            var shippingPrice = uow.ShippingMethodRepository.Find((string.IsNullOrWhiteSpace(shippingMethod) ? 0 : int.Parse(shippingMethod)));
            if (shippingPrice == null) shippingPrice = new ShippingMethod();

            foreach (var pid in productId)
            {
                if (!string.IsNullOrWhiteSpace(pid))
                {
                    decimal tax = 0;
                    var op = uow.ProductRepository.Find(int.Parse(pid));  //repo.FindProduct(op.ProductId);

                    string taxableVariant = "";

                    var pv = uow.ProductVariantRepository.Get(v => v.ProductId == op.ProductId && v.Country.CountryAbbreviation == country).FirstOrDefault();

                    if (pv != null)
                    {

                        var pve = uow.VariantExtraFieldRepository.Get(p => p.ProductVariantId == pv.ProductVariantId).FirstOrDefault();

                        if (pve != null)
                        {
                            if (pve.FieldName == "Taxable") taxableVariant = (!string.IsNullOrWhiteSpace(pve.FieldValue) && pve.FieldValue.ToLower() == "true" ? pve.FieldValue : "false");
                            if (pve.FieldName == "ShipValue")
                            {
                                if (shipValueVariant < Convert.ToDecimal(pve.FieldValue))
                                {
                                    shipValueVariant = Convert.ToDecimal(pve.FieldValue);
                                }
                            }

                        }

                    }

                    if (op.TaxProfileId.HasValue && (string.IsNullOrWhiteSpace(taxableVariant) || (!string.IsNullOrWhiteSpace(taxableVariant) && Convert.ToBoolean(taxableVariant))))
                    {
                        var taxProfile = op.TaxProfile ?? uow.TaxProfileRepository.Find(op.TaxProfileId.Value);

                        if (taxProfile.TaxRules != null)
                        {
                            var taxRule = taxProfile.TaxRules.Where(t =>
                               (t.ApplyToShipping && (t.Country.CountryAbbreviation == country
                               && ((!string.IsNullOrWhiteSpace(t.City) && t.City == city) || (string.IsNullOrWhiteSpace(t.City)))
                               && ((!string.IsNullOrWhiteSpace(t.Province) && t.Province == province) || (string.IsNullOrWhiteSpace(t.Province)))
                               && ((!string.IsNullOrWhiteSpace(t.PostalCode) && t.PostalCode == postalCode) || (string.IsNullOrWhiteSpace(t.PostalCode)))))
                               || (!t.ApplyToShipping && (t.Country.CountryAbbreviation == country
                               && ((!string.IsNullOrWhiteSpace(t.City) && t.City == city) || (string.IsNullOrWhiteSpace(t.City)))
                               && ((!string.IsNullOrWhiteSpace(t.Province) && t.Province == province) || (string.IsNullOrWhiteSpace(t.Province)))
                               && ((!string.IsNullOrWhiteSpace(t.PostalCode) && t.PostalCode == postalCode) || (string.IsNullOrWhiteSpace(t.PostalCode)))))

                               ).FirstOrDefault();

                            if (taxRule != null)
                            {
                                tax = taxRule.Tax * Convert.ToDecimal(pr[index]) / 100;
                                if (taxRule.ApplyToShipping)
                                {
                                    tax += taxRule.Tax * shippingPrice.Price / 100;
                                }

                            }
                        }
                    }

                    res.Add(pid, "$" + tax.ToString("#0.00"));
                    index += 1;
                }
            }

            return Json(res);
        }

        /// <summary>
        /// Return Orders against a CustomerId
        /// </summary>
        /// <param name="CustomerId"></param>
        /// <returns></returns>
        public ActionResult GetCustomerInfo(int CustomerId)
        {
            var order = uow.OrderRepository.Get(a => a.CustomerId == CustomerId).Select(b => new Order()
            {
                ShippingFirstName = b.ShippingFirstName,
                ShippingLastName = b.ShippingLastName,
                ShippingAddress1 = b.ShippingAddress1,
                ShippingAddress2 = b.ShippingAddress2,
                ShippingCity = b.ShippingCity,
                ShippingProvince = b.ShippingProvince,
                ShippingPostalCode = b.ShippingPostalCode,
                ShippingCountry = b.ShippingCountry,
                Phone = b.Phone,
                Email = b.Email,
                BillingFirstName = b.BillingFirstName,
                BillingLastName = b.BillingLastName,
                BillingAddress1 = b.BillingAddress1,
                BillingAddress2 = b.BillingAddress2,
                BillingCity = b.BillingCity,
                BillingProvince = b.BillingProvince,
                BillingPostalCode = b.BillingPostalCode,
                BillingCountry = b.BillingCountry,
                ShippingMethodId = b.ShippingMethodId,
                AffiliateId = b.AffiliateId,
                SubId = b.SubId
                //,
                //PaymentType = b.PaymentType,
                //CreditCardNumber = b.CreditCardNumber,
                //CreditCardExpirationMonth = b.CreditCardExpirationMonth,
                //CreditCardExpirationYear = b.CreditCardExpirationYear,
                //CreditCardCVV = b.CreditCardCVV

            }).OrderByDescending(a => a.OrderId).FirstOrDefault();
            if (order == null)
            {
                return Json(new Order()
                {
                    ShippingFirstName = "",
                    ShippingLastName = "",
                    ShippingAddress1 = "",
                    ShippingAddress2 = "",
                    ShippingCity = "",
                    ShippingProvince = "",
                    ShippingPostalCode = "",
                    ShippingCountry = "",
                    Phone = "",
                    Email = "",
                    BillingFirstName = "Emiliano",
                    BillingLastName = "Gonzalez",
                    BillingAddress1 = "NA",
                    BillingAddress2 = "",
                    BillingCity = "New York",
                    BillingProvince = "NY",
                    BillingPostalCode = "12001",
                    BillingCountry = "AR",
                    ShippingMethodId = 0,
                    AffiliateId = "",
                    SubId = "",
                    //PaymentType = 0,
                    //CreditCardExpirationMonth = Month.January,
                    //CreditCardExpirationYear = 2016,
                    //CreditCardCVV = ""

                }, JsonRequestBehavior.AllowGet);
            }
            return Json(order, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Return an order against an orderId
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public Order GetOrderInfo(int OrderId)
        {
            return uow.OrderRepository.Get(a => a.OrderId == OrderId).FirstOrDefault();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult MarkAsFraud(int id)
        {
            var order = uow.OrderRepository.Find(id);

            if (!string.IsNullOrWhiteSpace(order.CreditCardNumber))
            {
                var ccn = order.CreditCardNumber.Trim().Replace("-", "");

                if (uow.BlackListRepository.Get(c => c.CreditCard == ccn && c.OrderId == id).Count() == 0)
                {
                    BlackList bl = new BlackList();
                    bl.CreditCard = ccn;
                    bl.AddedDate = DateTime.Now;
                    bl.Attempts = 0;
                    bl.OrderId = id;
                    uow.BlackListRepository.Add(bl);
                    uow.Save(wsw.CurrentUserName);
                }
            }

            order.Status = OrderStatus.Fraud;
            uow.OrderRepository.Update(order);

            uow.OrderNoteRepository.Add(new OrderNote
            {
                OrderId = id,
                NoteDate = DateTime.UtcNow,
                Note = "Order Mark As Fraud",
            });

            uow.Save(wsw.CurrentUserName);

            return RedirectToAction("details", new { id = order.OrderId });
        }
        /// <summary>
        /// Get Payment Types against a Product with ProductId 
        /// </summary>
        /// <param name="ProductId"></param>
        /// <returns></returns>
        public ActionResult GetPaymentTypes(int ProductId)
        {
            return Json(uow.ProductPaymentTypeRepository.Get(a => a.ProductId == ProductId).Select(b => new PaymentTypes() { PaymentTypeId = b.PaymentTypeId, Name = b.PaymentType.Name }).ToList(), JsonRequestBehavior.AllowGet);
        }
    }
}
