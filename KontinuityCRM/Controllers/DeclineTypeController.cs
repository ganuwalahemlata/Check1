using KontinuityCRM.Filters;
using KontinuityCRM.Helpers;
using KontinuityCRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace KontinuityCRM.Controllers
{
    [CustomAuthorizeAttribute]
    public class DeclineTypeController : BaseController
    {
        public DeclineTypeController(IUnitOfWork uow, IWebSecurityWrapper wsw)
            : base(uow, wsw)
        {

        }

        
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? display)
        {
            var user = uow.UserProfileRepository.Find(wsw.CurrentUserId);
            int pageSize = user.DeclineTypeDisplay;

            if (display.HasValue && display != pageSize)
            {
                page = 1;
                pageSize = display.Value;

                user.DeclineTypeDisplay = pageSize;

                uow.UserProfileRepository.Update(user);
                uow.Save(wsw.CurrentUserName);
            }
            if (searchString != null)
                page = 1;
            else
                searchString = currentFilter;


            ViewBag.CurrentSort = sortOrder;
            ViewBag.IdSortParm = String.IsNullOrEmpty(sortOrder) ? "Id_asc" : "";
            ViewBag.NameSortParm = sortOrder == "Name" ? "Name_desc" : "Name";
            ViewBag.WildcardSortParm = sortOrder == "Wildcard" ? "Wildcard_desc" : "Wildcard";

            ViewBag.NameOrderIcon = "sort";
            ViewBag.WildcardOrderIcon = "sort";
            ViewBag.IdOrderIcon = "sort";

            Func<IQueryable<DeclineType>, IOrderedQueryable<DeclineType>> orderBy = o =>
            {
                switch (sortOrder)
                {
                    case "Id_asc":
                        ViewBag.IdOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Id);

                    case "Name":
                        ViewBag.NameOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Name).ThenByDescending(c => c.Id);
                    case "Name_desc":
                        ViewBag.NameOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Name).ThenByDescending(c => c.Id);

                    case "Wildcard":
                        ViewBag.WildcardOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.WildCard).ThenByDescending(c => c.Id);
                    case "Wildcard_desc":
                        ViewBag.WildcardOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.WildCard).ThenByDescending(c => c.Id);


                    default:
                        ViewBag.idOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Id);
                }
            };


            ViewBag.CurrentFilter = searchString;

            Expression<Func<DeclineType, bool>> filter = null;

            if (!String.IsNullOrEmpty(searchString))
            {
                filter = f => f.Name.Contains(searchString)
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

            var model = uow.DeclineTypeRepository
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

        [HttpPost]
        public ActionResult Create(DeclineType type)
        {
            if (ModelState.IsValid)
            {
                var defaultprofile = KontinuityCRMConfiguration.DefaultSalvageProfile;
                type.SalvageProfiles = new List<SalvageProfile>();
                type.SalvageProfiles.Add(defaultprofile);

                uow.DeclineTypeRepository.Add(type);
                uow.Save(wsw.CurrentUserName);
                return RedirectToAction("Index");
            }

            return View(type);
        }

        
        public ActionResult Edit(int id)
        {
            var declineType = uow.DeclineTypeRepository.Find(id);
            if (declineType == null)
            {
                return HttpNotFound();
            }
            return View(declineType);
        }

        // POST: DeclineType/Edit/5
        [HttpPost]
        public ActionResult Edit(DeclineType declineType)
        {
            if (ModelState.IsValid)
            {
                uow.DeclineTypeRepository.Update(declineType);
                uow.Save(wsw.CurrentUserName);
                return RedirectToAction("index");
            }

            return View(declineType);
        }

        // GET: DeclineType/Delete/5
        public ActionResult Delete(int id)
        {
            var declineType = uow.DeclineTypeRepository.Find(id);
            if (declineType == null)
            {
                return HttpNotFound();
            }
            uow.DeclineTypeRepository.Delete(declineType);
            uow.Save(wsw.CurrentUserName);
            return RedirectToAction("index");
        }
    }
}