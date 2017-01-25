using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KontinuityCRM.Models.ViewModels
{
    public class CustomFieldValueViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Indicates value of CustomeFields
        /// </summary>
        public string Value { get; set; }
    }

    public class IntViewModel
    {
        /// <summary>
        /// Indicates Value of InViewModel
        /// </summary>
        public int Value { get; set; }
    }
    public class RMAProcessReasonsViewModel
    {
        /// <summary>
        /// Indicates RMANumber
        /// </summary>
        public string RMANumber { get; set; }
        /// <summary>
        /// Indicates Reason
        /// </summary>
        public string Reason { get; set; }
    }
}