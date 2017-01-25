using KontinuityCRM.Filters;
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
    [CustomAuthorizeAttribute]
    [System.ComponentModel.DisplayName("Tax Rules")]
    public class TaxRuleController : BaseController
    {
        public TaxRuleController(IUnitOfWork uow, IWebSecurityWrapper wsw)
            : base(uow, wsw)
        {

        }
        /// <summary>
        /// Create view Bag Data
        /// </summary>
        private void CreateViewBag()
        {
            ViewBag.Countries = uow.CountryRepository.Get(orderBy: o => o.OrderBy(c => c.Name));
            ViewBag.Locations = uow.LocationRepository.Get();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">The tax profile id</param>
        /// <param name="sortOrder"></param>
        /// <param name="currentFilter"></param>
        /// <param name="searchString"></param>
        /// <param name="page"></param>
        /// <param name="display"></param>
        /// <returns></returns>
        public ActionResult Index(int id, string sortOrder, string currentFilter, string searchString, int? page, int? display)
        {
            var taxProfile = uow.TaxProfileRepository.Find(id);
            if (taxProfile == null)
            {
                return HttpNotFound();
            }
            ViewBag.TaxProfile = taxProfile;

            var user = uow.UserProfileRepository.Find(wsw.CurrentUserId);
            int pageSize = user.TaxRuleDisplay;

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
            ViewBag.ProvinceSortParm = sortOrder == "Province" ? "Province_desc" : "Province";
            ViewBag.CountrySortParm = sortOrder == "Country" ? "Country_desc" : "Country";
            ViewBag.TaxSortParm = sortOrder == "Tax" ? "Tax_desc" : "Tax";
            ViewBag.PostalCodeSortParm = sortOrder == "PostalCode" ? "PostalCode_desc" : "PostalCode";
            ViewBag.CitySortParm = sortOrder == "City" ? "City_desc" : "City";

            ViewBag.CityOrderIcon = "sort";
            ViewBag.CountryOrderIcon = "sort";
            ViewBag.ProvinceOrderIcon = "sort";
            ViewBag.IdOrderIcon = "sort";
            ViewBag.TaxOrderIcon = "sort";
            ViewBag.PostalCodeOrderIcon = "sort";

            Func<IQueryable<TaxRule>, IOrderedQueryable<TaxRule>> orderBy = o =>
            {
                switch (sortOrder)
                {
                    case "Id_asc":
                        ViewBag.IdOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Id);

                    case "Country":
                        ViewBag.CountryOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Country.Name).ThenByDescending(c => c.Id);
                    case "Country_desc":
                        ViewBag.CountryOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Country.Name).ThenByDescending(c => c.Id);

                    case "Province":
                        ViewBag.ProvinceOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Province).ThenByDescending(c => c.Id);
                    case "Province_desc":
                        ViewBag.ProvinceOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Province).ThenByDescending(c => c.Id);

                    case "Tax":
                        ViewBag.TaxOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Tax).ThenByDescending(c => c.Id);
                    case "Tax_desc":
                        ViewBag.TaxOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Tax).ThenByDescending(c => c.Id);

                    case "PostalCode":
                        ViewBag.PostalCodeOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.PostalCode).ThenByDescending(c => c.Id);
                    case "PostalCode_desc":
                        ViewBag.PostalCodeOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.PostalCode).ThenByDescending(c => c.Id);

                    case "City":
                        ViewBag.CityOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.City).ThenByDescending(c => c.Id);
                    case "City_desc":
                        ViewBag.CityOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.City).ThenByDescending(c => c.Id);

                    default:
                        ViewBag.IdOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Id);
                }
            };


            ViewBag.CurrentFilter = searchString;

            Expression<Func<TaxRule, bool>> filter2 = f => id == f.TaxProfileId;

            Expression<Func<TaxRule, bool>> filter = _ => true;

            if (!String.IsNullOrEmpty(searchString))
            {
                filter = f => f.Province.Contains(searchString)
                    || f.PostalCode.Contains(searchString)
                    || f.Country.Name.Contains(searchString)
                    || f.City.Contains(searchString)
                    ;
            }

            var lambda = filter2.AndAlso(filter);

            ViewBag.Display = pageSize;

            int pageNumber = (page ?? 1);

            var model = uow.TaxRuleRepository
                .GetPage(pageSize, pageNumber,
                filter: lambda,
                orderBy: orderBy
                );

            return View(model);

        }
        /// <summary>
        /// Redirects to create taxRule View
        /// </summary>
        /// <param name="id">TaxRule Id</param>
        /// <returns></returns>
        public ActionResult Create(int id)
        {
            var taxProfile = uow.TaxProfileRepository.Find(id);
            if (taxProfile == null)
            {
                return HttpNotFound();
            }
            ViewBag.TaxProfile = taxProfile;
            CreateViewBag();
            return View();
        }

        /// <summary>
        /// Post action to create taxRule
        /// </summary>
        /// <param name="taxRule">TaxRule model</param>
        /// <param name="id">taxRule Id</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(TaxRule taxRule, int id)
        {
            if (ModelState.IsValid)
            {
                uow.TaxRuleRepository.Add(taxRule);
                uow.Save(wsw.CurrentUserName);
                return RedirectToAction("Index", new { id = taxRule.TaxProfileId });
            }
            ViewBag.TaxProfile = uow.TaxProfileRepository.Find(id);
            CreateViewBag();
            return View(taxRule);
        }

        /// <summary>
        /// Redirects to Edit View
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            var taxRule = uow.TaxRuleRepository.Find(id);
            if (taxRule == null)
            {
                return HttpNotFound();
            }
            ViewBag.TaxProfile = taxRule.TaxProfile;
            CreateViewBag();
            return View(taxRule);
        }
        /// <summary>
        /// Post Action to update TaxRule
        /// </summary>
        /// <param name="taxRule"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(TaxRule taxRule)
        {
            if (ModelState.IsValid)
            {
                uow.TaxRuleRepository.Update(taxRule);
                uow.Save(wsw.CurrentUserName);
                return RedirectToAction("Index", new { id = taxRule.TaxProfileId });
            }
            ViewBag.TaxProfile = uow.TaxProfileRepository.Find(taxRule.TaxProfileId);
            CreateViewBag();
            return View(taxRule);
        }
        /// <summary>
        /// Edit TaxRule by id
        /// </summary>
        /// <param name="id">TaxRule Id</param>
        /// <returns></returns>
        public ActionResult Delete(int id)
        {
            var taxRule = uow.TaxRuleRepository.Find(id);
            if (taxRule == null)
            {
                return HttpNotFound();
            }
            uow.TaxRuleRepository.Delete(taxRule);
            uow.Save(wsw.CurrentUserName);
            return RedirectToAction("Index", new { id = taxRule.TaxProfileId });
        }
    }
}