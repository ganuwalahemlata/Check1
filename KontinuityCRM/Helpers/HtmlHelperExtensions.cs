using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DotNetOpenAuth.Messaging;
using WebMatrix.WebData;
using System.Collections.Generic;
using System.Text;

namespace KontinuityCRM.Helpers
{
    /// <summary>
    /// Html helper extensions
    /// </summary>
    public static class HtmlHelperExtensions
    {
        /// <summary>
        /// Renders a hidden css class depending on the security check
        /// </summary>
        /// <param name="html">HtmlHelper</param>
        /// <param name="securityRoles">Security roles to check</param>
        /// <param name="forceAll">If true, then all the security roles must be fullfilled</param>
        /// <returns>Return "hidden" css class if the authenticated user does not contain any of the roles</returns>
        public static string RenderHidden(this HtmlHelper html, string[] securityRoles, bool forceAll = false)
        {

            var wsw = new WebSecurityWrapper();


            // patch // njhones here!!
            //if (string.IsNullOrEmpty(wsw.CurrentUserName))
            //{
            //    return ""; // if "hidden" it doesn't show API documentation links
            //}
            // end patch

            var roleProvider = (SimpleRoleProvider)System.Web.Security.Roles.Provider;

            var roles = roleProvider.GetRolesForUser(wsw.CurrentUserName);
            if (roles != null)
            {


                if (roles.Contains(SecurityRole.Admin.ToString()))
                    return "";
                if (forceAll) return securityRoles.Any(securityRole => !roles.Contains(securityRole)) ? "hidden" : "";
                var containsRole = (roles.Any(securityRoles.Contains));
                return containsRole ? "" : "hidden";
            }
            else {
                return "hidden";
            }
        }

        /// <summary>
        /// Renders a hidden css class depending on the security check
        /// </summary>
        /// <param name="html">HtmlHelper</param>
        /// <param name="action">Action to check</param>
        /// <param name="controller">Controller to check</param>
        /// <returns>Return "hidden" css class if the authenticated user does not contain the role requested</returns>
        public static string RenderHidden(this HtmlHelper html, string action, string controller)
        {
            return RenderHidden(html, new[] {controller.ToUpperInvariant() + "_" + action.ToLowerInvariant()});
        }

        /// <summary>
        /// Renders a hidden css class depending on the security check, given an action
        /// </summary>
        /// <param name="html">HtmlHelper</param>
        /// <param name="action">Action to check</param>
        /// <returns>Return "hidden" css class if the authenticated user does not contain the role requested</returns>
        public static string RenderHidden(this HtmlHelper html, string action)
        {
            string controller = "";
            try
            {
                foreach (IValueProvider provider in ((ValueProviderCollection)html.ViewContext.Controller.ValueProvider))
                {
                    if (provider is RouteDataValueProvider)
                    {
                        var parsed = provider as RouteDataValueProvider;
                        controller = parsed.GetValue("controller").RawValue.ToString();
                        break;
                    }
                }
            }
            catch (Exception)
            {
                controller = "";
            }
            return RenderHidden(html, new[] {controller.ToUpperInvariant() + "_" + action.ToLowerInvariant()});
        }

        /// <summary>
        /// Renders the submenu entry according to the permissions granted to the logged user
        /// </summary>
        /// <param name="url">URL helper</param>
        /// <param name="action">Action to link to</param>
        /// <param name="controller">Controller to link to</param>
        /// <param name="menuText">Text to show as the menu entry</param>
        /// <param name="securityRoles">Roles to check in addition to the controller-action requested</param>
        /// <param name="routeValues">Route values to pass to the URL action</param>
        /// <returns>Returns the HTML element</returns>
        public static HtmlString RenderLayoutSubmenu(this UrlHelper url, string action, string controller, string menuText, 
            string currentController, string currentAction = "index",
            string[] securityRoles = null, object routeValues = null)
        {
            if (securityRoles == null)
                securityRoles = new string[0];
            var roles = new[] {controller.ToUpper() + "_" + action.ToLower()};
            roles.AddRange(securityRoles);
            var href = url.Action(action, controller);
            if (routeValues!=null)
                href = url.Action(action, controller, routeValues);

            var active = currentController.Equals(controller, StringComparison.InvariantCultureIgnoreCase) 
                && currentAction.Equals(action, StringComparison.InvariantCultureIgnoreCase);

            var html = "<li class='" + RenderHidden(null, roles) + (active ? "active open" : string.Empty) + "'>" +
                "    <a href='" + href + "' class='"+  (active ? "active" : string.Empty)  +"'>" +
                       "        <i class='menu-icon fa fa-caret-right'></i>" + menuText +
                       "    </a>" +
                       "    <b class='arrow'></b>" +
                       "</li>";
            return new HtmlString(html);
        }
        /// <summary>
        /// Url Extension method to render Layout SubMenu
        /// </summary>
        /// <param name="url">UrlHelper</param>
        /// <param name="action">action name</param>
        /// <param name="controller">controller name</param>
        /// <param name="menuText">menu text</param>
        /// <param name="active">active</param>
        /// <param name="securityRoles">security roles</param>
        /// <param name="routeValues">route Values</param>
        /// <returns></returns>
        public static HtmlString RenderLayoutSubmenu(this UrlHelper url, string action, string controller, string menuText,
           bool active,
           string[] securityRoles = null, object routeValues = null)
        {
            if (securityRoles == null)
                securityRoles = new string[0];
            var roles = new[] { controller.ToUpper() + "_" + action.ToLower() };
            roles.AddRange(securityRoles);
            var href = url.Action(action, controller);
            if (routeValues != null)
                href = url.Action(action, controller, routeValues);

            var html = "<li class='" + RenderHidden(null, roles) + (active ? "active open" : string.Empty) + "'>" +
                "    <a href='" + href + "' class='" + (active ? "active" : string.Empty) + "'>" +
                       "        <i class='menu-icon fa fa-caret-right'></i>" + menuText +
                       "    </a>" +
                       "    <b class='arrow'></b>" +
                       "</li>";
            return new HtmlString(html);
        }

        /*************************** to display js scripst and css styles on partial ***********************************/
        /// <summary>
        /// js view data name
        /// </summary>
        private const string _jSViewDataName = "RenderJavaScript";
        /// <summary>
        /// style view data name
        /// </summary>
        private const string _styleViewDataName = "RenderStyle";
        /// <summary>
        /// html helper exension method to add javascript
        /// </summary>
        /// <param name="htmlHelper">html helper</param>
        /// <param name="scriptURL">script url</param>
        public static void AddJavaScript(this HtmlHelper htmlHelper,
                                         string scriptURL)
        {
            List<string> scriptList = htmlHelper.ViewContext.HttpContext
              .Items[HtmlHelperExtensions._jSViewDataName] as List<string>;
            if (scriptList != null)
            {
                if (!scriptList.Contains(scriptURL))
                {
                    scriptList.Add(scriptURL);
                }
            }
            else
            {
                scriptList = new List<string>();
                scriptList.Add(scriptURL);
                htmlHelper.ViewContext.HttpContext
                  .Items.Add(HtmlHelperExtensions._jSViewDataName, scriptList);
            }
        }
        /// <summary>
        /// html helper exension method to render javascript
        /// </summary>
        /// <param name="HtmlHelper">htl helper</param>
        /// <returns></returns>
        public static MvcHtmlString RenderJavaScripts(this HtmlHelper HtmlHelper)
        {
            StringBuilder result = new StringBuilder();

            List<string> scriptList = HtmlHelper.ViewContext.HttpContext
              .Items[HtmlHelperExtensions._jSViewDataName] as List<string>;
            if (scriptList != null)
            {
                foreach (string script in scriptList)
                {
                    result.AppendLine(string.Format(
                      "<script type=\"text/javascript\" src=\"{0}\"></script>",
                      script));
                }
            }

            return MvcHtmlString.Create(result.ToString());
        }
        /// <summary>
        /// html helper exension method to add style
        /// </summary>
        /// <param name="htmlHelper">html helper</param>
        /// <param name="styleURL">style url</param>
        public static void AddStyle(this HtmlHelper htmlHelper, string styleURL)
        {
            List<string> styleList = htmlHelper.ViewContext.HttpContext
              .Items[HtmlHelperExtensions._styleViewDataName] as List<string>;

            if (styleList != null)
            {
                if (!styleList.Contains(styleURL))
                {
                    styleList.Add(styleURL);
                }
            }
            else
            {
                styleList = new List<string>();
                styleList.Add(styleURL);
                htmlHelper.ViewContext.HttpContext
                  .Items.Add(HtmlHelperExtensions._styleViewDataName, styleList);
            }
        }
        /// <summary>
        /// html helper exension method to render styles
        /// </summary>
        /// <param name="htmlHelper">html helper</param>
        /// <returns></returns>
        public static MvcHtmlString RenderStyles(this HtmlHelper htmlHelper)
        {
            StringBuilder result = new StringBuilder();

            List<string> styleList = htmlHelper.ViewContext.HttpContext
              .Items[HtmlHelperExtensions._styleViewDataName] as List<string>;

            if (styleList != null)
            {
                foreach (string script in styleList)
                {
                    result.AppendLine(string.Format(
                      "<link href=\"{0}\" rel=\"stylesheet\" type=\"text/css\" />",
                      script));
                }
            }

            return MvcHtmlString.Create(result.ToString());
        }
    }
}