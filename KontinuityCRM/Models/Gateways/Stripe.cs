using DotNet.Highcharts;
using KontinuityCRM.Helpers;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Collections.Specialized;
using Newtonsoft.Json;
using System.Net.Http;
using AutoMapper;

namespace KontinuityCRM.Models.Gateways
{
    [DisplayName("Stripe Payment")]
    public class Stripe : GatewayModel
    {
        public string Currency { get; set; }

        public readonly string url = "https://api.stripe.com/v1/";
        public override Transaction Sale(Order order, Processor processor)
        {
            var TokenResponse = GetToken(order);
            var TokenContent = TokenResponse.Content.ReadAsStringAsync().Result;
            Transaction stransaction;
            if (TokenResponse.IsSuccessStatusCode)
            {
                var data = Newtonsoft.Json.JsonConvert.DeserializeObject(TokenContent);
                var amount = Convert.ToInt32(order.Total * 100);
                var postData = new NameValueCollection();
                postData.Add("amount", amount.ToString());
                postData.Add("source", data.id.ToString());
                postData.Add("currency", "usd");
                var ChargeResponse = GatewayHelper.GetResponse(url + "charges", HttpMethod.Post, postData, AuthHeader());
                var ChargeContent = ChargeResponse.Content.ReadAsStringAsync().Result;
                stransaction = CreateTransaction(order, processor, ChargeContent);
                stransaction.Request = GatewayHelper.GetPostData(postData);
            }
            else
                stransaction = CreateTransaction(order, processor, TokenContent);


            stransaction.Type = TransactionType.Sale;
            return stransaction;



        }

        public override Transaction Authorize(Order order, Processor processor)
        {
            var TokenResponse = GetToken(order);
            var TokenContent = TokenResponse.Content.ReadAsStringAsync().Result;
            var amount = Convert.ToInt32(order.Total * 100);
            Transaction stransaction;
            if (TokenResponse.IsSuccessStatusCode)
            {
                var data = Newtonsoft.Json.JsonConvert.DeserializeObject(TokenContent);
                var postData = new NameValueCollection();
                postData.Add("amount", amount.ToString());
                postData.Add("source", data.id.ToString());
                postData.Add("currency", "usd");
                postData.Add("capture", "false");
                var ChargeResponse = GatewayHelper.GetResponse(url + "charges", HttpMethod.Post, postData, AuthHeader());
                var ChargeContent = ChargeResponse.Content.ReadAsStringAsync().Result;
                stransaction = CreateTransaction(order, processor, ChargeContent);
                stransaction.Request = GatewayHelper.GetPostData(postData);
            }
            else
                stransaction = CreateTransaction(order, processor, TokenContent);
            stransaction.Type = TransactionType.Auth;
            return stransaction;
        }

        public override Transaction Void(Transaction transaction)
        {
            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(transaction.Response);
            var postData = new NameValueCollection();
            postData.Add("charge", data.id.ToString());
            var VoidResponse = GatewayHelper.GetResponse(url + "refunds", HttpMethod.Post, postData, AuthHeader());
            var VoidContent = VoidResponse.Content.ReadAsStringAsync().Result;
            var vtransaction = CreateTransaction(transaction, VoidContent);
            vtransaction.Type = TransactionType.Void;
            vtransaction.Request = GatewayHelper.GetPostData(postData);
            return vtransaction;

        }

        public override Transaction Capture(Transaction transaction)
        {
            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(transaction.Response);
            var CaptureResponse = GatewayHelper.GetResponse(string.Format(url + "charges/{0}/capture", data.id.ToString()), HttpMethod.Post, new NameValueCollection() { }, AuthHeader());
            var ChargeContent = CaptureResponse.Content.ReadAsStringAsync().Result;
            var ctransaction = CreateTransaction(transaction, ChargeContent);
            ctransaction.Type = TransactionType.Capture;
            //  ctransaction.Request = GatewayHelper.GetPostData(postdata);
            return ctransaction;
        }

        public override Transaction Refund(IUnitOfWork uow, IMappingEngine mapper, NotificationType notificationType, int orderId, Transaction transaction, decimal amount)
        {

            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(transaction.Response);
            var postData = new NameValueCollection();
            int calcAmount = Convert.ToInt32(amount * 100);
            postData.Add("charge", data.id.ToString());
            postData.Add("amount", calcAmount.ToString());
            var RefundResponse = GatewayHelper.GetResponse(url + "refunds", HttpMethod.Post, postData, AuthHeader());
            var RefundContent = RefundResponse.Content.ReadAsStringAsync().Result;
            var rtransaction = CreateTransaction(transaction, RefundContent);
            rtransaction.Type = TransactionType.Refund;
            rtransaction.Request = GatewayHelper.GetPostData(postData);
            if (rtransaction.Success)
                base.Refund(uow, mapper, notificationType, orderId, transaction, amount);
            return rtransaction;
        }

        private dynamic GetToken(Order order)
        {
            // Create POST data and convert it to a byte array.
            var postData = new NameValueCollection();
            postData.Add("card[number]", order.CreditCardNumber);
            postData.Add("card[exp_month]", ((int)order.CreditCardExpirationMonth).ToString());
            postData.Add("card[exp_year]", order.CreditCardExpirationYear.ToString());
            postData.Add("card[cvc]", order.CreditCardCVV);
            postData.Add("card[name]", order.BillingFirstName + " " + order.BillingLastName);
            postData.Add("card[address_city]", order.BillingCity);
            postData.Add("card[address_country]", order.BillingCountry);
            postData.Add("card[address_state]", order.BillingProvince);
            postData.Add("card[address_zip]", order.BillingPostalCode);
            return GatewayHelper.GetResponse(url + "tokens", HttpMethod.Post, postData, AuthHeader());

        }

        public Dictionary<string, string> AuthHeader()
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Authorization", "Bearer " + this.Gateway.Username); //The UserName is API Key provided by stripe
            return headers;
        }


        private Transaction CreateTransaction(Order order, Processor processor, string response)
        {
            var responseObj = JsonConvert.DeserializeObject<dynamic>(response);

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
            if (responseObj.error != null)
            {
                transaction.Message = responseObj.error.message;
                transaction.Success = false;
                transaction.Status = TransactionStatus.Declined;
            }
            else
            {
                transaction.Success = true;
                transaction.Status = TransactionStatus.Approved;
            }
            return transaction;
        }
        /// <summary>
        /// Create Trransaction
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        private Transaction CreateTransaction(Transaction transaction, string response)
        {
            var rtransaction = CreateTransaction(transaction.Order, transaction.Processor, response);
            rtransaction.BalancerId = transaction.BalancerId;
            return rtransaction;
        }

        public override TransactionViaPrepaidCardQueue SalePrepaidCard(PrepaidCard prepaidCard, Processor processor, decimal amouunt)
        {
            throw new NotImplementedException();
        }
    }
}