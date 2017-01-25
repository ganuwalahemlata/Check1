using KontinuityCRM.Models.Gateways;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using AutoMapper;
using System.ComponentModel.DataAnnotations.Schema;

namespace KontinuityCRM.Models
{
    [TrackChanges]
    public class Processor
    {
        /// <summary>
        /// Indicates id as primary key
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Indicates gatewayid foreign kry to gateway
        /// </summary>
        public int GatewayId { get; set; }
        public virtual Gateway Gateway { get; set; }

        //public string Descriptor { get; set; }
        //public string PhoneNumber { get; set; }
        //public int MonthlyLimit { get; set; }
        /// <summary>
        /// Parameters array
        /// </summary>
        public byte[] Parameters { get; set; }
        /// <summary>
        /// Processor Name
        /// </summary>
        [Required]
        public string Name { get; set; }            // this field is not in the spec


        public int Status { get; set; }
        
        //public string Description { get; set; }     // this field is not in the spec
        /// <summary>
        /// Processor type
        /// </summary>
        public ProcessorType Type { get; set; }
        /// <summary>
        /// Processor Created date
        /// </summary>
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// Retry Processor Ids
        /// </summary>
		[NotMapped]
        [Required(ErrorMessage = "Select at least one Retry Processor")]
        public List<string> RetryProcessorIds { get; set; }

         [InverseProperty("Processor")]
        public virtual ICollection<ProcessorCascade> ProcessorCascade { get; set; }

         [InverseProperty("ProcessorsRetry")]
        public virtual ICollection<ProcessorCascade> ProcessorCascadeRetry { get; set; }
        
        #region Right fields
        /// <summary>
        /// Descriptor Field
        /// </summary>
        public string Descriptor { get; set; }
        /// <summary>
        /// Cusomer Service Number Field
        /// </summary>
        public string CustomerServiceNumber { get; set; }
        /// <summary>
        /// Global Monthly capital field
        /// </summary>
        public string GlobalMonthlyCap { get; set; }
        /// <summary>
        /// Transaction Fee Field
        /// </summary>
        public string TransactionFee { get; set; }
        /// <summary>
        /// Charge Back Fee Field
        /// </summary>
        public string ChargebackFee { get; set; }
        /// <summary>
        /// Processing Percent Fee
        /// </summary>
        public string ProcessingPercent { get; set; }
        /// <summary>
        /// Reverse Percent Fee
        /// </summary>
        public string ReversePercent { get; set; }
        /// <summary>
        /// Processor Specific Text Field
        /// </summary>
        public string ProcessorSpecificText { get; set; }


        /// <summary>
        /// Cature on shipment overrides capture delay
        /// </summary>
        public bool CaptureOnShipment { get; set; }

        public bool ShipmentOnCapture { get; set; } // warm the customer product might be shipped without capture
        /// <summary>
        /// Indicates delay in Capture in hours
        /// </summary>
        public int? CaptureDelayHours { get; set; } // warm the customer product might be shipped without capture

        #endregion

        #region NMI
        /// <summary>
        /// Indicates Currency
        /// </summary>
        public string Currency { get; set; }
        /// <summary>
        /// Indicates Post Processor Id
        /// </summary>
        public bool PostProcessorId { get; set; }
        /// <summary>
        /// Post Product Description
        /// </summary>
        public bool PostProductDescription { get; set; }
        /// <summary>
        /// Post Description enabled or not
        /// </summary>
        public bool PostDescriptor { get; set; }

        //public bool CaptureOnShipment { get; set; }
        /// <summary>
        /// Use PreAuthorization Filter or not
        /// </summary>
        public bool UsePreAuthorizationFilter { get; set; }

        //public bool UseDeclineSalvage { get; set; }

        //public int ProcessorId { get; set; }

        #endregion

        #region Argus
        /// <summary>
        /// siteId for argus
        /// </summary>
        public string SiteId { get; set; }
        /// <summary>
        /// Merchant Account ID
        /// </summary>
        public string MerchantAccountId { get; set; }
        /// <summary>
        /// Dynamic Product Id
        /// </summary>
        public string DynamicProductId { get; set; }

        #endregion

        public virtual ICollection<BalancerProcessor> BalancerProcessors { get; set; }

        /// <summary>
        /// Map Processor to GatewayModel
        /// </summary>
        /// <param name="mapper"></param>
        /// <returns></returns>
        public GatewayModel GatewayModel(IMappingEngine mapper)
        {
            return mapper.Map(this, this.GetType(), System.Type.GetType("KontinuityCRM.Models.Gateways." + this.Gateway.Type)) as GatewayModel;
        }
        
    }

    /// <summary>
    /// Enum for Processor Type
    /// </summary>
    public enum ProcessorType
    {
        Sale = 1,
        Authorize,
    }
    /// <summary>
    /// Eunm for Processor Type Secure Trading
    /// </summary>
    public enum ProcessorTypeSecureTrading
    {
        Sale = 1      
    }
}
