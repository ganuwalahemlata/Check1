
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KontinuityCRM.Models
{
    public class TransactionViaPrepaidCardQueue
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

        //public bool IsRebill { get; set; }
        /// <summary>
        /// Indicates the Transaction Type i.e. Sale, Auth, Void, Refund, Capture
        /// </summary>
        public TransactionType Type { get; set; }
        /// <summary>
        /// Transaction Id
        /// </summary>
        public string TransactionId { get; set; }
        /// <summary>
        /// Indicates the Reference for the transaction
        /// </summary>
        public string TransactionReference { get; set; }

        public TransactionStatus Status { get; set; }
        /// <summary>
        /// Indicates the OrderId for which the transaction has been processed
        /// </summary>
        //public int OrderId { get; set; }

        //public virtual Order Order { get; set; }
        /// <summary>
        /// Indicates the processirId as foreign key
        /// </summary>
        public int? ProcessorId { get; set; }

        public virtual Processor Processor { get; set; }
        /// <summary>
        /// Indicates the balancer Id as foreign key
        /// </summary>
        public int? BalancerId { get; set; } // to track if there is a balancer from which the preserved processor came by
        public virtual Balancer Balancer { get; set; }

        //public int? GatewayId { get; set; }

        //public virtual Gateway Gateway { get; set; }
        /// <summary>
        /// Indicates the date of transaction
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// Amount costed for the transaction
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// Indicates whether the transaction was success/failure
        /// </summary>
        public bool Success { get; set; }


        public int PrepaidCardId { get; set; }

        public virtual PrepaidCard PrepaidCard { get; set; }


       


        public int TransactionQueueMasterId { get; set; }


    }

   

}