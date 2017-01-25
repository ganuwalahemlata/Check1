using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace KontinuityCRM.Models
{
    public class ProcessorCascade
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        public int? ProcessorId { get; set; }
        
        public int? ProcessorRetryId { get; set; }

        [ForeignKey("ProcessorId")]
        public virtual Processor Processor { get; set; }

        [ForeignKey("ProcessorRetryId")]
        public virtual Processor ProcessorsRetry { get; set; }
    }
}