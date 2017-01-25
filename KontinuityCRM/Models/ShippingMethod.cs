using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace KontinuityCRM.Models
{
    [TrackChanges]
    public class ShippingMethod
    {
        /// <summary>
        /// ShippingMethod Id as primary key
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ShippingMethod Name
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// ShippingMethod Description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Indicates the ShippingMethod Price
        /// </summary>
        public Decimal Price { get; set; }
        /// <summary>
        /// Indicates the RecurringPrice of ShippingMethod
        /// </summary>
        [Display(Name="Recurring Price")]
        public Decimal RecurringPrice { get; set; }

        /// <summary>
        /// Indicates ShippingCategoryId of ShippingMethod
        /// </summary>
        [Required]
        public int ShippingCategoryId { get; set; }
        [Display(Name = "Shipping Category")]
        public virtual ShippingCategory ShippingCategory { get; set; }
    }
    
    //public enum ShipType 
    //{ 
    //    type1,
    //    type2,
    //}

    [TrackChanges]
    public class ShippingCategory 
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Code { get; set; }

        public virtual ICollection<ShippingMethod> ShippingMethods { get; set; }

        [Display(Name = "Last Update")]
        public DateTime? LastUpdate { get; set; }

        [ForeignKey("UpdatedBy")]
        public int? UpdatedUserId { get; set; }
        [Display(Name = "Updated By")]
        public virtual UserProfile UpdatedBy { get; set; }

        [ForeignKey("CreatedBy")]
        public int? CreatedUserId { get; set; }
        [Display(Name = "Created By")]
        public UserProfile CreatedBy { get; set; }
    }
}