using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using AutoMapper;

namespace KontinuityCRM.Models
{
    [TrackChanges]
    public class AutoResponderProvider
    {
       
        /**
         * Common Properties (might be required)
         */
       
        public int Id { get; set; }
        /// <summary>
        /// Indicates Alias
        /// </summary>
        public string Alias { get; set; }
        /// <summary>
        /// Created date
        /// </summary>
        [Display(Name="Date Added")]
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// User Name
        /// </summary>
        [Display(Name = "Api Username")]
        public string UserName { get; set; }
        /// <summary>
        /// Indicates Api key
        /// </summary>
        public string ApiKey { get; set; }

        public string ApiSecret { get; set; }

        public string ApiPassword { get; set; }
        /// <summary>
        /// Api End Point Url for AutoResponderProvider
        /// </summary>
        public string ApiEndPoint { get; set; }
        /// <summary>
        /// Password for AutoResponderProvider
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Custom Field Name
        /// </summary>
        public string CustomField1Name { get; set; }
        /// <summary>
        /// /// Custom Field Value
        /// </summary>
        public string CustomField1Value { get; set; }
        /// <summary>
        /// Custom Field Name
        /// </summary>
        public string CustomField2Name { get; set; }
        /// <summary>
        /// /// Custom Field Value
        /// </summary>
        public string CustomField2Value { get; set; }
        /// <summary>
        /// Custom Field Name
        /// </summary>
        public string CustomField3Name { get; set; }
        /// <summary>
        /// /// Custom Field Value
        /// </summary>
        public string CustomField3Value { get; set; }
        /// <summary>
        /// Custom Field Name
        /// </summary>
        public string CustomField4Name { get; set; }
        /// <summary>
        /// /// Custome Field Value
        /// </summary>
        public string CustomField4Value { get; set; }
        /// <summary>
        /// Custom Field Name
        /// </summary>
        public string CustomField5Name { get; set; }
        /// <summary>
        /// Custo Field Value
        /// </summary>
        public string CustomField5Value { get; set; }
        /// <summary>
        /// Indicates Type of AutoResponderProvider
        /// </summary>
        [Required]
        [MaxLength(128)]
        public string Type { get; set; }

        public AutoResponders.AutoResponder AutoResponder(IMappingEngine mapper)
        { 
            return mapper.Map(this, this.GetType(), System.Type.GetType("KontinuityCRM.Models.AutoResponders." + this.Type)) as AutoResponders.AutoResponder;
        }
    }

    //[DisplayName("iContact 2.2")]
    //public class iContact : AutoResponderProvider
    //{
    //    //public override KontinuityCRM.Models.AutoResponders.AutoResponder AutoResponder()
    //    //{
    //    //    return new KontinuityCRM.Models.AutoResponders.iContact 
    //    //    {
    //    //        Id = Id,
    //    //        Alias = Alias,

    //    //        UserName = UserName,
    //    //        ApiKey = ApiKey,
    //    //        ClientFolder = ApiSecret,
    //    //        ApiPassword = ApiPassword,
    //    //        CreatedDate = CreatedDate,
    //    //        CustomField1Name = CustomField1Name,
    //    //        CustomField2Name = CustomField2Name,
    //    //        CustomField3Name = CustomField3Name,
    //    //        CustomField4Name = CustomField4Name,
    //    //        CustomField5Name = CustomField5Name,
    //    //        CustomField1Value = CustomField1Value,
    //    //        CustomField2Value = CustomField2Value,
    //    //        CustomField3Value = CustomField3Value,
    //    //        CustomField4Value = CustomField4Value,
    //    //        CustomField5Value = CustomField5Value,

    //    //    };
    //    //}
    //}
    //[DisplayName("Get Response")]
    //public class GetResponse : AutoResponderProvider
    //{
    //    //public override KontinuityCRM.Models.AutoResponders.AutoResponder AutoResponder()
    //    //{
            
    //    //        return new KontinuityCRM.Models.AutoResponders.GetResponse
    //    //        {
    //    //            Id = Id,
    //    //            Alias = Alias,
    //    //            CreatedDate = CreatedDate,
    //    //            ApiKey = ApiKey,
                   
    //    //        };


            
    //    //}
    //}

    //[DisplayName("iContact 1.0")]
    //public class iContact1 : AutoResponderProvider { }

    //public class Imnica : AutoResponderProvider
    //{
    //    public override KontinuityCRM.Models.AutoResponders.AutoResponder AutoResponder()
    //    {

    //        return new KontinuityCRM.Models.AutoResponders.Imnica
    //        {
    //            Id = Id,
    //            Alias = Alias,
    //            CreatedDate = CreatedDate,
    //            UserName = UserName,
    //            Password = Password,
    //            ApiEndPoint = ApiEndPoint,
    //        };



    //    }
    //}

    //public class MailChimp : AutoResponderProvider
    //{
    //    public override KontinuityCRM.Models.AutoResponders.AutoResponder AutoResponder()
    //    {

    //        return new KontinuityCRM.Models.AutoResponders.MailChimp
    //        {
    //            Id = Id,
    //            Alias = Alias,
    //            CreatedDate = CreatedDate,
    //            UserName = UserName,
    //            ApiKey = ApiKey,
    //            Password = Password,


    //        };



    //    }
    //}
 

}