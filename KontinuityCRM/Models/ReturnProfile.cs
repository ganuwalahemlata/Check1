using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KontinuityCRM.Models
{
    public class ReturnProfile
    {
        /// <summary>
        /// retutn profile Id as primary key
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Indicates Name of return profile
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Indicates description of return profile
        /// </summary>
        public string Description { get; set; }

    }
}