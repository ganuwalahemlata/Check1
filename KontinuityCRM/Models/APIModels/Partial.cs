using KontinuityCRM.Areas.HelpPage.ModelDescriptions;
using KontinuityCRM.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KontinuityCRM.Models.APIModels
{
    //[KontinuityCRM.Areas.HelpPage.ModelDescriptions.ModelName("PartialModel")]    

    /// <summary>
    /// 
    /// </summary>
    public class PartialCreateModel
    {
        /// <summary>
        /// First name
        /// </summary>
        [Required]
        [Display(Name = "First Name")]       
        public string FirstName { get; set; }

        /// <summary>
        /// Last name
        /// </summary>
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        /// <summary>
        /// Address
        /// </summary>
        public string Address1 { get; set; }

        /// <summary>
        /// Second Address
        /// </summary>
        public string Address2 { get; set; }

        /// <summary>
        /// City
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Province
        /// </summary>
        public string Province { get; set; }

        /// <summary>
        /// PostalCode
        /// </summary>
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }

        /// <summary>
        /// Country
        /// </summary>
        [Required]
        public string Country { get; set; }

        /// <summary>
        /// Phone
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [Required]
        public string Email { get; set; }
        
        /// <summary>
        /// AffiliateId
        /// </summary>
        public string AffiliateId { get; set; }

        /// <summary>
        /// SubId
        /// </summary>
        public string SubId { get; set; }

        /// <summary>
        /// Provides autoresponder info.
        /// </summary>
        [Required]
        [CheckProductExitsAttribute(ErrorMessage = "Product with id {0} not found")]
        public int ProductId { get; set; } // to get the autoresponder provider setting

        /// <summary>
        /// IP address
        /// </summary>
        public string IPAddress { get; set; }
       
    }

    /// <summary>
    /// Partial Create Model
    /// </summary>
    public class PartialAPIModel : PartialCreateModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public int PartialId { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class PartialToOrderModel
    {
        /// <summary>
        /// Products in this order
        /// </summary>
        public virtual ICollection<OrderProduct> OrderProducts { get; set; }

        /// <summary>
        /// IP address
        /// </summary>
        public string IPAddress { get; set; }

        
        /// <summary>
        /// Shipping Method Id
        /// </summary>
        [Required]
        [ValidateShippingMethod(ErrorMessage = "Shipping Method not found")]
        public int ShippingMethodId { get; set; }
        
        /// <summary>
        /// AffiliateId
        /// </summary>
        [Display(Name = "Affiliate Id")]
        public string AffiliateId { get; set; }
        
        /// <summary>
        /// SubId
        /// </summary>
        [Display(Name = "Sub Id")]
        public string SubId { get; set; }
        

        #region CreditCard

        /// <summary>
        /// Payment Type
        /// </summary>
        [Display(Name = "Payment Type")]
        public PaymentType PaymentType { get; set; }

        /// <summary>
        /// Credit Card Number
        /// </summary>
        [Required]
        [Display(Name = "Credit Card Number")]
        [CustomCreditCard] //[CreditCard]
        [CheckBlackList(ErrorMessage = "Credit Card Number is in the blacklist")]
        [DataType(DataType.CreditCard)]
        public string CreditCardNumber { get; set; }

        /// <summary>
        /// Credit Card Expiration Month
        /// </summary>
        [Display(Name = "Credit Card Expiration Month")]
        [Required]
        public Month CreditCardExpirationMonth { get; set; }

        /// <summary>
        /// Credit Card Expiration Year
        /// </summary>
        [Required]
        [Display(Name = "Credit Card Expiration Year")]
        public int CreditCardExpirationYear { get; set; }

        /// <summary>
        /// Credit Card CVV
        /// </summary>
        [Required]
        [Display(Name = "Credit Card CVV")]
        public string CreditCardCVV { get; set; }

        #endregion
    }
}