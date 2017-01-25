using AutoMapper;
using KontinuityCRM.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KontinuityCRM.Models.Gateways
{
    public interface IGateway
    {
        /// <summary>
        /// Sale action
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="processor">Processor</param>
        /// <returns></returns>
        Transaction Sale(Order order, Processor processor);

        TransactionViaPrepaidCardQueue SalePrepaidCard(PrepaidCard prepaidCard,Processor processor, decimal amouunt);
        /// <summary>
        /// Authorize action
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="processor">Processor</param>
        /// <returns></returns>
        Transaction Authorize(Order order, Processor processor);
        /// <summary>
        /// Void action
        /// </summary>
        /// <param name="transaction">Transaction</param>
        /// <returns></returns>
        Transaction Void(Transaction transaction);
        /// <summary>
        /// Refund Action
        /// </summary>
        /// <param name="uow">UOW</param>
        /// <param name="mapper">Mapper</param>
        /// <param name="notificationType">Notification tyoe</param>
        /// <param name="orderId">OrderId</param>
        /// <param name="transaction">Transaction</param>
        /// <param name="amount">amount</param>
        /// <returns></returns>
        Transaction Refund(IUnitOfWork uow, IMappingEngine mapper, NotificationType notificationType, int orderId, Transaction transaction, decimal amount);
        /// <summary>
        /// Capture Action
        /// </summary>
        /// <param name="transaction">Transaction</param>
        /// <returns></returns>
        Transaction Capture(Transaction transaction);
    }

    public abstract class GatewayModel : IGateway
    {
        /// <summary>
        /// Processor Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Indicates Name
        /// </summary>
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }
        /// <summary>
        /// Created date
        /// </summary>
        public DateTime? CreatedDate { get; set; }
        /// <summary>
        /// Gateway Id
        /// </summary>
        public int GatewayId { get; set; }

        public virtual Gateway Gateway { get; set; }
        /// <summary>
        /// Retry Processors Ids
        /// </summary>
        [NotMapped]
       // [Required(ErrorMessage = "Select atleast one Retry Processor")]
        public List<string> RetryProcessorIds { get; set; }

        #region Right fields
        /// <summary>
        /// Field for description
        /// </summary>
        public string Descriptor { get; set; }
        /// <summary>
        /// Customer Service Nunmber Field
        /// </summary>
        [Display(Name = "Customer Service Number")]
        public string CustomerServiceNumber { get; set; }
        /// <summary>
        /// Global Monthly Capital Field
        /// </summary>
        [Display(Name = "Global Monthly Cap")]
        public string GlobalMonthlyCap { get; set; }
        /// <summary>
        /// Transaction Fee Field
        /// </summary>
        [Display(Name = "Transaction Fee")]
        public string TransactionFee { get; set; }
        /// <summary>
        /// Charge Back fee Field
        /// </summary>
        [Display(Name = "Chargeback Fee")]
        public string ChargebackFee { get; set; }
        /// <summary>
        /// Processing Percent fee field
        /// </summary>
        [Display(Name = "Processing Percent")]
        public string ProcessingPercent { get; set; }
        /// <summary>
        /// Reverse Percent Field
        /// </summary>
        [Display(Name = "Reverse Percent")]
        public string ReversePercent { get; set; }
        /// <summary>
        /// Processor Specific Text Field
        /// </summary>
        [Display(Name = "Processor Specific Text ")]
        public string ProcessorSpecificText { get; set; }
        /// <summary>
        /// Processor Type Field
        /// </summary>
        public ProcessorType Type { get; set; }
        /// <summary>
        /// Capture on Shipment Field
        /// </summary>
        [Display(Name = "Capture on Shipment")]
        public bool CaptureOnShipment { get; set; } // warm the customer product might be shipped without capture
        /// <summary>
        /// Shipment on Capture Field
        /// </summary>
        [Display(Name = "Shipment on Capture")]
        public bool ShipmentOnCapture { get; set; }
        /// <summary>
        /// Delay in Capture Field
        /// </summary>
        [Display(Name = "Capture Delay Hours")]
        public int? CaptureDelayHours { get; set; } // warm the customer product might be shipped without capture

        #endregion

        public abstract Transaction Sale(Order order, Processor processor);

        public abstract TransactionViaPrepaidCardQueue SalePrepaidCard(PrepaidCard prepaidCard, Processor processor, decimal amouunt);

        public abstract Transaction Authorize(Order order, Processor processor);

        public abstract Transaction Void(Transaction transaction);

        public virtual Transaction Refund(IUnitOfWork uow, IMappingEngine mapper, NotificationType notificationType, int orderId, Transaction transaction, decimal amount)
        {
            EmailHelper.SendOrderEmail(uow, mapper, notificationType, orderId);
            return null;
        }
        /// <summary>
        /// Capture
        /// </summary>
        /// <param name="transaction">Transaction</param>
        /// <returns></returns>
        public abstract Transaction Capture(Transaction transaction);
        /// <summary>
        /// Map to GatewayModel
        /// </summary>
        /// <param name="mapper">mapper</param>
        /// <returns></returns>
        internal Processor Processor(AutoMapper.IMappingEngine mapper)
        {
            return mapper.Map(this, this.GetType(), typeof(Processor)) as Processor;
        }
    }
}
