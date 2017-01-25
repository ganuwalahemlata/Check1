using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KontinuityCRM.Helpers;
using KontinuityCRM.Models;

namespace KontinuityCRM.Controllers
{
    public class productcategoryController : Controller
    {
        IKontinuityCRMRepo repo;

        public productcategoryController()
        {
            this.repo = new EFKontinuityCRMRepo();
        }

        //
        // GET: /category/

        public ActionResult Index()
        {
            return View(repo.ProductCategories());
        }

        //
        // GET: /category/Details/5

        public ActionResult Details(int id)
        {
            ProductCategory pc = repo.FindProductCategory(id);
            if (pc == null)
            {
                return HttpNotFound();
            }
            return View(pc);
        }

        //
        // GET: /category/Create

        public ActionResult Create()
        {
            ViewBag.Products = repo.Products();
            ViewBag.Categories = repo.Categories();
            return View();
        }

        //
        // POST: /category/Create

        [HttpPost]
        public ActionResult Create(ProductCategory pc)
        {
            var entity = repo.FindProductCategory(pc.ProductId, pc.CategoryId);
            if (entity != null)
            {
                ModelState.AddModelError("ProductId", "This product already belongs to the category");
            }
            if (ModelState.IsValid)
            {
                repo.AddProductCategory(pc);
                return RedirectToAction("Index");
            }
            ViewBag.Products = repo.Products();
            ViewBag.Categories = repo.Categories();
            return View(pc);
        }

        //
        // GET: /category/Edit/5

        public ActionResult Edit(int id)
        {
            ProductCategory pc = repo.FindProductCategory(id);
            if (pc == null)
            {
                return HttpNotFound();
            }
            ViewBag.Products = repo.Products();
            ViewBag.Categories = repo.Categories();
            return View(pc);
        }

        //
        // POST: /category/Edit/5

        [HttpPost]
        public ActionResult Edit(ProductCategory pc)
        {
            var entity = repo.FindProductCategory(pc.ProductId, pc.CategoryId);
            if (entity != null)
            {
                ModelState.AddModelError("ProductId", "This product already belongs to the category");
            }
            if (ModelState.IsValid)
            {
                repo.EditProductCategory(pc);
                return RedirectToAction("Index");
            }
            ViewBag.Products = repo.Products();
            ViewBag.Categories = repo.Categories();
            return View(pc);
        }

        //
        // GET: /category/Delete/5

        public ActionResult Delete(int id)
        {
            ProductCategory pc = repo.FindProductCategory(id);
            if (pc == null)
            {
                return HttpNotFound();
            }
            return View(pc);
        }

        //
        // POST: /category/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            ProductCategory pc = repo.FindProductCategory(id);
            repo.RemoveProductCategory(pc);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            repo.Dispose();
            base.Dispose(disposing);
        }
    }
}
