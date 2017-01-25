using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace KontinuityCRM.Models
{
    public class KLog
    {
        /// <summary>
        /// GuId for KLog
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Indicates User Name for KLog
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// Indicates IPAddress for KLog
        /// </summary>
        public string IPAddress { get; set; }
        /// <summary>
        /// Indicates Url for KLog
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// Indicates DateTime for KLog
        /// </summary>
        public DateTime Timestamp { get; set; }
    }
}