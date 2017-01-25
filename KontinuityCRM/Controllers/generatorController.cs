using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using KontinuityCRM.Helpers;
using KontinuityCRM.Models;
using KontinuityCRM.Models.ViewModels;

namespace KontinuityCRM.Controllers
{

    [AuthorizationRole(SecurityRoles = new[] { SecurityRole.FormGenerator })]
    [System.ComponentModel.DisplayName("Form Generator")]
    public class generatorController : BaseController
    {
        public generatorController(IUnitOfWork uow, IWebSecurityWrapper wsw)
            : base(uow, wsw)
        {

        }
        /// <summary>
        /// Redirects to FormGenerator View
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var model = new FormGenerationModel
            {
                Products = uow.ProductRepository.Get().ToList(),
                ShippingMethods = uow.ShippingMethodRepository.Get().ToList(),
                FormNames = new[] {
                    new formname { Name="Form1Name"},
                    new formname { Name="Form2Name"}
                },
                FormFields = new[]
                {
                    new FormFieldInfo {Name = "FirstName", Label = "First Name", Required = true, PageIndex = 1},
                    new FormFieldInfo {Name = "LastName", Label = "Last Name", Required = true, PageIndex = 1},
                    new FormFieldInfo {Name = "Address1", Label = "Address 1", Required = true, PageIndex = 1},
                    new FormFieldInfo {Name = "Address2", Label = "Address 2", PageIndex = 1},
                    new FormFieldInfo {Name = "City", Required = true, PageIndex = 1},
                    new FormFieldInfo {Name = "State", Required = true, PageIndex = 1},
                    new FormFieldInfo {Name = "Country", Required = true, PageIndex = 1},
                    new FormFieldInfo {Name = "PostalCode", Label = "Postal Code", Required = true, PageIndex = 1},
                    new FormFieldInfo {Name = "Email", InputType = HtmlInputType.email, Required = true, PageIndex = 1},
                    new FormFieldInfo {Name = "Phone", Required = true, PageIndex = 1},
                    new FormFieldInfo {Name = "CreditCardNumber", Label = "Credit Card", Required = true, PageIndex = 2},
                    new FormFieldInfo
                    {
                        Name = "CCExpiryDate",
                        Label = "CC Expiry Date",
                        InputType = HtmlInputType.date,
                        Required = true,
                        PageIndex = 2
                    },
                    new FormFieldInfo
                    {
                        Name = "CreditCardCVV",
                        Label = "CC CVV",
                        ValidationExpression = @"^\d{3,4}$",
                        Required = true,
                        PageIndex = 2
                    },
                    new FormFieldInfo {Name = "BillingFirstName", Label = "Billing First Name", PageIndex = 2},
                    new FormFieldInfo {Name = "BillingLastName", Label = "Billing Last Name", PageIndex = 2},
                    new FormFieldInfo {Name = "BillingAddress1", Label = "Billing Address", PageIndex = 2},
                    new FormFieldInfo {Name = "BillingCity", Label = "Billing City", PageIndex = 2},
                    new FormFieldInfo {Name = "BillingProvince", Label = "Billing State", PageIndex = 2},
                    new FormFieldInfo {Name = "BillingPostalCode", Label = "Billing Postal Code", PageIndex = 2},
                    new FormFieldInfo {Name = "BillingCountry", Label = "Billing Country", PageIndex = 2}
                }
            };
            return View(model);
        }
        /// <summary>
        /// Redirects to FormGenerator View
        /// </summary>
        /// <returns></returns>
        public ActionResult FormGenerator()
        {
            var model = new FormGenerationModel
            {
                Products = uow.ProductRepository.Get(c => true).ToList(),
                FormFields = new[]
                {
                    new FormFieldInfo {Name = "FirstName", Label = "First Name", Required = true, PageIndex = 1},
                    new FormFieldInfo {Name = "LastName", Label = "Last Name", Required = true, PageIndex = 1},
                    new FormFieldInfo {Name = "Address1", Label = "Address 1", Required = true, PageIndex = 1},
                    new FormFieldInfo {Name = "Address2", Label = "Address 2", PageIndex = 1},
                    new FormFieldInfo {Name = "City", Required = true, PageIndex = 1},
                    new FormFieldInfo {Name = "State", Required = true, PageIndex = 1},
                    new FormFieldInfo {Name = "Country", Required = true, PageIndex = 1},
                    new FormFieldInfo {Name = "PostalCode", Label = "Postal Code", Required = true, PageIndex = 1},
                    new FormFieldInfo {Name = "Email", InputType = HtmlInputType.email, Required = true, PageIndex = 1},
                    new FormFieldInfo {Name = "Phone", Required = true, PageIndex = 1},
                    new FormFieldInfo {Name = "CreditCardNumber", Label = "Credit Card", Required = true, PageIndex = 2},
                    new FormFieldInfo
                    {
                        Name = "CCExpiryDate",
                        Label = "CC Expiry Date",
                        Required = true,
                        InputType = HtmlInputType.date,
                        PageIndex = 2
                    },
                    new FormFieldInfo
                    {
                        Name = "CreditCardCVV",
                        Label = "CC CVV",
                        ValidationExpression = @"^\d{3,4}$",
                        Required = true,
                        PageIndex = 2
                    },
                    new FormFieldInfo {Name = "BillingFirstName", Label = "Billing First Name", PageIndex = 2},
                    new FormFieldInfo {Name = "BillingLastName", Label = "Billing Last Name", PageIndex = 2},
                    new FormFieldInfo {Name = "BillingAddress1", Label = "Billing Address", PageIndex = 2},
                    new FormFieldInfo {Name = "BillingCity", Label = "Billing City", PageIndex = 2},
                    new FormFieldInfo {Name = "BillingProvince", Label = "Billing State", PageIndex = 2},
                    new FormFieldInfo {Name = "BillingPostalCode", Label = "Billing Postal Code", PageIndex = 2},
                    new FormFieldInfo {Name = "BillingCountry", Label = "Billing Country", PageIndex = 2}
                }
            };
            ViewBag.Model = model;
            return View(model);
        }

        public ActionResult FormGenerator1()
        {
            var model = new FormGenerationModel
            {
                Products = uow.ProductRepository.Get(c => true).ToList(),

                FormFields = new[]
             {
                    new FormFieldInfo {Name = "FirstName", Label = "First Name", Required = true, PageIndex = 1},
                    new FormFieldInfo {Name = "LastName", Label = "Last Name", Required = true, PageIndex = 1},
                    new FormFieldInfo {Name = "Address1", Label = "Address 1", Required = true, PageIndex = 1},
                    new FormFieldInfo {Name = "Address2", Label = "Address 2", PageIndex = 1},
                    new FormFieldInfo {Name = "City", Required = true, PageIndex = 1},
                    new FormFieldInfo {Name = "State", Required = true, PageIndex = 1},
                    new FormFieldInfo {Name = "Country", Required = true, PageIndex = 1},
                    new FormFieldInfo {Name = "PostalCode", Label = "Postal Code", Required = true, PageIndex = 1},
                    new FormFieldInfo {Name = "Email", InputType = HtmlInputType.email, Required = true, PageIndex = 1},
                    new FormFieldInfo {Name = "Phone", Required = true, PageIndex = 1},
                    new FormFieldInfo {Name = "CreditCardNumber", Label = "Credit Card", Required = true, PageIndex = 2},
                    new FormFieldInfo
                    {
                        Name = "CCExpiryDate",
                        Label = "CC Expiry Date",
                        Required = true,
                        InputType = HtmlInputType.date,
                        PageIndex = 2
                    },
                    new FormFieldInfo
                    {
                        Name = "CreditCardCVV",
                        Label = "CC CVV",
                        ValidationExpression = @"^\d{3,4}$",
                        Required = true,
                        PageIndex = 2
                    },
                    new FormFieldInfo {Name = "BillingFirstName", Label = "Billing First Name", PageIndex = 2},
                    new FormFieldInfo {Name = "BillingLastName", Label = "Billing Last Name", PageIndex = 2},
                    new FormFieldInfo {Name = "BillingAddress1", Label = "Billing Address", PageIndex = 2},
                    new FormFieldInfo {Name = "BillingCity", Label = "Billing City", PageIndex = 2},
                    new FormFieldInfo {Name = "BillingProvince", Label = "Billing State", PageIndex = 2},
                    new FormFieldInfo {Name = "BillingPostalCode", Label = "Billing Postal Code", PageIndex = 2},
                    new FormFieldInfo {Name = "BillingCountry", Label = "Billing Country", PageIndex = 2}
                }
            };
            ViewBag.Model = model;
            return View(model);
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult FormGenerator1(FormGenerationModel model, string Form1Name, string Form2Name)
        {
            ProcessModel(model);
            model.IsPreview = true;
            string resultPreview = RenderPartialToString("~/Views/generator/FormGeneratorResultNew.cshtml", model, ControllerContext);
            model.IsPreview = false;
            string result1 = RenderPartialToString("~/Views/generator/FormGeneratorResultNew.cshtml", model, ControllerContext);
            string result2 = RenderPartialToString("~/Views/generator/FormGeneratorResultNew2.cshtml", model, ControllerContext);
            result1 = result1.Replace("nextpagename", model.FormNames[1].Value);
          

            FormGeneration form = new FormGeneration();
            form.Content = result1;
            form.Name = model.FormNames[0].Value;
            form.Create(uow, true);
            string prettify1 = result1.Replace("<", "&lt;").Replace(">", "&gt;");

            FormGeneration form2 = new FormGeneration();
            form2.Content = result2;
            form2.Name = model.FormNames[1].Value;
            form2.Create(uow, true);
            string prettify2 = result2.Replace("<", "&lt;").Replace(">", "&gt;");
            return new JsonResult
            {
                Data = new { Widget = resultPreview, Html = prettify1, Html2 = prettify2, FormCode = form.Id, FormCode2 = form2.Id }
            };
        }

        /// <summary>
        /// Form Generator against FormGenerationModel
        /// </summary>
        /// <param name="model">FormGenerationModel</param>
        /// <returns></returns>
        [System.Web.Mvc.HttpPost]
        public ActionResult FormGenerator(FormGenerationModel model)
        {
            ProcessModel(model);
            model.IsPreview = true;
            string resultPreview = RenderPartialToString("~/Views/generator/FormGeneratorResult.cshtml", model, ControllerContext);
            model.IsPreview = false;
            string result = RenderPartialToString("~/Views/generator/FormGeneratorResult.cshtml", model, ControllerContext);

            FormGeneration form = new FormGeneration();
            form.Content = result;
            form.Create(uow, true);
            string prettify = result.Replace("<", "&lt;").Replace(">", "&gt;");
            return new JsonResult
            {
                Data = new { Widget = resultPreview, Html = prettify, FormCode = form.Id }
            };
        }

        //public ActionResult IndexTemplate()
        //{
        //   // FormGenerationModel1 _objmodel = new FormGenerationModel1();
        //   // string resultPreview = RenderPartialToString("~/Views/generator/FormGeneratorResultTemplate2.cshtml", _objmodel, ControllerContext);

        //   // FormGeneration form = new FormGeneration();
        //   // form.Content = resultPreview;
        //   // form.Create(uow, true);
        //   //return Download(form.Id);

        //    FileResult f1 = IndexTemplate1();
        //    FileResult f2= IndexTemplate1();

        //    return RedirectToAction("Index");
        //    //   string prettify = resultPreview.Replace("<", "&lt;").Replace(">", "&gt;");

        //}

        //public ActionResult IndexTemplate1()
        //{
        //    FormGenerationModel1 _objmodel = new FormGenerationModel1();
        //    string resultPreview = RenderPartialToString("~/Views/generator/FormGeneratorResultTemplate3.cshtml", _objmodel, ControllerContext);
        //    FormGeneration form = new FormGeneration();
        //    form.Content = resultPreview;
        //    form.Create(uow, true);


        //    FileResult f2 = Download(form2.Id);

        //    return RedirectToAction("Index");
        //    //   string prettify = resultPreview.Replace("<", "&lt;").Replace(">", "&gt;");

        //}

        // public FileResult IndexTemplate1()
        //{
        //     FormGenerationModel1 _objmodel = new FormGenerationModel1();
        //     string resultPreview2 = RenderPartialToString("~/Views/generator/FormGeneratorResultTemplate2.cshtml", _objmodel, ControllerContext);
        //     FormGeneration form = new FormGeneration();
        //     form.Content = resultPreview2;
        //     form.Create(uow, true);

        //     return Download(form.Id);

        // }

        // public FileResult IndexTemplate2()
        // {
        //     FormGenerationModel1 _objmodel = new FormGenerationModel1();
        //     string resultPreview2 = RenderPartialToString("~/Views/generator/FormGeneratorResultTemplate3.cshtml", _objmodel, ControllerContext);
        //     FormGeneration form = new FormGeneration();
        //     form.Content = resultPreview2;
        //     form.Create(uow, true);

        //    return Download(form.Id);

        // }

        /// <summary>
        /// Process FormGenerationModel
        /// </summary>
        /// <param name="model">FormGenerationModel</param>
        private void ProcessModel(FormGenerationModel model)
        {
            model.Products = uow.ProductRepository.Get(c => model.ProductId.Contains(c.ProductId)).ToList();
            model.ShippingMethods = uow.ShippingMethodRepository.Get(c => model.ShippingMethodId.Contains(c.Id)).ToList();
            model.FormFields.Add(new FormFieldInfo()
            {
                InputType = HtmlInputType.product,
                Order = model.ProductOrder,
                PageIndex = model.ProductPage,
                Visible = true,
                Name = "Product"
            });
            model.FormFields.Add(new FormFieldInfo()
            {
                InputType = HtmlInputType.shippingMethod,
                Order = model.ShippingMethodOrder,
                PageIndex = model.ShippingMethodPage,
                Visible = true,
                Name = "Shipping Method"
            });
        }

        /// <summary>
        /// Renders a partial view to a string
        /// </summary>
        /// <param name="viewName">Name of the view to be rendered</param>
        /// <param name="model">Model to pass to the view</param>
        /// <param name="ControllerContext">Context of the controller holding the view</param>
        /// <returns>Returns the string conversion of the view</returns>
        public static string RenderPartialToString(string viewName, object model, ControllerContext ControllerContext)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");
            var ViewData = new ViewDataDictionary();
            var TempData = new TempDataDictionary();
            ViewData.Model = model;

            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }

        }

        /// <summary>
        /// Downloads the generated form as a file
        /// </summary>
        /// <param name="id">Database id for the generated form</param>
        /// <returns>The HTML file of the generated form</returns>
        [System.Web.Mvc.HttpGet]
        public FileResult Download(int id, string Form1Name)
        {
            var form = uow.FormGenerationRepository.Find(id);
            if (form != null)
            {
                string fileName = form.Name+".html";
                return File(System.Text.Encoding.UTF8.GetBytes(form.Content), System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent(string.Format("No generated form with ID = {0}", id)),
                ReasonPhrase = "Generated form ID Not Found"
            };
            throw new HttpResponseException(resp);
        }

        public FileResult Download2(int id, string Form2Name)
        {
            var form = uow.FormGenerationRepository.Find(id);
            if (form != null)
            {
                string fileName = form.Name + ".html";
                return File(System.Text.Encoding.UTF8.GetBytes(form.Content), System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent(string.Format("No generated form with ID = {0}", id)),
                ReasonPhrase = "Generated form ID Not Found"
            };
            throw new HttpResponseException(resp);
        }
    }
}