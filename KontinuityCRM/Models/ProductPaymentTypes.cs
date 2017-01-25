using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace KontinuityCRM.Models
{
    [TrackChanges]
    public class ProductPaymentType
    {
        /// <summary>
        /// Id as primary key
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
       /// <summary>
       /// Product Id as foreign key to Products
       /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// Payment Type Id asforeign key to paymentTypes
        /// </summary>
        public int PaymentTypeId { get; set; }
        /// <summary>
        /// Created On Date
        /// </summary>
        public DateTime CreatedOn { get; set; }

        public virtual PaymentTypes PaymentType { get; set; }

        public virtual Product Products { get; set; }
    }
}