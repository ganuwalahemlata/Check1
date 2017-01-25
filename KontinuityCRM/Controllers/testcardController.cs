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
    [System.ComponentModel.DisplayName("Test Cards")]
    public class testcardController : BaseController
    {
        public testcardController(IUnitOfWork uow, IWebSecurityWrapper wsw)
            : base(uow, wsw)
        {

        }
        
        // GET: testcardnumber
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? display)
        {
            var user = uow.UserProfileRepository.Find(wsw.CurrentUserId);
            int pageSize = user.TestCardDisplay;

            if (display.HasValue && display != pageSize)
            {
                page = 1;
                pageSize = display.Value;

                user.TestCardDisplay = pageSize;
                uow.UserProfileRepository.Update(user);
                uow.Save(wsw.CurrentUserName);
            }

            if (searchString != null)
                page = 1;
            else
                searchString = currentFilter;

            ViewBag.CurrentFilter = searchString;

            ViewBag.CurrentSort = sortOrder;
            ViewBag.CardNumberSortParm = String.IsNullOrEmpty(sortOrder) ? "CardNumber_desc" : "";

            ViewBag.CardNumberOrderIcon = "sort";

            Func<IQueryable<TestCardNumber>, IOrderedQueryable<TestCardNumber>> orderBy = o =>
            {
                switch (sortOrder)
                {
                    case "CardNumber_desc":
                        ViewBag.CardNumberOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Number);

                    default:
                        ViewBag.CardNumberOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Number);
                }
            };

            Expression<Func<TestCardNumber, bool>> filter = null;

            if (!String.IsNullOrEmpty(searchString))
            {
                filter = f => f.Number.Contains(searchString);
            }

            ViewBag.Display = pageSize;

            int pageNumber = (page ?? 1);

            var model = uow.TestCardNumberRepository
                .GetPage(pageSize, pageNumber,
                filter: filter,
                orderBy: orderBy
                );

            return View(model);


            //var testcards = uow.TestCardNumberRepository.Get();
            //return View(testcards);
        }

        /// <summary>
        /// Returns view for creating TestCardNumber
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            return View();
        }
        /// <summary>
        /// Post Action to create New TestCardNumber
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(TestCardNumber card)
        {
            if (ModelState.IsValid)
            {
                uow.TestCardNumberRepository.Add(card);
                uow.Save(wsw.CurrentUserName);
                return RedirectToAction("Index");
            }

            return View(card);
        }

        /// <summary>
        /// Returns View for Editing TestCardNumber
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            var card = uow.TestCardNumberRepository.Find(id);
            if (card == null)
            {
                return HttpNotFound();
            }
            return View(card);
        }
        /// <summary>
        /// Post Action for Updating TestCardNumber
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(TestCardNumber card)
        {
            if (ModelState.IsValid)
            {
                uow.TestCardNumberRepository.Update(card);
                uow.Save(wsw.CurrentUserName);
                return RedirectToAction("Index");
            }
            return View(card);
        }
        /// <summary>
        /// Delete TestCardNumber with id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int id)
        {
            var card = uow.TestCardNumberRepository.Find(id);
            if (card == null)
            {
                return HttpNotFound();
            }
            uow.TestCardNumberRepository.Delete(card);
            uow.Save(wsw.CurrentUserName);
            return RedirectToAction("Index");

        }
    }
}