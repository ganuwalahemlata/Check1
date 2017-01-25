using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KontinuityCRM.Helpers;
using KontinuityCRM.Models;

namespace KontinuityCRM.Controllers
{
    [Authorize]
    public class postbackController : BaseController
    {
        //
        // GET: /category/

        public ActionResult Index()
        {
            return View(repo.Postbacks());
        }

        //
        // GET: /category/Details/5

        public ActionResult Details(int id)
        {
            Postback postback = repo.FindPostback(id);
            if (postback == null)
            {
                return HttpNotFound();
            }
            return View(postback);
        }

        //
        // GET: /category/Create

        public ActionResult Create()
        {
            ViewBag.Products = repo.Products();
            return View();
        }

        //
        // POST: /category/Create

        [HttpPost]
        public ActionResult Create(Postback postback)
        {
            if (ModelState.IsValid)
            {
                repo.AddPostback(postback);
                return RedirectToAction("Index");
            }
            ViewBag.Products = repo.Products();
            return View(postback);
        }

        //
        // GET: /category/Edit/5

        public ActionResult Edit(int id)
        {
            Postback postback = repo.FindPostback(id);
            if (postback == null)
            {
                return HttpNotFound();
            }
            ViewBag.Products = repo.Products();
            return View(postback);
        }

        //
        // POST: /category/Edit/5

        [HttpPost]
        public ActionResult Edit(Postback postback)
        {
            if (ModelState.IsValid)
            {
                repo.EditPostback(postback);
                return RedirectToAction("Index");
            }
            ViewBag.Products = repo.Products();
            return View(postback);
        }

        //
        // GET: /category/Delete/5

        public ActionResult Delete(int id)
        {
            Postback postback = repo.FindPostback(id);
            if (postback == null)
            {
                return HttpNotFound();
            }
            return View(postback);
        }

        //
        // POST: /category/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Postback postback = repo.FindPostback(id);
            repo.RemovePostback(postback);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            repo.Dispose();
            base.Dispose(disposing);
        }
    }
}
