using KontinuityCRM.Models;
using KontinuityCRM.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Expressions;
using KontinuityCRM.Filters;

namespace KontinuityCRM.Controllers
{
    [CustomAuthorizeAttribute]
    [System.ComponentModel.DisplayName("Events")]
    public class eventController : BaseController
    {
        public eventController(IUnitOfWork uow, IWebSecurityWrapper wsw) : base(uow, wsw)
        {

        }

        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? display)
        {
            var user = uow.UserProfileRepository.Find(wsw.CurrentUserId);
            int pageSize = user.EventDisplay;

            if (display.HasValue && display != pageSize)
            {
                page = 1;
                pageSize = display.Value;

                user.EventDisplay = pageSize;

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
            ViewBag.typeSortParm = sortOrder == "type" ? "type_desc" : "type";
            ViewBag.lastupdateSortParm = sortOrder == "lastupdate" ? "lastupdate_desc" : "lastupdate";
            ViewBag.updatedbySortParm = sortOrder == "updatedby" ? "updatedby_desc" : "updatedby";
            ViewBag.templateSortParm = sortOrder == "template" ? "template_desc" : "template";
            ViewBag.createdbySortParm = sortOrder == "createdby" ? "createdby_desc" : "createdby";
            ViewBag.smtpSortParm = sortOrder == "smtp" ? "smtp_desc" : "smtp";
            ViewBag.productsSortParm = sortOrder == "products" ? "products_desc" : "products";

            ViewBag.productsOrderIcon = "sort";
            ViewBag.smtpOrderIcon = "sort";
            ViewBag.createdbyOrderIcon = "sort";
            ViewBag.templateOrderIcon = "sort";
            ViewBag.updatedbyOrderIcon = "sort";
            ViewBag.lastupdateOrderIcon = "sort";
            ViewBag.nameOrderIcon = "sort";
            ViewBag.typeOrderIcon = "sort";
            ViewBag.idOrderIcon = "sort";

            Func<IQueryable<Event>, IOrderedQueryable<Event>> orderBy = o =>
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

                    case "template":
                        ViewBag.templateOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Template.Name).ThenByDescending(c => c.Id);
                    case "template_desc":
                        ViewBag.templateOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Template.Name).ThenByDescending(c => c.Id);

                    case "products":
                        ViewBag.productsOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Products.Count()).ThenByDescending(c => c.Id);
                    case "products_desc":
                        ViewBag.productsOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Products.Count()).ThenByDescending(c => c.Id);

                    case "smtp":
                        ViewBag.smtpOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.SmtpServer.Name).ThenByDescending(c => c.Id);
                    case "smtp_desc":
                        ViewBag.smtpOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.SmtpServer.Name).ThenByDescending(c => c.Id);

                    case "createdby":
                        ViewBag.createdbyOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.CreatedBy.UserName).ThenByDescending(c => c.Id);
                    case "createdby_desc":
                        ViewBag.createdbyOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.CreatedBy.UserName).ThenByDescending(c => c.Id);

                    case "lastupdate":
                        ViewBag.lastupdateOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.LastUpdate).ThenByDescending(c => c.Id);
                    case "lastupdate_desc":
                        ViewBag.lastupdateOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.LastUpdate).ThenByDescending(c => c.Id);

                    case "updatedby":
                        ViewBag.updatedbyOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.UpdatedBy.UserName).ThenByDescending(c => c.Id);
                    case "updatedby_desc":
                        ViewBag.updatedbyOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.UpdatedBy.UserName).ThenByDescending(c => c.Id);


                    default:
                        ViewBag.idOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Id);
                }
            };


            ViewBag.CurrentFilter = searchString;

            Expression<Func<Event, bool>> filter = null;

            if (!String.IsNullOrEmpty(searchString))
            {
                filter = f => f.Name.Contains(searchString)
                    //|| f.Name.Contains(searchString)
                    //|| f.Address1.Contains(searchString)
                    //|| f.Address2.Contains(searchString)
                    //|| f.Country.Contains(searchString)
                    //|| f.IPAddress.Contains(searchString)
                    //|| f.Type.Contains(searchString)
                    //|| f.Phone.Contains(searchString)
                    ;
            }

            ViewBag.Display = pageSize;

            int pageNumber = (page ?? 1);

            var model = uow.EventRepository
                .GetPage(pageSize, pageNumber,
                //out count,
                filter: filter,
                orderBy: orderBy
                );

            var pageList = new PagedListMapper<Event>(model, model.CloneProps().ConvertUtcTime(TimeZoneUtility.GetTimeZone(uow.TimeZoneRepository.Find(user.TimeZoneId).Name)));
            return View(pageList);

            //return View(uow.EventRepository.Get());
        }        

        public ActionResult Create()
        {
            CreateViewBag();
            return View();
        }

        private void CreateViewBag()
        {
            ViewBag.Templates = uow.EmailTemplateRepository.Get(); // repo.EmailTemplates();
            ViewBag.SmtpServers = uow.SmtpServerRepository.Get(); // repo.SmtpServers();

        }

        

        [HttpPost]
        public ActionResult Create(Event e)
        {
            if (ModelState.IsValid)
            {
                e.CreatedUserId = wsw.CurrentUserId;                
                uow.EventRepository.Add(e);
                uow.Save(wsw.CurrentUserName);
                return RedirectToAction("Index");
            }
            CreateViewBag();
            return View(e);
        }

        
        public ActionResult Edit(int id)
        {
            Event e = uow.EventRepository.Find(id); 
            if (e == null)
            {
                return HttpNotFound();
            }
            CreateViewBag();
            return View(e);
        }

        

        [HttpPost]
        public ActionResult Edit(Event e)
        {
            if (ModelState.IsValid)
            {
                e.LastUpdate = DateTime.UtcNow;
                e.UpdatedUserId = wsw.CurrentUserId;
                
                uow.EventRepository.Update(e);
                uow.Save(wsw.CurrentUserName);
                return RedirectToAction("Index");
            }
            CreateViewBag();
            return View(e);
        }

        

        public ActionResult Delete(int id)
        {
            Event e = uow.EventRepository.Find(id);
            if (e == null)
            {
                return HttpNotFound();
            }
            uow.EventRepository.Delete(e);
            uow.Save(wsw.CurrentUserName);
            return RedirectToAction("Index");
        }
    }
   
}
