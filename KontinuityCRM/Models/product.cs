using KontinuityCRM.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using KontinuityCRM.Models.Enums;

namespace KontinuityCRM.Models
{
    [TrackChanges]
    public class Product
    {

        /// <summary>
        /// Primary Key
        /// </summary>
        [Key]
        public int ProductId { get; set; }
        /// <summary>
        /// Product Name
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// Description about the product if available
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Indicates Stock Keeping Unit of Product
        /// </summary>
        public string SKU { get; set; }
        /// <summary>
        /// Amount spent 
        /// </summary>
        public decimal Cost { get; set; }

        /// <summary>
        /// Amount that will be charged to the customer
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// Processing amount currency
        /// </summary>
        public Currency Currency { get; set; }

        /// <summary>
        /// Whether the tax applies on product or not
        /// </summary>
        [Display(Name = "Taxable")]
        public bool IsTaxable { get; set; }

        /// <summary>
        /// Whether the product is Shippable or not
        /// </summary>
        [Display(Name = "Shippable")]
        public bool IsShippable { get; set; }
        /// <summary>
        /// Weight of the product
        /// </summary>
        public int? Weight { get; set; }
        /// <summary>
        /// Ship Value of the product
        /// </summary>
        public decimal? ShipValue { get; set; }

        /// <summary>
        /// Whether the Signature confirmation is required or not
        /// </summary>
        [Display(Name = "Signature Confirmation")]
        public bool IsSignatureConfirmation { get; set; }

        //public virtual DeclineSalvage DeclineSalvage { get; set; }
        /// <summary>
        /// Ids of the fulfillment providers to the product
        /// </summary>
        public int? FulfillmentProviderId { get; set; }
        [Display(Name = "Fulfillment Provider")]
        public virtual FulfillmentProvider FulfillmentProvider { get; set; }

        /// <summary>
        /// Confirmation required when the product is delivered or not.
        /// </summary>
        [Display(Name = "Delivery Confirmation")]
        public bool IsDeliveryConfirmation { get; set; }

        #region Subscription Region NextProduct / NextDate

        [Display(Name = "Subscription")]
        public bool IsSubscription { get; set; }

        /// <summary>
        /// Next Product
        /// </summary>
        [Display(Name = "Next Recurring ProductId")]
        [ForeignKey("RecurringProduct")]
        public int? RecurringProductId { get; set; }
        public virtual Product RecurringProduct { get; set; }

        [Display(Name = "Subscription Type")]
        public BillType BillType { get; set; }

        public int? BillValue { get; set; }

        #endregion

        /// <summary>
        /// Apply Load Balancer of the product
        /// </summary>
        [Display(Name = "Load Balancer")]
        public bool LoadBalancer { get; set; }
        /// <summary>
        /// Indicates BalancerId for the product, foreign key to Balancer
        /// </summary>
        [RequiredIf(BooleanPropertyName = "LoadBalancer", ExpectedValue = true, ErrorMessage = "The Balancer field is required.")]
        public int? BalancerId { get; set; }
        public virtual Balancer Balancer { get; set; }
        /// <summary>
        /// Indicates the ProcessorId for the product, foreign key to Processor
        /// </summary>
        [RequiredIf(BooleanPropertyName = "LoadBalancer", ExpectedValue = false, ErrorMessage = "The Processor field is required.")]
        public int? ProcessorId { get; set; }
        public virtual Processor Processor { get; set; }

        [Display(Name = "Redemption")]
        public bool HasRedemption { get; set; }
        /// <summary>
        /// Indicates the SalePonits of the product
        /// </summary>
        [Display(Name = "Sale Points")]
        public int? SalePoints { get; set; }
        [Display(Name = "Redemption Points")]
        public int? RedemptionPoints { get; set; }
        /// <summary>
        /// AutoResponderProvider associated with the product
        /// </summary>
        [Display(Name = "Auto Responder")]
        public int? AutoResponderProviderId { get; set; }
        public virtual AutoResponderProvider AutoResponderProvider { get; set; }
        /// <summary>
        /// Indicates autoresponder prospect Id to be placed in autoresponder profile list
        /// </summary>
        [Display(Name = "Prospect Id")]
        public string AutoResponderProspectId { get; set; }
        /// <summary>
        /// Indicates autoresponder customer Id to be placed in autoresponder profile list
        /// </summary>
        [Display(Name = "Customer Id")]
        public string AutoResponderCustomerId { get; set; }

        //[Display(Name = "Customer Id")]
        //public int? CustomerId { get; set; }
        //public virtual Customer Customer { get; set; }

        //[Display(Name = "Partial Id")]
        //public int? PartialId { get; set; }
        //public virtual Partial Partial { get; set; }

        public virtual ICollection<ProductEvent> ProductEvents { get; set; }

        public virtual ICollection<PostBackUrl> PostBackUrls { get; set; }

        public virtual ICollection<Partial> Partials { get; set; }
        /// <summary>
        /// CategoryId of the Category to which the product belongs
        /// </summary>
        [Required]
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }

        /// <summary>
        /// ProductSalvages details
        /// </summary>
        public virtual ICollection<ProductSalvage> ProductSalvages { get; set; }

        /// <summary>
        /// TaxProfileId of TaxProfile that applies to the product
        /// </summary>
        public int? TaxProfileId { get; set; }
        public virtual TaxProfile TaxProfile { get; set; }
        /// <summary>
        /// Whether the product limit is single or not.
        /// </summary>
        [Display(Name = "Single Purchase Limit")]
        public Boolean IsSinglePurchaseLimit { get; set; }

        public virtual ICollection<ProductPaymentType> ProductPaymentType { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Select Atleast one payment type")]
        public IEnumerable<string> PaymentTypeIds { get; set; }
        /// <summary>
        /// Pre Auth amount of product
        /// </summary>
        [Display(Name = "Pre-auth amount")]
        public decimal? PreAuthAmount { get; set; }
        
        /// <summary>
        /// Re-Stocking fee of the product.
        /// </summary>
        [Display(Name="Re-Stocking Fee")]
        public decimal? ReStockingFee { get; set; }
    }

    [TrackChanges]
    public class Category
    {
        /// <summary>
        /// Category Id as primary key
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of the category
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Desccrption about the category
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Products that belong to category
        /// </summary>
        public virtual ICollection<Product> Products { get; set; }
    }

    //public class ProductCategory
    //{
    //    public int Id { get; set; }

    //    public int ProductId { get; set; }
    //    public virtual Product Product { get; set; }

    //    public int CategoryId { get; set; }
    //    public virtual Category Category { get; set; }
    //}

    //public class DeclineSalvage
    //{
    //    //public int Id { get; set; }

    //    // i can't do this yet i don't since this is the foreign key that goes here
    //    //public int CustomerId { get; set; }
    //    //public virtual Customer Customer { get; set; }

    //    [Key]
    //    [ForeignKey("Product")]
    //    public int ProductId { get; set; }
    //    public virtual Product Product { get; set; }

    //    [DisplayName("Recurring Type")]
    //    public BillType BillType { get; set; }

    //    [DisplayName("Recurring Value")]
    //    public string BillValue { get; set; }
    //    [Display(Name = "Cancel After")]
    //    public int CancelAfter { get; set; }
    //    [Display(Name = "Lower Price")]
    //    public bool LowerPrice { get; set; }
    //    [Display(Name = "Lower Price After")]
    //    public int? LowerPriceAfter { get; set; }
    //    public int? LowerPercent { get; set; }
    //    [Display(Name="Lower Amount")]
    //    public decimal? LowerAmount { get; set; }
    //}


    //public class ProductEditVM 
    //{
    //    public BillDay BillDay { get; set; }
    //    public DayOfWeek DayOfWeek { get; set; }

    //    public ProductEditVM() { }

    //    public ProductEditVM(Product p)
    //    {
    //        this.ProductId = p.ProductId;
    //        this.Name = p.Name;
    //        Price = p.Price;
    //        ProcessorId = p.ProcessorId;
    //        Processor = p.Processor;
    //        Description = p.Description;
    //        IsShippable = p.IsShippable;
    //        IsSignatureConfirmation = p.IsSignatureConfirmation;
    //        IsSubscription = p.IsSubscription;
    //        IsTaxable = p.IsTaxable;

    //    }
    //}

    public enum Currency
    {
        USD = 1,
        EUR,
        GBP,
        DKK,
        CNY,
    }

    public enum BillDay
    {
        First = 1,
        Second,
        Third,
        Fourth,
        Last,
    }

    public enum UrlOrderType
    {
        Both = 1,
        Initial,
        Subscription,
    }

    public enum UrlOrderStatus
    {
        Both = 1,
        Approved,
        Declined,
    }

    [Flags]
    public enum UrlOrderAction
    {
        Void = 0x1,
        Refund = 0x2,
        Cancel = 0x4,
        Hold = 0x8,
        Reset = 0x10,
        Recurring = 0x20,
        Chargeback = 0x40,
    }

    public enum UrlPayments
    {
        Both = 1,
        NonTestPayments,
        TestPayments,
    }

    public class PostBackUrl
    {
        public int Id { get; set; }
        public string Url { get; set; }

        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        public UrlOrderType OrderType { get; set; }
        public UrlOrderStatus OrderStatus { get; set; }
        public UrlPayments Payments { get; set; }

        public UrlOrderAction OrderActions { get; set; }

        public bool IsAction { get; set; }

    }

    //public class UrlType : PostBackUrl
    //{
    //    public UrlOrderType OrderType { get; set; }
    //    public UrlOrderStatus OrderStatus { get; set; }
    //    public UrlPayments Payments { get; set; }
    //}

    //public class UrlAction : PostBackUrl
    //{

    //}

}