using KontinuityCRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KontinuityCRM.Helpers;
using System.Data.Entity;
using System.Transactions;
using System.Text;
using System.Linq.Expressions;
using KontinuityCRM.Filters;
using KontinuityCRM.Models.Enums;

namespace KontinuityCRM.Controllers
{
    [CustomAuthorizeAttribute]
    [System.ComponentModel.DisplayName("Products")] // for the nav & breadcrumbs
    public class productController : BaseController
    {
        protected IKontinuityCRMRepo repo = new EFKontinuityCRMRepo();

        public productController(IUnitOfWork uow, IWebSecurityWrapper wsw)
            : base(uow, wsw)
        {

        }
        /// <summary>
        /// Redirect to products listing view
        /// </summary>
        /// <param name="sortOrder"></param>
        /// <param name="currentFilter"></param>
        /// <param name="searchString"></param>
        /// <param name="page"></param>
        /// <param name="display"></param>
        /// <param name="CategoryId"></param>
        /// <returns></returns>
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? display, int? CategoryId)
        {
            //throw new Exception("error here!");

            var user = uow.UserProfileRepository.Find(wsw.CurrentUserId);
            int pageSize = user.ProductDisplay;
            //var redirect = false;
            if (display.HasValue && display != pageSize)
            {
                page = 1;
                pageSize = display.Value;

                user.ProductDisplay = pageSize;
                uow.UserProfileRepository.Update(user);
                uow.Save(wsw.CurrentUserName);
                //redirect = true;

            }
            if (searchString != null)
            {
                page = 1;
                //redirect = true;
            }
            else searchString = currentFilter;
            //if (redirect)
            //    return RedirectToAction("index", new { sortOrder = sortOrder, currentFilter = searchString });


            ViewBag.CurrentFilter = searchString;


            ViewBag.CurrentSort = sortOrder;

            ViewBag.IdSortParm = String.IsNullOrEmpty(sortOrder) ? "id_asc" : "";
            ViewBag.NameSortParm = sortOrder == "name" ? "name_desc" : "name";
            ViewBag.PriceSortParm = sortOrder == "price" ? "price_desc" : "price";
            ViewBag.TaxableSortParm = sortOrder == "taxable" ? "taxable_desc" : "taxable";
            ViewBag.SKUSortParm = sortOrder == "sku" ? "sku_desc" : "sku";
            ViewBag.CategorySortParm = sortOrder == "category" ? "category_desc" : "category";

            ViewBag.idOrderIcon = "sort";
            ViewBag.nameOrderIcon = "sort";
            ViewBag.priceOrderIcon = "sort";
            ViewBag.taxableOrderIcon = "sort";
            ViewBag.skuOrderIcon = "sort";
            ViewBag.categoryOrderIcon = "sort";

            Func<IQueryable<Product>, IOrderedQueryable<Product>> orderBy = o =>
            {
                switch (sortOrder)
                {
                    case "id_asc":
                        ViewBag.idOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.ProductId);

                    case "name":
                        ViewBag.nameOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Name).ThenByDescending(c => c.ProductId);
                    case "name_desc":
                        ViewBag.nameOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Name).ThenByDescending(c => c.ProductId);

                    case "price":
                        ViewBag.priceOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Price).ThenByDescending(c => c.ProductId);
                    case "price_desc":
                        ViewBag.priceOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Price).ThenByDescending(c => c.ProductId);

                    case "taxable":
                        ViewBag.taxableOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.IsTaxable).ThenByDescending(c => c.ProductId);
                    case "taxable_desc":
                        ViewBag.taxableOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.IsTaxable).ThenByDescending(c => c.ProductId);

                    case "sku":
                        ViewBag.skuOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.SKU).ThenByDescending(c => c.ProductId);
                    case "sku_desc":
                        ViewBag.skuOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.SKU).ThenByDescending(c => c.ProductId);

                    case "category":
                        ViewBag.categoryOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Category.Name).ThenByDescending(c => c.ProductId);
                    case "category_desc":
                        ViewBag.categoryOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Category.Name).ThenByDescending(c => c.ProductId);


                    default:
                        ViewBag.idOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.ProductId);
                }
            };



            Expression<Func<Product, bool>> filter = _ => true;

            if (!String.IsNullOrEmpty(searchString))
            {
                filter = f => f.Name.Contains(searchString) || f.Description.Contains(searchString) ||
                    f.SKU.Contains(searchString) || f.Category.Name.Contains(searchString);
            }


            Expression<Func<Product, bool>> filter2 = f => !CategoryId.HasValue || CategoryId == f.CategoryId;

            //var body = Expression.AndAlso(filter2.Body, filter.Body);
            //var lambda = Expression.Lambda<Func<Product, bool>>(body, filter2.Parameters[0]);

            var lambda = filter2.AndAlso(filter);



            ViewBag.Display = pageSize;

            int pageNumber = (page ?? 1);

            var model = uow.ProductRepository
                .GetPage(pageSize, pageNumber,
                //out count,
                filter: lambda,
                orderBy: orderBy
                );

            return View(model); // uow.ProductRepository.Get(p => !CategoryId.HasValue || CategoryId.Value == p.CategoryId)
        }
        /// <summary>
        /// View Bag Data
        /// </summary>
        private void CreateViewBag()
        {
            ViewBag.FulfillmentProviders = uow.FulfillmentProviderRepository.Get(); // repo.FulfillmentProviders();
            ViewBag.Products = uow.ProductRepository.Get(); // repo.Products();
            ViewBag.Balancers = uow.BalancerRepository.Get(); // repo.Balancers();
            ViewBag.Processors = uow.ProcessorRepository.Get(); // repo.Processors();
            ViewBag.Categories = uow.CategoryRepository.Get(); // repo.Categories();
            ViewBag.AutoResponderProviders = uow.AutoResponderProviderRepository.Get(); //  repo.AutoResponderProviders;
            ViewBag.SalvageProfiles = uow.SalvageProfileRepository.Get();
            ViewBag.TaxProfiles = uow.TaxProfileRepository.Get();
           
            var sb = new StringBuilder();
            foreach (UrlOrderAction value in Enum.GetValues(typeof(UrlOrderAction)))
            {
                var name = KontinuityCRM.Helpers.EnumHelper<UrlOrderAction>.GetDisplayValue(value);

                sb.AppendFormat("<label><input type=\"checkbox\" name=\"PostBackUrls[_index].OrderActions\" value=\"{0}\"/> {1}</label>  ",
                       value, name);

            }

            ViewBag.OrderActions = sb.ToString();

        }
        /// <summary>
        /// Redirect to product create view
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            CreateViewBag();
            ViewBag.Events = uow.EventRepository.Get();
            ViewBag.PaymentTypes = new SelectList(uow.PaymentTypesRepository.Get(), "PaymentTypeId", "Name");

            return View();

        }
        /// <summary>
        /// post action to create Product
        /// </summary>
        /// <param name="product"></param>
        /// <param name="billday"></param>
        /// <param name="dayofweek"></param>
        /// <param name="selfrecurring"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(Product product, BillDay billday, DayOfWeek dayofweek, bool selfrecurring)
        {
            if (ModelState.IsValid)
            {
                // either a product has a balancer or has a processor
                if (product.LoadBalancer)
                    product.ProcessorId = null;
                else
                    product.BalancerId = null;

                var added = false;

                if (product.IsSubscription)
                {
                    if (product.BillType == BillType.ByDay)
                        product.BillValue = KontinuityCRMHelper.GetBillValue(billday, dayofweek);

                    if (selfrecurring)
                    {

                        uow.ProductRepository.Add(product);
                        uow.Save(wsw.CurrentUserName);

                        product.RecurringProductId = product.ProductId;

                        uow.ProductRepository.Update(product);
                        added = true;
                    }
                }
                else
                {
                    // clear subscription values
                    product.BillValue = null;
                    product.RecurringProductId = null;
                }

                if (!added)
                {
                    uow.ProductRepository.Add(product);
                }

                uow.Save(wsw.CurrentUserName);

                foreach (var item in product.PaymentTypeIds)
                {
                    uow.ProductPaymentTypeRepository.Add(new ProductPaymentType() { PaymentTypeId = Convert.ToInt32(item), ProductId = product.ProductId, CreatedOn = DateTime.Now });
                }

                uow.Save(wsw.CurrentUserName);
                return RedirectToAction("index");
            }
            CreateViewBag();
            ViewBag.Events = uow.EventRepository.Get();
            return View(product);
        }

        /// <summary>
        /// Create Product.
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public ActionResult CreateProduct(Product product)
        {
            ViewBag.errors = "";
                try
                {
                    // either a product has a balancer or has a processor
                    if (product.LoadBalancer)
                        product.ProcessorId = null;
                    else
                        product.BalancerId = null;

                    // clear subscription values
                    product.BillValue = null;
                    product.RecurringProductId = null;

                    uow.ProductRepository.Add(product);

                    uow.Save(wsw.CurrentUserName);

                    foreach (var item in product.PaymentTypeIds)
                    {
                        uow.ProductPaymentTypeRepository.Add(new ProductPaymentType() { PaymentTypeId = Convert.ToInt32(item), ProductId = product.ProductId, CreatedOn = DateTime.Now });
                    }

                    uow.Save(wsw.CurrentUserName);
                    ViewBag.message = "Product Added successfully";
                }
                catch (Exception ex)
                {
                    ViewBag.message = ex.Message;
                    return Json("Failed", JsonRequestBehavior.AllowGet);
                }
           
            return Json(ViewBag.message, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get All Product list.
        /// </summary>
        /// <returns></returns>
        public JsonResult GetAllProduct() {

            var productList = uow.ProductRepository.Get().AsEnumerable().Select(o => new Product
            {
                ProductId = o.ProductId,
                Name = o.Name
            }).ToList();
            
            return Json(new { ProductList = productList }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Redirects to product edit view
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            Product product = uow.ProductRepository.Find(id); // repo.FindProduct(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            CreateViewBag();

            var pe = product.ProductEvents.Select(e => e.EventId);
            ViewBag.Events = uow.EventRepository.Get(e => !pe.Contains(e.Id));
            ViewBag.PaymentTypes = new SelectList(uow.PaymentTypesRepository.Get(), "PaymentTypeId", "Name");
            //FillByDayViewbag(product);
            List<string> ids = new List<string>();
            foreach (var item in uow.ProductPaymentTypeRepository.Get(a => a.ProductId == product.ProductId))
            {
                ids.Add(item.PaymentTypeId.ToString());
            }
            product.PaymentTypeIds = ids;

            return View(product);
        }

        //private void FillByDayViewbag(Product product)
        //{
        //    var billday = BillDay.First;
        //    var dayofweek = DayOfWeek.Sunday;
        //    if (product.IsSubscription && product.BillType == BillType.ByDay)
        //    {
        //        KontinuityCRMHelper.DecodeBillValue(product.BillValue, out billday, out dayofweek);

        //        product.BillValue = "";
        //    }
        //    ViewBag.billday = billday;
        //    ViewBag.dayofweek = dayofweek;
        //}

        //
        // POST: /category/Edit/5
        /// <summary>
        /// Post action to update product
        /// </summary>
        /// <param name="product"></param>
        /// <param name="billday"></param>
        /// <param name="dayofweek"></param>
        /// <param name="Provider"></param>
        /// <param name="selfrecurring"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(Product product, BillDay billday, DayOfWeek dayofweek, bool Provider, bool selfrecurring)
        {
            if (ModelState.IsValid)
            {
                // either a product has a balancer or has a processor
                if (product.LoadBalancer)
                    product.ProcessorId = null;
                else
                    product.BalancerId = null;

                if (product.IsSubscription)
                {
                    if (product.BillType == BillType.ByDay)
                        product.BillValue = KontinuityCRMHelper.GetBillValue(billday, dayofweek);

                    if (selfrecurring)
                        product.RecurringProductId = product.ProductId;
                }
                else
                {
                    // clear subscription values
                    product.BillValue = null;
                    product.RecurringProductId = null;
                    product.BillType = BillType.ByCycle;
                    product.ProductSalvages = null;
                }

                if (!Provider)
                {
                    product.AutoResponderProviderId = null;
                    product.AutoResponderCustomerId = null;
                    product.AutoResponderProspectId = null;
                }

                if (!product.HasRedemption)
                {
                    product.RedemptionPoints = null;
                    product.SalePoints = null;
                }

                #region ##   Edit ProductEvents ##
                var productevents = uow.ProductEventRepository.Get(p => p.ProductId == product.ProductId);

                if (product.ProductEvents == null) // delete all
                {
                    foreach (var pe in productevents)
                        uow.ProductEventRepository.Delete(pe);
                }
                else
                {
                    // delete
                    foreach (var pe in productevents.Where(p => !product.ProductEvents.Select(e => e.EventId).Contains(p.EventId)))
                        uow.ProductEventRepository.Delete(pe);

                    // add 
                    foreach (var pe in product.ProductEvents.Where(p => !productevents.Select(e => e.EventId).Contains(p.EventId)))
                        uow.ProductEventRepository.Add(pe);
                }
                #endregion

                #region ##   Edit ProductSalavages ##
                var productsalvages = uow.ProductSalvageRepository.Get(p => p.ProductId == product.ProductId);

                if (product.ProductSalvages == null) // delete all
                {
                    foreach (var pe in productsalvages)
                        uow.ProductSalvageRepository.Delete(pe);
                }
                else
                {
                    // delete
                    foreach (var pe in productsalvages.Where(p => !product.ProductSalvages.Select(e => e.SalvageProfileId).Contains(p.SalvageProfileId)))
                        uow.ProductSalvageRepository.Delete(pe);

                    // add 
                    foreach (var pe in product.ProductSalvages.Where(p => !productsalvages.Select(e => e.SalvageProfileId).Contains(p.SalvageProfileId)))
                        uow.ProductSalvageRepository.Add(pe);
                }
                #endregion

                #region ## Edit PostBackUrls ##

                var postbackurls = uow.PostBackUrlRepository.Get(p => p.ProductId == product.ProductId);


                if (product.PostBackUrls == null)
                {
                    foreach (var pb in postbackurls)
                        uow.PostBackUrlRepository.Delete(pb);
                }
                else
                {
                    // delete
                    foreach (var pe in postbackurls.Where(p => !product.PostBackUrls.Select(e => e.Id).Contains(p.Id)))
                        uow.PostBackUrlRepository.Delete(pe);

                    // add 
                    foreach (var pe in product.PostBackUrls.Where(p => !postbackurls.Select(e => e.Id).Contains(p.Id)))
                        uow.PostBackUrlRepository.Add(pe);

                    // edit
                    foreach (var pe in product.PostBackUrls.Where(p => postbackurls.Select(e => e.Id).Contains(p.Id)))
                    {
                        var original = postbackurls.Single(e => e.Id == pe.Id);
                        uow.PostBackUrlRepository.Update(original, pe);
                    }
                }


                #endregion

                // avoid PK confict!!!
                product.ProductEvents = null;
                product.PostBackUrls = null;
                product.ProductSalvages = null;

                uow.ProductRepository.Update(product);
                uow.Save(wsw.CurrentUserName);

                foreach (var item in uow.ProductPaymentTypeRepository.Get(a => a.ProductId == product.ProductId))
                {
                    uow.ProductPaymentTypeRepository.Delete(item.Id);
                }
                uow.Save(wsw.CurrentUserName);

                foreach (var item in product.PaymentTypeIds)
                {
                    uow.ProductPaymentTypeRepository.Add(new ProductPaymentType() { PaymentTypeId = Convert.ToInt32(item), ProductId = product.ProductId, CreatedOn = DateTime.Now });
                }

                uow.Save(wsw.CurrentUserName);

                return RedirectToAction("index");
            }

            CreateViewBag();
            //FillByDayViewbag(product);
            var _pe = product.ProductEvents.Select(e => e.EventId);
            ViewBag.Events = uow.EventRepository.Get(e => !_pe.Contains(e.Id));

            return View(product);
        }
        /// <summary>
        /// Delete Product by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int id)
        {
            Product product = uow.ProductRepository.Find(id); // repo.FindProduct(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            //How remove the autoresponders list associated with this product ? Is that necessary??

            uow.ProductRepository.Delete(product);
            uow.Save(wsw.CurrentUserName);
            return RedirectToAction("Index");
        }

        #region Variants



        public ActionResult Variants(int id)
        {
            Product product = repo.FindProduct(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.Product = product;
            return View(repo.ProductVariants().Where(p => p.ProductId == product.ProductId));
        }

        public ActionResult EditVariant(int id)
        {
            var e = repo.FindProductVariant(id);
            if (e == null)
            {
                return HttpNotFound();
            }
            ViewBag.ExtraFields = from f in Enum.GetNames(typeof(ExtraFields))
                                  join x in repo.VariantExtraFields().Where(x => x.ProductVariantId == e.ProductVariantId) on f equals x.FieldName into fx
                                  from x in fx.DefaultIfEmpty()
                                  select new SelectListItem
                                  {
                                      Value = x != null ? x.FieldValue : "",
                                      Text = f,
                                      Selected = x != null
                                  };
            ViewBag.Product = e.Product;
            ViewBag.Countries = repo.Countries();
            return View(e);
        }

        [HttpPost]
        public ActionResult EditVariant(ProductVariant pv, string[] extra)
        {
            if (ModelState.IsValid)
            {
                using (var scope = new TransactionScope())
                {
                    repo.EditProductVariant(pv);

                    repo.RemoveExtraFields(pv);

                    if (extra != null)
                    {
                        foreach (var item in extra)
                        {
                            // add the extrafield to the 
                            repo.AddVariantExtraField(new VariantExtraField
                            {
                                FieldName = item,
                                FieldValue = Request.Form[item],
                                ProductVariantId = pv.ProductVariantId
                            });
                        }
                    }

                    scope.Complete();
                    return RedirectToAction("variants", new { id = pv.ProductId });
                }


            }
            ViewBag.ExtraFields = from f in Enum.GetNames(typeof(ExtraFields))
                                  join x in extra on f equals x into fx
                                  from x in fx.DefaultIfEmpty()
                                  select new SelectListItem
                                  {
                                      Value = x != null ? Request.Form[f] : "",
                                      Text = f,
                                      Selected = x != null
                                  };
            ViewBag.Countries = repo.Countries();
            ViewBag.Product = repo.FindProduct(pv.ProductId);
            return View(pv);
        }

        public ActionResult CreateVariant(int id)
        {
            Product p = repo.FindProduct(id);
            if (p == null)
            {
                return HttpNotFound();
            }
            ViewBag.Countries = repo.Countries();
            ViewBag.Product = p;
            return View();
        }

        [HttpPost]
        public ActionResult CreateVariant(ProductVariant pv, string[] extra)
        {
            if (ModelState.IsValid)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    repo.AddProductVariant(pv);

                    if (extra != null)
                    {
                        foreach (var item in extra)
                        {
                            // add the extrafield to the 

                            repo.AddVariantExtraField(new VariantExtraField
                            {
                                FieldName = item,
                                FieldValue = Request.Form[item],
                                ProductVariantId = pv.ProductVariantId
                            });
                        }
                    }

                    scope.Complete(); // if reach this point make the changes and go okay
                    return RedirectToAction("variants", new { id = pv.ProductId });
                }
            }

            ViewBag.Countries = repo.Countries();
            ViewBag.Product = repo.FindProduct(pv.ProductId);
            return View(pv);
        }

        public ActionResult DeleteVariant(int id)
        {
            ProductVariant pv = repo.FindProductVariant(id);
            if (pv == null)
            {
                return HttpNotFound();
            }
            ViewBag.Product = pv.Product;
            return View(pv);
        }

        [HttpPost, ActionName("DeleteVariant")]
        public ActionResult DeleteVariantConfirmed(int id)
        {
            ProductVariant pv = repo.FindProductVariant(id);
            repo.RemoveProductVariant(pv);
            return RedirectToAction("variants", new { id = pv.ProductId });
        }

        #endregion

        //public ActionResult Salvage(int id)
        //{
        //    var salvage = repo.DeclineSalvages().FirstOrDefault(s => s.ProductId == id);

        //    var billday = BillDay.First;
        //    var dayofweek = DayOfWeek.Sunday;

        //    if (salvage != null && salvage.BillType == BillType.ByDay)
        //    {
        //        KontinuityCRMHelper.DecodeBillValue(salvage.BillValue, out billday, out dayofweek);

        //        salvage.BillValue = "";
        //    }
        //    ViewBag.billday = billday;
        //    ViewBag.dayofweek = dayofweek;


        //    return View(salvage);
        //}

        //public ActionResult CreateSalvage(int id)
        //{
        //    repo.AddDeclineSalvage(new DeclineSalvage()
        //    {
        //        ProductId = id,
        //        CancelAfter = 3,
        //        BillType = BillType.ByCycle,
        //        BillValue = "30",
        //    });

        //    return RedirectToAction("salvage", new { id = id });

        //}

        //[HttpPost]
        //public ActionResult Salvage(DeclineSalvage salvage, BillDay billday, DayOfWeek dayofweek)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (salvage.BillType == BillType.ByDay)
        //        {
        //            salvage.BillValue = string.Format("{0}-{1}", billday, dayofweek);
        //        }
        //        uow.DeclineSalvageRepository.Update(salvage); //repo.EditDeclineSalvage(salvage);
        //        uow.Save();

        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.billday = billday;
        //    ViewBag.dayofweek = dayofweek;

        //    return View(salvage);
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    //repo.Dispose();
        //    base.Dispose(disposing);
        //}


    }
}
