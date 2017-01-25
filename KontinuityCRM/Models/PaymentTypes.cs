using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace KontinuityCRM.Models
{
    [TrackChanges]
    public class PaymentTypes
    {
        /// <summary>
        /// PaymentTypeId as primary key
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PaymentTypeId { get; set; }
        /// <summary>
        /// PaymentType Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Created Date of PaymentType
        /// </summary>
        public DateTime CreatedOn { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual ICollection<ProductPaymentType> ProductPaymentType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual ICollection<Order> Orders { get; set; }
    }
}