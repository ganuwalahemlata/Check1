using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KontinuityCRM.Models.ViewModels
{
    /// <summary>
    /// This entity stores information about the orders that are created from Partials
    /// </summary>
    public class PartialToOrdersConversion
    {
        /// <summary>
        /// Conversion Rate from Partials to Orders
        /// </summary>
        public double Conversion { get; set; }

        /// <summary>
        /// Total Partials
        /// </summary>
        public int TotalPartials { get; set; }

        /// <summary>
        /// No. of Partials from Orders
        /// </summary>
        public int PartialToOrders { get; set; }

        /// <summary>
        /// Date
        /// </summary>
        public DateTime Date { get; set; }


    }

    /// <summary>
    /// Model to apply filters in partial To orders conversion report
    /// </summary>
    public class PartialToOrdersFilter
    {

        public int? ProductId { get; set; }

        public int? CategoryId { get; set; }

        public string AffiliateId { get; set; }

        public string SubId { get; set; }

        [Display(Name = "Time Zone")]
        public int TimeZoneId { get; set; }

    }
}