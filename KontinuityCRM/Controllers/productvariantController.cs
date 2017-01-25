using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KontinuityCRM.Helpers;
using KontinuityCRM.Models;
using System.Transactions;

namespace KontinuityCRM.Controllers
{
    public class productvariantController : Controller
    {
        IKontinuityCRMRepo repo;

        public productvariantController()
        {
            this.repo = new EFKontinuityCRMRepo();
        }

        //
        // GET: /category/

        public ActionResult Index()
        {
            return View(repo.ProductVariants());
        }

        //
        // GET: /category/Details/5

        private void CreateViewBag()
        {
            ViewBag.Products = repo.Products();
            ViewBag.Countries = repo.Countries();
            
        }

        public ActionResult Details(int id)
        {
            ProductVariant pv = repo.FindProductVariant(id);
            if (pv == null)
            {
                return HttpNotFound();
            }
            return View(pv);
        }

        //
        // GET: /category/Create

        public ActionResult Create()
        {
            ViewBag.CustomFields = repo.CustomFields();
            CreateViewBag();
            return View();
        }

        //
        // POST: /category/Create

        [HttpPost]
        public ActionResult Create(ProductVariant pv)
        {
            if (ModelState.IsValid)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    repo.AddProductVariant(pv);

                    foreach (var item in repo.CustomFields())
                    {
                        if (!string.IsNullOrWhiteSpace(Request.Form["cf-" + item.Name]))
	                    {
                            repo.AddCustomFieldValue(new CustomFieldValue
                            {
                                CustomFieldId = item.CustomFieldId,
                                ProductVariantId = pv.ProductVariantId,
                                Value = Request.Form["cf-" + item.Name],
                            });
	                    }  
                    }
                    
                    scope.Complete();
                    return RedirectToAction("Index");
                }
            }
            ViewBag.CustomFields = repo.CustomFields();
            CreateViewBag();
            return View(pv);
        }

        //
        // GET: /category/Edit/5

        public ActionResult Edit(int id)
        {
            ProductVariant pv = repo.FindProductVariant(id);
            if (pv == null)
            {
                return HttpNotFound();
            }
            CreateViewBag();
            ViewBag.CustomFields = from cf in repo.CustomFields()
                                   join cfv in repo.CustomFieldValues().Where(v => v.ProductVariantId == id)
                                   on cf.CustomFieldId equals cfv.CustomFieldId into r
                                   from cfv in r.DefaultIfEmpty()
                                   select new KontinuityCRM.Models.ViewModels.CustomFieldValueViewModel
                                   {
                                       Name = cf.Name,
                                       Value = cfv.Value != null ? cfv.Value : ""
                                   };
            return View(pv);
        }

        //
        // POST: /category/Edit/5

        [HttpPost]
        public ActionResult Edit(ProductVariant pv)
        {
            if (ModelState.IsValid)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    repo.EditProductVariant(pv);

                    repo.RemoveCustomFields(repo.CustomFieldValues().Where(v => v.ProductVariantId == pv.ProductVariantId));
                    foreach (var item in repo.CustomFields())
                    {
                        if (!string.IsNullOrWhiteSpace(Request.Form["cf-" + item.Name]))
                        {
                            repo.AddCustomFieldValue(new CustomFieldValue
                            {
                                CustomFieldId = item.CustomFieldId,
                                ProductVariantId = pv.ProductVariantId,
                                Value = Request.Form["cf-" + item.Name],
                            });
                        }
                    }


                    scope.Complete();
                    return RedirectToAction("Index");
                }
            }
            CreateViewBag();
            return View(pv);
        }

        //
        // GET: /category/Delete/5

        public ActionResult Delete(int id)
        {
            ProductVariant pv = repo.FindProductVariant(id);
            if (pv == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomFields = from cf in repo.CustomFields()
                                   join cfv in repo.CustomFieldValues().Where(v => v.ProductVariantId == id)
                                   on cf.CustomFieldId equals cfv.CustomFieldId into r
                                   from cfv in r.DefaultIfEmpty()
                                   select new KontinuityCRM.Models.ViewModels.CustomFieldValueViewModel
                                   {
                                       Name = cf.Name,
                                       Value = cfv.Value != null ? cfv.Value : ""
                                   };
            return View(pv);
        }

        //
        // POST: /category/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            ProductVariant pv = repo.FindProductVariant(id);
            repo.RemoveProductVariant(pv);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            repo.Dispose();
            base.Dispose(disposing);
        }

    }
}
