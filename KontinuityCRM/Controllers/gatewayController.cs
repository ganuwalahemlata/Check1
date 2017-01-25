using KontinuityCRM.Models;
using KontinuityCRM.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using KontinuityCRM.Models.Gateways;
using System.Linq.Expressions;

namespace KontinuityCRM.Controllers
{
    [Authorize]
    [System.ComponentModel.DisplayName("Gateways")]
    public class gatewayController : BaseController
    {
        public gatewayController(IUnitOfWork uow, IWebSecurityWrapper wsw) : base(uow, wsw)
        {

        }

        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? display)
        {
            var type = typeof(GatewayModel);
            ViewBag.Providers = type.Assembly.GetTypes()
                  .Where(p => type.IsAssignableFrom(p) && !p.IsAbstract);

            var user = uow.UserProfileRepository.Find(wsw.CurrentUserId);
            int pageSize = user.GatewayDisplay;

            if (display.HasValue && display != pageSize)
            {
                page = 1;
                pageSize = display.Value;

                user.GatewayDisplay = pageSize;

                uow.UserProfileRepository.Update(user);
                uow.Save(wsw.CurrentUserName);
            }
            if (searchString != null)
                page = 1;
            else
                searchString = currentFilter;

            ViewBag.CurrentSort = sortOrder;
            ViewBag.IdSortParm = String.IsNullOrEmpty(sortOrder) ? "id_asc" : "";
            ViewBag.NameSortParm = sortOrder == "name" ? "name_desc" : "name";           
            ViewBag.aliasSortParm = sortOrder == "alias" ? "alias_desc" : "alias";
           

            ViewBag.nameOrderIcon = "sort";
            ViewBag.aliasOrderIcon = "sort";
            ViewBag.idOrderIcon = "sort";

            Func<IQueryable<Gateway>, IOrderedQueryable<Gateway>> orderBy = o =>
            {
                switch (sortOrder)
                {
                    case "id_asc":
                        ViewBag.idOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Id);

                    case "name":
                        ViewBag.nameOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Name).ThenByDescending(c => c.Id);
                    case "name_desc":
                        ViewBag.nameOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Name).ThenByDescending(c => c.Id);

                    case "alias":
                        ViewBag.aliasOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Type).ThenByDescending(c => c.Id);
                    case "alias_desc":
                        ViewBag.aliasOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Type).ThenByDescending(c => c.Id);


                    default:
                        ViewBag.idOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Id);
                }
            };


            ViewBag.CurrentFilter = searchString;

            Expression<Func<Gateway, bool>> filter = null;

            if (!String.IsNullOrEmpty(searchString))
            {
                filter = f => (f.Name.Contains(searchString) && f.Status != 2)
                    //|| f.Name.Contains(searchString)
                    //|| f.Address1.Contains(searchString)
                    //|| f.Address2.Contains(searchString)
                    //|| f.Country.Contains(searchString)
                    //|| f.IPAddress.Contains(searchString)
                    || (f.Type.Contains(searchString) && f.Status != 2)
                    //|| f.Phone.Contains(searchString)
                    ;
            }
            else
            {
                filter = f => f.Status != 2;
            }

            ViewBag.Display = pageSize;

            int pageNumber = (page ?? 1);

            var model = uow.GatewayRepository
                .GetPage(pageSize, pageNumber,
                //out count,
                filter: filter,
                orderBy: orderBy
                );

            return View(model);

            //return View(uow.GatewayRepository.Get());
        }

        public ActionResult Create(string id)
        {
            var type = typeof(GatewayModel);
            var providertype = type.Assembly.GetTypes()
                    .Where(p => type.IsAssignableFrom(p) && !p.IsAbstract
                        && p.Name.Equals(id, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

            if (providertype == null)
            {
                return HttpNotFound();
            }

            ViewBag.Type = providertype.Name;
            ViewBag.Title = string.Format("{0}", providertype.DisplayClassName());
            return View();
        }

        [HttpPost]
        public ActionResult Create([Bind(Exclude = "Id")] Gateway gateway)
        {
            if (ModelState.IsValid)
            {
                gateway.CreatedDate = DateTime.UtcNow;
                uow.GatewayRepository.Add(gateway);
                uow.Save(wsw.CurrentUserName);

                return RedirectToAction("Index");
            }

            ViewBag.Type = gateway.Type;
            ViewBag.Title = string.Format("{0}", gateway.GetType().DisplayClassName());
            return View(gateway);
        }

        public ActionResult Edit(int id)
        {
            var gateway = uow.GatewayRepository.Find(id);
            if (gateway == null)
            {
                return HttpNotFound();
            }
            
            return View(gateway);
        }

        [HttpPost]
        public ActionResult Edit(Gateway gateway)
        {
            if (ModelState.IsValid)
            {
                uow.GatewayRepository.Update(gateway);
                uow.Save(wsw.CurrentUserName);

                return RedirectToAction("Index");
            }
            return View(gateway);
        }

        public ActionResult Delete(int id)
        {
            var gateway = uow.GatewayRepository.Find(id);
            if (gateway == null)
            {
                return HttpNotFound();
            }

            uow.GatewayRepository.Delete(gateway);
            uow.Save(wsw.CurrentUserName);

            return RedirectToAction("Index");
        }
        public ActionResult ReplaceGateway(int newGid, int oldGid)
        {

            //foreach (var item in uow.ProcessorCascadeRepository.Get(a => a.ga == oldGid).ToList())
            //{
            //    item.ProcessorId = newGid;
            //    uow.ProcessorCascadeRepository.Update(item);
            //}
            //uow.Save(wsw.CurrentUserName);

            //foreach (var item in uow.TransactionRepository.Get(a => a.ProcessorId == oldGid).ToList())
            //{
            //    item.ProcessorId = newGid;

            //    uow.TransactionRepository.Update(item);

            //}
            //uow.Save(wsw.CurrentUserName);
            //foreach (var item in uow.ProductRepository.Get(a => a.ProcessorId == oldGid).ToList())
            //{
            //    item.PaymentTypeIds = new List<string>();
            //    item.PaymentTypeIds = item.PaymentTypeIds;

            //    item.ProcessorId = newGid;
            //    uow.ProductRepository.Update(item);
            //    uow.Save(wsw.CurrentUserName);
            //}

            var gateway = uow.GatewayRepository.Find(oldGid);

            if (gateway == null)
            {
                return HttpNotFound();
            }          

            foreach (var item in uow.ProcessorRepository.Get(a => a.GatewayId == oldGid))
            {
                item.RetryProcessorIds = new List<string>();
                foreach (var pc in uow.ProcessorCascadeRepository.Get(a => a.ProcessorId == item.Id))
                {
                    item.RetryProcessorIds.Add(pc.ProcessorRetryId.ToString());
                }

                item.GatewayId = newGid;
                uow.ProcessorRepository.Update(item);
            }
            gateway.Status = 2;
            // uow.Save(wsw.CurrentUserName);
            uow.GatewayRepository.Update(gateway);
            uow.Save(wsw.CurrentUserName);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public JsonResult GetProcessorAndBalancer(int id)
        {
            var gateway = uow.GatewayRepository.Find(id);

            if (gateway == null)
            {
                return new JsonResult();
            }
            //var ProcessorProducts = uow.ProductRepository.Get(a => a.ProcessorId == id).ToList();
            //var BalancerProcessors = uow.BalancerProcessorRepository.Get(a => a.ProcessorId == id).ToList();
            var replacableGateway = uow.GatewayRepository.Get(c => c.Id != id && c.Status!=2);

            ViewBag.AvailableGateway = replacableGateway;
            var genericResult = new { AvailableGateway = replacableGateway };//, OrphanProduct = ProcessorProducts, OrphanBalancer = BalancerProcessors };

            return Json(genericResult, JsonRequestBehavior.AllowGet);

        }
        protected override void Dispose(bool disposing)
        {            
            base.Dispose(disposing);
        }
    }
}
