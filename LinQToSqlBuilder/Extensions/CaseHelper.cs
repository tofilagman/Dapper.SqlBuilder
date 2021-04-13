using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Dapper.SqlBuilder.Extensions
{
    public class CaseSql<T> : ICaseSql<T>
    {
        private readonly Expression<Func<T, object>> Comparer;
        public CaseSql(Expression<Func<T, object>> comparer)
        {
            Comparer = comparer;
        }

        public ICaseSql<T> Else(Expression<Func<T, object>> returnExpr)
        {
            throw new NotImplementedException();
        }

        public TResult End<TResult>()
        {
            throw new NotImplementedException();
        }

        public ICaseSql<T> When(Expression<Func<T, object>> expression, Expression<Func<T, object>> returnExpr)
        {
            throw new NotImplementedException();
        }
    }

    public interface ICaseSql<T>
    {
        ICaseSql<T> When(Expression<Func<T, object>> expression, Expression<Func<T, object>> returnExpr);  
        ICaseSql<T> Else(Expression<Func<T, object>> returnExpr);
        TResult End<TResult>();
    }
}
