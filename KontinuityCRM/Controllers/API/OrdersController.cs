using KontinuityCRM.Helpers;
using KontinuityCRM.Models.APIModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Transactions;
using System.Web;
using System.Web.Security;
using System.Web.Http;
using System.Web.Http.OData;
using System.ServiceModel.Channels;
using System.Text;
using KontinuityCRM.Models.Enums;
using WebMatrix.WebData;
using AutoMapper;
//using ArtisanCode.SimpleAesEncryption;
using System.Threading.Tasks;
using KontinuityCRM.Models;
using KontinuityCRM.Filters;
using System.Diagnostics;
using NLog;

namespace KontinuityCRM.Controllers.API
{
    /// <summary>
    /// 
    /// </summary>
    [Authorize]
    public class ordersController : ApiController
    {
        private readonly IMappingEngine mapper = null;
        private readonly IUnitOfWork uow = null;
        private readonly IWebSecurityWrapper wsw = null;
        private readonly INLogger logger = null;
        //private readonly IMessageEncryptor me = null;
        //private readonly IMessageDecryptor md = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uow"></param>
        /// <param name="wsw"></param>
        /// <param name="mapper"></param>       
        public ordersController(IUnitOfWork uow, IWebSecurityWrapper wsw, /*IMessageEncryptor me,*/ IMappingEngine mapper/*, IMessageDecryptor md*/, INLogger logger)
        {
            this.uow = uow;
            this.wsw = wsw;
            //this.me = me;
            this.mapper = mapper;
            this.logger = logger;
            //this.md = md;
        }

        /// <summary>
        /// Get an order by id
        /// </summary>
        /// <param name="id">Order Id</param>
        /// <returns></returns>
        public HttpResponseMessage Get(int id) //IHttpActionResult
        {
            try
            {
                logger.LogInfo("Step1: Process started getting order by id:"+id);
                var order = uow.OrderRepository.Find(id);
                if (order != null)
                {
                    logger.LogInfo("Step2: Order found successfully. Now returning result");
                    var  model = mapper.Map<KontinuityCRM.Models.Order, KontinuityCRM.Models.APIModels.OrderAPIModel>(order);
                    return Request.CreateResponse(HttpStatusCode.Created, model);
                }
                else {
                    logger.LogInfo("Step2: No result found. Throwing exception");
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No order found against this id. Try with different order id.");
                }
                
            }
            catch (Exception ex)
            {
                logger.LogInfo("Process failed getting order by id. Possible reason could be:" + ex.Message);
                throw;
            }
            //var order = repo.FindOrder(id);
          
                //Ok<OrderDto>(mapper.Map<Order, OrderDto>(order));
        }

        /// <summary>
        /// Process an order. Only works on initial orders that haven't been processed or have been declined.
        /// </summary>
        /// <param name="id">Order Id</param>
        /// <returns></returns>        
        [HttpPost]
        [Route("api/orders/process/{id}")]
        public async Task<HttpResponseMessage> process([FromUri] int id)
        {
            try
            {
                var order = uow.OrderRepository.Find(id); //repo.FindOrder(id);
                if (order == null)
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No order found against this id. Try with different order id:"+id);

                if ((order.Status == OrderStatus.Declined && !order.HasRebills && !order.IsRebill)
                    || order.Status == OrderStatus.Unpaid)
                {
                    foreach (var op in order.OrderProducts)
                    {
                        op.Recurring = true; // will this be updated ? it not => update every op in another loop 
                    }
                    await order.Process(uow, wsw, mapper);
                }

                return Request.CreateResponse(HttpStatusCode.NoContent);
            }
            catch (Exception)
            {
                throw;
            } 
        }

        /// <summary>
        /// Force rebill on the order. Only works if the order has a scheduled rebill.
        /// </summary>
        /// <param name="id">Order Id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/orders/rebill/{id}")]
        public async Task<HttpResponseMessage> rebill(int id)
        {
            try
            {
                logger.LogInfo("Step1:Process started rebilling with id :"+id);
                var order = uow.OrderRepository.Find(id); // repo.FindOrder(id);
                if (order == null)
                {
                    logger.LogInfo("Step2: Order does not exists. Process terminating request with error not found");
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No order found against this id. Try with different order id:"+id);
                }

                logger.LogInfo("Step2: Order has been found and process would extract order products from order");
                var rebill = false;
                foreach (var op in order.OrderProducts.Where(p => p.NextDate != null))
                {
                    op.NextDate = DateTime.Today;
                    rebill = true;
                }
                
                if (rebill)
                {
                    logger.LogInfo("Step3: Rebill is enabled and now process is rebilling order");
                    await order.Rebill(uow, wsw, mapper);
                }
                else
                    logger.LogInfo("Step3: Rebill is disabled. Hence rebilling order is skipped");

                return Request.CreateResponse(HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                logger.LogInfo("Error: Rebill failed and possible reason could be:"+ex);
                throw;
            }
            
        }

        /// <summary>
        /// Create a new order
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        public async Task<HttpResponseMessage> Post(OrderCreateModel order)
        {
            if (Request.Method.Method == "OPTIONS")
            {
                var response = new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
                response.Headers.Add("Access-Control-Allow-Origin","*");
                response.Headers.Add("Access-Control-Allow-Headers", "X-Requested-With");
            }
            try
            {
                var IsTestCardNumber = uow.TestCardNumberRepository.Get(g => g.Number == order.CreditCardNumber);
                logger.LogInfo("Step1:Process started posting order by the user:"+order.Email+", process would iterate on order products to check single purchase limit and other");
                foreach (var op in order.OrderProducts)
                {
                    Product product = uow.ProductRepository.Find(op.ProductId);
                    logger.LogInfo("Checking single purchase limit for product:"+ product.ProductId+" product name :"+ product.Name);
                    if (IsTestCardNumber.Count() == 0 && product.IsSinglePurchaseLimit)
                    {
                        logger.LogInfo("Single purchase limit is enabled for product id:"+ product.ProductId);
                        var orders = uow.OrderRepository.Get(o =>
                            o.OrderProducts.Select(p => p.ProductId).Contains(product.ProductId)
                            && (int)o.Status == 1
                            && ((o.Email == order.Email) || (o.Customer.Phone == order.Phone) || (o.CreditCardNumber == order.CreditCardNumber))
                            );
                        //check the count as well bcz sometimes orders in not null but its count is 0
                        if (orders != null && orders.Count() > 0)
                        {
                            logger.LogInfo("Product id:"+product.ProductId+" has single purchase limit on and it has multiple orders :"+orders.Count()+". Process would stop here. You can't order this product");
                            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Signle purchase limit stops further processing. Product id: " +product.ProductId);
                        }
                        logger.LogInfo("Product id:" + product.ProductId + " has signle purchase limit on but no order found before this order. Process continued normally");
                    }
                }

                var _order = mapper.Map<KontinuityCRM.Models.Order>(order);
                _order.CreatedUserId = wsw.CurrentUserId;
                if (string.IsNullOrEmpty(_order.IPAddress))
                    _order.IPAddress = GetClientIp(Request);
                logger.LogInfo("Step2:Process started creating new order");
                await CreateOrder(_order, uow, wsw, mapper);
                logger.LogInfo("Step3:Process completed created new order successfully");
                #region ## Create response obj


                //var creationResponse = APIHelper.CreateOrderResponse(_order); // new OrderCreationResponse(_order, uow, wsw/*, me*/);
                var creationResponse = mapper.Map<OrderCreationResponse>(_order);


                #endregion
               // var response = Request.CreateResponse(HttpStatusCode.Created, creationResponse);
                //var response = Request.CreateResponse<Order>(HttpStatusCode.Created, order);
                // 
                // this is creating 201 codes for every decline.  
                // need to check the status and display the proper error code

                // this also got wiped out in merge

                HttpResponseMessage response;
                if (_order.Status == OrderStatus.Approved)
                {
                    logger.LogInfo("step4: Order status is APPROVED. hence returning result with CREATED code");
                    response = Request.CreateResponse(HttpStatusCode.Created, creationResponse);
                }
                else
                {
                    logger.LogInfo("step4: Order status is not APPROVED. hence returning result with PAYMENT_REQUIRED code");
                    response = Request.CreateResponse(HttpStatusCode.PaymentRequired, creationResponse);
                }
                string uri = Url.Link("DefaultApi", new { id = _order.OrderId });
                response.Headers.Location = new Uri(uri);

                return response;
            }
            catch (Exception ex)
            {
                logger.LogInfo("Error: Failed posting order - possible reason could be:"+ex);
                throw;
            }
        }

        protected virtual async Task CreateOrder(Order order, IUnitOfWork uow, IWebSecurityWrapper wsw, IMappingEngine mapper)
        {
            await order.Create(uow, wsw, mapper);
        }

        /// <summary>
        /// Upsell an order
        /// </summary>
        /// <param name="id">Order Id</param>
        /// <param name="upsellproducts">Upsell products</param>
        /// <returns></returns>
        [HttpPost]    
        [Route("api/orders/upsell/{id}")]
        public async Task<HttpResponseMessage> upsell(int id
            //, [FromBody] UpsellAPIModel upsellmodel
            , [FromBody] ICollection<KontinuityCRM.Models.APIModels.OrderProduct> upsellproducts
            )
        {
            try
            {

                if (upsellproducts == null || upsellproducts.Count == 0)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Upsell products are not available or invalid. Try again with valid data");
                }
                var previousOrder = uow.OrderRepository.Find(id);
                if (previousOrder == null)
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Previous order not found.");
                
                var IsTestCardNumber = uow.TestCardNumberRepository.Get(g => g.Number == previousOrder.CreditCardNumber);
                foreach (var op in upsellproducts)
                {
                    var product = uow.ProductRepository.Find(op.ProductId);
                    if (product == null)
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Product in upsell product not found. Prodcut id:"+op.ProductId);


                    if (IsTestCardNumber.Count() == 0 &&product.IsSinglePurchaseLimit)
                    {
                        logger.LogInfo("Upsell: Single purchase limit is enabled for product id:" + product.ProductId);
                        var orders = uow.OrderRepository.Get(o =>
                            o.OrderProducts.Select(p => p.ProductId).Contains(product.ProductId)
                            && (int)o.Status == 1
                            && ((o.Email == previousOrder.Email) || (o.Customer.Phone == previousOrder.Phone) || (o.CreditCardNumber == previousOrder.CreditCardNumber))
                            );
                        //check the count as well bcz sometimes orders in not null but its count is 0
                        if (orders != null && orders.Count() > 0)
                        {
                            logger.LogInfo("Product id:" + product.ProductId + " has single purchase limit on and it has multiple orders :" + orders.Count() + ". Process would stop here. You can't order this product");
                            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No order found against this id. Try with different order id.");
                        }
                        logger.LogInfo("Product id:" + product.ProductId + " has signle purchase limit on but no order found before this order. Process continued normally");
                    }
                }


                var _order = new KontinuityCRM.Models.Order()
                {
                    AffiliateId = previousOrder.AffiliateId,
                    SubId = previousOrder.SubId,

                    BillingAddress1 = previousOrder.BillingAddress1,
                    BillingAddress2 = previousOrder.BillingAddress2,
                    BillingCity = previousOrder.BillingCity,
                    BillingCountry = previousOrder.BillingCountry,
                    BillingFirstName = previousOrder.BillingFirstName,
                    BillingLastName = previousOrder.BillingLastName,
                    BillingPostalCode = previousOrder.BillingPostalCode,
                    BillingProvince = previousOrder.BillingProvince,

                    CreditCardCVV = previousOrder.CreditCardCVV,
                    CreditCardExpirationMonth = previousOrder.CreditCardExpirationMonth,
                    CreditCardExpirationYear = previousOrder.CreditCardExpirationYear,
                    CreditCardNumber = previousOrder.CreditCardNumber,

                    CustomerId = previousOrder.CustomerId,
                    Customer = previousOrder.Customer,

                    Email = previousOrder.Email,
                    Phone = previousOrder.Phone,

                    ShippingAddress1 = previousOrder.ShippingAddress1,
                    ShippingAddress2 = previousOrder.ShippingAddress2,
                    ShippingCity = previousOrder.ShippingCity,
                    ShippingCountry = previousOrder.ShippingCountry,
                    ShippingFirstName = previousOrder.ShippingFirstName,
                    ShippingLastName = previousOrder.ShippingLastName,
                    ShippingPostalCode = previousOrder.ShippingPostalCode,
                    ShippingProvince = previousOrder.ShippingProvince,

                    ShippingMethodId = previousOrder.ShippingMethodId,
                    ShippingMethod = previousOrder.ShippingMethod,

                    //OrderProducts = upsellproducts,

                };

                _order.OrderProducts = new List<KontinuityCRM.Models.OrderProduct>();

                foreach (var op in upsellproducts)
                {
                    Product Product = uow.ProductRepository.Find(op.ProductId);
                    if (Product != null)
                    {
                        var mappedOP = mapper.Map<KontinuityCRM.Models.OrderProduct>(Product);
                        mappedOP.Quantity = op.Quantity;
                        mappedOP.Price = op.Price;
                        _order.OrderProducts.Add(mappedOP);
                    }
                }

                _order.CreatedUserId = wsw.CurrentUserId;
                _order.IPAddress = GetClientIp(Request);

                await CreateOrder(_order, uow, wsw, mapper);

                var order = mapper.Map<OrderAPIModel>(_order);

                var response = Request.CreateResponse<OrderAPIModel>(HttpStatusCode.Created, order);
                string uri = Url.Link("DefaultApi", new { id = order.OrderId });
                response.Headers.Location = new Uri(uri);
                return response;
            }
            catch (Exception ex)
            {
                logger.LogException("Process failed upsell order. Possible reason could be:",ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Failed at upsell, possible reason could be:"+ex.Message);
            }
        }


        /// <summary>
        /// Find orders
        /// </summary>
        /// <param name="fromDate">Specify the start date of the date range in which the order was created.</param>
        /// <param name="toDate">Specify the end date of the date range in which the order was created.</param>
        /// <param name="status">Status filter. If zero is passed then there will be no filter by order status. An optional parameter.</param>
        /// <param name="productId">Product Id filter. An optional parameter.</param>
        /// <param name="orderId">Order Id filter. An optional parameter.</param>
        /// <param name="affiliateId">Affiliate Id filter. An optional parameter.</param>
        /// <param name="customerId">Customer Id filter. An optional parameter.</param>
        /// <param name="shipped">Shipped filter. If true it will return order that has been shipped or not otherwise. If omitted then no filter is applied.</param>
        /// <param name="address">Address filter. An optional parameter.</param>
        /// <param name="address2">Second address filter. An optional parameter.</param>
        /// <param name="firstName">First name filter. An optional parameter.</param>
        /// <param name="lastName">Last name filter. An optional parameter.</param>
        /// <param name="subId">Sub Id filter. An optional parameter.</param>
        /// <param name="email">Email filter. An optional parameter.</param>
        /// <param name="city">City filter. An optional parameter.</param>
        /// <param name="zip">ZIP filter. An optional parameter.</param>
        /// <param name="phone">Phone filter. An optional parameter.</param>
        /// <param name="state">State filter. An optional parameter.</param>
        /// <param name="country">Country filter. An optional parameter.</param>
        /// <param name="transactionId">Transaction Id filter. An optional parameter.</param>
        /// <param name="rma">RMA number filter. An optional parameter.</param>
        /// <param name="ip">IP address filter. An optional parameter.</param>
        /// <param name="depth">An optional parameter. It represents the order's rebill depth. If zero all rebill orders will be retrieved regardless of the rebill depth. 
        /// If depth if greater than zero the orders returned will match that rebill depth and if -1 the orders returned will be the initials orders.</param>
        /// <param name="orderView">An optional parameter that alters the result type. By default the method returns the list of the ids of those orders that match the request data, 
        /// if this parameter is set to true it will return instead a list with each order data.</param>
        /// <returns>Depending of the orderView parameter returns a list with the order's id or a list with the full order data.</returns>
        [HttpGet]
        public IHttpActionResult find(DateTime fromDate, DateTime toDate,
            OrderStatus status = OrderStatus.Approved,
            int? productId = null,
            int? orderId = null,
            string affiliateId = null,
            int? customerId = null,
            bool? shipped = null,
            string address = null,
            string address2 = null,
            string firstName = null,
            string lastName = null,
            string subId = null,
            string email = null,
            string city = null,
            string zip = null,
            string phone = null,
            string state = null,
            string country = null,
            string transactionId = null,
            string rma = null,
            string ip = null, int? depth = -1, bool orderView = false)
        {
            try
            {
                NInLogger logger = new NInLogger();
                logger.LogInfo("Process sarted finding order: from date:"+fromDate+", to date:"+toDate);
                // RMA # filter
                int? frmaOrderId = null;
                int? frmaProductId = null;
                if (!string.IsNullOrEmpty(rma))
                {
                    logger.LogInfo("RMA is not empty");
                    if (System.Text.RegularExpressions.Regex.IsMatch(rma, @"^\d+-\d+$"))
                    {
                        string[] rmaa = string.IsNullOrEmpty(rma) ? null : rma.Split('-');
                        frmaOrderId = int.Parse(rmaa[0]);
                        frmaProductId = int.Parse(rmaa[1]);
                    }
                    else
                    {
                        frmaOrderId = 0;
                    }
                }

                var ufdate = fromDate.ToUniversalTime();
                var utdate = toDate.ToUniversalTime();

                var orders = uow.OrderRepository.Get(o =>
                    (ip == null || o.IPAddress == ip)

                    && ufdate <= o.Created && o.Created <= utdate
                    && ((depth == -1 && o.Depth == 0) || (depth == 0 && o.Depth > 0) || (depth > 0 && o.Depth == depth))

                    && (orderId == null || o.OrderId == orderId)
                    && (((int)status) == 0 || o.Status == status)
                    && (affiliateId == null || o.AffiliateId == affiliateId)
                    && (customerId == null || o.CustomerId == customerId)
                    && (productId == null || o.OrderProducts.Select(p => p.ProductId).Contains(productId.Value))
                    && (shipped == null || o.Shipped == shipped)
                    && (address == null || o.Customer.Address1 == address)
                    && (address2 == null || o.Customer.Address2 == address2)
                    && (firstName == null || o.Customer.FirstName == firstName)
                    && (lastName == null || o.Customer.LastName == lastName)
                    && (subId == null || o.SubId == subId)
                    && (email == null || o.Customer.Email == email)
                    && (city == null || o.Customer.City == city)
                    && (zip == null || o.Customer.PostalCode == zip)
                    && (phone == null || o.Customer.Phone == phone)
                    && (state == null || o.Customer.Province == state)
                    && (country == null || o.ShippingCountry == country)
                    && (transactionId == null || o.Transactions.Select(t => t.TransactionId).Contains(transactionId))
                    && (!frmaOrderId.HasValue || o.OrderProducts.Any(p => p.OrderId == frmaOrderId && p.ProductId == frmaProductId && p.RMAReasonId.HasValue))
                    );

                logger.LogInfo("Orderview value is:"+ orderView);
                if (!orderView)
                    return Ok(orders.Select(o => o.OrderId));

                logger.LogInfo("Orderview value is not enabled therefore, showing order details");
                var result = new List<OrderAPIModel>();
                foreach (var order in orders)
                    result.Add(mapper.Map<OrderAPIModel>(order));

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogInfo("Process failed finding order. Possible reason could be:"+ex);
                throw;
            } 
        }

        private string GetClientIp(HttpRequestMessage request)
        {
            try
            {
                if (request.Properties.ContainsKey("MS_HttpContext"))
                {
                    return ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
                }
                else if (request.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
                {
                    RemoteEndpointMessageProperty prop;
                    prop = (RemoteEndpointMessageProperty)request.Properties[RemoteEndpointMessageProperty.Name];
                    return prop.Address;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                logger.LogInfo("Process failed getting Client IP, possible reason could be:"+ex);
                throw;
            }
            
        }

        // GET api/order/products/id 
        //[HttpGet]
        //public IEnumerable<Product> Products(int id)
        //{
        //    //var id = 7;
        //    var order = repo.FindOrder(id);
        //    if (order == null)
        //    {
        //        throw new HttpResponseException(this.Request.CreateResponse(HttpStatusCode.NotFound));
        //    }
        //    return repo.OrderProducts().Where(o => o.OrderId == id).Select(o => o.Product);
        //}



        // POST api/order

        //public HttpResponseMessage Post(PostOrderRequest request)

         // POST api/order/addproduct
        //[ActionName("AddProduct")]
        //[HttpPost]
        //public HttpResponseMessage AddProduct(OrderProduct op)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var product = repo.FindProduct(op.ProductId);
        //        op.Price = product.Price;
        //        repo.AddOrderProduct(op);

        //        HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.Created, op);
        //        response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = op.OrderId }));
        //        return response;
        //    }
        //    throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest));
        //}

        // DELETE api/order/id 
        //public HttpResponseMessage Delete(int id)
        //{
        //    var order = repo.FindOrder(id);
        //    if (order == null)
        //    {
        //        throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
        //    }
        //    repo.RemoveOrder(order);
        //    return this.Request.CreateResponse(HttpStatusCode.NoContent);
        //}

        //// PATCH api/order/id 
        //public HttpResponseMessage Patch(int id, Delta<Order> deltaorder)
        //{
        //    var order = repo.FindOrder(id);
        //    if (order == null)
        //    {
        //        throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
        //    }
        //    deltaorder.Patch(order);
        //    repo.SaveChanges();
        //    return Request.CreateResponse(HttpStatusCode.NoContent);
        //}

    }
}
