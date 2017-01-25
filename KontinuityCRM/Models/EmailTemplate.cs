using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KontinuityCRM.Models
{
    [TrackChanges]
    public class EmailTemplate
    {
        /// <summary>
        /// EmailTemplate Id as primary key
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// EmailTemplate Name
        /// </summary>
        [Required]
        [Display(Name = "Template Name")]
        public string Name { get; set; }
        /// <summary>
        /// EmailTemplate Description
        /// </summary>
        [Display(Name = "Template Description")]
        public string Description { get; set; }

        //public string To { get; set; }
        //public string From { get; set; }
        //public string Attachment { get; set; }
        /// <summary>
        /// Indicates the Notfiation Type for the Email Template
        /// </summary>
        public NotificationType Type { get; set; }
        /// <summary>
        /// Subject for the EmailTemplate
        /// </summary>
        [Required]
        [Display(Name="Email Subject")]
        public string Subject { get; set; }

        /// <summary>
        /// Indicates whether to puhlish Email
        /// </summary>
        public bool Publish { get; set; }
        /// <summary>
        /// Last Updated Time for the Email Template
        /// </summary>
        [Display(Name = "Last Update")]
        public DateTime? LastUpdate { get; set; }
        /// <summary>
        /// Indicates Last Update By
        /// </summary>
        [ForeignKey("UpdatedBy")]
        public int? UpdatedUserId { get; set; }
        [Display(Name = "Updated By")]
        public virtual UserProfile UpdatedBy { get; set; }
        /// <summary>
        /// HTML Body that goes in EmailTemplate
        /// </summary>
        [AllowHtml]
        [Display(Name = "HTML Template Body")]
        public string HtmlBody { get; set; }
        /// <summary>
        /// TextBocy for the EmailTemplate
        /// </summary>
        [AllowHtml]
        [Display(Name = "Plain Text Template Body")]
        public string TextBody { get; set; }
        /// <summary>
        /// Indicates the User who created the EmailTemplate
        /// </summary>
        [ForeignKey("CreatedBy")]
        public int? CreatedUserId { get; set; }
        [Display(Name = "Created By")]
        public UserProfile CreatedBy { get; set; }
    }

}