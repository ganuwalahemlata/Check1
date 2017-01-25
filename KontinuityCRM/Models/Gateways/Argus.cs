using AutoMapper;
using KontinuityCRM.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KontinuityCRM.Models.Gateways
{
    [DisplayName("Argus Payment")]
    public class Argus : GatewayModel
    {
        /// <summary>
        /// SiteId of Argus
        /// </summary>
        [Required]
        [Display(Name = "Site Id")]
        public string SiteId { get; set; }
        /// <summary>
        /// Indicates Merchant Account id for Argus 
        /// </summary>
        [Display(Name = "Merchant Account Id")]
        public string MerchantAccountId { get; set; }
        /// <summary>
        /// Indicates Dynamic Product Id for Argus
        /// </summary>
        [Required]
        [Display(Name = "Dynamic Product Id")]
        public string DynamicProductId { get; set; }
        /// <summary>
        /// Indicates UserName 
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// Password for Argus
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Currency 
        /// </summary>
        public string Currency { get; set; }

        //[Display(Name = "Use Decline Salvage")]
        //public bool UseDeclineSalvage { get; set; }
        /// <summary>
        /// Auth enabled or disabled
        /// </summary>
        [Display(Name = "3D Auth")]
        public bool Auth { get; set; }

        /// <summary>
        /// url
        /// </summary>
        public readonly string url = "https://svc.arguspayments.com/payment/pmt_service.cfm";
        /// <summary>
        /// Sale action
        /// </summary>
        /// <param name="order"></param>
        /// <param name="processor"></param>
        /// <returns></returns>
        public override Transaction Sale(Order order, Processor processor)
        {
            var postdata = fillRequest(order, processor, order.Total);

            // Payment and Bank Information Parameters
            // credit card data
            postdata.Add("pmt_numb", order.CreditCardNumber);
            postdata.Add("pmt_expiry", string.Format("{0}{1}", ((int)order.CreditCardExpirationMonth).ToString("D2"), order.CreditCardExpirationYear));
            postdata.Add("pmt_key", order.CreditCardCVV.ToString());
            //var currency = order.OrderProducts.First().Product.Currency.ToString();
            postdata.Add("request_currency", processor.Currency);

            // other data
            //postdata.Add("orderid", order.OrderId.ToString());
            //postdata.Add("ipaddress", order.IPAddress);

            //Customer Parameters
            postdata.Add("cust_fname", order.Customer.FirstName); // Cardholder’s First Name
            postdata.Add("cust_lname", order.Customer.LastName); // Cardholder’s Last Name
            postdata.Add("cust_email", order.Customer.Email); // Cardholder’s Email Address
            postdata.Add("cust_phone", order.Customer.Phone);
            // shipping data
            //postdata.Add("shipping_firstname", order.ShippingFirstName);
            //postdata.Add("shipping_lastname", order.ShippingLastName);
            postdata.Add("ship_addr", order.ShippingAddress1);
            //postdata.Add("shipping_address2", order.ShippingAddress2);
            postdata.Add("ship_addr_city", order.ShippingCity);
            postdata.Add("ship_addr_state", order.ShippingProvince);
            postdata.Add("ship_addr_zip", order.ShippingPostalCode);
            postdata.Add("ship_addr_country", order.ShippingCountry);
            // billing info
            //postdata.Add("firstname", order.BillingFirstName);
            //postdata.Add("lastname", order.BillingLastName);
            postdata.Add("bill_addr", order.BillingAddress1);
            //postdata.Add("address2", order.BillingAddress2);
            postdata.Add("bill_addr_city", order.BillingCity);
            postdata.Add("bill_addr_state", order.BillingProvince); // 2-letter State orTerritory Code
            postdata.Add("bill_addr_zip", order.BillingPostalCode);
            postdata.Add("bill_addr_country", order.BillingCountry);


            //postdata.Add("request_currency", "USD"); // 3-letter Currency Code
            //postdata.Add("bill_addr_state", order.BillingProvince); // 2-letter State orTerritory Code
            //postdata.Add("bill_addr_zip", order.BillingPostalCode);
            //postdata.Add("bill_addr_country", order.BillingCountry);

            //postdata.Add("phone", order.Phone);
            //postdata.Add("email", order.Email);



            //postdata.Add("amount", order.Amount.ToString());

            postdata.Add("request_action", "CCAUTHCAP");

            var response = GatewayHelper.GetResponse(url, postdata);

            //Dictionary<string, string> dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(response);

            //var transaction = new Transaction()
            //{
            //    Response = response,
            //    IsRebill = order.IsRebill,
            //    OrderId = order.OrderId, 
            //    //Order = order,
            //    ProcessorId = processor.Id,
            //    Type = TransactionType.Sale,
            //    TransactionId = dic["PO_ID"], // 
            //    GatewayId = this.Id,
            //    Status = dic["TRANS_STATUS_NAME"] == "APPROVED" ? TransactionStatus.Approved : TransactionStatus.Declined,
            //};

            var transaction = CreateTransaction(order, processor, response);
            transaction.Type = TransactionType.Sale;
            transaction.Request = GatewayHelper.GetPostData(postdata);
            return transaction;

            //return response;
        }

        public override TransactionViaPrepaidCardQueue SalePrepaidCard(PrepaidCard prepaidCard, Processor processor, decimal amount)
        {
            var postdata = fillRequestPrepaidCard(processor, amount);

            // Payment and Bank Information Parameters
            // credit card data
            postdata.Add("pmt_numb", prepaidCard.Number);
            postdata.Add("pmt_expiry", string.Format("{0}{1}", (Convert.ToInt32(prepaidCard.CreditCardExpirationMonth)).ToString("D2"), prepaidCard.CreditCardExpirationYear));
            postdata.Add("pmt_key", prepaidCard.CreditCardCVV.ToString());
            //var currency = order.OrderProducts.First().Product.Currency.ToString();
            postdata.Add("request_currency", processor.Currency);

            // other data
            //postdata.Add("orderid", order.OrderId.ToString());
            //postdata.Add("ipaddress", order.IPAddress);

            //Customer Parameters
            postdata.Add("cust_fname", prepaidCard.First_Name); // Cardholder’s First Name
            postdata.Add("cust_lname", prepaidCard.Last_Name); // Cardholder’s Last Name
            postdata.Add("cust_email", prepaidCard.Email); // Cardholder’s Email Address
            postdata.Add("cust_phone", prepaidCard.Phone);
            // shipping data
            //postdata.Add("shipping_firstname", order.ShippingFirstName);
            //postdata.Add("shipping_lastname", order.ShippingLastName);
            postdata.Add("ship_addr", prepaidCard.Address);
            //postdata.Add("shipping_address2", order.ShippingAddress2);
            postdata.Add("ship_addr_city", prepaidCard.City);
            postdata.Add("ship_addr_state", prepaidCard.State);
            postdata.Add("ship_addr_zip", prepaidCard.Zip);
            postdata.Add("ship_addr_country", prepaidCard.Country);
            // billing info
            //postdata.Add("firstname", order.BillingFirstName);
            //postdata.Add("lastname", order.BillingLastName);
            postdata.Add("bill_addr", prepaidCard.Address);
            //postdata.Add("address2", order.BillingAddress2);
            postdata.Add("bill_addr_city", prepaidCard.City);
            postdata.Add("bill_addr_state", prepaidCard.State); // 2-letter State orTerritory Code
            postdata.Add("bill_addr_zip", prepaidCard.Zip);
            postdata.Add("bill_addr_country", prepaidCard.Country);


            //postdata.Add("request_currency", "USD"); // 3-letter Currency Code
            //postdata.Add("bill_addr_state", order.BillingProvince); // 2-letter State orTerritory Code
            //postdata.Add("bill_addr_zip", order.BillingPostalCode);
            //postdata.Add("bill_addr_country", order.BillingCountry);

            //postdata.Add("phone", order.Phone);
            //postdata.Add("email", order.Email);



            //postdata.Add("amount", order.Amount.ToString());

            postdata.Add("request_action", "CCAUTHCAP");

            var response = GatewayHelper.GetResponse(url, postdata);

            //Dictionary<string, string> dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(response);

            //var transaction = new Transaction()
            //{
            //    Response = response,
            //    IsRebill = order.IsRebill,
            //    OrderId = order.OrderId, 
            //    //Order = order,
            //    ProcessorId = processor.Id,
            //    Type = TransactionType.Sale,
            //    TransactionId = dic["PO_ID"], // 
            //    GatewayId = this.Id,
            //    Status = dic["TRANS_STATUS_NAME"] == "APPROVED" ? TransactionStatus.Approved : TransactionStatus.Declined,
            //};

            var transaction = CreateTransactionPrepaidCard(processor, response,amount, prepaidCard.Id);
            transaction.Type = TransactionType.Sale;
            transaction.Request = GatewayHelper.GetPostData(postdata);
            return transaction;

            //return response;
        }
        /// <summary>
        /// Authorize Action
        /// </summary>
        /// <param name="order"></param>
        /// <param name="processor"></param>
        /// <returns></returns>
        public override Transaction Authorize(Order order, Processor processor)
        {
            var postdata = fillRequest(order, processor, order.Total);

            // Payment and Bank Information Parameters
            // credit card data
            postdata.Add("pmt_numb", order.CreditCardNumber);
            postdata.Add("pmt_expiry", string.Format("{0}{1}", ((int)order.CreditCardExpirationMonth).ToString("D2"), order.CreditCardExpirationYear));
            postdata.Add("pmt_key", order.CreditCardCVV.ToString());
            //var currency = order.OrderProducts.First().Product.Currency.ToString();
            postdata.Add("request_currency", processor.Currency);

            // other data
            //postdata.Add("orderid", order.OrderId.ToString());
            //postdata.Add("ipaddress", order.IPAddress);

            //Customer Parameters
            postdata.Add("cust_fname", order.Customer.FirstName); // Cardholder’s First Name
            postdata.Add("cust_lname", order.Customer.LastName); // Cardholder’s Last Name
            postdata.Add("cust_email", order.Customer.Email); // Cardholder’s Email Address
            postdata.Add("cust_phone", order.Customer.Phone);
            // shipping data
            //postdata.Add("shipping_firstname", order.ShippingFirstName);
            //postdata.Add("shipping_lastname", order.ShippingLastName);
            postdata.Add("ship_addr", order.ShippingAddress1);
            //postdata.Add("shipping_address2", order.ShippingAddress2);
            postdata.Add("ship_addr_city", order.ShippingCity);
            postdata.Add("ship_addr_state", order.ShippingProvince);
            postdata.Add("ship_addr_zip", order.ShippingPostalCode);
            postdata.Add("ship_addr_country", order.ShippingCountry);
            // billing info
            //postdata.Add("firstname", order.BillingFirstName);
            //postdata.Add("lastname", order.BillingLastName);
            postdata.Add("bill_addr", order.BillingAddress1);
            //postdata.Add("address2", order.BillingAddress2);
            postdata.Add("bill_addr_city", order.BillingCity);
            postdata.Add("bill_addr_state", order.BillingProvince); // 2-letter State orTerritory Code
            postdata.Add("bill_addr_zip", order.BillingPostalCode);
            postdata.Add("bill_addr_country", order.BillingCountry);


            //postdata.Add("request_currency", "USD"); // 3-letter Currency Code
            //postdata.Add("bill_addr_state", order.BillingProvince); // 2-letter State orTerritory Code
            //postdata.Add("bill_addr_zip", order.BillingPostalCode);
            //postdata.Add("bill_addr_country", order.BillingCountry);

            //postdata.Add("phone", order.Phone);
            //postdata.Add("email", order.Email);



            //postdata.Add("amount", order.Amount.ToString());

            postdata.Add("request_action", "CCAUTHORIZE");

            var response = GatewayHelper.GetResponse(url, postdata);


            Dictionary<string, string> dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(response);

            var transaction = CreateTransaction(order, processor, response);
            transaction.Type = TransactionType.Auth;
            transaction.Request = GatewayHelper.GetPostData(postdata);

            return transaction;
        }
        /// <summary>
        /// void action
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public override Transaction Void(Transaction transaction)
        {
            var postdata = fillRequest(transaction.Order, transaction.Processor, transaction.Amount);

            postdata.Add("request_action", "CCREVERSE");

            // To do a Reversal
            // request, send the
            // Order ID (PO_ID) of
            // the original
            // authorization in the
            // REQUEST_REF_PO_ID
            // parameter.

            postdata.Add("REQUEST_REF_PO_ID", transaction.TransactionId);

            var response = GatewayHelper.GetResponse(url, postdata);


            var ctransaction = CreateTransaction(transaction, response);
            ctransaction.Type = TransactionType.Void;
            ctransaction.Request = GatewayHelper.GetPostData(postdata);

            return ctransaction;
        }
        /// <summary>
        /// refund action
        /// </summary>
        /// <param name="uow"></param>
        /// <param name="mapper"></param>
        /// <param name="notificationType"></param>
        /// <param name="orderId"></param>
        /// <param name="transaction"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public override Transaction Refund(IUnitOfWork uow, IMappingEngine mapper, NotificationType notificationType, int orderId, Transaction transaction, decimal amount)
        {
            var postdata = fillRequest(transaction.Order, transaction.Processor, amount);

            postdata.Add("request_action", "CCREVERSECAP");

            // To do a Reversal
            // request, send the
            // Order ID (PO_ID) of
            // the original
            // authorization in the
            // REQUEST_REF_PO_ID
            // parameter.

            postdata.Add("REQUEST_REF_PO_ID", transaction.TransactionId);

            var response = GatewayHelper.GetResponse(url, postdata);

            var ctransaction = CreateTransaction(transaction, response);
            ctransaction.Type = TransactionType.Refund;
            ctransaction.Amount = amount;
            if (ctransaction.Success)
                base.Refund(uow, mapper, notificationType, orderId, transaction, amount);
            ctransaction.Request = GatewayHelper.GetPostData(postdata);
            return ctransaction;
        }
        /// <summary>
        /// capture action
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public override Transaction Capture(Transaction transaction) // this is the auth transaction
        {
            var postdata = fillRequest(transaction.Order, transaction.Processor, transaction.Amount);

            postdata.Add("request_action", "CCCAPTURE");

            postdata.Add("REQUEST_REF_PO_ID", transaction.TransactionId);

            var response = GatewayHelper.GetResponse(url, postdata);

            var ctransaction = CreateTransaction(transaction, response);
            ctransaction.Type = TransactionType.Capture;
            ctransaction.Request = GatewayHelper.GetPostData(postdata);

            //ctransaction.Amount = amount; // the transaction amount will be the same as the order amount so 
            return ctransaction;
        }

        #region ## Private ##
        /// <summary>
        /// create transaction
        /// </summary>
        /// <param name="transaction">Transaction</param>
        /// <param name="response">Response</param>
        /// <returns></returns>
        private Transaction CreateTransaction(Transaction transaction, string response)
        {
            var rtransaction = CreateTransaction(transaction.Order, transaction.Processor, response);
            rtransaction.BalancerId = transaction.BalancerId;
            return rtransaction;
        }

        private TransactionViaPrepaidCardQueue CreateTransactionPrepaidCard(TransactionViaPrepaidCardQueue processor, string response,decimal amount,int prepaidCardid)
        {
            var rtransaction = CreateTransactionPrepaidCard(processor.Processor, response,amount, prepaidCardid);
            rtransaction.BalancerId = processor.BalancerId;
            return rtransaction;
        }

        private TransactionViaPrepaidCardQueue CreateTransactionPrepaidCard(Processor processor, string response,decimal amount,int prepaidCardid)
        {
            Dictionary<string, string> dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(response);

            var transaction = new TransactionViaPrepaidCardQueue()
            {
                Response = response,
                PrepaidCardId= prepaidCardid,
                //IsRebill = order.IsRebill,

                ProcessorId = processor.Id,
                Processor = processor,
                //TransactionId = dic.ContainsKey("PO_ID") ? dic["PO_ID"] : null, // 
                //GatewayId = this.Id,
                Date = DateTime.UtcNow,
                Amount = amount,
            };
            string value;
            dic.TryGetValue("PO_ID", out value);
            transaction.TransactionId = value;

            dic.TryGetValue("TRANS_STATUS_NAME", out value);
            transaction.Status = value == "APPROVED" ? TransactionStatus.Approved : TransactionStatus.Declined;
            transaction.Success = value == "APPROVED";

            if (transaction.Status == TransactionStatus.Declined)
            {
                dic.TryGetValue("SERVICE_ADVICE", out value);

                if (string.IsNullOrWhiteSpace(value))
                {
                    dic.TryGetValue("API_ADVICE", out value);
                }

                if (string.IsNullOrWhiteSpace(value))
                {
                    dic.TryGetValue("PROCESSOR_ADVICE", out value);
                }

                if (string.IsNullOrWhiteSpace(value))
                {
                    dic.TryGetValue("INDUSTRY_ADVICE", out value);
                }

                transaction.Message = value ?? "Reason might be found in the transaction response";


            }

            return transaction;
        }

        /// <summary>
        /// Create Transaction against order
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="processor">Processor</param>
        /// <param name="response">Response</param>
        /// <returns></returns>
        private Transaction CreateTransaction(Order order, Processor processor, string response)
        {
            Dictionary<string, string> dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(response);

            var transaction = new Transaction()
            {
                Response = response,
                //IsRebill = order.IsRebill,
                OrderId = order.OrderId,
                Order = order,
                ProcessorId = processor.Id,
                Processor = processor,
                //TransactionId = dic.ContainsKey("PO_ID") ? dic["PO_ID"] : null, // 
                //GatewayId = this.Id,
                Date = DateTime.UtcNow,
                Amount = order.Total,
            };
            string value;
            dic.TryGetValue("PO_ID", out value);
            transaction.TransactionId = value;

            dic.TryGetValue("TRANS_STATUS_NAME", out value);
            transaction.Status = value == "APPROVED" ? TransactionStatus.Approved : TransactionStatus.Declined;
            transaction.Success = value == "APPROVED";

            if (transaction.Status == TransactionStatus.Declined)
            {
                dic.TryGetValue("SERVICE_ADVICE", out value);

                if (string.IsNullOrWhiteSpace(value))
                {
                    dic.TryGetValue("API_ADVICE", out value);
                }

                if (string.IsNullOrWhiteSpace(value))
                {
                    dic.TryGetValue("PROCESSOR_ADVICE", out value);
                }

                if (string.IsNullOrWhiteSpace(value))
                {
                    dic.TryGetValue("INDUSTRY_ADVICE", out value);
                }

                transaction.Message = value ?? "Reason might be found in the transaction response";


            }

            return transaction;
        }
        /// <summary>
        /// Fill Request
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="processor">Processor</param>
        /// <param name="amount">Amount</param>
        /// <returns></returns>
        private NameValueCollection fillRequest(Order order, Processor processor, decimal amount)
        {
            // ARGUS
            var postdata = new NameValueCollection();

            // Authentication Parameters
            postdata.Add("req_username", this.Gateway.Username); // Service Request Username
            postdata.Add("req_password", this.Gateway.Password); // Service Request Password            
            postdata.Add("request_response_format", "JSON"); // “XML”, “PIPES” and ”JSON” Optional (Default: XML)
            postdata.Add("request_api_version", "3.5");
            postdata.Add("site_id", processor.SiteId); // Merchant’s Website ID


            // Line Items Parameters
            // since an order might have multiple product we are passing the first productid only
            //postdata.Add("li_prod_id_1", order.OrderProducts.First().ProductId.ToString()); // Line Item Product ID 1
            postdata.Add("li_prod_id_1", processor.DynamicProductId);
            postdata.Add("li_value_1", amount.ToString()); // Line Item Transaction Amount 1
            postdata.Add("xtl_order_id", order.OrderId.ToString()); // Merchant’s Order ID Numeric (max length:24) Optional
            postdata.Add("li_count_1", order.Quantity.ToString()); // Line Item Count Max value is “99”.

            if (!string.IsNullOrEmpty(processor.MerchantAccountId))
            {
                postdata.Add("merch_acct_id", processor.MerchantAccountId); // Numeric If null, the system will follow merchant’s bank load balancer settings.
            }



            return postdata;
        }

        private NameValueCollection fillRequestPrepaidCard(Processor processor, decimal amount)
        {
            // ARGUS
            var postdata = new NameValueCollection();

            // Authentication Parameters
            postdata.Add("req_username", this.Gateway.Username); // Service Request Username
            postdata.Add("req_password", this.Gateway.Password); // Service Request Password            
            postdata.Add("request_response_format", "JSON"); // “XML”, “PIPES” and ”JSON” Optional (Default: XML)
            postdata.Add("request_api_version", "3.5");
            postdata.Add("site_id", processor.SiteId); // Merchant’s Website ID


            // Line Items Parameters
            // since an order might have multiple product we are passing the first productid only
            //postdata.Add("li_prod_id_1", order.OrderProducts.First().ProductId.ToString()); // Line Item Product ID 1
            postdata.Add("li_prod_id_1", processor.DynamicProductId);
            postdata.Add("li_value_1", amount.ToString()); // Line Item Transaction Amount 1
            //postdata.Add("xtl_order_id", order.OrderId.ToString()); // Merchant’s Order ID Numeric (max length:24) Optional
            //postdata.Add("li_count_1", order.Quantity.ToString()); // Line Item Count Max value is “99”.

            if (!string.IsNullOrEmpty(processor.MerchantAccountId))
            {
                postdata.Add("merch_acct_id", processor.MerchantAccountId); // Numeric If null, the system will follow merchant’s bank load balancer settings.
            }



            return postdata;
        }


        #endregion
    }
}