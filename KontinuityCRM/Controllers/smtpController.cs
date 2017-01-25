using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KontinuityCRM.Models;
using KontinuityCRM.Helpers;
using System.Linq.Expressions;
using KontinuityCRM.Filters;
using PagedList;

namespace KontinuityCRM.Controllers
{
    [CustomAuthorizeAttribute]
    [System.ComponentModel.DisplayName("SMTP Mail Servers")]
    public class smtpController : BaseController
    {

        public smtpController(IUnitOfWork uow, IWebSecurityWrapper wsw)
            : base(uow, wsw)
        {
                
        }
        /// <summary>
        /// Redirects to the configured SMTP Servers View
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sortOrder"></param>
        /// <param name="currentFilter"></param>
        /// <param name="searchString"></param>
        /// <param name="page"></param>
        /// <param name="display"></param>
        /// <returns></returns>
        public ActionResult Index(int? id, string sortOrder, string currentFilter, string searchString, int? page, int? display)
        {
            var user = uow.UserProfileRepository.Find(wsw.CurrentUserId);
            int pageSize = user.SmtpServerDisplay;

            if (display.HasValue && display != pageSize)
            {
                page = 1;
                pageSize = display.Value;

                user.SmtpServerDisplay = pageSize;

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
            ViewBag.emailSortParm = sortOrder == "email" ? "email_desc" : "email";
            ViewBag.lastupdateSortParm = sortOrder == "lastupdate" ? "lastupdate_desc" : "lastupdate";
            ViewBag.updatedbySortParm = sortOrder == "updatedby" ? "updatedby_desc" : "updatedby";
            ViewBag.authenticatedSortParm = sortOrder == "authenticated" ? "authenticated_desc" : "authenticated";
            ViewBag.publishSortParm = sortOrder == "publish" ? "publish_desc" : "publish";

            ViewBag.publishOrderIcon = "sort";
            ViewBag.authenticatedOrderIcon = "sort";
            ViewBag.updatedbyOrderIcon = "sort";
            ViewBag.lastupdateOrderIcon = "sort";
            ViewBag.nameOrderIcon = "sort";
            ViewBag.emailOrderIcon = "sort";
            ViewBag.idOrderIcon = "sort";

            Func<IQueryable<SmtpServer>, IOrderedQueryable<SmtpServer>> orderBy = o =>
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

                    case "email":
                        ViewBag.emailOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Email).ThenByDescending(c => c.Id);
                    case "email_desc":
                        ViewBag.emailOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Email).ThenByDescending(c => c.Id);

                    case "authenticated":
                        ViewBag.authenticatedOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Authenticated).ThenByDescending(c => c.Id);
                    case "authenticated_desc":
                        ViewBag.authenticatedOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Authenticated).ThenByDescending(c => c.Id);

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

            Expression<Func<SmtpServer, bool>> filter = f => true;

            if (!String.IsNullOrEmpty(searchString))
            {
                filter = f => f.Name.Contains(searchString)
                    || f.Email.Contains(searchString)
                    || f.Host.Contains(searchString)
                    //|| f.Address2.Contains(searchString)
                    //|| f.Country.Contains(searchString)
                    //|| f.IPAddress.Contains(searchString)
                    //|| f.Type.Contains(searchString)
                    //|| f.Phone.Contains(searchString)
                    ;
            }


            Expression<Func<SmtpServer, bool>> filter2 = f => !id.HasValue || id == f.Id;

            var lambda = filter2.AndAlso(filter);



            ViewBag.Display = pageSize;

            int pageNumber = (page ?? 1);

            var model = uow.SmtpServerRepository
                .GetPage(pageSize, pageNumber,
                //out count,
                filter: lambda,
                orderBy: orderBy
                );

            var pageList = new PagedListMapper<SmtpServer>(model, model.CloneProps().ConvertUtcTime(TimeZoneUtility.GetTimeZone(uow.TimeZoneRepository.Find(user.TimeZoneId).Name)));
            return View(pageList);

            //if (id.HasValue)
            //{
            //    return View(repo.SmtpServers().Where(s => s.Id == id));
            //}
            
            //return View(repo.SmtpServers());
        }
        /// <summary>
        /// Return View to Create New SmtpServer
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /smtp/Create
        /// <summary>
        /// Post Action to create new SmtpServer
        /// </summary>
        /// <param name="server"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(SmtpServer server)
        {
            if (ModelState.IsValid)
            {
                uow.SmtpServerRepository.Add(server);
                uow.Save(wsw.CurrentUserName);
                return RedirectToAction("Index");
            }
            
            return View(server);
        }

        //
        // GET: /smtp/Edit/5
        /// <summary>
        /// Returns View to Edit SmtpServer with id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            var server = uow.SmtpServerRepository.Find(id);

            if (server == null)
            {
                return HttpNotFound();
            }

            return View(server);
        }

        //
        // POST: /smtp/Edit/5
        /// <summary>
        /// Post action for editing smtp Server
        /// </summary>
        /// <param name="server"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(SmtpServer server)
        {
            if (ModelState.IsValid)
            {
                server.LastUpdate = DateTime.UtcNow;
                server.UpdatedUserId = wsw.CurrentUserId;
                uow.SmtpServerRepository.Update(server);
                uow.Save(wsw.CurrentUserName);
                return RedirectToAction("Index");
            }
            
            return View(server);
        }

        //
        // GET: /smtp/Delete/5
        /// <summary>
        /// Delete Smtp Server with id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int id)
        {
            var server = uow.SmtpServerRepository.Find(id);

            if (server == null)
            {
                return HttpNotFound();
            }
           
            uow.SmtpServerRepository.Delete(server);
            uow.Save(wsw.CurrentUserName);
            return RedirectToAction("Index");
        }

        //
        // POST: /smtp/Delete/5

        //[HttpPost]
        //public ActionResult Delete(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add delete logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
    }
}
