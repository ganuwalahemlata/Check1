using AutoMapper.Mappers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using System.Web.WebPages;
using WebMatrix.WebData;

namespace KontinuityCRM.Helpers
{
    public static class HttpExtensionHelper
    {
        /// <summary>
        /// Makes a shallow copy of an entity object. This works much like a MemberwiseClone
        /// but directly instantiates a new object and copies only properties that work with
        /// EF and don't have the NotMappedAttribute.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="source">The source entity.</param>
        //public static TEntity ShallowCopyEntity<TEntity>(this TEntity source) where TEntity : class, new()
        //{

        //    // Get properties from EF that are read/write and not marked witht he NotMappedAttribute
        //    var sourceProperties = typeof(TEntity)
        //                            .GetProperties()
        //                            .Where(p => p.CanRead && p.CanWrite 
        //                                && p.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute), true).Length == 0
        //                            );
        //    var newObj = new TEntity();

        //    foreach (var property in sourceProperties)
        //    {

        //        // Copy value
        //        property.SetValue(newObj, property.GetValue(source, null), null);

        //    }

        //    return newObj;

        //}

        //    public static Expression<Func<T, bool>> AndAlso<T>(
        //this Expression<Func<T, bool>> expr1,
        //Expression<Func<T, bool>> expr2)
        //    {
        //         need to detect whether they use the same
        //         parameter instance; if not, they need fixing
        //        ParameterExpression param = expr1.Parameters[0];
        //        if (ReferenceEquals(param, expr2.Parameters[0]))
        //        {
        //             simple version
        //            return Expression.Lambda<Func<T, bool>>(
        //                Expression.AndAlso(expr1.Body, expr2.Body), param);
        //        }
        //         otherwise, keep expr1 "as is" and invoke expr2
        //        return Expression.Lambda<Func<T, bool>>(
        //            Expression.AndAlso(
        //                expr1.Body,
        //                Expression.Invoke(expr2, param)), param);
        //    }

        public static Expression<Func<T, bool>> AndAlso<T>(
        this Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2)
        {
            var parameter = Expression.Parameter(typeof(T));

            var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
            var left = leftVisitor.Visit(expr1.Body);

            var rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
            var right = rightVisitor.Visit(expr2.Body);

            return Expression.Lambda<Func<T, bool>>(
                Expression.AndAlso(left, right), parameter);
        }

        private class ReplaceExpressionVisitor
       : ExpressionVisitor
        {
            private readonly Expression _oldValue;
            private readonly Expression _newValue;

            public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
            {
                _oldValue = oldValue;
                _newValue = newValue;
            }

            public override Expression Visit(Expression node)
            {
                if (node == _oldValue)
                    return _newValue;
                return base.Visit(node);
            }
        }




        /// <summary>
        /// Shallow Copy
        /// </summary>
        /// <typeparam name="T">type T</typeparam>
        /// <param name="i"></param>
        /// <returns></returns>
        public static T CloneShallow<T>(this T i)
        {
            return
                (T)i.GetType()
                    .GetMethod(
                        "MemberwiseClone",
                        BindingFlags.Instance | BindingFlags.NonPublic
                    ).Invoke(i, null);
        }
        /// <summary>
        /// Select List from Enum
        /// </summary>
        /// <typeparam name="TEnum">Enum</typeparam>
        /// <param name="enumObj">Enum Object</param>
        /// <returns></returns>
        public static SelectList ToSelectList<TEnum>(this TEnum enumObj) where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            var values = from TEnum e in Enum.GetValues(typeof(TEnum))
                         select new { Id = e, Name = e.DisplayValue() };
            return new SelectList(values, "Id", "Name", enumObj);
        }

        /// <summary>
        // /o SelectIntLists
        /// </summary>
        /// <typeparam name="TEnum">Enum</typeparam>
        /// <param name="enumObj">Enum Object</param>
        /// <returns></returns>
        public static SelectList ToSelectIntList<TEnum>(this TEnum enumObj) where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            var values = from TEnum e in Enum.GetValues(typeof(TEnum))
                         select new { Id = e.GetType().GetField(e.ToString()).GetRawConstantValue().ToString(), Name = e.DisplayValue() };
            return new SelectList(values, "Id", "Name", enumObj.GetType().GetField(enumObj.ToString()).GetRawConstantValue().ToString());
        }

        public static string DisplayValue<TEnum>(this TEnum enumObj)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            try
            {
                var fieldInfo = enumObj.GetType().GetField(enumObj.ToString());

                // display attributes
                if (fieldInfo != null)
                {
                    var displayAttributes = fieldInfo.GetCustomAttributes(typeof(DisplayAttribute), false) as DisplayAttribute[];
                    if (displayAttributes != null && displayAttributes.Length > 0)
                        return displayAttributes[0].Name;
                }
                // description attributes
                var description = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), true).FirstOrDefault();
                if (description != null)
                {
                    return (description as DescriptionAttribute).Description;
                }

                return enumObj.ToString();
            }
            catch (Exception ex)
            {
                
                return string.Empty;
            }

            
        }
        /// <summary>
        /// Display class name for type
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns></returns>
        public static string DisplayClassName(this Type type)
        {
            var displaynameattrib = type.GetCustomAttribute(typeof(DisplayNameAttribute)) as DisplayNameAttribute;

            if (displaynameattrib != null)
            {
                return displaynameattrib.DisplayName;
            }

            return type.Name;
        }
        /// <summary>
        /// Get Not Nullable Model type
        /// </summary>
        /// <param name="modelMetadata">ModelMetaData</param>
        /// <returns></returns>
        private static Type GetNonNullableModelType(ModelMetadata modelMetadata)
        {
            Type realModelType = modelMetadata.ModelType;

            Type underlyingType = Nullable.GetUnderlyingType(realModelType);
            if (underlyingType != null)
            {
                realModelType = underlyingType;
            }
            return realModelType;
        }

        private static readonly SelectListItem[] SingleEmptyItem = new[] { new SelectListItem { Text = "", Value = "" } };
        /// <summary>
        /// Get Enum Description
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetEnumDescription<TEnum>(TEnum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if ((attributes != null) && (attributes.Length > 0))
                return attributes[0].Description;
            else
                return value.ToString();
        }
        /// <summary>
        /// html helper extension method to convert string to Mvc html string
        /// </summary>
        /// <param name="htmlHelper">html helper</param>
        /// <param name="text">text</param>
        /// <returns></returns>
        public static MvcHtmlString Nl2Br(this HtmlHelper htmlHelper, string text)
        {
            if (string.IsNullOrEmpty(text))
                return MvcHtmlString.Create(text);
            else
            {
                StringBuilder builder = new StringBuilder();
                string[] lines = text.Split('\n');
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i > 0)
                        builder.Append("<br/>\n");
                    builder.Append(HttpUtility.HtmlEncode(lines[i]));
                }
                return MvcHtmlString.Create(builder.ToString());
            }
        }

        /// <summary>
        /// Disambiguation for MVC 5.3.2
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static MvcHtmlString EnumDropdownListFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression)
        {
            return EnumDropDownListFor(htmlHelper, expression);
        }
        /// <summary>
        /// html helper extension method for enumDropDownList
        /// </summary>
        /// <typeparam name="TModel">Model Type</typeparam>
        /// <typeparam name="TEnum">Enum Type</typeparam>
        /// <param name="htmlHelper">html helper</param>
        /// <param name="expression">Expression</param>
        /// <returns></returns>
        public static MvcHtmlString EnumDropDownListFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression)
        {
            return EnumDropDownListFor(htmlHelper, expression, null);
        }

        public static MvcHtmlString EnumDropDownListFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression, object htmlAttributes)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            Type enumType = GetNonNullableModelType(metadata);
            IEnumerable<TEnum> values = Enum.GetValues(enumType).Cast<TEnum>();

            IEnumerable<SelectListItem> items = from value in values
                                                select new SelectListItem
                                                {
                                                    Text = GetEnumDescription(value),
                                                    Value = value.ToString(),
                                                    Selected = value.Equals(metadata.Model)
                                                };

            // If the enum is nullable, add an 'empty' item to the collection
            if (metadata.IsNullableValueType)
                items = SingleEmptyItem.Concat(items);

            return htmlHelper.DropDownListFor(expression, items, htmlAttributes);
        }
        /// <summary>
        /// Disambiguation for MVC 5.3.2 EnumDropDownListFor
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString EnumDropdownListFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression, object htmlAttributes)
        {
            return EnumDropDownListFor(htmlHelper, expression, htmlAttributes);
        }

        public static MvcHtmlString DisplayEnumFor<TModel, TResult>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TResult>> expression) where TResult : struct
        {
            TResult value = expression.Compile().Invoke(helper.ViewData.Model);
            string propName = ExpressionHelper.GetExpressionText(expression);

            var description = typeof(TResult).GetMember(value.ToString())[0]
                .GetCustomAttributes(typeof(DescriptionAttribute), true).FirstOrDefault();
            if (description != null)
            {
                return MvcHtmlString.Create((description as DescriptionAttribute).Description);
            }

            return MvcHtmlString.Create(value.ToString());
        }

        public static IHtmlString CheckboxListForDynamicEnumBootStrap(this HtmlHelper htmlhelper, string name, long modelItems, string enumtypeName)
        {
            StringBuilder sb = new StringBuilder();

            //Type enumtype = CPAHelper.GetEnumType();
            var assembly = System.Reflection.Assembly.Load("EnumAssembly");
            Type enumtype = assembly.GetType(enumtypeName);

            foreach (object item in Enum.GetValues(enumtype))
            {
                long targetValue = Convert.ToInt64(item);

                var ti = htmlhelper.ViewData.TemplateInfo;
                var id = ti.GetFullHtmlFieldId(item.ToString());

                // builder is the input type=checkbox
                var builder = new TagBuilder("input");

                //if (penum.HasFlag(item as Enum))
                if ((targetValue & modelItems) == targetValue)
                    builder.MergeAttribute("checked", "checked");

                builder.MergeAttribute("type", "checkbox");
                builder.MergeAttribute("value", ((long)item).ToString());
                builder.MergeAttribute("name", name);

                //// label

                var label = new TagBuilder("label");
                //label.Attributes["for"] = id; bootstrap doesn't function with for attrib
                //label.Attributes["class"] = "checkbox";

                // this is for grab the display attribute
                var field = item.GetType().GetField(item.ToString());
                var display = field.GetCustomAttributes(typeof(DisplayAttribute), true).FirstOrDefault() as DisplayAttribute;
                if (display == null)
                    label.SetInnerText(item.ToString());
                else
                    label.SetInnerText(display.Name);

                // set the builder inside the label
                label.InnerHtml = builder.ToString(TagRenderMode.SelfClosing) + " " + item.ToString();

                sb.AppendLine(label.ToString());
            }

            return new HtmlString(sb.ToString());
        }

        /// <summary>
        /// Generates the checkboxes for the roles
        /// </summary>
        /// <param name="htmlhelper">Helper</param>
        /// <param name="name">Input name attribute</param>
        /// <param name="userName">User name to find the roles</param>
        /// <returns>Returns the string representation</returns>
        public static IHtmlString CheckboxListForRoles(this HtmlHelper htmlhelper, string name, string userName)
        {
            StringBuilder sb = new StringBuilder();
            var roleProvider = (SimpleRoleProvider)System.Web.Security.Roles.Provider;
            // Filter only those that really exist in the enum
            var roles = roleProvider.GetAllRoles().Where(r => Enum.GetNames(typeof(SecurityRole)).Contains(r)).ToList();
            var rolesForUser = roleProvider.GetRolesForUser(userName);
            roles.AddRange(DynamicPermissions.GetCurrentPermissions());
            roles = roles.OrderBy(role => role).ToList();
            foreach (var role in roles)
            {
                // builder is the input type=checkbox
                var builderDiv = new TagBuilder("div");
                builderDiv.AddCssClass("col-xs-12 col-sm-6 col-md-4");
                var builder = new TagBuilder("input");
                if (rolesForUser.Contains(role))
                    builder.MergeAttribute("checked", "checked");
                builder.MergeAttribute("type", "checkbox");
                builder.MergeAttribute("value", role);
                builder.MergeAttribute("name", name);

                //// label
                var label = new TagBuilder("label");
                label.SetInnerText(role);
                SecurityRole result;
                bool parsed = SecurityRole.TryParse(role, out result);
                var displayValue = parsed ? EnumHelper<SecurityRole>.GetDisplayValue(result) : role;
                label.InnerHtml = builder.ToString(TagRenderMode.SelfClosing) + " " + displayValue;
                builderDiv.InnerHtml = label.ToString();
                sb.AppendLine(builderDiv.ToString());
            }
            return new HtmlString(sb.ToString());
        }

        /// <summary>
        /// Generates the checkboxes for the roles
        /// </summary>
        /// <param name="htmlhelper">Helper</param>
        /// <param name="name">Input name attribute</param>
        /// <param name="group">Group to find the roles</param>
        /// <returns>Returns the string representation</returns>
        public static IHtmlString CheckboxListForGroupRoles(this HtmlHelper htmlhelper, string name, KontinuityCRM.Models.UserGroup group)
        {
            StringBuilder sb = new StringBuilder();
            var roleProvider = (SimpleRoleProvider)System.Web.Security.Roles.Provider;
            // Filter only those that really exist in the enum
            var roles = roleProvider.GetAllRoles().Where(r => Enum.GetNames(typeof(SecurityRole)).Contains(r)).ToList();

            // Aca recuperar los roles del grupo si hay
            string[] rolesForGroup = { };

            if (group != null && group.Roles != null && group.Roles.Count > 0)
            {
                rolesForGroup = group.Roles.Select(r => r.Role).ToArray();
            }
                    
            roles.AddRange(DynamicPermissions.GetCurrentPermissions());
            roles = roles.OrderBy(role => role).ToList();

            foreach (var role in roles)
            {
                // builder is the input type=checkbox
                var builderDiv = new TagBuilder("div");
                
                var builder = new TagBuilder("input");
                if (rolesForGroup.Contains(role))
                    builder.MergeAttribute("checked", "checked");
                builder.MergeAttribute("type", "checkbox");
                builder.MergeAttribute("value", role);
                builder.MergeAttribute("name", name);

                //// label
                var label = new TagBuilder("label");
                label.SetInnerText(role);
                SecurityRole result;
                bool parsed = SecurityRole.TryParse(role, out result);
                var displayValue = parsed ? EnumHelper<SecurityRole>.GetDisplayValue(result) : role;
                label.InnerHtml = builder.ToString(TagRenderMode.SelfClosing) + " " + displayValue;
                builderDiv.InnerHtml = label.ToString();
                sb.AppendLine(builderDiv.ToString());
            }
            return new HtmlString(sb.ToString());
        }
        /// <summary>
        /// html helper extension method to create Checkboxes for an enum model
        /// </summary>
        /// <typeparam name="TModel">Model Type</typeparam>
        /// <param name="htmlHelper">html helper</param>
        /// <returns></returns>
        public static IHtmlString CheckBoxesForEnumModel<TModel>(this HtmlHelper<TModel> htmlHelper)
        {
            if (!typeof(TModel).IsEnum)
            {
                throw new ArgumentException("this helper can only be used with enums");
            }
            var sb = new StringBuilder();
            foreach (Enum item in Enum.GetValues(typeof(TModel)))
            {
                var ti = htmlHelper.ViewData.TemplateInfo;
                var id = ti.GetFullHtmlFieldId(item.ToString());
                var name = ti.GetFullHtmlFieldName(string.Empty);
                var label = new TagBuilder("label");
                label.Attributes["for"] = id;
                label.SetInnerText(item.ToString());
                sb.AppendLine(label.ToString());

                var checkbox = new TagBuilder("input");
                checkbox.Attributes["id"] = id;
                checkbox.Attributes["name"] = name;
                checkbox.Attributes["type"] = "checkbox";
                checkbox.Attributes["value"] = item.ToString();
                var model = htmlHelper.ViewData.Model as Enum;
                if (model.HasFlag(item))
                {
                    checkbox.Attributes["checked"] = "checked";
                }
                sb.AppendLine(checkbox.ToString());
            }

            return new HtmlString(sb.ToString());
        }
        /// <summary>
        /// html helper extension method script
        /// </summary>
        /// <param name="htmlHelper">html helper</param>
        /// <param name="template">template</param>
        /// <returns></returns>
        public static MvcHtmlString Script(this HtmlHelper htmlHelper, Func<object, HelperResult> template)
        {
            htmlHelper.ViewContext.HttpContext.Items["_script_" + Guid.NewGuid()] = template;
            return MvcHtmlString.Empty;
        }
        /// <summary>
        /// html helper extension method to render scripts
        /// </summary>
        /// <param name="htmlHelper">html helper</param>
        /// <returns></returns>
        public static IHtmlString RenderScripts(this HtmlHelper htmlHelper)
        {
            foreach (object key in htmlHelper.ViewContext.HttpContext.Items.Keys)
            {
                if (key.ToString().StartsWith("_script_"))
                {
                    var template = htmlHelper.ViewContext.HttpContext.Items[key] as Func<object, HelperResult>;
                    if (template != null)
                    {
                        htmlHelper.ViewContext.Writer.Write(template(null));
                    }
                }
            }
            return MvcHtmlString.Empty;
        }


    }



    public class GroupedSelectListItem : SelectListItem
    {
        /// <summary>
        /// Grooup key
        /// </summary>
        public string GroupKey { get; set; }
        /// <summary>
        /// Group Name
        /// </summary>
        public string GroupName { get; set; }
    }

    public static class HtmlHelpers
    {
        /// <summary>
        /// Get DropDownGroupList
        /// </summary>
        /// <param name="htmlHelper">htmlHelper</param>
        /// <param name="name">name</param>
        /// <returns></returns>
        public static MvcHtmlString DropDownGropList(this HtmlHelper htmlHelper, string name)
        {
            return DropDownListHelper(htmlHelper, name, null, null, null);
        }
        /// <summary>
        /// Get DropDownGroupList
        /// </summary>
        /// <param name="htmlHelper">html helper</param>
        /// <param name="name">name</param>
        /// <param name="selectList">select List</param>
        /// <returns></returns>
        public static MvcHtmlString DropDownGropList(this HtmlHelper htmlHelper, string name, IEnumerable<GroupedSelectListItem> selectList)
        {
            return DropDownListHelper(htmlHelper, name, selectList, null, null);
        }
        /// <summary>
        /// Get DropDownGroupList
        /// </summary>
        /// <param name="htmlHelper">html hlper</param>
        /// <param name="name">name</param>
        /// <param name="optionLabel">option label</param>
        /// <returns></returns>
        public static MvcHtmlString DropDownGropList(this HtmlHelper htmlHelper, string name, string optionLabel)
        {
            return DropDownListHelper(htmlHelper, name, null, optionLabel, null);
        }
        /// <summary>
        /// Get DropDownGroupList
        /// </summary>
        /// <param name="htmlHelper">html helper</param>
        /// <param name="name">name</param>
        /// <param name="selectList">select list</param>
        /// <param name="htmlAttributes">html Attribute</param>
        /// <returns></returns>
        public static MvcHtmlString DropDownGropList(this HtmlHelper htmlHelper, string name, IEnumerable<GroupedSelectListItem> selectList, IDictionary<string, object> htmlAttributes)
        {
            return DropDownListHelper(htmlHelper, name, selectList, null, htmlAttributes);
        }
        /// <summary>
        /// Get DropDownGroupList
        /// </summary>
        /// <param name="htmlHelper">html helper</param>
        /// <param name="name">name</param>
        /// <param name="selectList">select list</param>
        /// <param name="htmlAttributes">html Attribute</param>
        /// <returns></returns>
        public static MvcHtmlString DropDownGropList(this HtmlHelper htmlHelper, string name, IEnumerable<GroupedSelectListItem> selectList, object htmlAttributes)
        {
            return DropDownListHelper(htmlHelper, name, selectList, null, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }
        /// <summary>
        /// Get DropDownGroupList
        /// </summary>
        /// <param name="htmlHelper">html helper</param>
        /// <param name="name"></param>
        /// <param name="selectList"></param>
        /// <param name="optionLabel"></param>
        /// <returns></returns>
        public static MvcHtmlString DropDownGropList(this HtmlHelper htmlHelper, string name, IEnumerable<GroupedSelectListItem> selectList, string optionLabel)
        {
            return DropDownListHelper(htmlHelper, name, selectList, optionLabel, null);
        }
        /// <summary>
        /// Get DropDownGroupList
        /// </summary>
        /// <param name="htmlHelper">html helper</param>
        /// <param name="name">name</param>
        /// <param name="selectList">select lists</param>
        /// <param name="optionLabel">option list</param>
        /// <param name="htmlAttributes">html Attribute</param>
        /// <returns></returns>
        public static MvcHtmlString DropDownGropList(this HtmlHelper htmlHelper, string name, IEnumerable<GroupedSelectListItem> selectList, string optionLabel, IDictionary<string, object> htmlAttributes)
        {
            return DropDownListHelper(htmlHelper, name, selectList, optionLabel, htmlAttributes);
        }
        /// <summary>
        /// Get DropDownGroupList
        /// </summary>
        /// <param name="htmlHelper">html helper</param>
        /// <param name="name">name</param>
        /// <param name="selectList">select list</param>
        /// <param name="optionLabel">option list</param>
        /// <param name="htmlAttributes">html Attribute</param>
        /// <returns></returns>
        public static MvcHtmlString DropDownGropList(this HtmlHelper htmlHelper, string name, IEnumerable<GroupedSelectListItem> selectList, string optionLabel, object htmlAttributes)
        {
            return DropDownListHelper(htmlHelper, name, selectList, optionLabel, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }
        /// <summary>
        /// Get DropDownGroupList
        /// </summary>
        /// <typeparam name="TModel">Model Type</typeparam>
        /// <typeparam name="TProperty">Property Type</typeparam>
        /// <param name="htmlHelper">html helper</param>
        /// <param name="expression">expression</param>
        /// <param name="selectList">select list</param>
        /// <returns></returns>
        public static MvcHtmlString DropDownGroupListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<GroupedSelectListItem> selectList)
        {
            return DropDownGroupListFor(htmlHelper, expression, selectList, null /* optionLabel */, null /* htmlAttributes */);
        }
        /// <summary>
        /// Get DropDownGroupListFor
        /// </summary>
        /// <typeparam name="TModel">Model Type</typeparam>
        /// <typeparam name="TProperty">Property Type</typeparam>
        /// <param name="htmlHelper">html Helper</param>
        /// <param name="expression">Expression</param>
        /// <param name="selectList">Select List</param>
        /// <param name="htmlAttributes">html Attribute</param>
        /// <returns></returns>
        public static MvcHtmlString DropDownGroupListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<GroupedSelectListItem> selectList, object htmlAttributes)
        {
            return DropDownGroupListFor(htmlHelper, expression, selectList, null /* optionLabel */, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }
        /// <summary>
        /// Get DropDownGroupListFor
        /// </summary>
        /// <typeparam name="TModel">Model Type</typeparam>
        /// <typeparam name="TProperty">Property Type</typeparam>
        /// <param name="htmlHelper">html Helper</param>
        /// <param name="expression">expression</param>
        /// <param name="selectList">select List</param>
        /// <param name="htmlAttributes">html Attribute</param>
        /// <returns></returns>
        public static MvcHtmlString DropDownGroupListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<GroupedSelectListItem> selectList, IDictionary<string, object> htmlAttributes)
        {
            return DropDownGroupListFor(htmlHelper, expression, selectList, null /* optionLabel */, htmlAttributes);
        }
        /// <summary>
        /// Get DropDownGroupListFor
        /// </summary>
        /// <typeparam name="TModel">Model Type</typeparam>
        /// <typeparam name="TProperty">Property Type</typeparam>
        /// <param name="htmlHelper">html Helper</param>
        /// <param name="expression">Expression</param>
        /// <param name="selectList">select List</param>
        /// <param name="optionLabel">Option List</param>
        /// <returns></returns>
        public static MvcHtmlString DropDownGroupListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<GroupedSelectListItem> selectList, string optionLabel)
        {
            return DropDownGroupListFor(htmlHelper, expression, selectList, optionLabel, null /* htmlAttributes */);
        }
        /// <summary>
        /// Get DropDownGroupListFor
        /// </summary>
        /// <typeparam name="TModel">Model Type</typeparam>
        /// <typeparam name="TProperty">Property Type</typeparam>
        /// <param name="htmlHelper">html helper</param>
        /// <param name="expression">Expression</param>
        /// <param name="selectList">Select List</param>
        /// <param name="optionLabel">Option Label</param>
        /// <param name="htmlAttributes">html Attribute</param>
        /// <returns></returns>
        public static MvcHtmlString DropDownGroupListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<GroupedSelectListItem> selectList, string optionLabel, object htmlAttributes)
        {
            return DropDownGroupListFor(htmlHelper, expression, selectList, optionLabel, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }
        /// <summary>
        /// Get DropDownGroupListFor
        /// </summary>
        /// <typeparam name="TModel">Model Type</typeparam>
        /// <typeparam name="TProperty">Property Type</typeparam>
        /// <param name="htmlHelper">html helper</param>
        /// <param name="expression">Expression</param>
        /// <param name="selectList">select List</param>
        /// <param name="optionLabel">option Label</param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString DropDownGroupListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<GroupedSelectListItem> selectList, string optionLabel, IDictionary<string, object> htmlAttributes)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            return DropDownListHelper(htmlHelper, ExpressionHelper.GetExpressionText(expression), selectList, optionLabel, htmlAttributes);
        }
        /// <summary>
        /// Get DropDownGroupListFor
        /// </summary>
        /// <param name="htmlHelper">html helper</param>
        /// <param name="expression">expression</param>
        /// <param name="selectList">select List</param>
        /// <param name="optionLabel">option List</param>
        /// <param name="htmlAttributes">html Attribute</param>
        /// <returns></returns>
        private static MvcHtmlString DropDownListHelper(HtmlHelper htmlHelper, string expression, IEnumerable<GroupedSelectListItem> selectList, string optionLabel, IDictionary<string, object> htmlAttributes)
        {
            return SelectInternal(htmlHelper, optionLabel, expression, selectList, false /* allowMultiple */, htmlAttributes);
        }




        // Helper methods
        /// <summary>
        /// Get Select Data
        /// </summary>
        /// <param name="htmlHelper">html Helper</param>
        /// <param name="name">name</param>
        /// <returns></returns>
        private static IEnumerable<GroupedSelectListItem> GetSelectData(this HtmlHelper htmlHelper, string name)
        {
            object o = null;
            if (htmlHelper.ViewData != null)
            {
                o = htmlHelper.ViewData.Eval(name);
            }
            if (o == null)
            {
                throw new InvalidOperationException(
                    String.Format(
                        CultureInfo.CurrentCulture,
                        "Missing Select Data"));
            }
            var selectList = o as IEnumerable<GroupedSelectListItem>;
            if (selectList == null)
            {
                throw new InvalidOperationException(
                    String.Format(
                        CultureInfo.CurrentCulture,
                        "Wrong Select DataType"));
            }
            return selectList;
        }
        /// <summary>
        /// Convert List Item to options
        /// </summary>
        /// <param name="item">GroupSelectListItem</param>
        /// <returns></returns>
        internal static string ListItemToOption(GroupedSelectListItem item)
        {
            var builder = new TagBuilder("option")
            {
                InnerHtml = HttpUtility.HtmlEncode(item.Text)
            };
            if (item.Value != null)
            {
                builder.Attributes["value"] = item.Value;
            }
            if (item.Selected)
            {
                builder.Attributes["selected"] = "selected";
            }
            if (item.Disabled)
            {
                builder.Attributes["disabled"] = "disabled";
            }
            return builder.ToString(TagRenderMode.Normal);
        }
        /// <summary>
        /// Select Internal
        /// </summary>
        /// <param name="htmlHelper">html Helper</param>
        /// <param name="optionLabel">option label</param>
        /// <param name="name">name </param>
        /// <param name="selectList">select list</param>
        /// <param name="allowMultiple">Boolean</param>
        /// <param name="htmlAttributes">html Attributr</param>
        /// <returns></returns>
        private static MvcHtmlString SelectInternal(this HtmlHelper htmlHelper, string optionLabel, string name, IEnumerable<GroupedSelectListItem> selectList, bool allowMultiple, IDictionary<string, object> htmlAttributes)
        {
            var originalname = name; // njhones here !!
            name = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Null Or Empty", "name");
            }

            bool usedViewData = false;

            // If we got a null selectList, try to use ViewData to get the list of items.
            if (selectList == null)
            {
                selectList = htmlHelper.GetSelectData(name);
                usedViewData = true;
            }

            object defaultValue = (allowMultiple) ? htmlHelper.GetModelStateValue(name, typeof(string[])) : htmlHelper.GetModelStateValue(name, typeof(string));
            if (defaultValue == null) // njhones here!!
            {
                defaultValue = htmlHelper.GetDefaultValue(originalname);
            }

            // If we haven't already used ViewData to get the entire list of items then we need to
            // use the ViewData-supplied value before using the parameter-supplied value.
            if (!usedViewData)
            {
                if (defaultValue == null)
                {
                    defaultValue = htmlHelper.ViewData.Eval(name);
                }
            }

            if (defaultValue != null)
            {
                var defaultValues = (allowMultiple) ? defaultValue as IEnumerable : new[] { defaultValue };
                var values = from object value in defaultValues select Convert.ToString(value, CultureInfo.CurrentCulture);
                var selectedValues = new HashSet<string>(values, StringComparer.OrdinalIgnoreCase);
                var newSelectList = new List<GroupedSelectListItem>();

                foreach (var item in selectList)
                {
                    item.Selected = (item.Value != null) ? selectedValues.Contains(item.Value) : selectedValues.Contains(item.Text);
                    newSelectList.Add(item);
                }
                selectList = newSelectList;
            }

            // Convert each ListItem to an <option> tag
            var listItemBuilder = new StringBuilder();

            // Make optionLabel the first item that gets rendered.
            if (optionLabel != null)
            {
                listItemBuilder.AppendLine(ListItemToOption(new GroupedSelectListItem { Text = optionLabel, Value = String.Empty, Selected = false }));
            }

            foreach (var group in selectList.GroupBy(i => i.GroupKey))
            {
                string groupName = selectList.Where(i => i.GroupKey == group.Key).Select(it => it.GroupName).FirstOrDefault();
                listItemBuilder.AppendLine(string.Format("<optgroup label=\"{0}\" value=\"{1}\">", groupName, group.Key));
                foreach (GroupedSelectListItem item in group)
                {
                    listItemBuilder.AppendLine(ListItemToOption(item));
                }
                listItemBuilder.AppendLine("</optgroup>");
            }

            var tagBuilder = new TagBuilder("select")
            {
                InnerHtml = listItemBuilder.ToString()
            };
            tagBuilder.MergeAttributes(htmlAttributes);
            tagBuilder.MergeAttribute("name", name, true /* replaceExisting */);
            tagBuilder.GenerateId(name);
            if (allowMultiple)
            {
                tagBuilder.MergeAttribute("multiple", "multiple");
            }

            // If there are any errors for a named field, we add the css attribute.
            ModelState modelState;
            if (htmlHelper.ViewData.ModelState.TryGetValue(name, out modelState))
            {
                if (modelState.Errors.Count > 0)
                {
                    tagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);
                }
            }

            return MvcHtmlString.Create(tagBuilder.ToString());
        }

        /// <summary>
        /// Get ModelStateValue
        /// </summary>
        /// <param name="helper">html helper</param>
        /// <param name="key">key</param>
        /// <param name="destinationType">Destination Type</param>
        /// <returns></returns>
        internal static object GetModelStateValue(this HtmlHelper helper, string key, Type destinationType)
        {
            ModelState modelState;
            if (helper.ViewData.ModelState.TryGetValue(key, out modelState))
            {
                if (modelState.Value != null)
                {
                    return modelState.Value.ConvertTo(destinationType, null /* culture */);
                }
            }

            return null;
        }

        internal static object GetDefaultValue(this HtmlHelper helper, string key)
        {
            object value = null;
            if (value == null && helper.ViewData.Model != null)
            {
                var p = TypeDescriptor.GetProperties(helper.ViewData.Model).Find(key, false);

                if (p != null)
                    value = p.GetValue(helper.ViewData.Model);
            }

            //get val from model metada
            if (value == null && helper.ViewData.ModelMetadata != null)
            {
                var p = TypeDescriptor.GetProperties(helper.ViewData.ModelMetadata.Model).Find(key, false);
                if (p != null)
                    value = p.GetValue(helper.ViewData.ModelMetadata.Model);
            }

            //get val from viewdata
            if (value == null && helper.ViewData.ContainsKey(key))
                value = helper.ViewData[key];


            //if (value == null)
            //{
            //    return value.ConvertTo(destinationType, null /* culture */);
            //}

            return value;
        }

        /////////////////////////////////

        /// <summary>
        /// ActionQueryLink
        /// </summary>
        /// <param name="htmlHelper">html helper</param>
        /// <param name="linkText">link text</param>
        /// <param name="action">action name</param>
        /// <returns></returns>
        public static MvcHtmlString ActionQueryLink(this HtmlHelper htmlHelper, string linkText, string action)
        {
            return ActionQueryLink(htmlHelper, linkText, action, null, null, null);
        }
        /// <summary>
        /// ActionQueryLink
        /// </summary>
        /// <param name="htmlHelper">html helper</param>
        /// <param name="linkText">link text</param>
        /// <param name="action">action name</param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString ActionQueryLink(this HtmlHelper htmlHelper, string linkText, string action, object htmlAttributes)
        {
            return ActionQueryLink(htmlHelper, linkText, action, null, htmlAttributes, null);
        }
        /// <summary>
        /// ActionQueryLink
        /// </summary>
        /// <param name="htmlHelper">html helper</param>
        /// <param name="linkText">link text</param>
        /// <param name="action">action name</param>
        /// <param name="routeValues">Route Values</param>
        /// <param name="htmlAttributes">html Attributes</param>
        /// <returns></returns>
        public static MvcHtmlString ActionQueryLink(this HtmlHelper htmlHelper, string linkText, string action, object routeValues, object htmlAttributes)
        {
            return ActionQueryLink(htmlHelper, linkText, action, routeValues, htmlAttributes, null);
        }
        /// <summary>
        /// ActionQueryLink
        /// </summary>
        /// <param name="htmlHelper">html helper</param>
        /// <param name="linkText">link text</param>
        /// <param name="actionName">action name</param>
        /// <param name="routeValues">route values</param>
        /// <param name="htmlAttributes">html Attribute</param>
        /// <param name="discardRoutes">discard route values</param>
        /// <returns></returns>
        public static MvcHtmlString ActionQueryLink(this HtmlHelper htmlHelper, string linkText, string actionName, object routeValues, object htmlAttributes, string discardRoutes)
        {
            var queryString =
                htmlHelper.ViewContext.HttpContext.Request.QueryString;

            var newRoute = routeValues == null
                ? htmlHelper.ViewContext.RouteData.Values
                : new RouteValueDictionary(routeValues);

            string[] discard = discardRoutes == null ? new string[] { } : discardRoutes.Split(',');

            foreach (string key in queryString.Keys)
            {
                if (!newRoute.ContainsKey(key) && !discard.Contains(key))
                    newRoute.Add(key, queryString[key]);
            }
            return MvcHtmlString.Create(HtmlHelper.GenerateLink(htmlHelper.ViewContext.RequestContext,
                htmlHelper.RouteCollection, linkText, null /* routeName */,
                actionName, null, newRoute, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes))); //  (IDictionary<string, object>)htmlAttributes
        }
        /// <summary>
        /// ActionQueryLink
        /// </summary>
        /// <param name="urlHelper">url Helper</param>
        /// <param name="action">action name</param>
        /// <param name="routeValues">route Values</param>
        /// <param name="discardRoutes">discard values</param>
        /// <returns></returns>
        public static string ActionQueryUrl(this System.Web.Mvc.UrlHelper urlHelper, string action, object routeValues = null, string discardRoutes = null)
        {
            var queryString =
                urlHelper.RequestContext.HttpContext.Request.QueryString;

            var newRoute = routeValues == null
                ? urlHelper.RequestContext.RouteData.Values
                : new RouteValueDictionary(routeValues);

            string[] discard = discardRoutes == null ? new string[] { } : discardRoutes.Split(',');

            foreach (string key in queryString.Keys)
            {
                if (!newRoute.ContainsKey(key) && !discard.Contains(key, new IngnoreCase()))
                    newRoute.Add(key, queryString[key]);
            }
            // var s = urlHelper.Action(action, newRoute);
            return urlHelper.Action(action, newRoute);

        }

    }

    public class IngnoreCase : IEqualityComparer<string>
    {
        /// <summary>
        /// string equality
        /// </summary>
        /// <param name="x">value one</param>
        /// <param name="y">value two</param>
        /// <returns></returns>
        public bool Equals(string x, string y)
        {
            return x.Equals(y, StringComparison.OrdinalIgnoreCase);
        }
        /// <summary>
        /// get hashcode of string
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetHashCode(string obj)
        {
            return obj.GetHashCode();
        }
    }

    public class CaseInsensitiveComparer : IComparer<string>
    {
        /// <summary>
        /// String Comparer
        /// </summary>
        /// <param name="x">First Value</param>
        /// <param name="y">Second Value</param>
        /// <returns></returns>
        public int Compare(string x, string y)
        {
            return string.Compare(x, y, StringComparison.OrdinalIgnoreCase);
        }
    }
}