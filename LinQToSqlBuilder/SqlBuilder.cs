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
        public static ISqlBuilder<T> Insert<T>(Expression<Func<T, T>> expression)
        {
            return new SqlBuilder<T>()
            {
                Operation = SqlOperations.Insert
            }.Insert(expression);
        }

        public static ISqlBuilder<T> Insert<T>(T data)
        {
            return Insert<T>(x => data);
        }

        /// <summary>
        /// Prepares an insert command to do the insert operation for many records of specified <typeparamref name="T"/> 
        /// </summary>
        /// <typeparam name="T">The type of entity that associates to the table to insert record(s) to</typeparam>
        /// <param name="expression">The expression that generates the records to insert</param>
        /// <returns>The instance of <see cref="SqlBuilder{T}"/> for chaining calls</returns>
        public static ISqlBuilder<T> InsertMany<T>(Expression<Func<T, IEnumerable<T>>> expression)
        {
            return new SqlBuilder<T>()
            {
                Operation = SqlOperations.Insert
            }.Insert(expression);
        }

        public static ISqlBuilder<T> InsertMany<T>(IEnumerable<T> arr)
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
        public static ISqlBuilder<T> InsertFrom<T, TTo>(Expression<Func<T, TTo>> expression)
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

        public static ISqlBuilder<T> Update<T>(Expression<Func<T, T>> expression)
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
        public static ISqlBuilder<T> Delete<T>(Expression<Func<T, bool>> expression)
        {
            return new SqlBuilder<T>()
            {
                Operation = SqlOperations.Delete
            }.Where(expression);
        }

        public static ISqlBuilder<T> Delete<T>()
        {
            return new SqlBuilder<T>() { Operation = SqlOperations.Delete };
        }

        /// <summary>
        /// Prepares a select query to specified <typeparamref name="T"/> from given expression
        /// </summary>
        /// <typeparam name="T">The type of entity that associates to the table to prepare the query to</typeparam>
        /// <param name="expressions">The expressions that describes which fields of the <typeparamref name="T"/> to return</param>
        /// <returns>The instance of <see cref="SqlBuilder{T}"/> for chaining calls</returns>
        public static ISqlBuilder<T> Select<T>(params Expression<Func<T, object>>[] expressions)
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
        public static ISqlBuilder<T> Select<T, TResult>(Expression<Func<T, TResult>> expression = null)
        {
            return new SqlBuilder<T>().Select(expression);
        }

        public static ISqlBuilder<T> Select<T, TResult>(string @as, Expression<Func<T, TResult>> expression = null)
        {
            return new SqlBuilder<T>().Select(expression);
        }

        public static SqlBuilderCollection From<T>(Func<ISqlBuilder<T>, ISqlBuilder<T>> builder = null)
        {
            return new SqlBuilderCollection().From(builder);
        }

        public static SqlBuilderCollection From<T, TResult>(this SqlBuilderCollection builderCollection, ISqlBuilder<T> builder)
        {
            return builderCollection.Add(builder);
        }

        public static SqlBuilderUnionCollection<THeader> Union<THeader>(Func<ISqlBuilder<THeader>, ISqlBuilder<THeader>> builder = null, bool all = true)
        {
            return new SqlBuilderUnionCollection<THeader>(all).Union(builder);
        }

        public static SqlBuilderUnionCollection<THeader> Union<THeader>(Func<ISqlBuilder<THeader>, ISqlBuilderResult<THeader>> builder = null, bool all = true)
        {
            return new SqlBuilderUnionCollection<THeader>(all).Union(builder);
        }

        public static SqlBuilderUnionCollection<TResult> Union<TResult, THeader>(Func<ISqlBuilder<THeader>, ISqlBuilder<TResult>> builder = null, bool all = true)
        {
            return new SqlBuilderUnionCollection<TResult>(all).Union<THeader>(builder);
        }

        public static SqlBuilderUnionCollection<TResult> Union<TResult, THeader>(Func<ISqlBuilder<THeader>, ISqlBuilderResult<TResult>> builder = null, bool all = true)
        {
            return new SqlBuilderUnionCollection<TResult>(all).Union<THeader>(builder);
        }

        /// <summary>
        /// Prepares a select query to retrieve a single record of specified type <typeparamref name="T"/> satisfies given expression
        /// </summary>
        /// <typeparam name="T">The type of entity that associates to the table to prepare the query to</typeparam>
        /// <param name="expressions">The expression that describes which fields of the <typeparamref name="T"/> to return</param>
        /// <returns>The instance of <see cref="SqlBuilder{T}"/> for chaining calls</returns>
        public static ISqlBuilder<T> SelectSingle<T>(params Expression<Func<T, object>>[] expressions)
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
        public static ISqlBuilder<T> Count<T>(Expression<Func<T, bool>> expression = null)
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
        public static ISqlBuilder<T> Count<T>(Expression<Func<T, object>> countExpression,
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

        public static ISqlBuilder<T> SelectFunction<T>(string functionStatement, Expression<Func<T, object>> expression, params object[] args)
        {
            return new SqlBuilder<T>(functionStatement).SelectFunction(functionStatement, expression, args);
        }
    }

    /// <summary>
    /// Represents the service that will generate SQL commands from given lambda expression
    /// </summary>
    /// <typeparam name="T">The type of entity that associates to the table, used to perform the table and field name resolution</typeparam>
    public partial class SqlBuilder<T> : SqlBuilderBase, ISqlBuilder<T>, ISqlBuilderResult<T>
    {
        public SqlBuilder(string tableName, int paramCount = 0)
        {
            var alias = LambdaResolver.GetTableName<T>();
            if (alias == tableName)
                Builder = new SqlQueryBuilder(tableName, DefaultAdapter, paramCount);
            else
                Builder = new SqlQueryBuilder(tableName, alias, DefaultAdapter, paramCount);

            Resolver = new LambdaResolver(Builder);
        }

        public SqlBuilder(int paramCount = 0) : this(LambdaResolver.GetTableName<T>(), paramCount)
        {
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

        public ISqlBuilder<T> Where(Expression<Func<T, bool>> expression)
        {
            return And(expression);
        }

        public ISqlBuilder<T> And(Expression<Func<T, bool>> expression)
        {
            Builder.And();
            Resolver.ResolveQuery(expression);
            return this;
        }

        public ISqlBuilder<T> AndIsIn(Expression<Func<T, object>> expression, SqlBuilderBase sqlQuery) => WhereIsIn(expression, sqlQuery);

        public ISqlBuilder<T> AndIsIn<T2>(Expression<Func<T, T2>> expression, IEnumerable<T2> values) => WhereIsIn(expression, values);

        public ISqlBuilder<T> Or(Expression<Func<T, bool>> expression)
        {
            Builder.Or();
            Resolver.ResolveQuery(expression);
            return this;
        }

        public ISqlBuilder<T> WhereIsIn(Expression<Func<T, object>> expression, SqlBuilderBase sqlQuery)
        {
            Builder.And();
            Resolver.QueryByIsIn(expression, sqlQuery);
            return this;
        }

        public ISqlBuilder<T> WhereIsIn<T2>(Expression<Func<T, T2>> expression, IEnumerable<T2> values)
        {
            Builder.And();
            Resolver.QueryByIsIn(expression, values);
            return this;
        }

        public ISqlBuilder<T> WhereNotIn(Expression<Func<T, object>> expression, SqlBuilderBase sqlQuery)
        {
            Builder.And();
            Resolver.QueryByNotIn(expression, sqlQuery);
            return this;
        }

        public ISqlBuilder<T> WhereNotIn<T2>(Expression<Func<T, T2>> expression, IEnumerable<T2> values)
        {
            Builder.And();
            Resolver.QueryByNotIn(expression, values);
            return this;
        }

        public ISqlBuilder<T> WhereBetween<T2>(Expression<Func<T, T2>> expression, T2 start, T2 end)
        {
            Builder.And();
            Resolver.QueryBetween(expression, start, end, false);
            return this;
        }

        public ISqlBuilder<T> WhereNotBetween<T2>(Expression<Func<T, T2>> expression, T2 start, T2 end)
        {
            Builder.And();
            Resolver.QueryBetween(expression, start, end, true);
            return this;
        }

        public ISqlBuilder<T> AndBetween<T2>(Expression<Func<T, T2>> expression, T2 start, T2 end) => WhereBetween(expression, start, end);

        public ISqlBuilder<T> AndNotBetween<T2>(Expression<Func<T, T2>> expression, T2 start, T2 end) => WhereNotBetween(expression, start, end);


        public ISqlBuilder<T> OrderBy(Expression<Func<T, object>> expression)
        {
            Resolver.OrderBy(expression);
            return this;
        }

        public ISqlBuilder<T> Take(int pageSize)
        {
            Builder.Take(pageSize);
            return this;
        }

        /// <summary>
        /// Use with <see cref="Take"/>(), to skip specified pages of result
        /// </summary>
        /// <param name="pageIndex">Number of pages to skip</param>
        /// <returns></returns>
        public ISqlBuilder<T> Skip(int pageIndex)
        {
            Builder.SkipPages(pageIndex);
            return this;
        }

        public ISqlBuilder<T> OrderByDescending(Expression<Func<T, object>> expression)
        {
            Resolver.OrderBy(expression, true);
            return this;
        }

        public ISqlBuilder<T> Select(params Expression<Func<T, object>>[] expressions)
        {
            foreach (var expression in expressions)
                Resolver.Select(expression);

            return this;
        }

        public ISqlBuilder<T> Select<TResult>(Expression<Func<T, TResult>> expression)
        {
            Resolver.Select(expression);
            return this;
        }

        public ISqlBuilder<T> Select<T2, TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            Resolver.Select(expression);
            return this;
        }

        public ISqlBuilder<T> Update(Expression<Func<T, T>> expression)
        {
            Resolver.Update(expression);
            return this;
        }

        /// <summary>
        /// Performs insert a new record from the given expression
        /// </summary>
        /// <param name="expression">The expression describes what to insert</param>
        /// <returns></returns>
        public ISqlBuilder<T> Insert(Expression<Func<T, T>> expression)
        {
            Resolver.Insert<T>(expression);
            return this;
        }

        /// <summary>
        /// Append OUTPUT to the insert statement to get the output identity of the inserted record.
        /// </summary>
        public ISqlBuilder<T> OutputIdentity()
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
        public ISqlBuilder<T> Insert(Expression<Func<T, IEnumerable<T>>> expression)
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
        public ISqlBuilder<T> Insert<TTo>(Expression<Func<T, TTo>> expression)
        {
            Builder.InsertTo<TTo>();
            Resolver.Insert<T, TTo>(expression);
            return this;
        }

        public ISqlBuilder<T> SelectCount(Expression<Func<T, object>> expression)
        {
            Resolver.SelectWithFunction(expression, SelectFunctionType.COUNT);
            return this;
        }

        public ISqlBuilder<T> SelectCountAll()
        {
            Resolver.SelectWithFunction<T>(SelectFunctionType.COUNT);
            return this;
        }

        public ISqlBuilder<T> SelectDistinct(Expression<Func<T, object>> expression)
        {
            Resolver.SelectWithFunction(expression, SelectFunctionType.DISTINCT);
            return this;
        }

        public ISqlBuilder<T> SelectSum(Expression<Func<T, object>> expression)
        {
            Resolver.SelectWithFunction(expression, SelectFunctionType.SUM);
            return this;
        }

        public ISqlBuilder<T> SelectMax(Expression<Func<T, object>> expression)
        {
            Resolver.SelectWithFunction(expression, SelectFunctionType.MAX);
            return this;
        }

        public ISqlBuilder<T> SelectMin(Expression<Func<T, object>> expression)
        {
            Resolver.SelectWithFunction(expression, SelectFunctionType.MIN);
            return this;
        }

        public ISqlBuilder<T> SelectAverage(Expression<Func<T, object>> expression)
        {
            Resolver.SelectWithFunction(expression, SelectFunctionType.AVG);
            return this;
        }
         
        public ISqlBuilder<T> GroupBy(Expression<Func<T, object>> expression)
        {
            Resolver.GroupBy(expression);
            return this;
        }

        public ISqlBuilder<T> SelectFunction<TResult>(string functionStatement, Expression<Func<T, TResult>> expression, params object[] args)
        {
            Builder.IgnoreTableBracket();
            Resolver.SelectWithFunction(functionStatement, expression, args);
            return this;
        }
    }

    public class SqlBuilderCollection : ISqlBuilder
    {
        private readonly List<ISqlBuilder> sqlBuilders;

        public SqlBuilderCollection()
        {
            sqlBuilders = new List<ISqlBuilder>();
        }

        public SqlBuilderCollection Add<T>(ISqlBuilder<T> builder)
        {
            sqlBuilders.Add(builder);
            return this;
        }

        public SqlBuilderCollection From<T>(Func<ISqlBuilder<T>, ISqlBuilder<T>> builder = null)
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
                return sqlBuilders.Max(x => x.CurrentParamIndex);
            }
        }

        public int CurrentParamIndex => LastCount;
    }

    public class SqlBuilderUnionCollection<TResult> : ISqlBuilderResult<TResult>
    {
        private readonly List<ISqlBuilder> sqlBuilders;
        private readonly bool All;

        public SqlBuilderUnionCollection(bool all)
        {
            sqlBuilders = new List<ISqlBuilder>();
            All = all;
        }

        public SqlBuilderUnionCollection<TResult> Union<T>(Func<ISqlBuilder<T>, ISqlBuilder<TResult>> builder = null)
        {
            var fg = new SqlBuilder<T>(LastCount);
            var nbd = builder?.Invoke(fg);
            if (nbd == null)
                sqlBuilders.Add(fg);
            else
                sqlBuilders.Add(nbd);
            return this;
        }

        public SqlBuilderUnionCollection<TResult> Union<T>(Func<ISqlBuilder<T>, ISqlBuilderResult<TResult>> builder = null)
        {
            var fg = new SqlBuilder<T>(LastCount);
            var nbd = builder?.Invoke(fg);
            if (nbd == null)
                sqlBuilders.Add(fg);
            else
                sqlBuilders.Add(nbd);
            return this;
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
                return sqlBuilders.Max(x => x.CurrentParamIndex);
            }
        }

        public int CurrentParamIndex => LastCount;
    }

    public class SqlBuilderMultiple<T> : ISqlBuilderResult<T>
    {
        private readonly List<ISqlBuilder<T>> sqlBuilders;
        public SqlBuilderMultiple()
        {
            sqlBuilders = new List<ISqlBuilder<T>>();
        }

        public SqlBuilderMultiple<T> Add(ISqlBuilder<T> builder)
        {
            sqlBuilders.Add(builder);
            return this;
        }

        public ISqlBuilder<T> Insert(Expression<Func<T, T>> expression)
        {
            var builder = new SqlBuilder<T>(LastCount)
            {
                Operation = SqlOperations.Insert
            }.Insert(expression);

            this.Add(builder);

            return builder;
        }

        public ISqlBuilder<T> Update(Expression<Func<T, T>> expression)
        {
            var builder = new SqlBuilder<T>(LastCount)
            {
                Operation = SqlOperations.Update
            }
               .Update(expression);

            this.Add(builder);

            return builder;
        }

        public ISqlBuilder<T> Delete(Expression<Func<T, bool>> expression)
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
                return sqlBuilders.Max(x => x.CurrentParamIndex);
            }
        }

        public int CurrentParamIndex => LastCount;
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

    class SqlJoinBuilder<T1, T2, T3> : SqlBuilderBase
    {
        public SqlJoinBuilder(int paramCount = 0)
        {
            Builder = new SqlQueryBuilder(LambdaResolver.GetTableName<T2>(), DefaultAdapter, paramCount);
            Resolver = new LambdaResolver(Builder);
        }

        public SqlJoinBuilder<T1, T2, T3> Build(Expression<Func<T1, T2, T3, bool>> expression)
        {
            Resolver.ResolveQuery(expression);
            return this;
        }
    }
}
