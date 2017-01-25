using KontinuityCRM.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;

namespace KontinuityCRM.Models
{   
    [TrackChanges]
    public class Gateway
    {
        /// <summary>
        /// Indicates Gateway Id as primary key
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Indicates Gateway name
        /// </summary>
        [Display(Name="Name")]
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// Indicates the createdDate for Gateway
        /// </summary>
        [Display(Name = "Created")]
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// Indicates the UserName for Gateway
        /// </summary>
        [Required]
        public string Username { get; set; }
        /// <summary>
        /// Indicates the Password for Gateway
        /// </summary>
        [Required]
        public string Password { get; set; }
        /// <summary>
        /// Indicates the Type of Gateway
        /// </summary>
        [Required]
        [MaxLength(128)]
        public string Type { get; set; }

       public int Status { get; set; }
    }

    //[DisplayName("Network Mechant Inc")]
    //public class NMI : Gateway
    //{
    //    public readonly string url = "https://secure.nmi.com/api/transact.php";

    //    public override Transaction Sale(Order order, Processor processor)
    //    {
    //        var postdata = fillRequest(order);

    //        postdata.Add("type", "sale");

    //        var response = GatewayHelper.GetResponse(url, postdata);

    //        var transaction = CreateTransaction(order, processor, response);
    //        transaction.Type = TransactionType.Sale;

    //        return transaction;
    //    }

    //    public override Transaction Authorize(Order order, Processor processor)
    //    {
    //        var postdata = fillRequest(order);

    //        postdata.Add("type", "auth");

    //        var response = GatewayHelper.GetResponse(url, postdata);

    //        var transaction = CreateTransaction(order, processor, response);
    //        transaction.Type = TransactionType.Auth;


    //        return transaction;
    //    }

    //    public override Transaction Void(Transaction transaction)
    //    {
    //        var postdata = new NameValueCollection();

    //        postdata.Add("type", "void");
    //        postdata.Add("transactionid", transaction.TransactionId);
    //        postdata.Add("username", Username);
    //        postdata.Add("password", Password);

    //        var response = GatewayHelper.GetResponse(url, postdata);

    //        var ctransaction = CreateTransaction(transaction, response);
    //        ctransaction.Type = TransactionType.Void;

    //        return ctransaction;

    //    }

    //    public override Transaction Refund(Transaction transaction, decimal amount = 0)
    //    {
    //        var postdata = new NameValueCollection();

    //        postdata.Add("type", "refund");
    //        postdata.Add("transactionid", transaction.TransactionId);
    //        postdata.Add("username", Username);
    //        postdata.Add("password", Password);
    //        postdata.Add("amount", amount.ToString());

    //        var response = GatewayHelper.GetResponse(url, postdata);

    //        var ctransaction = CreateTransaction(transaction, response);
    //        ctransaction.Type = TransactionType.Refund;

    //        return ctransaction;


    //    }

    //    public override Transaction Capture(Transaction transaction)
    //    {
    //        var postdata = new NameValueCollection();

    //        postdata.Add("type", "capture");
    //        postdata.Add("transactionid", transaction.TransactionId);
    //        postdata.Add("username", Username);
    //        postdata.Add("password", Password);
    //        postdata.Add("amount", transaction.Amount.ToString());

    //        var response = GatewayHelper.GetResponse(url, postdata);

    //        var ctransaction = CreateTransaction(transaction, response);
    //        ctransaction.Type = TransactionType.Capture;

    //        return ctransaction;


    //    }

    //    public override Gateways.GatewayModel Model(Processor processor = null)
    //    {
    //        var gatewayProvider = new KontinuityCRM.Models.Gateways.NMI();

    //        gatewayProvider.GatewayId = this.Id;

    //        if (processor != null)
    //        {
    //            //GatewayAlias = Alias,
    //            gatewayProvider.CreatedDate = processor.CreatedDate;
    //            gatewayProvider.Id = processor.Id;

    //            ///* nmi fields */
    //            gatewayProvider.ShipmentOnCapture = processor.ShipmentOnCapture;
    //            gatewayProvider.CaptureDelayHours = processor.CaptureDelayHours;
    //            gatewayProvider.CaptureOnShipment = processor.CaptureOnShipment;
    //            gatewayProvider.Currency = processor.Currency;
    //            //Password = processor.Password;
    //            //Username = Username;
    //            gatewayProvider.UseDeclineSalvage = processor.UseDeclineSalvage;
    //            gatewayProvider.PostDescriptor = processor.PostDescriptor;
    //            gatewayProvider.PostProcessorId = processor.PostProcessorId;
    //            gatewayProvider.PostProductDescription = processor.PostProductDescription;
    //            gatewayProvider.UsePreAuthorizationFilter = processor.UsePreAuthorizationFilter;

    //            ///* right fields */
    //            gatewayProvider.Name = processor.Name;
    //            gatewayProvider.Type = processor.Type;
    //            gatewayProvider.Descriptor = processor.Descriptor;
    //            gatewayProvider.CustomerServiceNumber = processor.CustomerServiceNumber;
    //            gatewayProvider.GlobalMonthlyCap = processor.GlobalMonthlyCap;
    //            gatewayProvider.TransactionFee = processor.TransactionFee;
    //            gatewayProvider.ChargebackFee = processor.ChargebackFee;
    //            gatewayProvider.ProcessingPercent = processor.ProcessingPercent;
    //            gatewayProvider.ReversePercent = processor.ReversePercent;

    //            // deserialize mdf fields and set them to the model

    //            if (processor.Parameters != null)
    //            {
    //                var dictionary = (Dictionary<string, string>)KontinuityCRMHelper.ByteArrayToObject(processor.Parameters);

    //                foreach (var key in dictionary.Keys)
    //                {
    //                    //obj.GetType().InvokeMember("Name",
    //                    //BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty,
    //                    //Type.DefaultBinder, obj, "MyName");

    //                    var propertyInfo = gatewayProvider.GetType().GetProperty(key, BindingFlags.Public | BindingFlags.Instance);
    //                    propertyInfo.SetValue(gatewayProvider, dictionary[key]);

    //                }
    //            }

    //        }



    //        // deserialize the dictionay and fill the MDF fields
    //        // var dictionary = 

    //        return gatewayProvider;
    //    }

    //    #region ## Private ##

    //    private NameValueCollection fillRequest(Order order)
    //    {
    //        var postdata = new NameValueCollection();
    //        postdata.Add("username", Username);
    //        postdata.Add("password", Password);

    //        // credit card data
    //        postdata.Add("ccnumber", order.CreditCardNumber);
    //        postdata.Add("ccexp", string.Format("{0}{1}", ((int)order.CreditCardExpirationMonth).ToString("D2"), order.CreditCardExpirationYear));
    //        postdata.Add("cvv", order.CreditCardCVV.ToString());

    //        // other data
    //        postdata.Add("orderid", order.OrderId.ToString());
    //        postdata.Add("ipaddress", order.IPAddress);


    //        // shipping data
    //        postdata.Add("shipping_firstname", order.ShippingFirstName);
    //        postdata.Add("shipping_lastname", order.ShippingLastName);
    //        postdata.Add("shipping_address1", order.ShippingAddress1);
    //        postdata.Add("shipping_address2", order.ShippingAddress2);
    //        postdata.Add("shipping_city", order.ShippingCity);
    //        postdata.Add("shipping_state", order.ShippingProvince);
    //        postdata.Add("shipping_zip", order.ShippingPostalCode);
    //        postdata.Add("shipping_country", order.ShippingCountry);

    //        // billing info
    //        postdata.Add("firstname", order.BillingFirstName);
    //        postdata.Add("lastname", order.BillingLastName);
    //        postdata.Add("address1", order.BillingAddress1);
    //        postdata.Add("address2", order.BillingAddress2);
    //        postdata.Add("city", order.BillingCity);
    //        postdata.Add("state", order.BillingProvince);
    //        postdata.Add("zip", order.BillingPostalCode);
    //        postdata.Add("country", order.BillingCountry);

    //        postdata.Add("phone", order.Phone);
    //        postdata.Add("email", order.Email);

    //        //this.postdata["amount"] = amount.ToString();

    //        postdata.Add("amount", order.Total.ToString());

    //        return postdata;
    //    }

    //    private Transaction CreateTransaction(Order order, Processor processor, string response)
    //    {
    //        var nvc = HttpUtility.ParseQueryString(response);

    //        var currentTransaction = new Transaction()
    //        {
    //            Response = response,
    //            //IsRebill = order.IsRebill,
    //            OrderId = order.OrderId,
    //            Order = order,
    //            ProcessorId = processor.Id,
    //            Processor = processor,
    //            GatewayId = this.Id,
    //            TransactionId = nvc["transactionid"],
    //            //Status = nvc[0].Equals("1") ? TransactionStatus.Approved : nvc[0].Equals("2") ? TransactionStatus.Declined : TransactionStatus.Error,
    //            Status = nvc[0].Equals("1") ? TransactionStatus.Approved : TransactionStatus.Declined,
    //            Success = nvc[0].Equals("1"),
    //            //Type = TransactionType.Refund,
    //            Message = nvc["responsetext"],
    //            Date = DateTime.UtcNow,
    //            Amount = order.Total,
    //        };

    //        return currentTransaction;
    //    }

    //    private Transaction CreateTransaction(Transaction transaction, string response)
    //    {
    //        var rtransaction = CreateTransaction(transaction.Order, transaction.Processor, response);
    //        rtransaction.BalancerId = transaction.BalancerId;
    //        return rtransaction;
    //    }


    //    #endregion
    //}



    //[DisplayName("Argus Payment")]
    //public class Argus : Gateway
    //{
    //    public readonly string url = "https://svc.arguspayments.com/payment/pmt_service.cfm";

    //    public override Transaction Sale(Order order, Processor processor)
    //    {
    //        var postdata = fillRequest(order, processor, order.Total);

    //        // Payment and Bank Information Parameters
    //        // credit card data
    //        postdata.Add("pmt_numb", order.CreditCardNumber);
    //        postdata.Add("pmt_expiry", string.Format("{0}{1}", ((int)order.CreditCardExpirationMonth).ToString("D2"), order.CreditCardExpirationYear));
    //        postdata.Add("pmt_key", order.CreditCardCVV.ToString());
    //        //var currency = order.OrderProducts.First().Product.Currency.ToString();
    //        postdata.Add("request_currency", processor.Currency);

    //        // other data
    //        //postdata.Add("orderid", order.OrderId.ToString());
    //        //postdata.Add("ipaddress", order.IPAddress);

    //        //Customer Parameters
    //        postdata.Add("cust_fname", order.Customer.FirstName); // Cardholder’s First Name
    //        postdata.Add("cust_lname", order.Customer.LastName); // Cardholder’s Last Name
    //        postdata.Add("cust_email", order.Customer.Email); // Cardholder’s Email Address
    //        postdata.Add("cust_phone", order.Customer.Phone);
    //        // shipping data
    //        //postdata.Add("shipping_firstname", order.ShippingFirstName);
    //        //postdata.Add("shipping_lastname", order.ShippingLastName);
    //        postdata.Add("ship_addr", order.ShippingAddress1);
    //        //postdata.Add("shipping_address2", order.ShippingAddress2);
    //        postdata.Add("ship_addr_city", order.ShippingCity);
    //        postdata.Add("ship_addr_state", order.ShippingProvince);
    //        postdata.Add("ship_addr_zip", order.ShippingPostalCode);
    //        postdata.Add("ship_addr_country", order.ShippingCountry);
    //        // billing info
    //        //postdata.Add("firstname", order.BillingFirstName);
    //        //postdata.Add("lastname", order.BillingLastName);
    //        postdata.Add("bill_addr", order.BillingAddress1);
    //        //postdata.Add("address2", order.BillingAddress2);
    //        postdata.Add("bill_addr_city", order.BillingCity);
    //        postdata.Add("bill_addr_state", order.BillingProvince); // 2-letter State orTerritory Code
    //        postdata.Add("bill_addr_zip", order.BillingPostalCode);
    //        postdata.Add("bill_addr_country", order.BillingCountry);


    //        //postdata.Add("request_currency", "USD"); // 3-letter Currency Code
    //        //postdata.Add("bill_addr_state", order.BillingProvince); // 2-letter State orTerritory Code
    //        //postdata.Add("bill_addr_zip", order.BillingPostalCode);
    //        //postdata.Add("bill_addr_country", order.BillingCountry);

    //        //postdata.Add("phone", order.Phone);
    //        //postdata.Add("email", order.Email);



    //        //postdata.Add("amount", order.Amount.ToString());

    //        postdata.Add("request_action", "CCAUTHCAP");

    //        var response = GatewayHelper.GetResponse(url, postdata);

    //        //Dictionary<string, string> dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(response);

    //        //var transaction = new Transaction()
    //        //{
    //        //    Response = response,
    //        //    IsRebill = order.IsRebill,
    //        //    OrderId = order.OrderId, 
    //        //    //Order = order,
    //        //    ProcessorId = processor.Id,
    //        //    Type = TransactionType.Sale,
    //        //    TransactionId = dic["PO_ID"], // 
    //        //    GatewayId = this.Id,
    //        //    Status = dic["TRANS_STATUS_NAME"] == "APPROVED" ? TransactionStatus.Approved : TransactionStatus.Declined,
    //        //};

    //        var transaction = CreateTransaction(order, processor, response);
    //        transaction.Type = TransactionType.Sale;

    //        return transaction;

    //        //return response;
    //    }

    //    public override Transaction Authorize(Order order, Processor processor)
    //    {
    //        var postdata = fillRequest(order, processor, order.Total);

    //        // Payment and Bank Information Parameters
    //        // credit card data
    //        postdata.Add("pmt_numb", order.CreditCardNumber);
    //        postdata.Add("pmt_expiry", string.Format("{0}{1}", ((int)order.CreditCardExpirationMonth).ToString("D2"), order.CreditCardExpirationYear));
    //        postdata.Add("pmt_key", order.CreditCardCVV.ToString());
    //        //var currency = order.OrderProducts.First().Product.Currency.ToString();
    //        postdata.Add("request_currency", processor.Currency);

    //        // other data
    //        //postdata.Add("orderid", order.OrderId.ToString());
    //        //postdata.Add("ipaddress", order.IPAddress);

    //        //Customer Parameters
    //        postdata.Add("cust_fname", order.Customer.FirstName); // Cardholder’s First Name
    //        postdata.Add("cust_lname", order.Customer.LastName); // Cardholder’s Last Name
    //        postdata.Add("cust_email", order.Customer.Email); // Cardholder’s Email Address
    //        postdata.Add("cust_phone", order.Customer.Phone);
    //        // shipping data
    //        //postdata.Add("shipping_firstname", order.ShippingFirstName);
    //        //postdata.Add("shipping_lastname", order.ShippingLastName);
    //        postdata.Add("ship_addr", order.ShippingAddress1);
    //        //postdata.Add("shipping_address2", order.ShippingAddress2);
    //        postdata.Add("ship_addr_city", order.ShippingCity);
    //        postdata.Add("ship_addr_state", order.ShippingProvince);
    //        postdata.Add("ship_addr_zip", order.ShippingPostalCode);
    //        postdata.Add("ship_addr_country", order.ShippingCountry);
    //        // billing info
    //        //postdata.Add("firstname", order.BillingFirstName);
    //        //postdata.Add("lastname", order.BillingLastName);
    //        postdata.Add("bill_addr", order.BillingAddress1);
    //        //postdata.Add("address2", order.BillingAddress2);
    //        postdata.Add("bill_addr_city", order.BillingCity);
    //        postdata.Add("bill_addr_state", order.BillingProvince); // 2-letter State orTerritory Code
    //        postdata.Add("bill_addr_zip", order.BillingPostalCode);
    //        postdata.Add("bill_addr_country", order.BillingCountry);


    //        //postdata.Add("request_currency", "USD"); // 3-letter Currency Code
    //        //postdata.Add("bill_addr_state", order.BillingProvince); // 2-letter State orTerritory Code
    //        //postdata.Add("bill_addr_zip", order.BillingPostalCode);
    //        //postdata.Add("bill_addr_country", order.BillingCountry);

    //        //postdata.Add("phone", order.Phone);
    //        //postdata.Add("email", order.Email);



    //        //postdata.Add("amount", order.Amount.ToString());

    //        postdata.Add("request_action", "CCAUTHORIZE");

    //        var response = GatewayHelper.GetResponse(url, postdata);

    //        //dynamic dynjson = JsonConvert.DeserializeObject(response);

    //        Dictionary<string, string> dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(response);

    //        // To do a Reversal
    //        // request, send the
    //        // Order ID (PO_ID) of
    //        // the original
    //        // authorization in the
    //        // REQUEST_REF_PO_ID
    //        // parameter.

    //        //var transaction = new Transaction()
    //        //{
    //        //    Response = response,
    //        //    IsRebill = order.IsRebill,
    //        //    OrderId = order.OrderId,
    //        //    //Order = order,
    //        //    ProcessorId = processor.Id,
    //        //    Type = TransactionType.Auth,
    //        //    TransactionId = dic["PO_ID"], // 
    //        //    GatewayId = this.Id,
    //        //    Status = dic["TRANS_STATUS_NAME"] == "APPROVED" ? TransactionStatus.Approved : TransactionStatus.Declined,
    //        //};

    //        var transaction = CreateTransaction(order, processor, response);
    //        transaction.Type = TransactionType.Auth;

    //        return transaction;

    //        //return response;
    //    }

    //    public override Transaction Void(Transaction transaction)
    //    {
    //        var postdata = fillRequest(transaction.Order, transaction.Processor, transaction.Amount);

    //        postdata.Add("request_action", "CCREVERSE");

    //        // To do a Reversal
    //        // request, send the
    //        // Order ID (PO_ID) of
    //        // the original
    //        // authorization in the
    //        // REQUEST_REF_PO_ID
    //        // parameter.

    //        postdata.Add("REQUEST_REF_PO_ID", transaction.TransactionId);

    //        var response = GatewayHelper.GetResponse(url, postdata);

    //        //Dictionary<string, string> dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(response);

    //        //var currenttransaction = new Transaction()
    //        //{
    //        //    Response = response,
    //        //    IsRebill = transaction.IsRebill,
    //        //    OrderId = transaction.OrderId,
    //        //    //Order = order,
    //        //    ProcessorId = transaction.ProcessorId,
    //        //    Type = TransactionType.Void,
    //        //    TransactionId = dic["PO_ID"], // 
    //        //    GatewayId = this.Id,
    //        //    Status = dic["TRANS_STATUS_NAME"] == "APPROVED" ? TransactionStatus.Approved : TransactionStatus.Declined,
    //        //};

    //        var ctransaction = CreateTransaction(transaction, response);
    //        ctransaction.Type = TransactionType.Void;

    //        return ctransaction;
    //    }

    //    public override Transaction Refund(Transaction transaction, decimal amount)
    //    {
    //        var postdata = fillRequest(transaction.Order, transaction.Processor, amount);

    //        postdata.Add("request_action", "CCREVERSECAP");

    //        // To do a Reversal
    //        // request, send the
    //        // Order ID (PO_ID) of
    //        // the original
    //        // authorization in the
    //        // REQUEST_REF_PO_ID
    //        // parameter.

    //        postdata.Add("REQUEST_REF_PO_ID", transaction.TransactionId);

    //        var response = GatewayHelper.GetResponse(url, postdata);

    //        var ctransaction = CreateTransaction(transaction, response);
    //        ctransaction.Type = TransactionType.Refund;
    //        ctransaction.Amount = amount;
    //        return ctransaction;

    //        //Dictionary<string, string> dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(response);

    //        //var currenttransaction = new Transaction()
    //        //{
    //        //    Response = response,
    //        //    IsRebill = transaction.IsRebill,
    //        //    OrderId = transaction.OrderId,
    //        //    //Order = order,
    //        //    ProcessorId = transaction.ProcessorId,
    //        //    Type = TransactionType.Refund,
    //        //    TransactionId = dic["PO_ID"], // 
    //        //    GatewayId = this.Id,
    //        //    Status = dic["TRANS_STATUS_NAME"] == "APPROVED" ? TransactionStatus.Approved : TransactionStatus.Declined,
    //        //};

    //        //return currenttransaction;
    //    }

    //    public override Transaction Capture(Transaction transaction) // this is the auth transaction
    //    {
    //        var postdata = fillRequest(transaction.Order, transaction.Processor, transaction.Amount);

    //        postdata.Add("request_action", "CCCAPTURE");

    //        postdata.Add("REQUEST_REF_PO_ID", transaction.TransactionId);

    //        var response = GatewayHelper.GetResponse(url, postdata);

    //        var ctransaction = CreateTransaction(transaction, response);
    //        ctransaction.Type = TransactionType.Capture;
    //        //ctransaction.Amount = amount; // the transaction amount will be the same as the order amount so 
    //        return ctransaction;
    //    }

    //    public override Gateways.GatewayModel Model(Processor processor = null)
    //    {
    //        var gatewayProvider = new KontinuityCRM.Models.Gateways.Argus();

    //        gatewayProvider.GatewayId = this.Id;

    //        if (processor != null)
    //        {
    //            //GatewayAlias = Alias,
    //            gatewayProvider.CreatedDate = processor.CreatedDate;
    //            gatewayProvider.Id = processor.Id;

    //            ///* nmi fields */
    //            gatewayProvider.MerchantAccountId = processor.MerchantAccountId;
    //            gatewayProvider.DynamicProductId = processor.DynamicProductId;
    //            gatewayProvider.SiteId = processor.SiteId;
    //            gatewayProvider.Currency = processor.Currency;
    //            //Password = Password;
    //            //Username = Username;
    //            gatewayProvider.UseDeclineSalvage = processor.UseDeclineSalvage;
    //            gatewayProvider.Auth = processor.UsePreAuthorizationFilter;

    //            ///* right fields */
    //            gatewayProvider.Name = processor.Name;
    //            gatewayProvider.Type = processor.Type;
    //            gatewayProvider.Descriptor = processor.Descriptor;
    //            gatewayProvider.CustomerServiceNumber = processor.CustomerServiceNumber;
    //            gatewayProvider.GlobalMonthlyCap = processor.GlobalMonthlyCap;
    //            gatewayProvider.TransactionFee = processor.TransactionFee;
    //            gatewayProvider.ChargebackFee = processor.ChargebackFee;
    //            gatewayProvider.ProcessingPercent = processor.ProcessingPercent;
    //            gatewayProvider.ReversePercent = processor.ReversePercent;

    //            gatewayProvider.CaptureDelayHours = processor.CaptureDelayHours;
    //            gatewayProvider.CaptureOnShipment = processor.CaptureOnShipment;



    //        }

    //        return gatewayProvider;
    //    }

    //    #region ## Private ##

    //    private Transaction CreateTransaction(Transaction transaction, string response)
    //    {
    //        var rtransaction = CreateTransaction(transaction.Order, transaction.Processor, response);
    //        rtransaction.BalancerId = transaction.BalancerId;
    //        return rtransaction;
    //    }

    //    private Transaction CreateTransaction(Order order, Processor processor, string response)
    //    {
    //        Dictionary<string, string> dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(response);

    //        var transaction = new Transaction()
    //        {
    //            Response = response,
    //            //IsRebill = order.IsRebill,
    //            OrderId = order.OrderId,
    //            Order = order,
    //            ProcessorId = processor.Id,
    //            Processor = processor,
    //            //TransactionId = dic.ContainsKey("PO_ID") ? dic["PO_ID"] : null, // 
    //            GatewayId = this.Id,
    //            Date = DateTime.UtcNow,
    //            Amount = order.Total,
    //        };
    //        string value;
    //        dic.TryGetValue("PO_ID", out value);
    //        transaction.TransactionId = value;

    //        dic.TryGetValue("TRANS_STATUS_NAME", out value);
    //        transaction.Status = value == "APPROVED" ? TransactionStatus.Approved : TransactionStatus.Declined;
    //        transaction.Success = value == "APPROVED";

    //        if (transaction.Status == TransactionStatus.Declined)
    //        {
    //            dic.TryGetValue("SERVICE_ADVICE", out value);

    //            if (string.IsNullOrWhiteSpace(value))
    //            {
    //                dic.TryGetValue("API_ADVICE", out value);
    //            }

    //            if (string.IsNullOrWhiteSpace(value))
    //            {
    //                dic.TryGetValue("PROCESSOR_ADVICE", out value);
    //            }

    //            if (string.IsNullOrWhiteSpace(value))
    //            {
    //                dic.TryGetValue("INDUSTRY_ADVICE", out value);
    //            }

    //            transaction.Message = value ?? "Reason might be found in the transaction response";

    //            //try
    //            //{
    //            //    transaction.Status = dic["TRANS_STATUS_NAME"] == "APPROVED" ? TransactionStatus.Approved : TransactionStatus.Declined;
    //            //    transaction.Message = dic["SERVICE_ADVICE"];
    //            //}
    //            //catch
    //            //{
    //            //    transaction.Status = TransactionStatus.Declined;
    //            //    transaction.Message = "Reason might be found in the transaction response";
    //            //}
    //        }

    //        return transaction;
    //    }

    //    private NameValueCollection fillRequest(Order order, Processor processor, decimal amount)
    //    {
    //        // ARGUS
    //        var postdata = new NameValueCollection();

    //        // Authentication Parameters
    //        postdata.Add("req_username", Username); // Service Request Username
    //        postdata.Add("req_password", Password); // Service Request Password            
    //        postdata.Add("request_response_format", "JSON"); // “XML”, “PIPES” and ”JSON” Optional (Default: XML)
    //        postdata.Add("request_api_version", "3.5");
    //        postdata.Add("site_id", processor.SiteId); // Merchant’s Website ID


    //        // Line Items Parameters
    //        // since an order might have multiple product we are passing the first productid only
    //        //postdata.Add("li_prod_id_1", order.OrderProducts.First().ProductId.ToString()); // Line Item Product ID 1
    //        postdata.Add("li_prod_id_1", processor.DynamicProductId);
    //        postdata.Add("li_value_1", amount.ToString()); // Line Item Transaction Amount 1
    //        postdata.Add("xtl_order_id", order.OrderId.ToString()); // Merchant’s Order ID Numeric (max length:24) Optional
    //        postdata.Add("li_count_1", order.Quantity.ToString()); // Line Item Count Max value is “99”.

    //        if (!string.IsNullOrEmpty(processor.MerchantAccountId))
    //        {
    //            postdata.Add("merch_acct_id", processor.MerchantAccountId); // Numeric If null, the system will follow merchant’s bank load balancer settings.
    //        }



    //        return postdata;
    //    }

    //    #endregion
    //}

}