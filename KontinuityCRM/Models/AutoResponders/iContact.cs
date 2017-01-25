using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;


namespace KontinuityCRM.Models.AutoResponders
{
    [DisplayName("iContact 2.2")]
    public class iContact : AutoResponder
    {
        [Required]
        [Display(Name = "Api Username")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Api Key")]
        public string ApiKey { get; set; }
        
        [Display(Name = "Client Folder Name")]
        public string ClientFolder { get; set; }
        [Required]
        [Display(Name = "Api Password")]
        public string ApiPassword { get; set; }

        public string CustomField1Name { get; set; }
        public string CustomField1Value { get; set; }

        public string CustomField2Name { get; set; }
        public string CustomField2Value { get; set; }

        public string CustomField3Name { get; set; }
        public string CustomField3Value { get; set; }

        public string CustomField4Name { get; set; }
        public string CustomField4Value { get; set; }

        public string CustomField5Name { get; set; }
        public string CustomField5Value { get; set; }
               
        public override async Task<string> AddContact(Contact contact, string campaign)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseurl);
                client.DefaultRequestHeaders.Accept.Clear();

                var mediatype = new MediaTypeWithQualityHeaderValue("application/json");
                client.DefaultRequestHeaders.Accept.Add(mediatype);

                client.DefaultRequestHeaders.Add("API-Version", "2.2");
                client.DefaultRequestHeaders.Add("API-AppId", this.ApiKey);
                client.DefaultRequestHeaders.Add("API-Username", this.UserName);
                client.DefaultRequestHeaders.Add("API-Password", this.ApiPassword);

                HttpResponseMessage response = await client.GetAsync("icp/a/");
                response.EnsureSuccessStatusCode();

                response.Content.Headers.ContentType = mediatype;
                dynamic responseContent = await response.Content.ReadAsAsync<object>();
                string accountId = responseContent.accounts[0].accountId;
                
                // get client forder 
                string clientFolderId = this.ClientFolder;
                if (string.IsNullOrEmpty(clientFolderId))
                {
                    response = await client.GetAsync("icp/a/" + accountId + "/c/");
                    response.EnsureSuccessStatusCode();

                    response.Content.Headers.ContentType = mediatype; // to ensure json format
                    responseContent = await response.Content.ReadAsAsync<object>();
                    clientFolderId = responseContent.clientfolders[0].clientFolderId;
                }

                var icontact = new Dictionary<string, object>();
                icontact.Add("email", contact.Email);
                icontact.Add("prefix", "");
                icontact.Add("firstName", contact.FirstName);
                icontact.Add("lastName", contact.LastName);
                icontact.Add("suffix", "");
                icontact.Add("street", contact.Address1);
                icontact.Add("city", contact.City);
                icontact.Add("state", contact.Province);
                icontact.Add("postalCode", contact.PostalCode);
                icontact.Add("phone", contact.Phone);
                icontact.Add("business", "ContinuityCRM");
                icontact.Add("status", "normal");

                #region Adding Custom Fields
               
                Func<string, string> setValue = (value) =>
                {
                    if (value == "{prospectid}")
                        return contact.PartialId.ToString(); // it will 0 when we add the customer
                    return ""; // it always has to be a placeholder otherwise it doesn't have sense
                };

                if (this.CustomField1Name != null)
                    icontact.Add(CustomField1Name, setValue(this.CustomField1Value));
                if (this.CustomField2Name != null)
                    icontact.Add(CustomField2Name, setValue(this.CustomField2Value));
                if (this.CustomField3Name != null)
                    icontact.Add(CustomField3Name, setValue(this.CustomField3Value));
                if (this.CustomField4Name != null)
                    icontact.Add(CustomField4Name, setValue(this.CustomField4Value));
                if (this.CustomField5Name != null)
                    icontact.Add(CustomField5Name, setValue(this.CustomField5Value));

                #endregion

                // add the contact 
                response = await client.PostAsJsonAsync("icp/a/" + accountId + "/c/" + clientFolderId + "/contacts/", new[] 
                {
                    icontact
                });

                response.EnsureSuccessStatusCode();
                
                response.Content.Headers.ContentType = mediatype;
                responseContent = await response.Content.ReadAsAsync<object>();
                string contactId = responseContent.contacts[0].contactId;

                // link the contact to the list (campaign)
                response = await client.PostAsJsonAsync("icp/a/" + accountId + "/c/" + clientFolderId + "/subscriptions/", new []
                {
                    new
                    {
                        contactId = contactId,
                        listId = campaign,
                        status = "normal",
                    }
                });

                response.EnsureSuccessStatusCode();
                responseContent = await response.Content.ReadAsStringAsync();
                return responseContent;
            }

           
        }        

        private readonly string baseurl = @"https://app.sandbox.icontact.com/";

        public override async Task<string> RemoveContact(Contact contact, string campaign)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseurl);
                client.DefaultRequestHeaders.Accept.Clear();

                var mediatype = new MediaTypeWithQualityHeaderValue("application/json");
                client.DefaultRequestHeaders.Accept.Add(mediatype);

                client.DefaultRequestHeaders.Add("API-Version", "2.2");
                client.DefaultRequestHeaders.Add("API-AppId", this.ApiKey);
                client.DefaultRequestHeaders.Add("API-Username", this.UserName);
                client.DefaultRequestHeaders.Add("API-Password", this.ApiPassword);

                HttpResponseMessage response = await client.GetAsync("icp/a/");
                response.EnsureSuccessStatusCode();

                response.Content.Headers.ContentType = mediatype;
                dynamic responseContent = await response.Content.ReadAsAsync<object>();
                string accountId = responseContent.accounts[0].accountId;

                // get client forder 
                string clientFolderId = this.ClientFolder;
                if (string.IsNullOrEmpty(clientFolderId))
                {
                    response = await client.GetAsync("icp/a/" + accountId + "/c/");
                    response.EnsureSuccessStatusCode();

                    response.Content.Headers.ContentType = mediatype; // to ensure json format
                    responseContent = await response.Content.ReadAsAsync<object>();
                    clientFolderId = responseContent.clientfolders[0].clientFolderId;
                }

                var jobj = JObject.Parse(contact.ProviderResponse);
                var contactId = (string)jobj["subscriptions"][0]["contactId"];

                var url = string.Format("icp/a/{0}/c/{1}/contacts/{2}", accountId, clientFolderId, contactId);
                response = await client.DeleteAsync(url);

                response.EnsureSuccessStatusCode();

                //response.Content.Headers.ContentType = mediatype;
                //responseContent = await response.Content.ReadAsAsync<object>();
                responseContent = await response.Content.ReadAsStringAsync();
                return responseContent;

            }
        }

        public override async Task<string> MoveContact(Contact contact, string origincampaign, string destinationcampaign)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseurl);
                client.DefaultRequestHeaders.Accept.Clear();

                var mediatype = new MediaTypeWithQualityHeaderValue("application/json");
                client.DefaultRequestHeaders.Accept.Add(mediatype);

                client.DefaultRequestHeaders.Add("API-Version", "2.2");
                client.DefaultRequestHeaders.Add("API-AppId", this.ApiKey);
                client.DefaultRequestHeaders.Add("API-Username", this.UserName);
                client.DefaultRequestHeaders.Add("API-Password", this.ApiPassword);

                HttpResponseMessage response = await client.GetAsync("icp/a/");
                response.EnsureSuccessStatusCode();

                response.Content.Headers.ContentType = mediatype;
                dynamic responseContent = await response.Content.ReadAsAsync<object>();
                string accountId = responseContent.accounts[0].accountId;

                // get client forder 
                string clientFolderId = this.ClientFolder;
                if (string.IsNullOrEmpty(clientFolderId))
                {
                    response = await client.GetAsync("icp/a/" + accountId + "/c/");
                    response.EnsureSuccessStatusCode();

                    response.Content.Headers.ContentType = mediatype; // to ensure json format
                    responseContent = await response.Content.ReadAsAsync<object>();
                    clientFolderId = responseContent.clientfolders[0].clientFolderId;
                }

                var jobj = JObject.Parse(contact.ProviderResponse);
                var subscriptionId = (string)jobj["subscriptions"][0]["subscriptionId"];

                var url = string.Format("icp/a/{0}/c/{1}/subscriptions/{2}", accountId, clientFolderId, subscriptionId);
                response = await client.PutAsJsonAsync(url, new
                {
                    listId = destinationcampaign, 
                    //status = "normal",
                });

                response.EnsureSuccessStatusCode();

                //response.Content.Headers.ContentType = mediatype;
                //responseContent = await response.Content.ReadAsAsync<object>();
                responseContent = await response.Content.ReadAsStringAsync();
                return responseContent;

            }
                
        }
    }
}