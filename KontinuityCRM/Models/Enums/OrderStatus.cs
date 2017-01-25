using System.ComponentModel.DataAnnotations;

namespace KontinuityCRM.Models.Enums
{
    /// <summary>
    /// entity status for order
    /// </summary>
    public enum OrderStatus
    {
        [Display(Name="All")]
        All,
        Approved = 1,
        Declined, // 2

        Void,  //3
        Refunded, //4
        Deleted, //5

        [Display(Name = "RMA Issued")]
        RMAIssued, //6

        [Display(Name = "Shipped Exception")]
        ShippedException, //7

        Unpaid, //8
        [Display(Name = "Failed Capture")]
        FailedCapture, //9
        [Display(Name = "Test Orders")]
        IsTest = 10,
        [Display(Name = "Fraud")]
        Fraud = 11,
        [Display(Name = "Order Returned")]
        OrderReturned = 12,
        [Display(Name = "Order Shipped")]
        Shipped = 13,
        [Display(Name = "Partially Refunded")]
        PartiallyRefunded = 14
    }
}