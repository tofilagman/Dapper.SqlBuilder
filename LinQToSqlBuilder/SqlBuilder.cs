using System;
using System.Collections.Generic;
using System.Linq;
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
        /// Prepares an insert command to do the insert operation for one record of specified <typeparamref name="T"/> 
        /// </summary>
        /// <typeparam name="T">The type of entity that associates to the table to insert record(s) to</typeparam>
        /// <param name="expression">The expression that generates the record to insert</param>
        /// <returns>The instance of <see cref="SqlBuilder{T}"/> for chaining calls</returns>
        public static SqlBuilder<T> Insert<T>(Expression<Func<T, T>> expression)
        {
            return new SqlBuilder<T>()
            {
                Operation = SqlOperations.Insert
            }.Insert(expression);
        }

        public static SqlBuilder<T> Insert<T>(T data)
        {
            return Insert<T>(x => data);
        }

        /// <summary>
        /// Prepares an insert command to do the insert operation for many records of specified <typeparamref name="T"/> 
        /// </summary>
        /// <typeparam name="T">The type of entity that associates to the table to insert record(s) to</typeparam>
        /// <param name="expression">The expression that generates the records to insert</param>
        /// <returns>The instance of <see cref="SqlBuilder{T}"/> for chaining calls</returns>
        public static SqlBuilder<T> InsertMany<T>(Expression<Func<T, IEnumerable<T>>> expression)
        {
            return new SqlBuilder<T>()
            {
                Operation = SqlOperations.Insert
            }.Insert(expression);
        }

        public static SqlBuilder<T> InsertMany<T>(IEnumerable<T> arr)
        {
            return InsertMany<T>(x => arr);
        }

        /// <summary>
        /// Prepares an insert command to copy record(s) from specific <typeparamref name="T"/> table to the <typeparamref name="TTo"/>  destination table
        /// </summary>
        /// <typeparam name="T">The type of entity that associates to the source table to copy record(s) from</typeparam>
        /// <typeparam name="TTo">The type of entity that associates to the destination table to copy record(s) to</typeparam>
        /// <param name="expression">The expression describes how to form the destination record</param>
        /// <returns>The instance of <see cref="SqlBuilder{T}"/> for chaining calls</returns>
        public static SqlBuilder<T> InsertFrom<T, TTo>(Expression<Func<T, TTo>> expression)
        {
            return new SqlBuilder<T>()
            {
                Operation = SqlOperations.InsertFrom
            }.Insert(expression);
        }

        /// <summary>
        /// Prepares an update command to specified <typeparamref name="T"/> 
        /// </summary>
        /// <typeparam name="T">The type of entity that associates to the table to performs the update</typeparam>
        /// <param name="expression">The expression that describes how to update the record</param>
        /// <returns>The instance of <see cref="SqlBuilder{T}"/> for chaining calls</returns>

        public static SqlBuilder<T> Update<T>(Expression<Func<T, T>> expression)
        {
            return new SqlBuilder<T>()
            {
                Operation = SqlOperations.Update
            }
               .Update(expression);
        }

        public static SqlBuilderMultiple<T> Many<T>()
        {
            return new SqlBuilderMultiple<T>();
        }

        /// <summary>
        /// Prepares a delete command to specified <typeparamref name="T"/> 
        /// </summary>
        /// <typeparam name="T">The type of entity that associates to the table to performs the deletion</typeparam>
        /// <param name="expression">The expression that filters the records to be deleted</param>
        /// <returns>The instance of <see cref="SqlBuilder{T}"/> for chaining calls</returns>
        public static SqlBuilder<T> Delete<T>(Expression<Func<T, bool>> expression)
        {
            return new SqlBuilder<T>()
            {
                Operation = SqlOperations.Delete
            }.Where(expression);
        }

        public static SqlBuilder<T> Delete<T>()
        {
            return new SqlBuilder<T>() { Operation = SqlOperations.Delete };
        }

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
        public static SqlBuilder<T> Select<T, TResult>(Expression<Func<T, TResult>> expression = null)
        {
            return new SqlBuilder<T>().Select(expression);
        }

        public static SqlBuilder<T> Select<T, TResult>(string @as, Expression<Func<T, TResult>> expression = null)
        {
            return new SqlBuilder<T>().Select(expression);
        }

        public static SqlBuilderCollection From<T>(Func<SqlBuilder<T>, SqlBuilder<T>> builder = null)
        {
            return new SqlBuilderCollection().From(builder);
        }

        public static SqlBuilderCollection From<T, TResult>(this SqlBuilderCollection builderCollection, SqlBuilder<T> builder)
        {
            return builderCollection.Add(builder);
        }

        public static SqlBuilderUnionCollection Union<T>(Func<SqlBuilder<T>, SqlBuilder<T>> builder = null, bool all = true)
        {
            return new SqlBuilderUnionCollection(all).Union(builder);
        }

        public static SqlBuilderUnionCollection Union<T, TResult>(this SqlBuilderUnionCollection builderCollection, SqlBuilder<T> builder)
        {
            return builderCollection.Add(builder);
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
    public partial class SqlBuilder<T> : SqlBuilderBase, ISqlBuilder<T>
    {
        public SqlBuilder(int paramCount = 0)
        {
            Builder = new SqlQueryBuilder(LambdaResolver.GetTableName<T>(), DefaultAdapter, paramCount);
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

        public SqlBuilder<T> AndIsIn(Expression<Func<T, object>> expression, SqlBuilderBase sqlQuery) => WhereIsIn(expression, sqlQuery);

        public SqlBuilder<T> AndIsIn<T2>(Expression<Func<T, T2>> expression, IEnumerable<T2> values) => WhereIsIn(expression, values);

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

        public SqlBuilder<T> WhereIsIn<T2>(Expression<Func<T, T2>> expression, IEnumerable<T2> values)
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

        public SqlBuilder<T> WhereNotIn<T2>(Expression<Func<T, T2>> expression, IEnumerable<T2> values)
        {
            Builder.And();
            Resolver.QueryByNotIn(expression, values);
            return this;
        }

        public SqlBuilder<T> WhereBetween<T2>(Expression<Func<T, T2>> expression, T2 start, T2 end)
        {
            Builder.And();
            Resolver.QueryBetween(expression, start, end, false);
            return this;
        }

        public SqlBuilder<T> WhereNotBetween<T2>(Expression<Func<T, T2>> expression, T2 start, T2 end)
        {
            Builder.And();
            Resolver.QueryBetween(expression, start, end, true);
            return this;
        }

        public SqlBuilder<T> AndBetween<T2>(Expression<Func<T, T2>> expression, T2 start, T2 end) => WhereBetween(expression, start, end);

        public SqlBuilder<T> AndNotBetween<T2>(Expression<Func<T, T2>> expression, T2 start, T2 end) => WhereNotBetween(expression, start, end);


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

        public SqlBuilder<T> Update(Expression<Func<T, T>> expression)
        {
            Resolver.Update(expression);
            return this;
        }

        /// <summary>
        /// Performs insert a new record from the given expression
        /// </summary>
        /// <param name="expression">The expression describes what to insert</param>
        /// <returns></returns>
        public SqlBuilder<T> Insert(Expression<Func<T, T>> expression)
        {
            Resolver.Insert<T>(expression);
            return this;
        }

        /// <summary>
        /// Append OUTPUT to the insert statement to get the output identity of the inserted record.
        /// </summary>
        public SqlBuilder<T> OutputIdentity()
        {
            if (Builder.Operation != SqlOperations.Insert)
                throw new InvalidOperationException($"Cannot OUTPUT identity for the SQL statement that is not insert");

            Resolver.OutputInsertIdentity<T>();
            return this;
        }

        /// <summary>
        /// Performs insert many records from the given expression
        /// </summary>
        /// <param name="expression">The expression describes the entities to insert</param>
        /// <returns></returns>
        public SqlBuilder<T> Insert(Expression<Func<T, IEnumerable<T>>> expression)
        {
            Resolver.Insert(expression);
            return this;
        }

        /// <summary>
        /// Performs insert to <see cref="TTo"/> table using the values copied from the given expression
        /// </summary>
        /// <typeparam name="TTo">The destination table</typeparam>
        /// <param name="expression">The expression describes how to copy values from original table <see cref="T"/></param>
        /// <returns></returns>
        public SqlBuilder<T> Insert<TTo>(Expression<Func<T, TTo>> expression)
        {
            Builder.InsertTo<TTo>();
            Resolver.Insert<T, TTo>(expression);
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

        //[Obsolete("Bug")]
        //public SqlBuilder<TResult> Join<T2, TKey, TResult>(SqlBuilder<T2> joinQuery,
        //    Expression<Func<T, TKey>> primaryKeySelector,
        //    Expression<Func<T, TKey>> foreignKeySelector,
        //    Func<T, T2, TResult> selection)
        //{
        //    var query = new SqlBuilder<TResult>(Builder, Resolver);
        //    Resolver.Join<T, T2, TKey>(primaryKeySelector, foreignKeySelector);
        //    return query;
        //}

        private SqlBuilder<T2> Join<T2>(Expression<Func<T, T2, bool>> expression, JoinType joinType)
        {
            var joinQuery = new SqlBuilder<T2>(Builder, Resolver);
            Resolver.Join(expression, joinType);
            return joinQuery;
        }

        private SqlBuilder<T2> Join<T2>(Expression<Func<T, T2, bool>> expression, Expression<Func<T2, object>> columns, JoinType joinType)
        {
            var joinQuery = new SqlBuilder<T2>(Builder, Resolver).Select(columns);
            Resolver.Join(expression, joinType);
            return joinQuery;
        }

        public SqlBuilder<T2> InnerJoin<T2>(Expression<Func<T, T2, bool>> expression) => Join(expression, JoinType.InnerJoin);
        public SqlBuilder<T2> InnerJoin<T2>(Expression<Func<T, T2, bool>> expression, Expression<Func<T2, object>> columns) => Join(expression, columns, JoinType.InnerJoin);
        public SqlBuilder<T2> LeftJoin<T2>(Expression<Func<T, T2, bool>> expression) => Join(expression, JoinType.LeftJoin);
        public SqlBuilder<T2> LeftJoin<T2>(Expression<Func<T, T2, bool>> expression, Expression<Func<T2, object>> columns) => Join(expression, columns, JoinType.LeftJoin);
        public SqlBuilder<T2> LeftOuterJoin<T2>(Expression<Func<T, T2, bool>> expression) => Join(expression, JoinType.LeftOuterJoin);
        public SqlBuilder<T2> LeftOuterJoin<T2>(Expression<Func<T, T2, bool>> expression, Expression<Func<T2, object>> columns) => Join(expression, columns, JoinType.LeftOuterJoin);
        public SqlBuilder<T2> RightJoin<T2>(Expression<Func<T, T2, bool>> expression) => Join(expression, JoinType.RightJoin);
        public SqlBuilder<T2> RightJoin<T2>(Expression<Func<T, T2, bool>> expression, Expression<Func<T2, object>> columns) => Join(expression, columns, JoinType.RightJoin);
        public SqlBuilder<T2> RightOuterJoin<T2>(Expression<Func<T, T2, bool>> expression) => Join(expression, JoinType.RightOuterJoin);
        public SqlBuilder<T2> RightOuterJoin<T2>(Expression<Func<T, T2, bool>> expression, Expression<Func<T2, object>> columns) => Join(expression, columns, JoinType.RightOuterJoin);
        public SqlBuilder<T2> FullJoin<T2>(Expression<Func<T, T2, bool>> expression) => Join(expression, JoinType.FullJoin);
        public SqlBuilder<T2> FullJoin<T2>(Expression<Func<T, T2, bool>> expression, Expression<Func<T2, object>> columns) => Join(expression, columns, JoinType.FullJoin);
        public SqlBuilder<T2> FullOuterJoin<T2>(Expression<Func<T, T2, bool>> expression) => Join(expression, JoinType.FullOuterJoin);
        public SqlBuilder<T2> FullOuterJoin<T2>(Expression<Func<T, T2, bool>> expression, Expression<Func<T2, object>> columns) => Join(expression, columns, JoinType.FullOuterJoin);
        public SqlBuilder<T2> CrossJoin<T2>(Expression<Func<T, T2, bool>> expression) => Join(expression, JoinType.CrossJoin);
        public SqlBuilder<T2> CrossJoin<T2>(Expression<Func<T, T2, bool>> expression, Expression<Func<T2, object>> columns) => Join(expression, columns, JoinType.CrossJoin);

        public SqlBuilder<T> GroupBy(Expression<Func<T, object>> expression)
        {
            Resolver.GroupBy(expression);
            return this;
        }
    }

    public interface ISqlBuilder<T> : ISqlBuilder
    {

    }

    public interface ISqlBuilder
    {
        string CommandText { get; }
        IDictionary<string, object> CommandParameters { get; }
    }

    public class SqlBuilderCollection : ISqlBuilder
    {
        private readonly List<SqlBuilderBase> sqlBuilders;

        public SqlBuilderCollection()
        {
            sqlBuilders = new List<SqlBuilderBase>();
        }

        public SqlBuilderCollection Add<T>(SqlBuilder<T> builder)
        {
            sqlBuilders.Add(builder);
            return this;
        }

        public SqlBuilderCollection From<T>(Func<SqlBuilder<T>, SqlBuilder<T>> builder = null)
        {
            var fg = new SqlBuilder<T>(LastCount);
            var nbd = builder?.Invoke(fg);
            return this.Add(nbd ?? fg);
        }

        public string CommandText
        {
            get
            {
                return string.Join("\r\n", sqlBuilders.Select(x => x.CommandText));
            }
        }

        public IDictionary<string, object> CommandParameters
        {
            get
            {
                return sqlBuilders.SelectMany(x => x.CommandParameters).ToDictionary(x => x.Key, x => x.Value);
            }
        }

        private int LastCount
        {
            get
            {
                if (sqlBuilders.Count == 0)
                    return 0;
                return sqlBuilders.Max(x => x.SqlBuilder.CurrentParamIndex);
            }
        }
    }

    public class SqlBuilderUnionCollection : ISqlBuilder
    {
        private readonly List<SqlBuilderBase> sqlBuilders;
        private readonly bool All;

        public SqlBuilderUnionCollection(bool all)
        {
            sqlBuilders = new List<SqlBuilderBase>();
            All = all;
        }

        public SqlBuilderUnionCollection Add<T>(SqlBuilder<T> builder)
        {
            sqlBuilders.Add(builder);
            return this;
        }

        public SqlBuilderUnionCollection Union<T>(Func<SqlBuilder<T>, SqlBuilder<T>> builder = null)
        {
            var fg = new SqlBuilder<T>(LastCount);
            var nbd = builder?.Invoke(fg);
            return this.Add(nbd ?? fg);
        }

        public string CommandText
        {
            get
            {
                var sAll = All ? "ALL" : "";
                return string.Join($"\r\nUNION { sAll }\r\n", sqlBuilders.Select(x => x.CommandText));
            }
        }

        public IDictionary<string, object> CommandParameters
        {
            get
            {
                return sqlBuilders.SelectMany(x => x.CommandParameters).ToDictionary(x => x.Key, x => x.Value);
            }
        }

        private int LastCount
        {
            get
            {
                if (sqlBuilders.Count == 0)
                    return 0;
                return sqlBuilders.Max(x => x.SqlBuilder.CurrentParamIndex);
            }
        }
    }

    public class SqlBuilderMultiple<T> : ISqlBuilder<T>
    {
        private readonly List<SqlBuilderBase> sqlBuilders;
        public SqlBuilderMultiple()
        {
            sqlBuilders = new List<SqlBuilderBase>();
        }

        public SqlBuilderMultiple<T> Add(SqlBuilder<T> builder)
        {
            sqlBuilders.Add(builder);
            return this;
        }

        public SqlBuilder<T> Insert(Expression<Func<T, T>> expression)
        {
            var builder = new SqlBuilder<T>(LastCount)
            {
                Operation = SqlOperations.Insert
            }.Insert(expression);

            this.Add(builder);

            return builder;
        }

        public SqlBuilder<T> Update(Expression<Func<T, T>> expression)
        {
            var builder = new SqlBuilder<T>(LastCount)
            {
                Operation = SqlOperations.Update
            }
               .Update(expression);

            this.Add(builder);

            return builder;
        }

        public SqlBuilder<T> Delete(Expression<Func<T, bool>> expression)
        {
            var builder = new SqlBuilder<T>(LastCount)
            {
                Operation = SqlOperations.Delete
            }.Where(expression);

            this.Add(builder);

            return builder;
        }


        public string CommandText
        {
            get
            {
                return string.Join("\r\n", sqlBuilders.Select(x => x.CommandText));
            }
        }

        public IDictionary<string, object> CommandParameters
        {
            get
            {
                return sqlBuilders.SelectMany(x => x.CommandParameters).ToDictionary(x => x.Key, x => x.Value);
            }
        }

        private int LastCount
        {
            get
            {
                if (sqlBuilders.Count == 0)
                    return 0;
                return sqlBuilders.Max(x => x.SqlBuilder.CurrentParamIndex);
            }
        }
    }

    class SqlJoinBuilder<T1, T2> : SqlBuilderBase
    {
        public SqlJoinBuilder(int paramCount = 0)
        {
            Builder = new SqlQueryBuilder(LambdaResolver.GetTableName<T2>(), DefaultAdapter, paramCount);
            Resolver = new LambdaResolver(Builder);
        }

        public SqlJoinBuilder<T1, T2> Build(Expression<Func<T1, T2, bool>> expression)
        {
            Resolver.ResolveQuery(expression);
            return this;
        }
    }
}
