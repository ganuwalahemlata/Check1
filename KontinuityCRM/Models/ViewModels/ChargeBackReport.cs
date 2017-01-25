using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KontinuityCRM.Models.ViewModels
{
    /// <summary>
    /// ChargeBack Report.
    /// </summary>
    public class ChargeBackReport
    {
        /// <summary>
        /// Processor
        /// </summary>
        public string Processor { get; set; }

        /// <summary>
        /// Chargebacks 
        /// </summary>
        [DisplayName("Total Chargebacks")]
        public int Chargebacks
        {
            get;
            set;
        }

        /// <summary>
        /// Gross Transaction Values.
        /// </summary>
        [DisplayName("Gross Transaction Values")]
        public decimal TotalAmounts
        {
            get;
            set;
        }


        /// <summary>
        /// Chargeback Amout.
        /// </summary>
        [DisplayName("Chargeback Amounts")]
        public decimal ChargebackAmounts
        {
            get;
            set;
        }

        /// <summary>
        /// ChargebacksRatio 
        /// </summary>
        [DisplayName("Chargeback Ratio")]
        public decimal ChargebacksRatio
        {
            get;
            set;
        }

        /// <summary>
        /// Total Transactions.
        /// </summary>
        [DisplayName("Total Transactions")]
        public int TotalTransactions
        {
            get;
            set;
        }

    }
}