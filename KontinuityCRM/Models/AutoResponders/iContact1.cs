using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace KontinuityCRM.Models.AutoResponders
{
    [DisplayName("iContact 1.0")]
    public class iContact1 : AutoResponder
    {
        [Required]
        [Display(Name = "Api Username")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Api Key")]
        public string ApiKey { get; set; }

        [Required]
        [Display(Name = "Api Secret")]
        public string ApiSecret { get; set; }
        
        [Required]
        [Display(Name = "Api Password")]
        public string ApiPassword { get; set; }

        private readonly string baseurl = @"https://app.sandbox.icontact.com/";

        public override System.Threading.Tasks.Task<string> AddContact(Contact contact, string campaign)
        {
            throw new NotImplementedException();

            //using (var client = new HttpClient())
            //{
            //    client.BaseAddress = new Uri(baseurl);
            //    client.DefaultRequestHeaders.Accept.Clear();

            //    var mediatype = new MediaTypeWithQualityHeaderValue("application/json");
            //    client.DefaultRequestHeaders.Accept.Add(mediatype);

            //    client.DefaultRequestHeaders.Add("API-Version", "1.0");
            //    client.DefaultRequestHeaders.Add("API-AppId", this.ApiKey);
            //    client.DefaultRequestHeaders.Add("API-Secret", this.ApiSecret);
            //    client.DefaultRequestHeaders.Add("API-Username", this.UserName);
            //    client.DefaultRequestHeaders.Add("API-Password", this.ApiPassword);

            //    HttpResponseMessage response = await client.GetAsync("icp/a/");
            //    response.EnsureSuccessStatusCode();

            //    response.Content.Headers.ContentType = mediatype;
            //    dynamic responseContent = await response.Content.ReadAsAsync<object>();
            //    string accountId = responseContent.accounts[0].accountId;

            //    var icontact = new Dictionary<string, object>();
            //    icontact.Add("email", contact.Email);
            //    icontact.Add("prefix", "");
            //    icontact.Add("firstName", contact.FirstName);
            //    icontact.Add("lastName", contact.LastName);
            //    icontact.Add("suffix", "");
            //    icontact.Add("street", contact.Address1);
            //    icontact.Add("city", contact.City);
            //    icontact.Add("state", contact.Province);
            //    icontact.Add("postalCode", contact.PostalCode);
            //    icontact.Add("phone", contact.Phone);
            //    icontact.Add("business", "ContinuityCRM");
            //    icontact.Add("status", "normal");

               


                // add the contact 
                //response = await client.PostAsJsonAsync("icp/a/" + accountId + "/c/" + clientFolderId + "/contacts/", new[] 
                //{
                //    icontact
                //});

                //response.EnsureSuccessStatusCode();

                //response.Content.Headers.ContentType = mediatype;
                //responseContent = await response.Content.ReadAsAsync<object>();
                //string contactId = responseContent.contacts[0].contactId;

                //// link the contact to the list (campaign)
                //response = await client.PostAsJsonAsync("icp/a/" + accountId + "/c/" + clientFolderId + "/subscriptions/", new[]
                //{
                //    new
                //    {
                //        contactId = contactId,
                //        listId = campaign,
                //        status = "normal",
                //    }
                //});

                //response.EnsureSuccessStatusCode();
                //responseContent = await response.Content.ReadAsStringAsync();
                //return responseContent;
            //}
        }

        public override System.Threading.Tasks.Task<string> RemoveContact(Contact contact, string campaign)
        {
            throw new NotImplementedException();
        }

        public override System.Threading.Tasks.Task<string> MoveContact(Contact contact, string origincampaign, string destinationcampaign)
        {
            throw new NotImplementedException();
        }
    }
}