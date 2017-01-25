using KontinuityCRM.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KontinuityCRM.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [OutputCache(Location = System.Web.UI.OutputCacheLocation.None, NoStore = true)]
    public class ValidationController : Controller
    {
        protected readonly IUnitOfWork uow;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uow"></param>
        public ValidationController(IUnitOfWork uow)
        {
            this.uow = uow;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="creditcardnumber"></param>
        /// <returns></returns>
        public JsonResult CheckCreditCard(string creditcardnumber)
        {
            if (uow.TestCardNumberRepository.Get(c => c.Number == creditcardnumber).Any())
                return Json(true, JsonRequestBehavior.AllowGet);
            
            if (new CreditCardAttribute().IsValid(creditcardnumber))
                return Json(true, JsonRequestBehavior.AllowGet);

            string errormessage = String.Format(System.Globalization.CultureInfo.InvariantCulture,
                "The Credit Card Number field is not a valid credit card number.");
            return Json(errormessage, JsonRequestBehavior.AllowGet);
        }
    }
}