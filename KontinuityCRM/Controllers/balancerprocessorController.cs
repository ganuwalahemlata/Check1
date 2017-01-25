using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KontinuityCRM.Helpers;
using KontinuityCRM.Models;

namespace KontinuityCRM.Controllers
{
    public class balancerprocessorController : Controller
    {
        IKontinuityCRMRepo repo;

        public balancerprocessorController()
        {
            this.repo = new EFKontinuityCRMRepo();
        }

        //
        // GET: /category/

        public ActionResult Index()
        {
            return View(repo.BalancerProcessors());
        }

        //
        // GET: /category/Details/5

        public ActionResult Details(int id)
        {
            BalancerProcessor bp = repo.FindBalancerProcessor(id);
            if (bp == null)
            {
                return HttpNotFound();
            }
            return View(bp);
        }

        //
        // GET: /category/Create

        public ActionResult Create()
        {
            ViewBag.Balancers = repo.Balancers();
            ViewBag.Processors = repo.Processors();
            return View();
        }

        //
        // POST: /category/Create

        [HttpPost]
        public ActionResult Create(BalancerProcessor bp)
        {
            var entity = repo.FindBalancerProcessor(bp.BalancerId, bp.ProcessorId);
            if (entity != null)
            {
                ModelState.AddModelError("OrderId", "There is already an order with this product");
            }
            if (ModelState.IsValid)
            {
                repo.AddBalancerProcessor(bp);
                return RedirectToAction("Index");
            }
            ViewBag.Balancers = repo.Balancers();
            ViewBag.Processors = repo.Processors();
            return View(bp);
        }

        //
        // GET: /category/Edit/5

        public ActionResult Edit(int id)
        {
            BalancerProcessor bp = repo.FindBalancerProcessor(id);
            if (bp == null)
            {
                return HttpNotFound();
            }
            ViewBag.Balancers = repo.Balancers();
            ViewBag.Processors = repo.Processors();
            return View(bp);
        }

        //
        // POST: /category/Edit/5

        [HttpPost]
        public ActionResult Edit(BalancerProcessor bp)
        {
            if (ModelState.IsValid)
            {
                repo.EditBalancerProcessor(bp);
                return RedirectToAction("Index");
            }
            ViewBag.Balancers = repo.Balancers();
            ViewBag.Processors = repo.Processors();
            return View(bp);
        }

        //
        // GET: /category/Delete/5

        public ActionResult Delete(int id)
        {
            BalancerProcessor bp = repo.FindBalancerProcessor(id);
            if (bp == null)
            {
                return HttpNotFound();
            }
            return View(bp);
        }

        //
        // POST: /category/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            BalancerProcessor bp = repo.FindBalancerProcessor(id);
            repo.RemoveBalancerProcessor(bp);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            repo.Dispose();
            base.Dispose(disposing);
        }
    }
}
