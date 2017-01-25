using KontinuityCRM.Helpers;
using KontinuityCRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace KontinuityCRM.Controllers
{
    [Authorize]
    public class KLogController : BaseController
    {
        //private readonly IUnitOfWork uow;
        //private readonly IWebSecurityWrapper wsw;

        public KLogController(IUnitOfWork uow, IWebSecurityWrapper wsw) : base(uow, wsw)
        {
            //this.uow = uow;
            //this.wsw = wsw;
        }

        // GET: KLog
        /// <summary>
        /// Returns to KLogs Listing View
        /// </summary>
        /// <param name="sortOrder">Sort order</param>
        /// <param name="currentFilter">selected filter</param>
        /// <param name="searchString">search string</param>
        /// <param name="page">page Number</param>
        /// <param name="display">Displaying Items No.</param>
        /// <returns></returns>
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? display)
        {
            var user = uow.UserProfileRepository.Find(wsw.CurrentUserId);
            int pageSize = user.KLogDisplay;

            if (display.HasValue && display != pageSize)
            {
                page = 1;
                pageSize = display.Value;

                user.KLogDisplay = pageSize;

                uow.UserProfileRepository.Update(user);
                uow.Save(wsw.CurrentUserName);
            }
            if (searchString != null)
                page = 1;
            else
                searchString = currentFilter;

            ViewBag.CurrentSort = sortOrder;
            ViewBag.TimeSortParm = string.IsNullOrEmpty(sortOrder) ? "time_asc" : "";
            ViewBag.UrlSortParm = sortOrder == "url" ? "url_desc" : "url";
            ViewBag.IpSortParm = sortOrder == "ip" ? "ip_desc" : "ip";

            ViewBag.ipOrderIcon = "sort";
            ViewBag.urlOrderIcon = "sort";
            ViewBag.timeOrderIcon = "sort";

            Func<IQueryable<KLog>, IOrderedQueryable<KLog>> orderBy = o =>
            {
                switch (sortOrder)
                {
                    case "time_asc":
                        ViewBag.timeOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Timestamp).ThenBy(c => c.Id);

                    case "ip":
                        ViewBag.ipOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.IPAddress).ThenByDescending(c => c.Id);
                    case "ip_desc":
                        ViewBag.ipOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.IPAddress).ThenByDescending(c => c.Id);

                    case "url":
                        ViewBag.urlOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Url).ThenByDescending(c => c.Id);
                    case "url_desc":
                        ViewBag.urlOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Url).ThenByDescending(c => c.Id);

                    default:
                        ViewBag.timeOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Timestamp).ThenBy(c => c.Id);
                }
            };


            ViewBag.CurrentFilter = searchString;

            Expression<Func<KLog, bool>> filter = f => true;

            if (!String.IsNullOrEmpty(searchString))
            {
                filter = f => f.Url.Contains(searchString)
                    //|| f.LastName.Contains(searchString)
                    //|| f.Address1.Contains(searchString)
                    //|| f.Address2.Contains(searchString)
                    //|| f.Country.Contains(searchString)
                    //|| f.IPAddress.Contains(searchString)
                    //|| f.Email.Contains(searchString)
                    //|| f.Phone.Contains(searchString)
                    ;
            }

            //Expression<Func<KLog, bool>> filter2 = f => f.LogDetails.Any();

            //var lambda = filter2.AndAlso(filter);

            ViewBag.Display = pageSize;

            int pageNumber = (page ?? 1);

            var model = uow.KLogRepository
                .GetPage(pageSize, pageNumber,
                //out count,
                filter: filter,
                orderBy: orderBy
                );

            return View(model);



            //return View(uow.KLogRepository.Get().OrderByDescending(k => k.Timestamp));
        }
        /// <summary>
        /// Delete By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(string id)
        {
            var klog = uow.KLogRepository.Find(Guid.Parse(id));
            if (klog == null)
            {
                return HttpNotFound();
            }
            uow.KLogRepository.Delete(klog);
            uow.Save();
            return RedirectToAction("index");
        }
    }
}