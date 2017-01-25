using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using AutoMapper;
using KontinuityCRM.Models.Fulfillments;

namespace KontinuityCRM.Models
{    
    [TrackChanges]
    public class FulfillmentProvider 
    {
        /// <summary>
        /// Id as primary key
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Indicates delay in hours for fulfillmentProvider
        /// </summary>
        [Display(Name="Delay Hours")]
        public int? Delay { get; set; }
        /// <summary>
        /// Alias for fulfillmentProvider
        /// </summary>
        public string Alias { get; set; }
        /// <summary>
        /// Create Date for fulfillmentProvider
        /// </summary>
        [Display(Name = "Date Added")]
        public DateTime CreatedDate { get; set; }

        /*
         * End of the required fields
         * */
        /// <summary>
        /// UserName
        /// </summary>
        public string UserName { get; set; } // Shipware
        /// <summary>
        /// Password for fulfillmentProvider
        /// </summary>
        public string Password { get; set; } // Shipware
        /// <summary>
        /// Indicates whether to receive tracking id for fulfillmentProvider
        /// </summary>
        public bool? RecieveTrackingId { get; set; } // Shipware
        /// <summary>
        /// Indicates database for fulfillmentProvider
        /// </summary>
        public string DataBase { get; set; }  // Nova Care
        /// <summary>
        /// Indicates client Name
        /// </summary>
        public string ClientName { get; set; } // BTB Mail Flight
        /// <summary>
        /// Api key for fulfillmentProvider
        /// </summary>
        public string ApiKey { get; set; } // Six Work
        /// <summary>
        /// Indicates sixwork subdomains for fulfillmentProvider
        /// </summary>
        [Display(Name="SixWork SubDomains")]
        public string SixWorkSubDomains { get; set; } // Six Work

        // ClientNumber => ClientName // PMA
        /// <summary>
        /// Indicates prefix for fulfillmentProvder
        /// </summary>
        public string Prefix { get; set; } // PMA
        /// <summary>
        /// Indicates whether Receive Returns for fulfillmentProvider
        /// </summary>
        public bool? ReceiveReturns { get; set; } // PMA
        /// <summary>
        /// Indicates Type of fulfillmentProvider
        /// </summary>
        [Required]
        [MaxLength(128)]
        public string Type { get; set; }

        // JM MerchandId => UserName

        // GLF already in 
        /// <summary>
        /// Map to fulfillmentProvider
        /// </summary>
        /// <param name="mapper">mapper</param>
        /// <returns></returns>
        public Fulfillment Fulfillment(IMappingEngine mapper) 
        {
            return mapper.Map(this, this.GetType(), System.Type.GetType("KontinuityCRM.Models.Fulfillments." + this.Type)) as Fulfillment;
        }
        
    }

    //[TrackChanges]
    //public class Shipwire : FulfillmentProvider
    //{
    //    //public override Fulfillments.Fulfillment Provider()
    //    //{
            
    //    //        return new Fulfillments.Shipwire 
    //    //        { 
    //    //            Alias = Alias,
    //    //            Delay = Delay,
    //    //            Id = Id,
    //    //            CreatedDate = CreatedDate,
                    
    //    //            UserName = UserName,
    //    //            Password = Password,
    //    //            RecieveTrackingId = RecieveTrackingId ?? false,

    //    //        };
            
    //    //}
    //}

    //public class Orderwave : FulfillmentProvider
    //{

        //public override Fulfillments.Fulfillment Provider()
        //{

        //    return new Fulfillments.Orderwave
        //    {
        //        Alias = Alias,
        //        Delay = Delay,
        //        Id = Id,
        //        CreatedDate = CreatedDate,

        //        UserName = UserName,
        //        Password = Password,
        //        RecieveTrackingId = RecieveTrackingId ?? false,

        //    };

        //}
    //}

    //[DisplayName("Nova Care")]
    //public class NovaCare : FulfillmentProvider
    //{
    //    public override Fulfillments.Fulfillment Provider()
    //    {
            
    //            return new Fulfillments.NovaCare
    //            {
    //                Alias = Alias,
    //                Delay = Delay,
    //                Id = Id,
    //                CreatedDate = CreatedDate,

    //                UserName = UserName,
    //                Password = Password,
    //                RecieveTrackingId = RecieveTrackingId ?? false,
    //                Database = DataBase,

    //            };
            
    //    }
    //}

    //[DisplayName("SixWorks Fulfillment")]
    //public class SixWorksFulfillment : FulfillmentProvider
    //{
    //    public override Fulfillments.Fulfillment Provider()
    //    {
    //        return new Fulfillments.SixWorksFulfillment
    //        {
    //            Alias = Alias,
    //            Delay = Delay,
    //            Id = Id,
    //            CreatedDate = CreatedDate,

    //            ApiKey = ApiKey,
    //            SixWorkSubDomains = SixWorkSubDomains,
    //            RecieveTrackingId = RecieveTrackingId ?? false,
    //        };
            
    //    }
    //}

 


}