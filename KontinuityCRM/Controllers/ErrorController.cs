using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KontinuityCRM.Controllers
{
    [Authorize]
    [System.ComponentModel.DisplayName("Error")]
    public class ErrorController : Controller
    {
        //
        // GET: /Error/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult NotFound()
        {
            return View();
        }

        public ActionResult InternalServerError()
        {
            return View();
        }

        public ActionResult Logs()
        {
            var db = new KontinuityCRM.Models.KontinuityDB();

            return View(db.KLogs.OrderByDescending(l => l.Timestamp).Take(50));
        }

    }
}
