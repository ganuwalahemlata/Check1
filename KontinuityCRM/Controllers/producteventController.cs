using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KontinuityCRM.Helpers;
using KontinuityCRM.Models;

namespace KontinuityCRM.Controllers
{
    public class producteventController : Base1Controller
    {
        
        //
        // GET: /category/

        public ActionResult Index()
        {
            return View(repo.ProductEvents());
        }

        //
        // GET: /category/Details/5

        public ActionResult Details(int id)
        {
            ProductEvent pe = repo.FindProductEvent(id);
            if (pe == null)
            {
                return HttpNotFound();
            }
            return View(pe);
        }

        //
        // GET: /category/Create

        public ActionResult Create()
        {
            ViewBag.Products = repo.Products();
            ViewBag.Events = repo.Events();
            return View();
        }

        //
        // POST: /category/Create

        [HttpPost]
        public ActionResult Create(ProductEvent pe)
        {
            var entity = repo.FindProductEvent(pe.ProductId, pe.EventId);
            if (entity != null)
            {
                ModelState.AddModelError("ProductId", "This product already has the event");
            }
            if (ModelState.IsValid)
            {
                repo.AddProductEvent(pe);
                return RedirectToAction("Index");
            }
            ViewBag.Products = repo.Products();
            ViewBag.Events = repo.Events();
            return View(pe);
        }

        //
        // GET: /category/Edit/5

        public ActionResult Edit(int id)
        {
            ProductEvent pe = repo.FindProductEvent(id);
            if (pe == null)
            {
                return HttpNotFound();
            }
            ViewBag.Products = repo.Products();
            ViewBag.Events = repo.Events();
            return View(pe);
        }

        //
        // POST: /category/Edit/5

        [HttpPost]
        public ActionResult Edit(ProductEvent pe)
        {
            var entity = repo.FindProductEvent(pe.ProductId, pe.EventId);
            if (entity != null)
            {
                ModelState.AddModelError("ProductId", "This product already has the event");
            }
            if (ModelState.IsValid)
            {
                repo.EditProductEvent(pe);
                return RedirectToAction("Index");
            }
            ViewBag.Products = repo.Products();
            ViewBag.Events = repo.Events();
            return View(pe);
        }

        //
        // GET: /category/Delete/5

        public ActionResult Delete(int id)
        {
            ProductEvent pe = repo.FindProductEvent(id);
            if (pe == null)
            {
                return HttpNotFound();
            }
            return View(pe);
        }

        //
        // POST: /category/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            ProductEvent pe = repo.FindProductEvent(id);
            repo.RemoveProductEvent(pe);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            repo.Dispose();
            base.Dispose(disposing);
        }
    }
}
