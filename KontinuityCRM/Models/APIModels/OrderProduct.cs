using KontinuityCRM.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KontinuityCRM.Models.APIModels
{
    /// <summary>
    /// Model used to retrieve and set the order's products
    /// </summary>
    public class OrderProduct
    {
        /// <summary>
        ///  Product Id
        /// </summary>
        [Required]
        [CheckProductExitsAttribute(ErrorMessage = "Product with id {0} not found")]
        public int ProductId { get; set; }

        /// <summary>
        /// Quantity ordered
        /// </summary>
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "The Quantity field must be greater than {1}")]
        public int Quantity { get; set; }

        /// <summary>
        /// Optional custom price
        /// </summary>
        public decimal? Price { get; set; }

        /// <summary>
        /// Product Name
        /// </summary>
        public string ProductName { get; set; }
    }
}