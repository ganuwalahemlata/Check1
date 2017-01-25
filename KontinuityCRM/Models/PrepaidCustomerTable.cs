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
    public class PrepaidCard
    {
        /// <summary>
        /// PrepaidCustomerTable Id as primary key
        /// </summary>
        public int Id { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string First_Name { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string Last_Name { get; set; }

        [Required]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Required]
        [Display(Name = "City")]
        public string City { get; set; }

        [Required]
        [Display(Name = "State")]
        public string State { get; set; }

        [Required]
        [Display(Name = "Country")]
        public string Country { get; set; }

        [Required]
        [Display(Name = "Zip")]
        public string Zip { get; set; }

        [Required]
        [Display(Name = "Phone")]
        public string Phone { get; set; }
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Amount")]
        public string Amount { get; set; }
        [Required]
        [Display(Name = "Number")]
        public string Number { get; set; }

        [Required]
        [Display(Name = "Card Type")]
        public string PaymentType { get; set; }

        [Required]
        [Display(Name = "Expiration Month")]
        public string CreditCardExpirationMonth { get; set; }

        [Required]
        [Display(Name = "Expiration Year")]
        public string CreditCardExpirationYear { get; set; }


        [Required]
        [Display(Name = "CVV")]
        public string CreditCardCVV { get; set; }



        [Display(Name = "Date")]
        public DateTime? Date { get; set; }


        public string RemainingAmount { get; set; }

        public bool declined { get; set; }

        public bool Stop { get; set; }

    }

}