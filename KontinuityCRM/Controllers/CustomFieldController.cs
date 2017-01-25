using KontinuityCRM.Helpers;
using KontinuityCRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KontinuityCRM.Controllers
{
    public class CustomFieldController : Controller
    {
        IKontinuityCRMRepo repo;

        public CustomFieldController()
        {
            this.repo = new EFKontinuityCRMRepo();
        }

        public ActionResult Index()
        {
            return View(repo.CustomFields());
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(CustomField cf)
        {
            if (ModelState.IsValid)
            {
                repo.AddCustomField(cf);
                return RedirectToAction("Index");
            }
            return View(cf);
        }

        public ActionResult Edit(int id)
        {
            CustomField cf = repo.FindCustomField(id);
            if (cf == null)
            {
                return HttpNotFound();
            }
            
            return View(cf);
        }

        [HttpPost]
        public ActionResult Edit(CustomField cf)
        {
            if (ModelState.IsValid)
            {
                repo.EditCustomField(cf);
                return RedirectToAction("Index");
            }
            
            return View(cf);
        }

        public ActionResult Delete(int id)
        {
            CustomField cf = repo.FindCustomField(id);
            if (cf == null)
            {
                return HttpNotFound();
            }
            return View(cf);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            CustomField cf = repo.FindCustomField(id);
            repo.RemoveCustomField(cf);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            repo.Dispose();
            base.Dispose(disposing);
        }

    }
}
