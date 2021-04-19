using Dapper.SqlBuilder.Adapter;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Dapper.SqlBuilder
{
    public partial interface ISqlBuilder<T> : ISqlBuilder
    {
        ISqlBuilder<T> Where(Expression<Func<T, bool>> expression);
        ISqlBuilder<T> And(Expression<Func<T, bool>> expression);
        ISqlBuilder<T> AndIsIn(Expression<Func<T, object>> expression, SqlBuilderBase sqlQuery);
        ISqlBuilder<T> AndIsIn<T2>(Expression<Func<T, T2>> expression, IEnumerable<T2> values);
        ISqlBuilder<T> Or(Expression<Func<T, bool>> expression);
        ISqlBuilder<T> WhereIsIn(Expression<Func<T, object>> expression, SqlBuilderBase sqlQuery);
        ISqlBuilder<T> WhereIsIn<T2>(Expression<Func<T, T2>> expression, IEnumerable<T2> values);
        ISqlBuilder<T> WhereNotIn(Expression<Func<T, object>> expression, SqlBuilderBase sqlQuery);
        ISqlBuilder<T> WhereNotIn<T2>(Expression<Func<T, T2>> expression, IEnumerable<T2> values);
        ISqlBuilder<T> WhereBetween<T2>(Expression<Func<T, T2>> expression, T2 start, T2 end);
        ISqlBuilder<T> WhereNotBetween<T2>(Expression<Func<T, T2>> expression, T2 start, T2 end);
        ISqlBuilder<T> AndBetween<T2>(Expression<Func<T, T2>> expression, T2 start, T2 end);
        ISqlBuilder<T> AndNotBetween<T2>(Expression<Func<T, T2>> expression, T2 start, T2 end);
        ISqlBuilder<T> OrderBy(Expression<Func<T, object>> expression);
        ISqlBuilder<T> Take(int pageSize);
        ISqlBuilder<T> Skip(int pageIndex);
        ISqlBuilder<T> OrderByDescending(Expression<Func<T, object>> expression);
        ISqlBuilder<T> Select(params Expression<Func<T, object>>[] expressions);
        ISqlBuilder<T> Select<TResult>(Expression<Func<T, TResult>> expression);
        ISqlBuilder<T> Update(Expression<Func<T, T>> expression);
        ISqlBuilder<T> Insert(Expression<Func<T, T>> expression);
        ISqlBuilder<T> OutputIdentity();
        ISqlBuilder<T> Insert(Expression<Func<T, IEnumerable<T>>> expression);
        ISqlBuilder<T> Insert<TTo>(Expression<Func<T, TTo>> expression);
        ISqlBuilder<T> SelectCount(Expression<Func<T, object>> expression);
        ISqlBuilder<T> SelectCountAll();
        ISqlBuilder<T> SelectDistinct(Expression<Func<T, object>> expression);
        ISqlBuilder<T> SelectSum(Expression<Func<T, object>> expression);
        ISqlBuilder<T> SelectMax(Expression<Func<T, object>> expression);
        ISqlBuilder<T> SelectMin(Expression<Func<T, object>> expression);
        ISqlBuilder<T> SelectAverage(Expression<Func<T, object>> expression); 
        ISqlBuilder<T> GroupBy(Expression<Func<T, object>> expression);  
    }

    public interface ISqlBuilder
    {
        string CommandText { get; }
        IDictionary<string, object> CommandParameters { get; }
        int CurrentParamIndex { get; }
        List<string> TableNames { get; }
    }

    public interface ISqlBuilderResult<T> : ISqlBuilder
    {

    }

}
