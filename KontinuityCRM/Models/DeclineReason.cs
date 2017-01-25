using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KontinuityCRM.Models
{
    public class DeclineReason
    {
        /// <summary>
        /// Id as primary Key for DeclineReason
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Indicates the DeclineReason Description
        /// </summary>
        public string Reason { get; set; }
        /// <summary>
        /// Indicates the random description for DeclineReason
        /// </summary>
        public string WildCard { get; set; }
    }
}