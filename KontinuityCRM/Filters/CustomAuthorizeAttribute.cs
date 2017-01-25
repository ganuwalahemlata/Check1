using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KontinuityCRM.App_Start;
using KontinuityCRM.Helpers;
using WebMatrix.WebData;
namespace KontinuityCRM.Filters
{
    /// <summary>
    /// Custom Authorization Attribute
    /// </summary>
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// Overriden function OnAuthorization
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            IUnitOfWork uow = IocConfig.Container.GetInstance<IUnitOfWork>();
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                if (uow.UserProfileRepository.Get(a => a.UserName == filterContext.HttpContext.User.Identity.Name).FirstOrDefault().Status == false)
                {
                    WebSecurity.Logout();
                    filterContext.HttpContext.Response.Redirect("/Home/Index");
                }
                else
                    base.OnAuthorization(filterContext);
            }
            else
                base.OnAuthorization(filterContext);
        }
    }
}