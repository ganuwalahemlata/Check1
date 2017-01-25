using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KontinuityCRM.Helpers;
using KontinuityCRM.Models;
using System.Transactions;
using System.Linq.Expressions;

namespace KontinuityCRM.Controllers
{
    [Authorize]
    [System.ComponentModel.DisplayName("Balancers")]
    public class balancerController : BaseController
    {
        public balancerController(IUnitOfWork uow, IWebSecurityWrapper wsw) : base(uow, wsw)
        {

        }
        
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? display)
        {
            var user = uow.UserProfileRepository.Find(wsw.CurrentUserId);
            int pageSize = user.BalancerDisplay;

            if (display.HasValue && display != pageSize)
            {
                page = 1;
                pageSize = display.Value;

                user.BalancerDisplay = pageSize;

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

            ViewBag.nameOrderIcon = "sort";
            ViewBag.idOrderIcon = "sort";

            Func<IQueryable<Balancer>, IOrderedQueryable<Balancer>> orderBy = o =>
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

                    default:
                        ViewBag.idOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Id);
                }
            };


            ViewBag.CurrentFilter = searchString;

            Expression<Func<Balancer, bool>> filter = null;

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

            var model = uow.BalancerRepository
                .GetPage(pageSize, pageNumber,
                //out count,
                filter: filter,
                orderBy: orderBy
                );

            return View(model);
            //return View(uow.BalancerRepository.Get());
        }
        
        public ActionResult Create()
        {
            ViewBag.Processors = uow.ProcessorRepository.Get();
            return View(new Balancer 
            { 
               
            });
        }

        //
        // POST: /category/Create

        [HttpPost]
        public ActionResult Create(Balancer balancer)
        {
            if (balancer.BalancerProcessors == null || balancer.BalancerProcessors.Count() < 2)
            {
                ModelState.AddModelError("", "A Balancer must have more than one processor");
            }

            // doesn't get in here don't work!!
            //foreach (var bp in balancer.BalancerProcessors) // for validation purpose
            //{
            //    bp.Balancer = balancer;
            //}

            // raw server validation
            else if (balancer.AllocationBalance)
            {
                foreach (var bp in balancer.BalancerProcessors)
                {
                    if (!bp.InitialLimit.HasValue)
                    {
                        ModelState.AddModelError("BalancerProcessors[" + bp.ProcessorId + "].IniatialLimit", "The Initial Limit field is required");
                        break;
                    }
                    
                }

                foreach (var bp in balancer.BalancerProcessors)
                {
                    if (!bp.Allocation.HasValue)
                    {
                        ModelState.AddModelError("BalancerProcessors[" + bp.ProcessorId + "].Allocation", "The Allocation field is required");
                        break;
                    }
                }
            }
            else 
            {
                foreach (var bp in balancer.BalancerProcessors)
                {
                    if (!bp.AllocationPercent.HasValue)
                    {
                        //BalancerProcessors[3].AllocationPercent
                        ModelState.AddModelError("BalancerProcessors[" + bp.ProcessorId + "].AllocationPercent", "The Allocation Percent field is required");
                        break;
                        //ModelState.AddModelError("AllocationPercent", "The Allocation Percent field is required");
                    }
                }
            
            }
            

            if (ModelState.IsValid)
            {
                
                uow.BalancerRepository.Add(balancer);
                uow.Save();
                   
                return RedirectToAction("Index");
            }
            
            ViewBag.Processors = uow.ProcessorRepository.Get();
            return View(balancer);
        }

        //
        // GET: /category/Edit/5

        public ActionResult Edit(int id)
        {
            var balancer = uow.BalancerRepository.Find(id);
            if (balancer == null)
            {
                return HttpNotFound();
            }
            ViewBag.Processors = uow.ProcessorRepository.Get();
            return View(balancer);
        }

        //
        // POST: /category/Edit/5

        [HttpPost]
        public ActionResult Edit(Balancer balancer)
        {
            if (balancer.BalancerProcessors == null || balancer.BalancerProcessors.Count() < 2)
            {
                ModelState.AddModelError("", "A Balancer must have more than one processor");
            }

            // doesn't get in here don't work!!
            //foreach (var bp in balancer.BalancerProcessors) // for validation purpose
            //{
            //    bp.Balancer = balancer;
            //}

            // raw server validation
            else if (balancer.AllocationBalance)
            {
                foreach (var bp in balancer.BalancerProcessors)
                {
                    if (!bp.InitialLimit.HasValue)
                    {
                        ModelState.AddModelError("BalancerProcessors[" + bp.ProcessorId + "].IniatialLimit", "The Initial Limit field is required");
                        break;
                    }

                }

                foreach (var bp in balancer.BalancerProcessors)
                {
                    if (!bp.Allocation.HasValue)
                    {
                        ModelState.AddModelError("BalancerProcessors[" + bp.ProcessorId + "].Allocation", "The Allocation field is required");
                        break;
                    }
                }
            }
            else
            {
                foreach (var bp in balancer.BalancerProcessors)
                {
                    if (!bp.AllocationPercent.HasValue)
                    {
                        //BalancerProcessors[3].AllocationPercent
                        ModelState.AddModelError("BalancerProcessors[" + bp.ProcessorId + "].AllocationPercent", "The Allocation Percent field is required");
                        break;
                        //ModelState.AddModelError("AllocationPercent", "The Allocation Percent field is required");
                    }
                }

            }

            if (ModelState.IsValid)
            {
                // delete every processor in this balancer
                foreach (var bp in uow.BalancerProcessorRepository.Get(b => b.BalancerId == balancer.Id))
                    uow.BalancerProcessorRepository.Delete(bp);
                uow.Save();

                foreach (var bp in balancer.BalancerProcessors)
                    uow.BalancerProcessorRepository.Add(bp);
                uow.BalancerRepository.Update(balancer); //repo.EditBalancer(balancer);
                uow.Save();
                return RedirectToAction("Index");
            }
            ViewBag.Processors = uow.ProcessorRepository.Get();
            return View(balancer);
        }

        //
        // GET: /category/Delete/5

        public ActionResult Delete(int id)
        {
            var balancer = uow.BalancerRepository.Find(id); // repo.FindBalancer(id);
            if (balancer == null)
            {
                return HttpNotFound();
            }

            uow.BalancerRepository.Delete(balancer); //repo.RemoveBalancer(balancer);
            uow.Save();
            return RedirectToAction("Index");
            //return View(balancer);
        }

        //
        // POST: /category/Delete/5

        //[HttpPost, ActionName("Delete")]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Balancer balancer = repo.FindBalancer(id);
        //    repo.RemoveBalancer(balancer);
        //    return RedirectToAction("Index");
        //}

        /*=====================================Processors============================================*/

        //public ActionResult Processors(int id)
        //{
        //    var balancer = repo.FindBalancer(id);
        //    if (balancer == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.Balancer = balancer;
        //    //return View(repo.BalancerProcessors().Where(b => b.BalancerId == id));

        //    return View(balancer.BalacerProcessors);
        //}

        //public ActionResult CreateProcessor(int id)
        //{
        //    var balancer = repo.FindBalancer(id);
        //    if (balancer == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.Balancer = balancer;

        //    var inprocessors = repo.BalancerProcessors().Where(b => b.BalancerId == id).Select(b => b.ProcessorId);

        //    var processors = repo.Processors().Where(p => !inprocessors.Contains(p.Id));
        //    ViewBag.Processors = processors;
        //    ViewBag.ProcessorCount = processors.Count();
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult CreateProcessor(BalancerProcessor bp)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        repo.AddBalancerProcessor(bp);
        //        return RedirectToAction("processors", new { id = bp.BalancerId });
        //    }
        //    ViewBag.Balancer = repo.FindBalancer(bp.BalancerId);
        //    ViewBag.Processors = repo.Processors();
        //    return View(bp);
        //}

        //public ActionResult EditProcessor(int id)
        //{
        //    var bp = repo.FindBalancerProcessor(id);
        //    if (bp == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.Balancer = bp.Balancer;
        //    //ViewBag.Processors = repo.Processors();
        //    return View(bp);
        //}

        //[HttpPost]
        //public ActionResult EditProcessor(BalancerProcessor bp)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        repo.EditBalancerProcessor(bp);
        //        return RedirectToAction("processors", new { id = bp.BalancerId });
        //    }
        //    ViewBag.Balancer = repo.FindBalancer(bp.BalancerId);
        //    //ViewBag.Processors = repo.Processors();
        //    return View(bp);
        //}

        //public ActionResult DeleteProcessor(int id)
        //{
        //    var bp = repo.FindBalancerProcessor(id);
        //    if (bp == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(bp);
        //}

        //[HttpPost, ActionName("DeleteProcessor")]
        //public ActionResult DeleteProcessorConfirmed(int id)
        //{
        //    BalancerProcessor bp = repo.FindBalancerProcessor(id);
        //    repo.RemoveBalancerProcessor(bp);
        //    return RedirectToAction("processors", new { id = bp.BalancerId });
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    repo.Dispose();
        //    base.Dispose(disposing);
        //}
    }
}
