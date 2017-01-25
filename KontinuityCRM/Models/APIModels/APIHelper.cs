using KontinuityCRM.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KontinuityCRM.Models.APIModels
{
    public static class APIHelper
    {
        public static OrderCreationResponse CreateOrderResponse(KontinuityCRM.Models.Order order)
        {
            var transaction = order.Transactions.FirstOrDefault();
            var orderProducts = new List<OrderProductResponse>();

            foreach (var op in order.OrderProducts)
            {
                orderProducts.Add(new OrderProductResponse
                {
                    Price = op.Price ?? 0,
                    ProductId = op.ProductId,
                    ProductName = op.Product.Name,
                });
            }

            return new OrderCreationResponse
            {
                Prepaid = order.IsPrepaid, // prepaidInfo != null && prepaidInfo.Prepaid;
                Total = order.Total,
                ShippingPrice = order.ShippingMethod.Price,
                OrderProducts = orderProducts,
                Descriptor = transaction == null ? null : transaction.Processor.Descriptor,
                CustomerServiceNumber = transaction == null ? null : transaction.Processor.CustomerServiceNumber,
                OrderId = order.OrderId,
            };
        }
    }
}