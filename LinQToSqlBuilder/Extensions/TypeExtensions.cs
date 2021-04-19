using Dapper.SqlBuilder.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Dapper.SqlBuilder.Extensions
{
    public static class TypeExtensions
    {
        public static Type GetCoreType(this Type @this)
        {
            if (@this?.IsNullable() == true)
            {
                @this = Nullable.GetUnderlyingType(@this);
            }
            return @this;
        }

        public static bool IsNullable(this Type @this)
        {
            return @this.IsValueType && @this.IsGenericType && @this.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static IEnumerable<PropertyInfo> GetMappedProperties(this Type type)
        {
            return type
              .GetProperties()
              .Select(p => GetMappedProperty(type, p.Name))
              .Where(p => p != null);
        }

        static PropertyInfo GetMappedProperty(Type type, string name)
        {
            if (type == null)
                return null;

            var prop = type.GetProperty(name);

            if (prop.DeclaringType == type)
                return prop;
            else
                return GetMappedProperty(type.BaseType, name);
        }

        public static LambdaExpression CreateExpression(this Type type, string propertyName)
        {
            var param = Expression.Parameter(type, "x");
            Expression body = param;
            foreach (var member in propertyName.Split('.'))
            {
                body = Expression.PropertyOrField(body, member);
            }
            return Expression.Lambda(body, param);
        }

        public static bool IsCollection(this Type type)
        {
            if (type.IsArray) return true;
            if (type.IsGenericType)
                return type.GetGenericTypeDefinition() == typeof(List<>) || type.GetGenericTypeDefinition() == typeof(IEnumerable<>);
            return false;
        }

        public static T As<T>(this object value)
        {
            return (T)value;
        }

        /// <summary>
        /// Allows to call the SQL format function
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string FormatSql<T>(this T value, string format)
        {
            return format;
        }

        public static T IsNullSql<T>(this T? value, T nullValue) where T : struct
        {
            return nullValue;
        }

        public static T IsNullSql<T>(this T value, T nullValue)
        {
            return nullValue;
        }

        public static bool EqNullSql<T>(this T value) => true;
        public static bool EqNullSql<T>(this T? value) where T : struct => true;

        public static bool EqNotNullSql<T>(this T value) => false;
        public static bool EqNotNullSql<T>(this T? value) where T : struct => false;

        public static string ConcatSql<T>(this T value, params object[] args)
        {
            return string.Join(' ', args);
        }

        public static int DatePartSql(this DateTime value, DatePart datePart)
        {
            return 0;
        }

        public static int DatePartSql(this DateTime? value, DatePart datePart)
        {
            return 0;
        }

        public static int DatePartSql(this DateTimeOffset value, DatePart datePart)
        {
            return 0;
        }

        public static int DatePartSql(this DateTimeOffset? value, DatePart datePart)
        {
            return 0;
        }

        internal static string SafeValue<T>(this T obj, string specialFunc = null)
        {
            if (specialFunc != null && specialFunc == obj.ToString())
            {
                return obj.ToString();
            }

            if (obj == null)
            {
                return "NULL";
            }
            else if (typeof(T).IsNumeric() || obj.ToString().IsNumeric())
            {
                return obj.ToString();
            }
            else if (typeof(T) == typeof(bool) || bool.TryParse(obj.ToString(), out bool b))
            {
                return bool.Parse(obj.ToString()) ? "1" : "0";
            }
            else
            {
                return $"'{ obj.ToString().Replace("'", "''") }'";
            }
        }

        internal static bool IsNumeric(this Type type)
        {
            if (type == null) { return false; }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                type = type.GetGenericArguments()[0];
            }

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        internal static bool IsNumeric(this string obj)
        {
            return int.TryParse(obj, out var s) ||
                long.TryParse(obj, out var s1) ||
                decimal.TryParse(obj, out var s2) ||
                double.TryParse(obj, out var s3) ||
                float.TryParse(obj, out var s4);
        }
    }
}
