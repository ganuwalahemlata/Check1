using KontinuityCRM.Models.APIModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
//using System.Web.Mvc;
using KontinuityCRM.Helpers;
using System.Net;
using System.Net.Http;
using System.Web.Http.OData;
using System.ServiceModel.Channels;
//using ArtisanCode.SimpleAesEncryption;
using System.Threading.Tasks;
using System.Web.Http.Results;
using AutoMapper;
using KontinuityCRM.Filters;
using System.Web.Http.Cors;
using KontinuityCRM.Models;
using KontinuityCRM.Models.Enums;
using NLog;

namespace KontinuityCRM.Controllers.API
{
    /// <summary>
    /// 
    /// </summary>
    [Authorize]

    public class partialsController : ApiController
    {
        private readonly IWebSecurityWrapper wsw;
        private readonly IUnitOfWork uow = null;
        private readonly IMappingEngine mapper = null;
        private readonly INLogger logger = null;
        //private readonly IMessageEncryptor me = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uow"></param>
        /// <param name="wsw"></param>
        /// <param name="mapper"></param>
        /// <param name="me"></param>
        public partialsController(IUnitOfWork uow, IWebSecurityWrapper wsw, IMappingEngine mapper/*, IMessageEncryptor me*/, INLogger logger)
        {
            //repo = new EFKontinuityCRMRepo();
            this.uow = uow;
            this.wsw = wsw;
            //this.me = me;
            this.mapper = mapper;
            this.logger = logger;
        }



        // GET api/partial
        //public IEnumerable<Partial> Get()
        //{
        //    return uow.PartialRepository.Get(); //repo.Partials();
        //}

        // GET api/partial/id 

        /// <summary>
        /// Get a partial by id
        /// </summary>
        /// <param name="id">Partial Id</param>
        /// <returns></returns>
        public HttpResponseMessage Get(int id) // IHttpActionResult
        {
            try
            {
                logger.LogInfo("step1:process started gettin partial order by id:" + id);
                var partial = uow.PartialRepository.Find(id);
                if (partial != null)
                {
                    logger.LogInfo("Step2: Order found successfully. Now returning result");
                    var model = mapper.Map<PartialAPIModel>(partial);
                    return Request.CreateResponse(HttpStatusCode.Created, model);
                }
                else {
                    logger.LogInfo("Step2: No result found. Throwing exception");
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No partial order found against this id. Try with different order id.");
                }              
            }
            catch (Exception ex)
            {
                logger.LogInfo("Process failed getting partial against id:"+id+". Possible reason could be:"+ex);
                throw;
            }
            
            //return partial; //Ok(partial);
        }

        /// <summary>
        /// Create a new partial
        /// </summary>
        /// <param name="partialcreatemodel"></param>
        /// <returns></returns>
        ///  
      
        [HttpPost]
        [ValidateModel]
        public async Task<HttpResponseMessage> post(KontinuityCRM.Models.APIModels.PartialCreateModel partialcreatemodel)
        {
            if (Request.Method.Method == "OPTIONS")
            {
                var response = new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
                response.Headers.Add("Access-Control-Allow-Origin", "*");
                response.Headers.Add("Access-Control-Allow-Headers", "*");
            }
            try
            {
                if (ModelState.IsValid)
                {
                    logger.LogInfo("step1:process started posting partial order");
                    var _partial = mapper.Map<KontinuityCRM.Models.Partial>(partialcreatemodel);

                    if (_partial.IPAddress == null) // if no ip is given then try to get one 
                        _partial.IPAddress = GetClientIp(Request);
                    logger.LogInfo("step2:process started creating order");
                    await _partial.Create(uow, wsw, mapper);
                    logger.LogInfo("step3:process has created partial successfully");
                    var partial = mapper.Map<PartialAPIModel>(_partial);
                    return Request.CreateResponse(HttpStatusCode.Created, partial);
                }
                {
                    logger.LogInfo("Model instance is not valid. System generating error");
                    var listErrors = new List<string>();
                    var error = new HttpException((int)HttpStatusCode.BadRequest, "Model Errors");
                    int errIndex = 0;

                    foreach (System.Web.Http.ModelBinding.ModelState modelState in ModelState.Values)
                    {
                        foreach (System.Web.Http.ModelBinding.ModelError err in modelState.Errors)
                        {
                            if (!listErrors.Contains(err.ErrorMessage))
                            {
                                error.Data[errIndex++] = err.ErrorMessage;
                                listErrors.Add(err.ErrorMessage);
                            }
                        }
                    }

                    return Request.CreateResponse(HttpStatusCode.BadRequest, error);
                }
            }
            catch (Exception ex)
            {
                logger.LogInfo("Process failed posting partial order Possible reason could be:" + ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Partial generation failed.");
            }
                
            //}
           
            //throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState));
            
            //var resp = new HttpResponseMessage(HttpStatusCode.BadRequest)
            //{
            //    Content = new StringContent(string.Join("; ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage))),
            //    ReasonPhrase = "Invalid Model",
            //};
            //throw new HttpResponseException(resp);
        }


//        [EnableCors(origins: "*", headers: "*", methods: "*")]
////        [AllowCrossSiteJson]
//        // POST api/partial
//        [HttpPost]
//        public IHttpActionResult Edit(Partial partial)
//        {
//            //partial.IPAddress = Request.UserHostAddress;
//            if (ModelState.IsValid)
//            {
//                var dbPartial = uow.PartialRepository.Find(partial.PartialId);
//                uow.PartialRepository.Update(dbPartial, partial);
//                return Ok();
//            }
//            throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest));
//        }


        /// <summary>
        /// Place new order upon partial data
        /// </summary>
        /// <param name="id">Partial Id</param>
        /// <param name="model"></param>
        /// <returns></returns>
      
        [HttpPost]
        [Route("api/partials/order/{id}")]
        [ValidateModel]
        public async Task<HttpResponseMessage> order([FromUri] int id, [FromBody] PartialToOrderModel model)
        {
            try
            {
                logger.LogInfo("step1: process started posting partial order");
                if (ModelState.IsValid)
                {
                    logger.LogInfo("step2:Model state is valid. Process would perform other actions now");
                    var partial = uow.PartialRepository.Find(id);

                    if (partial == null)
                    {
                        logger.LogInfo("step3:process would stop here because, partial not found. Returning error partial not found");
                        var error = new HttpException((int)HttpStatusCode.NotFound, "Partial not found");
                        return Request.CreateResponse(HttpStatusCode.NotFound, error);
                    }

                    if (model.IPAddress == null) model.IPAddress = partial.IPAddress; // set the ip address from the partial\

                    if (model.IPAddress == null) model.IPAddress = GetClientIp(this.Request); // set the ip address from the request
                    logger.LogInfo("step4: IP address for this request");
                    var singlePurchaseLimitErrors = new Dictionary<int, string>();
                    logger.LogInfo("step5: Total number of products in this order:"+model.OrderProducts.Count);
                    bool isTestCard = uow.TestCardNumberRepository.Get(c => c.Number == model.CreditCardNumber).Any();
                    foreach (var op in model.OrderProducts)
                    {
                        Product product = uow.ProductRepository.Find(op.ProductId);

                        if (product.IsSinglePurchaseLimit && !isTestCard)
                        {
                            var orders = uow.OrderRepository.Get(o =>
                                o.OrderProducts.Select(p => p.ProductId).Contains(product.ProductId)
                                && (int)o.Status == 1
                                && ((o.Email == partial.Email) || (o.Phone == partial.Phone) || (o.CreditCardNumber == model.CreditCardNumber))
                                ).ToList();

                            if (orders.Count > 0)
                            {
                                singlePurchaseLimitErrors[op.ProductId] = "Single Purchase Limit Error on ProductId: " + product.ProductId;
                            }
                        }
                    }

                    if (singlePurchaseLimitErrors.Count > 0)
                    {
                        logger.LogInfo("Step6: signle purchase limit doesn't allow multiple orders, system would stop operation");
                        var error = new HttpException((int)HttpStatusCode.Unauthorized, "Single Purchase Limit Error");

                        foreach (var err in singlePurchaseLimitErrors)
                        {
                            error.Data["ProdutId-" + err.Key] = err.Key;
                        }

                        return Request.CreateErrorResponse(HttpStatusCode.Forbidden, error);
                    }

                    logger.LogInfo("step7:Process creating order");
                    var order = await CreateOrder(partial, uow, model, mapper, wsw);
                    logger.LogInfo("step8:Process has created order successfully");
                    #region ## Create response obj

                    //var creationResponse = APIHelper.CreateOrderResponse(order); //var creationResponse = new OrderCreationResponse(order, uow, wsw/*, me*/);

                    var creationResponse = mapper.Map<OrderCreationResponse>(order);

                    #endregion

                    //var response = Request.CreateResponse(HttpStatusCode.Created, creationResponse);
                    //var response = Request.CreateResponse<Order>(HttpStatusCode.Created, order);
                    // this somehow got wiped out by a merge
                    // this is creating 201 codes for every decline.  
                    // need to check the status and display the proper error code
                    HttpResponseMessage response;
                    if (order.Status == OrderStatus.Approved)
                    {
                        response = Request.CreateResponse(HttpStatusCode.Created, creationResponse);
                    }
                    else
                    {
                        response = Request.CreateResponse(HttpStatusCode.PaymentRequired, creationResponse);
                    }

                    //var response = Request.CreateResponse<Order>(HttpStatusCode.Created, order);
                    string uri = Url.Link("DefaultApi", new { id = order.OrderId });
                    response.Headers.Location = new Uri(uri);

                    return response;
                }
                else
                {
                    logger.LogInfo("Model instance is not valid. System generating error");
                    var listErrors = new List<string>();
                    var error = new HttpException((int)HttpStatusCode.BadRequest, "Model Errors");
                    int errIndex = 0;

                    foreach (System.Web.Http.ModelBinding.ModelState modelState in ModelState.Values)
                    {
                        foreach (System.Web.Http.ModelBinding.ModelError err in modelState.Errors)
                        {
                            if (!listErrors.Contains(err.ErrorMessage))
                            {
                                error.Data[errIndex++] = err.ErrorMessage;
                                listErrors.Add(err.ErrorMessage);
                            }
                        }
                    }

                    return Request.CreateResponse(HttpStatusCode.BadRequest, error);
                }
            }
            catch (Exception ex)
            {
                logger.LogInfo("Process failed posting partial order.Possible reason could be:"+ex.Message);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }
          
            ////throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest));
            //throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState));
        }

        protected virtual async Task<Order> CreateOrder(Partial partial, IUnitOfWork uow, PartialToOrderModel model, IMappingEngine mapper, IWebSecurityWrapper wsw)
        {
            return await partial.CreateOrder(uow, model, mapper, wsw);
        }

        private string GetClientIp(HttpRequestMessage request)
        {
            try
            {
                logger.LogInfo("process started getting client IP");
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
                    logger.LogInfo("process failed getting ip address. Returning null");
                    return null;
                }
            }
            catch (Exception ex)
            {
                logger.LogInfo("Process failed  getting client ip. Possible reason could be:"+ex);
                throw;
            }
           
        }

        // DELETE api/partial/id 
        //public HttpResponseMessage Delete(int id)
        //{
        //    var partial = repo.FindPartial(id);
        //    if (partial == null)
        //    {
        //        throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
        //    }
        //    repo.RemovePartial(partial);
        //    return this.Request.CreateResponse(HttpStatusCode.NoContent);
        //}

        // PUT api/partial/id
        //public HttpResponseMessage Put(int id, Partial partial)
        //{
        //    //int index = list.ToList().FindIndex(e => e.Id == id);
        //    //list[index] = employee;
        //    var p = repo.FindPartial(id);
        //    if (p == null)
        //    {
        //        throw new HttpResponseException(this.Request.CreateResponse(HttpStatusCode.NotFound));
        //    }
        //    if (ModelState.IsValid)
        //    {
        //        p.PartialId = id;
        //        repo.EditPartial(partial);
        //    }
        //    throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.BadRequest));
        //}


        //// PATCH api/order/id 
        //public HttpResponseMessage Patch(int id, Delta<Partial> deltapartial)
        //{
        //    var partial = repo.FindPartial(id);
        //    if (partial == null)
        //    {
        //        throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
        //    }
        //    deltapartial.Patch(partial);
        //    repo.SaveChanges();
        //    return Request.CreateResponse(HttpStatusCode.NoContent);
        //}

    }
}
