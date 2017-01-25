using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using KontinuityCRM.Helpers;
using System.Transactions;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using KontinuityCRM.Models.Enums;
using KontinuityCRM.Models.Gateways;
using System.Collections.Specialized;
using AutoMapper;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Text;
using System.Data.SqlClient;

namespace KontinuityCRM.Models
{
    public class PrepaidTransactionQueueMasters
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

        [Required]
        [Display(Name = "Card Type")]
        public string CardType { get; set; }

        [Required]
        [Display(Name = "Remaining Transactions")]
        public string RemainingTransactions { get; set; }

        [Display(Name = "Date")]
        public DateTime? Date { get; set; }
        
        public bool finished { get; set; }

    }
}