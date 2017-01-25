using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KontinuityCRM.Models;
using System.Globalization;
using System.Net.Mail;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Net.Mime;
using System.Text;
using KontinuityCRM.Models.Enums;
using KontinuityCRM.Models.Gateways;

namespace KontinuityCRM.Helpers
{
    public static class KontinuityCRMHelper
    {
        //public static void DecodeBillValue(string billvalue, out BillDay billday, out DayOfWeek dayofweek)
        //{
        //    var arr = billvalue.Split('-');
        //    billday = EnumHelper<BillDay>.Parse(arr[0]);
        //    dayofweek = EnumHelper<DayOfWeek>.Parse(arr[1]);
        //}
        /// <summary>
        /// Get Bill Day
        /// </summary>
        /// <param name="billvalue">Bill Value</param>
        /// <returns></returns>
        public static BillDay GetBillDay(int billvalue)
        {
            return ((BillDay)(billvalue / 10));
        }
        /// <summary>
        /// Get Date of Week
        /// </summary>
        /// <param name="billvalue">Bill Value</param>
        /// <returns></returns>
        public static DayOfWeek GetDateOfWeek(int billvalue)
        {
            return ((DayOfWeek)(billvalue % 10));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="billday"></param>
        /// <param name="dayofweek"></param>
        /// <returns></returns>
        public static int GetBillValue(BillDay billday, DayOfWeek dayofweek)
        {
            return (int)billday * 10 + (int)dayofweek;
        }

        private static DateTime NextDate(DateTime currentdatetime, DayOfWeek dayofweek, BillDay billday)
        {
            int year = currentdatetime.Year;
            int month = currentdatetime.Month;
            var nextdate = GetDate(dayofweek, billday, year, month);

            if (currentdatetime < nextdate)
            {
                return nextdate;
            }
            month = month == 12 ? 1 : month + 1;
            year = month == 1 ? year + 1 : year;
            return GetDate(dayofweek, billday, year, month);
        }
        /// <summary>
        /// PopulateTokens populates email template tokens with their order values
        /// </summary>
        /// <param name="template"></param>
        /// <param name="order"></param>
        /// <param name="op"></param>
        /// <param name="processor"></param>
        /// <param name="bp"></param>
        /// <param name="gatewayModel"></param>
        /// <param name="balancerId"></param>
        /// <returns></returns>
        public static string PopulateTokens(string template, Order order, OrderProduct op, Processor processor, BalancerProcessor bp, GatewayModel gatewayModel, int? balancerId)
        {
            //TODO use nVelocity template engine for ability to code in templates themselves
            var product = op.Product;

            StringBuilder sb = new StringBuilder(template);
            sb.Replace("{billingaddress2}", order.BillingAddress2); //Customer's billing address 2
            sb.Replace("{billingaddress}", order.BillingAddress1);
            sb.Replace("{billingcity}", order.BillingCity);
            sb.Replace("{billingcountry}", order.BillingCountry);
            sb.Replace("{billingfirstname}", order.BillingFirstName);
            sb.Replace("{billinglastname}", order.BillingLastName);
            sb.Replace("{billingstate}", order.BillingProvince);
            sb.Replace("{billingzipcode}", order.BillingPostalCode);
            sb.Replace("{checkinglastfour}", order.CreditCardNumber);       // Customer's checking account number
            sb.Replace("{customerid}", order.CustomerId.ToString());

            // Should we use the processor or the gatewayModel ????
            sb.Replace("{customerservicenumber}", processor.CustomerServiceNumber); // Gateway customer service number

            // Should we use the processor or the gatewayModel ????
            sb.Replace("{descriptor}", processor.Descriptor);       // Gateway descriptor
            sb.Replace("{processor_specific_text}", processor.ProcessorSpecificText);

            sb.Replace("{emailaddress}", order.Email); // Customer's email address
            sb.Replace("{gatewayalias}", gatewayModel.Name);

            //sb.Replace("{gateway_specific_html}", order.CustomerId.ToString());
            //sb.Replace("{gateway_specific_text}", order.CustomerId.ToString());

            //sb.Replace("{giroaccountnumber}", order.CustomerId.ToString()); // Giro account number
            //sb.Replace("{girobankaddress}", order.CustomerId.ToString()); // Giro bank address
            //sb.Replace("{girobankname}", order.CustomerId.ToString()); // Giro bank name
            //sb.Replace("{giroiban}", order.CustomerId.ToString()); // Giro IBAN
            //sb.Replace("{girosortcode}", order.CustomerId.ToString()); // Giro sort code
            //sb.Replace("{giroswift}", order.CustomerId.ToString()); // Giro Swift code

            //sb.Replace("{nextsubscriptionamount}", order.Email); // Next subscription order amount total

            sb.Replace("{nextsubscriptiondate}", op.NextDate.ToString()); // Next subscription billing date
            sb.Replace("{nextsubscriptionproduct}", op.NextProduct == null ? string.Empty : op.NextProduct.Name); // Next subscription product name
            sb.Replace("{orderdatetimecustom}", order.Created.ToString()); // Order purchase date and time, using custom date format
            sb.Replace("{orderdatetime}", order.Created.ToString()); // Order purchase date and time
            sb.Replace("{orderid}", order.OrderId.ToString()); // Order identifer
            sb.Replace("{ordertotal}", order.Email); // Order total price
            sb.Replace("{phone}", order.Phone); // Customer's telephone number
            sb.Replace("{productdescription}", product.Description); // The description for the main product of the order
            sb.Replace("{productname}", product.Name); // List of products purchased
                                                       //sb.Replace("{producturl}", product.Name); // Product digitally delivery url

            //sb.Replace("{product_attributes}", order.BillingCity); // Product attributes associated with the main product of the order.
            //sb.Replace("{product_summary_text}", order.BillingCity); // Products, price and quantity breakdownin plain text
            //sb.Replace("{product_summary}", order.BillingCity); // Products, price and quantity breakdownin HTML
            //sb.Replace("{refundamount}", order.BillingCity); // Order refunded amount
            //sb.Replace("{retrydiscountamt}", order.BillingCity); // The amount discount applied by a decline salvage retry attempt.
            //sb.Replace("{retrydiscountpct}", order.BillingCity); // The percent discount applied by a decline salvage retry attempt.
            sb.Replace("{rmanumber}", op.RMAReasonId.HasValue ? string.Format("{0}-{1}", op.OrderId, op.ProductId) : string.Empty); // Order rma number
                                                                                                                                    //sb.Replace("{routinglastfour}", order.BillingCity); // Customer's routing number
                                                                                                                                    //sb.Replace("{salestax}", order.BillingCity);  // Sales Tax Amount and Percentage

            sb.Replace("{shippingaddress2}", order.ShippingAddress2);       //Customer's billing address 2
            sb.Replace("{shippingaddress}", order.ShippingAddress1);
            sb.Replace("{shippingcity}", order.ShippingCity);
            sb.Replace("{shippingcountry}", order.ShippingCountry);
            sb.Replace("{shippingfirstname}", order.ShippingFirstName);
            //sb.Replace("{shippinggroup}", order.BillingLastName);
            sb.Replace("{shippinglastname}", order.ShippingLastName);

            sb.Replace("{shippingprice}", order.ShippingMethod.Price.ToString());

            sb.Replace("{shippingstate}", order.ShippingProvince);  // Customer's checking account number
            sb.Replace("{shippingzipcode}", order.ShippingPostalCode);

            sb.Replace("{subtotal}", order.SubTotal.ToString());
            sb.Replace("{salestax}", order.Tax.ToString());
            //sb.Replace("{taxfactor}", order.ShippingLastName);
            //sb.Replace("{telnaaccountid}", order.ShippingLastName);
            //sb.Replace("{telnaiccid}", order.ShippingLastName);
            //sb.Replace("{telnaphonenumber}", order.ShippingLastName);
            //sb.Replace("{totalnontaxable}", order.ShippingLastName);

            //sb.Replace("{totaltaxable}", order.ShippingLastName);
            //sb.Replace("{tpl_product_attributes}", order.ShippingLastName);
            //sb.Replace("{tpl_product_base_cost}", order.ShippingLastName);
            //sb.Replace("{tpl_product_id}", order.ShippingLastName);
            //sb.Replace("{tpl_product_name}", order.ShippingLastName);
            //sb.Replace("{tpl_product_quantity}", order.ShippingLastName);

            //sb.Replace("{tpl_product_row}", order.ShippingLastName);
            //sb.Replace("{tpl_product_total_cost}", order.ShippingLastName);
            //sb.Replace("{trackingnumber}", order.ShippingLastName);
            //sb.Replace("{voidamount}", order.ShippingLastName);
            return sb.ToString();
        }


        /// <summary>
        /// PopulateTokens populates email template tokens with their order values
        /// </summary>
        /// <param name="template"></param>
        /// <param name="order"></param>
        /// <param name="op"></param>
        /// <param name="processor"></param>
        /// <param name="bp"></param>
        /// <param name="gatewayModel"></param>
        /// <param name="balancerId"></param>
        /// <returns></returns>
        public static string PopulateTokens(string template, Order order, List<OrderProduct> op, Processor processor, BalancerProcessor bp, GatewayModel gatewayModel, int? balancerId)
        {
            //TODO use nVelocity template engine for ability to code in templates themselves
            var product = op.FirstOrDefault().Product;

            StringBuilder sb = new StringBuilder(template);
            sb.Replace("{billingaddress2}", order.BillingAddress2); //Customer's billing address 2
            sb.Replace("{billingaddress}", order.BillingAddress1);
            sb.Replace("{billingcity}", order.BillingCity);
            sb.Replace("{billingcountry}", order.BillingCountry);
            sb.Replace("{billingfirstname}", order.BillingFirstName);
            sb.Replace("{billinglastname}", order.BillingLastName);
            sb.Replace("{billingstate}", order.BillingProvince);
            sb.Replace("{billingzipcode}", order.BillingPostalCode);
            sb.Replace("{checkinglastfour}", order.CreditCardNumber);       // Customer's checking account number
            sb.Replace("{customerid}", order.CustomerId.ToString());

            // Should we use the processor or the gatewayModel ????
            sb.Replace("{customerservicenumber}", processor.CustomerServiceNumber); // Gateway customer service number

            // Should we use the processor or the gatewayModel ????
            sb.Replace("{descriptor}", processor.Descriptor);       // Gateway descriptor

            sb.Replace("{emailaddress}", order.Email); // Customer's email address
            sb.Replace("{gatewayalias}", gatewayModel.Name);

            //sb.Replace("{gateway_specific_html}", order.CustomerId.ToString());
            //sb.Replace("{gateway_specific_text}", order.CustomerId.ToString());

            //sb.Replace("{giroaccountnumber}", order.CustomerId.ToString()); // Giro account number
            //sb.Replace("{girobankaddress}", order.CustomerId.ToString()); // Giro bank address
            //sb.Replace("{girobankname}", order.CustomerId.ToString()); // Giro bank name
            //sb.Replace("{giroiban}", order.CustomerId.ToString()); // Giro IBAN
            //sb.Replace("{girosortcode}", order.CustomerId.ToString()); // Giro sort code
            //sb.Replace("{giroswift}", order.CustomerId.ToString()); // Giro Swift code

            //sb.Replace("{nextsubscriptionamount}", order.Email); // Next subscription order amount total

            sb.Replace("{nextsubscriptiondate}", op.FirstOrDefault().NextDate.ToString()); // Next subscription billing date
            sb.Replace("{nextsubscriptionproduct}", op.FirstOrDefault().NextProduct == null ? string.Empty : op.FirstOrDefault().NextProduct.Name); // Next subscription product name
            sb.Replace("{orderdatetimecustom}", order.Created.ToString()); // Order purchase date and time, using custom date format
            sb.Replace("{orderdatetime}", order.Created.ToString()); // Order purchase date and time
            sb.Replace("{orderid}", order.OrderId.ToString()); // Order identifer
            sb.Replace("{ordertotal}", order.Email); // Order total price
            sb.Replace("{phone}", order.Phone); // Customer's telephone number
            sb.Replace("{productdescription}", product.Description); // The description for the main product of the order
            sb.Replace("{productname}", product.Name); // List of products purchased
            //sb.Replace("{producturl}", product.Name); // Product digitally delivery url

            //sb.Replace("{product_attributes}", order.BillingCity); // Product attributes associated with the main product of the order.
            //sb.Replace("{product_summary_text}", order.BillingCity); // Products, price and quantity breakdownin plain text
            //sb.Replace("{product_summary}", order.BillingCity); // Products, price and quantity breakdownin HTML
            //sb.Replace("{refundamount}", order.BillingCity); // Order refunded amount
            //sb.Replace("{retrydiscountamt}", order.BillingCity); // The amount discount applied by a decline salvage retry attempt.
            //sb.Replace("{retrydiscountpct}", order.BillingCity); // The percent discount applied by a decline salvage retry attempt.
            sb.Replace("{rmanumber}", op.FirstOrDefault().RMAReasonId.HasValue ? string.Format("{0}-{1}", op.FirstOrDefault().OrderId, op.FirstOrDefault().ProductId) : string.Empty); // Order rma number
            //sb.Replace("{routinglastfour}", order.BillingCity); // Customer's routing number
            //sb.Replace("{salestax}", order.BillingCity);  // Sales Tax Amount and Percentage

            sb.Replace("{shippingaddress2}", order.ShippingAddress2);       //Customer's billing address 2
            sb.Replace("{shippingaddress}", order.ShippingAddress1);
            sb.Replace("{shippingcity}", order.ShippingCity);
            sb.Replace("{shippingcountry}", order.ShippingCountry);
            sb.Replace("{shippingfirstname}", order.ShippingFirstName);
            //sb.Replace("{shippinggroup}", order.BillingLastName);
            sb.Replace("{shippinglastname}", order.ShippingLastName);

            sb.Replace("{shippingprice}", order.ShippingMethod.Price.ToString());

            sb.Replace("{shippingstate}", order.ShippingProvince);  // Customer's checking account number
            sb.Replace("{shippingzipcode}", order.ShippingPostalCode);

            sb.Replace("{subtotal}", order.SubTotal.ToString());
            sb.Replace("{salestax}", order.Tax.ToString());
            //sb.Replace("{taxfactor}", order.ShippingLastName);
            //sb.Replace("{telnaaccountid}", order.ShippingLastName);
            //sb.Replace("{telnaiccid}", order.ShippingLastName);
            //sb.Replace("{telnaphonenumber}", order.ShippingLastName);
            //sb.Replace("{totalnontaxable}", order.ShippingLastName);

            //sb.Replace("{totaltaxable}", order.ShippingLastName);
            //sb.Replace("{tpl_product_attributes}", order.ShippingLastName);
            //sb.Replace("{tpl_product_base_cost}", order.ShippingLastName);
            //sb.Replace("{tpl_product_id}", order.ShippingLastName);
            //sb.Replace("{tpl_product_name}", order.ShippingLastName);
            //sb.Replace("{tpl_product_quantity}", order.ShippingLastName);

            //sb.Replace("{tpl_product_row}", order.ShippingLastName);
            //sb.Replace("{tpl_product_total_cost}", order.ShippingLastName);
            //sb.Replace("{trackingnumber}", order.ShippingLastName);
            //sb.Replace("{voidamount}", order.ShippingLastName);
            string CurrencySymbol = "";
            foreach (var item in GetAllCultureInfo())
            {
                if (item.ISOCurrencySymbol as string == processor.Currency)
                {
                    CurrencySymbol = item.CurrencySymbol as string;
                    break;
                }
            }

            StringBuilder stringToPass = new StringBuilder();
            stringToPass.Append("<table style=\"width:100%; text-align:left\"><thead><tr><th>Product Name</th><th>Quantity</th><th>Price</th></tr></thead> <tbody>");
            foreach (var item in op)
            {
                stringToPass.Append("<tr>");
                stringToPass.Append(string.Format("{0} {1} {2}", "<td>", item.Product.Name, "</td>"));
                stringToPass.Append(string.Format("{0} {1} {2}", "<td>", item.Quantity, "</td>"));
                stringToPass.Append(string.Format("{0} {1} {2}", "<td>", string.Format("{0}{1}", CurrencySymbol, item.Price), "</td>"));
                stringToPass.Append("</tr>");
            }
            stringToPass.Append("<tr>");
            stringToPass.Append(string.Format("{0} {1} {2}", "<td>", "&nbsp;", "</td>"));
            stringToPass.Append(string.Format("{0} {1} {2}", "<td><b>", "Subtotal:", "</b></td>"));
            stringToPass.Append(string.Format("{0} {1} {2}", "<td>", string.Format("{0}{1}", CurrencySymbol, order.SubTotal), "</td>"));
            stringToPass.Append("</tr>");

            stringToPass.Append("<tr>");
            stringToPass.Append(string.Format("{0} {1} {2}", "<td>", "&nbsp;", "</td>"));
            stringToPass.Append(string.Format("{0} {1} {2}", "<td><b>", "Shipping:", "</b></td>"));
            stringToPass.Append(string.Format("{0} {1} {2}", "<td>", string.Format("{0}{1}", CurrencySymbol, order.ShippingMethod.Price.ToString()), "</td>"));
            stringToPass.Append("</tr>");

            stringToPass.Append("<tr>");
            stringToPass.Append(string.Format("{0} {1} {2}", "<td>", "&nbsp;", "</td>"));
            stringToPass.Append(string.Format("{0} {1} {2}", "<td><b>", "Sales Tax:", "</b></td>"));
            stringToPass.Append(string.Format("{0} {1} {2}", "<td>", string.Format("{0}{1}", CurrencySymbol, order.Tax.ToString()), "</td>"));
            stringToPass.Append("</tr>");
            stringToPass.Append("<tr>");
            stringToPass.Append(string.Format("{0} {1} {2}", "<td>", "&nbsp;", "</td>"));
            stringToPass.Append(string.Format("{0} {1} {2}", "<td><b>", "Total Billed:", "</b></td>"));
            stringToPass.Append(string.Format("{0} {1} {2}", "<td>", string.Format("{0}{1}", CurrencySymbol, order.Total), "</td>"));
            stringToPass.Append("</tr>");

            stringToPass.Append("</tbody></table>");
            //{multiple_product_summary}
            sb.Replace("{multiple_product_summary}", stringToPass.ToString());

            return sb.ToString();
        }
        /// <summary>
        /// Populate Tokens
        /// </summary>
        /// <param name="template">template</param>
        /// <param name="replaceableValue">replaceable Value</param>
        /// <returns></returns>
        public static string PopulateTokens(string template, string replaceableValue)
        {
            StringBuilder sb = new StringBuilder(template);
            sb.Replace("{ forgotpasswordlink }", replaceableValue);
            return sb.ToString();
        }

        // Generic method for sending emails
        /// <summary>
        /// Send Mail
        /// </summary>
        /// <param name="from">From</param>
        /// <param name="to">To</param>
        /// <param name="subject">Subject</param>
        /// <param name="body">Body</param>
        /// <param name="cc">CC</param>
        /// <param name="isBodyHtml">Body Html</param>
        public static void SendMail(string from, string to, string subject, string body, MailAddressCollection cc = null, bool isBodyHtml = false)
        {
            // Configure mail client

            SmtpClient mailClient = new SmtpClient(KontinuityCRMConfiguration.MailServer, KontinuityCRMConfiguration.MailPort);
            // Set credentials (for SMTP servers that require authentication)
            mailClient.Credentials = new NetworkCredential(KontinuityCRMConfiguration.MailUsername, KontinuityCRMConfiguration.MailPassword);
            // Enable SSL
            mailClient.EnableSsl = KontinuityCRMConfiguration.MailEnableSsl;
            // Create the mail message
            MailMessage mailMessage = new MailMessage(from, to, subject, body);

            if (cc != null)
            {
                foreach (var address in cc)
                {
                    mailMessage.CC.Add(address);
                }
            }

            mailMessage.IsBodyHtml = isBodyHtml;

            // Send mail
            mailClient.Send(mailMessage);
        }
       
        // send mail method
        /// <summary>
        /// Send Mail
        /// </summary>
        /// <param name="MailServer">Mail Server</param>
        /// <param name="MailPort">Port</param>
        /// <param name="MailUsername">User Name</param>
        /// <param name="MailPassword">Password</param>
        /// <param name="MailEnableSsl">Enable SSl</param>
        /// <param name="from">From</param>
        /// <param name="to">To</param>
        /// <param name="subject">Subject</param>
        /// <param name="textBody">Text Body</param>
        /// <param name="htmlBody">html Body</param>
        /// <param name="cc">CC</param>
        public static void SendMail(string MailServer, int MailPort, string MailUsername, string MailPassword, bool MailEnableSsl,
            string from, string to, string subject, string textBody, string htmlBody, MailAddressCollection cc = null)
        {
            SmtpClient mailClient = new SmtpClient(MailServer, MailPort);
            // Set credentials (for SMTP servers that require authentication)
            mailClient.Credentials = new NetworkCredential(MailUsername, MailPassword);
            // Enable SSL
            mailClient.EnableSsl = MailEnableSsl;
            // Create the mail message
            MailMessage mailMessage = new MailMessage(from, to, subject, textBody);
            ContentType mimeType = new System.Net.Mime.ContentType("text/html");
            AlternateView alternate = AlternateView.CreateAlternateViewFromString(htmlBody, mimeType);
            mailMessage.AlternateViews.Add(alternate);
            if (cc != null)
            {
                foreach (var address in cc)
                {
                    mailMessage.CC.Add(address);
                }
            }
            //mailMessage.IsBodyHtml = isBodyHtml;
            // Send mail

            mailClient.Send(mailMessage);
        }
        /// <summary>
        /// Get Date
        /// </summary>
        /// <param name="dayName">Day</param>
        /// <param name="billday">Bill Day</param>
        /// <param name="year">year</param>
        /// <param name="month">Month</param>
        /// <returns></returns>
        private static DateTime GetDate(DayOfWeek dayName, BillDay billday, int year, int month)
        {
            CultureInfo ci = new CultureInfo("en-US");
            List<DateTime> list = new List<DateTime>();
            for (int i = 1; i <= ci.Calendar.GetDaysInMonth(year, month); i++)
            {
                var date = new DateTime(year, month, i);
                if (date.DayOfWeek == dayName)
                    list.Add(date);
            }

            try
            {
                switch (billday)
                {
                    case BillDay.First:
                        return list[0];
                    case BillDay.Second:
                        return list[1];
                    case BillDay.Third:
                        return list[2];
                    case BillDay.Fourth:
                        return list[3];
                    default:
                        return list[list.Count - 1];
                }
            }
            catch
            {

                return list[list.Count - 1];
            }
        }

        /// <summary>
        /// Calculates the next date from the currentnextdate given with a bill type and value(decline or subscription rule)
        /// </summary>
        /// <param name="currentnextdate"></param>
        /// <param name="billtype"></param>
        /// <param name="billvalue"></param>
        /// <returns></returns>
        public static DateTime GetNextDate(DateTime currentnextdate, BillType billtype, int billvalue)
        {
            int days = 0;
            DateTime result;
            //DateTime result;
            //var nextproduct = this.NextProduct;

            // update next date
            switch (billtype)
            {
                case BillType.ByCycle: // days to the next bill
                    days = billvalue;
                    result = currentnextdate.AddDays(days);
                    break;

                case BillType.ByDate: // nth day of the month
                    //
                    int nth = billvalue;

                    if (currentnextdate.Day > nth)
                    {
                        // try - catch go here
                        result = new DateTime(
                            currentnextdate.Month == 12 ? currentnextdate.Year + 1 : currentnextdate.Year,
                            currentnextdate.Month == 12 ? 1 : currentnextdate.Month + 1, nth);

                    }
                    else
                    {
                        //days = nth - today.Day;
                        // try - catch go here
                        result = new DateTime(currentnextdate.Year, currentnextdate.Month, nth);
                    }

                    break;
                default: // first - sundays

                    var billday = GetBillDay(billvalue);
                    var dayofweek = GetDateOfWeek(billvalue);

                    result = KontinuityCRMHelper.NextDate(currentnextdate, dayofweek, billday);

                    break;
            }

            return result;

        }
        /// <summary>
        /// Subscription Description
        /// </summary>
        /// <param name="billtype">Bill Type</param>
        /// <param name="billvalue">Bill Value</param>
        /// <returns></returns>
        public static string SubscriptionDescription(BillType billtype, int billvalue)
        {
            switch (billtype)
            {
                case BillType.ByDate:
                    int value = billvalue;
                    var th = value > 3 ? "th" : value == 1 ? "st" : value == 2 ? "nd" : "rd";
                    return string.Format("Bills on the {0}{1} day of every month", billvalue, th);
                case BillType.ByDay:
                    BillDay day = GetBillDay(billvalue);
                    DayOfWeek dow = GetDateOfWeek(billvalue);
                    return string.Format("Bills the {0} {1} of every month", day.ToString().ToLower(), dow);
                default:
                    value = billvalue;
                    return value > 1 ? string.Format("Bills every {0} days", billvalue) : "Bills every day";
            }
        }
        /// <summary>
        /// Convert object to byte array
        /// </summary>
        /// <param name="obj">object</param>
        /// <returns></returns>
        public static byte[] ObjectToByteArray(Object obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }
        /// <summary>
        /// Byte Array  to object
        /// </summary>
        /// <param name="arrBytes"></param>
        /// <returns></returns>
        public static Object ByteArrayToObject(byte[] arrBytes)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            Object obj = (Object)binForm.Deserialize(memStream);
            return obj;
        }
        /// <summary>
        /// Get Gateway Type Name
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetGatewayTypeName(string type)
        {
            return Type.GetType("KontinuityCRM.Models.Gateways." + type).DisplayClassName();
        }
        /// <summary>
        /// Calculate All Permissions
        /// </summary>
        /// <param name="assembly">Assembly</param>
        /// <param name="type">Type</param>
        /// <returns></returns>
        public static long CalculateAllPermissions(System.Reflection.Assembly assembly, string type)
        {
            var enumtype = assembly.GetType(type);
            long total = 0;
            foreach (object item in Enum.GetValues(enumtype))
            {
                total += Convert.ToInt64(item);
            }
            return total;
        }
        /// <summary>
        /// Cancel Subscription
        /// </summary>
        /// <param name="orderProducts">order Products</param>
        /// <param name="uow"></param>
        /// <param name="wsw"></param>
        /// <param name="timeo"></param>
        public static void CancelSubscription(IEnumerable<OrderProduct> orderProducts, IUnitOfWork uow, IWebSecurityWrapper wsw, DateTimeOffset? timeo = null)
        {
            var time = timeo ?? DateTimeOffset.UtcNow;

            foreach (var op in orderProducts)
            {
                CancelSubscription(op, uow, wsw, time);
            }
        }

        /// <summary>
        /// Cancel the subscription and record a cancelation
        /// </summary>
        /// <param name="op"></param>
        /// <param name="uow"></param>
        /// <param name="time"></param>
        public static void CancelSubscription(OrderProduct op, IUnitOfWork uow, IWebSecurityWrapper wsw, DateTimeOffset time)
        {
            // bugfix #192 - we should only set the recurring false
            //op.NextDate = null;
            op.Recurring = false;
            //op.ReAttempts = 0;

            uow.OrderNoteRepository.Add(new OrderNote
            {
                OrderId = op.OrderId,
                NoteDate = DateTime.UtcNow,
                Note = "Stop Recurring",
            });

            if (wsw != null && wsw.CurrentUserName != null)
            {
                uow.Save(wsw.CurrentUserName);
            }
            else
            {
                uow.Save();
            }
          

            uow.OrderTimeEventRepository.Add(new OrderTimeEvent
            {
                OrderId = op.OrderId,
                ProductId = op.ProductId,
                Time = time,
                Event = OrderEvent.Canceled,
                Action = op.Order.ParentId.HasValue ? op.ReAttempts > 0 : (bool?)null,
                AffiliateId = op.Order.AffiliateId,
                SubId = op.Order.SubId,
            });
        }

        /// <summary>
        /// Get Controller Display Name
        /// </summary>
        /// <param name="name">name</param>
        /// <returns></returns>
        public static string GetControllerDisplayName(string name)
        {
            var type = Type.GetType("KontinuityCRM.Controllers." + name + "Controller", false, true);

            if (type == null)
            {
                return name;
            }

            return type.DisplayClassName();
        }
        /// <summary>
        /// Send Mail
        /// </summary>
        /// <param name="confirmationEvent">confirmation event</param>
        internal static void SendMail(Event confirmationEvent)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Get Culture Info
        /// </summary>
        /// <returns></returns>
        public static List<dynamic> GetAllCultureInfo()
        {
            List<dynamic> cultureList = new List<dynamic>();
            //cultures installed with the .Net Framework
            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures & ~CultureTypes.SpecificCultures);

            foreach (CultureInfo culture in cultures)
            {
                if (culture.LCID != 127 && !culture.IsNeutralCulture)
                {
                    RegionInfo region = new RegionInfo(culture.LCID);
                    if (!(cultureList.Contains(region.EnglishName)))
                        cultureList.Add(new { ISOCurrencySymbol = region.ISOCurrencySymbol, CurrencySymbol = region.CurrencySymbol });
                }
            }
            return cultureList;
        }
        /// <summary>
        /// Get Random Number
        /// </summary>
        /// <returns></returns>
        public static string GetRandomNumber()
        {
            return new Random().Next(100000, 999999).ToString();
        }

    }
}