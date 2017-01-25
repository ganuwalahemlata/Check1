using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KontinuityCRM.Models
{
    public class Provider
    {
        public int ProviderId { get; set; }
        [Required(ErrorMessage = "The Name of Provider is Required")]
        public string Name { get; set; }
        public string Alias { get; set; }
    }
}