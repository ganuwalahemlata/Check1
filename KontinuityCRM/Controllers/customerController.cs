using KontinuityCRM.Models;
using KontinuityCRM.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Linq.Expressions;
using KontinuityCRM.Filters;

namespace KontinuityCRM.Controllers
{
    [CustomAuthorizeAttribute]
    [System.ComponentModel.DisplayName("Customers")]
    public class customerController : BaseController
    {
        public customerController (IUnitOfWork uow, IWebSecurityWrapper wsw) : base(uow, wsw)
	    {

	    }

        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? display)
        {
            var user = uow.UserProfileRepository.Find(wsw.CurrentUserId);
            int pageSize = user.CustomerDisplay;

            if (display.HasValue && display != pageSize)
            {
                page = 1;
                pageSize = display.Value;

                user.CustomerDisplay = pageSize;

                uow.UserProfileRepository.Update(user);
                uow.Save(wsw.CurrentUserName);
            }
            if (searchString != null)
                page = 1;
            else
                searchString = currentFilter;

            ViewBag.CurrentSort = sortOrder;
            ViewBag.IdSortParm = String.IsNullOrEmpty(sortOrder) ? "id_asc" : "";
            ViewBag.FirstNameSortParm = sortOrder == "firstname" ? "firstname_desc" : "firstname";
            ViewBag.LastNameSortParm = sortOrder == "lastname" ? "lastname_desc" : "lastname";
            ViewBag.PhoneSortParm = sortOrder == "phone" ? "phone_desc" : "phone";
            ViewBag.EmailSortParm = sortOrder == "email" ? "email_desc" : "email";


            ViewBag.firstnameOrderIcon = "sort";
            ViewBag.phoneOrderIcon = "sort";
            ViewBag.lastnameOrderIcon = "sort";
            ViewBag.idOrderIcon = "sort";
            ViewBag.emailOrderIcon = "sort";


            Func<IQueryable<Customer>, IOrderedQueryable<Customer>> orderBy = o =>
            {
                switch (sortOrder)
                {
                    case "id_asc":
                        ViewBag.idOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.CustomerId);

                    case "phone":
                        ViewBag.phoneOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Phone).ThenByDescending(c => c.CustomerId);
                    case "phone_desc":
                        ViewBag.phoneOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Phone).ThenByDescending(c => c.CustomerId);

                    case "email":
                        ViewBag.emailOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Email).ThenByDescending(c => c.CustomerId);
                    case "email_desc":
                        ViewBag.emailOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Email).ThenByDescending(c => c.CustomerId);

                    case "firstname":
                        ViewBag.firstnameOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.FirstName).ThenByDescending(c => c.CustomerId);
                    case "firstname_desc":
                        ViewBag.firstnameOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.FirstName).ThenByDescending(c => c.CustomerId);

                    case "lastname":
                        ViewBag.lastnameOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.LastName).ThenByDescending(c => c.CustomerId);
                    case "lastname_desc":
                        ViewBag.lastnameOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.LastName).ThenByDescending(c => c.CustomerId);

                    default:
                        ViewBag.idOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.CustomerId);
                }
            };


            ViewBag.CurrentFilter = searchString;

            Expression<Func<Customer, bool>> filter = null;
            filter = f => f.Status.Value == null || f.Status.Value > 0;
            if (!String.IsNullOrEmpty(searchString))
            {
                filter = f => f.FirstName.Contains(searchString)
                    || f.LastName.Contains(searchString)
                    //|| f.Address1.Contains(searchString)
                    //|| f.Address2.Contains(searchString)
                    //|| f.Country.Contains(searchString)
                    //|| f.IPAddress.Contains(searchString)
                    || f.Email.Contains(searchString)
                    || f.Phone.Contains(searchString)
                    ;
            }

           

            ViewBag.Display = pageSize;

            int pageNumber = (page ?? 1);

            var model = uow.CustomerRepository
                .GetPage(pageSize, pageNumber,
                //out count,
                filter: filter,
                orderBy: orderBy
                );

            return View(model);
        }


        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /category/Create

        [HttpPost]
        public ActionResult Create(Customer customer)
        {
            if (ModelState.IsValid)
            {
                uow.CustomerRepository.Add(customer);
                uow.Save(wsw.CurrentUserName);
                return RedirectToAction("Index");
            }
            return View(customer);
        }

        //
        // GET: /category/Edit/5

        public ActionResult Edit(int id)
        {
            Customer customer = uow.CustomerRepository.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        //
        // POST: /category/Edit/5

        [HttpPost]
        public ActionResult Edit(Customer customer)
        {
            if (ModelState.IsValid)
            {
                uow.CustomerRepository.Update(customer);
                uow.Save(wsw.CurrentUserName);
                return RedirectToAction("Index");
            }
            return View(customer);
        }

        //
        // GET: /category/Delete/5

        public ActionResult Delete(int id) //async Task<ActionResult>
        {
            var customer = uow.CustomerRepository.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            //await customer.Delete(uow);
            if (ModelState.IsValid)
            {
                customer.Status = 0;
                uow.CustomerRepository.Update(customer);
                uow.Save(wsw.CurrentUserName);
                return RedirectToAction("Index");
            }
            return View(customer);

            //return View(customer);
        }

        //
        // GET: /category/View/5

        public ActionResult View(int id)
        {
            Customer customer = uow.CustomerRepository.Find(id);

            if (customer == null)
            {
                return HttpNotFound();
            }

            ViewBag.CustomerOrders = new Dictionary<Order, List<Order>>();

            foreach(var order in customer.Orders.Where(o => o.ParentId == null || o.ParentId == 0).OrderBy(o => o.Created).ToList())
            {
                ViewBag.CustomerOrders[order] = new List<Order>();

                foreach(var soon in customer.Orders.Where(o => o.ParentId == order.OrderId).OrderBy(o => o.Created).ToList())
                {
                    ViewBag.CustomerOrders[order].Add(soon);
                    this.AddSoonOrders(order, soon, customer);
                }
            }                                

            return View(customer);
        }

        private void AddSoonOrders(Order root, Order order, Customer customer)
        {
            foreach (var soon in customer.Orders.Where(o => o.ParentId == order.OrderId).OrderBy(o => o.Created).ToList())
            {
                ViewBag.CustomerOrders[root].Add(soon);
                this.AddSoonOrders(root, soon, customer);
            }
        }

        //[HttpPost, ActionName("Delete")]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Customer customer = repo.FindCustomer(id);
        //    repo.RemoveCustomer(customer);
        //    return RedirectToAction("Index");
        //}

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
}
