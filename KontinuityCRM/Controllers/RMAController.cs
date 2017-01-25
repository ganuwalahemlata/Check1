using AutoMapper;
using KontinuityCRM.Filters;
using KontinuityCRM.Helpers;
using KontinuityCRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using KontinuityCRM.Models.Enums;

namespace KontinuityCRM.Controllers
{
    [CustomAuthorizeAttribute]
    [System.ComponentModel.DisplayName("RMA Reasons")]
    public class RMAController : BaseController
    {
        private readonly IMappingEngine mapper;

        public RMAController(IUnitOfWork uow, IWebSecurityWrapper wsw, IMappingEngine mapper)
            : base(uow, wsw)
        {
            this.mapper = mapper;
        }

        // GET: RMA
        /// <summary>
        /// Redirect to tlisting RMAReason  view 
        /// </summary>
        /// <param name="sortOrder"></param>
        /// <param name="currentFilter"></param>
        /// <param name="searchString"></param>
        /// <param name="page"></param>
        /// <param name="display"></param>
        /// <returns></returns>
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? display)
        {
            var user = uow.UserProfileRepository.Find(wsw.CurrentUserId);
            int pageSize = user.RMAReasonDisplay;

            if (display.HasValue && display != pageSize)
            {
                page = 1;
                pageSize = display.Value;

                user.RMAReasonDisplay = pageSize;

                uow.UserProfileRepository.Update(user);
                uow.Save(wsw.CurrentUserName);
            }
            if (searchString != null)
                page = 1;
            else
                searchString = currentFilter;

            ViewBag.CurrentSort = sortOrder;
            ViewBag.IdSortParm = String.IsNullOrEmpty(sortOrder) ? "id_asc" : "";
            ViewBag.DescriptionSortParm = sortOrder == "description" ? "description_desc" : "description";
            ViewBag.ActiveSortParm = sortOrder == "active" ? "active_desc" : "active";
            ViewBag.CustomActionSortParm = sortOrder == "customaction" ? "customaction_desc" : "customaction";

            ViewBag.customactionOrderIcon = "sort";
            ViewBag.descriptionOrderIcon = "sort";
            ViewBag.activeOrderIcon = "sort";
            ViewBag.idOrderIcon = "sort";

            Func<IQueryable<RMAReason>, IOrderedQueryable<RMAReason>> orderBy = o =>
            {
                switch (sortOrder)
                {
                    case "id_asc":
                        ViewBag.idOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Id);

                    case "customaction":
                        ViewBag.customactionOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.CustomAction).ThenByDescending(c => c.Id);
                    case "customaction_desc":
                        ViewBag.customactionOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.CustomAction).ThenByDescending(c => c.Id);

                    case "description":
                        ViewBag.descriptionOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Description).ThenByDescending(c => c.Id);
                    case "description_desc":
                        ViewBag.descriptionOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Description).ThenByDescending(c => c.Id);

                    case "active":
                        ViewBag.activeOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Active).ThenByDescending(c => c.Id);
                    case "active_desc":
                        ViewBag.activeOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Active).ThenByDescending(c => c.Id);


                    default:
                        ViewBag.idOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Id);
                }
            };


            ViewBag.CurrentFilter = searchString;

            Expression<Func<RMAReason, bool>> filter = null;

            if (!String.IsNullOrEmpty(searchString))
            {
                filter = f => f.Description.Contains(searchString)
                    //|| f.Name.Contains(searchString)
                    //|| f.Address1.Contains(searchString)
                    //|| f.Address2.Contains(searchString)
                    //|| f.Country.Contains(searchString)
                    //|| f.IPAddress.Contains(searchString)
                    //|| f.Phone.Contains(searchString)
                    ;
            }



            ViewBag.Display = pageSize;

            int pageNumber = (page ?? 1);

            var model = uow.RMAReasonRepository
                .GetPage(pageSize, pageNumber,
                //out count,
                filter: filter,
                orderBy: orderBy
                );

            return View(model);

            //return View(uow.RMAReasonRepository.Get());
        }

        // GET: RMA/Create
        /// <summary>
        /// Redirect to create RMAReason View
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            return View(new RMAReason());
        }

        // POST: RMA/Create
        /// <summary>
        /// post Action to create RMAReason
        /// </summary>
        /// <param name="rma"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(RMAReason rma)
        {
            if (ModelState.IsValid)
            {
                resetCustomActions(rma);
                uow.RMAReasonRepository.Add(rma);
                uow.Save();

                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("ERROR", "One of the field is Invalid");
            }
            return View(rma);
        }

        // GET: RMA/Edit/5
        /// <summary>
        /// Redirects to edit RMAReason View
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            var rma = uow.RMAReasonRepository.Find(id);
            if (rma == null)
            {
                return HttpNotFound();
            }
            return View(rma);
        }

        // POST: RMA/Edit/5
        /// <summary>
        /// post action to update RMAReason
        /// </summary>
        /// <param name="rma"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(RMAReason rma)
        {
            if (ModelState.IsValid)
            {
                resetCustomActions(rma);
                uow.RMAReasonRepository.Update(rma);
                uow.Save();
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("ERROR", "One of the field is Invalid");
            }
            return View(rma);
        }

        // GET: RMA/Delete/5
        /// <summary>
        /// delete by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int id)
        {
            var rma = uow.RMAReasonRepository.Find(id);
            if (rma == null)
            {
                return HttpNotFound();
            }
            uow.RMAReasonRepository.Delete(rma);
            uow.Save();
            return RedirectToAction("Index");
        }

        private void resetCustomActions(RMAReason rma)
        {
            if (!rma.CustomAction)
            {
                rma.Action = 0;
                rma.ExpiredAction = 0;
            }
        }
        /// <summary>
        /// Provides RMA Number
        /// </summary>
        /// <param name="OrderId">OrderId</param>
        /// <param name="RMAReasonId">RMAReason Id</param>
        /// <returns></returns>
        public ActionResult GetRMANumber(int OrderId, int RMAReasonId)
        {
            string RMANumber = string.Format("{0}-{1}", OrderId, KontinuityCRMHelper.GetRandomNumber());
            var order = uow.OrderRepository.Find(OrderId);
            order.RMANumber = RMANumber;
            order.RMAReasonId = RMAReasonId;
            order.RMANumberCreatedOn = DateTime.Now;
            uow.Save();
            EmailHelper.SendOrderEmail(uow, mapper, NotificationType.RMANotification, OrderId);
            return Json(new { RMANumber = RMANumber }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// action to process RMAReason
        /// </summary>
        /// <param name="OrderId"></param>
        /// <param name="RMANumber"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ProcessRMAReason(int OrderId = 0, string RMANumber = "")
        {
            Order order;
            if (OrderId != 0)
                order = uow.OrderRepository.Find(OrderId);
            else
                order = uow.OrderRepository.Get(o => o.RMANumber == RMANumber).FirstOrDefault();

            if (order != null)//&& order.Shipped == true
            {
                var transaction = order.Transactions.Where(t =>  // do we refund authorization transactions ??
                    (t.Type == TransactionType.Sale || t.Type == TransactionType.Capture)
                    && t.Status == Models.TransactionStatus.Approved
                    && t.Success)
                    .FirstOrDefault();

                //changed with the order RMA reason later
                decimal refund_amount = RefundAmount(order.RMAReason, order);

                if (transaction != null) // this transaction is the current approved transaction the one that would be refund on a new transaction
                {
                    var refundTransaction = transaction.Processor.GatewayModel(mapper).Refund(uow, mapper, NotificationType.OrderRefund, order.OrderId, transaction, refund_amount);

                    if (refundTransaction.Success)
                    {
                        // edit the sale / auth transaction and add this new transaction
                        order.Status = OrderStatus.OrderReturned;

                        order.Notes.Add(new OrderNote
                        {
                            NoteDate = DateTime.Now,
                            Note = "Order Returned"
                        });

                        //foreach (var op in order.OrderProducts)
                        //{
                        //    order.TimeEvents.Add(new OrderTimeEvent
                        //    {
                        //        OrderId = order.OrderId,
                        //        ProductId = op.ProductId,
                        //        Time = time,
                        //        Event = OrderEvent.Refunded,
                        //        Action = op.Order.ParentId.HasValue ? op.ReAttempts > 0 : (bool?)null,
                        //        AffiliateId = op.Order.AffiliateId,
                        //        SubId = op.Order.SubId,
                        //    });
                        //}

                        //// if stop recurring after refund cancel subscription and all nextdate
                        //KontinuityCRMHelper.CancelSubscription(
                        //    order.OrderProducts.Where(op => op.NextDate != null)
                        //    , uow, time);

                        uow.TransactionRepository.Update(transaction);
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
                }

                return RedirectToAction("Details", "order", new { id = order.OrderId });
            }
            else
                return RedirectToAction("Details", "order", new { id = order.OrderId });
        }
        
        //Active RMA reasons are recieved
        /// <summary>
        /// Return Refund Amount against given rmaReason and Order
        /// </summary>
        /// <param name="rmaReason"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        private decimal RefundAmount(RMAReason rmaReason, Order order)
        {
            var nowDateAfterAddingRMAExpirationDays = Convert.ToDateTime(order.RMANumberCreatedOn).AddDays(rmaReason.ExpirationDays == null ? 0 : Convert.ToInt32(rmaReason.ExpirationDays));// Now date after adding the expiratio days in the RMA generation date
            if (DateTime.Now.Date < nowDateAfterAddingRMAExpirationDays.Date)
                return GetRefundAmount(rmaReason.Action, order);

            return 0;
        }
        /// <summary>
        /// Return Refund Amount against given rmaAction and Order
        /// </summary>
        /// <param name="rmaAction">RMA Action</param>
        /// <param name="order">Order</param>
        /// <returns></returns>
        private decimal GetRefundAmount(RMAAction rmaAction, Order order)
        {
            decimal returnAmount = 0;
            switch (rmaAction)
            {
                case RMAAction.NoAction:
                    // mark the status as returned
                    break;
                case RMAAction.FullRefund:
                    // mark the status as returned
                    //refund the whole amount
                    returnAmount = order.Total;
                    break;
                case RMAAction.ExcludeShipping:
                    // mark the status as returned
                    //only charge shipping method amount
                    returnAmount = order.Total - order.ShippingMethod.Price;
                    break;
                case RMAAction.ExcludeRestockingFee:
                    // mark the status as returned
                    // sum up all the restocking fees of the products in order and calculate the amount
                    foreach (var orderproduct in order.OrderProducts)
                    {
                        returnAmount = returnAmount + Convert.ToInt32(orderproduct.Product.ReStockingFee);
                    }
                    returnAmount = order.Total - returnAmount;
                    break;
                case RMAAction.ExcludeRestockingFee_ExcludeShipping:
                    //Adding both shipping and restocking fee and then remove
                    foreach (var orderproduct in order.OrderProducts)
                    {
                        returnAmount = returnAmount + Convert.ToInt32(orderproduct.Product.ReStockingFee);
                    }
                    returnAmount += order.ShippingMethod.Price;


                    returnAmount = order.Total - returnAmount;
                    break;
            }
            return returnAmount;
        }
        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
