using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KontinuityCRM.Helpers;
using KontinuityCRM.Models;

namespace KontinuityCRM.Controllers
{
    public class orderproductController : Controller
    {
        IKontinuityCRMRepo repo;

        public orderproductController()
        {
            this.repo = new EFKontinuityCRMRepo();
        }

        //
        // GET: /category/

        public ActionResult Index()
        {
            return View(repo.OrderProducts());
        }

        //
        // GET: /category/Details/5

        public ActionResult Details(int id)
        {
            OrderProduct op = repo.FindOrderProduct(id);
            if (op == null)
            {
                return HttpNotFound();
            }
            return View(op);
        }

        //
        // GET: /category/Create

        public ActionResult Create()
        {
            ViewBag.Products = repo.Products();
            ViewBag.Orders = repo.Orders();
            return View();
        }

        //
        // POST: /category/Create

        [HttpPost]
        public ActionResult Create(OrderProduct op)
        {
            //var entity = repo.FindOrderProduct(op.OrderId, op.ProductId);
            //if (entity != null)
            //{
            //    ModelState.AddModelError("OrderId", "There is already an order with this product");
            //}
            if (ModelState.IsValid)
            {
                var product = repo.FindProduct(op.ProductId);

                // calculated values
                op.Price = product.Price;
                op.Tax = 0;
                //op.Total = op.Price + op.Tax; calculate on the fly

                // update the subtotal & total of the order object
                //order.SubTotal += op.Price * op.Quantity;
                //order.Total += op.Price * op.Quantity; don't do it! best calculate on the fly

                repo.AddOrderProduct(op);
                return RedirectToAction("Index");
            }
            ViewBag.Products = repo.Products();
            ViewBag.Orders = repo.Orders();
            return View(op);
        }

        //
        // GET: /category/Edit/5

        public ActionResult Edit(int id)
        {
            OrderProduct op = repo.FindOrderProduct(id);
            if (op == null)
            {
                return HttpNotFound();
            }
            ViewBag.Products = repo.Products();
            ViewBag.Orders = repo.Orders();
            return View(op);
        }

        //
        // POST: /category/Edit/5

        [HttpPost]
        public ActionResult Edit(OrderProduct op)
        {
            //var entity = repo.FindOrderProduct(op.OrderId, op.ProductId);
            //if (entity != null)
            //{
            //    ModelState.AddModelError("OrderId", "There is already an order with this product");
            //}
            if (ModelState.IsValid)
            {
                var product = repo.FindProduct(op.ProductId);
                // calculated values
                op.Price = product.Price;
                op.Tax = 0;

                repo.EditOrderProduct(op);
                return RedirectToAction("Index");
            }
            ViewBag.Products = repo.Products();
            ViewBag.Orders = repo.Orders();
            return View(op);
        }

        //
        // GET: /category/Delete/5

        public ActionResult Delete(int id)
        {
            OrderProduct op = repo.FindOrderProduct(id);
            if (op == null)
            {
                return HttpNotFound();
            }
            return View(op);
        }

        //
        // POST: /category/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            OrderProduct op = repo.FindOrderProduct(id);
            repo.RemoveOrderProduct(op);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            repo.Dispose();
            base.Dispose(disposing);
        }
    }
}
