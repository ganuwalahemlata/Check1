using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace KontinuityCRM.Helpers
{
    public class CredentialsHandler : DelegatingHandler
    {
        /// <summary>
        /// SendAsync for CredentialsHandler
        /// </summary>
        /// <param name="request">http Request</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns></returns>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var headers = request.Headers;
            if (headers.Authorization == null)
            {
                string creds = String.Format("{0}:{1}", "njhones", "kont123");
                byte[] bytes = Encoding.Default.GetBytes(creds);
                var header = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(bytes));
                headers.Authorization = header;
            }
            return await base.SendAsync(request, cancellationToken);
        }
    }

    public class APIKeyCredentialsHandler : DelegatingHandler
    {
        /// <summary>
        /// SendAsync for APIKeyCredentialHandler
        /// </summary>
        /// <param name="request">http Request</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns></returns>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var headers = request.Headers;
            if (!headers.Any(k => k.Key == "APIKey"))
            {
                headers.Add("APIKey", "F39271FC-30EF-4F28-B226-67AF461B16CA");
            }
            return await base.SendAsync(request, cancellationToken);
        }
    }
}