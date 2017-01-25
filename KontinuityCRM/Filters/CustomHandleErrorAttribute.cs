using KontinuityCRM.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using DotNetOpenAuth.OpenId.Provider;
using System.Configuration;

namespace KontinuityCRM.Filters
{
    public class CustomHandleErrorAttribute : IExceptionFilter, IResultFilter
    {
        protected readonly IUnitOfWork uow = null;
        protected readonly IWebSecurityWrapper wsw = null;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uow"></param>
        /// <param name="wsw"></param>
        public CustomHandleErrorAttribute(IUnitOfWork uow, IWebSecurityWrapper wsw)
        {
            this.uow = uow;
            this.wsw = wsw;
        }

        //public IUnitOfWork uow { get; set; }
        //public IWebSecurityWrapper wsw { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterContext"></param>
        /// <returns></returns>
        private bool IsAjax(ExceptionContext filterContext)
        {
            return filterContext.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterContext"></param>
        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled || !filterContext.HttpContext.IsCustomErrorEnabled)
                return;

            // if the request is AJAX return JSON else view.
            //if (IsAjax(filterContext))
            //{
            //    //Because its a exception raised after ajax invocation
            //    //Lets return Json
            //    filterContext.Result = new JsonResult()
            //    {
            //        Data = filterContext.Exception.Message,
            //        JsonRequestBehavior = JsonRequestBehavior.AllowGet
            //    };

            //    filterContext.ExceptionHandled = true;
            //    filterContext.HttpContext.Response.Clear();
            //}
            //else
            //{
            //    //Normal Exception
            //    //So let it handle by its default ways.
            //    base.OnException(filterContext);

            //}

            // Write error logging code here if you wish.

            //if want to get different of the request
            var currentController = (string)filterContext.RouteData.Values["controller"];
            var currentActionName = (string)filterContext.RouteData.Values["action"];

            var statusCode = (int)HttpStatusCode.InternalServerError;
            if (filterContext.Exception is HttpException)
            {
                //statusCode = filterContext.Exception.As<HttpException>().GetHttpCode();
                statusCode = (filterContext.Exception as HttpException).GetHttpCode();
            }
            else if (filterContext.Exception is UnauthorizedAccessException)
            {
                //to prevent login prompt in IIS
                // which will appear when returning 401.
                statusCode = (int)HttpStatusCode.Forbidden;
            }

            //_logger.Error("Uncaught exception", filterContext.Exception);

            Exception emailException = null;


            try // send an email only when is an internal error
            {
               // var roleProvider = wsw.RoleProvider;
               // var userNames = roleProvider.GetUsersInRole("Technical");
                var connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

                //var users = uow.UserProfileRepository.Get(u => userNames.Contains(u.UserName));


                var to = "ccrm-dev@continuitycrm.com";
                //MailAddressCollection addressCollection = null;
                //if (users.Any())
                //{
                //    to = users.First().Email;

                //    addressCollection = new MailAddressCollection();
                //    foreach (var user in users.Skip(1))
                //    {
                //        addressCollection.Add(user.Email);
                //    }
                //}
                int line = (new StackTrace(filterContext.Exception, true)).GetFrame(0).GetFileLineNumber();
                var Username = "";
                if (WebMatrix.WebData.WebSecurity.CurrentUserId == -1 || WebMatrix.WebData.WebSecurity.CurrentUserId == 0)
                {
                    Username = "System";
                }
                else
                {
                    Username = WebMatrix.WebData.WebSecurity.CurrentUserName;
                }
                KontinuityCRM.Helpers.KontinuityCRMHelper.SendMail(
                    "noreply@continuitycrm.com", // from
                    to, // to
                    "ContinuityCRM uncaught Exception", // subject
                    string.Format("User:{3},IP:{4},Controller and View:{5} and {6}  ,Message: {0}\r\nConnection String : {7} \r\nLine: {1}\r\nStackTrace: {2} ", filterContext.Exception.Message, line.ToString(), filterContext.Exception.StackTrace, Username, HttpContext.Current.Request.UserHostAddress, currentController, currentActionName,connectionString)// body
                   // addressCollection
                    );
            }
            catch (Exception ex)
            {
                emailException = ex;
            }


            var result = CreateActionResult(filterContext, statusCode, emailException);
            filterContext.Result = result;

            // Prepare the response code.
            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.Clear();
            filterContext.HttpContext.Response.StatusCode = statusCode;
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterContext"></param>
        /// <param name="statusCode"></param>
        /// <param name="emailException"></param>
        /// <returns></returns>
        protected virtual ActionResult CreateActionResult(ExceptionContext filterContext, int statusCode, Exception emailException = null)
        {
            //var ctx = new ControllerContext(filterContext.RequestContext, filterContext.Controller);
            var statusCodeName = ((HttpStatusCode)statusCode).ToString();

            //var viewName = SelectFirstView(ctx,
            //                               "~/Views/Error/{0}.cshtml".FormatWith(statusCodeName),
            //                               "~/Views/Error/General.cshtml",
            //                               statusCodeName,
            //                               "Error");


            var controllerName = (string)filterContext.RouteData.Values["controller"];
            var actionName = (string)filterContext.RouteData.Values["action"];
            var model = new HandleErrorInfo(filterContext.Exception, controllerName, actionName);
            var result = new ViewResult
            {
                ViewName = string.Format("~/Views/Error/{0}.cshtml", statusCodeName),
                ViewData = new ViewDataDictionary<HandleErrorInfo>(model),
            };
            result.ViewBag.StatusCode = statusCode;
            result.ViewBag.EmailException = emailException;

            return result;
        }

        /// <summary>
        /// Handler for HttpNotFound 404 Code
        /// </summary>
        /// <param name="filterContext"></param>
        public void OnResultExecuted(ResultExecutedContext filterContext)
        {
            HttpStatusCodeResult httpStatusCodeResult = filterContext.Result as HttpStatusCodeResult;

            if (httpStatusCodeResult != null && httpStatusCodeResult.StatusCode == 404)
            {
                ViewResult viewResult = new ViewResult();
                viewResult.ViewName = string.Format("~/Views/Error/NotFound.cshtml");
                viewResult.ViewData = filterContext.Controller.ViewData;
                viewResult.TempData = filterContext.Controller.TempData;

                //filterContext.Result = viewResult;
                viewResult.ExecuteResult(filterContext);
                // Prepare the response code.
                filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;

            }
        }

        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
        }

        //protected string SelectFirstView(ControllerContext ctx, params string[] viewNames)
        //{
        //    return viewNames.First(view => ViewExists(ctx, view));
        //}

        //protected bool ViewExists(ControllerContext ctx, string name)
        //{
        //    var result = ViewEngines.Engines.FindView(ctx, name, null);
        //    return result.View != null;
        //}


    }

    public class CustomHandleApiErrorAttribute : System.Web.Http.Filters.ExceptionFilterAttribute
    {
        protected readonly IUnitOfWork uow = null;
        protected readonly IWebSecurityWrapper wsw = null;

        public CustomHandleApiErrorAttribute(IUnitOfWork uow, IWebSecurityWrapper wsw)
        {
            this.uow = uow;
            this.wsw = wsw;
        }

        public override void OnException(System.Web.Http.Filters.HttpActionExecutedContext context)
        {
            var statusCode = (int)HttpStatusCode.InternalServerError;
            if (context.Exception is HttpException)
            {
                //statusCode = filterContext.Exception.As<HttpException>().GetHttpCode();
                statusCode = (context.Exception as HttpException).GetHttpCode();
            }
            else if (context.Exception is UnauthorizedAccessException)
            {
                //to prevent login prompt in IIS
                // which will appear when returning 401.
                statusCode = (int)HttpStatusCode.Forbidden;
            }

            //_logger.Error("Uncaught exception", filterContext.Exception);

            Exception emailException = null;

            try // send an email only when is an internal error
            {
              //  var roleProvider = wsw.RoleProvider;
               // var userNames = roleProvider.GetUsersInRole("Technical");

              // var users = uow.UserProfileRepository.Get(u => userNames.Contains(u.UserName));

                var to = "ccrm-dev@continuitycrm.com";
                //MailAddressCollection addressCollection = null;
                //if (users.Any())
                //{
                //    to = users.First().Email;

                //    addressCollection = new MailAddressCollection();
                //    foreach (var user in users.Skip(1))
                //    {
                //        addressCollection.Add(user.Email);
                //    }
                //}

                var Username = "";
                if (WebMatrix.WebData.WebSecurity.CurrentUserId == -1 || WebMatrix.WebData.WebSecurity.CurrentUserId == 0)
                {
                    Username = "System";
                }
                else
                {
                    Username = WebMatrix.WebData.WebSecurity.CurrentUserName;
                }
                int line = (new StackTrace(context.Exception, true)).GetFrame(0).GetFileLineNumber();
                var controllername = context.ActionContext.ControllerContext.ControllerDescriptor.ControllerName;
                var connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                var Action = context.ActionContext.ActionDescriptor.ActionName;
                KontinuityCRM.Helpers.KontinuityCRMHelper.SendMail(
                    "noreply@continuitycrm.com", // from
                    to, // to
                    "ContinuityCRM uncaught Exception", // subject
                    string.Format("User:{3},IP:{4},Controller and View:{5} and {6}  ,Message: {0} \r\nConnection String : {7} \r\nLine: {1}\r\nStackTrace: {2}  ", context.Exception.Message, line.ToString(), context.Exception.StackTrace, Username, HttpContext.Current.Request.UserHostAddress, controllername, Action, connectionString) // body , addressCollection 
                    );
            }
            catch (Exception ex)
            {
                emailException = ex;
            }

            //find file name and line number from stack trace. This inofrmation could be retrieved 16 levels down untill it finds required data
            var st = new StackTrace(context.Exception, true);
            var frame = st.GetFrame(0);
            var lineNumber = frame.GetFileLineNumber();
            var file = frame.GetFileName();
            int frameNumber = 0;
            bool isFound = true;
            while (isFound)
            {
                frame = st.GetFrame(frameNumber);
                lineNumber = frame.GetFileLineNumber();
                file = frame.GetFileName();
                ++frameNumber;
                if (file != null)
                    isFound = false;
                if (frameNumber > 16)
                    break;
            }

            string fileName = Path.GetFileName(file);
            string ErrorMessage = @"{Error:" + context.Exception.Message + ". For details please see logs" + ",File:" + fileName + ",Line number:" + lineNumber + "}";

            context.Response = context.Request.CreateResponse(HttpStatusCode.InternalServerError, ErrorMessage);


        }
    }
}