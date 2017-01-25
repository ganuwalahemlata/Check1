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
    public class TransactionQueue
    {
        /// <summary>
        /// PrepaidCustomerTable Id as primary key
        /// </summary>
        public int Id { get; set; }

        [Required]
        public int PrepaidCardId { get; set; }
        public virtual PrepaidCard PrepaidCard { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public int? ProcessorId { get; set; }

        public virtual Processor Processor { get; set; }

        [Required]
        public DateTime? Date { get; set; }

        public bool finished { get; set; }

        public int TransactionQueMasterId { get; set; }

        public int TimeIntervalMin { get; set; }

        public int TimeIntervalMax { get; set; }

        public DateTime? LastUpdatedDate { get; set; }

        public int? Attempt { get; set; }

    }

  

}