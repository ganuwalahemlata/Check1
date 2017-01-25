using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KontinuityCRM.Models
{
    public class TransactionDetails
    {
        /// <summary>
        /// Transaction Id as primary key
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Request details for the transaction, XML
        /// </summary>
        public string Request { get; set; }
        /// <summary>
        /// Response details for transaction, XML
        /// </summary>
        public string Response { get; set; }
        /// <summary>
        /// Indicates the remarks for Transaction
        /// </summary>
        public string Message { get; set; }

        
        public string  ProcessorName { get; set; }

        public DateTime Date { get; set; }
        /// <summary>
        /// Amount costed for the transaction
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// Indicates whether the transaction was success/failure
        /// </summary>
        public bool Success { get; set; }

        public string  CreditCardNumber { get; set; }

    }
}