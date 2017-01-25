using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KontinuityCRM.Models.Fulfillments
{
    [DisplayName("Nova Care")]
    public class NovaCare : Fulfillment
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string Database { get; set; }

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
                return new KontinuityCRM.Models.NovaCare
                {
                    Alias = Alias,
                    Delay = Delay,
                    Id = Id,
                    CreatedDate = CreatedDate,

                    UserName = UserName,
                    Password = Password,
                    RecieveTrackingId = RecieveTrackingId,
                    DataBase = Database,
                };
            }
        }
    }
}