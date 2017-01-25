using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using AutoMapper;

namespace KontinuityCRM.Models.Fulfillments
{
    public interface IFulfillment
    {
        //bool GetTracking();
        
        /// <summary>
        /// Set the shipped and the fulfilmentResponse for the orderProduct. 
        /// Clear the FulfillmentDate (Prevents further fulfillments)
        /// </summary>
        /// <param name="order"></param>
        /// <param name="orderProducts"></param>
        /// <returns></returns>
        Task<bool> SendOrder(Order order, IEnumerable<OrderProduct> orderProducts);
        
        //bool ProcessReturn(); 
    }

    public abstract class Fulfillment : IFulfillment
    {
        /// <summary>
        /// FulFillmentId as primary key
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Indicates delay in hours
        /// </summary>
        [Display(Name = "Delay Hours")]
        public int? Delay { get; set; }
        /// <summary>
        /// Indicates the Alias for Fulfillment
        /// </summary>
        [Required]
        public string Alias { get; set; }
        /// <summary>
        /// Indicates the create date of fulfillment
        /// </summary>
        [Display(Name = "Date Added")]
        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// Indicates the type of fulfillment
        /// </summary>
        public string Type { get { return this.GetType().Name; } }

        //public abstract bool GetTracking();
        //public abstract string SendOrder(Order order);
        //public abstract bool ProcessReturn(); 
        public KontinuityCRM.Models.FulfillmentProvider Provider(IMappingEngine mapper)
        { 
            return mapper.Map(this, this.GetType(), typeof(FulfillmentProvider)) as FulfillmentProvider;
        }

        //public abstract bool GetTracking();
        /// <summary>
        /// SendOrder an abstract function
        /// </summary>
        /// <param name="order"></param>
        /// <param name="orderProducts"></param>
        /// <returns></returns>
        public abstract Task<bool> SendOrder(Order order, IEnumerable<OrderProduct> orderProducts);

        //public abstract bool ProcessReturn();

    }
}