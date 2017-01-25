//using KontinuityCRM.Helpers.GRepository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KontinuityCRM.Models
{
    public class RMAReason
    {
        /// <summary>
        /// Id of RMAReason as primar key
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Description for the RMA
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Indicates whether the RMA is Active or not
        /// </summary>
        public bool Active { get; set; }
        /// <summary>
        /// Indicates if the CustomAction for RMA applied or not
        /// </summary>
        [Display(Name = "Custom Action")]
        public bool CustomAction { get; set; }
        /// <summary>
        /// Action against particular RMA
        /// </summary>
        public RMAAction Action { get; set; }
        /// <summary>
        /// Expired Action against RMA
        /// </summary>
        [Display(Name = "Expired Action")]
        public RMAAction ExpiredAction { get; set; }
        /// <summary>
        /// Expiration Days against RMA
        /// </summary>
        [Display(Name = "Expiration Days")]
        public int? ExpirationDays { get; set; }

        public ICollection<Order> Order { get; set; }
        public RMAReason()
        {
            this.Active = true;
        }
    }

    public enum RMAAction
    {
        /// <summary>
        /// 
        /// </summary>
        [Description("No Action")]
        NoAction = 1,
        [Description("Full Refund")]
        FullRefund = 2,
        [Description("Parial Refund (Exclude Shipping)")]
        ExcludeShipping = 3,
        [Description("Parial Refund (Exclude Restocking Fee)")]
        ExcludeRestockingFee = 4,
        [Description("Parial Refund (Exclude Restocking Fee & Exclude Shipping)")]
        ExcludeRestockingFee_ExcludeShipping = 5,

    }

    public class RMANumber // doesnt work 
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }

        public RMANumber(string number) // orderid-productid
        {
            if (!string.IsNullOrEmpty(number) && System.Text.RegularExpressions.Regex.IsMatch(number, @"^\d+-\d+$"))
            {
                string[] rma = number.Split('-');
                OrderId = int.Parse(rma[0]);
                ProductId = int.Parse(rma[1]);
            }

        }
    }
}