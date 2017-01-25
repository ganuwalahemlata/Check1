using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KontinuityCRM.Models.AutoResponders
{
    public class MailChimp : AutoResponder
    {
        [Required]
        [Display(Name = "Api Key")]
        public string ApiKey { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }

        public MailChimp() { }

        public MailChimp(AutoResponderProvider responder)
            : base(responder)
        {
            this.ApiKey = responder.ApiKey;
            this.UserName = responder.UserName;
            this.Password = responder.Password;
        }

        public override AutoResponderProvider GetProvider()
        {

            return new KontinuityCRM.Models.MailChimp
            {
                Id = Id,
                Alias = Alias,
                ApiKey = ApiKey,
                UserName = UserName,
                Password = Password,
                CreatedDate = CreatedDate,
            };

        }


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