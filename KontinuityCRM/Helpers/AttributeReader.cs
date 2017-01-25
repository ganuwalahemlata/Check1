using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;

namespace KontinuityCRM.Helpers
{
    public static class AttributeReader
    {
        /// <summary>
        /// Get Attribute of Type T
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="member">Member</param>
        /// <param name="isRequired">Required</param>
        /// <returns></returns>
        public static T GetAttribute<T>(this MemberInfo member, bool isRequired)
    where T : Attribute
        {
            var attribute = member.GetCustomAttributes(typeof(T), false).SingleOrDefault();

            if (attribute == null && isRequired)
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "The {0} attribute must be defined on member {1}",
                        typeof(T).Name,
                        member.Name));
            }

            return (T)attribute;
        }
        /// <summary>
        /// Get Property Display Name
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="propertyExpression">Expression</param>
        /// <returns></returns>
        public static string GetPropertyDisplayName<T>(Expression<Func<T, object>> propertyExpression)
        {
            var memberInfo = GetPropertyInformation(propertyExpression.Body);
            if (memberInfo == null)
            {
                throw new ArgumentException(
                    "No property reference expression was found.",
                    "propertyExpression");
            }

            var attr = memberInfo.GetAttribute<DisplayNameAttribute>(false);
            if (attr == null)
            {
                return memberInfo.Name;
            }

            return attr.DisplayName;
        }
        /// <summary>
        /// Get Property Information
        /// </summary>
        /// <param name="propertyExpression">Expresssion</param>
        /// <returns></returns>
        public static MemberInfo GetPropertyInformation(Expression propertyExpression)
        {
            Debug.Assert(propertyExpression != null, "propertyExpression != null");
            MemberExpression memberExpr = propertyExpression as MemberExpression;
            if (memberExpr == null)
            {
                UnaryExpression unaryExpr = propertyExpression as UnaryExpression;
                if (unaryExpr != null && unaryExpr.NodeType == ExpressionType.Convert)
                {
                    memberExpr = unaryExpr.Operand as MemberExpression;
                }
            }

            if (memberExpr != null && memberExpr.Member.MemberType == MemberTypes.Property)
            {
                return memberExpr.Member;
            }

            return null;
        }
    }

    /// <summary>
    /// Attribute for describing the HTML name field for a property
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class CustomHtmlNameAttribute : Attribute
    {
        private string name = "";

        /// <summary>
        /// Input type to use
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
    }

    /// <summary>
    /// Attribute for describing the HTML input type for a property
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class CustomTypeAttribute : Attribute
    {
        /// <summary>
        /// Default input type is TEXT
        /// </summary>
        private HtmlInputType inputType = HtmlInputType.text;

        /// <summary>
        /// Input type to use
        /// </summary>
        public HtmlInputType InputType
        {
            get { return inputType; }
            set { inputType = value; }
        }
    }

    public enum HtmlInputType
    {
        text,
        radio,
        email,
        date,
        datetime,
        file,
        image,
        password,
        checkbox,
        hidden,
        product,
        shippingMethod
    }
}