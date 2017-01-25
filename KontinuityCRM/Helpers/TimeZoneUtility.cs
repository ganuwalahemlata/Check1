using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace KontinuityCRM.Helpers
{
    public static class TimeZoneUtility
    {
        public static IOrderedEnumerable<TimeZoneInfo> TimeZones()
        {
            return TimeZoneInfo.GetSystemTimeZones().OrderBy(x => x.DisplayName);
        }

        public static TimeZoneInfo GetTimeZone(string timeZoneName)
        {
            return TimeZoneInfo.FindSystemTimeZoneById(timeZoneName);
        }

        public static DateTime ToUtc(DateTime dateTime, TimeZoneInfo fromTimeZone)
        {
            dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);
            return TimeZoneInfo.ConvertTime(dateTime, fromTimeZone, TimeZoneInfo.Utc);
        }

        public static DateTime FromUtc(DateTime utcDateTime, TimeZoneInfo toTimeZone)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, toTimeZone);
        }

        public static IList<T> CloneProps<T>(this IEnumerable<T> items) where T : new()
        {
            IList<T> list = new List<T>();
            List<PropertyInfo> props = typeof (T).GetProperties()
                .Where(x => x.CanRead && x.CanWrite)
                .ToList();

            foreach (T item in items)
            {
                var obj = new T();
                foreach (PropertyInfo prop in props)
                {
                    object value = prop.GetValue(item, null);
                    prop.SetValue(obj, value, null);
                }
                list.Add(obj);
            }
            return list;
        }

        public static IEnumerable<T> ConvertUtcTime<T>(this IEnumerable<T> items,
            TimeZoneInfo toTimeZone)
        {
            TimeZoneInfo fromTimeZone = TimeZoneInfo.Utc;
            List<PropertyInfo> props = typeof (T).GetProperties()
                .Where(p => p.PropertyType == typeof (DateTime)
                            || p.PropertyType == typeof (DateTime?)
                            || p.PropertyType == typeof(DateTimeOffset)
                            || p.PropertyType == typeof(DateTimeOffset?)
                )
                .Where(x => x.CanRead && x.CanWrite)
                .ToList();

            foreach (T obj in items)
            {
                foreach (PropertyInfo prop in props)
                {
                    object value = prop.GetValue(obj, null);
                    if (value != null)
                    {
                        if (prop.PropertyType.ToString().Contains("DateTimeOffset"))
                        {
                            prop.SetValue(obj, TimeZoneInfo.ConvertTime((DateTimeOffset)value, toTimeZone), null);
                        }
                        else if(prop.PropertyType.ToString().Contains("DateTime"))
                        {
                            prop.SetValue(obj, TimeZoneInfo.ConvertTime((DateTime)value, fromTimeZone, toTimeZone), null);
                        }
                        else
                        {
                            throw new Exception("unknown data type.");
                        }
                    }
                }
                yield return obj;
            }
        }
    }
}