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
    }
}
