using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Dapper.SqlBuilder.Extensions
{
    public class CaseSql<T> : ICaseSql<T>
    {
        public ICaseSql<T> Else(object value)
        {
            throw new NotImplementedException();
        }

        public TResult End<TResult>()
        {
            throw new NotImplementedException();
        }

        public ICaseSql<T> Then(object value)
        {
            throw new NotImplementedException();
        }

        public ICaseSql<T> When(Expression<Func<T, bool>> expression)
        {
            throw new NotImplementedException();
        }
    }

    public interface ICaseSql<T>
    {
        ICaseSql<T> When(Expression<Func<T, bool>> expression);
        ICaseSql<T> Then(object value);
        ICaseSql<T> Else(object value);
        TResult End<TResult>();
    }
}
