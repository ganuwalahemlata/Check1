using KontinuityCRM.Helpers;
using KontinuityCRM.Models.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace KontinuityCRM.Models.Fulfillments
{
    public class ShipFusion : Fulfillment
    {
        private readonly string baseurl = @"https://www.Shipfusion.com/api/v1/";
        private readonly IUnitOfWork uow = null;
        public string UserName { get; set; } // Shipware
        public string Password { get; set; } // Shipware
        [Display(Name = "Receive Tracking #")]
        public bool RecieveTrackingId { get; set; } // Shipware

        public override async Task<bool> SendOrder(Order order, IEnumerable<OrderProduct> orderProducts)
        {
            try
            {
                //UserName = "testoforce";
                //Password = "$2y$10$fBugvRXRSUzptzU7.8.BbuyQyGmBQQfq9ATnn1BcAgDuonT7HQiAu";
                var items = new List<object>();
                foreach (var op in orderProducts)
                {
                    items.Add(new { SKU = op.Product.SKU, qty = op.Quantity });
                }

                object shipmentObj = new
                {
                    orderNumber = order.OrderId,
                    orderDate = DateTime.UtcNow.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture),
                    warehouse = "IL",
                    packingSlipMessage = "",
                    specialInstructions = "",
                    signature = 0,
                    insurance = 0,
                    shippingCarrier = "USPS",
                    shippingService = "First",
                    priorityBump = 0,
                    toAddress = new
                    {
                        firstName = order.Customer.FirstName,
                        lastName = order.Customer.LastName,
                        companyName = "",
                        emailAddress = order.Email,
                        address1 = order.ShippingAddress1,
                        address2 = order.ShippingAddress2,
                        city = order.ShippingCity,
                        state = order.ShippingProvince,
                        zipPostalCode = order.ShippingPostalCode,
                        country = order.ShippingCountry,
                        phoneNumber = order.Phone
                    },
                    items = items
                };

                string _auth = string.Format("{0}:{1}", UserName, Password);
                string _enc = Convert.ToBase64String(Encoding.ASCII.GetBytes(_auth));

                using (var httpClient = new HttpClient { BaseAddress = new Uri(baseurl) })
                {
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json");
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("authorization", string.Format("Basic {0}", _enc));
                    using (var content = new StringContent(new JavaScriptSerializer().Serialize(shipmentObj)))
                    {
                        using (var response = httpClient.PostAsync("shipments", content).Result)
                        {
                            var responseData = await response.Content.ReadAsStringAsync();
                            ResponseShipFusion successObj = JsonConvert.DeserializeObject<ResponseShipFusion>(responseData);
                            SaveShipmentIdInDatabase(successObj.shipmentId, orderProducts);
                            return response.IsSuccessStatusCode;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public ShipFusion()
        { }
        public ShipFusion(IUnitOfWork _uow)
        {
            uow = _uow;
        }
        private void SaveShipmentIdInDatabase(string ShipmentId, IEnumerable<OrderProduct> orderProducts)
        {
            foreach (var prodcuct in orderProducts)
            {
                var item = uow.OrderProductRepository.Get(a => a.OrderId == prodcuct.OrderId && a.ProductId == prodcuct.ProductId).FirstOrDefault();
                item.ShipmentId = ShipmentId;
                uow.OrderProductRepository.Update(item);
            }
            uow.Save();
        }

        public async Task<ResponseShipmentShipFusion> GetShipmentDetails(string ShipmentId)
        {
            string _auth = string.Format("{0}:{1}", UserName, Password);
            string _enc = Convert.ToBase64String(Encoding.ASCII.GetBytes(_auth));
            try
            {
                using (var httpClient = new HttpClient { BaseAddress = new Uri(baseurl) })
                {
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json");
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("authorization", string.Format("Basic {0}", _enc));

                    using (var response = httpClient.GetAsync("shipments/" + ShipmentId).Result)
                    {
                        var responseData = await response.Content.ReadAsStringAsync();
                        ResponseShipmentShipFusion successObj = JsonConvert.DeserializeObject<ResponseShipmentShipFusion>(responseData);
                        return successObj;
                    }
                }
            }
            catch (Exception ex)
            {
                return new ResponseShipmentShipFusion();
            }
        }
    }
}
