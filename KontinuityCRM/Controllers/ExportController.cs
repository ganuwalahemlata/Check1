using AutoMapper;
using KontinuityCRM.Helpers;
using KontinuityCRM.Models;
using KontinuityCRM.Models.Enums;
using KontinuityCRM.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace KontinuityCRM.Controllers
{
    [System.ComponentModel.DisplayName("Export Templates")]
    public class ExportController : BaseController
    {
        // GET: Export
        private readonly IMappingEngine mapper;
        public ExportController(IUnitOfWork uow, IWebSecurityWrapper wsw, IMappingEngine mapper)
            : base(uow, wsw)
        {
            this.mapper = mapper;
        }

        public ActionResult Index()
        {
            ViewBag.Heading = "Export Templates";

            ExportTemplateListModel ExportTemplateListModel = new ExportTemplateListModel();
            var ExportTemplate = uow.ExportTemplateRepository.Get().ToList();
            ExportTemplateListModel.ExportTemplates = ExportTemplate.ToList();


            return View(ExportTemplateListModel);
        }
        /// <summary>
        /// Redirects to create View
        /// </summary>
        /// <returns></returns>
        public ActionResult create()
        {
            //create code
            ViewBag.Title = "Create Template";
            ViewBag.Heading = "Create Export Template";
            ViewBag.ShippingOptions = new List<SelectListItem>()
            {
                new SelectListItem { Text = "Shipped", Value = "True" },
                new SelectListItem { Text = "Not shipped", Value = "False" },
            };

            ViewBag.Countries = uow.CountryRepository.Get(orderBy: c => c.OrderBy(t => t.Name));

            var defaultFromDate = new DateTime(DateTime.Today.AddDays(-365).Year, 1, 1);
            var defaultToDate = DateTime.Today.AddDays(1);
            @ViewBag.DefaultFromDate = defaultFromDate;
            @ViewBag.DefaultToDate = defaultToDate;
            var obj = new ExportTemplateModel();
            obj.TemplateFields = new OrderSearch();
            //Add Default Export Fields
            obj.ExportFields = GetExportFields();


            return View("create", obj);

        }
        /// <summary>
        /// Post action to create new ExportTemplate
        /// </summary>
        /// <param name="os"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult create(ExportTemplateModel os)
        {
            try
            {
                ExportTemplate objExportTemplate = new ExportTemplate();
                objExportTemplate.Name = os.TemplateName;
                if (os.Id != null && os.Id > 0)
                {
                    objExportTemplate.Id = os.Id;
                    uow.ExportTemplateRepository.Update(objExportTemplate);



                }
                else
                {
                    uow.ExportTemplateRepository.Add(objExportTemplate);


                }

                uow.Save();
                if (os.Id != null && os.Id > 0)
                {
                    //Update Fields that appear on CSV
                    foreach (var item in os.ExportFields)
                    {
                        item.ExportTemplate = objExportTemplate;
                        uow.ExportFieldsForCSV.Update(item);
                    }
                }
                else
                {
                    //Add Fields that appear on CSV
                    foreach (var item in os.ExportFields)
                    {

                        item.ExportTemplate = objExportTemplate;
                        uow.ExportFieldsForCSV.Add(item);
                    }
                }
                //Use reflection to loop through the items present in the fields and put them in the database...
                uow.Save();
                foreach (var item in os.TemplateFields.GetType().GetProperties())
                {

                    var value = item.GetValue(os.TemplateFields, null);
                    if (value != null)
                    {
                        if (os.Id != null && os.Id > 0)
                        {
                            //update Fields
                            ExportTemplateFields _obj = uow.ExportTemplateFieldsRepository.Get(o => o.FieldName == item.Name
                                                                && o.ExportTemplate.Id == os.Id).FirstOrDefault();
                            if (_obj != null)
                            {
                                _obj.FieldName = item.Name;
                                _obj.FieldValue = value.ToString();
                            }
                            else
                            {
                                _obj = new ExportTemplateFields();
                                _obj.FieldName = item.Name;
                                _obj.FieldValue = value.ToString();
                                _obj.ExportTemplate = objExportTemplate;
                                uow.ExportTemplateFieldsRepository.Add(_obj);
                            }
                        }
                        else
                        {
                            ExportTemplateFields exportTemplateField = new ExportTemplateFields();
                            exportTemplateField.FieldName = item.Name;
                            exportTemplateField.FieldValue = value.ToString();
                            exportTemplateField.ExportTemplate = objExportTemplate;
                            uow.ExportTemplateFieldsRepository.Add(exportTemplateField);

                        }
                    }
                }


                uow.Save();
                return RedirectToAction("Index");
            }

            catch (Exception ex)
            {

            }
            return View("Index");
        }
        /// <summary>
        /// Redirects to edit View
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult edit(int Id)
        {
            //edit code
            //create code
            ViewBag.Title = "Update Template";
            ViewBag.Heading = "Edit Template";
            var obj = new ExportTemplateModel();
            obj.TemplateFields = new OrderSearch();
            ViewBag.ShippingOptions = new List<SelectListItem>()
            {
                new SelectListItem { Text = "Shipped", Value = "True" },
                new SelectListItem { Text = "Not shipped", Value = "False" },
            };

            ViewBag.Countries = uow.CountryRepository.Get(orderBy: c => c.OrderBy(t => t.Name));
            if (Id != 0)
            {
                obj.TemplateName = uow.ExportTemplateRepository.Get(o => o.Id == Id).FirstOrDefault().Name;
                var fields = uow.ExportTemplateFieldsRepository.Get(o => o.ExportTemplate.Id == Id);
                foreach (var item in obj.TemplateFields.GetType().GetProperties())
                {
                    if (fields.Where(o => o.FieldName == item.Name).Any())
                    {
                        var value = fields.Where(o => o.FieldName == item.Name).FirstOrDefault().FieldValue;
                        var convertedValue = ChangeType(value, item.PropertyType);
                        if (convertedValue != "")
                            item.SetValue(obj.TemplateFields, convertedValue);
                        //if (item.Name.Contains("Date"))
                        //{
                        //    item.SetValue(obj.TemplateFields, Convert.ToDateTime(value));

                        //}
                        //else if (item.PropertyType.Name == "Nullable`1")
                        //{
                        //    item.SetValue(obj.TemplateFields, Convert.ToInt32(value));

                        //}
                        //else
                        //    item.SetValue(obj.TemplateFields, value);
                    }
                }
            }
            obj.ExportFields = uow.ExportFieldsForCSV.Get(o => o.ExportTemplate.Id == Id).ToList();

            //Assign Display Name
            var pifields = typeof(KontinuityCRM.Models.APIModels.OrderAPIModel).GetProperties();
            foreach (var item in obj.ExportFields)
            {
                if (pifields.Any(o => o.Name == item.Name))
                {
                    var prop = pifields.Where(o => o.Name == item.Name).FirstOrDefault();
                    var displayAttr = prop.GetCustomAttribute(typeof(DisplayAttribute), true);

                    item.DisplayName = (displayAttr as DisplayAttribute).Name;
                }
                else
                {
                    item.DisplayName = item.Name;
                }
            }
            return View("create", obj);
        }
        /// <summary>
        /// Post action to update ExportTemplateModel
        /// </summary>
        /// <param name="os"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult edit(ExportTemplateModel os)
        {
            try
            {
                ExportTemplate objExportTemplate = new ExportTemplate();
                objExportTemplate.Name = os.TemplateName;
                if (os.Id != null && os.Id > 0)
                {
                    objExportTemplate.Id = os.Id;
                    uow.ExportTemplateRepository.Update(objExportTemplate);



                }
                else
                {
                    uow.ExportTemplateRepository.Add(objExportTemplate);


                }

                uow.Save();
                if (os.Id != null && os.Id > 0)
                {
                    //Update Fields that appear on CSV
                    foreach (var item in os.ExportFields)
                    {
                        item.ExportTemplate = objExportTemplate;
                        uow.ExportFieldsForCSV.Update(item);
                    }
                }
                else
                {
                    //Add Fields that appear on CSV
                    foreach (var item in os.ExportFields)
                    {

                        item.ExportTemplate = objExportTemplate;
                        uow.ExportFieldsForCSV.Add(item);
                    }
                }
                //Use reflection to loop through the items present in the fields and put them in the database...
                uow.Save();
                foreach (var item in os.TemplateFields.GetType().GetProperties())
                {

                    var value = item.GetValue(os.TemplateFields, null);
                    if (value != null)
                    {
                        if (os.Id != null && os.Id > 0)
                        {
                            //update Fields
                            ExportTemplateFields _obj = uow.ExportTemplateFieldsRepository.Get(o => o.FieldName == item.Name
                                                                && o.ExportTemplate.Id == os.Id).FirstOrDefault();
                            if (_obj != null)
                            {
                                _obj.FieldName = item.Name;
                                _obj.FieldValue = value.ToString();
                            }
                            else
                            {
                                _obj = new ExportTemplateFields();
                                _obj.FieldName = item.Name;
                                _obj.FieldValue = value.ToString();
                                _obj.ExportTemplate = objExportTemplate;
                                uow.ExportTemplateFieldsRepository.Add(_obj);
                            }
                        }
                        else
                        {
                            ExportTemplateFields exportTemplateField = new ExportTemplateFields();
                            exportTemplateField.FieldName = item.Name;
                            exportTemplateField.FieldValue = value.ToString();
                            exportTemplateField.ExportTemplate = objExportTemplate;
                            uow.ExportTemplateFieldsRepository.Add(exportTemplateField);

                        }
                    }
                }


                uow.Save();
                return RedirectToAction("Index");
            }

            catch (Exception ex)
            {

            }
            return View("Index");
        }

        public bool delete(int Id)
        {
            //delete code
            Expression<Func<ExportTemplateFields, bool>> filter = o => Id == o.ExportTemplate.Id;

            //Remove all Filters
            var FieldsList = uow.ExportTemplateFieldsRepository.Get(filter);
            foreach (var item in FieldsList)
            {
                uow.ExportTemplateFieldsRepository.Delete(item.Id);
            }
            //Remove all Export Fields
            var ExportFields = uow.ExportFieldsForCSV.Get(o => o.ExportTemplate.Id == Id);
            foreach (var item in ExportFields)
            {
                uow.ExportFieldsForCSV.Delete(item.Id);
            }
            uow.ExportTemplateRepository.Delete(Id);
            uow.Save();
            return true;
        }
        public static object ChangeType(object value, Type conversion)
        {
            var t = conversion;

            if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                {
                    return null;
                }

                t = Nullable.GetUnderlyingType(t);
            }
            if (t.FullName.Contains("KontinuityCRM.Models"))
                return "";
            else
                return Convert.ChangeType(value, t);
        }


        /// <summary>
        /// Export the saved template with exported data selected by user
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="showBillingAddress"></param>
        /// <param name="showShippingAddress"></param>
        /// <returns></returns>
        public async Task<ActionResult> ExportOrders(int Id)
        {
            //var _oid = orderids.First();
            //var order = uow.OrderRepository.Find(_oid);

            var exportTemplates = uow.ExportTemplateFieldsRepository.Get(o => o.ExportTemplate.Id == Id);
            var os = createOrderSearch(exportTemplates.ToList());
            //Get Export Fields for CSV
            var exportFieldsCSV = uow.ExportFieldsForCSV.Get(o => o.ExportTemplate.Id == Id);

            //user timezone to utc
            TimeZoneInfo userTimezoneInfo = TimeZoneUtility.GetTimeZone(uow.TimeZoneRepository.Find(uow.UserProfileRepository.Find(wsw.CurrentUserId).TimeZoneId).Name);
            var ufdate = TimeZoneUtility.ToUtc((DateTime)os.fFromDate, userTimezoneInfo);
            var utdate = TimeZoneUtility.ToUtc((DateTime)os.fToDate, userTimezoneInfo);
            var arrOrderIds = (os.fOrderId != null ? os.fOrderId.Split(',') : new string[] { "" });
            Expression<Func<Order, bool>> filter = o =>

                   (os.fFromDate <= o.Created && o.Created <= os.fToDate)
                  && (os.fOrderId == null  || arrOrderIds.Contains(o.OrderId.ToString()))
                  && (os.fAffiliateId == null || o.AffiliateId == os.fAffiliateId)
                  && (!os.fCustomerId.HasValue || o.CustomerId == os.fCustomerId)
                  && (!os.fProductId.HasValue || o.OrderProducts.Select(p => p.ProductId).Contains(os.fProductId.Value))
                  && (os.fIP == null || o.IPAddress == os.fIP)
                  && (os.fShipped == null || o.Shipped == os.fShipped)
                  && (os.fAddress == null || o.Customer.Address1 == os.fAddress)
                  && (os.fAddress2 == null || o.Customer.Address2 == os.fAddress2)
                  && (os.fFirstname == null || o.Customer.FirstName == os.fFirstname)
                  && (os.fLastname == null || o.Customer.LastName == os.fLastname)
                  && (os.fSubId == null || o.SubId == os.fSubId)
                  && (os.fEmail == null || o.Customer.Email == os.fEmail)
                  && (os.fCity == null || o.Customer.City == os.fCity)
                  && (os.fZIP == null || o.Customer.PostalCode == os.fZIP)
                  && (os.fPhone == null || o.Customer.Phone == os.fPhone)
                  && (os.fState == null || o.Customer.Province == os.fState)
                  && (os.fCountry == null || o.ShippingCountry == os.fCountry);

            // start recurring not declined orders
            var orders = uow.OrderRepository.Get(filter);
            int maxNumberOfProducts = 0;
            if (orders != null && orders.Count() > 0)
                orders.Max(o => o.OrderProducts.Count);
            var sb = new StringBuilder();
            List<PropertyInfo> orderList = new List<PropertyInfo>();
            var pifields = typeof(KontinuityCRM.Models.APIModels.OrderAPIModel).GetProperties();
            var sortedFields = exportFieldsCSV.OrderBy(o => o.Order).ToList();


            foreach (var pi in pifields)
            {
                if (exportFieldsCSV.Any(o => o.Name == pi.Name) && exportFieldsCSV.Where(o => o.Name == pi.Name).FirstOrDefault().Value == false)
                    continue;
                //Shipping Address
                //Skip fields that are not included
                sb.AppendFormat("{0},", "\"" + pi.Name + "\"");

            }
            for (int i = 0; i < maxNumberOfProducts; i++)
            {
                if (exportFieldsCSV.Where(o => o.Name == "Order Products.SKU").FirstOrDefault().Value == true)
                    sb.AppendFormat("{0},", "\"" + "product " + (i + 1).ToString() + " sku" + "\"");
                if (exportFieldsCSV.Where(o => o.Name == "Order Products.qty").FirstOrDefault().Value == true)
                    sb.AppendFormat("{0},", "\"" + "product " + (i + 1).ToString() + "quantity" + "\"");
                if (exportFieldsCSV.Where(o => o.Name == "Order Products.price").FirstOrDefault().Value == true)
                    sb.AppendFormat("{0},", "\"" + "product " + (i + 1).ToString() + "price" + "\"");
            }

            sb.Length--;
            sb.AppendLine();

            foreach (var order in orders)
            {
                var ordermap = mapper.Map<KontinuityCRM.Models.Order, KontinuityCRM.Models.APIModels.OrderAPIModel>(order);

                foreach (var pi in pifields)
                {
                    try
                    {
                        var objvalue = ordermap.GetType().GetProperty(pi.Name).GetValue(ordermap);
                        //If Create Date
                        if (pi.Name.Equals("Created"))
                        {
                            objvalue = TimeZoneUtility.FromUtc(Convert.ToDateTime(objvalue), TimeZoneUtility.GetTimeZone(uow.TimeZoneRepository.Find(uow.UserProfileRepository.Find(wsw.CurrentUserId).TimeZoneId).Name));
                        }

                        if (exportFieldsCSV.Any(o => o.Name == pi.Name) && exportFieldsCSV.Where(o => o.Name == pi.Name).FirstOrDefault().Value == false)
                            continue;
                        if (objvalue == null)
                        {
                            sb.Append(",");
                        }
                        else
                        {
                            if (pi.Name == "OrderProducts")
                            {
                                var varsb = new StringBuilder();
                                foreach (var op in order.OrderProducts)
                                {
                                    varsb.AppendFormat("{0} ({1}),", op.Product.Name, op.ProductId);
                                }
                                if (varsb.Length > 0)
                                    varsb.Length--;
                                sb.AppendFormat("{0},", varsb.ToString());
                            }
                            else
                            {
                                sb.AppendFormat("{0},", "\"" + objvalue.ToString() + "\"");
                            }
                        }



                    }
                    catch (Exception ex)
                    {
                        var s = pi.Name;
                        throw ex;
                    }

                }

                foreach (var ops in order.OrderProducts)
                {
                    if (exportFieldsCSV.Where(o => o.Name == "Order Products.SKU").FirstOrDefault().Value == true)
                        if (ops.SKU != null)
                            sb.AppendFormat("{0},", ops.SKU.ToString());
                        else
                            sb.AppendFormat("{0},", "");

                    if (exportFieldsCSV.Where(o => o.Name == "Order Products.qty").FirstOrDefault().Value == true)
                        sb.AppendFormat("{0},", ops.Quantity.ToString());

                    if (exportFieldsCSV.Where(o => o.Name == "Order Products.price").FirstOrDefault().Value == true)
                        if (ops.Price != null)
                            sb.AppendFormat("{0},", ops.Price.ToString());
                        else
                            sb.AppendFormat("{0},", "");
                }

                sb.Length--;
                sb.AppendLine();
            }

            return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", string.Format("export_CSV_at_{0}.csv", DateTime.Now.ToString("dd_MMM_yy_HH_mm_ss")));
        }


        /// <summary>
        /// Create Order Search Object by Using Fields that has been saved in a template
        /// </summary>
        /// <param name="selectedFields"></param>
        /// <returns></returns>
        public OrderSearch createOrderSearch(List<ExportTemplateFields> selectedFields)
        {
            OrderSearch objOrderSearch = new OrderSearch();

            foreach (var item in selectedFields)
            {
                switch (item.FieldName)
                {
                    case "fOrderId":
                        objOrderSearch.fOrderId = item.FieldValue;
                        break;
                    case "fAddress":
                        objOrderSearch.fAddress = item.FieldValue;
                        break;
                    case "fAddress2":
                        objOrderSearch.fAddress2 = item.FieldValue;
                        break;
                    case "fAffiliateId":
                        objOrderSearch.fAffiliateId = item.FieldValue;
                        break;
                    case "fCity":
                        objOrderSearch.fCity = item.FieldValue;
                        break;
                    case "fCountry":
                        objOrderSearch.fCountry = item.FieldValue;
                        break;
                    case "fCustomerId":
                        objOrderSearch.fCustomerId = Convert.ToInt32(item.FieldValue);
                        break;
                    case "fEmail":
                        objOrderSearch.fEmail = item.FieldValue;
                        break;
                    case "fFirstname":
                        objOrderSearch.fFirstname = item.FieldValue;
                        break;
                    case "fFromDate":
                        objOrderSearch.fFromDate = Convert.ToDateTime(item.FieldValue);
                        break;
                    case "fIP":
                        objOrderSearch.fIP = item.FieldValue;
                        break;
                    case "fLastname":
                        objOrderSearch.fLastname = item.FieldValue;
                        break;
                    case "fPhone":
                        objOrderSearch.fPhone = item.FieldValue;
                        break;
                    case "fProductId":
                        objOrderSearch.fProductId = Convert.ToInt32(item.FieldValue);
                        break;
                    case "fRMA":
                        objOrderSearch.fRMA = item.FieldValue;
                        break;
                    case "fShipped":
                        objOrderSearch.fShipped = Convert.ToBoolean(item.FieldValue);
                        break;
                    case "fState":
                        objOrderSearch.fState = item.FieldValue;
                        break;
                    case "fStatus":
                        objOrderSearch.fStatus = (OrderStatus)Enum.Parse(typeof(OrderStatus), item.FieldValue);
                        break;
                    case "fSubId":
                        objOrderSearch.fSubId = item.FieldValue;
                        break;
                    case "fToDate":
                        objOrderSearch.fToDate = Convert.ToDateTime(item.FieldValue);
                        break;
                    case "fTransactionId":
                        objOrderSearch.fTransactionId = item.FieldValue;
                        break;
                    case "fZIP":
                        objOrderSearch.fZIP = item.FieldValue;
                        break;
                    default:
                        break;

                }
            }
            return objOrderSearch;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<ExportFields> GetExportFields()
        {
            List<ExportFields> lstExportFields = new List<ExportFields>();
            var pifields = typeof(KontinuityCRM.Models.APIModels.OrderAPIModel).GetProperties();
            for (int i = 0; i < pifields.Length; i++)
            {
                ExportFields obj = new ExportFields();
                obj.Name = pifields[i].Name;
                var prop = pifields[i].GetCustomAttribute(typeof(DisplayAttribute), true);
                obj.DisplayName = (prop as DisplayAttribute).Name;
                obj.Value = true;

                obj.Order = i + 1;
                lstExportFields.Add(obj);
            }

            ExportFields obj2 = new ExportFields();
            obj2.Name = "Order Products.SKU";
            obj2.DisplayName = "Order Products.SKU";
            obj2.Value = true;
            obj2.Order = lstExportFields.Count;
            lstExportFields.Add(obj2);



            ExportFields obj3 = new ExportFields();
            obj3.Name = "Order Products.qty";
            obj3.DisplayName = "Order Products.qty";
            obj3.Value = true;
            obj3.Order = lstExportFields.Count;
            lstExportFields.Add(obj3);

            ExportFields obj4 = new ExportFields();
            obj4.Name = "Order Products.price";
            obj4.DisplayName = "Order Products.price";
            obj4.Value = true;
            obj4.Order = lstExportFields.Count;
            lstExportFields.Add(obj4);



            ExportFields obj6 = new ExportFields();
            obj6.Name = "RM Number";
            obj6.DisplayName = "RM Number";
            obj6.Value = true;
            obj6.Order = lstExportFields.Count;
            lstExportFields.Add(obj6);

            return lstExportFields;

        }
    }
}