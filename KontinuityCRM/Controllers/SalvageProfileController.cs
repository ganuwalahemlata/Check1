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
    [System.ComponentModel.DisplayName("Salvage Profiles")]
    public class SalvageProfileController : BaseController
    {
        public SalvageProfileController(IUnitOfWork uow, IWebSecurityWrapper wsw)
            : base(uow, wsw)
        {

        }

        /// <summary>
        /// Listing of salvageProfile
        /// </summary>
        /// <param name="sortOrder">sortOrder</param>
        /// <param name="currentFilter">selectedFilter</param>
        /// <param name="searchString">search string</param>
        /// <param name="page">pageNumber</param>
        /// <param name="display">display No</param>
        /// <param name="DeclineTypeId">DeclineType</param>
        /// <returns></returns>
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? display, int? DeclineTypeId)
        {
            var user = uow.UserProfileRepository.Find(wsw.CurrentUserId);
            int pageSize = user.SalvageProfileDisplay;

            if (display.HasValue && display != pageSize)
            {
                page = 1;
                pageSize = display.Value;

                user.SalvageProfileDisplay = pageSize;

                uow.UserProfileRepository.Update(user);
                uow.Save(wsw.CurrentUserName);
            }
            if (searchString != null)
                page = 1;
            else
                searchString = currentFilter;


            ViewBag.CurrentSort = sortOrder;
            ViewBag.IdSortParm = String.IsNullOrEmpty(sortOrder) ? "Id_asc" : "";
            ViewBag.NameSortParm = sortOrder == "Name" ? "Name_desc" : "Name";
            ViewBag.DeclineTypeSortParm = sortOrder == "DeclineType" ? "DeclineType_desc" : "DeclineType";
            ViewBag.BillTypeSortParm = sortOrder == "BillType" ? "BillType_desc" : "BillType";
            ViewBag.LowerPriceSortParm = sortOrder == "LowerPrice" ? "LowerPrice_desc" : "LowerPrice";

            ViewBag.LowerPriceOrderIcon = "sort";           
            ViewBag.BillTypeOrderIcon = "sort";           
            ViewBag.DeclineTypeOrderIcon = "sort";
            ViewBag.NameOrderIcon = "sort";           
            ViewBag.IdOrderIcon = "sort";

            Func<IQueryable<SalvageProfile>, IOrderedQueryable<SalvageProfile>> orderBy = o =>
            {
                switch (sortOrder)
                {
                    case "Id_asc":
                        ViewBag.IdOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Id);

                    case "Name":
                        ViewBag.NameOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Name).ThenByDescending(c => c.Id);
                    case "Name_desc":
                        ViewBag.NameOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Name).ThenByDescending(c => c.Id);

                    case "LowerPrice":
                        ViewBag.LowerPriceOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.LowerPrice).ThenByDescending(c => c.Id);
                    case "LowerPrice_desc":
                        ViewBag.LowerPriceOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.LowerPrice).ThenByDescending(c => c.Id);

                    case "BillType":
                        ViewBag.BillTypeOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.BillType).ThenByDescending(c => c.Id);
                    case "BillType_desc":
                        ViewBag.BillTypeOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.BillType).ThenByDescending(c => c.Id);

                    case "DeclineType":
                        ViewBag.DeclineTypeOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.DeclineType.Name).ThenByDescending(c => c.Id);
                    case "DeclineType_desc":
                        ViewBag.DeclineTypeOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.DeclineType.Name).ThenByDescending(c => c.Id);

                    default:
                        ViewBag.idOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Id);
                }
            };


            ViewBag.CurrentFilter = searchString;

            Expression<Func<SalvageProfile, bool>> filter = _ => true;

            if (!String.IsNullOrEmpty(searchString))
            {
                filter = f => f.Name.Contains(searchString)
                    //|| f.Name.Contains(searchString)
                    ;
            }

            Expression<Func<SalvageProfile, bool>> filter2 = f => !DeclineTypeId.HasValue || DeclineTypeId == f.DeclineTypeId;

            var lambda = filter2.AndAlso(filter);

            ViewBag.Display = pageSize;

            int pageNumber = (page ?? 1);

            var model = uow.SalvageProfileRepository
                .GetPage(pageSize, pageNumber,
                //out count,
                filter: lambda,
                orderBy: orderBy
                );

            return View(model);
        }
        /// <summary>
        /// View bage data
        /// </summary>
        private void CreateViewBag()
        {
            ViewBag.DeclineTypes = uow.DeclineTypeRepository.Get(orderBy: t => t.OrderBy(d => d.Name));
            ViewBag.SalvageProfiles = uow.SalvageProfileRepository.Get(orderBy: t => t.OrderBy(d => d.Name));
        }
        /// <summary>
        /// redirects to create view
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            CreateViewBag();
            return View(); // new SalvageProfile()
        }
        /// <summary>
        /// post action to create
        /// </summary>
        /// <param name="salvageProfile">salvageprofile</param>
        /// <param name="billday">BillDay</param>
        /// <param name="dayofweek">DayOfWeek</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(SalvageProfile salvageProfile, BillDay billday, DayOfWeek dayofweek)
        {
            if (ModelState.IsValid)
            {
                if (salvageProfile.BillType == BillType.ByDay)
                    salvageProfile.BillValue = KontinuityCRMHelper.GetBillValue(billday, dayofweek);

                if (!salvageProfile.LowerPrice)
                {
                    salvageProfile.LowerAmount = null;
                    salvageProfile.LowerPriceAfter = null;
                }

                uow.SalvageProfileRepository.Add(salvageProfile);
                uow.Save(wsw.CurrentUserName);
                return RedirectToAction("Index");
            }
            CreateViewBag();
            return View(salvageProfile);
        }

        /// <summary>
        /// Redirects to edit view
        /// </summary>
        /// <param name="id">Salvageprofile id</param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            var SalvageProfile = uow.SalvageProfileRepository.Find(id);
            if (SalvageProfile == null)
            {
                return HttpNotFound();
            }
            CreateViewBag();
            return View(SalvageProfile);
        }

        // POST: SalvageProfile/Edit/5
        /// <summary>
        /// post action to edit
        /// </summary>
        /// <param name="salvageProfile">SalvageProfile</param>
        /// <param name="billday">billday</param>
        /// <param name="dayofweek">dayOfWeek</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(SalvageProfile salvageProfile, BillDay billday, DayOfWeek dayofweek)
        {
            if (salvageProfile.LowerPrice == true && salvageProfile.LowerPercentage == null && salvageProfile.LowerAmount == null)
                ModelState.AddModelError("LowerAmount", "Provide values for Lower By Percentage/Amount.");

            if (salvageProfile.LowerPrice == true && salvageProfile.LowerPriceAfter == null)
                ModelState.AddModelError("LowerPriceAfter", "Provide values for Lower Price After.");

            if (ModelState.IsValid)
            {
                if (salvageProfile.BillType == BillType.ByDay)
                    salvageProfile.BillValue = KontinuityCRMHelper.GetBillValue(billday, dayofweek);

                if (!salvageProfile.LowerPrice)
                {
                    salvageProfile.LowerPercentage = null;
                    salvageProfile.LowerAmount = null;
                    salvageProfile.LowerPriceAfter = null;
                }

                uow.SalvageProfileRepository.Update(salvageProfile);
                uow.Save(wsw.CurrentUserName);
                return RedirectToAction("index");
            }
            CreateViewBag();
            return View(salvageProfile);
        }

        // GET: SalvageProfile/Delete/5
        /// <summary>
        /// Delete by id
        /// </summary>
        /// <param name="id">SalvageProfile id</param>
        /// <returns></returns>
        public ActionResult Delete(int id)
        {
            var salvageProfile = uow.SalvageProfileRepository.Find(id);
            if (salvageProfile == null)
            {
                return HttpNotFound();
            }
            uow.SalvageProfileRepository.Delete(salvageProfile);
            uow.Save(wsw.CurrentUserName);
            return RedirectToAction("index");
        }
    }
}