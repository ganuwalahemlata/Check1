using KontinuityCRM.Helpers;
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
    /// Class Balancer.
    /// </summary>
    public class Balancer
    {
        /// <summary>
        /// Balancer Id as primary Key
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [Required]
        public string Name { get; set; }

        //public string Description { get; set; }
        //public bool IsEnabled { get; set; }
        //public bool IsPreserved { get; set; }
        //public double AmountAllocation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [allocation balance].
        /// </summary>
        /// <value><c>true</c> if [allocation balance]; otherwise, <c>false</c>.</value>
        [Display(Name = "Allocation Balance")]
        public bool AllocationBalance { get; set; }

        /// <summary>
        /// Gets or sets the balancer processors.
        /// </summary>
        /// <value>The balancer processors.</value>
        public virtual ICollection<BalancerProcessor> BalancerProcessors { get; set; }

        /// <summary>
        /// Selects the processor.
        /// </summary>
        /// <param name="amount">The amount.</param>
        /// <returns>BalancerProcessor.</returns>
        public BalancerProcessor SelectProcessor(decimal amount)
        {
            if (AllocationBalance)
            {
                // select the processor within the balancer with less allocation (utilization)
                return this.BalancerProcessors
                        .Where(p => p.Allocation > 0 && p.Initials < p.InitialLimit)
                        .OrderByDescending(p => p.Allocation)
                        .FirstOrDefault();
            }
            else 
            {
                var totalamount = this.BalancerProcessors.Sum(p => p.ProcessedAllocation) + amount; // toal
                var totalpercent = this.BalancerProcessors.Sum(p => p.AllocationPercent ?? 0); 

                /*
                 *  processedamount + newamount                  percent
                 *  ----------------------------- = percent = -------------
                 *  totalamount + newamount                    totalpercent
                 */

                // return the processor that, after recieve the incomming amount, best approach his percent
                return this.BalancerProcessors.OrderBy(p => Math.Abs(
                    (p.ProcessedAllocation + amount) * totalpercent - (totalamount) * p.AllocationPercent ?? 0
                    )).FirstOrDefault();

            }
        }
    }

    /// <summary>
    /// Class BalancerProcessor.
    /// </summary>
    public class BalancerProcessor
    {
        /// <summary>
        /// Gets or sets the balancer identifier.
        /// </summary>
        /// <value>The balancer identifier.</value>
        [Key, Column(Order = 0)]
        public int BalancerId { get; set; }
        /// <summary>
        /// Gets or sets the balancer.
        /// </summary>
        /// <value>The balancer.</value>
        public virtual Balancer Balancer { get; set; }

        /// <summary>
        /// Gets or sets the processor identifier.
        /// </summary>
        /// <value>The processor identifier.</value>
        [Key, Column(Order = 1)]
        public int ProcessorId { get; set; }
        /// <summary>
        /// Gets or sets the processor.
        /// </summary>
        /// <value>The processor.</value>
        public virtual Processor Processor { get; set; }

        //[RequiredIf(BooleanPropertyName = "AllocationBalancer", VirtualProperty = "Balancer", ExpectedValue = false, ErrorMessage = "The Allocation Percent field is required.")]
        /// <summary>
        /// Gets or sets the allocation percent.
        /// </summary>
        /// <value>The allocation percent.</value>
        public int? AllocationPercent { get; set; }

        /// <summary>
        /// Initial limit is the number of initial orders we allow to process
        /// </summary>
        //[RequiredIf(BooleanPropertyName = "AllocationBalancer", VirtualProperty = "Balancer", ExpectedValue = true, ErrorMessage = "The Initial Limit field is required.")]
        [Display(Name = "Initial Limit")]
        public int? InitialLimit { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is preserved.
        /// </summary>
        /// <value><c>true</c> if this instance is preserved; otherwise, <c>false</c>.</value>
        [Display(Name = "Preserved")]
        public bool IsPreserved { get; set; }

        /// <summary>
        /// Money allowed to process by this balancer. It is diminish every time the order is processed using this balance
        /// </summary>
        //[RequiredIf(BooleanPropertyName = "AllocationBalancer", VirtualProperty = "Balancer", ExpectedValue = true, ErrorMessage = "The Allocation field is required.")]//
        public decimal? Allocation { get; set; }

        [DefaultValue(0)]
        /// <summary>
        /// It is incremented each time an order is processed using this balancerprocessor
        /// </summary>
        public int Initials { get; set; }

        [DefaultValue(0.0)]
        /// <summary>
        /// This is the amount that have been processed by this bp
        /// </summary>
        public decimal ProcessedAllocation { get; set; }

   
    }
}