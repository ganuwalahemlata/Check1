using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KontinuityCRM.Models
{

    public class RegisterExternalLoginModel
    {
        /// <summary>
        /// UserName of RegisterExternalLoginModel
        /// </summary>
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }
        /// <summary>
        /// Indicates Data for ExternalData
        /// </summary>
        public string ExternalLoginData { get; set; }
    }

    public class LocalPasswordModel
    {
        /// <summary>
        /// Indicates OldPasswod of LocalPasswordModel
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }
        /// <summary>
        /// Indicates NewPasswod of LocalPasswordModel
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }
        /// <summary>
        /// Indicates Confirm of LocalPasswordModel
        /// </summary>
        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginModel
    {
        /// <summary>
        /// User Name of LoginModel
        /// </summary>
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }
        /// <summary>
        /// Password of LoginModel
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        /// <summary>
        /// Remember of LoginModel
        /// </summary>
        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        /// <summary>
        /// UserName of RegisterModel
        /// </summary>
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }
        /// <summary>
        /// Password of RegisterModel
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        /// <summary>
        /// Confirm Password of RegisterModel
        /// </summary>
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        /// <summary>
        /// Email of RegisterModel
        /// </summary>
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "The e-mail address isn't in a correct format")]
        [Required]
        [DataType(DataType.EmailAddress)]
        //[RemoteWithServerSideAttribute("CheckEmailExists", "Validation", "admin", ErrorMessage = "This email is already taken.")] 
        public string Email { get; set; }
        /// <summary>
        /// Group Id of RegosterModel if exists
        /// </summary>
        [Display(Name = "Group")]        
        public int? GroupId { get; set; }
        /// <summary>
        /// Timezone of RegisterModel
        /// </summary>
        [Display(Name = "Time Zone")]
        [Required]
        public int? TimeZoneId { get; set; }
    }

    public class ExternalLogin
    {
        /// <summary>
        /// Provider for ExternalLogin
        /// </summary>
        public string Provider { get; set; }
        /// <summary>
        /// ProviderDisplayName for ExternalLogin
        /// </summary>
        public string ProviderDisplayName { get; set; }

        /// <summary>
        /// Provider UserId for External Login
        /// </summary>
        public string ProviderUserId { get; set; }
    }

    public class ChangePasswordModel
    {
        /// <summary>
        /// Password for ChangePasswordModel
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        /// <summary>
        /// ConfirmPassword for ChangePasswordModel
        /// </summary>
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ResetModel
    {
        /// <summary>
        /// Email for ResetModel
        /// </summary>
        [Required]
        [Display(Name = "Email address")]
        public string Email { get; set; }
    }

    public class AccountModel
    {
        /// <summary>
        /// Indicates Login Model for AccountModel
        /// </summary>
        public LoginModel LoginModel { get; set; }
        /// <summary>
        /// Indicates Register Model for AccountModel
        /// </summary>
        public RegisterModel RegisterModel { get; set; }
        /// <summary>
        /// Indicates ChangePassword Model for AccountModel
        /// </summary>
        public ChangePasswordModel ChangePasswordModel { get; set; }
        /// <summary>
        /// Indicates Reset Model for AccountModel
        /// </summary>
        public ResetModel ResetModel { get; set; }
    }
}