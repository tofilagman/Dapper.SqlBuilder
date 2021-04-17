using Dapper.SqlBuilder.Adapter;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Dapper.SqlBuilder
{
    public interface ISqlBuilder<T> : ISqlBuilder
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
        ISqlBuilder<T2> InnerJoin<T2>(Expression<Func<T, T2, bool>> expression);
        ISqlBuilder<T2> InnerJoin<T2>(Expression<Func<T, T2, bool>> expression, Expression<Func<T2, object>> columns);
        ISqlBuilder<T2> LeftJoin<T2>(Expression<Func<T, T2, bool>> expression);
        ISqlBuilder<T2> LeftJoin<T2>(Expression<Func<T, T2, bool>> expression, Expression<Func<T2, object>> columns);
        ISqlBuilder<T2> LeftOuterJoin<T2>(Expression<Func<T, T2, bool>> expression);
        ISqlBuilder<T2> LeftOuterJoin<T2>(Expression<Func<T, T2, bool>> expression, Expression<Func<T2, object>> columns);
        ISqlBuilder<T2> RightJoin<T2>(Expression<Func<T, T2, bool>> expression);
        ISqlBuilder<T2> RightJoin<T2>(Expression<Func<T, T2, bool>> expression, Expression<Func<T2, object>> columns);
        ISqlBuilder<T2> RightOuterJoin<T2>(Expression<Func<T, T2, bool>> expression);
        ISqlBuilder<T2> RightOuterJoin<T2>(Expression<Func<T, T2, bool>> expression, Expression<Func<T2, object>> columns);
        ISqlBuilder<T2> FullJoin<T2>(Expression<Func<T, T2, bool>> expression);
        ISqlBuilder<T2> FullJoin<T2>(Expression<Func<T, T2, bool>> expression, Expression<Func<T2, object>> columns);
        ISqlBuilder<T2> FullOuterJoin<T2>(Expression<Func<T, T2, bool>> expression);
        ISqlBuilder<T2> FullOuterJoin<T2>(Expression<Func<T, T2, bool>> expression, Expression<Func<T2, object>> columns);
        ISqlBuilder<T2> CrossJoin<T2>(Expression<Func<T, T2, bool>> expression);
        ISqlBuilder<T2> CrossJoin<T2>(Expression<Func<T, T2, bool>> expression, Expression<Func<T2, object>> columns);
        ISqlBuilder<T> GroupBy(Expression<Func<T, object>> expression); 
        ISqlBuilderResult<TResult> ScalarResult<TResult>(Expression<Func<T, dynamic>> expression);
        ISqlBuilderResult<TResult> Result<TResult>(Expression<Func<T, TResult>> expression);
        ISqlBuilderResult<TResult> Result<T2, TResult>(Expression<Func<T, T2, TResult>> expression);
        ISqlBuilderResult<T> Result<T2, T3, TResult>(Expression<Func<T, T2, T3, TResult>> expression);
        ISqlBuilderResult<T> Result<T2, T3, T4, TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression);
        ISqlBuilderResult<T> Result<T2, T3, T4, T5, TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression);
        ISqlBuilderResult<T> Result<T2, T3, T4, T5, T6, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression);
        ISqlBuilderResult<T> Result<T2, T3, T4, T5, T6, T7, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression);
        ISqlBuilderResult<T> Result<T2, T3, T4, T5, T6, T7, T8, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression);
        ISqlBuilderResult<T> Result<T2, T3, T4, T5, T6, T7, T8, T9, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression);
        ISqlBuilderResult<T> Result<T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression);
        ISqlBuilderResult<T> Result<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>> expression);
        ISqlBuilderResult<T> Result<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>> expression);
        ISqlBuilderResult<T> Result<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>> expression);
        ISqlBuilderResult<T> Result<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>> expression);
        ISqlBuilderResult<T> Result<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>> expression);
        ISqlBuilderResult<T> Result<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>> expression);
    }

    public interface ISqlBuilder
    {
        string CommandText { get; }
        IDictionary<string, object> CommandParameters { get; }
        int CurrentParamIndex { get; }
    }

    public interface ISqlBuilderResult<T> : ISqlBuilder
    {

    }

}
