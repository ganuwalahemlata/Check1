using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using KontinuityCRM.Models;
using System.Reflection;
using WebMatrix.WebData;

namespace KontinuityCRM.Helpers
{
    public class RequiredIfTrueAttribute : ValidationAttribute, IClientValidatable
    {
        public string BooleanPropertyName { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (GetValue<bool>(validationContext.ObjectInstance, BooleanPropertyName))
            {
                return new RequiredAttribute().IsValid(value) ?
                    ValidationResult.Success :
                    new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
            return ValidationResult.Success;
        }

        private static T GetValue<T>(object objectInstance, string propertyName)
        {
            if (objectInstance == null) throw new ArgumentNullException("objectInstance");
            if (string.IsNullOrWhiteSpace(propertyName)) throw new ArgumentNullException("propertyName");

            var propertyInfo = objectInstance.GetType().GetProperty(propertyName);
            return (T)propertyInfo.GetValue(objectInstance);
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var modelClientValidationRule = new ModelClientValidationRule
            {
                ValidationType = "requirediftrue",
                ErrorMessage = FormatErrorMessage(metadata.DisplayName)
            };
            modelClientValidationRule.ValidationParameters.Add("boolprop", BooleanPropertyName);
            yield return modelClientValidationRule;
        }
    }

    public sealed class RequiredIfAttribute : ValidationAttribute, IClientValidatable
    {
        public string VirtualProperty { get; set; }

        public string BooleanPropertyName { get; set; }

        public bool ExpectedValue { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (GetValue<bool>(validationContext.ObjectInstance, BooleanPropertyName, VirtualProperty) == ExpectedValue)
            {
                return new RequiredAttribute().IsValid(value) ?
                    ValidationResult.Success :
                    new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
            return ValidationResult.Success;
        }

        private static T GetValue<T>(object objectInstance, string propertyName, string virtualproperty = null)
        {
            if (objectInstance == null) throw new ArgumentNullException("objectInstance");
            if (string.IsNullOrWhiteSpace(propertyName)) throw new ArgumentNullException("propertyName");

            if (string.IsNullOrEmpty(virtualproperty))
            {
                var propertyInfo = objectInstance.GetType().GetProperty(propertyName);
                return (T)propertyInfo.GetValue(objectInstance);
            }
            
            // try get the value of the virtual property and then get the property
            var parentpropertyInfo = objectInstance.GetType().GetProperty(virtualproperty);
            var virtualobj = parentpropertyInfo.GetValue(objectInstance);

            return (T)virtualproperty.GetType().GetProperty(propertyName).GetValue(virtualobj);
            
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var modelClientValidationRule = new ModelClientValidationRule
            {
                ValidationType = "requiredif",
                ErrorMessage = FormatErrorMessage(metadata.DisplayName)
            };
            modelClientValidationRule.ValidationParameters.Add("boolprop", BooleanPropertyName);
            modelClientValidationRule.ValidationParameters.Add("expected", ExpectedValue);
            yield return modelClientValidationRule;
        }
    }

    /// <summary>
    /// Attribute for automatically securing actions
    /// </summary>
    public class DynamicAuthorizeUserAttribute : AuthorizeAttribute
    {
        public IWebSecurityWrapper wsw { get; set; }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var isAuthorized = base.AuthorizeCore(httpContext);
            if (!isAuthorized)
            {
                return false;
            }

            if (httpContext.User.Identity.IsAuthenticated)
            {
                var rd = httpContext.Request.RequestContext.RouteData;
                string currentAction = rd.GetRequiredString("action").ToLower();
                string currentController = rd.GetRequiredString("controller");
                string permission = currentController.ToUpper() + "_" + currentAction;
                var roleProvider = (SimpleRoleProvider)System.Web.Security.Roles.Provider;
                if (roleProvider.GetRolesForUser(wsw.CurrentUserName).Contains(SecurityRole.Admin.ToString()))
                    return true;
                return (roleProvider.GetRolesForUser(wsw.CurrentUserName).Contains(permission));
            }

            return true;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new RedirectResult("~/Account/Login?ReturnUrl=%2fadmin");
                return;
            }

            if (filterContext.Result is HttpUnauthorizedResult)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary(
                        new
                        {
                            controller = "error",
                            action = "index"
                        })
                    );
            }
        }
    }

    public class AuthorizeUserAttribute : AuthorizeAttribute
    {
        public string EnumType { get; set; }
        public string EnumValue { get; set; }
        //public IKontinuityCRMRepo Repository { get; set; }
        public IUnitOfWork uow { get; set; }
        public IWebSecurityWrapper wsw { get; set; }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var isAuthorized = base.AuthorizeCore(httpContext);
            if (!isAuthorized)
            {
                return false;
            }

            if (httpContext.User.Identity.IsAuthenticated)
            {
                //var repo = new EFKontinuityCRMRepo();
                var user = uow.UserProfileRepository.Find(wsw.CurrentUserId); //Repository..FindUserProfile(WebMatrix.WebData.WebSecurity.CurrentUserId);
                //var result = !((p & RequiredPermission) == RequiredPermission);
                //return result;

                var roleProvider = (SimpleRoleProvider)System.Web.Security.Roles.Provider;
                if (roleProvider.GetRolesForUser(wsw.CurrentUserName).Contains(SecurityRole.Admin.ToString()))
                    return true;
                long actualvalue = 0;
                if (user!=null)
                switch (EnumType)
                {
                    case "ViewPermissiond":
                        actualvalue = user.Permissions ?? 0;
                        break;
                    case "ViewPermission":
                        actualvalue = user.Permissions1 ?? 0;
                        break;
                    case "ViewPermission2":
                        actualvalue = user.Permissions2 ?? 0;
                        break;
                }

                var assembly = System.Reflection.Assembly.Load("EnumAssembly");
                Type enumtype = assembly.GetType(EnumType);
                System.Reflection.FieldInfo enumItem = enumtype.GetField(EnumValue);
                long requiredvalue = (long)enumItem.GetRawConstantValue();

                // if marked means that the permissions had been set so that is why the negation
                return /*!*/((actualvalue & requiredvalue) == requiredvalue);
            }

            return true;
        }

        // this works!!
        //protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        //{
        //    filterContext.Result = new RedirectToRouteResult(
        //            new RouteValueDictionary(
        //                new
        //                {
        //                    controller = "Home",
        //                    action = "AuthorizationError"
        //                })
        //            );

        //    //filterContext.Result = new RedirectResult(urlHelper.Action("Index", "Error"));
        //}

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new RedirectResult("~/Account/Login?ReturnUrl=%2fadmin");
                return;
            }

            if (filterContext.Result is HttpUnauthorizedResult)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary(
                        new
                        {
                            controller = "error",
                            action = "index"
                        })
                    );
            }
        }
    }

    public class LogFilterAttribute : ActionFilterAttribute
    {
        private readonly IUnitOfWork uow;

        public LogFilterAttribute(IUnitOfWork uow)
        {
            this.uow = uow;
        }
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);

            var request = filterContext.HttpContext.Request; 

            if (request.HttpMethod == "GET" && !(filterContext.Result is RedirectToRouteResult) && !(filterContext.Result is RedirectToRouteResult))
            {
                //Generate a log entry
                var klog = new KLog()
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

        //public IUnitOfWork uow { get; set; }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class MultipleButtonAttribute : ActionNameSelectorAttribute
    {
        public string Name { get; set; }
        public string Argument { get; set; }

        public override bool IsValidName(ControllerContext controllerContext, string actionName, MethodInfo methodInfo)
        {
            var isValidName = false;
            var keyValue = string.Format("{0}:{1}", Name, Argument);
            var value = controllerContext.Controller.ValueProvider.GetValue(keyValue);

            if (value != null)
            {
                controllerContext.Controller.ControllerContext.RouteData.Values[Name] = Argument;
                isValidName = true;
            }
               
            return isValidName;
        }
    }

    //public class HttpParamActionAttribute : ActionNameSelectorAttribute
    //{
    //    public override bool IsValidName(ControllerContext controllerContext, string actionName, MethodInfo methodInfo)
    //    {
    //        if (actionName.Equals(methodInfo.Name, StringComparison.InvariantCultureIgnoreCase))
    //            return true;

    //        if (!actionName.Equals("Action", StringComparison.InvariantCultureIgnoreCase))
    //            return false;

    //        var request = controllerContext.RequestContext.HttpContext.Request;
    //        return request[methodInfo.Name] != null;
    //    }
    //}

    /// <summary>
    /// Validate that the product exits
    /// </summary>
    public class CheckProductExitsAttribute : ValidationAttribute
    {
        [StructureMap.Attributes.SetterProperty]
        public IUnitOfWork uow { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {            
            if(uow.ProductRepository.Find(value) == null)
                return new ValidationResult(String.Format(this.ErrorMessage, value));

            return null;
        }           
    }

    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class RemoteWithServerSideAttribute : RemoteAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string controllerName = this.RouteData["controller"].ToString();
            string actionName = this.RouteData["action"].ToString();
            string[] additionalFields = this.AdditionalFields.Split(',');

            List<object> propValues = new List<object>();
            propValues.Add(value);
            foreach (string additionalField in additionalFields)
            {
                PropertyInfo prop = validationContext.ObjectType.GetProperty(additionalField);
                if (prop != null)
                {
                    object propValue = prop.GetValue(validationContext.ObjectInstance, null);
                    propValues.Add(propValue);
                }
            }

            Type controllerType = Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(t => t.Name.ToLower() == (controllerName + "Controller").ToLower());
            if (controllerType != null)
            {
                // patch when EF save method it validates the attributes and at that moment is null
                //object instance = Activator.CreateInstance(controllerType, uow);

                object instance = KontinuityCRM.App_Start.IocConfig.Container.GetInstance(controllerType);

                MethodInfo method = controllerType.GetMethod(actionName);

                if (method != null)
                {
                    ActionResult response = (ActionResult)method.Invoke(instance, propValues.ToArray());

                    if (response is JsonResult)
                    {
                        bool isAvailable = false;
                        JsonResult json = (JsonResult)response;
                        string jsonData = json.Data.ToString();

                        bool.TryParse(jsonData, out isAvailable);

                        if (!isAvailable)
                            return new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName));
                    }
                }
            }

            return ValidationResult.Success;
        }

        public RemoteWithServerSideAttribute(string routeName)
            : base(routeName)
        {
        }

        public RemoteWithServerSideAttribute(string action, string controller)
            : base(action, controller)
        {
        }

        public RemoteWithServerSideAttribute(string action, string controller, string areaName)
            : base(action, controller, areaName)
        {
        }

    }

    /// <summary>
    /// Custom Credit Card Attribute for the Web.Api
    /// </summary>
    public class CustomCreditCardAttribute : ValidationAttribute
    {
        [StructureMap.Attributes.SetterProperty]
        public IUnitOfWork uow { get; set; }

        public override bool IsValid(object value)
        {
            if (uow.TestCardNumberRepository.Get(c => c.Number == value.ToString()).Any())
                return true;

            return new CreditCardAttribute().IsValid(value);
        }
       
    }

    /// <summary>
    /// 
    /// </summary>
    public class ValidateShippingMethodAttribute : ValidationAttribute
    {
        [StructureMap.Attributes.SetterProperty]
        public IUnitOfWork uow { get; set; }

        public override bool IsValid(object value)
        {
            return uow.ShippingMethodRepository.Find(value) != null;
        }

    }


    /// <summary>
    /// Validate that the credit card is not in the blacklist
    /// </summary>
    public class CheckBlackListAttribute : ValidationAttribute
    {
        [StructureMap.Attributes.SetterProperty]
        public IUnitOfWork uow { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) value = "";
            string cc = value.ToString().Trim().Replace("-", "");
            if (uow.BlackListRepository.Get(b => b.CreditCard.Equals(cc)).ToList().Count > 0)
                return new ValidationResult(String.Format(this.ErrorMessage, value));

            return null;
        }
    }
}