using KontinuityCRM.Helpers;
using KontinuityCRM.Helpers.ScheduledTasks;
using KontinuityCRM.Models;
using KontinuityCRM.Models.ViewModels;
using Quartz;
using Quartz.Impl.Matchers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Quartz.Listener;

namespace KontinuityCRM.Controllers
{
    public class TestController : Controller
    {
        private readonly IUnitOfWork uow;
        private readonly IScheduler scheduler;

        public TestController(IUnitOfWork uow, IScheduler scheduler)
        {
            this.uow = uow;
            this.scheduler = scheduler;
        }

        // GET: Test
        /// <summary>
        /// Redirects to Essence of Argan Product Page
        /// </summary>
        /// <returns></returns>
        public ActionResult OfficialArgan()
        {
            ViewBag.ProductId = uow.ProductRepository.GetSet().First().ProductId;
            return View();
        }
        /// <summary>
        /// Post action for providing credentials to order Essence of Argan (Product)
        /// </summary>
        /// <param name="partial"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> OfficialArgan(KontinuityCRM.Models.APIModels.PartialCreateModel partial)
        {
            if (ModelState.IsValid)
            {
                using (var client = System.Net.Http.HttpClientFactory.Create(new APIKeyCredentialsHandler()))
                //using (var client = new HttpClient())
                {
                    //client.BaseAddress = new Uri(Request.Url.Authority);

                    //var url = Url.RouteUrl("DefaultApiPost", new { httproute = "", controller = "partial" });
                    var url = Url.RouteUrl("DefaultApiPost", new { httproute = "", controller = "partial" }, this.Request.Url.Scheme);

                    var baseurl = Request.Url.GetLeftPart(UriPartial.Authority);
                    client.BaseAddress = new Uri(baseurl);


                    //partial.IPAddress = Request.UserHostAddress;

                    // TODO - Send HTTP requests
                    var response = await client.PostAsJsonAsync("api/partials", partial);
                    //var response = await client.PostAsJsonAsync(url, partial);
                    if (response.IsSuccessStatusCode)
                    {
                        // Get the URI of the created resource.
                        Uri gizmoUrl = response.Headers.Location;

                        var createdPartial = await response.Content.ReadAsAsync<Partial>();

                        return RedirectToAction("OfficialArganOrder", new { id = createdPartial.PartialId });
                    }
                }
            }
            ViewBag.ProductId = uow.ProductRepository.GetSet().First().ProductId;
            return View(partial);
        }
        /// <summary>
        /// Redirects to Official Argan (Product)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult OfficialArganOrder(int id)
        {
            ViewBag.ShippingMethod = uow.ShippingMethodRepository.GetSet().First();
            ViewBag.Product = uow.ProductRepository.GetSet().First();

            return View();
        }
        /// <summary>
        /// Post Action to Order Product
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> OfficialArganOrder(int id, KontinuityCRM.Models.APIModels.PartialToOrderModel model)
        {
            if (ModelState.IsValid)
            {
                //using (var client = System.Net.Http.HttpClientFactory.Create(new CredentialsHandler()))
                using (var client = System.Net.Http.HttpClientFactory.Create(new APIKeyCredentialsHandler()))
                //using (var client = new HttpClient())
                {
                    //client.BaseAddress = new Uri(Request.Url.Authority);

                    //var url = Url.RouteUrl("DefaultApiPost", new { httproute = "", controller = "partial" });
                    //var url = Url.RouteUrl("DefaultApiPost", new { httproute = "", controller = "partial", action = "toorder" }, this.Request.Url.Scheme);

                    var baseurl = Request.Url.GetLeftPart(UriPartial.Authority);
                    client.BaseAddress = new Uri(baseurl);

                    // TODO - Send HTTP requests
                    model.IPAddress = Request.UserHostAddress;
                    var response = await client.PostAsJsonAsync("api/partials/order/" + id, model);
                    //var response = await client.PostAsJsonAsync(url, partial);
                    if (response.IsSuccessStatusCode)
                    {
                        // Get the URI of the created resource.
                        //Uri gizmoUrl = response.Headers.Location;

                        //var createdObj = await response.Content.ReadAsAsync<Order>();

                        //return RedirectToAction("OfficialArganOrder", new { id = createdPartial.PartialId });
                        //var orderid = int.Parse(response.Headers.Location.Segments.Last());

                        //                       var encrytedorderid = await response.Content.ReadAsStringAsync();

                        //                        dynamic data = System.Web.Helpers.Json.Decode(encrytedorderid);
                        ////                       encrytedorderid = encrytedorderid.Trim('"'); // tmp patch
                        //                       encrytedorderid = data.Cypher; // tmp patch

                        var responseData = await response.Content.ReadAsAsync<KontinuityCRM.Models.APIModels.OrderCreationResponse>();
                        //var encrytedorderid = responseData.Cypher;


                        //return RedirectToAction("upsell", new { id = orderid });
                        //return RedirectToAction("upsell", new { orderid = encrytedorderid });
                        return RedirectToAction("upsell", new { id = responseData.OrderId });
                    }
                }
            }
            ViewBag.ShippingMethod = uow.ShippingMethodRepository.GetSet().First();
            ViewBag.Product = uow.ProductRepository.GetSet().First();
            return View(model);
        }
        /// <summary>
        /// Redirects to Upsell View for an order having orderId as id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Upsell(int id)
        {
            ViewBag.OrderId = id;
            ViewBag.ProductId = uow.ProductRepository.GetSet().Select(p => p.ProductId).FirstOrDefault();
            return View();
        }
        /// <summary>
        /// Post Action for upsell products
        /// </summary>
        /// <param name="upsellproducts"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Upsell(
            ICollection<KontinuityCRM.Models.APIModels.OrderProduct> upsellproducts,
            int id)
        {
            if (upsellproducts == null || upsellproducts.Count == 0)
            {
                ModelState.AddModelError(string.Empty, "invalid products");
            }

            if (ModelState.IsValid)
            {
                using (var client = System.Net.Http.HttpClientFactory.Create(new APIKeyCredentialsHandler()))
                //using (var client = System.Net.Http.HttpClientFactory.Create(new CredentialsHandler()))
                //using (var client = new HttpClient())
                {
                    //client.BaseAddress = new Uri(Request.Url.Authority);

                    //var url = Url.RouteUrl("DefaultApiPost", new { httproute = "", controller = "partial" });
                    //var url = Url.RouteUrl("Upsell", new { httproute = "", controller = "orders", action = "upsell", id = orderid }, this.Request.Url.Scheme);

                    var baseurl = Request.Url.GetLeftPart(UriPartial.Authority);
                    client.BaseAddress = new Uri(baseurl);

                    //var uri = new Uri(url);

                    //var encodedordeerid = HttpUtility.UrlPathEncode(orderid);

                    //var upsellmodel = new KontinuityCRM.Models.APIModels.UpsellAPIModel { OrderProducts = upsellproducts};


                    // TODO - Send HTTP requests
                    //model.IPAddress = Request.UserHostAddress;
                    //var response = await client.PostAsJsonAsync("api/orders/upsell/" + Url.Encode(orderid), upsellproducts);
                    //var response = await client.PostAsJsonAsync(HttpUtility.UrlPathEncode("api/orders/upsell/?id=" + 43), upsellproducts);
                    var response = await client.PostAsJsonAsync("api/orders/upsell/" + id, upsellproducts);
                    //var response = await client.PostAsJsonAsync(uri, upsellproducts);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("index", "order");
                    }
                }
            }
            ViewBag.OrderId = id;
            ViewBag.ProductId = uow.ProductRepository.GetSet().Select(p => p.ProductId).FirstOrDefault();
            return View(upsellproducts);
        }
        /// <summary>
        /// Redirects to avialable jobs View
        /// </summary>
        /// <returns></returns>
        public ActionResult Jobs()
        {
            var qjobs = scheduler
                .GetJobKeys(GroupMatcher<JobKey>.AnyGroup())
                .Select(k =>
                {
                    var trigger = scheduler.GetTriggersOfJob(k).First();
                    var jobdetail = scheduler.GetJobDetail(k);

                    var previous = jobdetail.JobDataMap.ContainsKey("PreviousFireTime") ?
                        ((DateTimeOffset)jobdetail.JobDataMap["PreviousFireTime"]).ToLocalTime().ToString() : string.Empty;

                    return new QuartzJob
                    {
                        JobName = k.Name,
                        JobGroup = k.Group,
                        NextFireTime = trigger.GetNextFireTimeUtc(),
                        PreviousFireTime = trigger.GetPreviousFireTimeUtc(),
                        //PreviousFireTime = jobdetail.JobDataMap["PreviousFireTime"]
                        //PreviousFireTimePersist = trigger.JobDataMap["PreviousFireTime"].ToString(),
                        //PreviousFireTimePersist = jobdetail.JobDataMap["PreviousFireTime"].ToString(),
                        PreviousFireTimePersist = previous,
                    };

                });


            return View(qjobs);
        }
        /// <summary>
        /// Execute Job with JobName
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult FireJob(string id) // jobname
        {
            var jobKey = scheduler
               .GetJobKeys(GroupMatcher<JobKey>.AnyGroup())
               .FirstOrDefault(k => k.Name == id);

            if (jobKey != null)
            {
                // Create listener ??

                var jobdetail = scheduler.GetJobDetail(jobKey);
                jobdetail.JobDataMap["PreviousFireTime"] = DateTimeOffset.UtcNow; // still doesn't work it need we need to refresh the page to see the changes

                scheduler.TriggerJob(jobKey);
            }

            return RedirectToAction("jobs");
        }

        public ActionResult throwEx()
        {
            throw new Exception("Test Exception Email Capture");
        }

        public ActionResult tmp()
        {
            return View();
        }

        //[HttpPost]
        //public ActionResult CaptureTest()
        //{
        //    var job = new CaptureJob();
        //    JobKey jobKey = new JobKey("ds", "ds");

        //    //job.Execute();

        //    return View();

        //}
    }
}