using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;

namespace KontinuityCRM.Helpers
{
    public static class EnumHelper<T> where T : struct, IComparable, IFormattable, IConvertible
    {
        /// <summary>
        /// Get Values of Enum
        /// </summary>
        /// <param name="value">provided Enum</param>
        /// <returns></returns>
        public static IList<T> GetValues(Enum value)
        {
            var enumValues = new List<T>();

            foreach (FieldInfo fi in value.GetType().GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                enumValues.Add((T)Enum.Parse(value.GetType(), fi.Name, false));
            }
            return enumValues;
        }
        /// <summary>
        /// Ge Value
        /// Csting object to Type T
        /// </summary>
        /// <param name="underLayingValue"></param>
        /// <returns></returns>
        public static T GetValue(object underLayingValue)
        {
            return (T)underLayingValue;
        }
        /// <summary>
        /// Parse string to Enum
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T Parse(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
        /// <summary>
        /// Get Names
        /// </summary>
        /// <param name="value">Enum</param>
        /// <returns></returns>
        public static IList<string> GetNames(Enum value)
        {
            return value.GetType().GetFields(BindingFlags.Static | BindingFlags.Public).Select(fi => fi.Name).ToList();
        }
        /// <summary>
        /// Get Display Names
        /// </summary>
        /// <param name="value">enum</param>
        /// <returns></returns>
        public static IList<string> GetDisplayValues(Enum value)
        {
            return GetNames(value).Select(obj => GetDisplayValue(Parse(obj))).ToList();
        }
        /// <summary>
        /// Get Display Value
        /// </summary>
        /// <param name="value">type T</param>
        /// <returns></returns>
        public static string GetDisplayValue(T value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());

            var descriptionAttributes = fieldInfo.GetCustomAttributes(
                typeof(DisplayAttribute), false) as DisplayAttribute[];

            if (descriptionAttributes == null) return string.Empty;
            return (descriptionAttributes.Length > 0) ? descriptionAttributes[0].Name : value.ToString();
        }

        /// <summary>
        ///     Get enum's names
        /// </summary>
        /// <returns>
        ///     A list of names as string
        /// </returns>
        public static List<string> Names()
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }
            return Enum.GetNames(typeof(T)).ToList();
        }
    }
}