using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KontinuityCRM.Models.ViewModels
{
    public class RebillModel
    {
        public DateTime Date { get; set; }

        public DateTime DateT { get { return new DateTime(this.Year, this.Month, this.Day); } }

        public int Gross { get { return Attempted + Reattempts + Rebills; } } // gross               

        /*--------------------------Inititals----------------------------------------*/

        public int Attempted { get { return Approved + Declined; } } // initials attempts

        public int Approved { get; set; } // initials approve

        public int Declined { get; set; } // initial decline

        public int Refunded { get; set; } // initial refund

        [Display(Name = "Pre Cancels")]
        public int Canceled { get; set; } // intial cancel        

        /*--------------------------Reattempts----------------------------------------*/

        public int Reattempts { get { return ReattemptsApproved + ReattemptsDeclined; } } // decline salvage attempts

        [Display(Name = "Reattempt Approved")]
        public int ReattemptsApproved { get; set; } // decline salvage approve

        [Display(Name = "Reattempt Declined")]
        public int ReattemptsDeclined { get; set; } // decline salvage decline

        [Display(Name = "Reattempt Refunded")]
        public int ReattemptsRefunded { get; set; } // decline salvage refund
         
        [Display(Name = "Post Reattempt Cancels")]
        public int ReattemptsCanceled { get; set; } // decline salvage cancel

        /*--------------------------Rebills----------------------------------------*/

        public int Rebills { get { return RebillsApproved + RebillsDeclined; } } // rebill attempts
        public List<OrderTimeEvent> OrderList { get; set; }
        /// <summary>
        /// rebill approve
        /// </summary>
        [Display(Name = "Rebills Approved")]
        public int RebillsApproved { get; set; }

        /// <summary>
        /// rebill decline
        /// </summary>
        [Display(Name = "Rebills Declined")]
        public int RebillsDeclined { get; set; } // rebill decline

        /// <summary>
        /// rebill refund
        /// </summary>
        [Display(Name = "Rebills Refunded")]
        public int RebillsRefunded { get; set; } // rebill refund

        /// <summary>
        /// rebill cancel
        /// </summary>
        [Display(Name = "Post Cancels")]
        public int RebillsCanceled { get; set; } // rebill cancel

        /*------------------------------------------------------------------*/

        public int? ProductId { get; set; }

        public string AffiliateId { get; set; }

        public string SubId { get; set; }

        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public decimal Total { get; set; }
        public decimal Numberoforder { get; set; }
    }
}