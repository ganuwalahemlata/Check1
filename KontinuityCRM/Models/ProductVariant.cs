using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KontinuityCRM.Models
{
    public class ProductVariant
    {
        /// <summary>
        /// ProductvariantId as primary key
        /// </summary>
        public int ProductVariantId { get; set; }
        /// <summary>
        /// ProductId, Foreign key to Product
        /// </summary>
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        /// <summary>
        /// CountryId, Foreign Key to Country
        /// </summary>
        [Display(Name = "Country")]
        public int CountryId { get; set; }
        public virtual Country Country { get; set; }
        /// <summary>
        /// Stock Keeping Unit of ProductVariant
        /// </summary>
        public string SKU { get; set; }
        /// <summary>
        /// Indicates of Cost of ProductVariant
        /// </summary>
        public decimal? Cost { get; set; }
        /// <summary>
        /// Indicates Price of ProductVariant
        /// </summary>
        public decimal? Price { get; set; }
        /// <summary>
        /// Indicates the Currency
        /// </summary>
        public Currency Currency { get; set; }        

    }

    public class VariantExtraField
    {
        public int Id { get; set; }

        public int ProductVariantId { get; set; }
        public virtual ProductVariant ProductVariant { get; set; }
        
        public string FieldName { get; set; }

        public string FieldValue { get; set; }
    }

    public enum ExtraFields
    { 
        Name,
        Description,
        Taxable,
        ShipValue,
        Weight,
        FulfillmentProvider,
        SignatureConfirmation,
        DeliveryConfirmation,
        RecurringProductId,
        BillType,
        BillValue,
        Balancer,
        Processor,
    }



    
}