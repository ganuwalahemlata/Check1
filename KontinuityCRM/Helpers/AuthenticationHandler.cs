using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using WebMatrix.WebData;

namespace KontinuityCRM.Helpers
{
    public class AuthenticationHandler : DelegatingHandler
    {
        private const string SCHEME = "Basic";

        

        //private readonly IKontinuityCRMRepo repo = null;
        //private readonly IRepository<User> repository = null;

        //public AuthenticationHandler(/*IRepository<User> repository*/)
        //{
        //    //this.repository = repository;
        //}

        //public AuthenticationHandler(IKontinuityCRMRepo repo)
        //{
        //    this.repo = repo;
        //}

        /// <summary>
        /// SendAsync for Authentication Handlers
        /// </summary>
        /// <param name="request">Http Request</param>
        /// <param name="cancellationToken">Cancellation Request</param>
        /// <returns></returns>
        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                // Perform request processing 
                var headers = request.Headers;
                if (headers.Authorization != null && SCHEME.Equals(headers.Authorization.Scheme))
                {
                    Encoding encoding = Encoding.Default; //.GetEncoding("iso-8859-1");
                    string credentials = encoding.GetString(Convert.FromBase64String(headers.Authorization.Parameter));

                    string[] parts = credentials.Split(':');
                    string userName = parts[0].Trim();
                    string password = parts[1].Trim();

                    var membership = (SimpleMembershipProvider)Membership.Provider;

                    if (membership.ValidateUser(userName, password))
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, userName)
                        };
                        var principal = new ClaimsPrincipal(new[] { new ClaimsIdentity(claims, SCHEME) });
                        
                        Thread.CurrentPrincipal = principal;
                        
                        if (HttpContext.Current != null)
                            HttpContext.Current.User = principal;
                    }
                }                

                var response = await base.SendAsync(request, cancellationToken);
                
                // Perform response processing 
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    response.Headers.WwwAuthenticate.Add(new AuthenticationHeaderValue(SCHEME));
                }


                return response;
            }
            catch (Exception)
            {
                // Perform error processing 
                var response = request.CreateResponse(HttpStatusCode.Unauthorized);
                response.Headers.WwwAuthenticate.Add(new AuthenticationHeaderValue(SCHEME));
                return response;
            }
        }
    }

    public class APIKeyAuthenticationHandle : DelegatingHandler
    {
        private readonly IUnitOfWork uow = null;
        private readonly INLogger logger = null;

        /// <summary>
        /// APIKeyAuthenticationHandle
        /// </summary>
        /// <param name="uow"></param>
        /// <param name="logger"></param>
        public APIKeyAuthenticationHandle(IUnitOfWork uow, INLogger logger)
        {
            this.uow = uow;
            this.logger = logger;
        }
        /// <summary>
        /// SendAsync for APIKeyAuthenticationHandle
        /// </summary>
        /// <param name="request">Http Request Message</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns></returns>
        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            
            string apikey = null; 

            try
            {
                var clientIp = ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
                
                // Perform request processing 

                IEnumerable<string> headerValues;
                bool present = request.Headers.TryGetValues("APIKey", out headerValues); //.GetValues("APIKey");
                if (present)
                {
                    apikey = headerValues.FirstOrDefault();
                }


                if (apikey != null)
                {
                    var guid = Guid.Parse(apikey);
                    var user = uow.UserProfileRepository.Get(u => u.APIKey == guid).SingleOrDefault();

                    if (user != null)
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.UserName)
                        };
                        var principal = new ClaimsPrincipal(new[] { new ClaimsIdentity(claims, "Custom") });

                        Thread.CurrentPrincipal = principal;

                        if (HttpContext.Current != null)
                            HttpContext.Current.User = principal;
                    }
                }

                string requestBody = await request.Content.ReadAsStringAsync();
                logger.LogInfo("Client Ip:" + clientIp + " \n RequestBody:" + requestBody);

                var response = await base.SendAsync(request, cancellationToken);


                if (response.Content != null)
                {
                    // once response body is ready, log it
                    var responseBody = await response.Content.ReadAsStringAsync();
                    logger.LogInfo("Client Ip:" + clientIp + " \n Response Body:" + responseBody);
                }

                // Perform response processing 
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    response.Headers.Add("APIKey", apikey);
                }


                return response;


                //return base.SendAsync(request, cancellationToken).ContinueWith(t => 
                //{
                //    if (t.)
                //    {
                        
                //    }
                
                //});

                // Perform response processing 
                //if (response.StatusCode == HttpStatusCode.Unauthorized)
                //{
                //    response.Headers.Add("APIKey", apikey);
                //}

                

                //return response;
            }
            catch (Exception)
            {
                // Perform error processing 
                var response = request.CreateResponse(HttpStatusCode.Unauthorized);
                response.Headers.Add("APIKey", apikey);
                return response;
            }
        }
    }
}