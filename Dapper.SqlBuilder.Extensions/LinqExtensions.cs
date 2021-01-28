using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using z.Data;

namespace Dapper.SqlBuilder.Extensions
{
    public static class LinqExtensions
    {
        public static T CopyTo<R, T>(this R source, T data)
        {
            var h = data;
            var df = source.GetType().GetProperties();
            var hd = typeof(T).GetProperties();
            foreach (var d in df)
            {
                var ss = hd.SingleOrDefault(x => x.Name == d.Name);
                if (ss != null)
                { 
                    h.SetObjectProperty(d.Name, source.GetObjectProperty(d.Name));
                }
            }
            return h;
        }

        public static async Task<T> CopyToAsync<R, T>(this R data, T func)
        {
            return await Task.FromResult(data.CopyTo(func));
        }

        public static async IAsyncEnumerable<T> CopyListTo<T, R>(this IEnumerable<R> data, T func)
        {
            foreach (var q in data)
                yield return await q.CopyToAsync(func);
        }

        public static string GetTableName(this Type obj)
        {
            return obj.GetAttributeValue((TableAttribute table) => table.Name);
        }

        public static TValue GetAttributeValue<TAttribute, TValue>(this Type type, Func<TAttribute, TValue> valueSelector) where TAttribute : Attribute
        {
            var att = type.GetCustomAttributes(typeof(TAttribute), true).FirstOrDefault() as TAttribute;
            if (att != null)
            {
                return valueSelector(att);
            }

            throw new InvalidOperationException($"Attribute {nameof(TAttribute)} is not defined in object: { nameof(type) }");
        }
    }
}
