using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using KontinuityCRM.Models.Enums;
using System.ComponentModel;

namespace KontinuityCRM.Models
{
    [TrackChanges]
    public class DeclineType
    {
        /// <summary>
        /// Id as primary key
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identify the transaction type
        /// </summary>
        public string Name { get; set; }  // no funds on the card, fraud

        [Required]
        public string WildCard { get; set; }

        public virtual ICollection<SalvageProfile> SalvageProfiles { get; set; }

        //[ForeignKey("UserProfile")]
        //public int UserId { get; set; }
        //public virtual UserProfile UserProfile { get; set; }

        //#region # Rule if not specified use global rule #
        
        //[Display(Name = "Recurring Type")]
        //public BillType BillType { get; set; }
        //[Display(Name = "Recurring Value")]
        //public string BillValue { get; set; }
        //[Display(Name = "Cancel After")]
        //public int CancelAfter { get; set; }
        //[Display(Name = "Lower Price")]
        //public bool LowerPrice { get; set; }
        //[Display(Name = "Lower Price After")]
        //public int? LowerPriceAfter { get; set; }
        //public int? LowerPercent { get; set; }
        //[Display(Name = "Lower Amount")]
        //public decimal? LowerAmount { get; set; }


        //#endregion
    }
    /// <summary>
    /// Salvage Profile
    /// </summary>
    [TrackChanges]
    public class SalvageProfile // decline salvage profile
    {
        /// <summary>
        /// id as primary key
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// SalvageProfile name
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// SalvageProfile Decline Type
        /// </summary>
        public int DeclineTypeId { get; set; }
        [Display(Name = "Decline Type")]
        public virtual DeclineType DeclineType { get; set; }
        /// <summary>
        /// Indicates Bill Type
        /// </summary>
        [Display(Name = "Recurring Type")]
        public BillType BillType { get; set; }
        /// <summary>
        /// Indicates Bill Value
        /// </summary>
        [Display(Name = "Recurring Value")]
        public int BillValue { get; set; }
        /// <summary>
        /// Cancel After attempts
        /// </summary>
        [Display(Name = "Cancel After")]
        public int CancelAfter { get; set; }
        /// <summary>
        /// Indicates Lower Price for SalvageProfile
        /// </summary>
        [Display(Name = "Lower Price")]
        public bool LowerPrice { get; set; }
        /// <summary>
        /// Indicates lowerPrice after how many attempts
        /// </summary>
        [Display(Name = "Lower After")]
        public int? LowerPriceAfter { get; set; }
        /// <summary>
        /// Indicates lower Amount
        /// </summary>
        [Display(Name = "Lower Amount")]
        public decimal? LowerAmount { get; set; }
        [Display(Name = "Lower Percentage")]
        public decimal? LowerPercentage { get; set; }
        /// <summary>
        /// Prepaid icrement for salvageProfile
        /// </summary>
        [Display(Name = "Prepaid Increment")]
        public decimal PrepaidIncrement { get; set; }

        [Display(Name = "Next Salvage profile")]
        public int? NextSalvageProfile { get; set; }

        [Display(Name = "Enable Discount")]
        [DefaultValue(false)]
        public bool EnableDiscount { get; set; }

        [Display(Name = "After Declined")]
        [DefaultValue(false)]
        public bool AfterDecline { get; set; }

        [Display(Name = "After Approved")]
        [DefaultValue(false)]
        public bool AfterApprove { get; set; }

    }
    /// <summary>
    /// Product Salvage
    /// </summary>
    [TrackChanges]
    public class ProductSalvage
    {

        /// <summary>
        /// SalvageProfile id foreign key to SalvageProfile
        /// </summary>
        [Key, Column(Order = 0)]
        [ForeignKey("SalvageProfile")]
        public int SalvageProfileId { get; set; }
        public virtual SalvageProfile SalvageProfile { get; set; }
        /// <summary>
        /// ProductId foreign key to product
        /// </summary>
        [Key, Column(Order = 1)]
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
    }
}