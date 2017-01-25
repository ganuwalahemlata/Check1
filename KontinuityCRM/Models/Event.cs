using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace KontinuityCRM.Models
{
    [TrackChanges]
    public class Event
    {
        /// <summary>
        /// Event Id as primary key
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Indicates the Name of Event
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// Indicates whether the Event is published or not
        /// </summary>
        public bool Publish { get; set; }
        /// <summary>
        /// Indicates the Type of Event  i.e. OrderRefund, OrderConfirmation, RMANotification etc
        /// </summary>
        [Display(Name = "Event Type")]
        public NotificationType Type { get; set; }
        /// <summary>
        /// Indicates the Template Id for the Template to be applied for the event
        /// </summary>
        [Display(Name = "Template")]
        [ForeignKey("Template")]
        public int TemplateId { get; set; }
        public virtual EmailTemplate Template { get; set; }
        /// <summary>
        /// Indicates the SmtpServerId 
        /// </summary>
        [Display(Name = "Smtp Profile")]
        [ForeignKey("SmtpServer")]
        public int SmtpServerId { get; set; }
        public virtual SmtpServer SmtpServer { get; set; }
        /// <summary>
        /// Description of the Event
        /// </summary>
        public string Description { get; set; }

        public virtual ICollection<ProductEvent> Products { get; set; }
        /// <summary>
        /// Indicates the UserId who created this event
        /// </summary>
        [ForeignKey("CreatedBy")]
        public int? CreatedUserId { get; set; }
        [Display(Name = "Created")]
        public virtual UserProfile CreatedBy { get; set; }
        /// <summary>
        /// Indicates UserId of the User who updated this event
        /// </summary>
        [ForeignKey("UpdatedBy")]
        public int? UpdatedUserId { get; set; }
        [Display(Name = "Updated By")]
        public virtual UserProfile UpdatedBy { get; set; }
        /// <summary>
        /// Indicates the Last Time the Event was updated
        /// </summary>

        [Display(Name = "X Days")]

        public string NoOfDays { get; set; }

        //public int triggertime { get; set; }

        [Display(Name = "Last Update")]
        public DateTime? LastUpdate { get; set; }


       
        public int SecondTemplateId { get; set; }
        //public virtual EmailTemplate SecondTemplate { get; set; }
    }

    public enum NotificationType
    {
        [Display(Name = "Order Confirmation")]
        OrderConfirmation = 1,
        [Display(Name = "Order Refund")]
        OrderRefund,
        [Display(Name = "RMA Notification")]
        RMANotification,
        [Display(Name = "Payment Issue Notification")]
        PaymentIssueNotification,
        [Display(Name = "Void Notification")]
        VoidNotification,
        [Display(Name = "Cancellation Notification")]
        CancellationNotification,
        [Display(Name = "Return Notification")]
        ReturnNotification,
        [Display(Name = "Pending Order Notification")]
        PendingOrderNotification,
        [Display(Name = "Shipping Notification")]
        ShippingNotification
    }
    /// <summary>
    /// Bridge Entity b/w Product and Event
    /// </summary>
    public class ProductEvent
    {
        /// <summary>
        /// Indicates the ProductId
        /// </summary>
        [Key, Column(Order = 0)]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
        /// <summary>
        /// Indicates the EventId
        /// </summary>
        [Key, Column(Order = 1)]
        public int EventId { get; set; }
        public virtual Event Event { get; set; }
    }
}