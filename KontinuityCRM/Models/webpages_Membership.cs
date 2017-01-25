using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace KontinuityCRM.Models
{
    /// <summary>
    /// webpages Membership.
    /// </summary>
    [Table("webpages_Membership")]
    public class webpages_Membership
    {
        /// <summary>
        /// User Id.
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UserId { get; set; }

        /// <summary>
        /// Create Date.
        /// </summary>
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// Confirmation Token.
        /// </summary>
        public string ConfirmationToken { get; set; }

        /// <summary>
        /// Is Confirmed.
        /// </summary>
        public bool? IsConfirmed { get; set; }

        /// <summary>
        /// Last Password FailureDate.
        /// </summary>
        public DateTime? LastPasswordFailureDate { get; set; }

        /// <summary>
        /// Password Failures SinceLastSuccess.
        /// </summary>
        public int PasswordFailuresSinceLastSuccess { get; set; }

        public string Password { get; set; }

        /// <summary>
        /// Password ChangedDate.
        /// </summary>
        public DateTime? PasswordChangedDate { get; set; }

        /// <summary>
        /// Password Salt.
        /// </summary>
        public string PasswordSalt { get; set; }

        /// <summary>
        /// Password VerificationToken.
        /// </summary>
        public string PasswordVerificationToken { get; set; }

        /// <summary>
        /// Password Verification TokenExpirationDate.
        /// </summary>
        public DateTime? PasswordVerificationTokenExpirationDate { get; set; }
    }

}