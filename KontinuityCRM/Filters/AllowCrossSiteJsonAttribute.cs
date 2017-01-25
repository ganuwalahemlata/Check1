
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;
namespace KontinuityCRM.Filters
{
    /// <summary>
    /// Cross Origin Customizations
    /// </summary>
    public class AllowCrossSiteJsonAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Enabling Access to all Origins
        /// </summary>
        /// <param name="actionExecutedContext"></param>
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Response != null)
            {

                if (!actionExecutedContext.Response.Headers.Contains("Access-Control-Allow-Origin"))
                    actionExecutedContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            }
            else
            {

                actionExecutedContext.Response = new HttpResponseMessage();
                actionExecutedContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                if (actionExecutedContext.Exception != null)
                {
                    if (actionExecutedContext.Exception is InvalidOperationException)
                        actionExecutedContext.Response.StatusCode = HttpStatusCode.Conflict;
                    else
                        actionExecutedContext.Response.StatusCode = HttpStatusCode.InternalServerError;
                }
            }

            base.OnActionExecuted(actionExecutedContext);
        }
        /// <summary>
        /// Enabling Access to all Origins for Async
        /// </summary>
        /// <param name="actionExecutedContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            if (actionExecutedContext.Response != null)
            {
                if (!actionExecutedContext.Response.Headers.Contains("Access-Control-Allow-Origin"))
                    actionExecutedContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            }
            return base.OnActionExecutedAsync(actionExecutedContext, cancellationToken);
        }
    }
}