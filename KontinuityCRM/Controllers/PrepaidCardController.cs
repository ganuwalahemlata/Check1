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
    [System.ComponentModel.DisplayName("Prepaid Cards")]
    public class PrepaidCardController : BaseController
    {
        public PrepaidCardController(IUnitOfWork uow, IWebSecurityWrapper wsw) : base(uow, wsw)
        {

        }

        public ActionResult Index(int? id, string sortOrder, string currentFilter, string searchString, int? page, int? display)
        {
            var user = uow.UserProfileRepository.Find(wsw.CurrentUserId);
            int pageSize = user.PrepaidCardDisplay;

            if (display.HasValue && display != pageSize)
            {
                page = 1;
                pageSize = display.Value;
                user.PrepaidCardDisplay = pageSize;
                uow.UserProfileRepository.Update(user);
                uow.Save(wsw.CurrentUserName);
            }
            if (searchString != null)
                page = 1;
            else
                searchString = currentFilter;

            ViewBag.CurrentSort = sortOrder;
            ViewBag.IdSortParm = String.IsNullOrEmpty(sortOrder) ? "id_asc" : "";
            ViewBag.FirstNameSortParm = sortOrder == "firstname" ? "firstname_desc" : "firstname";
            ViewBag.LastNameSortParm = sortOrder == "lastname" ? "lastname_desc" : "lastname";


            ViewBag.lastnameOrderIcon = "sort";
            ViewBag.firstnameOrderIcon = "sort";

            ViewBag.idOrderIcon = "sort";

            Func<IQueryable<PrepaidCard>, IOrderedQueryable<PrepaidCard>> orderBy = o =>
            {
                switch (sortOrder)
                {
                    case "id_asc":
                        ViewBag.idOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Id);

                    case "firstname":
                        ViewBag.nameOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.First_Name).ThenByDescending(c => c.Id);
                    case "firstname_desc":
                        ViewBag.nameOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.First_Name).ThenByDescending(c => c.Id);

                    case "lastname":
                        ViewBag.nameOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Last_Name).ThenByDescending(c => c.Id);
                    case "lastname_desc":
                        ViewBag.nameOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Last_Name).ThenByDescending(c => c.Id);


                    default:
                        ViewBag.idOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Id);
                }
            };


            ViewBag.CurrentFilter = searchString;

            Expression<Func<PrepaidCard, bool>> filter = f => true;

            if (!String.IsNullOrEmpty(searchString))
            {
                filter = f => f.First_Name.Contains(searchString)
                    //|| f.Name.Contains(searchString)
                    //|| f.Address1.Contains(searchString)
                    //|| f.Address2.Contains(searchString)
                    //|| f.Country.Contains(searchString)
                    //|| f.IPAddress.Contains(searchString)
                    //|| f.Type.Contains(searchString)
                    //|| f.Phone.Contains(searchString)
                    ;
            }


            Expression<Func<PrepaidCard, bool>> filter2 = f => !id.HasValue || id == f.Id;

            var lambda = filter2.AndAlso(filter);



            ViewBag.Display = pageSize;

            int pageNumber = (page ?? 1);

            var model = uow.PrepaidCardRepository
                .GetPage(pageSize, pageNumber,
                //out count,
                filter: lambda,
                orderBy: orderBy
                );

            var pageList = new PagedListMapper<PrepaidCard>(model, model.CloneProps().ConvertUtcTime(TimeZoneUtility.GetTimeZone(uow.TimeZoneRepository.Find(user.TimeZoneId).Name)));
            return View(pageList);

            //return View(uow.PrepaidCardRepository.Get(e => !id.HasValue || e.Id == id));

        }

        public ActionResult GetPaymentTypes(int ProductId)
        {
            return Json(uow.ProductPaymentTypeRepository.Get(a => a.ProductId == ProductId).Select(b => new PaymentTypes() { PaymentTypeId = b.PaymentTypeId, Name = b.PaymentType.Name }).ToList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create()
        {
            var yarr = new List<SelectListItem>();

            PrepaidCard ord = new PrepaidCard();
            //var yearCard = int.Parse(DateTime.Today.Year.ToString().Substring(0, 2) + (ord.CreditCardExpirationYear.ToString().Length > 2 ? ord.CreditCardExpirationYear.ToString().Substring(2).PadLeft(2, '0') : ord.CreditCardExpirationYear.ToString().PadLeft(2, '0')));

            //if (yearCard < DateTime.Today.Year)
            //{
            //    for (int i = yearCard; i < DateTime.Today.Year; i++)
            //    {
            //        var y = new SelectListItem();
            //        y.Text = i.ToString();
            //        y.Value = i.ToString().Substring(2);
            //        yarr.Add(y);
            //    }
            //}
            SelectList ObjList = new SelectList(
        new List<SelectListItem>
        {
        new SelectListItem { Text = "VISA", Value = "VISA"},
        new SelectListItem { Text = "AMEX", Value = "AMEX"},
         new SelectListItem { Text = "DISCOVER", Value = "DISCOVER"},
          new SelectListItem { Text = "MASTERCARD", Value = "MASTERCARD"},
        }, "Value", "Text");

            //Assigning generic list to ViewBag
            ViewBag.PaymentType = ObjList;

            for (int i = 0; i < 20; i++)
            {
                var year = DateTime.Today.Year + i;
                var y = new SelectListItem();
                y.Text = year.ToString();
                y.Value = year.ToString().Substring(2);
                yarr.Add(y);
            }

            ViewBag.Countries = uow.CountryRepository.Get(orderBy: c => c.OrderBy(t => t.Name));
            ViewBag.CreditCardExpirationYears = yarr;
            return View();
        }

        [HttpPost]
        public ActionResult Create(PrepaidCard prepaidCardData)
        {
            prepaidCardData.Date = DateTime.UtcNow;
            prepaidCardData.RemainingAmount = prepaidCardData.Amount;
            //email.UpdatedBy = repo.FindUserProfile(CurrentUserId);

            if (ModelState.IsValid)
            {
                //email.CreatedUserId = wsw.CurrentUserId;

                uow.PrepaidCardRepository.Add(prepaidCardData);
                uow.Save(wsw.CurrentUserName);
                return RedirectToAction("Index");
            }
            return View(prepaidCardData);
        }

        public ActionResult Edit(int id)
        {
            var email = uow.PrepaidCardRepository.Find(id); //repo.FindPrepaidCard(id);
            if (email == null)
            {
                return HttpNotFound();
            }
            return View(email);
        }

        [HttpPost]
        public ActionResult Edit(PrepaidCard prepaidCardData)
        {
            if (ModelState.IsValid)
            {
                prepaidCardData.Date = DateTime.UtcNow;
                //email.UpdatedUserId = wsw.CurrentUserId;

                uow.PrepaidCardRepository.Update(prepaidCardData);
                uow.Save(wsw.CurrentUserName);
                return RedirectToAction("Index");
            }
            return View(prepaidCardData);
        }

        public ActionResult Delete(int id)
        {
            var email = uow.PrepaidCardRepository.Find(id);
            if (email == null)
            {
                return HttpNotFound();
            }

            uow.PrepaidCardRepository.Delete(email);
            uow.Save(wsw.CurrentUserName);
            return RedirectToAction("Index");
        }
    }
}
