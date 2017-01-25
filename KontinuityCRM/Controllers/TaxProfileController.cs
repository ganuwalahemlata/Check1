using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KontinuityCRM.Helpers;
using KontinuityCRM.Models;
using System.Linq.Expressions;
using KontinuityCRM.Filters;

namespace KontinuityCRM.Controllers
{
    [CustomAuthorizeAttribute]
    [System.ComponentModel.DisplayName("Tax Profiles")]
    public class TaxProfileController : BaseController
    {
        public TaxProfileController(IUnitOfWork uow, IWebSecurityWrapper wsw) : base(uow, wsw)
        {

        }

        // GET: TaxProfile
        /// <summary>
        /// Redirects to all available TaxProfiles
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
            int pageSize = user.TaxProfileDisplay;

            if (display.HasValue && display != pageSize)
            {
                page = 1;
                pageSize = display.Value;

                user.TaxProfileDisplay = pageSize;

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
            //ViewBag.CountrySortParm = sortOrder == "Country" ? "Country_desc" : "Country";
            //ViewBag.TaxSortParm = sortOrder == "Tax" ? "Tax_desc" : "Tax";

            //ViewBag.CountryOrderIcon = "sort";
            ViewBag.NameOrderIcon = "sort";
            ViewBag.IdOrderIcon = "sort";
            //ViewBag.TaxOrderIcon = "sort";


            Func<IQueryable<TaxProfile>, IOrderedQueryable<TaxProfile>> orderBy = o =>
            {
                switch (sortOrder)
                {
                    case "Id_asc":
                        ViewBag.IdOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Id);

                    //case "Country":
                    //    ViewBag.CountryOrderIcon = "sorting_asc";
                    //    return o.OrderBy(c => c.Country.Name).ThenByDescending(c => c.Id);
                    //case "Country_desc":
                    //    ViewBag.CountryOrderIcon = "sorting_desc";
                    //    return o.OrderByDescending(c => c.Country.Name).ThenByDescending(c => c.Id);

                    case "Name":
                        ViewBag.NameOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Name).ThenByDescending(c => c.Id);
                    case "Name_desc":
                        ViewBag.NameOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Name).ThenByDescending(c => c.Id);

                    //case "Tax":
                    //    ViewBag.TaxOrderIcon = "sorting_asc";
                    //    return o.OrderBy(c => c.Tax).ThenByDescending(c => c.Id);
                    //case "Tax_desc":
                    //    ViewBag.TaxOrderIcon = "sorting_desc";
                    //    return o.OrderByDescending(c => c.Tax).ThenByDescending(c => c.Id);

                    default:
                        ViewBag.IdOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Id);
                }
            };


            ViewBag.CurrentFilter = searchString;

            Expression<Func<TaxProfile, bool>> filter = null;

            if (!String.IsNullOrEmpty(searchString))
            {
                filter = f => f.Name.Contains(searchString)
                    //|| f.Country.Name.Contains(searchString)
                    ;
            }



            ViewBag.Display = pageSize;

            int pageNumber = (page ?? 1);

            var model = uow.TaxProfileRepository
                .GetPage(pageSize, pageNumber,
                filter: filter,
                orderBy: orderBy
                );

            return View(model);
        }

        
        /// <summary>
        /// Redirects to create view of taxProfile
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            //CreateViewBag();
            return View();
        }

       /// <summary>
       /// Post Action to create new TaxPrfile
       /// </summary>
       /// <param name="taxProfile"></param>
       /// <returns></returns>
        [HttpPost]
        public ActionResult Create(TaxProfile taxProfile)
        {
            if (ModelState.IsValid)
            {
                uow.TaxProfileRepository.Add(taxProfile);
                uow.Save(wsw.CurrentUserName);
                return RedirectToAction("Index");
            }
            //CreateViewBag();
            return View(taxProfile);
        }

       /// <summary>
       /// Redirects to edit TaxProfile with Id
       /// </summary>
       /// <param name="id"></param>
       /// <returns></returns>
        public ActionResult Edit(int id)
        {
            var taxProfile = uow.TaxProfileRepository.Find(id);
            if (taxProfile == null)
            {
                return HttpNotFound();
            }
            //CreateViewBag();
            return View(taxProfile);
        }
        /// <summary>
        /// Post action to update TaxProfile
        /// </summary>
        /// <param name="taxProfile"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(TaxProfile taxProfile)
        {
            if (ModelState.IsValid)
            {
                uow.TaxProfileRepository.Update(taxProfile);
                uow.Save(wsw.CurrentUserName);
                return RedirectToAction("Index");
            }
            //CreateViewBag();
            return View(taxProfile);
        }
        /// <summary>
        /// Delete TaxProfile with Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int id) 
        {
            var taxProfile = uow.TaxProfileRepository.Find(id);
            if (taxProfile == null)
            {
                return HttpNotFound();
            }
            uow.TaxProfileRepository.Delete(taxProfile);
            uow.Save(wsw.CurrentUserName);
            return RedirectToAction("Index");
        }
    }
}