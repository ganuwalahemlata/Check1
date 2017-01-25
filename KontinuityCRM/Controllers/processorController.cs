using KontinuityCRM.Models;
using KontinuityCRM.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using KontinuityCRM.Models.Gateways;
using System.Linq.Expressions;

namespace KontinuityCRM.Controllers
{
    [Authorize]
    [System.ComponentModel.DisplayName("Processors")]
    public class processorController : BaseController
    {
        private readonly IMappingEngine mapper;

        public processorController(IUnitOfWork uow, IWebSecurityWrapper wsw, IMappingEngine mapper)
            : base(uow, wsw)
        {
            this.mapper = mapper;
        }
        /// <summary>
        /// Redirects to Processor Listings
        /// </summary>
        /// <param name="sortOrder">Sort Order</param>
        /// <param name="currentFilter">selected filter</param>
        /// <param name="searchString">search value</param>
        /// <param name="page">Page Number</param>
        /// <param name="display">Diaplying Items No.</param>
        /// <returns></returns>
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? display)
        {
            ViewBag.Gateways = uow.GatewayRepository.Get();

            var user = uow.UserProfileRepository.Find(wsw.CurrentUserId);
            int pageSize = user.ProcessorDisplay;

            if (display.HasValue && display != pageSize)
            {
                page = 1;
                pageSize = display.Value;

                user.ProcessorDisplay = pageSize;

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
            ViewBag.gatewaySortParm = sortOrder == "gateway" ? "gateway_desc" : "gateway";
            ViewBag.typeSortParm = sortOrder == "type" ? "type_desc" : "type";

            ViewBag.typeOrderIcon = "sort";
            ViewBag.nameOrderIcon = "sort";
            ViewBag.gatewayOrderIcon = "sort";
            ViewBag.idOrderIcon = "sort";

            Func<IQueryable<Processor>, IOrderedQueryable<Processor>> orderBy = o =>
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

                    case "type":
                        ViewBag.typeOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Type).ThenByDescending(c => c.Id);
                    case "type_desc":
                        ViewBag.typeOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Type).ThenByDescending(c => c.Id);

                    case "gateway":
                        ViewBag.gatewayOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Gateway.Name).ThenByDescending(c => c.Id);
                    case "gateway_desc":
                        ViewBag.gatewayOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Gateway.Name).ThenByDescending(c => c.Id);


                    default:
                        ViewBag.idOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Id);
                }
            };


            ViewBag.CurrentFilter = searchString;

            Expression<Func<Processor, bool>> filter = null;

            if (!String.IsNullOrEmpty(searchString))
            {
                filter = f => f.Name.Contains(searchString)
                     && f.Status != 2 ///status 2 is deleted
                    //|| f.Name.Contains(searchString)
                    //|| f.Address1.Contains(searchString)
                    //|| f.Address2.Contains(searchString)
                    //|| f.Country.Contains(searchString)
                    //|| f.IPAddress.Contains(searchString)
                    //|| f.Type.Contains(searchString)
                    //|| f.Phone.Contains(searchString)
                    ;
            }
            else
            {
                filter = f => f.Status != 2;
            }



            ViewBag.Display = pageSize;

            int pageNumber = (page ?? 1);

            var model = uow.ProcessorRepository
                .GetPage(pageSize, pageNumber,
                //out count,
                filter: filter,
                orderBy: orderBy
                );

            return View(model);

            //return View(uow.ProcessorRepository.Get());
        }
        /// <summary>
        /// Redirects to create view
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Create(int id)
        {

            var gateway = uow.GatewayRepository.Find(id);
            if (gateway == null)
            {
                return HttpNotFound();
            }

            var processor = new Processor { Gateway = gateway, GatewayId = gateway.Id };
            ViewBag.RetryProcessors = new SelectList(uow.ProcessorRepository.Get(), "Id", "Name");
            //Activator.CreateInstance("KontinuityCRM", gateway.Type).Unwrap();
            return View(processor.GatewayModel(mapper));
        }
        /// <summary>
        /// post action to create
        /// </summary>
        /// <param name="gatewayModel">Gateway Model</param>
        /// <param name="capture">Capture Value</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(GatewayModel gatewayModel, string capture = null)
        {
            if (ModelState.IsValid)
            {
                handleCapture(gatewayModel, capture);
                gatewayModel.CreatedDate = DateTime.UtcNow;
                var processor = gatewayModel.Processor(mapper);
                uow.ProcessorRepository.Add(processor);
                uow.Save(wsw.CurrentUserName);
                if (gatewayModel.RetryProcessorIds != null)
                    foreach (var item in gatewayModel.RetryProcessorIds)
                    {
                        uow.ProcessorCascadeRepository.Add(new ProcessorCascade() { ProcessorId = processor.Id, ProcessorRetryId = Convert.ToInt32(item) });
                    }
                uow.Save(wsw.CurrentUserName);

                return RedirectToAction("Index");
            }

            return View(gatewayModel);
        }
        /// <summary>
        /// updates gateway model based on provided capture value
        /// </summary>
        /// <param name="gatewayModel">gateway Model</param>
        /// <param name="capture"></param>
        private void handleCapture(KontinuityCRM.Models.Gateways.GatewayModel gatewayModel, string capture)
        {
            // reset 
            gatewayModel.ShipmentOnCapture = false;
            gatewayModel.CaptureOnShipment = false;
            gatewayModel.CaptureDelayHours = null;

            switch (capture)
            {
                case "ShipmentOnCapture":
                    gatewayModel.ShipmentOnCapture = true;
                    break;
                case "CaptureOnShipment":
                    gatewayModel.CaptureOnShipment = true;
                    break;
                default:
                    gatewayModel.CaptureDelayHours = string.IsNullOrEmpty(Request.Form["CaptureDelayHours"]) ? null :
                       (int?)Convert.ToInt32(Request.Form["CaptureDelayHours"]);
                    break;
            }
        }
        /// <summary>
        /// Redirects to edit view
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            var processor = uow.ProcessorRepository.Find(id);
            processor.RetryProcessorIds = new List<string>();
            ViewBag.RetryProcessors = new SelectList(uow.ProcessorRepository.Get(a => a.Id != id), "Id", "Name");
            foreach (var item in uow.ProcessorCascadeRepository.Get(a => a.ProcessorId == id))
            {
                processor.RetryProcessorIds.Add(item.ProcessorRetryId.ToString());
            }
            if (processor == null)
            {
                return HttpNotFound();
            }

            return View(processor.GatewayModel(mapper));
        }
        /// <summary>
        /// post action to update
        /// </summary>
        /// <param name="gatewayModel">Gateway Model</param>
        /// <param name="capture">capture Value</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(GatewayModel gatewayModel, string capture)
        {
            if (ModelState.IsValid)
            {
                handleCapture(gatewayModel, capture);
                var processor = gatewayModel.Processor(mapper);
                uow.ProcessorRepository.Update(processor);
                uow.Save(wsw.CurrentUserName);

                foreach (var item in uow.ProcessorCascadeRepository.Get(a => a.ProcessorId == processor.Id))
                {
                    uow.ProcessorCascadeRepository.Delete(item);
                }
                if (gatewayModel.RetryProcessorIds != null)
                    foreach (var item in gatewayModel.RetryProcessorIds)
                    {
                        uow.ProcessorCascadeRepository.Add(new ProcessorCascade() { ProcessorId = processor.Id, ProcessorRetryId = Convert.ToInt32(item) });
                    }
                uow.Save(wsw.CurrentUserName);

                return RedirectToAction("Index");
            }

            return View(gatewayModel);
        }
        /// <summary>
        /// delete by id
        /// </summary>
        /// <param name="id">processor id</param>
        /// <returns></returns>
        public ActionResult Delete(int id)
        {
            var processor = uow.ProcessorRepository.Find(id);

            if (processor == null)
            {
                return HttpNotFound();
            }
            //foreach (var item in uow.ProcessorCascadeRepository.Get(a => a.ProcessorId == id).ToList())
            //{
            //    uow.ProcessorCascadeRepository.Delete(item);
            //}
            //uow.Save(wsw.CurrentUserName);

            //foreach (var item in uow.TransactionRepository.Get(a => a.ProcessorId == id).ToList())
            //{
            //    uow.TransactionRepository.Delete(item);
            //}
            //uow.Save(wsw.CurrentUserName);
            //foreach (var item in uow.ProductRepository.Get(a => a.ProcessorId == id).ToList())
            //{
            //    foreach (var orderproduct in uow.OrderProductRepository.Get(a => a.ProductId == item.ProductId).ToList())
            //    {
            //        uow.OrderProductRepository.Delete(orderproduct);

            //    }
            //    uow.Save(wsw.CurrentUserName);
            //    foreach (var orderproduct in uow.OrderProductRepository.Get(a => a.NextProductId == item.ProductId).ToList())
            //    {
            //        uow.OrderProductRepository.Delete(orderproduct);
            //    }
            //    uow.Save(wsw.CurrentUserName);
            //    uow.ProductRepository.Delete(item);
            //}
            processor.RetryProcessorIds = new List<string>();
            foreach (var item in uow.ProcessorCascadeRepository.Get(a => a.ProcessorId == id))
            {
                processor.RetryProcessorIds.Add(item.ProcessorRetryId.ToString());
            }
            processor.Status = 2;
            // uow.Save(wsw.CurrentUserName);
            uow.ProcessorRepository.Update(processor);
            uow.Save(wsw.CurrentUserName);
            return RedirectToAction("Index");
        }

        public ActionResult ReplaceProcessor(int newPid, int oldPid)
        {

            foreach (var item in uow.ProcessorCascadeRepository.Get(a => a.ProcessorId == oldPid).ToList())
            {
                item.ProcessorId = newPid;
                uow.ProcessorCascadeRepository.Update(item);
            }
            uow.Save(wsw.CurrentUserName);

            foreach (var item in uow.TransactionRepository.Get(a => a.ProcessorId == oldPid).ToList())
            {
                item.ProcessorId = newPid;

                uow.TransactionRepository.Update(item);

            }
            uow.Save(wsw.CurrentUserName);
            foreach (var item in uow.ProductRepository.Get(a => a.ProcessorId == oldPid).ToList())
            {
                item.PaymentTypeIds = new List<string>();
                item.PaymentTypeIds = item.PaymentTypeIds;

                item.ProcessorId = newPid;
                uow.ProductRepository.Update(item);
                uow.Save(wsw.CurrentUserName);
            }

            var processor = uow.ProcessorRepository.Find(oldPid);

            if (processor == null)
            {
                return HttpNotFound();
            }
            processor.RetryProcessorIds = new List<string>();
            foreach (var item in uow.ProcessorCascadeRepository.Get(a => a.ProcessorId == oldPid))
            {
                processor.RetryProcessorIds.Add(item.ProcessorRetryId.ToString());
            }
            uow.Save(wsw.CurrentUserName);
            foreach (var item in uow.BalancerProcessorRepository.Get(a => a.ProcessorId == oldPid))
            {
                item.ProcessorId = newPid;
                uow.BalancerProcessorRepository.Update(item);
            }
            uow.Save(wsw.CurrentUserName);

            processor.Status = 2;
            // uow.Save(wsw.CurrentUserName);
            uow.ProcessorRepository.Update(processor);
            uow.Save(wsw.CurrentUserName);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public JsonResult GetProductsAndBalancer(int id)
        {
            var processor = uow.ProcessorRepository.Find(id);

            if (processor == null)
            {
                return new JsonResult();
            }
            var ProcessorProducts = uow.ProductRepository.Get(a => a.ProcessorId == id).ToList();
            var BalancerProcessors = uow.BalancerProcessorRepository.Get(a => a.ProcessorId == id).ToList();
            var replacableProcessor = uow.ProcessorRepository.Get(c => c.Id != id && c.Status!=2);

            ViewBag.AvailableProcessor = replacableProcessor;
            var genericResult = new { AvailableProcessor = replacableProcessor };//, OrphanProduct = ProcessorProducts, OrphanBalancer = BalancerProcessors };

            return Json(genericResult, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 
        /// Dispose
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }

}
