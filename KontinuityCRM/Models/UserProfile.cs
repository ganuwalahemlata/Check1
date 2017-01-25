using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace KontinuityCRM.Models
{
    [Table("UserProfile")]
    public class UserProfile
    {
        /// <summary>
        /// User Id as primary key
        /// </summary>
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        /// <summary>
        /// Indicates UserName of UserProfile
        /// </summary>
        public string UserName { get; set; }   
        /// <summary>
        /// Indicates Permissions of User
        /// </summary>
        public long? Permissions { get; set; }
        /// <summary>
        /// Indicates set of more Permission 2
        /// </summary>
        public long? Permissions1 { get; set; }
        /// <summary>
        /// Indicates set of more Permission 3
        /// </summary>
        public long? Permissions2 { get; set; }
        /// <summary>
        /// Indicates Email of UserProfile
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Indicates API key of UserProfile
        /// </summary>
        public Guid APIKey { get; set; }
        /// <summary>
        /// Indicates No. of Categories diaplayed to UserProfile
        /// </summary>
       
        public int CategoryDisplay { get; set; }
        /// <summary>
        ///  Indicates No. of Product diaplayed to UserProfile
        /// </summary>
        public int ProductDisplay { get; set; }
        /// <summary>
        ///  Indicates No. of Order diaplayed to UserProfile
        /// </summary>
        public int OrderDisplay { get; set; }
        /// <summary>
        ///  Indicates No. of ShippingMethod diaplayed to UserProfile
        /// </summary>
        public int ShippingMethodDisplay { get; set; }
        /// <summary>
        ///  Indicates No. of ShippingCategories diaplayed to UserProfile
        /// </summary>
        public int ShippingCategoryDisplay { get; set; }
        /// <summary>
        ///  Indicates No. of Balancer diaplayed to UserProfile
        /// </summary>
        public int BalancerDisplay { get; set; }
        /// <summary>
        ///  Indicates No. of Gateway diaplayed to UserProfile
        /// </summary>
        public int GatewayDisplay { get; set; }
        /// <summary>
        ///  Indicates No. of FulFillment diaplayed to UserProfile
        /// </summary>
        public int FulfillmentDisplay { get; set; }
        /// <summary>
        ///  Indicates No. of AutoResponder diaplayed to UserProfile
        /// </summary>
        public int AutoresponderDisplay { get; set; }
        /// <summary>
        ///  Indicates No. of Processor diaplayed to UserProfile
        /// </summary>
        public int ProcessorDisplay { get; set; }
        /// <summary>
        ///  Indicates No. of EmailTemplate diaplayed to UserProfile
        /// </summary>
        public int EmailTemplateDisplay { get; set; }
        /// <summary>
        ///  Indicates No. of RMAReason diaplayed to UserProfile
        /// </summary>
        public int RMAReasonDisplay { get; set; }
        /// <summary>
        ///  Indicates No. of User diaplayed to UserProfile
        /// </summary>
        public int UserDisplay { get; set; }
        /// <summary>
        ///  Indicates No. of Customer diaplayed to UserProfile
        /// </summary>
        public int CustomerDisplay { get; set; }    
        /// <summary>
        ///  Indicates No. of Partial diaplayed to UserProfile
        /// </summary>
        public int PartialDisplay { get; set; }
        /// <summary>
        ///  Indicates No. of Audit Logs diaplayed to UserProfile
        /// </summary>
        public int AuditLogDisplay { get; set; }
        /// <summary>
        ///  Indicates No. of Klog diaplayed to UserProfile
        /// </summary>
        public int KLogDisplay { get; set; }
        /// <summary>
        ///  Indicates No. of Decline Reason diaplayed to UserProfile
        /// </summary>
        public int DeclineReasonDisplay { get; set; }
        /// <summary>
        ///  Indicates No. of SmtpServer diaplayed to UserProfile
        /// </summary>
        public int SmtpServerDisplay { get; set; }
        /// <summary>
        ///  Indicates No. of Event diaplayed to UserProfile
        /// </summary>
        public int EventDisplay { get; set; }
        /// <summary>
        ///  Indicates No. of RebillRport diaplayed to UserProfile
        /// </summary>
        public int RebillReportDisplay { get; set; }
        /// <summary>
        ///  Indicates No. of DeclineType diaplayed to UserProfile
        /// </summary>
        public int DeclineTypeDisplay { get; set; }
        /// <summary>
        ///  Indicates No. of SalvageProfile diaplayed to UserProfile
        /// </summary>
        public int SalvageProfileDisplay { get; set; }

        /// <summary>
        ///  Indicates No. of TestCard diaplayed to UserProfile
        /// </summary>
        public int TestCardDisplay { get; set; }
        /// <summary>
        ///  Indicates No. of TaxProfile diaplayed to UserProfile
        /// </summary>
        public int TaxProfileDisplay { get; set; }
        /// <summary>
        ///  Indicates No. of TaxRuke diaplayed to UserProfile
        /// </summary>
        public int TaxRuleDisplay { get; set; }
        /// <summary>
        /// Indicates Status of userProfile
        /// </summary>
        public Nullable<bool> Status { get; set; }

        /// <summary>
        /// Indicates UserGroup Id as foreign key
        /// </summary>
        [ForeignKey("UserGroup")]        
        public int? UserGroupId { get; set; }
        public virtual UserGroup UserGroup { get; set; }

        /// <summary>
        ///  Indicates No. of BlackList diaplayed to UserProfile
        /// </summary>
        public int BlackListDisplay { get; set; }
        /// <summary>
        /// Timezone of USerProfile
        /// </summary>
        [ForeignKey("TimeZone")]
        [Display(Name = "Time Zone")]
        public int TimeZoneId { get; set; }
        public virtual StandardTimeZone TimeZone { get; set; }
        /// <summary>
        /// Indicates forot password token of userprofile
        /// </summary>
        public string ForgotPasswordToken { get; set; }
        public int PrepaidCardDisplay { get; internal set; }
    } 
}