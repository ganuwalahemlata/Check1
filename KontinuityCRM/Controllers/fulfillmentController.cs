using KontinuityCRM.Models;
using KontinuityCRM.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KontinuityCRM.Models.Fulfillments;
using AutoMapper;
using System.Linq.Expressions;
using KontinuityCRM.Filters;

namespace KontinuityCRM.Controllers
{
    [CustomAuthorizeAttribute]
    [System.ComponentModel.DisplayName("Fulfillment Providers")]
    public class fulfillmentController : BaseController
    {
        private readonly IMappingEngine mapper;

        public fulfillmentController(IUnitOfWork uow, IWebSecurityWrapper wsw, IMappingEngine mapper) : base(uow, wsw)
        {
            this.mapper = mapper;
        }


        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? display)
        {
            var type = typeof(Fulfillment);

            ViewBag.Providers = type.Assembly.GetTypes()
              .Where(p => type.IsAssignableFrom(p) && !p.IsAbstract)
              .Select(p => new SelectListItem
              {
                  Value = p.Name,
                  Text = p.DisplayClassName(),
              });

            var user = uow.UserProfileRepository.Find(wsw.CurrentUserId);
            int pageSize = user.FulfillmentDisplay;

            if (display.HasValue && display != pageSize)
            {
                page = 1;
                pageSize = display.Value;

                user.FulfillmentDisplay = pageSize;

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
            ViewBag.dateSortParm = sortOrder == "date" ? "date_desc" : "date";

            ViewBag.dateOrderIcon = "sort";
            ViewBag.nameOrderIcon = "sort";
            ViewBag.aliasOrderIcon = "sort";
            ViewBag.idOrderIcon = "sort";

            Func<IQueryable<FulfillmentProvider>, IOrderedQueryable<FulfillmentProvider>> orderBy = o =>
            {
                switch (sortOrder)
                {
                    case "id_asc":
                        ViewBag.idOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Id);

                    case "name":
                        ViewBag.nameOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Type).ThenByDescending(c => c.Id);
                    case "name_desc":
                        ViewBag.nameOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Type).ThenByDescending(c => c.Id);

                    case "alias":
                        ViewBag.aliasOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Alias).ThenByDescending(c => c.Id);
                    case "alias_desc":
                        ViewBag.aliasOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Alias).ThenByDescending(c => c.Id);
                    
                    case "date":
                        ViewBag.dateOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.CreatedDate).ThenByDescending(c => c.Id);
                    case "date_desc":
                        ViewBag.dateOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.CreatedDate).ThenByDescending(c => c.Id);


                    default:
                        ViewBag.idOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Id);
                }
            };


            ViewBag.CurrentFilter = searchString;

            Expression<Func<FulfillmentProvider, bool>> filter = null;

            if (!String.IsNullOrEmpty(searchString))
            {
                filter = f => f.Alias.Contains(searchString)
                    //|| f.Name.Contains(searchString)
                    //|| f.Address1.Contains(searchString)
                    //|| f.Address2.Contains(searchString)
                    //|| f.Country.Contains(searchString)
                    //|| f.IPAddress.Contains(searchString)
                    || f.Type.Contains(searchString)
                    //|| f.Phone.Contains(searchString)
                    ;
            }



            ViewBag.Display = pageSize;

            int pageNumber = (page ?? 1);

            var model = uow.FulfillmentProviderRepository
                .GetPage(pageSize, pageNumber,
                //out count,
                filter: filter,
                orderBy: orderBy
                );

            var pageList = new PagedListMapper<FulfillmentProvider>(model, model.CloneProps().ConvertUtcTime(TimeZoneUtility.GetTimeZone(uow.TimeZoneRepository.Find(user.TimeZoneId).Name)));
            return View(pageList);

            //return View(uow.FulfillmentProviderRepository.Get());
        }
                
        public ActionResult Create(string id)
        {
            Type providertype;

            try
            {
                providertype = Type.GetType("KontinuityCRM.Models.Fulfillments." + id);
            }
            catch
            {
                return HttpNotFound();
            }

            ViewBag.Title = string.Format("{0}", providertype.DisplayClassName());
            return View(Activator.CreateInstance(providertype));
        }

        //
        // POST: /shipping/Create

        [HttpPost]
        public ActionResult Create(Fulfillment fp)
        {
            if (ModelState.IsValid)
            {
                var provider = fp.Provider(mapper);
                
                provider.CreatedDate = DateTime.UtcNow;
                              
                uow.FulfillmentProviderRepository.Add(provider);
                uow.Save(wsw.CurrentUserName);
                
                return RedirectToAction("Index");
            }

            ViewBag.Title = string.Format("{0}", fp.GetType().DisplayClassName());
            return View(fp);
        }

        //
        // GET: /shipping/Edit/5

        public ActionResult Edit(int id)
        {
            var fp = uow.FulfillmentProviderRepository.Find(id); 
            if (fp == null)
            {
                return HttpNotFound();
            }
          
            return View(fp.Fulfillment(mapper));
        }

        //
        // POST: /shipping/Edit/5

        [HttpPost]
        public ActionResult Edit(Fulfillment fp)
        {
            if (ModelState.IsValid)
            {
                uow.FulfillmentProviderRepository.Update(fp.Provider(mapper));
                uow.Save(wsw.CurrentUserName);
                return RedirectToAction("Index");
            }
           
            return View(fp);
        }

        //
        // GET: /shipping/Delete/5

        public ActionResult Delete(int id)
        {
            var fp = uow.FulfillmentProviderRepository.Find(id);
            if (fp == null)
            {
                return HttpNotFound();
            }           
          
            uow.FulfillmentProviderRepository.Delete(fp);
            uow.Save(wsw.CurrentUserName);
           
            return RedirectToAction("Index");
        }

        //
        // POST: /shipping/Delete/5

        //[HttpPost, ActionName("Delete")]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    var entity = repo.FindFulfillmentProvider(id);
        //    repo.RemoveFulfillmentProvider(entity);
        //    return RedirectToAction("Index");
        //}


    }
}
