using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.ModelBinding;

namespace KontinuityCRM.Filters
{
    /// <summary>
    /// Validation for Model Attribute
    /// </summary>
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Overriden functionality on ActionExecuting
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (actionContext.ModelState.IsValid == false)
            {
                //actionContext.Response = actionContext.Request.CreateErrorResponse(
                //    HttpStatusCode.BadRequest, actionContext.ModelState);

                //fixing issue of duplicate error messages
                var values = actionContext.ModelState.Values.ToList();//SelectMany(p => p.Errors.GroupBy(g => g.ErrorMessage).Select(t => t.First())).ToList();
                var keys = actionContext.ModelState.Keys.ToList();
                ModelStateDictionary md = new ModelStateDictionary();
                if (keys != null && keys.Count > 0)
                {
                    for (int i = 0; i < keys.Count; i++)
                    {
                        if (values[i] != null && values[i].Errors != null && values[i].Errors.Count > 0)
                        {
                            md.AddModelError(keys[i], values[i].Errors[0].ErrorMessage);
                        }
                    }
                    actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, md);
                }
                else
                    actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, actionContext.ModelState);
            }
        }
    }
}