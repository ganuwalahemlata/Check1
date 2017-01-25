using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace KontinuityCRM.Helpers
{
    /// <summary>
    /// Roles that are going to be supported
    /// </summary>
    public enum SecurityRole
    {
        /// <summary>
        /// Form Generator
        /// </summary>
        [Display(Name="Form generation")]
        FormGenerator,
        /// <summary>
        /// Admin Security Role
        /// </summary>
        [Display(Name="Administrator")]
        Admin,
        /// <summary>
        /// Import CRM Security Role
        /// </summary>
        [Display(Name="Import Crm")]
        ImportCrm,
        [Display(Name = "Order Details")]
        OrderDetail,
        /// <summary>
        /// Order Batch Security Role
        /// </summary>
        [Display(Name = "Order Batch")]
        OrderBatch,
        /// <summary>
        /// Order Rebill Security Role
        /// </summary>
        [Display(Name = "Order Details")]
        OrderRebill,
        /// <summary>
        /// Order Start Security Role
        /// </summary>
        [Display(Name = "Order Start")]
        OrderStart,
        /// <summary>
        /// Order Stop Security Role
        /// </summary>
        [Display(Name = "Order Stop")]
        OrderStop,
        /// <summary>
        /// Order Export Security Role
        /// </summary>
        [Display(Name = "Order Export")]
        OrderExport,
        /// <summary>
        /// Order Delete Security Role
        /// </summary>
        [Display(Name = "Order Delete")]
        OrderDelete,
        /// <summary>
        /// For support team. Get email alerts
        /// </summary>
        Technical,
    }

    /// <summary>
    /// Verifies the created roles in the database versus the defined roles in SecurityRole enum
    /// If there is a role that doesn't exist in database, it will be created
    /// </summary>
    public static class SecurityCheck
    {
        /// <summary>
        /// Creates the missing roles
        /// </summary>
        public static void FillRoles()
        {
            var roleProvider = (SimpleRoleProvider)System.Web.Security.Roles.Provider;
            var allRoles = roleProvider.GetAllRoles();
            foreach (int value in Enum.GetValues(typeof(SecurityRole)))
            {
                string role = Enum.GetName(typeof (SecurityRole), value);
                if (!allRoles.Contains(role))
                    roleProvider.CreateRole(role);

            }
        }
    }

    /// <summary>
    /// Attribute that defines the roles that should be checked
    /// </summary>
    public class AuthorizationRoleAttribute : System.Web.Mvc.AuthorizeAttribute
    {
        private SecurityRole[] securityRoles;

        /// <summary>
        /// Security roles to verify
        /// </summary>
        public SecurityRole[] SecurityRoles
        {
            get
            {
                return securityRoles;
            }
            set
            {
                var extended = value.ToList();
                extended.Add(SecurityRole.Admin);                
                List<string> roles = extended.Select(role => role.ToString() + ",").ToList();
                Roles = roles.Aggregate((first, second) => first + second);
                if (Roles.Length > 0)
                    Roles = Roles.Remove(Roles.Length - 1);
                securityRoles = extended.ToArray();
            }
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
}