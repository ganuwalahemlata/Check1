using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Http.ValueProviders;
using KontinuityCRM.Models.Enums;

namespace KontinuityCRM.Models
{
    //[ValueProvider()]
    public class OrderSearch
    {
        public OrderSearch() {
            this.fStatus = OrderStatus.Approved;
        }
        /// <summary>
        /// Date Range for Created Date of Order
        /// </summary>
        [Display(Name = "From")]
        public DateTime? fFromDate { get; set; }
        /// <summary>
        /// Date Range for Created Date of Order
        /// </summary>
        [Display(Name = "To")]
        public DateTime? fToDate { get; set; }

        /// <summary>
        /// Orders having Customer with CustomerId as fCustomerId
        /// </summary>
        [Display(Name = "Customer Id")]
        public int? fCustomerId { get; set; }
        /// <summary>
        /// Search with OrderId
        /// </summary>
        [Display(Name = "Order Id")]
        public string fOrderId { get; set; }
        
        //public ICollection<int> oid { get; set; }
        /// <summary>
        /// Search with AffiliateId of Order
        /// </summary>
        [Display(Name = "Affiliate Id")]
        public string fAffiliateId { get; set; }
        /// <summary>
        /// Search Order with product having product Id as fProductId
        /// </summary>
        [Display(Name = "Product Id")]
        public int? fProductId { get; set; }
        /// <summary>
        /// Search Order with Status 
        /// </summary>
        public OrderStatus fStatus { get; set; }

        /// <summary>
        /// Search Order with SubId
        /// </summary>
        [Display(Name = "Sub Id")]
        public string fSubId { get; set; }
        /// <summary>
        /// Search Order with FirstName
        /// </summary>
        public string fFirstname { get; set; }
        /// <summary>
        /// Search Order with LastName
        /// </summary>
        public string fLastname { get; set; }
        /// <summary>
        /// Search Order with Email
        /// </summary>
        public string fEmail { get; set; }
        /// <summary>
        /// Search Order Address
        /// </summary>
        public string fAddress { get; set; }

        /// <summary>
        /// Search Order with Address 2
        /// </summary>
        public string fAddress2 { get; set; }
        /// <summary>
        /// Search Order with City
        /// </summary>
        public string fCity { get; set; }
        /// <summary>
        /// Search Order with ZIP
        /// </summary>
        public string fZIP { get; set; }
        /// <summary>
        /// Search Order with Phone
        /// </summary>
        public string fPhone { get; set; }
        /// <summary>
        /// Search Order with State
        /// </summary>
        public string fState { get; set; }
        /// <summary>
        /// Search Order with Country
        /// </summary>
        public string fCountry { get; set; }
        /// <summary>
        /// Search Order with IP
        /// </summary>
        public string fIP { get; set; }
        /// <summary>
        /// Search Order with RMA
        /// </summary>
        public string fRMA { get; set; }
        /// <summary>
        /// Search Order with Shipped
        /// </summary>
        public bool? fShipped { get; set; }
        /// <summary>
        /// User Action on Selected Orders
        /// </summary>
        public UserAction UserAction { get; set; }
        /// <summary>
        /// Serch Order with TransactionId
        /// </summary>
        public string fTransactionId { get; set; }

        /// <summary>
        /// Serch Order with CreditCardNumber
        /// </summary>
        public string fCreditCardNumber { get; set; }


        /// <summary>
        /// Serch Order with BIN
        /// </summary>
        public string fBIN { get; set; }

        /// <summary>
        /// Serch Order with LastFour
        /// </summary>
        public string fLastFour { get; set; }

        /// <summary>
        /// Serch Order with Recurring
        /// </summary>
        public string Recurring { get; set; }

        /// <summary>
        /// This is the orders result set for this search
        /// </summary>
        public virtual PagedList.IPagedList<Order> Orders { get; set; }

    }

    public enum UserAction
    {
        filter,
        detail,
        delete,
        rebill,
        neworder,
        Void,
        refund,
        reattempt,
        start,
        batch,
        stop,
        export,
    }    

    public class OrderBatch
    {
        [Display(Name = "Shipping First Name")]
        public string ShippingFirstName { get; set; }
       
        [Display(Name = "Shipping Last Name")]
        public string ShippingLastName { get; set; }
        
        [Display(Name = "Shipping Address")]
        public string ShippingAddress1 { get; set; }

        [Display(Name = "Shipping Address 2")]
        public string ShippingAddress2 { get; set; }
        
        [Display(Name = "Shipping City")]
        public string ShippingCity { get; set; }

        [Display(Name = "Shipping Province")]
        public string ShippingProvince { get; set; }
        
        [Display(Name = "Shipping Postal Code")]
        public string ShippingPostalCode { get; set; }
        
        [Display(Name = "Shipping Country")]
        public string ShippingCountry { get; set; }
        
        public string Phone { get; set; } // shipping or billing client ?
       
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "The e-mail address isn't in a correct format")]
        public string Email { get; set; }
        
        [Display(Name = "Billing First Name")]
        public string BillingFirstName { get; set; }
        
        [Display(Name = "Billing Last Name")]
        public string BillingLastName { get; set; }
        
        [Display(Name = "Billing Address1")]
        public string BillingAddress1 { get; set; }

        [Display(Name = "Billing Address2")]
        public string BillingAddress2 { get; set; }
       
        [Display(Name = "Billing City")]
        public string BillingCity { get; set; }

        [Display(Name = "Billing Province")]
        public string BillingProvince { get; set; }
        
        [Display(Name = "Billing Postal Code")]
        public string BillingPostalCode { get; set; }
        
        [Display(Name = "Billing Country")]
        public string BillingCountry { get; set; }

        [Display(Name = "Shipping Method")]
        public int? ShippingMethodId { get; set; }

        public int? ProcessorId { get; set; }

        public bool? Shipped { get; set; }

        //public string IPAddress { get; set; }

        [Display(Name = "Affiliate Id")]
        public string AffiliateId { get; set; }

        [Display(Name = "Sub Id")]
        public string SubId { get; set; }

        [Display(Name = "Chargeback Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? ChargebackDate { get; set; }

        //[Display(Name = "Payment Type")]
        //public PaymentType? PaymentType { get; set; }
        
        [Display(Name = "Credit Card Number")]
        [CreditCard]
        [DataType(DataType.CreditCard)]
        public string CreditCardNumber { get; set; }

        [Display(Name = "Credit Card Expiration Month")]
        public Month? CreditCardExpirationMonth { get; set; }

        [Display(Name = "Credit Card Expiration Year")]
        public int? CreditCardExpirationYear { get; set; }
       
        [Display(Name = "Credit Card CVV")]
        public string CreditCardCVV { get; set; }

        //[Index]
        //public OrderStatus Status { get; set; }

        public int? BalancerId { get; set; }
        

    }
}