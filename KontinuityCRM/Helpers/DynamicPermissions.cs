using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using KontinuityCRM.Controllers;
using WebMatrix.WebData;
using HttpPostAttribute = System.Web.Mvc.HttpPostAttribute;

namespace KontinuityCRM.Helpers
{

    public class DynamicPermissions
    {
        /// <summary>
        /// Generate Permissions based on role Provider
        /// </summary>
        public static void GeneratePermissions()
        {
            var roleProvider = (SimpleRoleProvider)System.Web.Security.Roles.Provider;
            List<string> detectedRoles = DetectPermissionsFromActions();
            var allRoles = roleProvider.GetAllRoles();
            var securityRoles = Enum.GetNames(typeof(SecurityRole));
            var toInsert = detectedRoles.Where(role => !allRoles.Contains(role));
            var toDelete = allRoles.Where(role => !detectedRoles.Contains(role) && !securityRoles.Contains(role));
            foreach (var role in toInsert)
            {
                roleProvider.CreateRole(role);
            }
            foreach (var role in toDelete)
            {
                roleProvider.DeleteRole(role, false);
            }
        }
        /// <summary>
        /// Get Assigned Permission
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetCurrentPermissions()
        {
            var roleProvider = (SimpleRoleProvider)System.Web.Security.Roles.Provider;
            var allRoles = roleProvider.GetAllRoles();
            List<string> detectedRoles = DetectPermissionsFromActions();
            var realPermissions = detectedRoles.Where(role => allRoles.Contains(role));
            return realPermissions;
        }
        /// <summary>
        /// detect permissions based on controller actions
        /// </summary>
        /// <returns></returns>
        private static List<string> DetectPermissionsFromActions()
        {
            List<string> detectedRoles = new List<string>();
            List<string> controllers = GetControllerNames();
            foreach (string controllerName in controllers)
            {
                Type controller = Assembly.GetExecutingAssembly().GetType(controllerName);
                var simpleName = controllerName.Split('.').Last().Replace("Controller", "");
                var actions = ActionNames(controller);
                detectedRoles.AddRange(actions.Select(action => simpleName.ToUpper() + "_" + action.ToLower()));
            }
            return detectedRoles;
        }

        /// <summary>
        /// Get SubClasses for Type T
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns></returns>
        private static List<Type> GetSubClasses<T>()
        {
            return Assembly.GetCallingAssembly().GetTypes().Where(
                type => type.IsSubclassOf(typeof(T))).ToList();
        }
        /// <summary>
        /// Returns list of actions of Controller
        /// </summary>
        /// <param name="controllerType">Type</param>
        /// <returns></returns>
        private static List<string> ActionNames(Type controllerType)
        {
            //return new ReflectedControllerDescriptor(controllerType).GetCanonicalActions().Select(x => x.ActionName).ToList();

            //return new ReflectedControllerDescriptor(controllerType).GetCanonicalActions().
            //    Where(
            //    x => !x.IsDefined(typeof(HttpPostAttribute), false) &&
            //    (((ReflectedActionDescriptor)x).MethodInfo.ReturnType == typeof(ActionResult) || ((ReflectedActionDescriptor)x).MethodInfo.ReturnType.IsSubclassOf(typeof(ActionResult)))
            //    ).Select(x => x.ActionName.ToLower()).ToList();

            ActionDescriptor[] adlist = new ReflectedControllerDescriptor(controllerType).GetCanonicalActions();
            List<string> rlist = new List<string>();
            foreach (var action in adlist)
            {

                // this is for skip actions with the httppostattribute
                if (action.IsDefined(typeof(HttpPostAttribute), false))
                    continue;
                ReflectedActionDescriptor rad = action as ReflectedActionDescriptor;
                if (rad != null)
                {
                    if (rad.MethodInfo.ReturnType == typeof(ActionResult) ||
                        rad.MethodInfo.ReturnType.IsSubclassOf(typeof(ActionResult)) ||
                        rad.MethodInfo.ReturnType.IsAssignableFrom(typeof(Task<ActionResult>)))
                        rlist.Add(rad.ActionName.ToLower());
                }

            }

            return rlist;

        }
        /// <summary>
        /// Returns List of ControllerNames
        /// </summary>
        /// <returns></returns>
        private static List<string> GetControllerNames()
        {
            List<string> controllerNames = new List<string>();
            GetSubClasses<Controller>().ForEach(
                type => controllerNames.Add(type.FullName));
            List<string> controllerNamesApi = new List<string>();
            GetSubClasses<ApiController>().ForEach(
                type => controllerNamesApi.Add(type.FullName));
            controllerNames.AddRange(controllerNamesApi);
            return controllerNames.Where(c=> 
                //c!=typeof(SuperController).FullName &&
                c != typeof(BaseController).FullName //&&
                //c != typeof(Base1Controller).FullName &&
                //c != typeof(Base2Controller).FullName
                )
                .ToList();
        }
    }
}