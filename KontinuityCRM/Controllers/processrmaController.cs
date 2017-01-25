using AutoMapper;
using KontinuityCRM.Helpers;
using KontinuityCRM.Models;
using KontinuityCRM.Models.Enums;
using KontinuityCRM.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KontinuityCRM.Controllers
{
    public class processrmaController : BaseController
    {
        private readonly IMappingEngine mapper;
        public processrmaController(IUnitOfWork uow, IWebSecurityWrapper wsw, IMappingEngine mapper)
            : base(uow, wsw)
        {
            this.mapper = mapper;

        }
        //
        // GET: /processrma/
        public ActionResult Index()
        {
            ViewBag.SummaryList = new List<RMAProcessReasonsViewModel>();
            return View();
        }

        [HttpPost]
        public ActionResult Index(string RMANumbers)
        {
            List<RMAProcessReasonsViewModel> lstRmaToReturn = new List<RMAProcessReasonsViewModel>();
            if (!string.IsNullOrEmpty(RMANumbers))
            {
                RMANumbers = RMANumbers.Replace(" ", "");

                string[] Rma = RMANumbers.Split(',');

                foreach (var item in Rma)
                {
                    try
                    {
                        var order = uow.OrderRepository.Get(a => a.RMANumber == item).FirstOrDefault();
                        decimal amountToRefund = 0;
                        //Validate RMA
                        if (order != null)
                        {
                            if (order.Status == OrderStatus.OrderReturned)
                            {
                                lstRmaToReturn.Add(new RMAProcessReasonsViewModel() { RMANumber = item, Reason = "already processed" });
                                continue;
                            }
                            //check Expiry of RMA
                            var nowDateAfterAddingRMAExpirationDays = Convert.ToDateTime(order.RMANumberCreatedOn).AddDays(order.RMAReason.ExpirationDays == null ? 0 : Convert.ToInt32(order.RMAReason.ExpirationDays));// Now date after adding the expiratio days in the RMA generation date
                            if (DateTime.Now.Date < nowDateAfterAddingRMAExpirationDays.Date)
                            {
                                switch (order.RMAReason.Action)
                                {
                                    case RMAAction.NoAction:
                                        // mark the status as returned
                                        lstRmaToReturn.Add(new RMAProcessReasonsViewModel() { RMANumber = item, Reason = "returned- no action to process" });
                                        break;
                                    case RMAAction.FullRefund:
                                        // mark the status as returned
                                        //refund the whole amount
                                        amountToRefund = order.Total;
                                        lstRmaToReturn.Add(new RMAProcessReasonsViewModel() { RMANumber = item, Reason = "returned- full refund" });
                                        break;
                                    case RMAAction.ExcludeShipping:
                                        // mark the status as returned
                                        //only charge shipping method amount
                                        amountToRefund = order.Total - order.ShippingMethod.Price;
                                        lstRmaToReturn.Add(new RMAProcessReasonsViewModel() { RMANumber = item, Reason = "returned- exclude shipping fee" });
                                        break;
                                    case RMAAction.ExcludeRestockingFee:
                                        // mark the status as returned
                                        // sum up all the restocking fees of the products in order and calculate the amount
                                        foreach (var orderproduct in order.OrderProducts)
                                        {
                                            amountToRefund = amountToRefund + Convert.ToInt32(orderproduct.Product.ReStockingFee);
                                        }
                                        amountToRefund = order.Total - amountToRefund;
                                        lstRmaToReturn.Add(new RMAProcessReasonsViewModel() { RMANumber = item, Reason = "returned- exclude restocking fee" });
                                        break;
                                    case RMAAction.ExcludeRestockingFee_ExcludeShipping:
                                        //Adding both shipping and restocking fee and then remove
                                        foreach (var orderproduct in order.OrderProducts)
                                        {
                                            amountToRefund = amountToRefund + Convert.ToInt32(orderproduct.Product.ReStockingFee);
                                        }
                                        amountToRefund += order.ShippingMethod.Price;
                                        amountToRefund = order.Total - amountToRefund;
                                        lstRmaToReturn.Add(new RMAProcessReasonsViewModel() { RMANumber = item, Reason = "returned- exclude restocking fee and shipping fee" });
                                        break;
                                }


                            }
                            //return GetRefundAmount(order.RMAReason, order);
                            else
                            {
                                lstRmaToReturn.Add(new RMAProcessReasonsViewModel() { RMANumber = item, Reason = "expired" });
                                amountToRefund = 0;
                            }

                            #region Processind Action
                            //Process Action

                            var transaction = order.Transactions.Where(t =>  // do we refund authorization transactions ??
                             (t.Type == TransactionType.Sale || t.Type == TransactionType.Capture)
                             && t.Status == Models.TransactionStatus.Approved
                              && t.Success).FirstOrDefault();

                            if (transaction != null) // this transaction is the current approved transaction the one that would be refund on a new transaction
                            {
                                var refundTransaction = transaction.Processor.GatewayModel(mapper).Refund(uow, mapper, NotificationType.OrderRefund, order.OrderId, transaction, amountToRefund);

                                if (refundTransaction.Success)
                                {
                                    // edit the sale / auth transaction and add this new transaction
                                    order.Status = OrderStatus.OrderReturned;

                                    order.Notes.Add(new OrderNote
                                    {
                                        NoteDate = DateTime.Now,
                                        Note = "Order Returned"
                                    });

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

                            #endregion

                        }
                        else
                            lstRmaToReturn.Add(new RMAProcessReasonsViewModel() { RMANumber = item, Reason = "invalid" });


                    }
                    catch (Exception)
                    {
                        lstRmaToReturn.Add(new RMAProcessReasonsViewModel() { RMANumber = item, Reason = "invalid" });
                    }
                }


                ViewBag.SummaryList = lstRmaToReturn;
                ViewBag.Error = "";
                return View();
            }
            else
            {
                ViewBag.Error = "Enter RMA Number's";
                return View();
            }

        }
    }
}