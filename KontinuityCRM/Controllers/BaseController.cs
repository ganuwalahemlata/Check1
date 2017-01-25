using AutoMapper;
using KontinuityCRM.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Async;
using KontinuityCRM.Models;

namespace KontinuityCRM.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        protected readonly IUnitOfWork uow = null;
        protected readonly IWebSecurityWrapper wsw = null;

        public BaseController(IUnitOfWork uow, IWebSecurityWrapper wsw)
        {
            this.uow = uow;
            this.wsw = wsw;
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);

            var request = filterContext.HttpContext.Request;

            if (request.HttpMethod == "GET" && !(filterContext.Result is RedirectToRouteResult) && !(filterContext.Result is RedirectToRouteResult))
            {
                //Generate a log entry
                var klog = new KontinuityCRM.Models.KLog()
                {
                    //Your Audit Identifier     
                    Id = Guid.NewGuid(),
                    //Our Username (if available)
                    UserName = (request.IsAuthenticated) ? filterContext.HttpContext.User.Identity.Name : "Anonymous",
                    //The IP Address of the Request
                    IPAddress = request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? request.UserHostAddress,
                    //The URL that was accessed
                    Url = request.RawUrl,
                    //Creates our Timestamp
                    Timestamp = DateTime.UtcNow,

                };

                //Stores the Audit in the Database
                //Repository.AddLog(klog);
                uow.KLogRepository.Add(klog);
                uow.Save();
            }


        }

        protected override void Dispose(bool disposing)
        {
            uow.Dispose();
            base.Dispose(disposing);
        }

        protected virtual TimeZoneInfo CurrentUsersTimezone()
        {
            return TimeZoneUtility.GetTimeZone(uow.TimeZoneRepository.Find(CurrentUsersProfile().TimeZoneId).Name);
        }

        protected virtual UserProfile CurrentUsersProfile()
        {
            return uow.UserProfileRepository.Find(wsw.CurrentUserId);
        }

    }

    //[Authorize]
    //public class SuperController : Controller
    //{ 
    //    protected readonly IUnitOfWork uow = null;
    //    protected readonly IWebSecurityWrapper wsw = null;

    //    public SuperController(IUnitOfWork uow, IWebSecurityWrapper wsw, string enumType)
    //    {
    //        //this.ActionInvoker = new CustomActionInvoker(enumType, wsw, uow /*repo*/);
    //        this.uow = uow;
    //        this.wsw = wsw;
    //    }
    //}

    //public class BaseController : SuperController
    //{
    //    public BaseController(IUnitOfWork uow, IWebSecurityWrapper wsw)
    //        : base(uow, wsw, "ViewPermissiond")
    //    {

    //    }        
    //}

    //public class Base1Controller : SuperController
    //{
    //    public Base1Controller(IUnitOfWork uow, IWebSecurityWrapper wsw)
    //        : base(uow, wsw, "ViewPermission")
    //    {

    //    }
    //}

    //public class Base2Controller : SuperController
    //{
    //    public Base2Controller()
    //        : base(new UnitOfWork(), new WebSecurityWrapper(), "ViewPermission2")
    //    {

    //    }
    //}





    //public class CustomActionInvoker : AsyncControllerActionInvoker // ControllerActionInvoker
    //{
    //    private string enumType;
    //    private readonly IUnitOfWork uow;
    //    private readonly IWebSecurityWrapper wsw;

    //    public CustomActionInvoker() : base() { }

    //    public CustomActionInvoker(string enumtype, IWebSecurityWrapper wsw, IUnitOfWork uow)
    //        : base() 
    //    {
    //        this.enumType = enumtype;
    //        this.uow = uow;
    //        this.wsw = wsw;
    //    }

    //    protected override FilterInfo GetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
    //    {
    //        var filters = base.GetFilters(controllerContext, actionDescriptor);

    //        string _action = actionDescriptor.ActionName.ToLower();


    //        string _controllername = actionDescriptor.ControllerDescriptor.ControllerName;


    //        try
    //        {
    //            DynamicAuthorizeUserAttribute _afilter = new DynamicAuthorizeUserAttribute()
    //            {
    //                wsw = this.wsw,
    //            };

    //            //var _logfilter = new LogFilterAttribute()
    //            //{
    //            //    uow = uow,                    
    //            //};

    //            filters.AuthorizationFilters.Add(_afilter);
    //            //filters.ActionFilters.Add(_logfilter);
    //        }
    //        catch//(Exception ex)
    //        //catch (System.ArgumentException)
    //        {
    //            // if we are here is because there is no associate permitions for the action
    //            // either because is not defined by forget or intentionally
    //            // do nothing...


    //            throw new Exception(_action);
    //        }


    //        return filters;
    //    }
    //}



}
