using AutoMapper;
using KontinuityCRM.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KontinuityCRM.Models.Gateways
{
    [DisplayName("Network Merchants")]
    public class NMI : GatewayModel
    {
        /// <summary>
        /// indicates Currency
        /// </summary>
        public string Currency { get; set; }
        /// <summary>
        /// Indicates Processor Id
        /// </summary>
        [Display(Name = "Post Processor Id")]
        public bool PostProcessorId { get; set; }
        /// <summary>
        /// Indicates Post Product Description
        /// </summary>
        [Display(Name = "Post Product Description")]
        public bool PostProductDescription { get; set; }
        /// <summary>
        /// Indicates whether post description required
        /// </summary>
        [Display(Name = "Post Descriptor")]
        public bool PostDescriptor { get; set; }
        /// <summary>
        /// Indicates if to use preAuthorizeFilter
        /// </summary>
        [Display(Name = "Use Pre-Auth Filter")]
        public bool UsePreAuthorizationFilter { get; set; }

        //[Display(Name = "Use Decline Salvage")]
        //public bool UseDeclineSalvage { get; set; }

        public string MDF1 { get; set; }

        public string MDF2 { get; set; }

        public string MDF3 { get; set; }

        public string MDF4 { get; set; }

        public string MDF5 { get; set; }

        public string MDF6 { get; set; }

        public string MDF7 { get; set; }

        public string MDF8 { get; set; }

        public string MDF9 { get; set; }

        public string MDF10 { get; set; }

        public string MDF11 { get; set; }

        public string MDF12 { get; set; }

        public string MDF13 { get; set; }

        public string MDF14 { get; set; }

        public string MDF15 { get; set; }

        public string MDF16 { get; set; }

        public string MDF17 { get; set; }

        public string MDF18 { get; set; }

        public string MDF19 { get; set; }

        public string MDF20 { get; set; }


        public readonly string url = "https://secure.nmi.com/api/transact.php";
        /// <summary>
        /// sale action
        /// </summary>
        /// <param name="order"></param>
        /// <param name="processor"></param>
        /// <returns></returns>
        public override Transaction Sale(Order order, Processor processor)
        {
            var postdata = fillRequest(order);

            postdata.Add("type", "sale");

            var response = GatewayHelper.GetResponse(url, postdata);

            var transaction = CreateTransaction(order, processor, response);
            transaction.Type = TransactionType.Sale;
            transaction.Request = GatewayHelper.GetPostData(postdata);
            return transaction;
        }

        public override TransactionViaPrepaidCardQueue SalePrepaidCard(PrepaidCard prepaidCard, Processor processor,decimal amouunt)
        {
            var postdata = fillRequestPrepaidCard(prepaidCard,processor, amouunt);

            postdata.Add("type", "sale");

            var response = GatewayHelper.GetResponse(url, postdata);

            var transaction = CreateTransactionNMI(processor, response, GatewayHelper.GetPostData(postdata), amouunt,prepaidCard.Id);
            transaction.Type = TransactionType.Sale;
            transaction.Request = GatewayHelper.GetPostData(postdata);
            return transaction;
        }
        /// <summary>
        /// authorize action
        /// </summary>
        /// <param name="order"></param>
        /// <param name="processor"></param>
        /// <returns></returns>
        public override Transaction Authorize(Order order, Processor processor)
        {
            var postdata = fillRequest(order);

            postdata.Add("type", "auth");

            var response = GatewayHelper.GetResponse(url, postdata);

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
            var postdata = new NameValueCollection();

            postdata.Add("type", "void");
            postdata.Add("transactionid", transaction.TransactionId);
            postdata.Add("username", this.Gateway.Username);
            postdata.Add("password", this.Gateway.Password);

            var response = GatewayHelper.GetResponse(url, postdata);

            var ctransaction = CreateTransaction(transaction, response);
            ctransaction.Type = TransactionType.Void;
            ctransaction.Request = GatewayHelper.GetPostData(postdata);
            return ctransaction;

        }
        /// <summary>
        /// Refund action and returns Transaction in response
        /// </summary>
        /// <param name="uow">UOW</param>
        /// <param name="mapper">mapper</param>
        /// <param name="notificationType">Transaction Id</param>
        /// <param name="orderId">Order Id</param>
        /// <param name="transaction">Transaction</param>
        /// <param name="amount">Amount</param>
        /// <returns></returns>
        public override Transaction Refund(IUnitOfWork uow, IMappingEngine mapper, NotificationType notificationType,int orderId,Transaction transaction, decimal amount = 0)
        {
            var postdata = new NameValueCollection();

            postdata.Add("type", "refund");
            postdata.Add("transactionid", transaction.TransactionId);
            postdata.Add("username", this.Gateway.Username);
            postdata.Add("password", this.Gateway.Password);
            postdata.Add("amount", amount.ToString());

            var response = GatewayHelper.GetResponse(url, postdata);

            var ctransaction = CreateTransaction(transaction, response);
            ctransaction.Type = TransactionType.Refund;
            if (ctransaction.Success)
                base.Refund(uow, mapper, notificationType, orderId, transaction, amount);
            ctransaction.Request = GatewayHelper.GetPostData(postdata);
            return ctransaction;


        }
        /// <summary>
        /// Capture and returns Transaction
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public override Transaction Capture(Transaction transaction)
        {
            var postdata = new NameValueCollection();

            postdata.Add("type", "capture");
            postdata.Add("transactionid", transaction.TransactionId);
            postdata.Add("username", this.Gateway.Username);
            postdata.Add("password", this.Gateway.Password);
            postdata.Add("amount", transaction.Amount.ToString());

            var response = GatewayHelper.GetResponse(url, postdata);

            var ctransaction = CreateTransaction(transaction, response);
            ctransaction.Type = TransactionType.Capture;
            ctransaction.Request = GatewayHelper.GetPostData(postdata);
            return ctransaction;


        }

        #region ## Private ##
        /// <summary>
        /// Makes Request from Order and retruns NameValueColleaction
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns></returns>
        private NameValueCollection fillRequest(Order order)
        {
            var postdata = new NameValueCollection();
            postdata.Add("username", this.Gateway.Username);
            postdata.Add("password", this.Gateway.Password);

            // credit card data
            postdata.Add("ccnumber", order.CreditCardNumber);
            postdata.Add("ccexp", string.Format("{0}{1}", ((int)order.CreditCardExpirationMonth).ToString("D2"), order.CreditCardExpirationYear));
            postdata.Add("cvv", order.CreditCardCVV.ToString());

            // other data
            postdata.Add("orderid", order.OrderId.ToString());
            postdata.Add("ipaddress", order.IPAddress);
            
            // shipping data
            postdata.Add("shipping_firstname", order.ShippingFirstName);
            postdata.Add("shipping_lastname", order.ShippingLastName);
            postdata.Add("shipping_address1", order.ShippingAddress1);
            postdata.Add("shipping_address2", order.ShippingAddress2);
            postdata.Add("shipping_city", order.ShippingCity);
            postdata.Add("shipping_state", order.ShippingProvince);
            postdata.Add("shipping_zip", order.ShippingPostalCode);
            postdata.Add("shipping_country", order.ShippingCountry);

            // billing info
            postdata.Add("firstname", order.BillingFirstName);
            postdata.Add("lastname", order.BillingLastName);
            postdata.Add("address1", order.BillingAddress1);
            postdata.Add("address2", order.BillingAddress2);
            postdata.Add("city", order.BillingCity);
            postdata.Add("state", order.BillingProvince);
            postdata.Add("zip", order.BillingPostalCode);
            postdata.Add("country", order.BillingCountry);

            postdata.Add("phone", order.Phone);
            postdata.Add("email", order.Email);
            
            postdata.Add("amount", order.Total.ToString().Replace(",", "."));

            return postdata;
        }

        private NameValueCollection fillRequestPrepaidCard(PrepaidCard card,Processor processorDetails,decimal amount)
        {
            var postdata = new NameValueCollection();
            postdata.Add("username", processorDetails.Gateway.Username);
            postdata.Add("password", processorDetails.Gateway.Password);
            // credit card data
            postdata.Add("ccnumber", card.Number);
            postdata.Add("ccexp", string.Format("{0}{1}", (Convert.ToInt32(card.CreditCardExpirationMonth)).ToString("D2"), (Convert.ToInt32(card.CreditCardExpirationYear))));
            postdata.Add("cvv", card.CreditCardCVV.ToString());

            // billing info
            postdata.Add("firstname", card.First_Name);
            postdata.Add("lastname", card.Last_Name);
            postdata.Add("address1", card.Address);
            postdata.Add("address2", card.Address);
            postdata.Add("city", card.City);
            postdata.Add("state", card.State);
            postdata.Add("zip", card.Zip);
            postdata.Add("country", card.Country);

            postdata.Add("phone", card.Phone);
            postdata.Add("email", card.Email);
            postdata.Add("amount", amount.ToString().Replace(",", "."));

            return postdata;
        }
        /// <summary>
        /// Create Transaction against Order
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="processor">Processor</param>
        /// <param name="response">Response</param>
        /// <returns></returns>
        private Transaction CreateTransaction(Order order, Processor processor, string response)
        {
            var nvc = HttpUtility.ParseQueryString(response);

            var currentTransaction = new Transaction()
            {
                Response = response,
                //IsRebill = order.IsRebill,
                OrderId = order.OrderId,
                Order = order,
                ProcessorId = processor.Id,
                Processor = processor,
                //GatewayId = this.Id,
                TransactionId = nvc["transactionid"],
                //Status = nvc[0].Equals("1") ? TransactionStatus.Approved : nvc[0].Equals("2") ? TransactionStatus.Declined : TransactionStatus.Error,
                Status = nvc[0].Equals("1") ? TransactionStatus.Approved : TransactionStatus.Declined,
                Success = nvc[0].Equals("1"),
                //Type = TransactionType.Refund,
                Message = nvc["responsetext"],
                Date = DateTime.UtcNow,
                Amount = order.Total,
            };

            return currentTransaction;
        }

        private TransactionViaPrepaidCardQueue CreateTransactionNMI(Processor processor, string response, string request, decimal amount, int prepaidCardId)
        {
            var nvc = HttpUtility.ParseQueryString(response);

            var currentTransaction = new TransactionViaPrepaidCardQueue()
            {
                Response = response,
                Request = request,
                //IsRebill = order.IsRebill,
                PrepaidCardId = prepaidCardId,
                ProcessorId = processor.Id,
                Processor = processor,
                //GatewayId = this.Id,
                TransactionId = nvc["transactionid"],
                //Status = nvc[0].Equals("1") ? TransactionStatus.Approved : nvc[0].Equals("2") ? TransactionStatus.Declined : TransactionStatus.Error,
                Status = nvc[0].Equals("1") ? Models.TransactionStatus.Approved : Models.TransactionStatus.Declined,
                Success = nvc[0].Equals("1"),
                //Type = TransactionType.Refund,
                Message = nvc["responsetext"],
                Date = DateTime.UtcNow,
                Amount = amount,
            };

            return currentTransaction;
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


        #endregion
    }
    /// <summary>
    /// Currency Enum
    /// </summary>
    public enum Currency
    { 
        USD,
        EUR,
        GBP,
        CAD,
        AUD,
        NZD,
        SGD,
        CHF,
        MXN,
        HKD,
        NOK,
        DKK,
        JPY,
        ZAR,
    }
}