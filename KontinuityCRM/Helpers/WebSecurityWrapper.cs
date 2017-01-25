using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace KontinuityCRM.Helpers
{
    public interface IWebSecurityWrapper
    {
        int CurrentUserId { get; }

        string CurrentUserName { get; }

        RoleProvider RoleProvider { get; }
    }

    public class WebSecurityWrapper : IWebSecurityWrapper
    {
        /// <summary>
        /// Indicates the userId of current logged In User
        /// </summary>
        public int CurrentUserId
        {
            get
            {
                return WebMatrix.WebData.WebSecurity.CurrentUserId;
            }

        }
        /// <summary>
        /// Indicates the Name of curret Logged In User
        /// </summary>
        public string CurrentUserName
        {
            get
            {
                return WebMatrix.WebData.WebSecurity.CurrentUserName;
            }
        }
        /// <summary>
        /// Indicates the roles associated with LoggedInUser provided by Web.Security
        /// </summary>
        public RoleProvider RoleProvider
        {
            get { return Roles.Provider; }
        }
    }
}