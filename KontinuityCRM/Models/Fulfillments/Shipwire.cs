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

namespace KontinuityCRM.Models.Fulfillments
{
    public class Shipwire : Fulfillment
    {
        /// <summary>
        /// Base url for shipwire
        /// </summary>
        private readonly string baseurl = @"https://api.beta.shipwire.com";
        /// <summary>
        /// userName for the Shipwire
        /// </summary>
        public string UserName { get; set; } // Shipware
        /// <summary>
        /// password for the shipwire
        /// </summary>
        public string Password { get; set; } // Shipware
        /// <summary>
        /// Indicates whether to receive tracking Id for shipwire
        /// </summary>
        [Display(Name = "Receive Tracking #")]
        public bool RecieveTrackingId { get; set; } // Shipware
          /// <summary>
          /// Implementation of shipwire
          /// </summary>
          /// <param name="order"></param>
          /// <param name="orderProducts"></param>
          /// <returns></returns>
      
        public override async Task<bool> SendOrder(Order order, IEnumerable<OrderProduct> orderProducts)
        {
            var items = new List<object>();
            foreach (var op in orderProducts)
            {
                items.Add(new { sku = op.Product.SKU, quantity = op.Quantity });
            }

            object obj = new
            {
                orderNo = order.OrderId,
                externalId = "CCRM" + order.OrderId,
                processAfterDate = DateTime.Now./*AddHours(Delay ?? 0).*/ToString("yyyy-MM-ddTHH:mm:sszzz"),
                commerceName = "ContinuityCRM",
                options = new { forceDuplicate = 1 },
                items = items,
                shipFrom = new { company = "ContinuityCRM" },
                shipTo = new
                {
                    email = order.Email,
                    name = order.Customer.FullName,
                    address1 = order.ShippingAddress1,
                    address2 = order.ShippingAddress2,
                    address3 = "",
                    city = order.ShippingCity,
                    state = order.ShippingProvince,
                    postalCode = order.ShippingPostalCode,
                    country = order.ShippingCountry,
                    phone = order.Phone,
                    isCommercial = 0,
                    isPoBox = 0
                }
            };

            var request = WebRequest.Create(baseurl + "/api/v3/orders") as HttpWebRequest;

            request.Method = "POST";
            request.ContentType = "application/json";

            string _auth = string.Format("{0}:{1}", UserName, Password);
            string _enc = Convert.ToBase64String(Encoding.ASCII.GetBytes(_auth));
            request.Headers.Add(HttpRequestHeader.Authorization, string.Format("Basic {0}", _enc));

            using (var streamWriter = new System.IO.StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(JsonConvert.SerializeObject(obj));
            }

            var response = await request.GetResponseAsync();
            using (var streamReader = new System.IO.StreamReader(response.GetResponseStream()))
            {
                var responseFromServer = await streamReader.ReadToEndAsync();

                dynamic jsonObj = JsonConvert.DeserializeObject(responseFromServer);
                
                var sucsess = jsonObj.GetType().GetProperty("errors") == null;
                //return !((IDictionary<string, object>)jsonObj).ContainsKey("errors");

                foreach (var op in orderProducts)
                {
                    op.FulfillmentProviderResponse = responseFromServer;
                    op.Shipped = sucsess;
                    op.FulfillmentDate = null;
                }

                return sucsess;

            }          

            
        }

        


    }
}

/*
{
    "orderNo": "foobar1",
    "externalId": "rFooBar1",
    "processAfterDate": "2014-06-10T16:30:00-07:00",
    "commerceName": "Foo Commerce",
    # List of items ordered
    "items": [
        {
            # Item's SKU
            "sku": "Laura-s_Pen",
            # Number of items to order
            "quantity": 4,
            # Amount to show in invoice (for customs declaration purposes)
            "commercialInvoiceValue": 4.5,
            # Currency for the above value
            "commercialInvoiceValueCurrency": "USD"
        },
        {
            "sku": "TwinPianos",
            "quantity": 4,
            "commercialInvoiceValue": 6.5,
            "commercialInvoiceValueCurrency": "USD"
        }
    ],
    "options": {
        # Specify one of warehouseId, warehouseExternalId, warehouseRegion, warehouseArea
        "warehouseId": null,
        "warehouseExternalId": null,
        "warehouseRegion": "LAX",
        "warehouseArea": null,
        # Service requested for this order
        "serviceLevelCode": "1D",
        # Delivery carrier requested for this order
        "carrierCode": null,
        # Was "Same Day" processing requested?
        "sameDay": "NOT REQUESTED",
        # Used to assign a pre-defined set of shipping and/or customization preferences on an order.
        # A channel must be defined prior to order creation for the desired preferences to be applied.
        # Please contact us if you believe your application requires a channel.
        "channelName": "My Channel",
        "forceDuplicate": 0,
        "forceAddress": 0,
        "testOrder": 0
        "referrer": "Foo Referrer",
        "affiliate": null,
        "currency": "USD",
        # Specifies whether the items to be shipped can be split into two packages if needed
        "canSplit": 1,
        # Set a manual hold
        "hold": 1,
        # A discount code
        "discountCode": "FREE STUFF",
        "server": "Production"
        # Process this request asynchronously. Not yet supported (coming soon)
        "forceAsync": 0,
    },
    # Shipping source
    "shipFrom": {"company": "We Sell'em Co."},
    "shipTo": {
        # Recipient details
        "email": "audrey.horne@greatnothern.com",
        "name": "Audrey Horne",
        "company": "Audrey's Bikes",
        "address1": "6501 Railroad Avenue SE",
        "address2": "Room 315",
        "address3": "",
        "city": "Snoqualmie",
        "state": "WA",
        "postalCode": "98065",
        "country": "US",
        "phone": "4258882556",
        # Specifies whether the recipient is a commercial entity. 0 = no, 1 = yes
        "isCommercial": 0,
        # Specifies whether the recipient is a PO box. 0 = no, 1 = yes
        "isPoBox": 0
    },
    # Invoiced amounts (for customs declaration only)
    "commercialInvoice": {
        # Amount for shipping service
        "shippingValue": 4.85,
        # Amount for insurance
        "insuranceValue": 6.57,
        "additionalValue": 8.29,
        # Currencies to interpret the amounts above
        "shippingValueCurrency": "USD",
        "insuranceValueCurrency": "USD",
        "additionalValueCurrency": "USD"
    },
    # Message to include in package
    "packingList": {
        "message1": {
            "body": "This must be where pies go when they die. Enjoy!",
            "header": "Enjoy this product!"
        }
    }
}

*/