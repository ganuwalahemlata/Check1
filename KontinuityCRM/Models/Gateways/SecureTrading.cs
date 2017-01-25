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
using System.Xml;


namespace KontinuityCRM.Models.Gateways
{
    /// <summary>
    /// Class SecureTrading
    /// </summary>
    /// <seealso cref="KontinuityCRM.Models.Gateways.GatewayModel" />
    [DisplayName("Secure Trading")]
    public class SecureTrading : GatewayModel
    {
        /// <summary>
        /// Gets or sets the site identifier.
        /// </summary>
        /// <value>The site identifier.</value>
        [Required]
        [Display(Name = "Site Name")]
        public string SiteId { get; set; }

        /// <summary>
        /// Gets or sets the currency.
        /// </summary>
        /// <value>The currency.</value>
        public string Currency { get; set; }

        /// <summary>
        /// The URL
        /// </summary>
        public readonly string url = "https://webservices.securetrading.net/xml/";

        /// <summary>
        /// Sales the specified order.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <param name="processor">The processor.</param>
        /// <returns>Transaction.</returns>
        public override Transaction Sale(Order order, Processor processor)
        {
            var request = this.GenerateRequestXML(order, processor, true);
            var response = GatewayHelper.GetResponse(url, request, this.GetHeaderData());
            var res = this.CreateTransaction(order, processor, response, order.Total);
            res.Type = TransactionType.Sale;
            res.Request = request;
            return res;
        }

        public override TransactionViaPrepaidCardQueue SalePrepaidCard(PrepaidCard prepaidCard, Processor processor, decimal amouunt)
        {
            var request = this.GenerateRequestXMLPrepaidCard(processor,prepaidCard, amouunt, true);
            var response = GatewayHelper.GetResponse(url, request, this.GetHeaderData());
            var res = this.CreateTransactionPrepaidCard(processor, response, amouunt,prepaidCard.Id);
            res.Type = TransactionType.Sale;
            res.Request = request;
            return res;
        }

        /// <summary>
        /// Authorizes the specified order.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <param name="processor">The processor.</param>
        /// <returns>Transaction.</returns>
        public override Transaction Authorize(Order order, Processor processor)
        {
            var request = this.GenerateRequestXML(order, processor, false);
            var response = GatewayHelper.GetResponse(url, request, this.GetHeaderData());
            var res = this.CreateTransaction(order, processor, response, order.Total);
            res.Type = TransactionType.Auth;
            res.Request = request;
            return res;
        }

        /// <summary>
        /// Voids the specified transaction.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <returns>Transaction.</returns>
        public override Transaction Void(Transaction transaction)
        {
            // TODO
            // Refactor this

            XmlDocument doc = new XmlDocument();

            XmlElement root = this.CreateElement(doc, "requestblock");
            var attr = doc.CreateAttribute("version");
            attr.Value = "3.67";
            root.Attributes.Append(attr);

            root.AppendChild(this.CreateElement(doc, "alias", this.Gateway.Username));

            XmlElement request = this.CreateElement(doc, "request");
            attr = doc.CreateAttribute("type");
            attr.Value = "TRANSACTIONUPDATE";
            request.Attributes.Append(attr);

            XmlElement filter = this.CreateElement(doc, "filter");
            filter.AppendChild(this.CreateElement(doc, "sitereference", this.SiteId));
            filter.AppendChild(this.CreateElement(doc, "transactionreference", transaction.TransactionReference));
            request.AppendChild(filter);

            XmlElement updates = this.CreateElement(doc, "updates");

            XmlElement settlement = this.CreateElement(doc, "settlement");
            settlement.AppendChild(this.CreateElement(doc, "settlestatus", "3"));
            updates.AppendChild(settlement);

            request.AppendChild(updates);
            root.AppendChild(request);
            doc.AppendChild(root);

            var response = GatewayHelper.GetResponse(url, doc.InnerXml, this.GetHeaderData());
            var res = this.CreateTransactionUpdate(transaction.Order, transaction.Processor, response);
            res.Type = TransactionType.Refund;
            res.Request = doc.InnerXml;
            return res;
        }

        /// <summary>
        /// Refunds the specified transaction.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="amount">The amount.</param>
        /// <returns>Transaction.</returns>
        public override Transaction Refund(IUnitOfWork uow, IMappingEngine mapper, NotificationType notificationType, int orderId, Transaction transaction, decimal amount)
        {
            // TODO Refactor this
            XmlDocument doc = new XmlDocument();

            XmlElement root = this.CreateElement(doc, "requestblock");
            var attr = doc.CreateAttribute("version");
            attr.Value = "3.67";
            root.Attributes.Append(attr);

            root.AppendChild(this.CreateElement(doc, "alias", this.Gateway.Username));

            XmlElement request = this.CreateElement(doc, "request");
            attr = doc.CreateAttribute("type");
            attr.Value = "REFUND";
            request.Attributes.Append(attr);

            XmlElement merchant = this.CreateElement(doc, "merchant");
            merchant.AppendChild(this.CreateElement(doc, "orderreference", transaction.Order.OrderId.ToString()));
            request.AppendChild(merchant);

            XmlElement billing = this.CreateElement(doc, "billing");

            XmlElement amountE = this.CreateElement(doc, "amount", amount.ToString("#####0.00").Replace(".", ""));
            billing.AppendChild(amountE);
            request.AppendChild(billing);

            XmlElement operation = this.CreateElement(doc, "operation");
            operation.AppendChild(this.CreateElement(doc, "sitereference", this.SiteId));
            operation.AppendChild(this.CreateElement(doc, "parenttransactionreference", transaction.TransactionReference));
            request.AppendChild(operation);

            root.AppendChild(request);
            doc.AppendChild(root);

            var response = GatewayHelper.GetResponse(url, doc.InnerXml, this.GetHeaderData());
            var res = this.CreateTransaction(transaction.Order, transaction.Processor, response, amount);
            res.Type = TransactionType.Refund;
            res.Request = doc.InnerXml;
            // check to make sure that the transaction is settled -- if not, update the transaction to the amount less the refund amount
            if (res.Message == "Refund requires a settled parent")
            {
                // we use the order.total because a refund has not been processed yet, so it's safe to use the entire order total amount.
                var newamount = transaction.Order.Total - amount;

                doc = CreateTransactionUpdateRefund(newamount, transaction);

                response = GatewayHelper.GetResponse(url, doc.InnerXml, this.GetHeaderData());
                res = CreateTransaction(transaction.Order, transaction.Processor, response, amount);
                res.Request = doc.InnerXml;
            }
            if (res.Success)
                base.Refund(uow, mapper, notificationType, orderId, transaction, amount);

            return res;
        }

        public override Transaction Capture(Transaction transaction) // this is the auth transaction
        {
            return null;
        }


        private XmlElement CreateElement(XmlDocument doc, string name, string value = "")
        {
            XmlElement ele = doc.CreateElement(string.Empty, name, string.Empty);
            if (!string.IsNullOrWhiteSpace(value)) ele.InnerText = value;
            return ele;
        }

        private string GenerateRequestXML(Order order, Processor processor, bool captured = false)
        {
            XmlDocument doc = new XmlDocument();

            XmlElement root = this.CreateElement(doc, "requestblock");
            var attr = doc.CreateAttribute("version");
            attr.Value = "3.67";
            root.Attributes.Append(attr);

            root.AppendChild(this.CreateElement(doc, "alias", this.Gateway.Username));

            XmlElement request = this.CreateElement(doc, "request");
            attr = doc.CreateAttribute("type");
            attr.Value = "AUTH";
            request.Attributes.Append(attr);

            XmlElement merchant = this.CreateElement(doc, "merchant");
            merchant.AppendChild(this.CreateElement(doc, "orderreference", order.OrderId.ToString()));
            request.AppendChild(merchant);

            XmlElement customer = this.CreateElement(doc, "customer");
            customer.AppendChild(this.CreateElement(doc, "town", order.Customer.City));

            XmlElement name = this.CreateElement(doc, "name");
            name.AppendChild(this.CreateElement(doc, "middle"));
            name.AppendChild(this.CreateElement(doc, "prefix"));
            name.AppendChild(this.CreateElement(doc, "last", order.Customer.LastName));
            name.AppendChild(this.CreateElement(doc, "first", order.Customer.FirstName));
            customer.AppendChild(name);
            customer.AppendChild(this.CreateElement(doc, "ip", order.Customer.IPAddress));

            XmlElement telephone = this.CreateElement(doc, "telephone", order.Customer.Phone);
            attr = doc.CreateAttribute("type");
            attr.Value = "H";
            telephone.Attributes.Append(attr);
            customer.AppendChild(telephone);
            customer.AppendChild(this.CreateElement(doc, "street", order.Customer.Address1));
            customer.AppendChild(this.CreateElement(doc, "postcode", order.Customer.PostalCode));
            customer.AppendChild(this.CreateElement(doc, "premise"));
            request.AppendChild(customer);

            XmlElement billing = this.CreateElement(doc, "billing");

            XmlElement telephoneBilling = this.CreateElement(doc, "telephone", order.Phone);
            attr = doc.CreateAttribute("type");
            attr.Value = "H";
            telephoneBilling.Attributes.Append(attr);
            billing.AppendChild(telephoneBilling);
            billing.AppendChild(this.CreateElement(doc, "county"));
            billing.AppendChild(this.CreateElement(doc, "street", order.BillingAddress1));
            billing.AppendChild(this.CreateElement(doc, "postcode", order.BillingPostalCode));
            billing.AppendChild(this.CreateElement(doc, "premise", ""));

            XmlElement payment = this.CreateElement(doc, "payment");
            attr = doc.CreateAttribute("type");

            if (order.PaymentType == PaymentType.Visa) attr.Value = "VISA";
            else if (order.PaymentType == PaymentType.AmericanExpress) attr.Value = "AMEX";
            else if (order.PaymentType == PaymentType.Discover) attr.Value = "DISCOVER";
            else if (order.PaymentType == PaymentType.MasterCard) attr.Value = "MASTERCARD";
            else attr.Value = "";

            payment.Attributes.Append(attr);

            string year = (order.CreditCardExpirationYear.ToString().Length == 2 ? DateTime.Today.Year.ToString().Substring(0, 2) + order.CreditCardExpirationYear.ToString() : order.CreditCardExpirationYear.ToString());
            string expiration = string.Format("{0}/{1}", ((int)order.CreditCardExpirationMonth).ToString("D2"), year);
            payment.AppendChild(this.CreateElement(doc, "expirydate", expiration));
            payment.AppendChild(this.CreateElement(doc, "pan", order.CreditCardNumber.Replace(" ", "").Replace("-", "").Replace(".", "").Trim()));
            if (order.ParentId != null)
            {
                // can't send cvv on rebills - big no no
                payment.AppendChild(this.CreateElement(doc, "securitycode", "000"));

            }
            else
            {
                payment.AppendChild(this.CreateElement(doc, "securitycode", order.CreditCardCVV));

            }
            billing.AppendChild(payment);
            billing.AppendChild(this.CreateElement(doc, "town", order.BillingCity));

            XmlElement namebilling = this.CreateElement(doc, "name");
            namebilling.AppendChild(this.CreateElement(doc, "middle"));
            namebilling.AppendChild(this.CreateElement(doc, "prefix"));
            namebilling.AppendChild(this.CreateElement(doc, "last", order.BillingLastName));
            namebilling.AppendChild(this.CreateElement(doc, "first", order.BillingFirstName));
            billing.AppendChild(namebilling);
            billing.AppendChild(this.CreateElement(doc, "country", order.BillingCountry));

            XmlElement amount = this.CreateElement(doc, "amount", order.Total.ToString("#0.00").Replace(".", ""));
            attr = doc.CreateAttribute("currencycode");
            attr.Value = this.Currency.ToUpper();
            amount.Attributes.Append(attr);
            billing.AppendChild(amount);
            billing.AppendChild(this.CreateElement(doc, "email", order.Email));
            if (order.ParentId != null)
            {
                XmlElement subscription = this.CreateElement(doc, "subscription");
                attr = doc.CreateAttribute("type");
                attr.Value = "RECURRING";
                subscription.Attributes.Append(attr);
                subscription.AppendChild(this.CreateElement(doc, "number", (order.Depth).ToString()));
                billing.AppendChild(subscription);
            }
            request.AppendChild(billing);

            XmlElement operation = this.CreateElement(doc, "operation");
            operation.AppendChild(this.CreateElement(doc, "sitereference", this.SiteId));
            if (order.ParentId != null)
                        {
                            operation.AppendChild(this.CreateElement(doc, "accounttypedescription", "RECUR"));

                        }
                        else
                        {
                            operation.AppendChild(this.CreateElement(doc, "accounttypedescription", "ECOM"));

                        }
            request.AppendChild(operation);

            XmlElement settlement = this.CreateElement(doc, "settlement");
            settlement.AppendChild(this.CreateElement(doc, "settlestatus", (captured ? "0" : "1")));

            if (processor.CaptureDelayHours.HasValue && processor.CaptureDelayHours > 0)
            {
                settlement.AppendChild(this.CreateElement(doc, "settleduedate", DateTime.Today.AddDays(processor.CaptureDelayHours.Value).ToString("yyyy-MM-dd")));
            }

            request.AppendChild(settlement);

            root.AppendChild(request);
            doc.AppendChild(root);

            return doc.InnerXml;
        }

        private string GenerateRequestXMLPrepaidCard(Processor processor, PrepaidCard prepaidCard,decimal transactionAmount,bool captured = false)
        {
            XmlDocument doc = new XmlDocument();

            XmlElement root = this.CreateElement(doc, "requestblock");
            var attr = doc.CreateAttribute("version");
            attr.Value = "3.67";
            root.Attributes.Append(attr);

            root.AppendChild(this.CreateElement(doc, "alias", this.Gateway.Username));

            XmlElement request = this.CreateElement(doc, "request");
            attr = doc.CreateAttribute("type");
            attr.Value = "AUTH";
            request.Attributes.Append(attr);

            XmlElement merchant = this.CreateElement(doc, "merchant");
           // merchant.AppendChild(this.CreateElement(doc, "orderreference", order.OrderId.ToString()));
            request.AppendChild(merchant);

            XmlElement customer = this.CreateElement(doc, "customer");
            customer.AppendChild(this.CreateElement(doc, "town", prepaidCard.City));

            XmlElement name = this.CreateElement(doc, "name");
            name.AppendChild(this.CreateElement(doc, "middle"));
            name.AppendChild(this.CreateElement(doc, "prefix"));
            name.AppendChild(this.CreateElement(doc, "last", prepaidCard.Last_Name));
            name.AppendChild(this.CreateElement(doc, "first", prepaidCard.First_Name));
            customer.AppendChild(name);
           // customer.AppendChild(this.CreateElement(doc, "ip", order.Customer.IPAddress));

            XmlElement telephone = this.CreateElement(doc, "telephone", prepaidCard.Phone);
            attr = doc.CreateAttribute("type");
            attr.Value = "H";
            telephone.Attributes.Append(attr);
            customer.AppendChild(telephone);
            customer.AppendChild(this.CreateElement(doc, "street", prepaidCard.Address));
            customer.AppendChild(this.CreateElement(doc, "postcode", prepaidCard.Zip));
            customer.AppendChild(this.CreateElement(doc, "premise"));
            request.AppendChild(customer);

            XmlElement billing = this.CreateElement(doc, "billing");

            XmlElement telephoneBilling = this.CreateElement(doc, "telephone", prepaidCard.Phone);
            attr = doc.CreateAttribute("type");
            attr.Value = "H";
            telephoneBilling.Attributes.Append(attr);
            billing.AppendChild(telephoneBilling);
            billing.AppendChild(this.CreateElement(doc, "county"));
            billing.AppendChild(this.CreateElement(doc, "street", prepaidCard.Address));
            billing.AppendChild(this.CreateElement(doc, "postcode", prepaidCard.Zip));
            billing.AppendChild(this.CreateElement(doc, "premise", ""));

            XmlElement payment = this.CreateElement(doc, "payment");
            attr = doc.CreateAttribute("type");

            if (prepaidCard.PaymentType == "VISA") attr.Value = "VISA";
            else if (prepaidCard.PaymentType == "AMEX") attr.Value = "AMEX";
            else if (prepaidCard.PaymentType == "DISCOVER") attr.Value = "DISCOVER";
            else if (prepaidCard.PaymentType == "MASTERCARD") attr.Value = "MASTERCARD";
            else attr.Value = "";

            payment.Attributes.Append(attr);

            string year = (prepaidCard.CreditCardExpirationYear.ToString().Length == 2 ? DateTime.Today.Year.ToString().Substring(0, 2) + prepaidCard.CreditCardExpirationYear.ToString() : prepaidCard.CreditCardExpirationYear.ToString());
            string expiration = string.Format("{0}/{1}", (Convert.ToInt32(prepaidCard.CreditCardExpirationMonth)).ToString("D2"), year);
            payment.AppendChild(this.CreateElement(doc, "expirydate", expiration));
            payment.AppendChild(this.CreateElement(doc, "pan", prepaidCard.Number.Replace(" ", "").Replace("-", "").Replace(".", "").Trim()));
            if (prepaidCard.CreditCardCVV == null)
            {
                // can't send cvv on rebills - big no no
                payment.AppendChild(this.CreateElement(doc, "securitycode", "000"));

            }
            else
            {
                payment.AppendChild(this.CreateElement(doc, "securitycode", prepaidCard.CreditCardCVV));

            }
            billing.AppendChild(payment);
            billing.AppendChild(this.CreateElement(doc, "town", prepaidCard.City));

            XmlElement namebilling = this.CreateElement(doc, "name");
            namebilling.AppendChild(this.CreateElement(doc, "middle"));
            namebilling.AppendChild(this.CreateElement(doc, "prefix"));
            namebilling.AppendChild(this.CreateElement(doc, "last", prepaidCard.First_Name));
            namebilling.AppendChild(this.CreateElement(doc, "first", prepaidCard.Last_Name));
            billing.AppendChild(namebilling);
           billing.AppendChild(this.CreateElement(doc, "country", prepaidCard.Country));

            XmlElement amount = this.CreateElement(doc, "amount", transactionAmount.ToString("#0.00").Replace(".", ""));
            attr = doc.CreateAttribute("currencycode");
            attr.Value = this.Currency.ToUpper();
            amount.Attributes.Append(attr);
            billing.AppendChild(amount);
            billing.AppendChild(this.CreateElement(doc, "email", prepaidCard.Email));
            //if (order.ParentId != null)
            //{
            //    XmlElement subscription = this.CreateElement(doc, "subscription");
            //    attr = doc.CreateAttribute("type");
            //    attr.Value = "RECURRING";
            //    subscription.Attributes.Append(attr);
            //    subscription.AppendChild(this.CreateElement(doc, "number", (order.Depth).ToString()));
            //    billing.AppendChild(subscription);
            //}
            request.AppendChild(billing);

            XmlElement operation = this.CreateElement(doc, "operation");
            operation.AppendChild(this.CreateElement(doc, "sitereference", this.SiteId));
            //if (prepaidCard.ParentId != null)
            //{
            //    operation.AppendChild(this.CreateElement(doc, "accounttypedescription", "RECUR"));

            //}
            //else
            //{
                operation.AppendChild(this.CreateElement(doc, "accounttypedescription", "ECOM"));

            //}
            request.AppendChild(operation);

            XmlElement settlement = this.CreateElement(doc, "settlement");
            settlement.AppendChild(this.CreateElement(doc, "settlestatus", (captured ? "0" : "1")));

            if (processor.CaptureDelayHours.HasValue && processor.CaptureDelayHours > 0)
            {
                settlement.AppendChild(this.CreateElement(doc, "settleduedate", DateTime.Today.AddDays(processor.CaptureDelayHours.Value).ToString("yyyy-MM-dd")));
            }

            request.AppendChild(settlement);

            root.AppendChild(request);
            doc.AppendChild(root);

            return doc.InnerXml;
        }

        public Dictionary<string, string> GetHeaderData()
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(string.Format("{0}:{1}", this.Gateway.Username, this.Gateway.Password));
            string key = Convert.ToBase64String(plainTextBytes);
            headers.Add("Authorization", "Basic " + key);
            return headers;
        }

        private Transaction CreateTransaction(Order order, Processor processor, string response, decimal amount)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(response);

            string transactionId = "";
            bool approved = false;
            string message = "";
            string transactionRef = "";

            XmlNode res = doc.SelectSingleNode("//response");
            approved = res.Attributes["type"].Value.ToUpper() != "ERROR";

            if (approved)
            {
                XmlNode settlement = doc.SelectSingleNode("//settlement");
                XmlNode settlementStatus = settlement.SelectSingleNode("settlestatus");

                switch (settlementStatus.InnerText)
                {
                    case "3":
                        try
                        {
                            XmlNode error = doc.SelectSingleNode("//error");
                            message = ParseErrorMessage(error);
                        }
                        catch
                        {
                            message = "Undefined Error";
                        }
                        //message = "Status Cancelled";
                        approved = false;
                        break;
                    default:
                        XmlNode merchant = doc.SelectSingleNode("//merchant");
                        //try {
                            //transactionId = merchant.SelectSingleNode("tid").InnerText;
                            transactionRef = doc.SelectSingleNode("//transactionreference").InnerText;
                        //}
                        //catch{ }
                        break;
                }
            }
            else
            {
                XmlNode error = doc.SelectSingleNode("//error");
                message = ParseErrorMessage(error);
            }

            var currentTransaction = new Transaction()
            {
                Response = response,
                OrderId = order.OrderId,
                Order = order,
                ProcessorId = processor.Id,
                Processor = processor,
                TransactionId = transactionId,
                Status = approved ? TransactionStatus.Approved : TransactionStatus.Declined,
                Success = approved,
                Message = message,
                Date = DateTime.UtcNow,
                Amount = amount,
                TransactionReference = transactionRef
            };

            return currentTransaction;
        }


        private TransactionViaPrepaidCardQueue CreateTransactionPrepaidCard(Processor processor, string response, decimal amount,int prepaidCardId)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(response);

            string transactionId = "";
            bool approved = false;
            string message = "";
            string transactionRef = "";

            XmlNode res = doc.SelectSingleNode("//response");
            approved = res.Attributes["type"].Value.ToUpper() != "ERROR";

            if (approved)
            {
                XmlNode settlement = doc.SelectSingleNode("//settlement");
                XmlNode settlementStatus = settlement.SelectSingleNode("settlestatus");

                switch (settlementStatus.InnerText)
                {
                    case "3":
                        try
                        {
                            XmlNode error = doc.SelectSingleNode("//error");
                            message = ParseErrorMessage(error);
                        }
                        catch
                        {
                            message = "Undefined Error";
                        }
                        //message = "Status Cancelled";
                        approved = false;
                        break;
                    default:
                        XmlNode merchant = doc.SelectSingleNode("//merchant");
                        //try {
                        //transactionId = merchant.SelectSingleNode("tid").InnerText;
                        transactionRef = doc.SelectSingleNode("//transactionreference").InnerText;
                        //}
                        //catch{ }
                        break;
                }
            }
            else
            {
                XmlNode error = doc.SelectSingleNode("//error");
                message = ParseErrorMessage(error);
            }

            var currentTransaction = new TransactionViaPrepaidCardQueue()
            {
                Response = response,
                PrepaidCardId = prepaidCardId,
                ProcessorId = processor.Id,
                Processor = processor,
                TransactionId = transactionId,
                Status = approved ? TransactionStatus.Approved : TransactionStatus.Declined,
                Success = approved,
                Message = message,
                Date = DateTime.UtcNow,
                Amount = amount,
                TransactionReference = transactionRef
            };

            return currentTransaction;
        }

        private string ParseErrorMessage(XmlNode error)
        {
            string dataString = "";
            foreach (XmlNode data in error.SelectNodes("data"))
            {
                if (dataString == "")
                {
                    dataString += data.InnerText;
                }
                else
                {
                    dataString += ":" + data.InnerText;
                }
            }
            return error.SelectSingleNode("message").InnerText + ": " + dataString;
        }
        private Transaction CreateTransactionUpdate(Order order, Processor processor, string response)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(response);

            string transactionId = "";
            bool approved = false;
            string message = "";
            string transactionRef = "";

            XmlNode res = doc.SelectSingleNode("//response");
            approved = res.Attributes["type"].Value.ToUpper() != "ERROR";

            if (approved)
            {                
                transactionRef = doc.SelectSingleNode("//requestreference").InnerText;
            }
            else
            {
                XmlNode error = doc.SelectSingleNode("//error");
                message = error.SelectSingleNode("message").InnerText + ": " + error.SelectSingleNode("data").InnerText;
            }

            var currentTransaction = new Transaction()
            {
                Response = response,
                OrderId = order.OrderId,
                Order = order,
                ProcessorId = processor.Id,
                Processor = processor,
                TransactionId = transactionId,
                Status = approved ? TransactionStatus.Approved : TransactionStatus.Declined,
                Success = approved,
                Message = message,
                Date = DateTime.UtcNow,
                Amount = order.Total,
                TransactionReference = transactionRef
            };

            return currentTransaction;
        }

        private XmlDocument CreateTransactionUpdateRefund(decimal newamount, Transaction transaction)
        {
            XmlDocument doc = new XmlDocument();

            XmlElement root = this.CreateElement(doc, "requestblock");
            var attr = doc.CreateAttribute("version");
            attr.Value = "3.67";
            root.Attributes.Append(attr);

            root.AppendChild(this.CreateElement(doc, "alias", this.Gateway.Username));

            XmlElement request = this.CreateElement(doc, "request");
            attr = doc.CreateAttribute("type");
            attr.Value = "TRANSACTIONUPDATE";
            request.Attributes.Append(attr);

            XmlElement filter = this.CreateElement(doc, "filter");
            filter.AppendChild(this.CreateElement(doc, "sitereference", this.SiteId));
            filter.AppendChild(this.CreateElement(doc, "transactionreference", transaction.TransactionReference));
            request.AppendChild(filter);

            XmlElement updates = this.CreateElement(doc, "updates");

            XmlElement settlement = this.CreateElement(doc, "settlement");
            settlement.AppendChild(this.CreateElement(doc, "settlebaseamount", newamount.ToString("#0.00").Replace(".", "")));
            updates.AppendChild(settlement);

            request.AppendChild(updates);
            root.AppendChild(request);
            doc.AppendChild(root);
            return doc;
        }
    }
}
