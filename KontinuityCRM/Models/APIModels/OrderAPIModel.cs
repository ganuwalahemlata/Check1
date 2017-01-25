using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using KontinuityCRM.Models.Enums;

namespace KontinuityCRM.Models.APIModels
{
    /// <summary>
    /// Model used to retrieve the order data
    /// </summary>
    public class OrderAPIModel
    {
        /// <summary>
        /// Order Id
        /// </summary>
        /// 
        [Display(Name = "Order Id")]
        public int OrderId { get; set; }
        /// <summary>
        /// Customer Id
        /// </summary>
        /// 

        [Display(Name = "Order Create Date")]
        public DateTime Created { get; set; }


        [Display(Name = "Is Test")]
        public bool IsTest { get; set; }

        [Display(Name = "Is Prepaid")]
        public bool IsPrepaid { get; set; }


        [Display(Name = "Shipped")]
        public bool? Shipped { get; set; }

        [Display(Name = "Customer Id")]
        public int CustomerId { get; set; }
        /// <summary>
        /// Shipping FirstName
        /// </summary>
        /// 
        [Display(Name = "Shipping First Name")]
        public string ShippingFirstName { get; set; }
        /// <summary>
        /// Shipping LastName
        /// </summary>
        /// 
        [Display(Name = "Shipping Last Name")]
        public string ShippingLastName { get; set; }
        /// <summary>
        /// Shipping Address
        /// </summary>
        /// 
        [Display(Name = "Shipping Address 1")]
        public string ShippingAddress1 { get; set; }
        /// <summary>
        /// Shipping second address
        /// </summary>
        /// 
        [Display(Name = "Shipping Address 2")]
        public string ShippingAddress2 { get; set; }
        /// <summary>
        /// Shipping City
        /// </summary>
        /// 
        [Display(Name = "Shipping City")]
        public string ShippingCity { get; set; }
        /// <summary>
        /// Shipping Province
        /// </summary>
        /// 
        [Display(Name = "Shipping Province")]
        public string ShippingProvince { get; set; }
        /// <summary>
        /// Shipping PostalCode
        /// </summary>
        /// 
        [Display(Name = "Shipping Postal Code")]
        public string ShippingPostalCode { get; set; }
        /// <summary>
        /// Shipping Country
        /// </summary>
        /// 
        [Display(Name = "Shipping Country")]
        public string ShippingCountry { get; set; }
        /// <summary>
        /// Phone
        /// </summary>
        /// 
        [Display(Name = "Phone")]
        public string Phone { get; set; }
        /// <summary>
        /// Email
        /// </summary>
        /// 
        [Display(Name = "Email")]
        public string Email { get; set; }
        /// <summary>
        /// Billing FirstName
        /// </summary>
        /// 
        [Display(Name = "Billing First Name")]
        public string BillingFirstName { get; set; }
        /// <summary>
        /// Billing LastName
        /// </summary>
        /// 
        [Display(Name = "Billing Last Name")]
        public string BillingLastName { get; set; }
        /// <summary>
        /// Billing Address
        /// </summary>
        [Display(Name = "Billing Address1")]
        public string BillingAddress1 { get; set; }
        /// <summary>
        /// Billing second address
        /// </summary>
        [Display(Name = "Billing Address2")]
        public string BillingAddress2 { get; set; }
        /// <summary>
        /// Billing City
        /// </summary>
        [Display(Name = "Billing City")]
        public string BillingCity { get; set; }
        /// <summary>
        /// Billing Province
        /// </summary>
        [Display(Name = "Billing Province")]
        public string BillingProvince { get; set; }
        /// <summary>
        /// Billing Postal Code
        /// </summary>
        [Display(Name = "Billing Postal Code")]
        public string BillingPostalCode { get; set; }
        /// <summary>
        /// Billing Country
        /// </summary>
        [Display(Name = "Billing Country")]
        public string BillingCountry { get; set; }
        /// <summary>
        /// Shipping MethodId
        /// </summary>
        [Display(Name = "Shipping Method")]
        public int ShippingMethodId { get; set; }
        /// <summary>
        /// Processor Id
        /// </summary>
        [Display(Name = "Processor")]
        public int? ProcessorId { get; set; }
        /// <summary>
        /// AffiliateId
        /// </summary>
        [Display(Name = "Affiliate Id")]
        public string AffiliateId { get; set; }
        /// <summary>
        /// SubId
        /// </summary>
        [Display(Name = "Sub Id")]
        public string SubId { get; set; }

        /// <summary>
        /// Chargeback Date
        /// </summary>
        [Display(Name = "Chargeback Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? ChargebackDate { get; set; }

        /// <summary>
        /// Indicates the parent order id
        /// </summary>
        /// 
        [Display(Name = "Parent Id")]
        public int? ParentId { get; set; }

        /// <summary>
        /// Order Status
        /// </summary>
        /// 
        [Display(Name = "Status")]
        public OrderStatus Status { get; set; }

        /// <summary>
        /// IP address
        /// </summary>
        /// 
        [Display(Name = "IP Address")]
        public string IPAddress { get; set; }

        /// <summary>
        /// Price of all products
        /// </summary>
        /// 
        [Display(Name = "Sub Total")]
        public decimal SubTotal { get; set; }

        /// <summary>
        /// Tax applied to this order
        /// </summary>
        /// 
        [Display(Name = "Tax")]
        public decimal Tax { get; set; }

        /// <summary>
        /// Shipping Price
        /// </summary>
        /// 
        [Display(Name = "Shipping Price")]
        public decimal ShippingPrice { get; set; }

        /// <summary>
        /// Total order amount
        /// </summary>
        /// 
        [Display(Name = "Total")]
        public decimal Total { get; set; }

        [Display(Name = "Depth")]
        public int Depth { get; set; }

        /// <summary>
        /// Products in this order
        /// </summary>
        /// 
        [Display(Name = "Order Products")]
        public virtual ICollection<OrderProduct> OrderProducts { get; set; }


    }







    //public class UpsellAPIModel
    //{
    //    /// <summary>
    //    /// Encoded Order Id
    //    /// </summary>
    //    //public int OrderId { get; set; }

    //    /// <summary>
    //    /// Upsell products
    //    /// </summary>
    //    public virtual ICollection<OrderProduct> OrderProducts { get; set; }
    //}
}