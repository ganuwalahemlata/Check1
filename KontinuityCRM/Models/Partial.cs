using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using KontinuityCRM.Helpers;
using System.ComponentModel.DataAnnotations.Schema;
using AutoMapper;
using System.Threading.Tasks;
using KontinuityCRM.Models.Enums;
using System.Globalization;

namespace KontinuityCRM.Models
{
    [KontinuityCRM.Areas.HelpPage.ModelDescriptions.ModelName("PartialModel")]
    [TrackChanges]
    public class Partial
    {
        /// <summary>
        /// partial id as primary key
        /// </summary>
        [Key]
        public int PartialId { get; set; }
        /// <summary>
        /// indicates partial firstname
        /// </summary>
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        /// <summary>
        /// indicates partial lastName
        /// </summary>
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        /// <summary>
        /// Indicates partial Address 1
        /// </summary>
        public string Address1 { get; set; }
        /// <summary>
        /// Indicates partial Address 2
        /// </summary>
        public string Address2 { get; set; }
        /// <summary>
        /// Indicates partial City
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// Indicates partial Province
        /// </summary>
        public string Province { get; set; }
        /// <summary>
        /// Indicates partial postalCode
        /// </summary>
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }
        /// <summary>
        /// Indicates partial Country
        /// </summary>
        public string Country { get; set; }
        /// <summary>
        /// Indicates partial Phone
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// Indicates partial Email
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Indicates partial IPAddress
        /// </summary>
        public string IPAddress { get; set; }
        /// <summary>
        /// Indicates partial AffiliateId
        /// </summary>
        public string AffiliateId { get; set; }
        /// <summary>
        /// Indicates partial SubId
        /// </summary>
        public string SubId { get; set; }
        /// <summary>
        /// Indicates partail's last updated date
        /// </summary>
        public DateTime? LastUpdate { get; set; }
        /// <summary>
        /// Indicates partial created date
        /// </summary>
        public DateTime Created { get; set; }
        /// <summary>
        /// Indicates product id as foreign key
        /// </summary>
        [Required]
        public int ProductId { get; set; } // to get the autoresponder provider setting
        public virtual Product Product { get; set; }

        public string ProviderResponse { get; set; }

        [Display(Name = "Full Name")]
        public string FullName { get { return string.Format("{0} {1}", this.FirstName, this.LastName); } }

        public async Task Create(IUnitOfWork uow, IWebSecurityWrapper wsw, IMappingEngine mapper)
        {
            // check if the partial already exists

            var partialExists = uow.PartialRepository.Get(p => p.Email == this.Email && p.Phone == this.Phone && p.PostalCode == this.PostalCode && p.ProductId == this.ProductId).FirstOrDefault();

            if (partialExists == null)
            {
                this.Product = uow.ProductRepository.Find(ProductId);
                if (this.Product != null)
                {
                    uow.PartialRepository.Add(this);
                    this.Created = DateTimeOffset.UtcNow.DateTime;
                    uow.Save(wsw.CurrentUserName); // to retrieve the partialid

                    #region ## Update Product AutoResponder prospect list ##

                    //if (ProductId.HasValue) // get the autoresponder
                    //{
                    this.Product = uow.ProductRepository.Find(ProductId);  //repo.FindProduct(ProductId.Value);
                    if (this.Product.AutoResponderProvider != null)
                    {
                        try
                        {
                            var autoresponder = this.Product.AutoResponderProvider.AutoResponder(mapper);
                            var response = await autoresponder.AddContact(mapper.Map<Partial, Contact>(this), this.Product.AutoResponderProspectId);
                            this.ProviderResponse = response;
                        }
                        catch (Exception ex)
                        {
                            this.ProviderResponse = ex.Message;
                        }
                        uow.PartialRepository.Update(this);
                        uow.Save(wsw.CurrentUserName);
                    }

                    //}
                    #endregion
                }
                else
                {

                }

            }

            else
            {
                // update the existing partial with the new data ??
                //uow.PartialRepository.Update(partialExists, this); throw exception due to id
                partialExists.LastUpdate = DateTimeOffset.UtcNow.DateTime;
                partialExists.IPAddress = this.IPAddress;
                partialExists.Address1 = this.Address1;
                partialExists.Address2 = this.Address2;
                partialExists.AffiliateId = this.AffiliateId;
                partialExists.City = this.City;
                partialExists.Country = this.Country;
                partialExists.FirstName = this.FirstName;
                partialExists.LastName = this.LastName;
                partialExists.SubId = this.SubId;

                uow.PartialRepository.Update(partialExists);
                uow.Save(wsw.CurrentUserName);

                this.PartialId = partialExists.PartialId; // set the partialId if there is an existing
            }



        }

        public async Task<Order> CreateOrder(/*IKontinuityCRMRepo repo,*/ IUnitOfWork uow, KontinuityCRM.Models.APIModels.PartialToOrderModel model, IMappingEngine mapper, IWebSecurityWrapper wsw)
        {
            // partial 
            // check for customer by email
            //var customer = uow.CustomerRepository.Get(c => c.Email == this.Email).SingleOrDefault(); //repo.Customers().SingleOrDefault(c => c.Email == this.Email);
            //if (customer == null) // if the customer doesn't exist
            //{
            //    /* ADD NEW CUSTOMER */
            //    customer = new Customer
            //    {
            //        FirstName = this.FirstName, // must not be null for create a Customer
            //        IPAddress = this.IPAddress,
            //        LastName = this.LastName,
            //        Phone = this.Phone,
            //        PostalCode = this.PostalCode,
            //        Province = this.Province,
            //        Email = this.Email,
            //        Country = this.Country,
            //        City = this.City,
            //        Address1 = this.Address1,
            //        Address2 = this.Address2,
            //        ProviderResponse = ProviderResponse,
            //    };
            //    // the customer is added when we add the orders since we add the order

            //    //uow.CustomerRepository.Add(customer);
            //    //repo.AddCustomer(customer);
            //}

            // create the order
            var order = new Order()
            {
                BillingAddress1 = this.Address1,
                BillingAddress2 = this.Address2,
                BillingCity = this.City,
                BillingCountry = this.Country,
                BillingFirstName = this.FirstName,
                BillingProvince = this.Province,
                BillingLastName = this.LastName,
                BillingPostalCode = this.PostalCode,

                ShippingAddress1 = this.Address1,
                ShippingAddress2 = this.Address2,
                ShippingCity = this.City,
                ShippingCountry = this.Country,
                ShippingFirstName = this.FirstName,
                ShippingLastName = this.LastName,
                ShippingProvince = this.Province,
                ShippingPostalCode = this.PostalCode,

                Phone = this.Phone,
                Email = this.Email,

                IPAddress = model.IPAddress,

                SubId = this.SubId,
                AffiliateId = this.AffiliateId,

                //CustomerId = customer.CustomerId,
                //Customer = customer,

                //ProcessorId = model.ProcessorId, // order goes with the product processor
                ShippingMethodId = model.ShippingMethodId,
                //OrderProducts = model.OrderProducts,

                PaymentType = model.PaymentType,
                CreditCardNumber = model.CreditCardNumber,
                CreditCardCVV = model.CreditCardCVV,
                CreditCardExpirationMonth = model.CreditCardExpirationMonth,
                CreditCardExpirationYear = model.CreditCardExpirationYear,
                isFromPartial = true, //This order has been created from a partial,
                PartialDate = this.Created.ToString(CultureInfo.InvariantCulture)

            };

            order.OrderProducts = new List<OrderProduct>();

            foreach (var op in model.OrderProducts)
            {
                order.OrderProducts.Add(mapper.Map<OrderProduct>(op));
            }

            order.CreatedUserId = wsw.CurrentUserId;
            try
            {
                await order.Create(uow, wsw, mapper); // create the order in db
            }
            catch (Exception)
            {
                { }
                //throw;
            }

            // delete this partial ?? yes
            //can't delete the partial unless we have an approval
            //var _order = new Order();
            //List<Transaction> _trans = new List<Transaction>();
            if (order.Status == OrderStatus.Approved)
            {

                uow.PartialRepository.Delete(this);
            }
            //else
            //{
            //    //_order = order;
            //    //foreach (var transa in order.Transactions)
            //    //{
            //    //    Transaction _transaction = new Transaction();
            //    //    _transaction.ProcessorId = transa.ProcessorId;
            //    //    _transaction.Processor = transa.Processor;
            //    //    _transaction.Message = transa.Message;
            //    //    _trans.Add(transa);
            //    //}
            //    ////If Order is not approved remove the order
            //    //uow.OrderRepository.Delete(order);

            //}

            //repo.RemovePartial(this);

            #region Update the autoresponder's list

            foreach (var op in order.OrderProducts) // update the product autoresponder's lists (partial & customer)
            {
                var product = op.Product ?? uow.ProductRepository.Find(op.ProductId); //repo.FindProduct(op.ProductId);

                // remove from the prospectid list / campaign. it must be because there, is a partial
                if (product.AutoResponderProviderId.HasValue)
                {
                    if (product.AutoResponderProvider == null) // check autoresponderprovider not null
                        product.AutoResponderProvider = uow.AutoResponderProviderRepository.Find(product.AutoResponderProviderId);


                    var autoresponder = product.AutoResponderProvider.AutoResponder(mapper);

                    var contact = mapper.Map<Customer, Contact>(order.Customer);

                    try
                    {
                        contact.ProviderResponse = this.ProviderResponse;
                        var response = await autoresponder.MoveContact(contact, product.AutoResponderProspectId, product.AutoResponderCustomerId);

                        order.Customer.ProviderResponse = response;
                    }
                    catch (Exception ex)
                    {
                        order.Customer.ProviderResponse = ex.Message;
                    }

                    // it doesn't need to update the customer 

                }

            }

            #endregion

            // record all changes
            uow.Save(wsw.CurrentUserName);
            //order = _order;
            //order.Transactions = _trans;
            return order;
        }

        public async Task Delete(IUnitOfWork uow, IWebSecurityWrapper wsw, IMappingEngine mapper)
        {
            #region Remove the contact from the prospect list in the autoresponder

            //if (ProductId.HasValue) // get the autoresponder
            //{
            if (this.Product == null)
                this.Product = uow.ProductRepository.Find(ProductId);

            if (this.Product.AutoResponderProvider != null)
            {
                try
                {
                    var autoresponder = this.Product.AutoResponderProvider.AutoResponder(mapper);

                    var response = await autoresponder.RemoveContact(mapper.Map<Partial, Contact>(this), this.Product.AutoResponderProspectId);
                    this.ProviderResponse = response;
                }
                catch (Exception ex)
                {
                    this.ProviderResponse = ex.Message;
                }
            }

            //}

            #endregion

            uow.PartialRepository.Delete(this);
            uow.Save(wsw.CurrentUserName);
        }
    }


}