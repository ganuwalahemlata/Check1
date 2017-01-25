using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KontinuityCRM.Models.APIModels
{
    /// <summary>
    /// Model to create an order through the API
    /// </summary>
    public class OrderCreateModel
    {
        /// <summary>
        /// Cusotmer Id
        /// </summary>
        public int CustomerId { get; set; }
        
        /// <summary>
        /// Shipping First Name
        /// </summary>
        [Required]
        public string ShippingFirstName { get; set; }

        /// <summary>
        /// Shipping Last Name
        /// </summary>
        [Required]
        public string ShippingLastName { get; set; }


        /// <summary>
        /// Shipping first address
        /// </summary>
        /// 
        [Required]
        public string ShippingAddress1 { get; set; }

        /// <summary>
        /// Shipping second address
        /// </summary>
        public string ShippingAddress2 { get; set; }

        /// <summary>
        /// Shipping City
        /// </summary>
        /// 
        [Required]
        public string ShippingCity { get; set; }

        /// <summary>
        /// Shipping Province
        /// </summary>
        /// 
        [Required]
        public string ShippingProvince { get; set; }

        /// <summary>
        /// Shipping PostalCode
        /// </summary>
        /// 
        [Required]
        public string ShippingPostalCode { get; set; }
        /// <summary>
        /// Shipping Country
        /// </summary>
        /// 
        [Required]
        public string ShippingCountry { get; set; }

        /// <summary>
        /// Phone
        /// </summary>
        /// 
        [Required]
        public string Phone { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "The e-mail address isn't in a correct format")]
        [Required]
        public string Email { get; set; }

        /// <summary>
        /// Billing First Name
        /// </summary>
        /// 
        [Required]
        public string BillingFirstName { get; set; }

        /// <summary>
        /// Billing Last Name
        /// </summary>
        /// 
        [Required]
        public string BillingLastName { get; set; }
        /// <summary>
        /// Billing First Address
        /// </summary>
        /// 
        [Required]
        [Display(Name = "Billing Address1")]
        public string BillingAddress1 { get; set; }

        /// <summary>
        /// Billing Second Address
        /// </summary>
        /// 
        [Display(Name = "Billing Address2")]
        public string BillingAddress2 { get; set; }

        /// <summary>
        /// Billing City
        /// </summary>
        /// 
        [Required]
        [Display(Name = "Billing City")]
        public string BillingCity { get; set; }
        /// <summary>
        /// Billing Province
        /// </summary>
        /// 
        [Required]
        [Display(Name = "Billing Province")]
        public string BillingProvince { get; set; }
        /// <summary>
        /// Billing PostalCode
        /// </summary>
        /// 
        [Required]
        [Display(Name = "Billing PostalCode")]
        public string BillingPostalCode { get; set; }

        /// <summary>
        /// Billing Country
        /// </summary>
        /// 
        [Required]
        [Display(Name = "Billing Country")]
        public string BillingCountry { get; set; }

        /// <summary>
        /// Shipping Method Id
        /// </summary>
        [Display(Name = "Shipping Method")]
        [Required]
        [KontinuityCRM.Helpers.ValidateShippingMethod(ErrorMessage="Shipping Method not found")]
        public int ShippingMethodId { get; set; }
        /// <summary>
        /// AffiliateId
        /// </summary>
        [Display(Name = "AffiliateId")]
        public int? AffiliateId { get; set; }
        /// <summary>
        /// SubId
        /// </summary>
        [Display(Name = "SubId")]
        public int? SubId { get; set; }

        /// <summary>
        /// IP address
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// Products in this order
        /// </summary>
        [Required]
        public IEnumerable<OrderProduct> OrderProducts { get; set; }


        #region CreditCard

        /// <summary>
        /// Payment Type
        /// </summary>
        [Display(Name = "Payment Type")]
        public PaymentType PaymentType { get; set; }

        /// <summary>
        /// Credit Card Number
        /// </summary>
        [Required]
        [Display(Name = "Credit Card Number")]
        [KontinuityCRM.Helpers.CustomCreditCard]//[CreditCard]
        [DataType(DataType.CreditCard)]
        public string CreditCardNumber { get; set; }

        /// <summary>
        /// Credit Card Expiration Month
        /// </summary>
        [Display(Name = "Credit Card Expiration Month")]
        [Required]
        public Month CreditCardExpirationMonth { get; set; }

        /// <summary>
        /// Credit Card Expiration Year
        /// </summary>
        [Required]
        [Display(Name = "Credit Card Expiration Year")]
        public int CreditCardExpirationYear { get; set; }

        /// <summary>
        /// Credit Card CVV
        /// </summary>
        [Required]
        [Display(Name = "Credit Card CVV")]
        public string CreditCardCVV { get; set; }

        #endregion

    }
}