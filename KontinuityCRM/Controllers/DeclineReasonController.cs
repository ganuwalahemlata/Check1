using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KontinuityCRM.Helpers;
using KontinuityCRM.Models;
using System.Linq.Expressions;
using KontinuityCRM.Filters;

namespace KontinuityCRM.Controllers
{
    [CustomAuthorizeAttribute]
    public class DeclineReasonController : 
        BaseController 
        
    {

        public DeclineReasonController(IUnitOfWork uow, IWebSecurityWrapper wsw) 
            : base(uow, wsw)
        {
            //this.uow = uow;
        }
        

        // GET: DeclineReason
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? display)
        {
            var user = uow.UserProfileRepository.Find(wsw.CurrentUserId);
            int pageSize = user.DeclineReasonDisplay;

            if (display.HasValue && display != pageSize)
            {
                page = 1;
                pageSize = display.Value;

                user.DeclineReasonDisplay = pageSize;

                uow.UserProfileRepository.Update(user);
                uow.Save(wsw.CurrentUserName);
            }
            if (searchString != null)
                page = 1;
            else
                searchString = currentFilter;

            ViewBag.CurrentSort = sortOrder;
            ViewBag.IdSortParm = String.IsNullOrEmpty(sortOrder) ? "id_asc" : "";
            ViewBag.ReasonSortParm = sortOrder == "reason" ? "reason_desc" : "reason";
            ViewBag.WildcardSortParm = sortOrder == "wildcard" ? "wildcard_desc" : "wildcard";

            ViewBag.reasonOrderIcon = "sort";
            ViewBag.wildcardOrderIcon = "sort";
            ViewBag.idOrderIcon = "sort";

            Func<IQueryable<DeclineReason>, IOrderedQueryable<DeclineReason>> orderBy = o =>
            {
                switch (sortOrder)
                {
                    case "id_asc":
                        ViewBag.idOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Id);

                    case "reason":
                        ViewBag.reasonOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Reason).ThenByDescending(c => c.Id);
                    case "reason_desc":
                        ViewBag.reasonOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Reason).ThenByDescending(c => c.Id);

                    case "wildcard":
                        ViewBag.wildcardOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.WildCard).ThenByDescending(c => c.Id);
                    case "wildcard_desc":
                        ViewBag.wildcardOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.WildCard).ThenByDescending(c => c.Id);


                    default:
                        ViewBag.idOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Id);
                }
            };


            ViewBag.CurrentFilter = searchString;

            Expression<Func<DeclineReason, bool>> filter = null;

            if (!String.IsNullOrEmpty(searchString))
            {
                filter = f => f.Reason.Contains(searchString)
                    //|| f.Name.Contains(searchString)
                    //|| f.Address1.Contains(searchString)
                    //|| f.Address2.Contains(searchString)
                    //|| f.Country.Contains(searchString)
                    //|| f.IPAddress.Contains(searchString)
                    || f.WildCard.Contains(searchString)
                    //|| f.Phone.Contains(searchString)
                    ;
            }



            ViewBag.Display = pageSize;

            int pageNumber = (page ?? 1);

            var model = uow.DeclineReasonRepository
                .GetPage(pageSize, pageNumber,
                //out count,
                filter: filter,
                orderBy: orderBy
                );

            return View(model);

            //return View(uow.DeclineReasonRepository.Get());
        }

        

        // GET: DeclineReason/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DeclineReason/Create
        [HttpPost]
        public ActionResult Create(DeclineReason declineReason)
        {
            if (ModelState.IsValid)
            {
                uow.DeclineReasonRepository.Add(declineReason);
                uow.Save(wsw.CurrentUserName);
                return RedirectToAction("index");
            }

            return View(declineReason);
        }

        // GET: DeclineReason/Edit/5
        public ActionResult Edit(int id)
        {
            var declineReason = uow.DeclineReasonRepository.Find(id);
            if (declineReason == null)
            {
                return HttpNotFound();
            }
            return View(declineReason);
        }

        // POST: DeclineReason/Edit/5
        [HttpPost]
        public ActionResult Edit(DeclineReason declineReason)
        {
            if (ModelState.IsValid)
            {
                uow.DeclineReasonRepository.Update(declineReason);
                uow.Save(wsw.CurrentUserName);
                return RedirectToAction("index");
            }

            return View(declineReason);
        }

        // GET: DeclineReason/Delete/5
        public ActionResult Delete(int id)
        {
            var declineReason = uow.DeclineReasonRepository.Find(id);
            if (declineReason == null)
            {
                return HttpNotFound();
            }
            uow.DeclineReasonRepository.Delete(declineReason);
            uow.Save(wsw.CurrentUserName);
            return RedirectToAction("index");
        }

        // POST: DeclineReason/Delete/5
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
