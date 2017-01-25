using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KontinuityCRM.Helpers;
using KontinuityCRM.Models;
using KontinuityCRM.Models.ViewModels;
using System.Linq.Expressions;
using KontinuityCRM.Filters;

namespace KontinuityCRM.Controllers
{
    [CustomAuthorizeAttribute]
    [System.ComponentModel.DisplayName("Categories")]
    public class categoryController : BaseController
    {
        public categoryController(IUnitOfWork uow, IWebSecurityWrapper wsw) : base(uow, wsw)
        {

        }

        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? display)
        {
            //int count; 

            var user = uow.UserProfileRepository.Find(wsw.CurrentUserId);
            int pageSize = user.CategoryDisplay;
            //var redirect = false;

            if (display.HasValue && display != pageSize)
            {
                page = 1;
                pageSize = display.Value;

                //if (user.CategoryDisplay != pageSize)
                //{
                user.CategoryDisplay = pageSize;
                uow.UserProfileRepository.Update(user);
                uow.Save(wsw.CurrentUserName);
                //}
                //redirect = true;
                //return RedirectToAction("index", new { sortOrder = sortOrder, currentFilter = currentFilter, searchString = searchString });
            }

            if (searchString != null)
            {
                page = 1;
                //redirect = true;
                
            }
            else {
                searchString = currentFilter;
            }
            //if (redirect)
            //{
            //    return RedirectToAction("index", new { sortOrder = sortOrder, currentFilter = searchString });
            //}
            
            ViewBag.CurrentFilter = searchString;

            ViewBag.CurrentSort = sortOrder;
            ViewBag.IdSortParm = String.IsNullOrEmpty(sortOrder) ? "id_asc" : "";
            ViewBag.NameSortParm = sortOrder == "name" ? "name_desc" : "name";
            ViewBag.DescriptionSortParm = sortOrder == "description" ? "description_desc" : "description";
            ViewBag.ProductsSortParm = sortOrder == "products" ? "products_desc" : "products";

            ViewBag.nameOrderIcon = "sort";
            ViewBag.descriptionOrderIcon = "sort";
            ViewBag.productsOrderIcon = "sort";
            ViewBag.idOrderIcon = "sort";

            Func<IQueryable<Category>, IOrderedQueryable<Category>> orderBy = o =>
            {
                switch (sortOrder)
                {
                    case "id_asc":
                        ViewBag.idOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Id);

                    case "description":
                        ViewBag.descriptionOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Description).ThenByDescending(c => c.Id);
                    case "description_desc":
                        ViewBag.descriptionOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Description).ThenByDescending(c => c.Id);
                    case "products":
                        ViewBag.productsOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Products.Count()).ThenByDescending(c => c.Id);
                    case "products_desc":
                        ViewBag.productsOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Products.Count()).ThenByDescending(c => c.Id);

                    case "name":
                        ViewBag.nameOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Name).ThenByDescending(c => c.Id);
                    case "name_desc":
                        ViewBag.nameOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Name).ThenByDescending(c => c.Id);

                    default:
                        ViewBag.idOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Id);
                }
            };


            

            Expression<Func<Category, bool>> filter = null;
            
            if (!String.IsNullOrEmpty(searchString))
            {
                filter = f => f.Name.Contains(searchString) || f.Description.Contains(searchString);
            }
                        

            ViewBag.Display = pageSize;

            int pageNumber = (page ?? 1);

            var categories = uow.CategoryRepository
                .GetPage(pageSize, pageNumber, 
                //out count,
                filter: filter, 
                orderBy: orderBy
                );

            

            //ViewBag.Count = count;

            return View(categories);
        }
        
        

        //
        // GET: /category/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /category/Create

        [HttpPost]
        public ActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                uow.CategoryRepository.Add(category);
                uow.Save(wsw.CurrentUserName);
                return RedirectToAction("Index");
            }

            return View(category);
        }

        //
        // GET: /category/Edit/5

        public ActionResult Edit(int id)
        {
            Category category = uow.CategoryRepository.Find(id); //repo.FindCategory(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        //
        // POST: /category/Edit/5

        [HttpPost]
        public ActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                uow.CategoryRepository.Update(category);
                uow.Save(wsw.CurrentUserName);
                return RedirectToAction("Index");
            }
            return View(category);
        }

        //
        // GET: /category/Delete/5

        public ActionResult Delete(int id)
        {
            Category category = uow.CategoryRepository.Find(id); 
            if (category == null)
            {
                return HttpNotFound();
            }           
            uow.CategoryRepository.Delete(category);
            uow.Save(wsw.CurrentUserName);
            return RedirectToAction("Index");
            
        }

        //
        // POST: /category/Delete/5

        //[HttpPost, ActionName("Delete")]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Category category = repo.FindCategory(id);
        //    repo.RemoveCategory(category);
        //    return RedirectToAction("Index");
        //}

        //public ActionResult Products(int id)
        //{
        //    Category category = repo.FindCategory(id);
        //    if (category == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.CategoryName = category.Name;
        //    return View(repo.ProductCategories().Where(pc => pc.CategoryId == id));
        //}



        //public ActionResult AddProduct(int id)
        //{
        //    Category category = repo.FindCategory(id);
        //    if (category == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.CategoryName = category.Name;
        //    var pclist = repo.ProductCategories().Where(pc => pc.CategoryId == id).Select(pc => pc.ProductId);
        //    ViewBag.Products = repo.Products().Where(p => !pclist.Contains(p.ProductId));
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult AddProduct(IntViewModel model, int id)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        repo.AddProductCategory(new ProductCategory
        //        {
        //            ProductId = model.Value,
        //            CategoryId = id

        //        });
        //        return RedirectToAction("products", new { id = id });
        //    }
        //    ViewBag.CategoryName = repo.FindCategory(id).Name;
        //    var pclist = repo.ProductCategories().Where(pc => pc.CategoryId == id).Select(pc => pc.ProductId);
        //    ViewBag.Products = repo.Products().Where(p => !pclist.Contains(p.ProductId));
        //    return View(model);
        //}

        //public ActionResult DeleteProduct(int id)
        //{
        //    var pc = repo.FindProductCategory(id);
        //    repo.RemoveProductCategory(pc);
        //    return RedirectToAction("products", new { id = pc.CategoryId });
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    repo.Dispose();
        //    base.Dispose(disposing);
        //}


    }
}
