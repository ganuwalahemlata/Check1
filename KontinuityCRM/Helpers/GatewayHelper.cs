using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace KontinuityCRM.Helpers
{
    public class GatewayHelper
    {
        /// <summary>
        /// Converts a NameValueCollection to its string representation
        /// </summary>
        /// <param name="postdata">NameValueCollection</param>
        /// <returns>System.String</returns>
        public static string GetPostData(NameValueCollection postdata)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < postdata.Count; i++)
            {
                sb.AppendFormat("{0}={1}&", postdata.Keys[i], postdata[i]);
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }
        /// <summary>
        /// Uses WebRequest to post to URL and retrieves response
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="postdata">The postdata.</param>
        /// <returns>System.String.</returns>
        public static string GetResponse(string url, NameValueCollection postdata = null)
        {
            // Create a request using a URL that can receive a post. 
            var request = WebRequest.Create(new Uri(url)) as HttpWebRequest;
            // Set the Method property of the request to POST.
            request.Method = "POST";
            // Create POST data and convert it to a byte array.
            string postData = postdata == null ? String.Empty : GetPostData(postdata);
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            // Set the ContentType property of the WebRequest.
            request.ContentType = "application/x-www-form-urlencoded";

            // Set the ContentLength property of the WebRequest.
            request.ContentLength = byteArray.Length;

            // Get the request stream.
            Stream dataStream = request.GetRequestStream();

            // Write the data to the request stream.
            dataStream.Write(byteArray, 0, byteArray.Length);
            // Close the Stream object.
            dataStream.Close();

            // Get the response.
            WebResponse response = request.GetResponse();

            // Display the status.
            //Console.WriteLine(((HttpWebResponse)response).StatusDescription);

            // Get the stream containing content returned by the server.
            dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();
            // Display the content.
            //Console.WriteLine(responseFromServer);

            reader.Close();
            dataStream.Close();
            response.Close();

            //NameValueCollection query = HttpUtility.ParseQueryString(responseFromServer);

            //var result = new LimeLightResponse() { allfields = query };

            // Clean up the streams.
            

            return responseFromServer;
        }
        /// <summary>
        ///Uses Web Request to post to url with headers dictionary and retrieves response
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="xml">key value pair</param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static string GetResponse(string url, string xml, Dictionary<string, string> headers)
        {
            // Create a request using a URL that can receive a post. 
            var request = WebRequest.Create(new Uri(url)) as HttpWebRequest;

            // Set the Method property of the request to POST.
            request.Method = "POST";

            // Create POST data and convert it to a byte array.
            byte[] byteArray = Encoding.UTF8.GetBytes(xml);

            // Set the ContentType property of the WebRequest.
            request.ContentType = "text/xml;charset=utf-8";

            // Set the ContentLength property of the WebRequest.
            request.ContentLength = byteArray.Length;

            request.Accept = "text/xml";

            foreach (var kv in headers)
            {
                request.Headers.Add(kv.Key, kv.Value);
            }

            // Get the request stream.
            Stream dataStream = request.GetRequestStream();

            // Write the data to the request stream.
            dataStream.Write(byteArray, 0, byteArray.Length);
            // Close the Stream object.
            dataStream.Close();

            // Get the response.
            WebResponse response = request.GetResponse();

            // Display the status.
            //Console.WriteLine(((HttpWebResponse)response).StatusDescription);

            // Get the stream containing content returned by the server.
            dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();
            // Display the content.
            //Console.WriteLine(responseFromServer);

            reader.Close();
            dataStream.Close();
            response.Close();

            //NameValueCollection query = HttpUtility.ParseQueryString(responseFromServer);

            //var result = new LimeLightResponse() { allfields = query };

            // Clean up the streams.


            return responseFromServer;
        }
        /// <summary>
        /// Get Async response 
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="postdata">post data</param>
        /// <param name="headers">headers key value pairs</param>
        /// <returns></returns>
        public async static Task<string> GetResponseAsync(string url, NameValueCollection postdata = null, NameValueCollection headers = null)
        {
            // Create a request using a URL that can receive a post. 
            var request = WebRequest.Create(new Uri(url)) as HttpWebRequest;
            // Set the Method property of the request to POST.
            request.Method = "POST";
            // Create POST data and convert it to a byte array.
            string postData = postdata == null ? String.Empty : GetPostData(postdata);
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            // Set the ContentType property of the WebRequest.
            request.ContentType = "application/x-www-form-urlencoded";

            // Set the ContentLength property of the WebRequest.
            request.ContentLength = byteArray.Length;

            // Get the request stream.
            Stream dataStream = await request.GetRequestStreamAsync();

            // Write the data to the request stream.
            dataStream.Write(byteArray, 0, byteArray.Length);
            // Close the Stream object.
            dataStream.Close();

            // Get the response.
            WebResponse response = await request.GetResponseAsync();

            // Display the status.
            //Console.WriteLine(((HttpWebResponse)response).StatusDescription);

            // Get the stream containing content returned by the server.
            dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseFromServer = await reader.ReadToEndAsync();
            // Display the content.
            //Console.WriteLine(responseFromServer);

            reader.Close();
            dataStream.Close();
            response.Close();

            //NameValueCollection query = HttpUtility.ParseQueryString(responseFromServer);

            //var result = new LimeLightResponse() { allfields = query };

            // Clean up the streams.


            return responseFromServer;
        }

        private static HttpRequestMessage BuildRequest(HttpMethod method, string url, string postData)
        {
            if (method != HttpMethod.Post)
                return new HttpRequestMessage(method, new Uri(url));

            var request = new HttpRequestMessage(method, new Uri(url))
            {
                Content = new StringContent(postData, Encoding.UTF8, "application/x-www-form-urlencoded")
            };

            return request;
        }

        public static dynamic GetResponse(string url, HttpMethod method, NameValueCollection content, Dictionary<string, string> headers)
        {

            var request = BuildRequest(method, url, content.Count > 0 ? GetPostData(content) : string.Empty);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            //Add headers to the request
            foreach (var kv in headers)
            {
                request.Headers.Add(kv.Key, kv.Value);
            }

            var Client = new HttpClient();
            return Client.SendAsync(request).Result;
        }
    }
}