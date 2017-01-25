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
    [System.ComponentModel.DisplayName("Shipping Methods")]
    public class shippingController : BaseController
    {
        //
        // GET: /shipping/

        public shippingController(IUnitOfWork uow, IWebSecurityWrapper wsw)
            : base(uow, wsw)
        {

        }
        /// <summary>
        /// Redirects to View showing all the available shippingMethods
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="sortOrder"></param>
        /// <param name="currentFilter"></param>
        /// <param name="searchString"></param>
        /// <param name="page"></param>
        /// <param name="display"></param>
        /// <returns></returns>
        public ActionResult Index(int? categoryId, string sortOrder, string currentFilter, string searchString, int? page, int? display)
        {
            var user = uow.UserProfileRepository.Find(wsw.CurrentUserId);
            int pageSize = user.ShippingMethodDisplay;

            if (display.HasValue && display != pageSize)
            {
                page = 1;
                pageSize = display.Value;

                user.ShippingMethodDisplay = pageSize;

                uow.UserProfileRepository.Update(user);
                uow.Save(wsw.CurrentUserName);
            }
            if (searchString != null)
                page = 1;
            else
                searchString = currentFilter;

            ViewBag.CurrentSort = sortOrder;
            ViewBag.IdSortParm = String.IsNullOrEmpty(sortOrder) ? "id_asc" : "";
            ViewBag.NameSortParm = sortOrder == "name" ? "name_desc" : "name";
            ViewBag.categorySortParm = sortOrder == "category" ? "category_desc" : "category";
            ViewBag.priceSortParm = sortOrder == "price" ? "price_desc" : "price";
            ViewBag.recurringpriceSortParm = sortOrder == "recurringprice" ? "recurringprice_desc" : "recurringprice";

            ViewBag.recurringpriceOrderIcon = "sort";
            ViewBag.priceOrderIcon = "sort";
            ViewBag.categoryOrderIcon = "sort";
            ViewBag.nameOrderIcon = "sort";
            ViewBag.categoryOrderIcon = "sort";
            ViewBag.idOrderIcon = "sort";

            Func<IQueryable<ShippingMethod>, IOrderedQueryable<ShippingMethod>> orderBy = o =>
            {
                switch (sortOrder)
                {
                    case "id_asc":
                        ViewBag.idOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Id);

                    case "name":
                        ViewBag.nameOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Name).ThenByDescending(c => c.Id);
                    case "name_desc":
                        ViewBag.nameOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Name).ThenByDescending(c => c.Id);

                    case "category":
                        ViewBag.categoryOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.ShippingCategory.Name).ThenByDescending(c => c.Id);
                    case "category_desc":
                        ViewBag.categoryOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.ShippingCategory.Name).ThenByDescending(c => c.Id);

                    case "recurringprice":
                        ViewBag.recurringpriceOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.RecurringPrice).ThenByDescending(c => c.Id);
                    case "recurringprice_desc":
                        ViewBag.recurringpriceOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.RecurringPrice).ThenByDescending(c => c.Id);


                    case "price":
                        ViewBag.priceOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Price).ThenByDescending(c => c.Id);
                    case "price_desc":
                        ViewBag.priceOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Price).ThenByDescending(c => c.Id);


                    default:
                        ViewBag.idOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Id);
                }
            };


            ViewBag.CurrentFilter = searchString;

            Expression<Func<ShippingMethod, bool>> filter = f => true;

            if (!String.IsNullOrEmpty(searchString))
            {
                filter = f => f.Name.Contains(searchString)
                    || f.ShippingCategory.Name.Contains(searchString)
                    //|| f.Address1.Contains(searchString)
                    //|| f.Address2.Contains(searchString)
                    //|| f.Country.Contains(searchString)
                    //|| f.IPAddress.Contains(searchString)
                    //|| f.Type.Contains(searchString)
                    //|| f.Phone.Contains(searchString)
                    ;
            }


            Expression<Func<ShippingMethod, bool>> filter2 = f => !categoryId.HasValue || categoryId == f.ShippingCategoryId;

            var lambda = filter2.AndAlso(filter);



            ViewBag.Display = pageSize;

            int pageNumber = (page ?? 1);

            var model = uow.ShippingMethodRepository
                .GetPage(pageSize, pageNumber,
                //out count,
                filter: lambda,
                orderBy: orderBy
                );

            return View(model);

            //return View(
            //    uow.ShippingMethodRepository.Get
            //    //repo.ShippingMethods().Where
            //    (p => !CategoryId.HasValue || CategoryId.Value == p.ShippingCategoryId ));
        }

        private void CreateViewBag()
        {
            ViewBag.ShippingCategories = uow.ShippingCategoryRepository.Get(); // repo.ShippingCategories;
        }
        /// <summary>
        /// Redirects to create new ShippingMethod
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            CreateViewBag();
            return View();
        }

        //
        // POST: /shipping/Create
        /// <summary>
        /// post action to create ShippingMethod
        /// </summary>
        /// <param name="sm"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(ShippingMethod sm)
        {
            if (ModelState.IsValid)
            {
                //repo.AddShippingMethod(sm);
                uow.ShippingMethodRepository.Add(sm);
                uow.Save(wsw.CurrentUserName);
                return RedirectToAction("Index");
            }
            CreateViewBag();
            return View(sm);
        }

        //
        // GET: /shipping/Edit/5
        /// <summary>
        /// Redirect to edit page of ShippingMethod having id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            ShippingMethod sm = uow.ShippingMethodRepository.Find(id); //repo.FindShippingMethod(id);
            if (sm == null)
            {
                return HttpNotFound();
            }
            CreateViewBag();
            return View(sm);
        }

        //
        // POST: /shipping/Edit/5
        /// <summary>
        /// Post action to edit ShippingMethod
        /// </summary>
        /// <param name="sm"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(ShippingMethod sm)
        {
            if (ModelState.IsValid)
            {
                //repo.EditShippingMethod(sm);
                uow.ShippingMethodRepository.Update(sm);
                uow.Save(wsw.CurrentUserName);
                return RedirectToAction("Index");
            }
            CreateViewBag();
            return View(sm);
        }

        //
        // GET: /shipping/Delete/5
        /// <summary>
        /// Delete ShippingMethod By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int id)
        {
            var model = uow.ShippingMethodRepository.Find(id); //repo.FindShippingMethod(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            //repo.RemoveShippingMethod(model);
            uow.ShippingMethodRepository.Delete(model);
            uow.Save(wsw.CurrentUserName);
            return RedirectToAction("Index");
            //return View(model);
        }

        //
        // POST: /shipping/Delete/5

        //[HttpPost, ActionName("Delete")]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    var entity = repo.FindShippingMethod(id);
        //    repo.RemoveShippingMethod(entity);
        //    return RedirectToAction("Index");
        //}
    }
}
