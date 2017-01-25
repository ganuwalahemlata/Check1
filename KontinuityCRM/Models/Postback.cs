using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KontinuityCRM.Models
{
    public class Postback
    {
        /// <summary>
        /// PostbackId as primary key of Postback
        /// </summary>
        public int PostbackId { get; set; }
        /// <summary>
        /// Indicates foreign key to the product
        /// </summary>
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
        /// <summary>
        /// string for the PostBackURL
        /// </summary>
        public string PostbackURL { get; set; }

        public NotificationType EventType { get; set; }
        /// <summary>
        /// Indicates the parameters for the postback 
        /// </summary>
        public byte[] Parameters { get; set; }
    }
}