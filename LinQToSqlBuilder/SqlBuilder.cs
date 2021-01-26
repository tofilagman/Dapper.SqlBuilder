using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Dapper.SqlBuilder.Adapter;
using Dapper.SqlBuilder.Builder;
using Dapper.SqlBuilder.Resolver;
using Dapper.SqlBuilder.ValueObjects;

namespace Dapper.SqlBuilder
{
    public static class SqlBuilder
    {

        /// <summary>
        /// Prepares a select query to specified <typeparamref name="T"/> from given expression
        /// </summary>
        /// <typeparam name="T">The type of entity that associates to the table to prepare the query to</typeparam>
        /// <param name="expressions">The expressions that describes which fields of the <typeparamref name="T"/> to return</param>
        /// <returns>The instance of <see cref="SqlBuilder{T}"/> for chaining calls</returns>
        public static SqlBuilder<T> Select<T>(params Expression<Func<T, object>>[] expressions)
        {
            return new SqlBuilder<T>().Select(expressions);
        }

        /// <summary>
        /// Prepares a select query to specified <typeparamref name="T"/> from given expression
        /// </summary>
        /// <typeparam name="T">The type of entity that associates to the table to prepare the query to</typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="expression">The expression that describes which fields of the <typeparamref name="T"/> to return</param>
        /// <returns>The instance of <see cref="SqlBuilder{T}"/> for chaining calls</returns>
        public static SqlBuilder<T> Select<T, TResult>(Expression<Func<T, TResult>> expression)
        {
            return new SqlBuilder<T>().Select(expression);
        }

        /// <summary>
        /// Prepares a select query to retrieve a single record of specified type <typeparamref name="T"/> satisfies given expression
        /// </summary>
        /// <typeparam name="T">The type of entity that associates to the table to prepare the query to</typeparam>
        /// <param name="expressions">The expression that describes which fields of the <typeparamref name="T"/> to return</param>
        /// <returns>The instance of <see cref="SqlBuilder{T}"/> for chaining calls</returns>
        public static SqlBuilder<T> SelectSingle<T>(params Expression<Func<T, object>>[] expressions)
        {
            return new SqlBuilder<T>().Select(expressions)
                                      .Take(1);
        }

        /// <summary>
        /// Prepares a select count query to specified <typeparamref name="T"/> from given expression
        /// </summary>
        /// <typeparam name="T">The type of entity that associates to the table to prepare the query to</typeparam>
        /// <param name="expression">The expression that describe how to filter the results</param>
        /// <returns>The instance of <see cref="SqlBuilder{T}"/> for chaining calls</returns>
        public static SqlBuilder<T> Count<T>(Expression<Func<T, bool>> expression = null)
        {
            return Count<T>(null, expression);
        }

        /// <summary>
        /// Prepares a select count query to specified <typeparamref name="T"/> from given expression
        /// </summary>
        /// <typeparam name="T">The type of entity that associates to the table to prepare the query to</typeparam>
        /// <param name="countExpression">The expression that describe how to pick the fields for counting</param>
        /// <param name="expression">The expression that describe how to filter the results</param>
        /// <returns>The instance of <see cref="SqlBuilder{T}"/> for chaining calls</returns>
        public static SqlBuilder<T> Count<T>(Expression<Func<T, object>> countExpression,
                                             Expression<Func<T, bool>> expression = null)
        {
            var sqlBuilder = new SqlBuilder<T>();

            if (countExpression != null)
            {
                sqlBuilder.SelectCount(countExpression);
            }
            else
            {
                sqlBuilder.SelectCountAll();
            }

            if (expression != null)
            {
                sqlBuilder.Where(expression);
            }

            return sqlBuilder;
        }

        public static void SetAdapter(ISqlAdapter adapter)
        {
            SqlBuilderBase.SetAdapter(adapter);
        }
    }

    /// <summary>
    /// Represents the service that will generate SQL commands from given lambda expression
    /// </summary>
    /// <typeparam name="T">The type of entity that associates to the table, used to perform the table and field name resolution</typeparam>
    public class SqlBuilder<T> : SqlBuilderBase
    {
        public SqlBuilder()
        {
            Builder = new SqlQueryBuilder(LambdaResolver.GetTableName<T>(), DefaultAdapter);
            Resolver = new LambdaResolver(Builder);
        }

        public SqlBuilder(Expression<Func<T, bool>> expression) : this()
        {
            Where(expression);
        }

        internal SqlBuilder(SqlQueryBuilder builder, LambdaResolver resolver)
        {
            Builder = builder;
            Resolver = resolver;
        }

        public SqlBuilder<T> Where(Expression<Func<T, bool>> expression)
        {
            return And(expression);
        }

        public SqlBuilder<T> And(Expression<Func<T, bool>> expression)
        {
            Builder.And();
            Resolver.ResolveQuery(expression);
            return this;
        }

        public SqlBuilder<T> Or(Expression<Func<T, bool>> expression)
        {
            Builder.Or();
            Resolver.ResolveQuery(expression);
            return this;
        }

        public SqlBuilder<T> WhereIsIn(Expression<Func<T, object>> expression, SqlBuilderBase sqlQuery)
        {
            Builder.And();
            Resolver.QueryByIsIn(expression, sqlQuery);
            return this;
        }

        public SqlBuilder<T> WhereIsIn(Expression<Func<T, object>> expression, IEnumerable<object> values)
        {
            Builder.And();
            Resolver.QueryByIsIn(expression, values);
            return this;
        }

        public SqlBuilder<T> WhereNotIn(Expression<Func<T, object>> expression, SqlBuilderBase sqlQuery)
        {
            Builder.And();
            Resolver.QueryByNotIn(expression, sqlQuery);
            return this;
        }

        public SqlBuilder<T> WhereNotIn(Expression<Func<T, object>> expression, IEnumerable<object> values)
        {
            Builder.And();
            Resolver.QueryByNotIn(expression, values);
            return this;
        }

        public SqlBuilder<T> OrderBy(Expression<Func<T, object>> expression)
        {
            Resolver.OrderBy(expression);
            return this;
        }

        public SqlBuilder<T> Take(int pageSize)
        {
            Builder.Take(pageSize);
            return this;
        }

        /// <summary>
        /// Use with <see cref="Take"/>(), to skip specified pages of result
        /// </summary>
        /// <param name="pageIndex">Number of pages to skip</param>
        /// <returns></returns>
        public SqlBuilder<T> Skip(int pageIndex)
        {
            Builder.SkipPages(pageIndex);
            return this;
        }

        public SqlBuilder<T> OrderByDescending(Expression<Func<T, object>> expression)
        {
            Resolver.OrderBy(expression, true);
            return this;
        }

        public SqlBuilder<T> Select(params Expression<Func<T, object>>[] expressions)
        {
            foreach (var expression in expressions)
                Resolver.Select(expression);

            return this;
        }

        public SqlBuilder<T> Select<TResult>(Expression<Func<T, TResult>> expression)
        {
            Resolver.Select(expression);

            return this;
        }

        public SqlBuilder<T> SelectCount(Expression<Func<T, object>> expression)
        {
            Resolver.SelectWithFunction(expression, SelectFunction.COUNT);
            return this;
        }

        public SqlBuilder<T> SelectCountAll()
        {
            Resolver.SelectWithFunction<T>(SelectFunction.COUNT);
            return this;
        }

        public SqlBuilder<T> SelectDistinct(Expression<Func<T, object>> expression)
        {
            Resolver.SelectWithFunction(expression, SelectFunction.DISTINCT);
            return this;
        }

        public SqlBuilder<T> SelectSum(Expression<Func<T, object>> expression)
        {
            Resolver.SelectWithFunction(expression, SelectFunction.SUM);
            return this;
        }

        public SqlBuilder<T> SelectMax(Expression<Func<T, object>> expression)
        {
            Resolver.SelectWithFunction(expression, SelectFunction.MAX);
            return this;
        }

        public SqlBuilder<T> SelectMin(Expression<Func<T, object>> expression)
        {
            Resolver.SelectWithFunction(expression, SelectFunction.MIN);
            return this;
        }

        public SqlBuilder<T> SelectAverage(Expression<Func<T, object>> expression)
        {
            Resolver.SelectWithFunction(expression, SelectFunction.AVG);
            return this;
        }

        public SqlBuilder<TResult> Join<T2, TKey, TResult>(SqlBuilder<T2> joinQuery,
            Expression<Func<T, TKey>> primaryKeySelector,
            Expression<Func<T, TKey>> foreignKeySelector,
            Func<T, T2, TResult> selection)
        {
            var query = new SqlBuilder<TResult>(Builder, Resolver);
            Resolver.Join<T, T2, TKey>(primaryKeySelector, foreignKeySelector);
            return query;
        }

        public SqlBuilder<T2> Join<T2>(Expression<Func<T, T2, bool>> expression)
        {
            var joinQuery = new SqlBuilder<T2>(Builder, Resolver);
            Resolver.Join(expression);
            return joinQuery;
        }

        public SqlBuilder<T> GroupBy(Expression<Func<T, object>> expression)
        {
            Resolver.GroupBy(expression);
            return this;
        }
    }
}
