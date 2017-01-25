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
    [System.ComponentModel.DisplayName("Email Templates")]
    public class EmailTemplateController : BaseController
    {
        public EmailTemplateController(IUnitOfWork uow, IWebSecurityWrapper wsw) : base(uow, wsw)
        {

        }

        public ActionResult Index(int? id, string sortOrder, string currentFilter, string searchString, int? page, int? display)
        {
            var user = uow.UserProfileRepository.Find(wsw.CurrentUserId);
            int pageSize = user.EmailTemplateDisplay;

            if (display.HasValue && display != pageSize)
            {
                page = 1;
                pageSize = display.Value;

                user.EmailTemplateDisplay = pageSize;

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
            ViewBag.publishSortParm = sortOrder == "publish" ? "publish_desc" : "publish";

            ViewBag.publishOrderIcon = "sort";
            ViewBag.updatedbyOrderIcon = "sort";
            ViewBag.lastupdateOrderIcon = "sort";
            ViewBag.nameOrderIcon = "sort";
            ViewBag.typeOrderIcon = "sort";
            ViewBag.idOrderIcon = "sort";

            Func<IQueryable<EmailTemplate>, IOrderedQueryable<EmailTemplate>> orderBy = o =>
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

                    case "publish":
                        ViewBag.publishOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Publish).ThenByDescending(c => c.Id);
                    case "publish_desc":
                        ViewBag.publishOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Publish).ThenByDescending(c => c.Id);

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

            Expression<Func<EmailTemplate, bool>> filter = f => true;

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


            Expression<Func<EmailTemplate, bool>> filter2 = f => !id.HasValue || id == f.Id;

            var lambda = filter2.AndAlso(filter);



            ViewBag.Display = pageSize;

            int pageNumber = (page ?? 1);

            var model = uow.EmailTemplateRepository
                .GetPage(pageSize, pageNumber,
                //out count,
                filter: lambda,
                orderBy: orderBy
                );

            var pageList = new PagedListMapper<EmailTemplate>(model, model.CloneProps().ConvertUtcTime(TimeZoneUtility.GetTimeZone(uow.TimeZoneRepository.Find(user.TimeZoneId).Name)));
            return View(pageList);

            //return View(uow.EmailTemplateRepository.Get(e => !id.HasValue || e.Id == id));
          
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(EmailTemplate email)
        {
            //email.LastUpdate = DateTime.UtcNow;
            //email.UpdatedBy = repo.FindUserProfile(CurrentUserId);

            if (ModelState.IsValid)
            {
                email.CreatedUserId = wsw.CurrentUserId;
                
                uow.EmailTemplateRepository.Add(email);
                uow.Save(wsw.CurrentUserName);
                return RedirectToAction("Index");
            }
            return View(email);
        }

        public ActionResult Edit(int id)
        {
            var email = uow.EmailTemplateRepository.Find(id); //repo.FindEmailTemplate(id);
            if (email == null)
            {
                return HttpNotFound();
            }
            return View(email);
        }

        [HttpPost]
        public ActionResult Edit(EmailTemplate email)
        {
            if (ModelState.IsValid)
            {
                email.LastUpdate = DateTime.UtcNow;
                email.UpdatedUserId = wsw.CurrentUserId;

                uow.EmailTemplateRepository.Update(email);
                uow.Save(wsw.CurrentUserName);
                return RedirectToAction("Index");
            }
            return View(email);
        }

        public ActionResult Delete(int id)
        {
            var email = uow.EmailTemplateRepository.Find(id);
            if (email == null)
            {
                return HttpNotFound();
            }
            
            uow.EmailTemplateRepository.Delete(email);
            uow.Save(wsw.CurrentUserName);
            return RedirectToAction("Index");
        }
    }
}
