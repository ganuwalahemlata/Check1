using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KontinuityCRM.Models.Fulfillments
{
    [DisplayName("SixWorks Fulfillment")]
    public class SixWorksFulfillment : Fulfillment
    {
       [Display(Name = "API Key")]
        public string ApiKey { get; set; }

        [Display(Name = "SixWork Sub Domains")]
        public string SixWorkSubDomains { get; set; } // Six Work

        [Display(Name = "Receive Tracking #")]
        public bool RecieveTrackingId { get; set; }

        public override bool GetTracking()
        {
            throw new NotImplementedException();
        }

        public override string SendOrder(Order order)
        {
            throw new NotImplementedException();
        }

        public override bool ProcessReturn()
        {
            throw new NotImplementedException();
        }

        public override FulfillmentProvider Provider
        {
            get
            {
                return new KontinuityCRM.Models.SixWorksFulfillment
                {
                    Alias = Alias,
                    Delay = Delay,
                    Id = Id,
                    CreatedDate = CreatedDate,

                    ApiKey = ApiKey,
                    SixWorkSubDomains = SixWorkSubDomains,
                    RecieveTrackingId = RecieveTrackingId,

                };
            }
        }
    }
}