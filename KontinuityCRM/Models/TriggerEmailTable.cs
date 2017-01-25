using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KontinuityCRM.Models
{

    [TrackChanges]
    public class TriggerEmailTable
    {
        public int Id { get; set; }
        /// <summary>
        /// Indicates the Name of Event
        /// </summary>
        [Required]
        public int OrderId { get; set; }

        public bool sent { get; set; }
        /// <summary>
        /// Indicates whether the Event is published or not
        /// </summary>

        /// <summary>
        /// Indicates the Type of Event  i.e. OrderRefund, OrderConfirmation, RMANotification etc
        /// </summary>

        public NotificationType Type { get; set; }
        /// <summary>
        /// Indicates the Template Id for the Template to be applied for the event
        /// </summary>
     
        public DateTime TriggerTime { get; set; }

        public int EventId { get; set; }
    }
}