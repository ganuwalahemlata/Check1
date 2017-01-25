using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using KontinuityCRM.Helpers;
using System.Transactions;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using KontinuityCRM.Models.Enums;
using KontinuityCRM.Models.Gateways;
using System.Collections.Specialized;
using AutoMapper;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Text;
using System.Data.SqlClient;

namespace KontinuityCRM.Models
{
    [TrackChanges]
    public class Order
    {

        [Key]
        public int OrderId { get; set; }

        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

        [Required]
        [Display(Name = "Shipping First Name")]
        public string ShippingFirstName { get; set; }
        [Required]
        [Display(Name = "Shipping Last Name")]
        public string ShippingLastName { get; set; }
        [Required]
        [Display(Name = "Shipping Address")]
        public string ShippingAddress1 { get; set; }

        [Display(Name = "Shipping Address 2")]
        public string ShippingAddress2 { get; set; }
        [Required]
        [Display(Name = "Shipping City")]
        public string ShippingCity { get; set; }

        [Display(Name = "Shipping Province")]
        public string ShippingProvince { get; set; }
        [Required]
        [Display(Name = "Shipping Postal Code")]
        public string ShippingPostalCode { get; set; }
        [Required]
        [Display(Name = "Shipping Country")]
        public string ShippingCountry { get; set; }
        [Required]
        public string Phone { get; set; } // shipping or billing client ?
        [Required]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "The e-mail address isn't in a correct format")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Billing First Name")]
        public string BillingFirstName { get; set; }
        [Required]
        [Display(Name = "Billing Last Name")]
        public string BillingLastName { get; set; }
        [Required]
        [Display(Name = "Billing Address1")]
        public string BillingAddress1 { get; set; }

        [Display(Name = "Billing Address2")]
        public string BillingAddress2 { get; set; }
        [Required]
        [Display(Name = "Billing City")]
        public string BillingCity { get; set; }

        [Display(Name = "Billing Province")]
        public string BillingProvince { get; set; }
        [Required]
        [Display(Name = "Billing Postal Code")]
        public string BillingPostalCode { get; set; }
        [Required]
        [Display(Name = "Billing Country")]
        public string BillingCountry { get; set; }

        [Required]
        public int ShippingMethodId { get; set; }
        [Display(Name = "Shipping Method")]
        public virtual ShippingMethod ShippingMethod { get; set; }

        public int? ProcessorId { get; set; }
        public virtual Processor Processor { get; set; }

        public bool? Shipped { get; set; }

        //public decimal SubTotal { get; set; }

        //[Display(Name = "Shipping Total")]
        //public decimal ShippingTotal { get; set; }

        /// <summary>
        /// Tax applied to this order
        /// </summary>
        public decimal Tax { get; set; }

        [Display(Name = "RMA Number")]
        public string RMANumber { get; set; }

        public DateTime? RMANumberCreatedOn { get; set; }
        //[Display(Name = "Rebill Price")]
        //public double? RebillPrice { get; set; }
        [ForeignKey("RMAReason")]
        [Display(Name = "RMA Reason")]
        public int? RMAReasonId { get; set; }

        public virtual RMAReason RMAReason { get; set; }
        public string IPAddress { get; set; }

        [Display(Name = "Affiliate Id")]
        public string AffiliateId { get; set; }

        [Display(Name = "Sub Id")]
        public string SubId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsChargeBack { get { return ChargebackDate.HasValue; } }

        /// <summary>
        /// 
        /// </summary>
        [DisplayName("Chargeback Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? ChargebackDate { get; set; }

        [Display(Name = "Payment Type")]
        public PaymentType PaymentType { get; set; }

        [Display(Name = "Payment Type")]
        public int? PaymentTypeId { get; set; }

        public virtual PaymentTypes PaymentTypes { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        [Display(Name = "Credit Card Number")]
        [RemoteWithServerSide("CheckCreditCard", "Validation")]  //[CreditCard]
        [DataType(DataType.CreditCard)]
        public string CreditCardNumber { get; set; }

        [NotMappedAttribute]
        public string CreditCardNumberX
        {
            get
            {
                if (CreditCardNumber == null) // tmp patch
                {
                    return "XXXX-XXXX-XXXX-";
                }

                return "XXXX-XXXX-XXXX-"
                       + CreditCardNumber.Substring(CreditCardNumber.Length - 4, 4);
            }
            set
            {
                if (!value.StartsWith("XXXX-XXXX-XXXX-"))
                {
                    this.CreditCardNumber = value;
                }

            }
        }

        public string BIN { get; set; }

        public string LastFour { get; set; }


        [Display(Name = "Credit Card Expiration Month")]
        public Month CreditCardExpirationMonth { get; set; }

        [Display(Name = "Credit Card Expiration Year")]
        public int CreditCardExpirationYear { get; set; }
        [Required]
        [Display(Name = "Credit Card CVV")]
        public string CreditCardCVV { get; set; }

        [Index]
        public OrderStatus Status { get; set; }

        public virtual ICollection<OrderProduct> OrderProducts { get; set; }

        public virtual ICollection<OrderNote> Notes { get; set; }

        public virtual ICollection<Transaction> Transactions { get; set; }

        public virtual PagedList.IPagedList<Transaction> TransactionsNew { get; set; }

        public virtual ICollection<OrderTimeEvent> TimeEvents { get; set; }

        [ForeignKey("CreatedBy")]
        public int? CreatedUserId { get; set; }

        [Display(Name = "Created")]
        public virtual UserProfile CreatedBy { get; set; }

        [Display(Name = "Last Update")]
        public DateTime? LastUpdate { get; set; }

        public DateTime Created { get; set; }

        [ForeignKey("Parent")]
        public int? ParentId { get; set; }
        public virtual Order Parent { get; set; }

        /// <summary>
        /// Total order amount
        /// </summary>
        public decimal Total
        {
            get;
            set;
        }

        public bool IsTest { get; set; }

        public bool IsPrepaid { get; set; }

        public decimal SubTotal
        {
            get;
            set;
        }

        //[Index]
        //[DefaultValue(false)]
        //public bool HasSubscription { get; set; }

        public int? BalancerId { get; set; } // to track if there is a balancer from which the preserved processor came by
        public virtual Balancer Balancer { get; set; }

        public DateTime? CaptureDate { get; set; }

        /// <summary>
        /// Indicates the rebill depth
        /// </summary>
        public int Depth { get; set; }

        /// <summary>
        /// Indicates whether the order has been created from Partial or not
        /// </summary>
        public bool isFromPartial { get; set; }
        /// <summary>
        /// Indicates createDate of partial if order is created from partial
        /// </summary>
        public string PartialDate { get; set; }


        /// <summary>
        /// Top ParentId of this order.
        /// </summary>
        [Display(Name = "Top Parent Order")]
        public int? TopParentId { get; set; }

        [NotMapped]
        public string OrderNote { get; set; }
        //public decimal GetTotal()
        //{
        //    // calculated fields

        //    //Sub Total = total of all the products' price
        //    //shipping = shipping price
        //    //tax = sum of all tax configured (right now, none of this logic is built, so this will be 0)
        //    //total = sub total + shipping + tax

        //    //decimal total = 0;

        //    //foreach (var op in OrderProducts)
        //    //{
        //    //    total += op.Price.Value; // subtotal
        //    //}

        //    //total += this.ShippingMethod.Price; // shipping price
        //    //total += 0; // tax


        //    //return total;


        //}


        #region Calculated Properties

        /// <summary>
        /// Sum of all product quantities on this order
        /// </summary>
        public int Quantity
        {
            get
            {
                return this.OrderProducts == null ? 0 : OrderProducts.Sum(o => o.Quantity);
            }
        }

        /// <summary>
        /// Indicates were the order was created by the rebill process or not
        /// </summary>
        public bool IsRebill
        {
            get
            {
                return this.ParentId.HasValue;
            }
        }

        /// <summary>
        /// Indicates were some product have been rebilled (this order has child orders, recurring was inherited)
        /// </summary>
        public bool HasRebills
        {
            get
            {
                return OrderProducts != null && OrderProducts.Any(op => op.ChildOrderId.HasValue);
            }
        }

        #endregion

        //private decimal CalculateSubTotalAmount()
        //{
        //    decimal amount = 0;

        //    foreach (var op in OrderProducts)
        //    {
        //        var price = op.Price ?? 0; // op.Product.Price;

        //        if (op.Order.IsPrepaid && op.ReAttempts > 0)
        //        {
        //            // see if this product has the special 'prepaid decline profile'
        //            var decline
        //        }
        //        else 
        //        {
        //            price -= op.RebillDiscount;
        //        }



        //        amount += price * op.Quantity;
        //    }

        //    this.SubTotal = amount;
        //    return amount;
        //}

        ///// <summary>
        ///// Throws exception if the shippingmethod and products are not included in this order
        ///// </summary>
        ///// <returns></returns>
        //private decimal CalculateTotalAmount()
        //{
        //    this.Total = this.SubTotal + ShippingMethod.Price;
        //    return this.Total;
        //}        

        /// <summary>
        /// Find the processor that will process this order
        /// </summary>
        /// <param name="uow"></param>
        /// <param name="processor"></param>
        /// <param name="bp"></param>
        internal void FindProcessor(IUnitOfWork uow, out Processor processor, out BalancerProcessor bp, out int? balancerId)
        {
            #region Find the Processor
            // TODO this needs to be refactored and cleaned up with the variant logic

            // always if the processorId has value was because it came from a balancer and it was preserved in it
            // for ence if it has value then (=>) balancerId also has value
            processor = this.ProcessorId.HasValue ? this.Processor ?? uow.ProcessorRepository.Find(this.ProcessorId) : null;

            bp = this.BalancerId.HasValue ? this.Processor.BalancerProcessors.Single(b => b.BalancerId == this.BalancerId) : null;

            balancerId = this.BalancerId;

            // if no procsesor preserved, then we first look for variants


            // variant logic
            // get product variant off first product
            var firstproduct = this.OrderProducts.First().Product;
            var pv = uow.ProductVariantRepository.Get(v => v.ProductId == firstproduct.ProductId && v.Country.CountryAbbreviation == this.ShippingCountry).FirstOrDefault();

            if (pv != null)
            {
                var pvprocessor = uow.VariantExtraFieldRepository.Get(p => p.ProductVariantId == pv.ProductVariantId).Where(p => p.FieldName == "Processor").FirstOrDefault();
                if (pvprocessor != null)
                {
                    processor = uow.ProcessorRepository.Find(Convert.ToInt32(pvprocessor.FieldValue));
                }
                else
                {
                    var pvbalancer = uow.VariantExtraFieldRepository.Get(p => p.ProductVariantId == pv.ProductVariantId).Where(p => p.FieldName == "Balancer").FirstOrDefault();
                    if (pvbalancer != null)
                    {
                        var variantBalancer = uow.BalancerProcessorRepository.Find(Convert.ToInt32(pvbalancer.FieldValue));
                        bp = variantBalancer.Balancer.SelectProcessor(this.Total);
                        processor = variantBalancer.Processor;
                    }

                }
            }

            // no variants, we can look for the balancer / processors from the product config

            if (processor == null)
            {
                // get processor / balancer processor from the first product 

                if (firstproduct.LoadBalancer)
                {
                    bp = firstproduct.Balancer.SelectProcessor(this.Total);

                    if (bp != null)
                    {
                        processor = bp.Processor;
                        balancerId = bp.BalancerId;
                    }

                }
                else
                {
                    processor = firstproduct.Processor;
                }
            }

            #endregion

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uow"></param>
        /// <param name="mapper"></param>
        /// <param name="retryProcessor"></param>
        /// <returns></returns>
        public async Task Process(IUnitOfWork uow, IWebSecurityWrapper wsw, IMappingEngine mapper, Processor retryProcessor = null)
        {
            INLogger _logger = new NInLogger();
            Processor processor = null; // processor that will be used
            BalancerProcessor bp = null; // balancer processor if used
            int? balancerId = null; // balancer if there is a balancerprocessor

            _logger.LogInfo("Going to set BIN and Last Four from  CreditCardNumber.");

            this.BIN = this.CreditCardNumber.Substring(0, 6);
            this.LastFour = this.CreditCardNumber.Substring(this.CreditCardNumber.Length - 4);

            _logger.LogInfo("Step1: Process mthod called. finding processor ");

            if (retryProcessor != null)
            {
                _logger.LogInfo("Step 1(a):Retry Processor Found");
                processor = retryProcessor;

            }
            else
            {
                this.FindProcessor(uow, out processor, out bp, out balancerId);

            }

            _logger.LogInfo("step2:Processor found succeffully. balancer id:" + balancerId);

            bool success = this.IsTest;
            bool? captureSuccess = null;
            string declinereason = string.Empty;
            Transaction transaction = null, captureTransaction = null;
            if (this.Transactions == null)
                this.Transactions = new List<Transaction>();

            GatewayModel gatewayModel = null;
            if (!success)
            {
                if (processor != null)
                {
                    _logger.LogInfo("step3:Processor is not null and test is not success. Now checking for pre-auth amount");
                    gatewayModel = processor.GatewayModel(mapper);

                    try
                    {
                        #region Implementaion of Pre Auth Amount
                        if (processor.UsePreAuthorizationFilter)
                        {
                            _logger.LogInfo("step4: user pre authorize filter is enabled. Now authorizing transaction");

                            transaction = gatewayModel.Authorize(this, processor);
                            if (!transaction.Success)
                            {
                                _logger.LogInfo("step5:transaction is not success. now going to decline order");
                                #region Decline Order
                                //Decline the Order
                                // if the transactions has a decline reason wildcard it must have been failed so
                                // either way we are canceling the subscription here??

                                // FAIL // clear subscription on this order if the order fails then set No to the recurring
                                foreach (var op in OrderProducts)
                                {
                                    op.NextDate = null;
                                    op.Recurring = false;

                                    uow.OrderNoteRepository.Add(new OrderNote
                                    {
                                        OrderId = op.OrderId,
                                        NoteDate = DateTime.UtcNow,
                                        Note = "Stop Recurring",
                                    });
                                }

                                if (wsw != null && wsw.CurrentUserName != null)
                                {
                                    uow.Save(wsw.CurrentUserName);
                                }
                                else
                                {
                                    uow.Save();
                                }


                                if (captureSuccess == false)
                                {

                                    this.Status = OrderStatus.FailedCapture;

                                    if (captureTransaction != null)
                                    {
                                        declinereason = "Transaction Message: " + captureTransaction.Message;
                                    }

                                    this.Notes.Add(new OrderNote()
                                    {
                                        NoteDate = DateTime.UtcNow,
                                        Note = "Order Capture Failed. " + declinereason,
                                    });
                                    _logger.LogInfo("step6:Capture success is false, Order capture failure. Made next date, recuuring empty and added a note:" + declinereason);
                                }
                                else
                                {
                                    this.Status = OrderStatus.Declined;

                                    // get the decline reason from the transaction if it was created
                                    if (transaction != null)
                                    {
                                        declinereason = "Transaction Message: " + transaction.Message;
                                    }

                                    this.Notes.Add(new OrderNote()
                                    {
                                        NoteDate = DateTime.UtcNow,
                                        Note = string.Format("Order Declined ({0})", declinereason),
                                    });
                                    _logger.LogInfo("step6:Capture success is true, Order status marked as declined . Made next date, recuuring empty and added a note:" + declinereason);
                                }
                                #endregion
                            }
                            else
                            {
                                _logger.LogInfo("step5:transaction is success");
                                gatewayModel.Void(transaction);
                            }

                        }
                        else
                            _logger.LogInfo("step4:pre authorize filter is not enabled");
                        #endregion

                        #region Flow to Process an order
                        _logger.LogInfo("step7:Flow to process the order started");
                        if (!processor.UsePreAuthorizationFilter || (processor.UsePreAuthorizationFilter && transaction.Success))
                        {
                            _logger.LogInfo("step8:User pre auth filter is not enabled:" + processor.UsePreAuthorizationFilter + ", or trasaction is sucess and pre auth filter is enabled:");
                            if (processor.Type == ProcessorType.Authorize)
                            {
                                _logger.LogInfo("step8:Processor type is AUTHORIZE:" + processor.Type + ", and now authorizing");
                                transaction = gatewayModel.Authorize(this, processor);

                                transaction.BalancerId = balancerId;
                                this.Transactions.Add(transaction);
                                _logger.LogInfo("step9:Authorization is success. Transaction created");
                                if (transaction.Success)
                                {
                                    _logger.LogInfo("Step10:Transaction is success");
                                    if (!(processor.CaptureOnShipment || processor.CaptureDelayHours > 0)) // capture right away
                                    {
                                        _logger.LogInfo("step11:Capture shippment is false or capture delay hour less tha 0");
                                        captureTransaction = gatewayModel.Capture(transaction); //processor.Gateway.Capture(transaction);
                                        captureSuccess = captureTransaction.Success;
                                        this.Transactions.Add(captureTransaction);

                                        if (captureTransaction.Success)
                                        {
                                            this.Notes.Add(new OrderNote()
                                            {
                                                NoteDate = DateTime.UtcNow,
                                                Note = string.Format("Order Captured on {0}", processor.Name)
                                            });
                                        }
                                    }
                                    else if (!processor.CaptureOnShipment && processor.CaptureDelayHours > 0) // set the de
                                    {
                                        _logger.LogInfo("step11:Capture shippment is false and capture delay hour greater tha 0");
                                        this.CaptureDate = DateTime.Now.AddHours(processor.CaptureDelayHours.Value);
                                    }
                                }
                                else
                                    _logger.LogInfo("step10:Transaction is not success");

                            }
                            else
                            {
                                _logger.LogInfo("step8:Processor type is not authorize. Now sale method calling");
                                try
                                {
                                    transaction = gatewayModel.Sale(this, processor);
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogException("Failded sale....reason could be", ex);
                                }
                                //processor.Gateway.Sale(this, processor);
                                if (transaction != null)
                                {
                                    transaction.BalancerId = balancerId;
                                    this.Transactions.Add(transaction);
                                    _logger.LogInfo("step9:Sale is success and transaction created");
                                }
                                else
                                    _logger.LogInfo("step9: Transaction is null.... not adding into transactions");

                            }

                            success = transaction.Success && captureSuccess != false;
                        }
                        else
                            _logger.LogInfo("step8:User pre auth filter is  enabled:" + processor.UsePreAuthorizationFilter + ", or trasaction is failure and pre auth filter is disabled");
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        _logger.LogInfo("Failed: Calculating preauthorize amount. Possible reason could be:" + ex);
                        //throw ex;
                        declinereason = ex.Message;
                    }
                }
                else
                {

                    // no available processor so this is the reason
                    declinereason = "balancer allocation limit reached. Processor not found";
                    _logger.LogInfo("step3: Processor is null and possible reason could be:" + declinereason);
                }
            }

            if (success && !IsTest)
            {
                _logger.LogInfo("step3:Its not test and success");
                #region Ship the products if they are shippable

                // if the order gets approved and the product is shippable then send the product to the fulfillment provider
                // What is the role of the shipping method of the order ????
                // DO WE NEED TO HANDLER THE FULFILLMET DELAY OR IT IS TAKE CARE BY THE PROVIDER ????

                // shippable products group by fulfillment provider

                if ((!processor.ShipmentOnCapture || captureSuccess != false)) // if shipmentoncapture => captureSucess != false
                {
                    _logger.LogInfo("step4:It processor shipment on capture fail or capture sucess is true");
                    // get shippable products
                    var shippableProducts = this.OrderProducts.Where(op => op.Product.IsShippable);

                    if (shippableProducts.Any())
                    {
                        _logger.LogInfo("step5:shipment products available");

                        this.Shipped = false;

                        #region Ship all products or schedule the fulfillment for each of them

                        _logger.LogInfo("step6:Shipping all products or scheduling fulfillment for each of them");

                        foreach (var group in shippableProducts.GroupBy(op => op.Product.FulfillmentProvider))
                        {

                            var fulfillmentProvider = group.Key;

                            if (fulfillmentProvider != null)
                            {
                                if (!(fulfillmentProvider.Delay > 0) || processor.ShipmentOnCapture) // if there is no delay in the fulfillment
                                {
                                    var fulfillment = fulfillmentProvider.Fulfillment(mapper);

                                    try
                                    {
                                        _logger.LogInfo("step7" + fulfillmentProvider + ". No fulfillment delay or processor has captured shipment. Sending order now");
                                        var shipment = await fulfillment.SendOrder(this, group);
                                        _logger.LogInfo("step8:Order sent the result of shipment is:" + shipment);
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.LogInfo("step8: Failed sending order :" + this.OrderId + " possible reason could be:" + ex);
                                        foreach (var op in group)
                                        {
                                            op.Shipped = false;
                                            op.FulfillmentProviderResponse = ex.Message;
                                            op.FulfillmentDate = null;
                                        }
                                        //break; // continue sending the rest of the products 
                                    }
                                }
                                else
                                {
                                    _logger.LogInfo("step7" + fulfillmentProvider + ". Either fulfillment  has delay or processor has not captured shipment. Scheduling the order now");
                                    foreach (var op in group) // schedule the fulfillment job for this products 
                                    {
                                        op.FulfillmentDate = DateTime.Now.AddHours(fulfillmentProvider.Delay.Value);
                                    }
                                }
                            }
                            else
                            {
                                this.Notes.Add(new OrderNote()
                                {
                                    NoteDate = DateTime.UtcNow,
                                    Note = "No fulfillment provider",
                                });
                            }
                        }

                        #endregion

                        if (shippableProducts.Any(op => op.Shipped == false)) // ALL FAIL - all shipping fail for the whole or some product
                        {
                            this.Status = OrderStatus.ShippedException;
                            this.Notes.Add(new OrderNote()
                            {
                                NoteDate = DateTime.UtcNow,
                                Note = "Order Shipped Fail",
                            });
                            _logger.LogInfo("step8:All or some of shipment failed");
                        }
                        else if (shippableProducts.All(op => op.Shipped == true)) // ALL SUCCESS - whole order shipped we need to send all products to realize the capture
                        {
                            this.Shipped = true;

                            this.Notes.Add(new OrderNote()
                            {
                                NoteDate = DateTime.UtcNow,
                                Note = "Order Shipped",
                            });
                            _logger.LogInfo("step8:All of shipment is success");
                            // see the capture on fulfillment

                            // if capture on shipment obviosly the processor type is authorize

                            if (processor.CaptureOnShipment && processor.Type == ProcessorType.Authorize)
                            {
                                //if (processor.CaptureDelayHours > 0)  // cature on shipment overrides capture delay
                                //{
                                //    CaptureDate = DateTime.Now.AddHours(processor.CaptureDelayHours.Value);
                                //}
                                //else
                                //{
                                try
                                {
                                    _logger.LogInfo("step9:As processor type is authorize and capture shipment is true therefore, capture method being called for transaction");
                                    captureTransaction = gatewayModel.Capture(transaction); //processor.Gateway.Capture(transaction);
                                    success = captureTransaction.Success;
                                    captureSuccess = captureTransaction.Success;
                                    this.Transactions.Add(captureTransaction);

                                    if (captureTransaction.Success)
                                    {
                                        _logger.LogInfo("step10:Capture transaction is true");
                                        this.Notes.Add(new OrderNote()
                                        {
                                            NoteDate = DateTime.UtcNow,
                                            Note = string.Format("Order Captured on {0}", processor.Name),
                                        });
                                    }
                                    else
                                        _logger.LogInfo("step10:Capture transaction failed");
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogInfo("Capturing shipment failed and possible reason could be:" + ex);
                                    success = false;
                                    captureSuccess = false;
                                    declinereason = ex.Message;

                                }

                                //}

                            }
                        }

                        else if (shippableProducts.Any(op => op.Shipped == true)) // PARTIAL SUCCESS partial shipment
                        {
                            _logger.LogInfo("step7:Either all or some of products have been shipped");
                            this.Notes.Add(new OrderNote()
                            {
                                NoteDate = DateTime.UtcNow,
                                Note = "Order Partial Shipping",
                            });
                        }

                        // else 
                        // there was a delay in the shipment or there is a delay in the capture and shipmentoncapture

                    }
                    _logger.LogInfo("step5:No shippable products available");
                    // END IF no products are shippable in this order

                }
                else
                    _logger.LogInfo("step4:It processor shipment on capture true or capture sucess is false");

                #endregion
            }

            if (success)
            {
                _logger.LogInfo("step3:Its only success. Now sending order email.");
                try
                {
                    EmailHelper.SendOrderEmail(uow, mapper, NotificationType.OrderConfirmation, this.OrderId);
                    _logger.LogInfo("step4:Order email sent");
                }
                catch (Exception ex)
                {
                    _logger.LogInfo("Failed sending order email. Possible reason could be:" + ex);
                }

                if (!IsTest)
                {
                    _logger.LogInfo("step4:Its not a test. Updating balancer processor if load is balancer");
                    #region update BalancerProcessor if is load balancer (Maybe we should do this after sale or capture only ? NO also on auth )
                    if (bp != null) // && processor.Type == ProcessorType.Sale)
                    {
                        if (bp.IsPreserved)
                        {
                            _logger.LogInfo("step5:Balancer processor is not null and its preserved");

                            // if we're using a balancer and the processor is preserved and it was a success then set it for order to keep 
                            // if the processor is preserved we stick with it

                            if (!this.ProcessorId.HasValue)
                                this.ProcessorId = processor.Id;

                            if (!this.BalancerId.HasValue)
                                this.BalancerId = bp.BalancerId;
                        }

                        bp.Allocation -= Total;
                        bp.ProcessedAllocation += Total;
                        bp.Initials++;
                        try
                        {
                            _logger.LogInfo("step6:Updating balancer processor");
                            uow.BalancerProcessorRepository.Update(bp);
                            _logger.LogInfo("step7:Balancer processor updated successfully");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogException("Failed updating balancer processor", ex);
                        }

                        //repo.EditBalancerProcessor(bp);
                    }

                    #endregion

                    #region Update NextDate & NextProduct
                    _logger.LogInfo("Updating next date and next product");
                    foreach (var op in this.OrderProducts)
                    {
                        var product = op.Product;//?? repo.FindProduct(op.ProductId);

                        if (!op.NextDate.HasValue && product.IsSubscription && op.Recurring)
                        {
                            op.NextProductId = product.RecurringProductId;
                            op.NextDate = KontinuityCRMHelper.GetNextDate(DateTime.Now, op.BillType.Value, op.BillValue.Value);
                        }
                        else
                        {
                            // set the recurring to false if the product has no subscription
                            op.NextDate = null;
                            op.Recurring = false;

                                uow.OrderNoteRepository.Add(new OrderNote
                                {
                                    OrderId = op.OrderId,
                                    NoteDate = DateTime.UtcNow,
                                    Note = "Stop Recurring",
                                });
                        }
                        if (wsw != null && wsw.CurrentUserName != null)
                        {
                            uow.Save(wsw.CurrentUserName);
                        }
                        else
                        {
                            uow.Save();
                        }
                    }
                    #endregion
                }

                this.Status = OrderStatus.Approved;

                this.Notes.Add(new OrderNote()
                {
                    NoteDate = DateTime.UtcNow,
                    Note = string.Format("Order Approved for {0} on {1}", transaction != null ? transaction.Amount : Total, processor != null ? processor.Name : "Test Transaction"),
                });
                if (wsw != null && wsw.CurrentUserName != null)
                {
                    uow.Save(wsw.CurrentUserName);
                }
                else
                {
                    uow.Save();
                }

            }

            else // FAIL
            {
                _logger.LogInfo("step3:Its fail. Making reccurring false");
                // if the transactions has a decline reason wildcard it must have been failed so
                // either way we are canceling the subscription here??

                // FAIL // clear subscription on this order if the order fails then set No to the recurring
                foreach (var op in OrderProducts)
                {
                    op.NextDate = null;
                    op.Recurring = false;

                    uow.OrderNoteRepository.Add(new OrderNote
                    {
                        OrderId = op.OrderId,
                        NoteDate = DateTime.UtcNow,
                        Note = "Stop Recurring",
                    });
                }

                uow.Save();


                if (captureSuccess == false)
                {
                    _logger.LogInfo("step4: capture success is false");
                    this.Status = OrderStatus.FailedCapture;

                    if (captureTransaction != null)
                    {
                        declinereason = "Transaction Message: " + captureTransaction.Message;
                    }
                    string processorName = processor != null ? processor.Name : "";

                    if (this.Notes != null)
                    {
                        this.Notes.Add(new OrderNote()
                        {
                            NoteDate = DateTime.UtcNow,
                            Note = string.Format("Order Capture Failed ({0}) for on {1}. ", declinereason, processorName)
                        });
                    }
                }
                else
                {
                    _logger.LogInfo("step4: capture success is not false. Declining status");
                    this.Status = OrderStatus.Declined;

                    // get the decline reason from the transaction if it was created
                    if (transaction != null)
                    {
                        declinereason = "Transaction Message: " + transaction.Message;
                    }

                    this.Notes.Add(new OrderNote()
                    {
                        NoteDate = DateTime.UtcNow,
                        Note = string.Format("Order Declined ({0}) for {1} on {2}", declinereason, transaction.Amount, processor.Name),
                    });
                }
            }

            if (!this.IsRebill && !IsTest)
            {
                _logger.LogInfo("step5: Its not a rebill and not a test");
                if (this.TimeEvents == null)
                    this.TimeEvents = new List<OrderTimeEvent>();

                var time = DateTimeOffset.UtcNow;
                foreach (var op in this.OrderProducts)
                {
                    this.TimeEvents.Add(new OrderTimeEvent
                    {
                        OrderId = this.OrderId,
                        Time = time,
                        ProductId = op.ProductId,
                        Action = null,
                        Event = this.Status == OrderStatus.Approved ? OrderEvent.Approved : OrderEvent.Declined,
                        AffiliateId = this.AffiliateId,
                        SubId = this.SubId,

                    });
                }
                _logger.LogInfo("step5: Added order time events and its end of process method huuhhhh.............");
            }

        }

        public void SendEmail(IUnitOfWork uow, IMappingEngine mapper, NotificationType notificationType)
        {
            var order = uow.OrderRepository.Find(this.OrderId);
            bool success = true;

            Processor processor = null; // processor that will be used
            BalancerProcessor bp = null; // balancer processor if used
            int? balancerId = null; // balancer if there is a balancerprocessor

            order.FindProcessor(uow, out processor, out bp, out balancerId);

            var gatewayModel = processor.GatewayModel(mapper);

            // TODO
            // Retarded logic - this needs to populate product specific values, not send an email for every product
            foreach (var op in order.OrderProducts)
            {
                var product = op.Product;
                var _event = product.ProductEvents.FirstOrDefault(p => p.Event.Type == notificationType);
                if (_event != null)
                {

                    var sbhtmlbody = KontinuityCRMHelper.PopulateTokens(_event.Event.Template.HtmlBody, order, op, processor, bp, gatewayModel, balancerId);
                    var sbbody = KontinuityCRMHelper.PopulateTokens(_event.Event.Template.TextBody, order, op, processor, bp, gatewayModel, balancerId);

                    // refactored all token replacement to KontinuityCRMHelper.PopulateTokens

                    KontinuityCRMHelper.SendMail(
                       MailServer: _event.Event.SmtpServer.Host,
                       MailPort: _event.Event.SmtpServer.Port,
                       MailUsername: _event.Event.SmtpServer.UserName,
                       MailPassword: _event.Event.SmtpServer.Password,
                       MailEnableSsl: _event.Event.SmtpServer.Authorization,
                       from: _event.Event.SmtpServer.Email,
                       to: order.Email,
                       subject: _event.Event.Template.Subject,
                       textBody: sbbody.ToString(),
                       htmlBody: sbhtmlbody.ToString()
                        //body: _event.Template.HtmlBody,
                        //isBodyHtml: true
                       );
                }
            }
        }


        /// <summary>
        /// Dont take into account the processor delay. Set the CaptureDate to null
        /// </summary>
        /// <param name="uow"></param>
        /// <param name="mapper"></param>
        /// <returns></returns>
        public Transaction Capture(IUnitOfWork uow, IMappingEngine mapper)
        {
            var authtransaction = Transactions.Single(t => t.Type == Models.TransactionType.Auth && t.Success);
            return Capture(uow, authtransaction, mapper);
        }

        /// <summary>
        /// Dont take into account the processor delay. Set the CaptureDate to null
        /// </summary>
        /// <param name="uow"></param>
        /// <param name="authtransaction"></param>
        /// <param name="mapper"></param>
        /// <returns></returns>
        public Transaction Capture(IUnitOfWork uow, Transaction authtransaction, IMappingEngine mapper)
        {
            CaptureDate = null; // prevent further captures

            // make the transaction using the same processor that approved the auth transaction 
            /* if we have the approved the approved order by a reattempt it could be more than one 
             * auth transaction however it will be just one approved (success) */
            //var authtransaction = auth ?? Transactions.Single(t => t.Type == Models.TransactionType.Auth && t.Success);

            Transaction captureTransaction = null;
            try
            {
                captureTransaction = authtransaction.Processor.GatewayModel(mapper).Capture(authtransaction); //authtransaction.Gateway.Capture(authtransaction); // there must be not exception  and if it is it must go again

                Transactions.Add(captureTransaction);

                if (captureTransaction.Success)
                {
                    Notes.Add(new OrderNote()
                    {
                        NoteDate = DateTime.UtcNow,
                        Note = "Order Captured",
                    });

                }
                else
                {
                    throw new Exception("Transaction Message: " + captureTransaction.Message);
                }
            }
            catch (Exception ex)
            {
                Status = OrderStatus.FailedCapture;

                Notes.Add(new OrderNote()
                {
                    NoteDate = DateTime.UtcNow,
                    Note = "Order Failed Capture. " + ex.Message,
                });

                // revert the balancer allocation and initials
                if (authtransaction.BalancerId.HasValue)
                {
                    var bp = authtransaction.Balancer.BalancerProcessors.Single(p => p.ProcessorId == authtransaction.ProcessorId);

                    bp.Initials--;
                    bp.ProcessedAllocation -= authtransaction.Amount; // Total;
                    bp.Allocation += authtransaction.Amount; //Total;

                    uow.BalancerProcessorRepository.Update(bp);
                }
            }

            return captureTransaction;
        }

        public void RebillSyn(IUnitOfWork uow, IWebSecurityWrapper wsw, IMappingEngine mapper)
        {
            this.Rebill(uow, wsw, mapper).RunSynchronously();

            //var task = this.Rebill(uow, wsw, mapper);
            //task.Start();
            //task.Wait();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uow"></param>
        /// <param name="wsw"></param>
        /// <param name="mapper"></param>
        /// <returns></returns>
        public async Task Rebill(IUnitOfWork uow, IWebSecurityWrapper wsw, IMappingEngine mapper, Processor retryProcessor = null)
        {
            INLogger _logger = new NInLogger();
            // isn't credit card expired or invalid && no credit card invalid ? when a credit card isn't valid??
            try
            {
                _logger.LogInfo("Step1: Process started rebilling for orderId: " + this.OrderId);

                var _ccexpirationDate = new DateTime(this.CreditCardExpirationYear + 2000, (int)this.CreditCardExpirationMonth, DateTime.DaysInMonth(this.CreditCardExpirationYear, (int)this.CreditCardExpirationMonth));
                _ccexpirationDate = _ccexpirationDate.AddDays(1).AddTicks(-1); //make expiry date to the last minute and last day of the month
                _logger.LogInfo("Step2:Credit card expiration year:" + this.CreditCardExpirationYear + 2000 + ", Credit card expiration month:" + (int)this.CreditCardExpirationMonth + "");
                _logger.LogInfo("Step2: Calculate credit card expiration date: " + _ccexpirationDate + ", now checking if expiration date passed");
                if (DateTime.Now < _ccexpirationDate)
                {
                    _logger.LogInfo("Step3: Credit card date is not expired. So it would continue rebill. Now checking product's next date is less than current date time");
                    /*
                     * WE WILL CREATE A NEW CHILD ORDER HERE FOR EACH PRODUCT WHO NEXTDATE MATCH TODAY
                     */
                    var timeUtc = DateTime.UtcNow;
                    TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                    DateTime easternTime = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, easternZone);


                    List<OrderProduct> orderProducts = null;
                    if (OrderProducts != null)
                        orderProducts = this.OrderProducts.Where(o => o.NextDate != null && o.NextDate < easternTime).ToList();
                    else
                        orderProducts = new List<OrderProduct>();
                    var time = DateTimeOffset.UtcNow;
                    decimal taxTotal = 0.0M;
                    foreach (var op in orderProducts)
                    {
                        _logger.LogInfo("Step4: Order products are available, now creating child order");
                        #region  Create child order
                        //Assign NextProduct if op has NextProductId
                        if (op.NextProductId != null)
                        {

                            op.NextProduct = uow.ProductRepository.Find(op.NextProductId);
                            if (op.NextProduct != null)
                                _logger.LogInfo("Step5:Product next product id is not null and next product name is :" + op.NextProduct.Name);
                        }
                        else
                            _logger.LogInfo("Step5:Product next product id is null and no next product is fetched. For OrderId :" +this.OrderId);

                        var childproduct = new OrderProduct()
                        {
                            Quantity = op.Quantity,
                            ProductId = op.NextProductId.Value, // we will bill the next product
                            Product = op.NextProduct,
                            Recurring = op.Recurring, // stop recurring in the next rebill
                            RebillDiscount = op.RebillDiscount,
                            ReAttempts = op.ReAttempts,
                            Price = op.NextProduct.Price,
                            SalvageProfileId = op.SalvageProfileId,
                            SalvageProfile = op.SalvageProfile,
                        };
                        _logger.LogInfo("Step6:Child product created :" + childproduct);

                        if (childproduct.ProductId == op.ProductId) // self recurring 
                        {
                            childproduct.BillType = op.BillType;
                            childproduct.BillValue = op.BillValue;
                            _logger.LogInfo("Step7:Child product id :" + childproduct.ProductId + " and next product id:" + op.ProductId + ". Self recurring is true becuase they both are equal");
                            _logger.LogInfo("Step7:next product bill type:" + op.BillType + ", next product bill value:" + op.BillValue);
                        }
                        else if (childproduct.Product != null && childproduct.Product.IsSubscription)  // get the next subscription settings
                        {
                            //TODO double check this logic - we should be pulling from the configured bill cycle
                            childproduct.BillValue = childproduct.Product.BillValue;
                            childproduct.BillType = childproduct.Product.BillType;
                            _logger.LogInfo("Step7: child product bill type: " + childproduct.BillType + ", next product bill value: " + childproduct.BillValue);
                        }

                        if (childproduct.Product.TaxProfileId.HasValue)
                        {
                            _logger.LogInfo("Step8:Child product tax profile is enabled. Its value is:" + childproduct.Product.TaxProfileId.HasValue);
                            // TODO i have a feeling that the childproduct.Product.TaxProfileId isn't populating because the NextProduct doesn't populate the value.
                            var taxProfile = childproduct.Product.TaxProfile ?? uow.TaxProfileRepository.Find(childproduct.Product.TaxProfileId.Value);

                            if (taxProfile != null && taxProfile.TaxRules != null)
                            {
                                _logger.LogInfo("Step9:TaxProfile and tax rules are not disabled, calculating tax rules");
                                var taxRule = taxProfile.TaxRules.Where(t =>
                                   (t.ApplyToShipping && (t.Country.CountryAbbreviation == this.ShippingCountry
                                   && ((t.City != "" && t.City == this.ShippingCity) || (String.IsNullOrEmpty(t.City)))
                                   && ((t.Province != "" && t.Province == this.ShippingProvince) || (String.IsNullOrEmpty(t.Province)))
                                   && ((t.PostalCode != "" && t.PostalCode == this.ShippingPostalCode) || (String.IsNullOrEmpty(t.PostalCode)))))
                                   || (!t.ApplyToShipping && (t.Country.CountryAbbreviation == this.BillingCountry
                                   && ((t.City != "" && t.City == this.BillingCity) || (String.IsNullOrEmpty(t.City)))
                                   && ((t.Province != "" && t.Province == this.BillingProvince) || (String.IsNullOrEmpty(t.Province)))
                                   && ((t.PostalCode != "" && t.PostalCode == this.BillingPostalCode) || (String.IsNullOrEmpty(t.PostalCode)))))
                                   ).FirstOrDefault();

                                if (taxRule != null)
                                {
                                    _logger.LogInfo("Step10:tax rule calculated successfully.");
                                    childproduct.Tax = taxRule.Tax * childproduct.Price.Value / 100; // do we discount the rebill discount here??
                                    if (taxRule.ApplyToShipping)
                                    {
                                        _logger.LogInfo("step11: Tax rule shipping applied. The value is: " + taxRule.ApplyToShipping);
                                        if (this.ShippingMethod != null)
                                            childproduct.Tax += taxRule.Tax * this.ShippingMethod.RecurringPrice / 100;
                                        else
                                            _logger.LogInfo("step12: Tax is not calculated as ShippingMethod object is null");
                                    }
                                    taxTotal += childproduct.Tax;
                                    _logger.LogInfo("Step12:total tax calculated for child product id :" + childproduct.ProductId + " is :" + taxTotal);
                                }
                                else
                                    _logger.LogInfo("Step11:calculated tax rule is not available.");
                            }
                            else
                                _logger.LogInfo("Step9:Tax profile or tax rules are not not available");

                        }
                        else
                            _logger.LogInfo("Step8:Child product tax profile is not enabled and hence skipped all the tax calculation steps");


                        var chargeIncrement = this.IsPrepaid && op.ReAttempts > 0;
                        // this was somehow removed but was originally in commit ed7a6fed3cdd56b2197698514eaa2396eb51013a
                        var subtotal = chargeIncrement ? op.SalvageProfile.PrepaidIncrement : (childproduct.Price.Value * childproduct.Quantity - op.RebillDiscount);
                        _logger.LogInfo("step13: charge increment is:" + chargeIncrement + " and subtotal calculated:" + subtotal);
                        Order childorder = null;
                        var topParentId = uow.SqlQuery<int?>("select dbo.GetParentID(@OrderId)", new SqlParameter("OrderId", this.OrderId)).SingleOrDefault();
                        try
                        {
                            _logger.LogInfo("step14: Filling child order");
                            childorder = new Order()
                            {
                                AffiliateId = this.AffiliateId,
                                SubId = SubId,

                                ProcessorId = ProcessorId,
                                Processor = Processor,

                                BillingAddress1 = this.BillingAddress1,
                                BillingAddress2 = BillingAddress2,
                                BillingCity = BillingCity,
                                BillingCountry = BillingCountry,
                                BillingFirstName = BillingFirstName,
                                BillingLastName = BillingLastName,
                                BillingPostalCode = BillingPostalCode,
                                BillingProvince = BillingProvince,

                                CreditCardCVV = CreditCardCVV,
                                CreditCardExpirationMonth = CreditCardExpirationMonth,
                                CreditCardExpirationYear = CreditCardExpirationYear,
                                CreditCardNumber = CreditCardNumber,

                                CustomerId = CustomerId,
                                Customer = Customer,
                                Email = Email,
                                Phone = Phone,
                                IsPrepaid = IsPrepaid,

                                // we check for the actual product for the salvage increment and rebill discount but we apply the 
                                // child product price and quantity
                                SubTotal = subtotal,
                                Total = subtotal + (chargeIncrement ? 0 : (childproduct.Product != null && childproduct.Product.IsShippable ? ShippingMethod.RecurringPrice : 0)) + childproduct.Tax, //Charge ShippingMethod.Recurring Price only if Product is Shippable
                                Tax = taxTotal,
                                ShippingAddress1 = ShippingAddress1,
                                ShippingAddress2 = ShippingAddress2,
                                ShippingCity = ShippingCity,
                                ShippingCountry = ShippingCountry,
                                ShippingFirstName = ShippingFirstName,
                                ShippingLastName = ShippingLastName,
                                ShippingPostalCode = ShippingPostalCode,
                                ShippingProvince = ShippingProvince,

                                ShippingMethodId = ShippingMethodId,
                                ShippingMethod = ShippingMethod,

                                TopParentId = topParentId,

                                ParentId = this.OrderId,
                                IPAddress = IPAddress, // preserve the IP address
                                Depth = Depth + 1, // increment the depth

                                // set the created user the parent user
                                CreatedUserId = this.CreatedUserId,

                                Status = OrderStatus.Unpaid,
                                Created = time.DateTime,
                                Notes = new List<OrderNote>
                        {
                            new OrderNote()
                            {
                                Note = "Order Created (Rebill)",
                                NoteDate = time.DateTime,
                            },
                        },

                                OrderProducts = new List<OrderProduct>()
                        {
                            childproduct,
                        },

                            };

                            _logger.LogInfo("step15: Filling child order completed for orderid: "+ this.OrderId);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogException("Failed filling child order", ex);

                        }

                        try
                        {
                            _logger.LogInfo("step16:Process saving child order");
                            uow.OrderRepository.Add(childorder);
                            uow.Save();
                            _logger.LogInfo("step15:Order saved successfully. ChildOrderId:"+ childorder.OrderId + " ParentId:"+ childorder.ParentId);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogInfo("Error save child order: for order id :"+ this.OrderId + " Reason could be:" + ex);
                            _logger.LogException("Error save child order: reason could be:" + ex.Message, ex);

                        }
                        // we need the orderId

                        try
                        {
                            _logger.LogInfo("step16: process started processing order");

                            await childorder.Process(uow, wsw, mapper, retryProcessor);

                            #region Process with Retry Processors, If Declined
                            if (childorder.Status != OrderStatus.Approved)
                            {
                                //Get the processor and see if its having retry Processors
                                Processor processor = null;
                                BalancerProcessor bp = null;
                                int? bId = null;
                                this.FindProcessor(uow, out processor, out bp, out bId);
                                if (processor != null)
                                {
                                    await this.ProcessRetryProcessors(uow, wsw, mapper, processor, childorder);
                                }
                            }
                            #endregion


                            _logger.LogInfo("step17 processing order done");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogException("Processing order failed for Order Id: "+this.OrderId+" and possible reason could be:", ex);
                        }

                        //await childorder.Create(uow, wsw, mapper);  // create & process the child Order

                        #endregion

                        // FAIL
                        if (childorder.Status != OrderStatus.Approved) // clear subscription on the childorder is taken care in the process
                        {
                            _logger.LogInfo("step18: child order status is not APPROVED, the fail scenario");
                            // if fail we always cancel the subscription on the child order
                            childproduct.NextDate = null;
                            childproduct.Recurring = false;
                            childproduct.ReAttempts = 0;

                            _logger.LogInfo("step19:recording a declined attempt for child order");
                            // record a declined attempt for the child order
                            uow.OrderTimeEventRepository.Add(new OrderTimeEvent
                            {
                                OrderId = childorder.OrderId,
                                Time = time,
                                Event = OrderEvent.Declined,
                                Action = childproduct.ReAttempts > 0,
                                ProductId = childproduct.ProductId,
                                AffiliateId = childorder.AffiliateId,
                                SubId = childorder.SubId,
                            });

                            _logger.LogInfo("step20:SUCCESS recording a declined attempt for child order");
                            string transasctionResponse = string.Empty;
                            _logger.LogInfo("21:checking for child order transaction if it fails somewhere");
                            if (childorder != null && childorder.Transactions != null && childorder.Transactions.Any()) // if false then an internal error happened
                                transasctionResponse = childorder.Transactions.LastOrDefault().Response != null ? childorder.Transactions.LastOrDefault().Response.ToLower() : null; // must have just one transaction ?
                            else
                                _logger.LogInfo("step21:Failed. Either child order transaction empty or no transaction found against child ordder");
                            var subscriptionCancelled = false;

                            // Check the negative decline management
                            if (!string.IsNullOrEmpty(transasctionResponse)) // if false then an internal error happened
                            {
                                _logger.LogInfo("step20:Transaction response is not null:" + transasctionResponse);
                                // check the wildcard agains the transaction cancel it if it does
                                var dr = uow.DeclineReasonRepository.Get().FirstOrDefault(d => transasctionResponse.Contains(d.WildCard.ToLower()));

                                if (dr != null)
                                {
                                    _logger.LogInfo("step21:Decline response found and now canceling subscription for orderid: "+ this.OrderId);
                                    // cancel the subscription on the parent
                                    try
                                    {
                                        KontinuityCRMHelper.CancelSubscription(op, uow, wsw, time);
                                        _logger.LogInfo("step22:Cancel subscription success");
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.LogInfo("step21:Cancel subscription failed "+ ex);
                                    }


                                    // add the note to the parent 
                                    this.Notes.Add(new OrderNote
                                    {
                                        NoteDate = DateTime.UtcNow,
                                        Note = string.Format("Rebill failed. Negative decline: {0}", dr.Reason),
                                    });

                                    subscriptionCancelled = true;
                                }

                            }
                            else
                                _logger.LogInfo("step20:Transaction response is null");

                            #region ## Go throught the Decline Salvage Rule ##

                            if (!subscriptionCancelled)
                            {
                                _logger.LogInfo("DECLINE SALVAGE RULE BEING APPLIED");
                                SalvageProfile salvageprofile = null;

                                // a prepaid decline so cancel the subscription
                                if (this.IsPrepaid && op.ReAttempts > 0) //(childproduct.PrepaidIncrement.HasValue)
                                {
                                    KontinuityCRMHelper.CancelSubscription(op, uow, wsw, time); // cancel subscription on the parent

                                    if (this.Notes != null)
                                    {
                                        this.Notes.Add(new OrderNote // add note to the parent
                                        {
                                            NoteDate = DateTime.UtcNow,
                                            Note = "Rebill failed. Prepaid decline",
                                        });
                                    }
                                }
                                else
                                {
                                    // Push the next date according the reattempt rule get the product decline salvage rule
                                    if (transasctionResponse != null)
                                    {
                                        //salvageprofile = childproduct.Product.ProductSalvages // get the decline rule of the childproduct
                                        //    .Select(s => s.SalvageProfile)
                                        //    .FirstOrDefault(s =>
                                        //        //transasctionResponse.Contains(s.DeclineType.WildCard.ToLower())
                                        //        Regex.IsMatch(transasctionResponse, s.DeclineType.WildCard.ToLower())
                                        //    );

                                        if (op.NextSalvageProfileId != null)
                                        {
                                            salvageprofile = uow.SalvageProfileRepository.Get(p => p.Id == op.NextSalvageProfileId).FirstOrDefault();

                                            //Set Next Salvage Profile
                                            #region Set Next Salvage Profile Type
                                            if (salvageprofile != null)
                                                if (salvageprofile.NextSalvageProfile != null)
                                                {
                                                    var nextProfile = uow.SalvageProfileRepository.Get(p => p.Id == salvageprofile.NextSalvageProfile).FirstOrDefault();
                                                    if (nextProfile != null)
                                                    {
                                                        op.NextSalvageProfileId = nextProfile.Id;
                                                        op.NextSalvageProfile = nextProfile;
                                                        if (nextProfile != null)
                                                        {
                                                            if (nextProfile.EnableDiscount == true)
                                                            {
                                                                if (op.NextSalvageProfile.AfterDecline == true)
                                                                    op.RebillDiscount = (op.Price.Value * op.Quantity) / 2; //Offers Discount After Decined
                                                                else
                                                                    if (nextProfile.AfterDecline == false)
                                                                        op.RebillDiscount = 0; //Removes discount after decline
                                                            }
                                                        }
                                                    }
                                                }

                                            #endregion
                                        }
                                        else
                                        {
                                            salvageprofile = uow.SalvageProfileRepository.Get().FirstOrDefault(s => Regex.IsMatch(transasctionResponse, s.DeclineType.WildCard.ToLower()));

                                            //Set Next Salvage Profile
                                            #region Set Next Salvage Profile Type
                                            if (salvageprofile != null)
                                                if (salvageprofile.NextSalvageProfile != null)
                                                {
                                                    var nextProfile = uow.SalvageProfileRepository.Get(p => p.Id == salvageprofile.NextSalvageProfile).FirstOrDefault();
                                                    if (nextProfile != null)
                                                    {
                                                        op.NextSalvageProfileId = nextProfile.Id;
                                                        op.NextSalvageProfile = nextProfile;
                                                        if (nextProfile != null)
                                                        {
                                                            if (nextProfile.EnableDiscount == true)
                                                            {
                                                                if (op.NextSalvageProfile.AfterDecline == true)
                                                                    op.RebillDiscount = (op.Price.Value * op.Quantity) / 2;
                                                                else
                                                                    if (nextProfile.AfterDecline == false)
                                                                        op.RebillDiscount = 0;
                                                            }
                                                        }

                                                    }
                                                }

                                            #endregion
                                        }

                                    }
                                    // if no decline rule cancel subscription or too many reattempts
                                    if (salvageprofile == null || op.ReAttempts >= salvageprofile.CancelAfter)
                                    {
                                        KontinuityCRMHelper.CancelSubscription(op, uow, wsw, time);

                                        this.Notes.Add(new OrderNote
                                        {
                                            NoteDate = DateTime.UtcNow,
                                            Note = string.Format("Rebill failed for product {0}. No decline profile or too many reattempts", op.ProductId),
                                        });
                                    }
                                    else // apply Salvage Profile Rule
                                    {
                                        op.SalvageProfile = salvageprofile;
                                        op.SalvageProfileId = salvageprofile.Id;

                                        if (!this.IsPrepaid && op.ReAttempts >= salvageprofile.LowerPriceAfter) // check this op reattempts
                                        {
                                            //op.RebillDiscount = salvageprofile.LowerAmount ?? 0;

                                            if (salvageprofile.LowerAmount.HasValue)
                                                op.RebillDiscount = salvageprofile.LowerAmount.Value;
                                            else if (salvageprofile.LowerPercentage.HasValue)
                                            {
                                                op.RebillDiscount = this.Total * (salvageprofile.LowerPercentage.Value / 100);
                                            }
                                            else
                                                op.RebillDiscount = 0;

                                        }

                                        // push forward the next date
                                        op.NextDate = KontinuityCRMHelper.GetNextDate(op.NextDate.Value, salvageprofile.BillType, salvageprofile.BillValue); // next date isnt' null because the subscription hasn't been canceled, nextdate should be today 
                                        op.ReAttempts++;
                                    }
                                }

                            }
                            #endregion

                        }
                        else  // SUCCESS
                        {
                            _logger.LogInfo("step18:child order status is SUCCESS");

                            // SUCCESS
                            // updates the order's product upon a success
                            // the child order approves so move on with it
                            // set the nextdate to null to prevent further rebill on this order for this product
                            // though it is redundancy
                            //Remove nextSalvageProfile if Order is removed
                            if (op.NextSalvageProfileId != null)
                            {
                                op.NextSalvageProfile = uow.SalvageProfileRepository.Get(o => o.Id == op.NextSalvageProfileId).FirstOrDefault();
                                if (op.NextSalvageProfile != null)
                                {
                                    if (op.NextSalvageProfile.EnableDiscount == true)
                                    {
                                        if (op.NextSalvageProfile.AfterApprove == false) //Removes Discount
                                            op.RebillDiscount = 0;
                                    }
                                }
                                //Do we have to remove the nextSalvageProfile If order is removed?
                                op.NextSalvageProfileId = null;
                            }
                            // add the child order approved attempt do not record a cancelation in this case
                            _logger.LogInfo("step19:Recording APPROVED attempt. In this case no cancelation would be recorded.");
                            uow.OrderTimeEventRepository.Add(new OrderTimeEvent
                            {
                                OrderId = childorder.OrderId,
                                Time = time,
                                Event = OrderEvent.Approved,
                                Action = childproduct.ReAttempts > 0, // reattempts 
                                ProductId = childproduct.ProductId,
                                AffiliateId = childorder.AffiliateId,
                                SubId = childorder.SubId,
                            });
                            _logger.LogInfo("step20: APPROVED attempt recorded successfully");
                            op.ChildOrder = childorder;
                            op.NextDate = null;

                            // update the next date by the salvage rule in case is a prepaid
                            if (childproduct != null && childproduct.SalvageProfileId != null && this.IsPrepaid)
                            {
                                _logger.LogInfo("step21: updating next date by salvage rule in case it is prepaid");
                                childproduct.NextDate = KontinuityCRMHelper.GetNextDate(op.NextDate.Value, childproduct.SalvageProfile.BillType, childproduct.SalvageProfile.BillValue);
                            }
                            else if (childproduct != null)
                            {
                                _logger.LogInfo("step21: updating next date by salvage failed. either salvage profile id null or it not prepaid");
                                childproduct.ReAttempts = 0; // clear the reattempts in the child order
                                childproduct.SalvageProfileId = null;
                            }
                        }
                        //should not set recurring based on NextDate
                        // op.Recurring = op.NextDate.HasValue;

                        _logger.LogInfo("rebill dependent declined order only once, if no salvage profile found.");
                        // rebill dependent declined order only once, if no salvage profile found.
                        if (ParentId != null && childorder.Status == OrderStatus.Declined)
                        {
                            _logger.LogInfo("parent id is not null and status is declined");
                            var salvageprofile = op.SalvageProfile; //op.Product.ProductSalvages.Select(s => s.SalvageProfile).FirstOrDefault();
                            if (salvageprofile != null)
                            {
                                _logger.LogInfo("salvage profile is not null, setting recurring to true and pushing next date");
                                _logger.LogInfo("Next date valud:" + op.NextDate.Value + ", Bill type:" + salvageprofile.BillType + ", Bill value:" + salvageprofile.BillValue);
                                op.Recurring = true;
                                op.NextDate = KontinuityCRMHelper.GetNextDate(op.NextDate.HasValue ? op.NextDate.Value : DateTime.Now, salvageprofile.BillType, salvageprofile.BillValue);
                                if (op.NextDate == null)
                                    op.NextDate = DateTime.Now.AddDays(2);
                                _logger.LogInfo("Next date in order product is:" + op.NextDate);

                                uow.OrderNoteRepository.Add(new OrderNote
                                {
                                    OrderId = op.OrderId,
                                    NoteDate = DateTime.UtcNow,
                                    Note = "Start Recurring",
                                });
                                uow.Save();

                                #region Set Next Salvage Profile Type
                                if (salvageprofile.NextSalvageProfile != null)
                                {
                                    var nextProfile = uow.SalvageProfileRepository.Get(p => p.Id == salvageprofile.NextSalvageProfile).FirstOrDefault();
                                    if (nextProfile != null)
                                        op.NextSalvageProfileId = nextProfile.Id;
                                }
                                #endregion
                            }
                            else
                            {
                                _logger.LogInfo("salvage profile is null: now making order product recurring to false and next date to previous day");
                                op.Recurring = false;
                                op.NextDate = DateTime.Now.AddDays(5);

                                uow.OrderNoteRepository.Add(new OrderNote
                                {
                                    OrderId = op.OrderId,
                                    NoteDate = DateTime.UtcNow,
                                    Note = "Stop Recurring",
                                });
                                uow.Save();

                            }
                        }

                        try
                        {
                            _logger.LogInfo("step22:Updating order product repo");
                            uow.OrderProductRepository.Update(op);

                            uow.OrderProductRepository.Update(childproduct);
                            _logger.LogInfo("step23: Updating order product repository success");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogException("Failed updating order product ", ex);
                        }

                    } // end foreache op

                    try
                    {
                        _logger.LogInfo("Updating order repository");
                        uow.OrderRepository.Update(this);
                        //uow.Save(wsw.CurrentUserName);
                        // this was fixed originally in commit 19aa4d2588237dc91331dff99953f85728ea12f1 but somehow got wiped
                        // Commenting this out due to Quartz not having a username when running the job.
                        uow.Save();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogException("Failed updating order repo", ex);
                    }


                }
                else // Expired Credit Card 
                {
                    _logger.LogInfo("Step2: Calculate credit card expiration date: " + _ccexpirationDate + ", Its passed away now and its EXPIRED for orderid :" +this.OrderId);

                    this.Notes.Add(new OrderNote
                    {
                        NoteDate = DateTime.UtcNow,
                        Note = "Credit Card expired. Rebill failed.",
                    });

                    uow.Save();
                    // is already cc expired or invalid email / notify customer 
                    //KontinuityCRMHelper.SendMail("kontinuity", this.Customer.Email, "Expired Credit Card", "some info");

                    // fire some event we need to have an event
                    //var em = new EventManager();
                    //em.CancellationEvent +=em_CancellationEvent; subcription 
                    //em.Cancellation(this);

                    #region cancel the order subscription

                    //foreach (var op in this.OrderProducts)
                    //{
                    //    op.NextDate = null;
                    //    repo.EditOrderProduct(op);
                    //}
                    //this.HasSubscription = false;
                    //repo.EditOrder(this);

                    #endregion
                }
            }
            catch (Exception ex)
            {
                _logger.LogInfo("Rebill failed: For orderid: "+ this.OrderId +". Possible reason could be:" + ex.Message + " and:" + ex);
                throw;
            }



        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uow"></param>
        /// <param name="wsw"></param>
        /// <param name="mapper"></param>
        /// <returns></returns>
        public async Task Create(IUnitOfWork uow, IWebSecurityWrapper wsw, IMappingEngine mapper)
        {
            // First chcek if the credit card is in the black list
            // TODO Refactor all of these calls to make the method tighter and cleaner
            if (!string.IsNullOrWhiteSpace(this.CreditCardNumber))
            {
                string cc = this.CreditCardNumber.ToString().Trim().Replace("-", "");

                var blackListCC = uow.BlackListRepository.Get(b => b.CreditCard == cc);

                if (blackListCC.Count() > 0) throw new Exception("Credit Card is in the blacklist");
            }

            //if (!this.IsRebill)
            //{
            if (this.CustomerId == 0)
            {
                // check if there is a customer with this new customer Email
                this.Customer = uow.CustomerRepository.Get(c => c.Email == this.Email).SingleOrDefault();

                if (this.Customer == null)
                {
                    // is a new customer create a new customer and then assign the customer id to that request.Order
                    this.Customer = new Customer
                    {
                        FirstName = this.BillingFirstName, // must not be null for create a Customer
                        IPAddress = this.IPAddress,
                        LastName = this.BillingLastName,
                        Phone = this.Phone,
                        PostalCode = this.BillingPostalCode,
                        Province = this.BillingProvince,
                        Email = this.Email,
                        Country = this.BillingCountry,
                        City = this.BillingCity,
                        Address1 = this.BillingAddress1,
                        Address2 = this.BillingAddress2,
                    };
                    // add the customer to the uow ??
                }

                else
                {
                    // update the customer with this new data ??
                    this.CustomerId = this.Customer.CustomerId;
                    this.Customer.FirstName = this.BillingFirstName; // must not be null for create a this.Customer
                    this.Customer.IPAddress = this.IPAddress;
                    this.Customer.LastName = this.BillingLastName;
                    this.Customer.Phone = this.Phone;
                    this.Customer.PostalCode = this.BillingPostalCode;
                    this.Customer.Province = this.BillingProvince;

                    this.Customer.Country = this.BillingCountry;
                    this.Customer.City = this.BillingCity;
                    this.Customer.Address1 = this.BillingAddress1;
                    this.Customer.Address2 = this.BillingAddress2;

                    // update the customer ??
                    uow.CustomerRepository.Update(this.Customer);
                }
            }
            else
            {
                this.Customer = uow.CustomerRepository.Get(c => c.CustomerId == this.CustomerId).SingleOrDefault();
            }


            string BIN = this.CreditCardNumber.Substring(0, 6);
            var prepaidInfo = uow.PrepaidInfoRepository.GetSet().FirstOrDefault(c => c.BIN == BIN);
            this.IsPrepaid = prepaidInfo != null && prepaidInfo.Prepaid;
            this.IsTest = uow.TestCardNumberRepository.Get(c => c.Number == this.CreditCardNumber).Any();

            // set the shippingmethod before calculate amounts
            if (this.ShippingMethod == null)
                this.ShippingMethod = uow.ShippingMethodRepository.Find(this.ShippingMethodId);

            #region Set Prices and assign the product

            decimal amount = 0;
            decimal taxamount = 0;
            decimal shipValueVariant = 0;
            //decimal taxshipping = 0;
            foreach (var op in OrderProducts)
            {
                // get the product
                if (op.Product == null)
                    op.Product = uow.ProductRepository.Find(op.ProductId);  //repo.FindProduct(op.ProductId);

                // recovery product variant
                string billTypeVariant = "";
                int billValueVariant = 0;
                string taxableVariant = "";

                var pv = uow.ProductVariantRepository.Get(v => v.ProductId == op.ProductId && v.Country.CountryAbbreviation == this.ShippingCountry).FirstOrDefault();

                if (pv != null)
                {
                    op.Cost = pv.Cost != null ? Convert.ToDecimal(pv.Cost) : op.Product.Cost;
                    op.SKU = pv.SKU;
                    op.Currency = pv.Currency;
                    op.Price = pv.Price != null ? Convert.ToDecimal(pv.Price) : op.Product.Price;
                    // TODO this needs to be corrected to iterate through the variants
                    // messy logic
                    var pve = uow.VariantExtraFieldRepository.Get(p => p.ProductVariantId == pv.ProductVariantId).FirstOrDefault();

                    if (pve != null)
                    {
                        // TODO this also needs to append the field values once we iterate through the variants
                        op.FieldValue = pve.FieldValue;
                        op.FieldName = pve.FieldName;
                        if (pve.FieldName == "BillType") billTypeVariant = pve.FieldValue;
                        if (pve.FieldName == "BillValue") billValueVariant = Convert.ToInt32(pve.FieldValue);
                        if (pve.FieldName == "Taxable") taxableVariant = (!string.IsNullOrWhiteSpace(pve.FieldValue) && pve.FieldValue.ToLower() == "true" ? pve.FieldValue : "false");
                        if (pve.FieldName == "ShipValue")
                        {
                            if (shipValueVariant < Convert.ToDecimal(pve.FieldValue))
                            {
                                shipValueVariant = Convert.ToDecimal(pve.FieldValue);
                            }
                        }

                    }

                }

                // assign the price is not set
                decimal price = op.Price ?? op.Product.Price;
                op.Price = price; // set the current product price

                if (op.Product.IsSubscription)
                {
                    op.BillType = (string.IsNullOrWhiteSpace(billTypeVariant) ? op.Product.BillType : (BillType)System.Enum.Parse(typeof(BillType), billTypeVariant));
                    op.BillValue = (billValueVariant > 0 ? billValueVariant : op.Product.BillValue);
                }

                if (op.Product.TaxProfileId.HasValue && (string.IsNullOrWhiteSpace(taxableVariant) || (!string.IsNullOrWhiteSpace(taxableVariant) && Convert.ToBoolean(taxableVariant))))
                {
                    var taxProfile = op.Product.TaxProfile ?? uow.TaxProfileRepository.Find(op.Product.TaxProfileId.Value);

                    if (taxProfile.TaxRules != null)
                    {
                        var taxRule = taxProfile.TaxRules.Where(t =>
                           (t.ApplyToShipping && (t.Country.CountryAbbreviation == this.ShippingCountry
                           && ((!string.IsNullOrWhiteSpace(t.City) && t.City == this.ShippingCity) || (string.IsNullOrWhiteSpace(t.City)))
                           && ((!string.IsNullOrWhiteSpace(t.Province) && t.Province == this.ShippingProvince) || (string.IsNullOrWhiteSpace(t.Province)))
                           && ((!string.IsNullOrWhiteSpace(t.PostalCode) && t.PostalCode == this.ShippingPostalCode) || (string.IsNullOrWhiteSpace(t.PostalCode)))))
                           || (!t.ApplyToShipping && (t.Country.CountryAbbreviation == this.BillingCountry
                           && ((!string.IsNullOrWhiteSpace(t.City) && t.City == this.BillingCity) || (string.IsNullOrWhiteSpace(t.City)))
                           && ((!string.IsNullOrWhiteSpace(t.Province) && t.Province == this.BillingProvince) || (string.IsNullOrWhiteSpace(t.Province)))
                           && ((!string.IsNullOrWhiteSpace(t.PostalCode) && t.PostalCode == this.BillingPostalCode) || (string.IsNullOrWhiteSpace(t.PostalCode)))))
                           ).FirstOrDefault();

                        if (taxRule != null)
                        {
                            op.Tax = taxRule.Tax * price / 100;
                            if (taxRule.ApplyToShipping)
                            {
                                op.Tax += taxRule.Tax * this.ShippingMethod.Price / 100;
                            }
                            taxamount += op.Tax;
                        }
                    }
                }
                //if (op.PrepaidIncrement.HasValue)
                //{
                //    price = op.PrepaidIncrement.Value;
                //}
                //else
                //{
                //    price -= op.RebillDiscount; // rebill discount should be 0 if not a rebill
                //}

                amount += price * op.Quantity;
            }


            this.Tax = taxamount;
            this.SubTotal = amount;
            //Find if any product is shippable, then charge Shipping Method Price
            this.Total = this.SubTotal + (OrderProducts.Any(o => o.Product.IsShippable) ? this.ShippingMethod.Price : 0) + this.Tax; // include shipping price ?  

            #endregion

            //}

            this.Status = OrderStatus.Unpaid;
            this.Created = DateTime.UtcNow;
            this.Notes = new List<OrderNote> 
            { 
                new OrderNote()
                {
                    Note = "Order Created",
                    NoteDate = this.Created,
                },
            };

            //this.ReAttempts = 0;
            //this.HasSubscription = false;
            //this.IsChargeBack = false;
            //this.ChargebackDate = null;

            // calculated fields

            //Sub Total = total of all the products' price
            //shipping = shipping price
            //tax = sum of all tax configured (right now, none of this logic is built, so this will be 0)
            //total = sub total + shipping + tax

            //model.SubTotal = 0; // this will be updated when we add products to this model                
            //if (this.ShippingMethod == null)
            //{
            //     this.ShippingMethod = uow.ShippingMethodRepository.Find(this.ShippingMethodId);
            //}    

            //this.ShippingTotal = this.ShippingMethod.Price;
            //this.Tax = 0; // not implemented yet
            //model.Total = model.SubTotal + model.ShippingTotal + model.Tax; will be calculated on the fly

            // computes the amount for this order
            //CalculateSubTotalAmount();
            //CalculateTotalAmount();


            uow.OrderRepository.Add(this);
            uow.Save(wsw.CurrentUserName);  // we need to orderId

            await this.Process(uow, wsw, mapper);

            uow.OrderRepository.Update(this); // this should update all references (ordernotes, orderproducts, ordertransactions) ?? YES
            uow.Save(wsw.CurrentUserName);

        }

        /// <summary>
        /// Processes with retryProcessors
        /// </summary>
        /// <param name="uow"></param>
        /// <param name="wsw"></param>
        /// <param name="mapper"></param>
        /// <param name="processor"></param>
        /// <returns></returns>
        public async Task ProcessRetryProcessors(IUnitOfWork uow, IWebSecurityWrapper wsw, IMappingEngine mapper, Processor processor)
        {
            var processorRetryIds = uow.ProcessorCascadeRepository.Get(o => o.ProcessorId == processor.Id).Select(o => o.ProcessorRetryId).ToList();
            foreach (var item in processorRetryIds)
            {
                var retryProcessor = uow.ProcessorRepository.Find(item);
                if (retryProcessor != null)
                {
                    await Process(uow, wsw, mapper, retryProcessor);
                }
                if (this.Status == OrderStatus.Approved)
                    break;
                await ProcessRetryProcessors(uow, wsw, mapper, retryProcessor);
            }
        }

        /// <summary>
        /// Processes with retryProcessors, passed an order 
        /// </summary>
        /// <param name="uow"></param>
        /// <param name="wsw"></param>
        /// <param name="mapper"></param>
        /// <param name="processor"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public async Task ProcessRetryProcessors(IUnitOfWork uow, IWebSecurityWrapper wsw, IMappingEngine mapper, Processor processor, Order order)
        {
            var processorRetryIds = uow.ProcessorCascadeRepository.Get(o => o.ProcessorId == processor.Id).Select(o => o.ProcessorRetryId).ToList();
            foreach (var item in processorRetryIds)
            {
                var retryProcessor = uow.ProcessorRepository.Find(item);
                if (retryProcessor != null)
                {
                    await order.Process(uow, wsw, mapper, retryProcessor);
                }
                if (order.Status == OrderStatus.Approved)
                    break;
                await ProcessRetryProcessors(uow, wsw, mapper, retryProcessor, order);
            }
        }
        /// <summary>
        /// Return true only on full shipment
        /// </summary>
        /// <param name="mapper"></param>
        /// <returns>Return true only on full shipment</returns>
        internal async Task<bool> Fulfill(IMappingEngine mapper)
        {
            // group the orderproduct by fullfillment 
            // send only those products on fulfillment schedule 
            // some products might be shipped before other (fulfillment delay)
            var fulfillments = this.OrderProducts
                .Where(op => op.Product.IsShippable && op.Shipped == null && op.FulfillmentDate < DateTime.Now)
                .GroupBy(op => op.Product.FulfillmentProvider);

            foreach (var fop in fulfillments) // the loop continues event if there was any exception
            {
                var fulfillmentProvider = fop.Key;
                var fulfillment = fulfillmentProvider.Fulfillment(mapper);
                try
                {
                    await fulfillment.SendOrder(this, fop);
                }
                catch (Exception ex)
                {
                    foreach (var op in fop)
                    {
                        op.Shipped = false;
                        op.FulfillmentProviderResponse = ex.Message;
                        op.FulfillmentDate = null;
                    }
                }
            }
            if (OrderProducts.Any(op => op.Shipped == false)) // ALL FAIL - all shipping fail for the whole or some product
            {
                this.Status = OrderStatus.ShippedException;

                this.Notes.Add(new OrderNote()
                {
                    NoteDate = DateTime.UtcNow,
                    Note = "Order Shipped Fail",
                });
            }


            else if (OrderProducts.All(op => !op.Product.IsShippable || op.Shipped == true)) // // ALL SUCCESS full shipment
            {
                this.Shipped = true; // mark the order as shipped

                this.Notes.Add(new OrderNote()
                {
                    NoteDate = DateTime.UtcNow,
                    Note = "Order Shipped",
                });

                return true;
            }
            else if (OrderProducts.Any(op => op.Shipped == true)) // PARTIAL SUCCESS partial shipment
            {
                this.Notes.Add(new OrderNote()
                {
                    NoteDate = DateTime.UtcNow,
                    Note = "Order Partial Shipping",
                });
            }

            return false;


        }
    }

}
