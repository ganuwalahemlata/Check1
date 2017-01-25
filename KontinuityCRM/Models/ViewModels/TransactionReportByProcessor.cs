using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KontinuityCRM.Models.ViewModels
{
    public class TransactionReportByProcessor
    {
        public int NoOfSuccess { get; set; }

        public int NoOfFailed { get; set; }

        public string PaymentType { get; set; }

        public string ProcessorName { get; set; }

       


    }
}