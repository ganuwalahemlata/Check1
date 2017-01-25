using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KontinuityCRM.Helpers
{
    public static class UrlHelperExtensions
    {
        /// <summary>
        /// Provides absolute Url for provided url
        /// </summary>
        /// <param name="url">Given Url</param>
        /// <param name="relativeContentPath">Provided Path</param>
        /// <returns></returns>
        public static string ContentAbsUrl(this UrlHelper url, string relativeContentPath)
        {
            Uri contextUri = HttpContext.Current.Request.Url;

            var baseUri = string.Format("{0}://{1}{2}", contextUri.Scheme,
                contextUri.Host, contextUri.Port == 80 ? string.Empty : ":" + contextUri.Port);

            return string.Format("{0}{1}", baseUri, VirtualPathUtility.ToAbsolute(relativeContentPath));
        }

    }
}