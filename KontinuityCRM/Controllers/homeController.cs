using DotNet.Highcharts;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Helpers;
using DotNet.Highcharts.Options;
using KontinuityCRM.Helpers;
using KontinuityCRM.Models;
using KontinuityCRM.Models.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace KontinuityCRM.Controllers
{
    [Authorize]
    [System.ComponentModel.DisplayName("Home")]
    public class homeController : BaseController
    {
        public homeController(IUnitOfWork uow, IWebSecurityWrapper wsw)
            : base(uow, wsw)
        {

        }

        public ActionResult index()
        {
            //Get Widgets
            Highcharts chart = new Highcharts("chart");
            return View(chart);
        }

        //public ActionResult Dashboard()
        //{
        //    return View();
        //}
        /// <summary>
        /// Create Report with given type
        /// </summary>
        /// <param name="reportType"></param>
        /// <returns></returns>
        public ActionResult Reports(int reportType)
        {
            ReportType Type = (ReportType)reportType;
            ReportingWidgets reportingWidgets = null;
            if (Type == ReportType.Rebill)
                reportingWidgets = CreateRebillReport();
            else if (Type == ReportType.CustomerLifeTime)
                reportingWidgets = customerLifeTimeValue();
            return Json(new { chart = reportingWidgets }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Add Reporting Widget 
        /// </summary>
        /// <param name="widget"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddReportingWidget(Widgets widget)
        {
            widget.CreatedBy = wsw.CurrentUserId;
            widget.CreatedOn = DateTime.Now;
            uow.WidgetsRepository.Add(widget);
            uow.Save();
            return Json(new { id = widget.Id }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get All Widgets from DB
        /// </summary>
        /// <returns></returns>

        public ActionResult GetAvailableWidgets()
        {
            var widgets = uow.WidgetsRepository.Get();
            List<ReportingWidgets> reportingWidgets = new List<ReportingWidgets>();
            foreach (var item in widgets)
            {

                if ((ReportType)item.Type == ReportType.Rebill)
                {
                    var report = CreateRebillReport();
                    report.id = item.Id;
                    report.Col_Pos = item.Col_Position;
                    report.Row_Pos = item.Row_Position;
                    reportingWidgets.Add(report);
                }
                else if ((ReportType)item.Type == ReportType.CustomerLifeTime)
                {

                    var report = customerLifeTimeValue();
                    report.id = item.Id;
                    report.Col_Pos = item.Col_Position;
                    report.Row_Pos = item.Row_Position;
                    reportingWidgets.Add(report);
                }
            }

            return Json(new { data = reportingWidgets }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Remove widget from db with given Id
        /// </summary>
        /// <param name="widgetId"></param>
        public void RemoveWidget(int widgetId)
        {
            uow.WidgetsRepository.Delete(widgetId);
            uow.Save();

        }

        /// <summary>
        /// Update Widget Positions in DB
        /// </summary>
        /// <param name="widgetId"></param>
        /// <param name="col_pos"></param>
        /// <param name="row_pos"></param>
        public void UpdateWidgetPosition(int widgetId, int col_pos, int row_pos)
        {

            var Widget = uow.WidgetsRepository.Find(widgetId);
            Widget.Col_Position = col_pos;
            Widget.Row_Position = row_pos;
            uow.Save();
        }
        /// <summary>
        /// Update All Widgets Position
        /// </summary>
        /// <param name="widgets"></param>
        [HttpPost]
        public void UpdateAllWidgets(List<Widgets> widgets)
        {
            foreach (var item in widgets)
            {
                var Widget = uow.WidgetsRepository.Find(item.Id);
                Widget.Col_Position = item.Col_Position;
                Widget.Row_Position = item.Row_Position;
            }

            uow.Save();
        }

        //Create Rebill Report
        private ReportingWidgets CreateRebillReport()
        {
            var today = DateTime.Today;
            var days = 7;
            var xAxis = new string[days];
            for (int i = 0; i < days; i++)
            {
                xAxis[i] = today.AddDays(i + 1 - days).ToString("MMM d");
            }

            TimeSpan time = new TimeSpan(0, 1 ,0, 0);
            DateTime combined = today.Add(time);
            var aweekago = combined.AddDays(1 - days);
            //var aweekago = today.AddDays(1 - days).ToUniversalTime();

            var transactions = uow.TransactionRepository.Get(t =>
                            aweekago <= t.Date 
                            && t.Type != Models.TransactionType.Void  // capture || sale || refund  
                            && t.Type != Models.TransactionType.Auth // disregard auth & void
                            && t.Success
                            && t.Status != Models.TransactionStatus.Void // not voided transactions (it doesnt' represent money)
                            ).Where(t=>t.Order.ParentId != null);
            

            //  (capture + sale) - refund // per date
            var tAmounts = from t in transactions
                           group t by t.Date.ToLocalTime().ToString("MMM d")
                               into td
                               select new
                               {
                                   Date = td.Key,
                                   Amount = td.Where(d => d.Type != Models.TransactionType.Refund).Sum(d => d.Amount)
                                            - td.Where(d => d.Type == Models.TransactionType.Refund).Sum(d => d.Amount)
                               };
            var dateAmounts = (from d in xAxis
                               join t in tAmounts on d equals t.Date into dt
                               from t in dt.DefaultIfEmpty()
                               select t == null ? 0 : t.Amount).Cast<object>().ToArray();
            
            ReportingWidgets rWidgets = new ReportingWidgets()
            {
                XAxis = xAxis,
                YAxis = dateAmounts,
                ChartType = "line",
                ChartTitle = "Rebill Report",
                SeriesName = "Amount"
            };

            return rWidgets;

        }

        /// <summary>
        /// Generic function to create report
        /// </summary>
        /// <param name="reportType"></param>
        /// <returns></returns>
        private ReportingWidgets CreateReport(ReportType reportType)
        {
            //Will be creating reports based on conditionS
            return CreateRebillReport();
        }
        /// <summary>
        /// Computes CLV against all
        /// </summary>
        public ReportingWidgets customerLifeTimeValue()
        {
            var categories = uow.CategoryRepository.Get();
            List<string> xAxis = new List<string>();
            ArrayList yAxis = new ArrayList();
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
                                         CategoryId = item.Id

                                     });
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
                    yAxis.Add(clv);
            }

            ReportingWidgets rWidgets = new ReportingWidgets()
            {
                XAxis = xAxis.ToArray(),
                YAxis = yAxis.ToArray(),
                ChartType = "column",
                ChartTitle = "Customer Life Time Value Report",
                SeriesName = "CLV"
            };
            return rWidgets;
        }
    }
}
