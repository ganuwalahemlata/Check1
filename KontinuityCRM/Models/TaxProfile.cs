using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KontinuityCRM.Models
{
    [TrackChanges]
    public class TaxProfile
    {
        /// <summary>
        /// TaxProfile Id as primary key
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// TaxProfile Name
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// Indicates Rules associated with TaxProfile
        /// </summary>
        public virtual ICollection<TaxRule> TaxRules { get; set; } 
    }

    [TrackChanges]
    public class TaxRule
    {

        /// <summary>
        /// TaxRule Id as primary key
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Percent % 
        /// </summary>
        public decimal Tax { get; set; }
        /// <summary>
        /// Foreign key to country
        /// </summary>
        public int CountryId { get; set; }
        public virtual Country Country { get; set; }

        //public string Country { get; set; }
        /// <summary>
        /// Indicates to which TaxProfile, TaxRule Belongs
        /// </summary>
        public int TaxProfileId { get; set; }
        public virtual TaxProfile TaxProfile { get; set; }
        /// <summary>
        /// TaxRule City
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// TaxRule Province
        /// </summary>
        public string Province { get; set; }
        /// <summary>
        /// TaxRule PostalCode
        /// </summary>
        [Display(Name="Postal Code")]
        public string PostalCode { get; set; }

        /// <summary>
        /// Indicates if the percent tax is applied to the shipping price as well
        /// </summary>
        [Display(Name = "Apply Tax to Shipping")]
        public bool ApplyToShipping { get; set; }

        /// <summary>
        /// Indicates if it applies to the shipping address. If not apply to the billing address
        /// </summary>
        [Display(Name = "Address to Tax")]
        public bool ShippingAddress { get; set; }
    }
}