using KontinuityCRM.Filters;
using KontinuityCRM.Helpers;
using KontinuityCRM.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace KontinuityCRM.Controllers
{
    [CustomAuthorizeAttribute]
    [DisplayName("Shipping Categories")] // used for the navigation & breadcrumbs
    public class ShippingCategoryController : BaseController
    {
        public ShippingCategoryController(IUnitOfWork uow, IWebSecurityWrapper wsw) : base (uow, wsw)
        {

        }
        /// <summary>
        /// Redirects to available ShippingCategory listings
        /// </summary>
        /// <param name="sortOrder">sortOrder</param>
        /// <param name="currentFilter">Filter selected</param>
        /// <param name="searchString">Search key</param>
        /// <param name="page">pageNumber</param>
        /// <param name="display">Display</param>
        /// <returns></returns>
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? display)
        {
            var user = uow.UserProfileRepository.Find(wsw.CurrentUserId);
            int pageSize = user.ShippingCategoryDisplay;

            if (display.HasValue && display != pageSize)
            {
                page = 1;
                pageSize = display.Value;

                user.ShippingCategoryDisplay = pageSize;

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
            ViewBag.codeSortParm = sortOrder == "code" ? "code_desc" : "code";
          
            ViewBag.nameOrderIcon = "sort";
            ViewBag.codeOrderIcon = "sort";
            ViewBag.idOrderIcon = "sort";

            Func<IQueryable<ShippingCategory>, IOrderedQueryable<ShippingCategory>> orderBy = o =>
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

                    case "code":
                        ViewBag.codeOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Code).ThenByDescending(c => c.Id);
                    case "code_desc":
                        ViewBag.codeOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Code).ThenByDescending(c => c.Id);

                    default:
                        ViewBag.idOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Id);
                }
            };


            ViewBag.CurrentFilter = searchString;

            Expression<Func<ShippingCategory, bool>> filter = null;

            if (!String.IsNullOrEmpty(searchString))
            {
                filter = f => f.Name.Contains(searchString)
                    || f.Code.Contains(searchString)
                    //|| f.Address1.Contains(searchString)
                    //|| f.Address2.Contains(searchString)
                    //|| f.Country.Contains(searchString)
                    //|| f.IPAddress.Contains(searchString)
                    //|| f.Type.Contains(searchString)
                    //|| f.Phone.Contains(searchString)
                    ;
            }


            //Expression<Func<EmailTemplate, bool>> filter2 = f => !id.HasValue || id == f.Id;

            //var lambda = filter2.AndAlso(filter);



            ViewBag.Display = pageSize;

            int pageNumber = (page ?? 1);

            var model = uow.ShippingCategoryRepository
                .GetPage(pageSize, pageNumber,
                //out count,
                filter: filter,
                orderBy: orderBy
                );

            return View(model);
            //return View(uow.ShippingCategoryRepository.Get());
        }        
        /// <summary>
        /// Redirects to create ShippingCategory View
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /ShippingCategory/Create
        /// <summary>
        /// post action to create ShippingCategory
        /// </summary>
        /// <param name="shippingcategory"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(ShippingCategory shippingcategory)
        {
            if (ModelState.IsValid)
            {
                shippingcategory.CreatedUserId = wsw.CurrentUserId;
               
                uow.ShippingCategoryRepository.Add(shippingcategory);
                uow.Save(wsw.CurrentUserName);
                return RedirectToAction("Index");
            }
            return View(shippingcategory);
        }

        //
        // GET: /ShippingCategory/Edit/5
        /// <summary>
        /// Redirects to edit view 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            var sc = uow.ShippingCategoryRepository.Find(id);
            if (sc == null)
            {
                return HttpNotFound();
            }
            return View(sc);
        }

        //
        // POST: /ShippingCategory/Edit/5
        /// <summary>
        /// post acion to update ShippingCategory
        /// </summary>
        /// <param name="sc"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(ShippingCategory sc)
        {
            if (ModelState.IsValid)
            {
                sc.LastUpdate = DateTime.UtcNow;
                sc.UpdatedUserId = wsw.CurrentUserId;
                                
                uow.ShippingCategoryRepository.Update(sc);
                uow.Save(wsw.CurrentUserName);
                return RedirectToAction("Index");
            }
            return View(sc);
        }

        //
        // GET: /ShippingCategory/Delete/5
        /// <summary>
        /// Delete ShippingCategory by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int id)
        {
            var sc = uow.ShippingCategoryRepository.Find(id);
            if (sc == null)
            {
                return HttpNotFound();
            }
            
            uow.ShippingCategoryRepository.Delete(sc);
            uow.Save(wsw.CurrentUserName);
            return RedirectToAction("Index");
        }

       
    }
}
