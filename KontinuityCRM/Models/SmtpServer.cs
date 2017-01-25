using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace KontinuityCRM.Models
{
    [TrackChanges]
    public class SmtpServer
    {
        /// <summary>
        /// SmtpServer Id as primary  key
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Indicates Name of SmtpServer
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// Indicates the EmailAddress for SmtpServer
        /// </summary>
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "The e-mail address isn't in a correct format")]
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        /// <summary>
        /// Indicates Host Address of SmtpServer
        /// </summary>
        [Required]
        public string Host { get; set; }
        /// <summary>
        /// Indicates Port of SmtpServer
        /// </summary>
        [Required]
        public int Port { get; set; }
        /// <summary>
        /// Indicates Mail From Name of SmtpServer
        /// </summary>
        [Display(Name = "Mail From Name")]
        public string From { get; set; }

        /// <summary>
        /// Indicates UserName for SmtpServer
        /// </summary>
        public string UserName { get; set; }
        
        //[DataType(DataType.Password)]
        /// <summary>
        /// Indicates Password for the smtpServer
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Indicates whether Authorized or not for smtpServer
        /// </summary>
        [Display(Name="Use SMTP Authorization")]
        public bool Authorization { get; set; }
        /// <summary>
        /// Indiacates whether the smtpServer is published or not
        /// </summary>
        public bool Publish { get; set; }
        /// <summary>
        /// Indicates whether the smtpServer is Authenticated or not
        /// </summary>
        public bool Authenticated { get; set; }
        /// <summary>
        /// Last Updated date of smptServer
        /// </summary>
        public DateTime? LastUpdate { get; set; }
        /// <summary>
        /// Indicates User Id who created this smtpServer
        /// </summary>
        [ForeignKey("CreatedBy")]
        public int? CreatedUserId { get; set; }
        [Display(Name = "Created")]
        public virtual UserProfile CreatedBy { get; set; }
        /// <summary>
        /// Indicates User Id who updated this smtpServer
        /// </summary>
        [ForeignKey("UpdatedBy")]
        public int? UpdatedUserId { get; set; }
        [Display(Name = "Updated By")]
        public virtual UserProfile UpdatedBy { get; set; }


    }
}