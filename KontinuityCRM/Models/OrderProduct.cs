using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using KontinuityCRM.Models.Enums;

namespace KontinuityCRM.Models
{
    [TrackChanges]
    public class OrderProduct
    {
        /// <summary>
        /// OrderId for referential Integrity
        /// </summary>
        [Key, Column(Order = 0)]
        [ForeignKey("Order")]
        public int OrderId { get; set; }
        /// <summary>
        /// Order for referential Integrity
        /// </summary>
        public virtual Order Order { get; set; }
        /// <summary>
        /// ProductId for referential integrity
        /// </summary>
        [Key, Column(Order = 1)]
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        /// <summary>
        /// Product for referential integrity
        /// </summary>
        public virtual Product Product { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Next Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? NextDate { get; set; }

        [Display(Name = "Next Product")]
        [ForeignKey("NextProduct")]
        public int? NextProductId { get; set; } // is this a FK to product ? yes 
        [Display(Name = "Next Product")]
        public virtual Product NextProduct { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "The Quantity field must be greater than {1}")]
        public int Quantity { get; set; }

        [Display(Name = "Rebill Discount")]
        public decimal RebillDiscount { get; set; } //  Amount Discount on next recurring product, not including shipping price

        /// <summary>
        /// Price of the current Product
        /// </summary>
        public decimal? Price { get; set; } // It is necessary. this will not be null ever because it will default to the product price. Though 
        // it could be custom it will be override in the next rebill
        /// <summary>
        /// Tax amount on to charge on this product. Includes the shipping tax
        /// </summary>
        public decimal Tax { get; set; }

        //public decimal Total { get; set; }

        /// <summary>
        /// Is null either because the product isnot shippable or because it has been sent to fulfillment yet
        /// </summary>
        public bool? Shipped { get; set; }

        [DefaultValue(true)] // at the creation moment this value is always true
        /// <summary>
        /// Indicates were the order is recurring or is stopped or should stop after the next succeful bill
        /// </summary>
        public bool Recurring { get; set; }

        /// <summary>
        /// Indicates the reattemps rebills on this orderproduct
        /// </summary>
        public int ReAttempts { get; set; }

        [ForeignKey("ProductVariant")]
        public int? ProductVariantId { get; set; }
        public virtual ProductVariant ProductVariant { get; set; }
        /// <summary>
        /// Stock Keeping Unit of Product
        /// </summary>
        public string SKU { get; set; }
        /// <summary>
        /// Indicates the cost that has been invested on the product
        /// </summary>
        public decimal Cost { get; set; }

        /// <summary>
        /// Indicates the Currency for the orderprocess
        /// </summary>
        public Currency Currency { get; set; }
        public string FieldName { get; set; }

   
        public string FieldValue { get; set; }

        public OrderProduct()
        {
            this.Recurring = true; // at the creation moment this value is always true
        }

        /// <summary>
        /// Indicates were this product has been rebilled for this order. The value is the Id of the approved child order
        /// </summary>
        [ForeignKey("ChildOrder")]
        public int? ChildOrderId { get; set; }

        public virtual Order ChildOrder { get; set; }

        public int? RMAReasonId { get; set; }
        public virtual RMAReason RMAReason { get; set; }

        public string FulfillmentProviderResponse { get; set; }

        [Index]
        public DateTime? FulfillmentDate { get; set; }

        /// <summary>
        /// It depends on the salvage rule applied.
        /// </summary>
        //public decimal? PrepaidIncrement { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? SalvageProfileId { get; set; }
        public virtual SalvageProfile SalvageProfile { get; set; }

        public int? NextSalvageProfileId { get; set; }
        public virtual SalvageProfile NextSalvageProfile { get; set; }

        public BillType? BillType { get; set; }

        [Display(Name = "Rebill Frequency")]
        public int? BillValue { get; set; }
        public string TrackingNo { get; set; }
        public string ShipmentId { get; set; }
    }

    public class OrderNote
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public virtual Order Order { get; set; }

        public DateTime NoteDate { get; set; }

        public string Note { get; set; }

        //public string GatewayResponse { get; set; }

        //public string Type { get; set; }

        //public bool Success { get; set; }

        //public string FulfillmentProviderResponse { get; set; }

    }

    public enum PaymentType
    {
        AmericanExpress = 1,
        Discover,
        MasterCard,
        Visa,
        Other,
    }

    public enum Month
    {
        January = 1,
        February = 2,
        March = 3,
        April,
        May,
        June,
        July,
        August,
        September,
        October,
        November,
        December
    }

    public class OrderTimeEvent
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public virtual Order Order { get; set; }

        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        public DateTimeOffset Time { get; set; }

        [Index]
        public OrderEvent Event { get; set; }

        /// <summary>
        /// reattempt = true, rebill = false, null 
        /// </summary>
        [Index]
        public bool? Action { get; set; } // reattempt = true, rebill = false, null 

        [Display(Name = "Affiliate Id")]
        public string AffiliateId { get; set; }

        [Display(Name = "Sub Id")]
        public string SubId { get; set; }

    }

    public enum OrderEvent : byte
    {
        Approved = 1,
        Declined,
        Refunded,
        Canceled
    }

}
