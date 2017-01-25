using System.Globalization;
using KontinuityCRM.Helpers;
using KontinuityCRM.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KontinuityCRM.Models;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.Data.Common;
using System.Data;
using KontinuityCRM.Models.ViewModels.Enum;
using PagedList;
using KontinuityCRM.Filters;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Routing;
using System.Text.RegularExpressions;
using AutoMapper;
using System.Linq.Expressions;
using Newtonsoft.Json;
using DotNet.Highcharts;
using DotNet.Highcharts.Options;
using DotNet.Highcharts.Enums;
using System.Drawing;
using DotNet.Highcharts.Helpers;
using KontinuityCRM.Models.Gateways;

namespace KontinuityCRM.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [CustomAuthorizeAttribute]
    [System.ComponentModel.DisplayName("Reports")]
    public class reportController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uow"></param>
        /// <param name="wsw"></param>
        /// 
        private readonly IMappingEngine mapper;

        public reportController(IUnitOfWork uow, IWebSecurityWrapper wsw, IMappingEngine mapper)
            : base(uow, wsw)
        {
            this.mapper = mapper;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="Date"></param>        
        /// <param name="sortOrder"></param>
        /// <param name="page"></param>
        /// <param name="display"></param>
        /// <param name="ProductId"></param>
        /// <param name="AffiliateId"></param>
        /// <param name="SubId"></param>
        /// <param name="AffiliateIdVal"></param>
        /// <param name="ProductIdVal"></param>
        /// <param name="SubIdVal"></param>
        /// <returns></returns>
        public ActionResult View(string Date, string sortOrder, int? page, int? display, bool ProductId, bool AffiliateId, bool SubId, string AffiliateIdVal, int? ProductIdVal, string SubIdVal, RebillSearchOptionEnum searchByDateOption, string status)
        {
            DateTime fromDateTime = DateTime.ParseExact(Date, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            DateTime toDateTime = fromDateTime.AddDays(1);
            TimeZoneInfo userTimezoneInfo = CurrentUsersTimezone();
            fromDateTime = TimeZoneUtility.ToUtc(fromDateTime, userTimezoneInfo);
            toDateTime = TimeZoneUtility.ToUtc(toDateTime, userTimezoneInfo);

            var ordersTime = uow.OrderTimeEventRepository.GetSet()
                            .WhereIf(searchByDateOption == RebillSearchOptionEnum.RebillDate, o => fromDateTime <= o.Time && o.Time < toDateTime)
                            .WhereIf(searchByDateOption == RebillSearchOptionEnum.SignUpDate, o => fromDateTime <= o.Order.Created && o.Order.Created < toDateTime)
                            .Where(o => ((AffiliateId == false) || (AffiliateId == true && o.AffiliateId == AffiliateIdVal))
                                && ((ProductId == false) || (ProductId == true && o.ProductId == ProductIdVal))
                                && ((SubId == false) || (SubId == true && o.SubId == SubIdVal)))
                            .ToList();

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

            //Rebill Report Details show all orders on click
            List<int> ordersId = null;
            switch (status)
            {
                case "Gross":
                    ordersId = ordersTime.Where(o => o.Event == OrderEvent.Approved || o.Event == OrderEvent.Declined).Select(o => o.OrderId).ToList();
                    break;
                case "PreCancels":
                    ordersId = ordersTime.Where(o => o.Event == OrderEvent.Canceled && o.Action == null).Select(o => o.OrderId).ToList();
                    break;
                case "Attempted":
                    ordersId = ordersTime.Where(o => (o.Event == OrderEvent.Approved && o.Action == null) || (o.Event == OrderEvent.Declined && o.Action == null)).Select(o => o.OrderId).ToList();
                    break;
                case "Approved":
                    ordersId = ordersTime.Where(o => o.Event == OrderEvent.Approved && o.Action == null).Select(o => o.OrderId).ToList();
                    break;
                case "Declined":
                    ordersId = ordersTime.Where(o => o.Event == OrderEvent.Declined && o.Action == null).Select(o => o.OrderId).ToList();
                    break;
                case "Refunded":
                    ordersId = ordersTime.Where(o => o.Event == OrderEvent.Refunded && o.Action == null).Select(o => o.OrderId).ToList();
                    break;
                case "Rebills":
                    ordersId = ordersTime.Where(o => (o.Event == OrderEvent.Approved && o.Action == false) || (o.Event == OrderEvent.Declined && o.Action == false)).Select(o => o.OrderId).ToList();
                    break;
                case "PostCancels":
                    ordersId = ordersTime.Where(o => o.Event == OrderEvent.Canceled && o.Action == false).Select(o => o.OrderId).ToList();
                    break;
                case "RebillsApproved":
                    ordersId = ordersTime.Where(o => o.Event == OrderEvent.Approved && o.Action == false).Select(o => o.OrderId).ToList();
                    break;
                case "RebillsDeclined":
                    ordersId = ordersTime.Where(o => o.Event == OrderEvent.Declined && o.Action == false).Select(o => o.OrderId).ToList();
                    break;
                case "RebillsRefunded":
                    ordersId = ordersTime.Where(o => o.Event == OrderEvent.Refunded && o.Action == false).Select(o => o.OrderId).ToList();
                    break;
                case "Reattempts":
                    ordersId = ordersTime.Where(o => (o.Event == OrderEvent.Approved && o.Action == true) || (o.Event == OrderEvent.Declined && o.Action == true)).Select(o => o.OrderId).ToList();
                    break;
                case "ReattemptsApproved":
                    ordersId = ordersTime.Where(o => o.Event == OrderEvent.Approved && o.Action == true).Select(o => o.OrderId).ToList();
                    break;
                case "ReattemptsDeclined":
                    ordersId = ordersTime.Where(o => o.Event == OrderEvent.Declined && o.Action == true).Select(o => o.OrderId).ToList();
                    break;
                case "ReattemptsRefunded":
                    ordersId = ordersTime.Where(o => o.Event == OrderEvent.Refunded && o.Action == true).Select(o => o.OrderId).ToList();
                    break;
                case "PostReattemptCancels":
                    ordersId = ordersTime.Where(o => o.Event == OrderEvent.Canceled && o.Action == true).Select(o => o.OrderId).ToList();
                    break;
                default:
                    break;
            }

            Expression<Func<Order, bool>> filter = o => ordersId.Contains(o.OrderId);

            var user = CurrentUsersProfile();
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

            var os = new OrderSearch();
            os.Orders = new PagedListMapper<Order>(model, model.CloneProps().ConvertUtcTime(userTimezoneInfo));

            return View("view", os);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="sortOrder"></param>
        /// <param name="page"></param>
        /// <param name="display"></param>
        /// <param name="ProductId"></param>
        /// <param name="AffiliateId"></param>
        /// <param name="SubId"></param>
        /// <returns></returns>
        public ActionResult Rebill(DateTime? start, DateTime? end, string sortOrder, int? page, int? display, bool? ProductId, bool? AffiliateId, bool? SubId, RebillSearchOptionEnum? searchByDateOption)
        {
            end = end ?? DateTime.Today;
            start = start ?? DateTime.Today.AddDays(-7);
            searchByDateOption = searchByDateOption ?? RebillSearchOptionEnum.RebillDate;

            var utdate = end.Value.AddDays(1);
            var ufdate = start.Value;

            ViewBag.end = end.Value.ToString("MM/dd/yyyy");
            ViewBag.start = start.Value.ToString("MM/dd/yyyy");
            ViewBag.searchByDateOption = (int)searchByDateOption;

            var user = CurrentUsersProfile();
            int pageSize = user.RebillReportDisplay;

            if (display.HasValue && display != pageSize)
            {
                page = 1;
                pageSize = display.Value;

                user.RebillReportDisplay = pageSize;
                uow.UserProfileRepository.Update(user);
                uow.Save(wsw.CurrentUserName);
            }

            #region Columns

            ViewBag.ProductId = ProductId ?? false;
            ViewBag.SubId = SubId ?? false;
            ViewBag.AffiliateId = AffiliateId ?? false;

            ViewBag.CurrentSort = sortOrder;
            ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrder) ? "Date_asc" : "";
            ViewBag.ProductIdSortParm = sortOrder == "ProductId" ? "ProductId_desc" : "ProductId";
            ViewBag.AffiliateIdSortParm = sortOrder == "AffiliateId" ? "AffiliateId_desc" : "AffiliateId";
            ViewBag.SubIdSortParm = sortOrder == "SubId" ? "SubId_desc" : "SubId";

            ViewBag.ApprovedSortParm = sortOrder == "Approved" ? "Approved_desc" : "Approved";
            ViewBag.DeclinedSortParm = sortOrder == "Declined" ? "Declined_desc" : "Declined";
            ViewBag.RefundedSortParm = sortOrder == "Refunded" ? "Refunded_desc" : "Refunded";
            ViewBag.CanceledSortParm = sortOrder == "Canceled" ? "Canceled_desc" : "Canceled";

            ViewBag.RebillsApprovedSortParm = sortOrder == "RebillsApproved" ? "RebillsApproved_desc" : "RebillsApproved";
            ViewBag.RebillsDeclinedSortParm = sortOrder == "RebillsDeclined" ? "RebillsDeclined_desc" : "RebillsDeclined";
            ViewBag.RebillsRefundedSortParm = sortOrder == "RebillsRefunded" ? "RebillsRefunded_desc" : "RebillsRefunded";
            ViewBag.RebillsCanceledSortParm = sortOrder == "RebillsCanceled" ? "RebillsCanceled_desc" : "RebillsCanceled";

            ViewBag.ReattemptsApprovedSortParm = sortOrder == "ReattemptsApproved" ? "ReattemptsApproved_desc" : "ReattemptsApproved";
            ViewBag.ReattemptsDeclinedSortParm = sortOrder == "ReattemptsDeclined" ? "ReattemptsDeclined_desc" : "ReattemptsDeclined";
            ViewBag.ReattemptsRefundedSortParm = sortOrder == "ReattemptsRefunded" ? "ReattemptsRefunded_desc" : "ReattemptsRefunded";
            ViewBag.ReattemptsCanceledSortParm = sortOrder == "ReattemptsCanceled" ? "ReattemptsCanceled_desc" : "ReattemptsCanceled";

            ViewBag.GrossSortParm = sortOrder == "Gross" ? "Gross_desc" : "Gross";
            ViewBag.RebillsSortParm = sortOrder == "Rebills" ? "Rebills_desc" : "Rebills";
            ViewBag.ReattemptsSortParm = sortOrder == "Reattempts" ? "Reattempts_desc" : "Reattempts";
            ViewBag.AttemptedSortParm = sortOrder == "Attempted" ? "Attempted_desc" : "Attempted";


            ViewBag.DateOrderIcon = "sort";
            ViewBag.ProductIdOrderIcon = "sort";
            ViewBag.AffiliateIdOrderIcon = "sort";
            ViewBag.SubIdOrderIcon = "sort";

            ViewBag.AttemptedOrderIcon = "sort";
            ViewBag.ReattemptsOrderIcon = "sort";
            ViewBag.GrossOrderIcon = "sort";
            ViewBag.RebillsOrderIcon = "sort";

            ViewBag.ReattemptsApprovedOrderIcon = "sort";
            ViewBag.ReattemptsDeclinedOrderIcon = "sort";
            ViewBag.ReattemptsRefundedOrderIcon = "sort";
            ViewBag.ReattemptsCanceledOrderIcon = "sort";

            ViewBag.RebillsCanceledOrderIcon = "sort";
            ViewBag.RebillsRefundedOrderIcon = "sort";
            ViewBag.RebillsDeclinedOrderIcon = "sort";
            ViewBag.RebillsApprovedOrderIcon = "sort";

            ViewBag.ApprovedOrderIcon = "sort";
            ViewBag.CanceledOrderIcon = "sort";
            ViewBag.DeclinedOrderIcon = "sort";
            ViewBag.RefundedOrderIcon = "sort";

            #endregion

            #region Sort
            Func<IQueryable<RebillModel>, IOrderedQueryable<RebillModel>> _orderBy = o =>
            {
                switch (sortOrder)
                {
                    case "Date_asc":
                        ViewBag.DateOrderIcon = "sorting_asc";
                        return o.OrderBy(r => r.Year).ThenBy(r => r.Month).ThenBy(c => c.Day);

                    case "Reattempts":
                        ViewBag.ReattemptsOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.ReattemptsApproved + c.ReattemptsDeclined);
                    case "Reattempts_desc":
                        ViewBag.ReattemptsOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.ReattemptsApproved + c.ReattemptsDeclined);

                    case "ProductId":
                        ViewBag.ProductIdOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.ProductId);
                    case "ProductId_desc":
                        ViewBag.ProductIdOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.ProductId);

                    case "AffiliateId":
                        ViewBag.AffiliateIdOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.AffiliateId);
                    case "AffiliateId_desc":
                        ViewBag.AffiliateIdOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.AffiliateId);

                    case "SubId":
                        ViewBag.SubIdOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.SubId);
                    case "SubId_desc":
                        ViewBag.SubIdOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.SubId);

                    /***************** Initials ******************/

                    case "Approved":
                        ViewBag.ApprovedOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Approved);
                    case "Approved_desc":
                        ViewBag.ApprovedOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Approved);

                    case "Declined":
                        ViewBag.DeclinedOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Declined);
                    case "Declined_desc":
                        ViewBag.DeclinedOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Declined);

                    case "Refunded":
                        ViewBag.RefundedOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Refunded);
                    case "Refunded_desc":
                        ViewBag.RefundedOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Refunded);

                    case "Canceled":
                        ViewBag.CanceledOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Canceled);
                    case "Canceled_desc":
                        ViewBag.CanceledOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Canceled);

                    /*************** Reattempts ********************/


                    case "ReattemptsApproved":
                        ViewBag.ReattemptsApprovedOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.ReattemptsApproved);
                    case "ReattemptsApproved_desc":
                        ViewBag.ReattemptsApprovedOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.ReattemptsApproved);

                    case "ReattemptsDeclined":
                        ViewBag.ReattemptsDeclinedOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.ReattemptsDeclined);
                    case "ReattemptsDeclined_desc":
                        ViewBag.ReattemptsDeclinedOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.ReattemptsDeclined);

                    case "ReattemptsRefunded":
                        ViewBag.ReattemptsRefundedOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.ReattemptsRefunded);
                    case "ReattemptsRefunded_desc":
                        ViewBag.ReattemptsRefundedOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.ReattemptsRefunded);

                    case "ReattemptsCanceled":
                        ViewBag.ReattemptsCanceledOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.ReattemptsCanceled);
                    case "ReattemptsCanceled_desc":
                        ViewBag.ReattemptsCanceledOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.ReattemptsCanceled);

                    /**************** Rebills *******************/

                    case "RebillsApproved":
                        ViewBag.RebillsApprovedOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.RebillsApproved);
                    case "RebillsApproved_desc":
                        ViewBag.RebillsApprovedOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.RebillsApproved);

                    case "RebillsDeclined":
                        ViewBag.RebillsDeclinedOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.RebillsDeclined);
                    case "RebillsDeclined_desc":
                        ViewBag.RebillsDeclinedOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.RebillsDeclined);

                    case "RebillsRefunded":
                        ViewBag.RebillsRefundedOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.RebillsRefunded);
                    case "RebillsRefunded_desc":
                        ViewBag.RebillsRefundedOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.RebillsRefunded);

                    case "RebillsCanceled":
                        ViewBag.RebillsCanceledOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.RebillsCanceled);
                    case "RebillsCanceled_desc":
                        ViewBag.RebillsCanceledOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.RebillsCanceled);

                    /***********************************/


                    case "Attempted":
                        ViewBag.AttemptedOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Approved + c.Declined);
                    case "Attempted_desc":
                        ViewBag.AttemptedOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Approved + c.Declined);

                    case "Rebills":
                        ViewBag.RebillsOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.RebillsApproved + c.RebillsDeclined);
                    case "Rebills_desc":
                        ViewBag.RebillsOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.RebillsApproved + c.RebillsDeclined);

                    case "Gross":
                        ViewBag.GrossOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Approved + c.Declined + c.RebillsDeclined + c.RebillsApproved + c.ReattemptsDeclined + c.ReattemptsApproved);
                    case "Gross_desc":
                        ViewBag.GrossOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Approved + c.Declined + c.RebillsDeclined + c.RebillsApproved + c.ReattemptsDeclined + c.ReattemptsApproved);

                    default:
                        ViewBag.DateOrderIcon = "sorting_desc";
                        return o.OrderByDescending(r => r.Year).ThenByDescending(r => r.Month).ThenByDescending(r => r.Day);
                }
            };
            #endregion

            //user timezone to utc
            TimeZoneInfo userTimezoneInfo = CurrentUsersTimezone();
            var offset = userTimezoneInfo.BaseUtcOffset.TotalMinutes; // daylightsaving will cause an hour difference!!
            ufdate = TimeZoneUtility.ToUtc(ufdate, userTimezoneInfo);
            utdate = TimeZoneUtility.ToUtc(utdate, userTimezoneInfo);

            var query = from _event in uow.OrderTimeEventRepository.GetSet()
                            .WhereIf(searchByDateOption == RebillSearchOptionEnum.RebillDate, o => ufdate <= o.Time && o.Time < utdate)
                            .WhereIf(searchByDateOption == RebillSearchOptionEnum.SignUpDate, o => ufdate <= o.Order.Created && o.Order.Created < utdate)
                        group _event by new
                        {
                            //Date = SqlFunctions.DateAdd("mi", offset, _event.Time).Value,

                            Year = //_event.Time.Year,  
                                SqlFunctions.DateAdd("mi", offset, searchByDateOption == RebillSearchOptionEnum.RebillDate ? _event.Time : _event.Order.Created).Value.Year,
                            Day = //_event.Time.Day,
                                 SqlFunctions.DateAdd("mi", offset, searchByDateOption == RebillSearchOptionEnum.RebillDate ? _event.Time : _event.Order.Created).Value.Day,
                            Month = //_event.Time.Month,
                                SqlFunctions.DateAdd("mi", offset, searchByDateOption == RebillSearchOptionEnum.RebillDate ? _event.Time : _event.Order.Created).Value.Month,

                            ProductId = ProductId == true ? _event.ProductId : 0,
                            SubId = SubId == true ? _event.SubId : string.Empty,
                            AffiliateId = AffiliateId == true ? _event.AffiliateId : string.Empty,

                        } into od
                        select new RebillModel
                        {
                            Year = od.Key.Year,
                            Month = od.Key.Month,
                            Day = od.Key.Day,
                            ProductId = od.Key.ProductId,
                            AffiliateId = od.Key.AffiliateId,
                            SubId = od.Key.SubId,

                            // initials
                            Approved = od.Count(o => o.Event == OrderEvent.Approved && o.Action == null),
                            Declined = od.Count(o => o.Event == OrderEvent.Declined && o.Action == null),
                            Refunded = od.Count(o => o.Event == OrderEvent.Refunded && o.Action == null),
                            Canceled = od.Count(o => o.Event == OrderEvent.Canceled && o.Action == null),
                         //   OrderList = uow.OrderTimeEventRepository.GetSet().Where(a => a.Event == OrderEvent.Approved && a.Action == false && a.Event == OrderEvent.Declined && a.Time== new DateTime(od.Key.Year, od.Key.Month, od.Key.Day)).ToList(),
            //Reattempts

            ReattemptsApproved = od.Count(o => o.Event == OrderEvent.Approved && o.Action == true),
                            ReattemptsDeclined = od.Count(o => o.Event == OrderEvent.Declined && o.Action == true),
                            ReattemptsRefunded = od.Count(o => o.Event == OrderEvent.Refunded && o.Action == true),
                            ReattemptsCanceled = od.Count(o => o.Event == OrderEvent.Canceled && o.Action == true),

                            //rebills
                            RebillsApproved = od.Count(o => o.Event == OrderEvent.Approved && o.Action == false),
                            RebillsDeclined = od.Count(o => o.Event == OrderEvent.Declined && o.Action == false),
                            RebillsRefunded = od.Count(o => o.Event == OrderEvent.Refunded && o.Action == false),
                            RebillsCanceled = od.Count(o => o.Event == OrderEvent.Canceled && o.Action == false),
                        };


            query = _orderBy(query).ThenByDescending(c => c.Year)
                                   .ThenByDescending(c => c.Month)
                                   .ThenByDescending(c => c.Day)
                                   .ThenByDescending(c => c.ProductId)
                                   .ThenBy(c => c.AffiliateId)
                                   .ThenBy(c => c.SubId)
                                   ;

            int pageNumber = (page ?? 1);
            ViewBag.Display = pageSize;
            //ViewBag.Rebills = uow.OrderTimeEventRepository.GetSet().Where(a => a.Event == OrderEvent.Approved && a.Action == false && a.Event == OrderEvent.Declined && a.Time==).ToList();
            var report = query.ToPagedList(pageNumber, pageSize);

            for (int i = 0; i < report.Count; i++)
            {
              //  var value1 = uow.OrderTimeEventRepository.Get().Where(a => (a.Event == OrderEvent.Approved || a.Event == OrderEvent.Declined) && a.Action == false &&  a.Time == report[i].DateT).ToList();

                var value = uow.OrderTimeEventRepository.Get().Where(a => (a.Event == OrderEvent.Approved || a.Event == OrderEvent.Declined) && a.Action == false && a.Time == report[i].DateT).ToList();
                report[i].OrderList = value;
                //Reattempts
            }

            //var command = uow.CreateCommand();

            //Func<string, DbType, object, DbParameter> createParam = (name, type, value) => 
            //{
            //    var param = command.CreateParameter();
            //    param.DbType = type;
            //    param.ParameterName = name;
            //    param.Value = value;
            //    return param;
            //};


            //var _howMany = command.CreateParameter();
            //_howMany.Direction = System.Data.ParameterDirection.Output;
            //_howMany.ParameterName = "@HowMany";
            //_howMany.DbType = System.Data.DbType.Int32;

            //var report = uow.SqlQuery<RebillModel>(
            //    "EXEC [RebillReport] @ProductId, @AffiliateId, @SubId, @FromDate, @ToDate, @sortOrder, @PageNumber, @PageSize, @HowMany OUT"  //
            //    //, ProductId, AffiliateId, SubId, ufdate, utdate, sortOrder, page, display, _howMany)
            //    , createParam("@ProductId", DbType.Boolean, ProductId ?? false)
            //    , createParam("@AffiliateId", DbType.Boolean, AffiliateId ?? false)
            //    , createParam("@SubId", DbType.Boolean, SubId ?? false)
            //    , createParam("@FromDate", DbType.DateTime, ufdate)
            //    , createParam("@ToDate", DbType.DateTime, utdate)
            //    , createParam("@sortOrder", DbType.String, "")
            //    , createParam("@PageNumber", DbType.Int32, 1)
            //    , createParam("@PageSize", DbType.Int32, 10)
            //    , _howMany) 
            //    .ToList();

            //var howMany = Convert.ToInt32(_howMany.Value);              

            return View(report);
        }

        private void ExecuteSelectedFailedTransactions(int id, string ProcessorTypeVal, string CardTypeVal, bool? ProcessorType, bool? CardType)
        {
            var modelPrepaidTransaction = uow.TransactionViaPrepaidCardQueueRepository.Get().Where(a => a.Success == false && a.Id == id).FirstOrDefault();

           // var modelPrepaidTransaction = uow.TransactionViaPrepaidCardQueueRepository.GetSet()

          //              .Where(o => ((ProcessorType == false) || (ProcessorType == true && o.Processor.Name == ProcessorTypeVal))
          //                  && ((CardType == false) || (CardType == true && o.PrepaidCard.PaymentType == CardTypeVal)) && o.Success==false && o.Id==id)
          //              .FirstOrDefault();

            // modelPrepaidTransaction[count].Id;
            //foreach (var itemForTransaction in modelPrepaidTransaction)
            //{
            TransactionSingle(modelPrepaidTransaction.Processor, modelPrepaidTransaction.Amount, modelPrepaidTransaction.PrepaidCard, modelPrepaidTransaction.Id, mapper);
            //}
        }

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

        public ActionResult ViewPrepaidTransaction(string sortOrder, int? page, int? display, bool? ProcessorType, bool? CardType, string ProcessorTypeVal, string CardTypeVal, string status, string processDeclineSelected, string[] checkedItem)
        {

            if (processDeclineSelected != null)
            {
                if (checkedItem != null)
                {
                    var modelPrepaidTransaction = uow.TransactionViaPrepaidCardQueueRepository.GetSet()

                          .Where(o => ((ProcessorType == false) || (ProcessorType == true && o.Processor.Name == ProcessorTypeVal))
                              && ((CardType == false) || (CardType == true && o.PrepaidCard.PaymentType == CardTypeVal)) && o.Success == false).OrderByDescending(a => a.Id)
                          .ToList();
                    int j = 0;
                    for (int i = 0; i < checkedItem.Length; i++)
                    {
                        if (checkedItem[i] == "true")
                        {
                            int Transaction_Id = modelPrepaidTransaction[j].Id;
                            ExecuteSelectedFailedTransactions(Transaction_Id, ProcessorTypeVal, CardTypeVal, ProcessorType, CardType);
                            i++;
                           
                        }
                        j++;
                    }
                }
                ViewBag.Decline = "checked";
                return RedirectToAction("PrepaidTransactionReport");
            }

            var ordersTime = uow.TransactionViaPrepaidCardQueueRepository.GetSet()

                            .Where(o => ((ProcessorType == false) || (ProcessorType == true && o.Processor.Name == ProcessorTypeVal))
                                && ((CardType == false) || (CardType == true && o.PrepaidCard.PaymentType == CardTypeVal)))
                            .ToList();

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

            Func<IQueryable<TransactionViaPrepaidCardQueue>, IOrderedQueryable<TransactionViaPrepaidCardQueue>> orderBy = o =>
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

            //Rebill Report Details show all orders on click
            List<int> TransactionId = null;
            switch (status)
            {

                case "Approved":
                    TransactionId = ordersTime.Where(o => o.Success == true).Select(o => o.Id).ToList();
                    ViewBag.Status = "Approved";
                    break;
                case "Declined":
                    TransactionId = ordersTime.Where(o => o.Success == false).Select(o => o.Id).ToList();
                    ViewBag.Status = "Declined";
                    break;
                default:
                    break;
            }

            Expression<Func<TransactionViaPrepaidCardQueue, bool>> filter = o => TransactionId.Contains(o.Id);

            var user = CurrentUsersProfile();
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

            var model = uow.TransactionViaPrepaidCardQueueRepository
                .GetPage(pageSize, pageNumber,
                //out count,
                filter: filter,
                orderBy: orderBy
                );

            var pageList = new PagedListMapper<TransactionViaPrepaidCardQueue>(model, model.CloneProps().ConvertUtcTime(TimeZoneUtility.GetTimeZone(uow.TimeZoneRepository.Find(user.TimeZoneId).Name)));
            return View("ViewPrepaidTransaction", pageList);
        }

        public ActionResult ViewTransactionQueueDetailsByMasterID(int Id, string sortOrder, int? page, int? display, string currentFilter, string searchString, bool? All, bool? Remaining, bool? Finished, string decline, string ProcessorTypeVal, string CardTypeVal, string status, string processDeclineSelected, string[] checkedItem)
        {
            var ordersTime = uow.TransactionQueueRepository.GetSet()

                            .Where(o => o.TransactionQueMasterId == Id).ToList();

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

            Func<IQueryable<TransactionQueue>, IOrderedQueryable<TransactionQueue>> orderBy = o =>
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
            Expression<Func<TransactionQueue, bool>> filter = f => true;
            Expression<Func<TransactionQueue, bool>> filterForSuccessDecline = f => true;

            if (!String.IsNullOrEmpty(searchString))
            {
                filter = f => f.PrepaidCard.Number == searchString;
            }

            if (Session["StatusValue"] != null && page != 0)
            {
                if (decline == null)
                {
                    decline = Session["StatusValue"].ToString();
                }
                else
                {
                    page = 1;
                }
               
            }

            if (decline == "1")
            {
                ViewBag.All = "checked";
                ViewBag.Remaining = "";
                ViewBag.Finished = "";
                Session["StatusValue"] = 1;
               

            }
            else if (decline == "2")
            {
                filterForSuccessDecline = f => f.finished == false;
                ViewBag.All = "";
                ViewBag.Remaining = "checked";
                ViewBag.Finished = "";
                Session["StatusValue"] = 2;
            }

            else if (decline == "3")
            {
                filterForSuccessDecline = f => f.finished == true;
                ViewBag.All = "";
                ViewBag.Remaining = "";
                ViewBag.Finished = "checked";
                Session["StatusValue"] = 3;
            }

            var user = CurrentUsersProfile();
            int pageSize = user.OrderDisplay;
            if (display.HasValue && display != pageSize)
            {
                page = 1;
                pageSize = display.Value;


                user.OrderDisplay = pageSize;
                uow.UserProfileRepository.Update(user);
                uow.Save(wsw.CurrentUserName);

            }
            
            Expression<Func<TransactionQueue, bool>> filter2 = f => f.TransactionQueMasterId == Id;
            var  lambda = filter2.AndAlso(filter).AndAlso(filterForSuccessDecline);
            ViewBag.TransactionQueMasterId = Id;
            ViewBag.Display = pageSize;

            int pageNumber = (page ?? 1);

            var model = uow.TransactionQueueRepository
                .GetPage(pageSize, pageNumber,
                //out count,
                filter: lambda,
                orderBy: orderBy
                );

            var pageList = new PagedListMapper<TransactionQueue>(model, model.CloneProps().ConvertUtcTime(TimeZoneUtility.GetTimeZone(uow.TimeZoneRepository.Find(user.TimeZoneId).Name)));
            return View("ViewTransactionQueueDetailsByMasterID", pageList);
        }


        public ActionResult DeleteTransactionQueueMaster(int id)
        {
            var TransactionMasterID = uow.TransactionQueueMasterRepository.Find(id);
            if (TransactionMasterID == null)
            {
                return HttpNotFound();
            }

            var TransactionQueuelist = uow.TransactionQueueRepository.Get().Where(a => a.TransactionQueMasterId == id).ToList();
            if (TransactionQueuelist == null)
            {
                return HttpNotFound();
            }

            if (TransactionQueuelist != null)
            {
                foreach (var TQ in TransactionQueuelist)
                {
                    uow.TransactionQueueRepository.Delete(TQ.Id);
                    uow.Save(wsw.CurrentUserName);
                }
            }

            uow.TransactionQueueMasterRepository.Delete(TransactionMasterID);
            uow.Save(wsw.CurrentUserName);

            return RedirectToAction("TransactionQueueMasters");
        }

        public ActionResult EditTransactionQueueMaster(int id)
        {
            var TransactionQueueMasterID = uow.TransactionQueueMasterRepository.Find(id);
            if (TransactionQueueMasterID == null)
            {
                return HttpNotFound();
            }

            if (TransactionQueueMasterID.finished == true)
            {
                TransactionQueueMasterID.finished = false;
            }
            else
            {
                TransactionQueueMasterID.finished = true;
            }

            uow.TransactionQueueMasterRepository.Update(TransactionQueueMasterID);
            uow.Save(wsw.CurrentUserName);

            return RedirectToAction("TransactionQueueMasters");
        }
        public ActionResult TransactionQueueMasters(int? id, string sortOrder, string currentFilter, string searchString, int? page, int? display)
        {
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
            ViewBag.NoOfTransactionsSortParm = sortOrder == "NoOfTransactions" ? "NoOfTransactions_desc" : "NoOfTransactions";
            ViewBag.AmountSortParm = sortOrder == "Amount" ? "Amount_desc" : "Amount";
            ViewBag.ProcessorIdSortParm = sortOrder == "ProcessorId" ? "ProcessorId_desc" : "ProcessorId";
            ViewBag.CardTypeSortParm = sortOrder == "CardType" ? "CardType_desc" : "CardType";
            ViewBag.RemainingTransactionsSortParm = sortOrder == "RemainingTransactions" ? "RemainingTransactions_desc" : "RemainingTransactions";
            ViewBag.DateSortParm = sortOrder == "Date" ? "Date_desc" : "Date";
            ViewBag.finishedSortParm = sortOrder == "firstname" ? "finished_desc" : "finished";

            ViewBag.idTQMIcon = "sort";
            ViewBag.nooftransactionTQMIcon = "sort";
            ViewBag.amountTQMIcon = "sort";
            ViewBag.processoridTQMIcon = "sort";
            ViewBag.cardtypeTQMIcon = "sort";
            ViewBag.remainingtransactionsTQMIcon = "sort";
            ViewBag.dateTQMIcon = "sort";
            ViewBag.finishedTQMIcon = "sort";

            Func<IQueryable<TransactionQueueMaster>, IOrderedQueryable<TransactionQueueMaster>> orderBy = o =>
            {
                switch (sortOrder)
                {
                    case "id_asc":
                        ViewBag.idTQMIcon = "sorting_asc";
                        return o.OrderBy(c => c.Id);

                    case "NoOfTransactions":
                        ViewBag.nooftransactionTQMIcon = "sorting_asc";
                        return o.OrderBy(c => c.NoOfTransactions).ThenByDescending(c => c.Id);
                    case "NoOfTransactions_desc":
                        ViewBag.nooftransactionTQMIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.NoOfTransactions).ThenByDescending(c => c.Id);

                    case "Amount":
                        ViewBag.amountTQMIcon = "sorting_asc";
                        return o.OrderBy(c => c.Amount).ThenByDescending(c => c.Id);
                    case "Amount_desc":
                        ViewBag.amountTQMIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Amount).ThenByDescending(c => c.Id);

                    case "ProcessorId":
                        ViewBag.processoridTQMIcon = "sorting_asc";
                        return o.OrderBy(c => c.Processor.Name).ThenByDescending(c => c.Id);
                    case "ProcessorId_desc":
                        ViewBag.processoridTQMIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Processor.Name).ThenByDescending(c => c.Id);

                    case "CardType":
                        ViewBag.cardtypeTQMIcon = "sorting_asc";
                        return o.OrderBy(c => c.CardType).ThenByDescending(c => c.Id);
                    case "CardType_desc":
                        ViewBag.cardtypeTQMIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.CardType).ThenByDescending(c => c.Id);

                    case "RemainingTransactions":
                        ViewBag.remainingtransactionsTQMIcon = "sorting_asc";
                        return o.OrderBy(c => c.RemainingTransactions).ThenByDescending(c => c.Id);
                    case "RemainingTransactions_desc":
                        ViewBag.remainingtransactionsTQMIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.RemainingTransactions).ThenByDescending(c => c.Id);

                    case "Date":
                        ViewBag.dateTQMIcon = "sorting_asc";
                        return o.OrderBy(c => c.Date).ThenByDescending(c => c.Id);
                    case "Date_desc":
                        ViewBag.dateTQMIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Date).ThenByDescending(c => c.Id);

                    case "finished":
                        ViewBag.finishedTQMIcon = "sorting_asc";
                        return o.OrderBy(c => c.finished).ThenByDescending(c => c.Id);
                    case "finished_desc":
                        ViewBag.finishedTQMIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.finished).ThenByDescending(c => c.Id);

                    default:
                        ViewBag.idOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Id);
                }
            };


            ViewBag.CurrentFilter = searchString;


            string searchStringType = "";
            decimal searchDecimalType = 0;
            int searchIntType = 0;
            Expression<Func<TransactionQueueMaster, bool>> filter = f => true;

            if (!String.IsNullOrEmpty(searchString))
        {
                if (Regex.IsMatch(searchString, @"^[0-9]+$") == true)
                {
                    searchIntType = Convert.ToInt32(searchString);
                }
                else if (Regex.IsMatch(searchString, @"[\d]{1,4}([.,][\d]{1,2})?") == true)
                {
                    searchDecimalType = Convert.ToDecimal(searchString);
                }
                else
                {
                    searchStringType = searchString;
                }

            }

            if (!String.IsNullOrEmpty(searchString))
            {
                if (searchStringType != "" && searchStringType != null)
                {
                    filter = f => f.Processor.Name.Contains(searchStringType)
                        || f.CardType.Contains(searchStringType);
                }
                if (searchIntType != 0)
                {
                    filter = f => f.NoOfTransactions == searchIntType
                 || f.RemainingTransactions == searchIntType;
                }
                if (searchDecimalType != 0)
                {
                    filter = f => f.Amount == searchDecimalType;
                }

            }


            Expression<Func<TransactionQueueMaster, bool>> filter2 = f => !id.HasValue || id == f.Id;
            var lambda = filter2.AndAlso(filter);

            ViewBag.Display = pageSize;

            int pageNumber = (page ?? 1);

            var model = uow.TransactionQueueMasterRepository
                .GetPage(pageSize, pageNumber,
                //out count,
                filter: lambda,
                orderBy: orderBy
                );

            var pageList = new PagedListMapper<TransactionQueueMaster>(model, model.CloneProps().ConvertUtcTime(TimeZoneUtility.GetTimeZone(uow.TimeZoneRepository.Find(user.TimeZoneId).Name)));
            return View(pageList);

        }

        public ActionResult PrepaidTransactionReport(string sortOrder, string currentFilter, string searchString, int? page, int? display, int? ProcessorId, bool? ProcessorType, bool? CardType)
        {
            var user = CurrentUsersProfile();
            int pageSize = user.RebillReportDisplay;

            if (display.HasValue && display != pageSize)
            {
                page = 1;
                pageSize = display.Value;
                user.RebillReportDisplay = pageSize;
                uow.UserProfileRepository.Update(user);
                uow.Save(wsw.CurrentUserName);
            }

            if (searchString != null)
                page = 1;
            else
                searchString = currentFilter;

            #region Columns
            ViewBag.ProcessorId = ProcessorId ?? 0;
            ViewBag.ProcessorType = ProcessorType ?? false;
            ViewBag.CardType = CardType ?? false;
            ViewBag.CurrentSort = sortOrder;

            ViewBag.ProcessorIdSortParm = sortOrder == "ProcessorId" ? "ProcessorId_desc" : "ProcessorId";
            ViewBag.ProcessorTypeSortParm = sortOrder == "ProcessorType" ? "ProcessorType_desc" : "ProcessorType";
            ViewBag.CardTypeSortParm = sortOrder == "CardType" ? "CardType_desc" : "CardType";

            ViewBag.ProcessorIdIcon = "sort";
            ViewBag.ProcessorTypeIcon = "sort";
            ViewBag.CardTypeIcon = "sort";


            #endregion

            #region Sort
            Func<IQueryable<TransactionReportModel>, IOrderedQueryable<TransactionReportModel>> orderBy = o =>
            {
                switch (sortOrder)
                {
                    case "ProcessorId_desc":
                        ViewBag.ProcessorIdIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.ProcessorId);

                    case "ProcessorType":
                        ViewBag.ProcessorTypeIcon = "sorting_asc";
                        return o.OrderBy(c => c.ProcessorType);
                    case "ProcessorType_desc":
                        ViewBag.ProcessorTypeIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.ProcessorType);


                    case "CardType":
                        ViewBag.CardTypeIcon = "sorting_asc";
                        return o.OrderBy(c => c.CardType);
                    case "CardType_desc":
                        ViewBag.CardTypeIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.CardType);

                    //case "NoOfTransactionsApproved":
                    //    ViewBag.NoOfTransactionsApprovedIcon = "sorting_asc";
                    //    return o.OrderBy(c => c.NoOfTransactionsApproved);
                    //case "NoOfTransactionsApproved_desc":
                    //    ViewBag.NoOfTransactionsApprovedIcon = "sorting_desc";
                    //    return o.OrderByDescending(c => c.NoOfTransactionsApproved);

                    //case "NoOfTransactionsDeclined":
                    //    ViewBag.NoOfTransactionsDeclinedIcon = "sorting_asc";
                    //    return o.OrderBy(c => c.NoOfTransactionsDeclined);
                    //case "NoOfTransactionsDeclined_desc":
                    //    ViewBag.NoOfTransactionsDeclinedIcon = "sorting_desc";
                    //    return o.OrderByDescending(c => c.NoOfTransactionsDeclined);

                    default:
                        ViewBag.ProcessorIdIcon = "sorting_asc";
                        return o.OrderBy(c => c.ProcessorId);
                }
            };
            #endregion

            ViewBag.CurrentFilter = searchString;
            int searchIntType = 0;
            Expression<Func<TransactionReportModel, bool>> filter = f => true;

            if (!String.IsNullOrEmpty(searchString))
            {
                if (Regex.IsMatch(searchString, @"^[0-9]+$") == true)
                {
                    searchIntType = Convert.ToInt32(searchString);
                }
                if (searchIntType != 0)
                {
                    filter = f => f.ProcessorId == searchIntType;
                }
                filter = f => f.ProcessorType.Contains(searchString)
                    || f.CardType.ToString().Contains(searchString)
                    ;

            }

            var query = from _event in uow.TransactionViaPrepaidCardQueueRepository.GetSet()
                        group _event by new
                        {
                            ProcessorId = ProcessorId == ProcessorId ? _event.ProcessorId : 0,
                            ProcessorType = ProcessorType == true ? _event.Processor.Name : string.Empty,
                            CardType = CardType == true ? _event.PrepaidCard.PaymentType : string.Empty,

                        } into od
                        select new TransactionReportModel
                        {
                            ProcessorId = od.Key.ProcessorId,
                            ProcessorType = od.Key.ProcessorType,
                            CardType = od.Key.CardType,
                            // initials
                            NoOfTransactionsApproved = od.Count(a => a.Success == true),
                            NoOfTransactionsDeclined = od.Count(a => a.Success == false),

                        };

            if (!String.IsNullOrEmpty(searchString))
            {
                if (Regex.IsMatch(searchString, @"^[0-9]+$") == true)
                {
                    if ((ProcessorType == true) && searchIntType != 0)
                    {
                        query = query.Where(a => a.ProcessorId == searchIntType);
                    }
                    else
                    {
                        query = query.Where(a => a.ProcessorType.Contains(searchString) || a.CardType.Contains(searchString));
                    }
                }
                else
                {
                    if ((ProcessorType == true || CardType == true))
                    {
                        query = query.Where(a => a.ProcessorType.Contains(searchString) || a.CardType.Contains(searchString));

                    }

                }
            }

            query = orderBy(query).ThenByDescending(c => c.ProcessorId)
                                   .ThenByDescending(c => c.ProcessorType)
                                     .ThenBy(c => c.CardType)
                                   ;

            int pageNumber = (page ?? 1);
            ViewBag.Display = pageSize;

            var report = query.ToPagedList(pageNumber, pageSize);
            return View(report);
        }

        /// <summary>
        /// Renders Customer Life Time Value Report
        /// </summary>
        /// <param name="from">From Date Filter for orders createdDate</param>
        /// <param name="to">To Date Filter for Orders Created</param>
        /// <returns></returns>
        public ActionResult CLV(DateTime? from, DateTime? to)
        {
            var categories = uow.CategoryRepository.Get();
            List<string> xAxis = new List<string>();
            List<string> yAxis = new List<string>();
            foreach (var item in categories)
            {

                var OrderProducts = (from products in uow.ProductRepository.Get(o => o.CategoryId == item.Id)
                                     join orderProducts in uow.OrderProductRepository.Get() on products.ProductId equals orderProducts.ProductId
                                     join orders in uow.OrderRepository.Get() on orderProducts.OrderId equals orders.OrderId
                                     select new
                                     {

                                         Cost = orderProducts.Cost,
                                         Price = orderProducts.Price,
                                         CustomerId = orders.CustomerId,
                                         CategoryId = item.Id,
                                         Created = orders.Created //Convert Date to UserTimeZone

                                     });
                if (from != null && to != null)
                {
                    ViewBag.to = to;
                    ViewBag.from = from;
                    //user timezone to utc
                    TimeZoneInfo userTimezoneInfo = TimeZoneUtility.GetTimeZone(uow.TimeZoneRepository.Find(uow.UserProfileRepository.Find(wsw.CurrentUserId).TimeZoneId).Name);
                    var ufdate = TimeZoneUtility.ToUtc((DateTime)from, userTimezoneInfo);
                    var utdate = TimeZoneUtility.ToUtc((DateTime)to, userTimezoneInfo);

                    OrderProducts = OrderProducts.Where(o => o.Created <= utdate && o.Created >= ufdate);

                }

                int noOfUniqueCustomers = OrderProducts.GroupBy(o => o.CustomerId).Count();
                decimal? Price = OrderProducts.Sum(o => (decimal)o.Price);
                decimal? Cost = OrderProducts.Sum(o => (decimal)o.Cost);

                decimal? clv = null;
                if (noOfUniqueCustomers != 0)
                    clv = (Price - Cost) / noOfUniqueCustomers;

                xAxis.Add(item.Name);
                if (clv == null)
                    yAxis.Add("0");
                else
                    yAxis.Add(clv.ToString());

            }

            if (from != null && to != null)
            {
                ViewBag.to = to;
                ViewBag.from = from;
            }
            Highcharts chart = new Highcharts("chart")
                 .InitChart(new Chart
                 {
                     DefaultSeriesType = ChartTypes.Column,
                     MarginRight = 130,
                     MarginBottom = 25,
                     ClassName = "chart"
                 })
                 .SetTitle(new Title
                 {
                     Text = "Customer Life Time Value",
                     X = -20
                 })
                 .SetXAxis(new XAxis { Categories = xAxis.ToArray() })
                 .SetYAxis(new YAxis
                 {
                     Title = new YAxisTitle { Text = "CLV" },
                     PlotLines = new[]
                                {
                                    new YAxisPlotLines
                                    {
                                        Value = 0,
                                        Width = 1,
                                        Color = ColorTranslator.FromHtml("#808080")
                                    }
                                },
                     Labels = new YAxisLabels { Formatter = "function() { return '$' + this.value.toFixed(2) }" }
                 })
                 .SetTooltip(new Tooltip
                 {
                     Formatter = @"function() {
                                                    return '<b>'+ this.x +'</b><br/>'+
                                                '$'+ this.y.toFixed(2);
                                            }"
                 })
                 .SetCredits(new Credits()
                 {
                     Enabled = false,
                 })
                 .SetLegend(new Legend
                 {
                     Layout = Layouts.Vertical,
                     Align = HorizontalAligns.Right,
                     VerticalAlign = VerticalAligns.Top,
                     X = -10,
                     Y = 100,
                     BorderWidth = 0
                 })
                 .SetSeries(new[]
                            {
                                new Series { Name = "CLV Value", Data = new Data(yAxis.ToArray<object>()) },
                                //new Series { Name = "New York", Data = new Data(ChartsData.NewYorkData) },
                                //new Series { Name = "Berlin", Data = new Data(ChartsData.BerlinData) },
                                //new Series { Name = "London", Data = new Data(ChartsData.LondonData) }
                            }
                 );


            return View(chart);
        }

        public ActionResult PartialOrderConversions(PartialToOrdersFilter filterModel)
        {

            //Filter Orders
            Expression<Func<Order, bool>> filter = o => o.isFromPartial == true
                                                        && (filterModel.AffiliateId == null || filterModel.AffiliateId == o.AffiliateId)
                                                        && (filterModel.SubId == null || filterModel.SubId == o.SubId)
                                                        && (!filterModel.ProductId.HasValue || o.OrderProducts.Select(p => p.ProductId).Contains(filterModel.ProductId.Value));


            if (filterModel.TimeZoneId == 0)
                filterModel.TimeZoneId = uow.UserProfileRepository.Find(wsw.CurrentUserId).TimeZoneId;

            TimeZoneInfo userTimezoneInfo = TimeZoneUtility.GetTimeZone(uow.TimeZoneRepository.Find(filterModel.TimeZoneId).Name);


            var orders = uow.OrderRepository.Get(filter);

            #region Filter On Category
            //Precedence whether filtering should be on categoryId or product Id
            if (filterModel.CategoryId.HasValue && !filterModel.ProductId.HasValue)
            {
                var Products = uow.ProductRepository.Get(o => o.CategoryId == filterModel.CategoryId.Value);
                //Orders Filtered on Category
                List<Order> filterOrders = new List<Order>();
                foreach (var item in Products)
                {
                    filterOrders.AddRange(orders.Where(o => o.OrderProducts.Select(p => p.ProductId).Contains(item.ProductId)));
                }
                orders = filterOrders;
            }

            #endregion

            var partialsGroupedByDay = (from partial in uow.PartialRepository.Get()
                                        group partial by new { date = new DateTime(partial.Created.Year, partial.Created.Month, partial.Created.Day) } into g
                                        select new
                                        {
                                            Date = g.Key.date,
                                            Count = g.Count()

                                        });

            var data = (from order in orders
                        group order by new { date = new DateTime(Convert.ToDateTime(order.PartialDate).Year, Convert.ToDateTime(order.PartialDate).Month, Convert.ToDateTime(order.PartialDate).Day) } into g
                        select new PartialToOrdersConversion()
                        {
                            Date = TimeZoneUtility.FromUtc((DateTime)g.Key.date, userTimezoneInfo),
                            PartialToOrders = g.Count(),
                            TotalPartials = partialsGroupedByDay.Where(o => o.Date == new DateTime(g.Key.date.Year, g.Key.date.Month, g.Key.date.Day)).Count() + g.Count(),
                            Conversion = (Convert.ToDouble(g.Count()) / Convert.ToDouble(partialsGroupedByDay.Where(o => o.Date == new DateTime(g.Key.date.Year, g.Key.date.Month, g.Key.date.Day)).Count() + g.Count())) * 100

                        });

            ViewBag.TimeZones = uow.TimeZoneRepository.Get().ToList();
            ViewBag.Data = data;

            return View(filterModel);
        }


        /// <summary>
        /// Get Chargeback list.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="affiliateId"></param>
        /// <param name="subId"></param>
        /// <param name="sortOrder"></param>
        /// <param name="page"></param>
        /// <param name="processors"></param>
        /// <returns></returns>
        public async Task<ActionResult> ChargebackReport(DateTime? start, DateTime? end, string affiliateId, string subId, string sortOrder, int? page, int? processors)
        {
            var user = uow.UserProfileRepository.Find(wsw.CurrentUserId);
            ViewBag.Processors = uow.ProcessorRepository.Get();

            Func<IQueryable<ChargeBackReport>, IOrderedQueryable<ChargeBackReport>> orderBy = o =>
            {
                return o.OrderByDescending(c => c.Processor);
            };

            end = end ?? DateTime.Today;
            start = start ?? DateTime.Today.AddDays(-30);

            var utdate = end.Value.AddDays(1);
            var ufdate = start.Value;

            ViewBag.end = end.Value.ToString("MM/dd/yyyy");
            ViewBag.start = start.Value.ToString("MM/dd/yyyy");

            //user timezone to utc

            TimeZoneInfo userTimezoneInfo = CurrentUsersTimezone();
            var offset = userTimezoneInfo.BaseUtcOffset.TotalMinutes; // daylightsaving will cause an hour difference!!
            ufdate = TimeZoneUtility.ToUtc(ufdate, userTimezoneInfo);
            utdate = TimeZoneUtility.ToUtc(utdate, userTimezoneInfo);

            var results = from p in uow.TransactionRepository.GetSet()
                       .Where(t => t.Type == TransactionType.Sale || t.Type == TransactionType.Capture || t.Type == TransactionType.Refund)
                       .Where(t => ufdate <= t.Date && t.Date < utdate)
                       .WhereIf(affiliateId != null && affiliateId != "", o => o.Order.AffiliateId == affiliateId)
                       .WhereIf(subId != null && subId != "", o => o.Order.SubId == subId)
                       .WhereIf(processors != null && processors != 0, o => o.ProcessorId == processors)
                          group p by p.Processor.Name into g
                          select new
                          {
                              ProcessorId = g.Key,
                              Processor = g.Select(m => m.Processor.Name),
                              Chargebacks = g.Where(m => m.Type == TransactionType.Refund).Count(),
                              TotalAmounts = g.Where(m => m.Type == TransactionType.Sale || m.Type == TransactionType.Capture).Count() > 1 ? g.Where(m => m.Type == TransactionType.Sale || m.Type == TransactionType.Capture).Sum(m => m.Amount) : 0,
                              ChargebackAmounts = g.Where(m => m.Type == TransactionType.Refund).Count() > 1 ? g.Where(m => m.Type == TransactionType.Refund).Sum(m => m.Amount) : 0,
                              TotalTransactions = g.Where(m => m.Type == TransactionType.Sale || m.Type == TransactionType.Capture).Count(),
                              ChargebacksRatio = g.Where(m => m.Type == TransactionType.Refund).Count() > 1 ? g.Where(m => m.Type == TransactionType.Refund).Count() * 100 / g.Where(m => m.Type == TransactionType.Sale || m.Type == TransactionType.Capture).Count() : 0
                          } into t
                          select new ChargeBackReport
                          {
                              Processor = t.ProcessorId,
                              Chargebacks = t.Chargebacks,
                              ChargebackAmounts = t.ChargebackAmounts,
                              TotalAmounts = t.TotalAmounts,
                              TotalTransactions = t.TotalTransactions,
                              ChargebacksRatio = t.ChargebacksRatio
                          };


            results = orderBy(results);


            int pageNumber = (page ?? 1);
            int pageSize = 100;

            var report = results.ToPagedList(pageNumber, pageSize);
            var pageList = new PagedListMapper<ChargeBackReport>(report, report.CloneProps().ConvertUtcTime(TimeZoneUtility.GetTimeZone(uow.TimeZoneRepository.Find(user.TimeZoneId).Name)));
            return View(pageList);
        }

        public ActionResult RebillForecast(DateTime? start, DateTime? end, string sortOrder, int? page, int? display, bool? ProductId, bool? AffiliateId, bool? SubId, RebillSearchOptionEnum? searchByDateOption)
        {

            end = end ?? DateTime.Today;
            start = start ?? DateTime.Today.AddDays(-7);
            searchByDateOption = searchByDateOption ?? RebillSearchOptionEnum.RebillDate;

            var utdate = end.Value.AddDays(1);
            var ufdate = start.Value;

            ViewBag.end = end.Value.ToString("MM/dd/yyyy");
            ViewBag.start = start.Value.ToString("MM/dd/yyyy");

            ViewBag.searchByDateOption = (int)searchByDateOption;

            var user = CurrentUsersProfile();
            int pageSize = user.RebillReportDisplay;

            if (display.HasValue && display != pageSize)
            {
                page = 1;
                pageSize = display.Value;

                user.RebillReportDisplay = pageSize;
                uow.UserProfileRepository.Update(user);
                uow.Save(wsw.CurrentUserName);
            }

            #region Columns

            ViewBag.ProductId = ProductId ?? false;
            ViewBag.SubId = SubId ?? false;
            ViewBag.AffiliateId = AffiliateId ?? false;

            ViewBag.CurrentSort = sortOrder;
            ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrder) ? "Date_asc" : "";
            ViewBag.ProductIdSortParm = sortOrder == "ProductId" ? "ProductId_desc" : "ProductId";
            ViewBag.AffiliateIdSortParm = sortOrder == "AffiliateId" ? "AffiliateId_desc" : "AffiliateId";
            ViewBag.SubIdSortParm = sortOrder == "SubId" ? "SubId_desc" : "SubId";

            #endregion

            #region Sort
            Func<IQueryable<RebillModel>, IOrderedQueryable<RebillModel>> _orderBy = o =>
            {
                switch (sortOrder)
                {
                    case "Date_asc":
                        ViewBag.DateOrderIcon = "sorting_asc";
                        return o.OrderBy(r => r.Year).ThenBy(r => r.Month).ThenBy(c => c.Day);

                    case "ProductId":
                        ViewBag.ProductIdOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.ProductId);
                    case "ProductId_desc":
                        ViewBag.ProductIdOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.ProductId);

                    case "AffiliateId":
                        ViewBag.AffiliateIdOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.AffiliateId);
                    case "AffiliateId_desc":
                        ViewBag.AffiliateIdOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.AffiliateId);

                    case "SubId":
                        ViewBag.SubIdOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.SubId);
                    case "SubId_desc":
                        ViewBag.SubIdOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.SubId);
                    default:
                        ViewBag.DateOrderIcon = "sorting_desc";
                        return o.OrderByDescending(r => r.Year).ThenByDescending(r => r.Month).ThenByDescending(r => r.Day);
                }
            };
            #endregion

            //user timezone to utc
            TimeZoneInfo userTimezoneInfo = CurrentUsersTimezone();
            var offset = userTimezoneInfo.BaseUtcOffset.TotalMinutes; // daylightsaving will cause an hour difference!!
            ufdate = TimeZoneUtility.ToUtc(ufdate, userTimezoneInfo);
            utdate = TimeZoneUtility.ToUtc(utdate, userTimezoneInfo);

            var query = from _event in uow.OrderProductRepository.GetSet().WhereIf(searchByDateOption == RebillSearchOptionEnum.RebillDate, o => ufdate <= o.Order.Created && o.Order.Created < utdate).WhereIf(searchByDateOption == RebillSearchOptionEnum.SignUpDate, o => ufdate <= o.Order.Created && o.Order.Created < utdate)
                        join _event1 in uow.OrderRepository.GetSet() on _event.OrderId equals _event1.OrderId
                        join _event2 in uow.ProductRepository.GetSet() on _event.NextProductId equals _event2.ProductId
                        group _event by new
                        {
                            Year = _event.Order.Created.Year,
                            //SqlFunctions.DateAdd("mi", offset, searchByDateOption == RebillSearchOptionEnum.RebillDate ? (_event.NextDate.HasValue ? _event.NextDate.Value : _event.Order.Created) : _event.Order.Created).Value.Year,
                            Day = _event.Order.Created.Day,
                            //SqlFunctions.DateAdd("mi", offset, searchByDateOption == RebillSearchOptionEnum.RebillDate ? (_event.NextDate.HasValue ? _event.NextDate.Value : _event.Order.Created) : _event.Order.Created).Value.Day,
                            Month = _event.Order.Created.Month,
                            // SqlFunctions.DateAdd("mi", offset, searchByDateOption == RebillSearchOptionEnum.RebillDate ? (_event.NextDate.HasValue ? _event.NextDate.Value : _event.Order.Created) : _event.Order.Created).Value.Month,

                            ProductId = ProductId == true ? _event.NextProductId : 0,
                            SubId = SubId == true ? _event1.SubId : string.Empty,
                            AffiliateId = AffiliateId == true ? _event1.AffiliateId : string.Empty,


                        } into od
                        select new RebillModel
                        {
                            Total = od.Sum(c =>c.ChildOrderId.HasValue? c.Product.Price * c.Quantity:0) - od.Sum(c =>c.ChildOrderId.HasValue? c.Quantity * c.RebillDiscount:0),
                            Numberoforder = od.Sum(c=>c.ChildOrderId.HasValue?c.Quantity:0),
                            Year = od.Key.Year,
                            Month = od.Key.Month,
                            Day = od.Key.Day,
                            ProductId = od.Key.ProductId,
                            AffiliateId = od.Key.AffiliateId,
                            SubId = od.Key.SubId,
                        };



            IList<RebillModel> list = query.ToList<RebillModel>();
            //if (list != null && list.Count > 0)
            //{
            //DateTime? mindate = list.Min(c => c.DateT).Date;
            //DateTime? maxDate= list.Max(c => c.DateT).Date;
            //int datediff = (end.Value - start.Value).Days;
            //if (start.HasValue)
            //{
            //    for (int i = 1; i < datediff; i++)
            //    {
            //        if (list.Where(c => c.DateT.ToShortDateString() == start.Value.AddDays(i).ToShortDateString()).Count() == 0)
            //        {
            //            list.Add(new RebillModel() { Day = start.Value.AddDays(i).Day, Year = start.Value.AddDays(i).Year, Month = start.Value.AddDays(i).Month });
            //        }
            //    }
            //}
            // }

            query = _orderBy(list.AsQueryable()).ThenByDescending(c => c.Year)
                                  .ThenByDescending(c => c.Month)
                                  .ThenByDescending(c => c.Day)
                                  .ThenByDescending(c => c.ProductId.Value)
                                  .ThenBy(c => c.AffiliateId)
                                  .ThenBy(c => c.SubId)
                                  ;



            int pageNumber = (page ?? 1);
            ViewBag.Display = pageSize;

            var report = query.ToPagedList(pageNumber, pageSize);


            return View(report);
        }

    }
}
