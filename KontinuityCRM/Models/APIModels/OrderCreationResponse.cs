using ArtisanCode.SimpleAesEncryption;
using KontinuityCRM.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KontinuityCRM.Models.APIModels
{
    /// <summary>
    /// Model used to retrieve the order's data upon a successful order creation.
    /// </summary>
    public class OrderCreationResponse
    {
        /// <summary>
        /// Cyphered response
        /// </summary>
        //public string Cypher { get; set; }

        public int OrderId { get; set; }

        /// <summary>
        /// Defines if the CC used to create the order is prepaid or not
        /// </summary>
        public bool Prepaid { get; set; }
        /// <summary>
        /// Total order amount
        /// </summary>
        public decimal Total { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ShippingPrice { get; set; }
       
        /// <summary>
        /// 
        /// </summary>
        public string Descriptor { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CustomerServiceNumber { get; set; }

        /// <summary>
        /// IP address
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// Price of all products
        /// </summary>
        public decimal SubTotal { get; set; }

        /// <summary>
        /// Tax applied to this order
        /// </summary>
        public decimal Tax { get; set; }
        /// <summary>
        /// Provides the message from the processor
        /// </summary>
        public string TransactionResponse { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<OrderProductResponse> OrderProducts { get; set; }
    }

    /// <summary>
    /// Model used to retrieve the order's product upon a successful order creation.
    /// </summary>
    public class OrderProductResponse
    {
        /// <summary>
        /// 
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ProductName { get; set; }
    }
}