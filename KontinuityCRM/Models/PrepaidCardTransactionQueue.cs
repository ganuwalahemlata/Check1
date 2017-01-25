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
    public class PrepaidCardTransactionQueue
    {
        /// <summary>
        /// PrepaidCustomerTable Id as primary key
        /// </summary>
        public int Id { get; set; }

        [Required]
        [Display(Name = "No of Transactions")]
        public string no_of_transactions { get; set; } 

        [Required]
        [Display(Name = "Amount")]
        public string amount { get; set; }

        [Required]
        [Display(Name = "Processor")]
        public string ProcessorID { get; set; }


        [Display(Name = "Date")]
        public DateTime? Date { get; set; }


        public bool stop { get; set; }
       
    }

    public enum TransactionStatusEnum
    {
        decline=1,
        success,
       
    }

}