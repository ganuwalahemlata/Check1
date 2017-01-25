using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KontinuityCRM.Models.AutoResponders
{
    public class Imnica : AutoResponder
    {
        [Required]
        [Display(Name = "Api Endpoint")]
        public string ApiEndPoint { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }

        public Imnica() { }

        public Imnica(AutoResponderProvider responder)
            : base(responder)
        {
            this.ApiEndPoint = responder.ApiEndPoint;
            this.UserName = responder.UserName;
            this.Password = responder.Password;
        }

        public override AutoResponderProvider GetProvider()
        {

            return new KontinuityCRM.Models.Imnica
            {
                Id = Id,
                Alias = Alias,
                UserName = UserName,
                ApiEndPoint = ApiEndPoint,
                Password = Password,
                CreatedDate = CreatedDate,
            };


        }

        

        //public override string AddContact(string email, string name, string campaign)
        //{
        //    throw new NotImplementedException();
        //}

        //public override string RemoveContact(string email, string campaign)
        //{
        //    throw new NotImplementedException();
        //}

        public override System.Threading.Tasks.Task<string> AddContact(Contact contact, string campaign)
        {
            throw new NotImplementedException();
        }

        public override System.Threading.Tasks.Task<string> RemoveContact(Contact contact, string campaign)
        {
            throw new NotImplementedException();
        }
    }
}