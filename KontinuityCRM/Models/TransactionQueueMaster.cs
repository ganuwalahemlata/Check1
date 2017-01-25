using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KontinuityCRM.Models
{
    [TrackChanges]
    public class TransactionQueueMaster
    {
        /// <summary>
        /// PrepaidCustomerTable Id as primary key
        /// </summary>
        public int Id { get; set; }

        [Required]
        [Display(Name = "No of Transactions")]
        public int NoOfTransactions { get; set; }

        [Required]
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }

        [Required]
        [Display(Name = "Processor")]
        public int? ProcessorId { get; set; }

        public virtual Processor Processor { get; set; }

        public string CardType { get; set; }

        public int RemainingTransactions { get; set; }

        [Display(Name = "Date")]
        public DateTime? Date { get; set; }

        public bool finished { get; set; }

        [Required]
        [NotMapped]
        public int TimeIntervalMin { get; set; }

        [Required]
        [NotMapped]
        public int TimeIntervalMax { get; set; }

    }
}