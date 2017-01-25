using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KontinuityCRM.Controllers
{
    public class ReturnController : Controller
    {
        // GET: Return
        public ActionResult Index()
        {
            return View();
        }

        // GET: Return/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Return/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Return/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Return/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Return/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Return/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Return/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
