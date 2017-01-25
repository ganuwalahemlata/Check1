using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KontinuityCRM.Models;
using KontinuityCRM.Models.AutoResponders;
using KontinuityCRM.Helpers;
using AutoMapper;
using System.Linq.Expressions;
using KontinuityCRM.Filters;

namespace KontinuityCRM.Controllers
{
    [CustomAuthorizeAttribute]
    [System.ComponentModel.DisplayName("Auto Responder Providers")]
    public class AutoResponderController : BaseController
    {
        public readonly IMappingEngine mapper;

        public AutoResponderController(IUnitOfWork uow, IWebSecurityWrapper wsw, IMappingEngine mapper) : base(uow, wsw)
        {
            this.mapper = mapper;
        }

        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? display)
        {
            var type = typeof(AutoResponder);
          
            ViewBag.Providers = type.Assembly.GetTypes()  // System.Reflection.Assembly.GetExecutingAssembly()
              .Where(p => type.IsAssignableFrom(p) && !p.IsAbstract)
              .Select(p => new SelectListItem 
              { 
                  Value = p.Name,
                  Text = p.DisplayClassName(),
              })
              .OrderBy(s => s.Text)
              ;

            var user = uow.UserProfileRepository.Find(wsw.CurrentUserId);
            int pageSize = user.AutoresponderDisplay;

            if (display.HasValue && display != pageSize)
            {
                page = 1;
                pageSize = display.Value;

                user.AutoresponderDisplay = pageSize;

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

            Func<IQueryable<AutoResponderProvider>, IOrderedQueryable<AutoResponderProvider>> orderBy = o =>
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

            Expression<Func<AutoResponderProvider, bool>> filter = null;

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

            var model = uow.AutoResponderProviderRepository
                .GetPage(pageSize, pageNumber,
                //out count,
                filter: filter,
                orderBy: orderBy
                );

            var pageList = new PagedListMapper<AutoResponderProvider>(model, model.CloneProps().ConvertUtcTime(TimeZoneUtility.GetTimeZone(uow.TimeZoneRepository.Find(user.TimeZoneId).Name)));
            return View(pageList);

            //return View(uow.AutoResponderProviderRepository.Get());
        }

        public ActionResult Create(string id) // is the provider
        {
            Type providertype;

            try
            {
                providertype = Type.GetType("KontinuityCRM.Models.AutoResponders." + id);
            }
            catch 
            {
                return HttpNotFound();
            }
           
            ViewBag.Title =  string.Format("{0}", providertype.DisplayClassName());
            return View(Activator.CreateInstance(providertype));
            
        }
        
        [HttpPost]        
        public ActionResult Create(AutoResponder responder)
        {
            if (ModelState.IsValid)
            {
                var provider = responder.Provider(mapper); 
                provider.CreatedDate = DateTime.UtcNow;
                uow.AutoResponderProviderRepository.Add(provider);
                uow.Save(wsw.CurrentUserName);
                return RedirectToAction("Index");
            }
            
            ViewBag.Title = string.Format("{0}", responder.GetType().DisplayClassName());
            return View(responder);
        }

        public ActionResult Edit(int id)
        {
            var responder = uow.AutoResponderProviderRepository.Find(id); //repo.FindAutoResponderProvider(id);
            if (responder == null)
            {
                return HttpNotFound();
            }
           
            return View(responder.AutoResponder(mapper));  
        }

        
        [HttpPost]
        public ActionResult Edit(AutoResponder responder)
        {
            if (ModelState.IsValid)
            {
                uow.AutoResponderProviderRepository.Update(responder.Provider(mapper));
                uow.Save(wsw.CurrentUserName);

                return RedirectToAction("Index");
            }
           
            return View(responder);
        }

       
        public ActionResult Delete(int id)
        {
            var responder = uow.AutoResponderProviderRepository.Find(id); 
            if (responder == null)
            {
                return HttpNotFound();
            }
           
            uow.AutoResponderProviderRepository.Delete(responder); 
            uow.Save(wsw.CurrentUserName);
            return RedirectToAction("Index");
        }

    }
}
