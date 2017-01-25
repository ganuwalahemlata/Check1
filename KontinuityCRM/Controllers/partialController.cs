using KontinuityCRM.Models;
using KontinuityCRM.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Transactions;
using System.Threading.Tasks;
using AutoMapper;
using System.Linq.Expressions;
using KontinuityCRM.Filters;
using PagedList;
using Quartz.Util;

namespace KontinuityCRM.Controllers
{
    [CustomAuthorizeAttribute]
    [System.ComponentModel.DisplayName("Partials")]
    public class partialController : BaseController
    {
        private readonly IMappingEngine mapper = null;
        public partialController(IUnitOfWork uow, IWebSecurityWrapper wsw, IMappingEngine mapper)
            : base(uow, wsw)
        {
            this.mapper = mapper;
        }
        /// <summary>
        /// Redirect to partial Listings View
        /// </summary>
        /// <param name="sortOrder">Sort Order</param>
        /// <param name="currentFilter">selected Filter</param>
        /// <param name="searchString">Search String</param>
        /// <param name="page">Page Number</param>
        /// <param name="display">Displaying Items No.</param>
        /// <returns></returns>
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? display)
        {
            var user = uow.UserProfileRepository.Find(wsw.CurrentUserId);
            int pageSize = user.PartialDisplay;
            //var redirect = false;
            if (display.HasValue && display != pageSize)
            {
                page = 1;
                pageSize = display.Value;

                user.PartialDisplay = pageSize;

                uow.UserProfileRepository.Update(user);
                uow.Save(wsw.CurrentUserName);
                //redirect = true;
            }
            if (searchString != null)
                page = 1;
            //redirect = true;
            else
                searchString = currentFilter;
            //if (redirect)
            //    return RedirectToAction("index", new { sortOrder = sortOrder, currentFilter = searchString });

            ViewBag.CurrentSort = sortOrder;
            ViewBag.IdSortParm = String.IsNullOrEmpty(sortOrder) ? "id_asc" : "";
            ViewBag.FullNameSortParm = sortOrder == "fullname" ? "fullname_desc" : "fullname";
            ViewBag.PhoneSortParm = sortOrder == "phone" ? "phone_desc" : "phone";
            ViewBag.EmailSortParm = sortOrder == "email" ? "email_desc" : "email";
            ViewBag.AddressSortParm = sortOrder == "address" ? "address_desc" : "address";
            ViewBag.IPSortParm = sortOrder == "ip" ? "ip_desc" : "ip";
            ViewBag.CountrySortParm = sortOrder == "country" ? "country_desc" : "country";
            ViewBag.createdSortParm = sortOrder == "created" ? "created_desc" : "created";
            ViewBag.lastupdateSortParm = sortOrder == "lastupdate" ? "lastupdate_desc" : "lastupdate";

            ViewBag.fullnameOrderIcon = "sort";
            ViewBag.phoneOrderIcon = "sort";
            ViewBag.addressOrderIcon = "sort";
            ViewBag.idOrderIcon = "sort";
            ViewBag.emailOrderIcon = "sort";
            ViewBag.ipOrderIcon = "sort";
            ViewBag.countryOrderIcon = "sort";
            ViewBag.createdOrderIcon = "sort";
            ViewBag.lastupdateOrderIcon = "sort";

            Func<IQueryable<Partial>, IOrderedQueryable<Partial>> orderBy = o =>
            {
                switch (sortOrder)
                {
                    case "id_asc":
                        ViewBag.idOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.PartialId);

                    case "phone":
                        ViewBag.phoneOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Phone).ThenByDescending(c => c.PartialId);
                    case "phone_desc":
                        ViewBag.phoneOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Phone).ThenByDescending(c => c.PartialId);

                    case "ip":
                        ViewBag.ipOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.IPAddress).ThenByDescending(c => c.PartialId);
                    case "ip_desc":
                        ViewBag.ipOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.IPAddress).ThenByDescending(c => c.PartialId);

                    case "country":
                        ViewBag.countryOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Country).ThenByDescending(c => c.PartialId);
                    case "country_desc":
                        ViewBag.countryOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Country).ThenByDescending(c => c.PartialId);

                    case "email":
                        ViewBag.emailOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Email).ThenByDescending(c => c.PartialId);
                    case "email_desc":
                        ViewBag.emailOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Email).ThenByDescending(c => c.PartialId);

                    case "fullname":
                        ViewBag.fullnameOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.FirstName).ThenByDescending(c => c.PartialId);
                    case "fullname_desc":
                        ViewBag.fullnameOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.FirstName).ThenByDescending(c => c.PartialId);

                    case "address":
                        ViewBag.addressOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Address1).ThenByDescending(c => c.PartialId);
                    case "address_desc":
                        ViewBag.addressOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Address1).ThenByDescending(c => c.PartialId);

                    case "created":
                        ViewBag.createdOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Created).ThenByDescending(c => c.PartialId);
                    case "created_desc":
                        ViewBag.createdOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Created).ThenByDescending(c => c.PartialId);

                    case "lastupdate":
                        ViewBag.lastupdateOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.LastUpdate).ThenByDescending(c => c.PartialId);
                    case "lastupdate_desc":
                        ViewBag.lastupdateOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.LastUpdate).ThenByDescending(c => c.PartialId);

                    default:
                        ViewBag.idOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.PartialId);
                }
            };

            ViewBag.CurrentFilter = searchString;

            Expression<Func<Partial, bool>> filter = null;

            if (!String.IsNullOrEmpty(searchString))
            {
                filter = f => f.FirstName.Contains(searchString)
                    || f.LastName.Contains(searchString)
                    || f.Address1.Contains(searchString)
                    || f.Address2.Contains(searchString)
                    || f.Country.Contains(searchString)
                    || f.IPAddress.Contains(searchString)
                    || f.Email.Contains(searchString)
                    || f.Phone.Contains(searchString)
                    ;
            }



            ViewBag.Display = pageSize;

            int pageNumber = (page ?? 1);

            var model = uow.PartialRepository
                .GetPage(pageSize, pageNumber,
                //out count,
                filter: filter,
                orderBy: orderBy
                );

            var pageList = new PagedListMapper<Partial>(model, model.CloneProps().ConvertUtcTime(TimeZoneUtility.GetTimeZone(uow.TimeZoneRepository.Find(user.TimeZoneId).Name)));
            return View(pageList);
        }
        /// <summary>
        /// Redirect to create partial View
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            CreateViewBag();
            return View();
        }

        private void CreateViewBag()
        {
            ViewBag.Products = uow.ProductRepository.Get(); //repo.Products();
            ViewBag.Countries = uow.CountryRepository.Get(orderBy: c => c.OrderBy(p => p.Name));
        }
        /// <summary>
        /// post action to create partial
        /// </summary>
        /// <param name="partial">Partial model</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Create(Partial partial)
        {
            if (string.IsNullOrEmpty(partial.IPAddress))
                partial.IPAddress = Request.UserHostAddress; // set the ip address if none is provided

            if (ModelState.IsValid)
            {
                await partial.Create(uow, wsw, mapper);
                return RedirectToAction("Index");
            }
            CreateViewBag();
            return View(partial);
        }
        /// <summary>
        /// Redirects to Edit View
        /// </summary>
        /// <param name="id">Partial Id</param>
        /// <returns></returns>

        public ActionResult Edit(int id)
        {
            Partial partial = uow.PartialRepository.Find(id);
            if (partial == null)
            {
                return HttpNotFound();
            }
            CreateViewBag();
            return View(partial);
        }

        //
        // POST: /category/Edit/5
        /// <summary>
        /// post action to update partial
        /// </summary>
        /// <param name="partial">Partial Model</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(Partial partial)
        {
            if (ModelState.IsValid)
            {

                partial.LastUpdate = System.DateTime.Now;

                uow.PartialRepository.Update(partial);
                uow.Save(wsw.CurrentUserName);
                return RedirectToAction("Index");
            }
            CreateViewBag();
            return View(partial);
        }

        public ActionResult Order(int id)
        {
            var partial = uow.PartialRepository.Find(id);

            if (partial == null)
            {
                return HttpNotFound();
            }

            ViewBag.Partial = partial;
            ViewBag.ShippingMethods = uow.ShippingMethodRepository.Get();
            ViewBag.Processors = uow.ProcessorRepository.Get();
            ViewBag.Products = uow.ProductRepository.Get();
            ViewBag.Countries = uow.CountryRepository.Get(orderBy: c => c.OrderBy(t => t.Name));

            var yarr = new List<SelectListItem>();

            for (int i = 0; i < 20; i++)
            {
                var year = DateTime.Today.Year + i;
                var y = new SelectListItem();
                y.Text = year.ToString();
                y.Value = year.ToString().Substring(2);
                yarr.Add(y);
            }

            ViewBag.CreditCardExpirationYears = yarr;

            return View();
        }
        /// <summary>
        /// post action to create Order
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Order(int id, KontinuityCRM.Models.APIModels.PartialToOrderModel model)
        {
            var partial = uow.PartialRepository.Find(id); // repo.FindPartial(id);

            if (model.OrderProducts == null || model.OrderProducts.Count == 0)
            {
                ModelState.AddModelError("", "At least one product is required");
            }

            if (ModelState.IsValid)
            {
                model.IPAddress = Request.UserHostAddress;
                var order = await partial.CreateOrder(uow, model, mapper, wsw);
                return RedirectToAction("index", "order");
            }

            // something went wrong to reach this point
            ViewBag.Partial = partial;
            ViewBag.ShippingMethods = uow.ShippingMethodRepository.Get();
            ViewBag.Processors = uow.ProcessorRepository.Get();
            ViewBag.Products = uow.ProductRepository.Get();
            ViewBag.Countries = uow.CountryRepository.Get(orderBy: c => c.OrderBy(t => t.Name));

            var yarr = new List<SelectListItem>();

            for (int i = 0; i < 20; i++)
            {
                var year = DateTime.Today.Year + i;
                var y = new SelectListItem();
                y.Text = year.ToString();
                y.Value = year.ToString().Substring(2);
                yarr.Add(y);
            }

            ViewBag.CreditCardExpirationYears = yarr;
            return View();
        }

        //
        // GET: /category/Delete/5
        /// <summary>
        /// Delete by id
        /// </summary>
        /// <param name="id">Partial Id</param>
        /// <returns></returns>
        public async Task<ActionResult> Delete(int id)
        {
            var partial = uow.PartialRepository.Find(id);
            if (partial == null)
            {
                return HttpNotFound();
            }
            await partial.Delete(uow, wsw, mapper);
            return RedirectToAction("Index");
        }

        //protected override void Dispose(bool disposing)
        //{
        //    repo.Dispose();
        //    base.Dispose(disposing);
        //}
    }
}
