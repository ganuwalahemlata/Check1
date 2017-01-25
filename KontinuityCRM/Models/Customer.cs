using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace KontinuityCRM.Models
{
    public class Customer
    {
        /// <summary>
        /// CustomerId as primary key
        /// </summary>
        public int CustomerId { get; set; }
        /// <summary>
        /// Customer FirstName
        /// </summary>
        [Required]
        public string FirstName { get; set; }
        /// <summary>
        /// Customer LastName
        /// </summary>
        public string LastName { get; set; }
        /// <summary>
        /// Customer Address 1
        /// </summary>
        public string Address1 { get; set; }
        /// <summary>
        /// Customer Address 2
        /// </summary>
        public string Address2 { get; set; }
        /// <summary>
        /// Customer City
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// Customer Province
        /// </summary>
        public string Province { get; set; }
        /// <summary>
        /// Customer's Postal Code
        /// </summary>
        public string PostalCode { get; set; }
        /// <summary>
        /// Customer's Country
        /// </summary>
        public string Country { get; set; }
        /// <summary>
        /// Customer's Phone
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// Customer's Email
        /// </summary>
        public string Email { get; set; }        

        /// <summary>
        /// Customer's IP Address
        /// </summary>
        public string IPAddress { get; set; }
        /// <summary>
        /// Customer's Response provider
        /// </summary>
        public string ProviderResponse { get; set; }

        /// <summary>
        /// Customer's Status
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// Customer's FullName
        /// </summary>
        public string FullName { get { return string.Format("{0} {1}", this.FirstName, this.LastName); } }

        /// <summary>
        /// Delete Customer 
        /// </summary>
        /// <param name="uow"></param>
        public void Delete(Helpers.IUnitOfWork uow)
        {
            // remove the customer from the autoresponder customer list

            uow.CustomerRepository.Delete(this);
            uow.Save();
            //throw new NotImplementedException();
        }

        public virtual ICollection<Order> Orders { get; set; }
    }

    public class Contact
    {
        public int PartialId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string ProviderResponse { get; set; }
    }
}