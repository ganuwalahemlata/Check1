using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace KontinuityCRM.Models
{
    public class TestCardNumber
    {
        /// <summary>
        /// ID of the TestCardNumber as primary key
        /// </summary>
        public int Id { get; set; }
        
        //[Key]
        /// <summary>
        /// Indicates Number of the card just for Test
        /// </summary>
        [Display(Name="Card Number")]
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Number { get; set; }
    }
}