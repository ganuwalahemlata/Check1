using KontinuityCRM.Helpers;
using KontinuityCRM.Models;
using KontinuityCRM.Models.Enums;
using KontinuityCRM.Models.Fulfillments;
using KontinuityCRM.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace KontinuityCRM.Controllers.API
{
    public class shipfusionController : ApiController
    {
        private readonly IUnitOfWork uow = null;
        private readonly IWebSecurityWrapper wsw = null;
        //private readonly IMessageEncryptor me = null;
        //private readonly IMessageDecryptor md = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uow"></param>
        /// <param name="wsw"></param>
        /// <param name="mapper"></param>       
        public shipfusionController(IUnitOfWork uow, IWebSecurityWrapper wsw)
        {
            this.uow = uow;
            this.wsw = wsw;
        }

        //
        // GET: /shipfusion/
        public async Task<HttpResponseMessage> postback(dynamic shipFusionPostBackResponse)
        {
            var fullfillment = uow.FulfillmentProviderRepository.Get(ful => ful.Alias == FullFillmentProvidersEnum.Shipfusion.ToString()).FirstOrDefault();

            ShipFusion shipFusion = new ShipFusion(uow);
            shipFusion.UserName = fullfillment.UserName;
            shipFusion.Password = fullfillment.Password;

            //ResponseShipmentShipFusion retrieveShippment = await shipFusion.GetShipmentDetails(shipFusionPostBackResponse.shipmentId);
            string shipmentStatus = shipFusionPostBackResponse.shipmentStatus.toString();
            switch (shipmentStatus)
            {
                case "backOrder":
                    UpdateOrderStatus(shipFusionPostBackResponse.shipmentId, OrderStatus.ShippedException);
                    break;
                case "onHold":
                    UpdateOrderStatus(shipFusionPostBackResponse.shipmentId, OrderStatus.ShippedException);
                    break;
                case "cancelled":
                    UpdateOrderStatus(shipFusionPostBackResponse.shipmentId, OrderStatus.ShippedException);
                    break;
                case "shipped":
                    UpdateOrderStatus(shipFusionPostBackResponse.shipmentId, OrderStatus.Shipped);
                    break;
                default:
                    break;
            }

            return Request.CreateResponse(HttpStatusCode.NoContent);

        }
        private void UpdateOrderStatus(string ShipmentId, OrderStatus orderStatus)
        {
            var order = uow.OrderRepository.Get(a => a.OrderProducts.Any(p => p.ShipmentId == ShipmentId)).FirstOrDefault();
            if (orderStatus == OrderStatus.Shipped)
            {
                order.Status = orderStatus;
                order.Shipped = true;
            }
            else
                order.Status = orderStatus;

            uow.OrderRepository.Update(order);
            uow.Save();
        }
    }
}
