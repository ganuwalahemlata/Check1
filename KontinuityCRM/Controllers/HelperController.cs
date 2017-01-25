using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KontinuityCRM.Helpers;
using KontinuityCRM.Models;
//using EnumAssembly;

namespace KontinuityCRM.Controllers
{
    public class HelperController : Controller
    {
        private readonly IUnitOfWork uow;

        public HelperController(IUnitOfWork uow)
        {
            this.uow = uow;
        }
        /// <summary>
        /// Returs States if any exists against country 
        /// </summary>
        /// <param name="id">Country Id</param>
        /// <returns></returns>
        public ActionResult States(int id)
        {
            var country_iso_code = uow.CountryRepository.Find(id).CountryAbbreviation;

            var data = uow.LocationRepository.Get(l => l.country_iso_code == country_iso_code && l.subdivision_1_name != null && l.subdivision_1_name != string.Empty)
                .GroupBy(l => l.subdivision_1_name)
                .Select(g => g.First().subdivision_1_name)
                .OrderBy(s => s);
                //.Select(g => g.First())
                //.Select(l => new { code = l.subdivision_1_iso_code, name = l.subdivision_1_name });
            
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Provides Cities against Country and Province
        /// </summary>
        /// <param name="countryId"></param>
        /// <param name="province"></param>
        /// <returns></returns>
        public ActionResult Cities(int countryId, string province)
        {
            var country_iso_code = uow.CountryRepository.Find(countryId).CountryAbbreviation;

            var data = uow.LocationRepository.Get(l => l.country_iso_code == country_iso_code && l.subdivision_1_name == province && l.city_name != string.Empty && l.city_name != null)
                .GroupBy(l => l.city_name)
                .Select(g => g.First().city_name)
                .OrderBy(s => s);

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Returns FulfillmentProviderDDL
        /// </summary>
        /// <returns></returns>
        public ActionResult FulfillmentProvidersDDL()
        {
            //var data = repo.FulfillmentProviders().Select(p => new { Id = p.Id, Name = p.Alias });
            var data = uow.FulfillmentProviderRepository.Get().Select(p => new { Id = p.Id, Name = p.Alias });
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Returns ProcessorDDL
        /// </summary>
        /// <returns></returns>
        public ActionResult ProcessorsDDL()
        {
            //var data = repo.Processors().Select(p => new { Id = p.Id, Name = p.Name });
            var data = uow.ProcessorRepository.Get().Select(p => new { Id = p.Id, Name = p.Name });
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Returns BalancerDDL
        /// </summary>
        /// <returns></returns>
        public ActionResult BalancersDDL()
        {
            //var data = repo.Balancers().Select(p => new { Id = p.Id, Name = p.Name });
            var data = uow.BalancerRepository.Get().Select(p => new { Id = p.Id, Name = p.Name });
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Returns RecurringProductDDL
        /// </summary>
        /// <returns></returns>
        public ActionResult RecurringProductIdDDL()
        {
            //var data = repo.Products().Select(p => new { Id = p.ProductId, Name = p.Name });
            var data = uow.ProductRepository.Get().Select(p => new { Id = p.ProductId, Name = p.Name });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult ProductsInCategory(int categoryid) 
        //{
        //    var data = repo.ProductCategories().Where(c => c.CategoryId == categoryid).Select(c => c.Product);
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}
        /// <summary>
        /// Creates ViewBag Data
        /// </summary>
        /// <returns></returns>
        public ActionResult TestAPI()
        {
            var newcustomer = new List<Customer> { new Customer { FirstName = "New", CustomerId = 0 } };
            //ViewBag.Customers = newcustomer.Concat(repo.Customers());
            //ViewBag.ShippingMethods = repo.ShippingMethods();
            //ViewBag.Processors = repo.Processors();
            //ViewBag.Products = repo.Products();
            ViewBag.Customers = newcustomer.Concat(uow.CustomerRepository.Get());
            ViewBag.ShippingMethods = uow.ShippingMethodRepository.Get();
            ViewBag.Processors = uow.ProcessorRepository.Get();
            ViewBag.Products = uow.ProductRepository.Get();

            int cyear = System.DateTime.Today.Year;
            var yarr = new int[20];
            for (int i = 0; i < 20; i++)
            {
                yarr[i] = cyear + i;
            }

            ViewBag.CreditCardExpirationYear = yarr;
            return View();
        }

        //[HttpPost]
        //public ActionResult TestAPI(KontinuityCRM.Models.APIModels.PostOrderRequest por)
        //{
        //    var name = por.Order.ShippingFirstName;
        //    foreach (var item in por.OrderProducts)
        //    {
        //        var s = item.Price;
        //    }
        //    return RedirectToAction("index", "home");
        //}

        public ActionResult Tmp()
        {
            var assembly = System.Reflection.Assembly.Load("EnumAssembly");
            Type type = assembly.GetType("ViewPermissiond");

            List<string> list = new List<string>();
            long total = 0;

            foreach (object o in Enum.GetValues(type))
            {
                long value = (long)o;
                total += value;
                list.Add(String.Format("{0}.{1} = {2}", type, o, value));
            }

            ViewBag.values = list;
            ViewBag.total = total;
            /*****************************************************************/


            Type type1 = assembly.GetType("ViewPermission");
            total = 0;
            //Enum.GetValues(

            List<string> list1 = new List<string>();
            foreach (object o in Enum.GetValues(type1))
            {
                long value = (long)o;
                total += value;
                list1.Add(String.Format("{0}.{1} = {2}", type1, o, value));
            }

            ViewBag.values1 = list1;
            ViewBag.total1 = total;
            /*****************************************************************/

            Type type2 = assembly.GetType("ViewPermission2");
            total = 0;
           

            List<string> list2 = new List<string>();
            foreach (object o in Enum.GetValues(type2))
            {
                long value = (long)o;
                total += value;
                list2.Add(String.Format("{0}.{1} = {2}", type2, o, value));
            }

            ViewBag.values2 = list2;
            ViewBag.total2 = total;

            return View();
        }
        /// <summary>
        /// Redirects to Order's Transactions
        /// </summary>
        /// <param name="id">Order Id</param>
        /// <returns></returns>
        public ActionResult Transactions(int id)
        {
            var order = uow.OrderRepository.Find(id);
            if (order == null)
	        {
		         return HttpNotFound();
	        }
            return View(order.Transactions);
        }
        /// <summary>
        /// Redirects to avialable Products
        /// </summary>
        /// <returns></returns>
        public ActionResult Test()
        {
            ViewBag.Products = uow.ProductRepository.Get();
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public ActionResult CustomerAutocomplete(string term)
        {
            var customers = uow.CustomerRepository.Get(c => ((c.FirstName + " " + c.LastName).ToLower()).StartsWith(term.ToLower()))
                .Select(c => new { id = c.CustomerId, value = c.FullName });

            return Json(customers, JsonRequestBehavior.AllowGet);
        }
    }
}
