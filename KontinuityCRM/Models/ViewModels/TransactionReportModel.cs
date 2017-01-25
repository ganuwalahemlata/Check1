using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KontinuityCRM.Models.ViewModels
{
    public class TransactionReportModel
    {
        public int NoOfTransactionsApproved { get; set; }

        public int NoOfTransactionsDeclined { get; set; }

        public int? ProcessorId { get; set; }
        public string ProcessorType { get; set; }

        public string CardType { get; set; }
    }
}