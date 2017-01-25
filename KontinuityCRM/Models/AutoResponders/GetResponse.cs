using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace KontinuityCRM.Models.AutoResponders
{
    [DisplayName("Get Response")]
    public class GetResponse : AutoResponder
    {
        [Required]
        [Display(Name = "Api Key")]
        public string ApiKey { get; set; }

        public readonly string api_url = "http://api2.getresponse.com";
        /// <summary>
        /// AddContact
        /// </summary>
        /// <param name="contact"></param>
        /// <param name="campaign"></param>
        /// <returns></returns>
        public override async Task<string> AddContact(Contact contact, string campaign)
        {
            var jss = new JavaScriptSerializer();

            // get CAMPAIGN_ID of 'sample_marketing' campaign

            // new request object
            Hashtable _request = new Hashtable();

            _request["jsonrpc"] = "2.0";
            _request["id"] = 1;

            // set method name
            _request["method"] = "get_campaigns";

            // set conditions
            Hashtable operator_obj = new Hashtable();

            operator_obj["EQUALS"] = campaign;

            Hashtable name_obj = new Hashtable();
            name_obj["name"] = operator_obj;

            // set params request object
            object[] params_array = { ApiKey, name_obj };

            _request["params"] = params_array;

            // send headers and content in one request
            // (disable 100 Continue behavior)
            System.Net.ServicePointManager.Expect100Continue = false;


            // initialize client
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(api_url);
            request.Method = "POST";

            byte[] request_bytes = Encoding.UTF8.GetBytes(jss.Serialize(_request));

            String response_string = null;

            try
            {
                // call method 'get_messages' and get result
                Stream request_stream = request.GetRequestStream();
                request_stream.Write(request_bytes, 0, request_bytes.Length);
                request_stream.Close();

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream response_stream = response.GetResponseStream();

                StreamReader reader = new StreamReader(response_stream);
                response_string = await reader.ReadToEndAsync();
                reader.Close();

                response_stream.Close();
                response.Close();
            }
            catch (Exception e)
            {
                //check for communication and response errors
                //implement handling if needed
                //Console.WriteLine(e.Message);
                //Environment.Exit(0);
                throw e;
                //return response_string;
            }

            // decode response to Json object
            Dictionary<string, object> jsonContent = jss.DeserializeObject(response_string) as Dictionary<string, object>;

            // get result
            Dictionary<string, object> result = jsonContent["result"] as Dictionary<string, object>;

            string campaign_id = null;

            // get campaign id
            foreach (object key in result.Keys)
            {
                campaign_id = key.ToString();
            }


            //// check if campaign isn't created
            //if (campaign_id == null)
            //{
            //    // create the campaign

            //    // get confirmation 
            //}

            // at this point the campaign will be already in campaign_id so add the contact to it

            // new request object
            _request = new Hashtable();

            _request["jsonrpc"] = "2.0";
            _request["id"] = 2;

            // set method name
            _request["method"] = "add_contact";

            Hashtable contact_params = new Hashtable();
            contact_params["campaign"] = campaign_id;
            contact_params["name"] = string.Format("{0} {1}", contact.FirstName, contact.LastName);
            contact_params["email"] = contact.Email;

            //Hashtable custom = new Hashtable();
            //custom["name"] = "last_purchased_product";
            //custom["content"] = "netbook";

            //// contact customs array
            //object[] customs_array = { custom };

            //// add customs to contact params
            //contact_params["customs"] = customs_array;


            // set params request object
            object[] add_contact_params_array = { ApiKey, contact_params };

            _request["params"] = add_contact_params_array;

            // send headers and content in one request
            // (disable 100 Continue behavior)
            System.Net.ServicePointManager.Expect100Continue = false;

            // initialize client
            request = (HttpWebRequest)WebRequest.Create(api_url);
            request.Method = "POST";

            request_bytes = Encoding.UTF8.GetBytes(jss.Serialize(_request));

            response_string = null;

            try
            {
                // call method 'add_contact' and get result
                Stream request_stream = request.GetRequestStream();
                request_stream.Write(request_bytes, 0, request_bytes.Length);
                request_stream.Close();

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream response_stream = response.GetResponseStream();

                StreamReader reader = new StreamReader(response_stream);
                response_string = reader.ReadToEnd();
                reader.Close();

                response_stream.Close();
                response.Close();
            }
            catch (Exception e)
            {
                //check for communication and response errors
                //implement handling if needed
                //Console.WriteLine(e.Message);
                //Environment.Exit(0);
                throw e;
            }

            return response_string;
        }
        /// <summary>
        /// RemoveContact
        /// </summary>
        /// <param name="contact"></param>
        /// <param name="campaign"></param>
        /// <returns></returns>
        public override async Task<string> RemoveContact(Contact contact, string campaign)
        {
            var jss = new JavaScriptSerializer();

            // get CAMPAIGN_ID of 'sample_marketing' campaign

            // new request object
            Hashtable _request = new Hashtable();

            _request["jsonrpc"] = "2.0";
            _request["id"] = 1;

            // set method name
            _request["method"] = "get_campaigns";

            // set conditions
            Hashtable operator_obj = new Hashtable();

            operator_obj["EQUALS"] = campaign;

            Hashtable name_obj = new Hashtable();
            name_obj["name"] = operator_obj;

            // set params request object
            object[] params_array = { ApiKey, name_obj };

            _request["params"] = params_array;

            // send headers and content in one request
            // (disable 100 Continue behavior)
            System.Net.ServicePointManager.Expect100Continue = false;


            // initialize client
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(api_url);
            request.Method = "POST";

            byte[] request_bytes = Encoding.UTF8.GetBytes(jss.Serialize(_request));

            String response_string = null;

            try
            {
                // call method 'get_messages' and get result
                Stream request_stream = request.GetRequestStream();
                request_stream.Write(request_bytes, 0, request_bytes.Length);
                request_stream.Close();

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream response_stream = response.GetResponseStream();

                StreamReader reader = new StreamReader(response_stream);
                response_string = await reader.ReadToEndAsync();
                reader.Close();

                response_stream.Close();
                response.Close();
            }
            catch (Exception e)
            {
                //check for communication and response errors
                //implement handling if needed
                //Console.WriteLine(e.Message);
                //Environment.Exit(0);
                throw e;
                //return response_string;
            }

            // decode response to Json object
            Dictionary<string, object> jsonContent = jss.DeserializeObject(response_string) as Dictionary<string, object>;

            // get result
            Dictionary<string, object> result = jsonContent["result"] as Dictionary<string, object>;

            string campaign_id = null;

            // get campaign id
            foreach (object key in result.Keys)
            {
                campaign_id = key.ToString();
            }

            /*-*********************************************************/

            // new request object
            _request = new Hashtable();

            _request["jsonrpc"] = "2.0";
            _request["id"] = 2;

            // set method name
            _request["method"] = "get_contacts";

            Hashtable get_contacts = new Hashtable();
            // search contact in this campaign
            get_contacts["campaigns"] = new object [] { campaign_id };

             //search for the email 
            Hashtable _email = new Hashtable();
            _email["EQUALS"] = contact.Email;

            // add the email params to the request
            get_contacts["email"] = _email;

           
            
            // set params request object
            object[] get_contacts_params_array = { ApiKey, get_contacts }; //

            _request["params"] = get_contacts_params_array;

            // send headers and content in one request
            // (disable 100 Continue behavior)
            System.Net.ServicePointManager.Expect100Continue = false;

            // initialize client
            request = (HttpWebRequest)WebRequest.Create(api_url);
            request.Method = "POST";

            var debug = jss.Serialize(_request);
            request_bytes = Encoding.UTF8.GetBytes(jss.Serialize(_request));

            response_string = null;

            try
            {
                // call method 'add_contact' and get result
                Stream request_stream = request.GetRequestStream();
                request_stream.Write(request_bytes, 0, request_bytes.Length);
                request_stream.Close();

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream response_stream = response.GetResponseStream();

                StreamReader reader = new StreamReader(response_stream);
                response_string = reader.ReadToEnd();
                reader.Close();

                response_stream.Close();
                response.Close();
            }
            catch (Exception e)
            {
                //check for communication and response errors
                //implement handling if needed
                //Console.WriteLine(e.Message);
                //Environment.Exit(0);
                throw e;
            }

            // retrieve the contactid

            // decode response to Json object
            jsonContent = jss.DeserializeObject(response_string) as Dictionary<string, object>;

            // get result
            result = jsonContent["result"] as Dictionary<string, object>;

            string contact_id = null;

            // get campaign id
            foreach (object key in result.Keys)
            {
                contact_id = key.ToString();
            }
            
            /*****************************************************************************************************************************/

            // new request object
            _request = new Hashtable();

            _request["jsonrpc"] = "2.0";
            _request["id"] = 3;

            // set method name
            _request["method"] = "delete_contact";

            Hashtable delete_contact = new Hashtable();
            // search contact in this campaign
            delete_contact["contact"] = contact_id;

            // set params request object
            object[] delete_contact_params_array = { ApiKey, delete_contact };

            _request["params"] = delete_contact_params_array;

            // send headers and content in one request
            // (disable 100 Continue behavior)
            System.Net.ServicePointManager.Expect100Continue = false;

            // initialize client
            request = (HttpWebRequest)WebRequest.Create(api_url);
            request.Method = "POST";

            request_bytes = Encoding.UTF8.GetBytes(jss.Serialize(_request));

            response_string = null;

            try
            {
                // call method 'add_contact' and get result
                Stream request_stream = request.GetRequestStream();
                request_stream.Write(request_bytes, 0, request_bytes.Length);
                request_stream.Close();

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream response_stream = response.GetResponseStream();

                StreamReader reader = new StreamReader(response_stream);
                response_string = reader.ReadToEnd();
                reader.Close();

                response_stream.Close();
                response.Close();
            }
            catch (Exception e)
            {
                //check for communication and response errors
                //implement handling if needed
                //Console.WriteLine(e.Message);
                //Environment.Exit(0);
                throw e;
            }



            return response_string;
        }
        /// <summary>
        /// MoveContact
        /// </summary>
        /// <param name="contact"></param>
        /// <param name="origincampaign"></param>
        /// <param name="destinationcampaign"></param>
        /// <returns></returns>
        public override async Task<string> MoveContact(Contact contact, string origincampaign, string destinationcampaign)
        {
            await RemoveContact(contact, origincampaign);
            return await AddContact(contact, destinationcampaign);
        }
    }
}