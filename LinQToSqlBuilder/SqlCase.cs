using Dapper.SqlBuilder.Builder;
using Dapper.SqlBuilder.Extensions;
using Dapper.SqlBuilder.Resolver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Dapper.SqlBuilder
{
    public static class SqlCase
    {
        public static ISqlCase<T> Case<T>(Expression<Func<T, object>> expression)
        {
            return new SqlCase<T>().Case(expression);
        }

        public static ISqlCase<T> Case<T>(Expression<Func<T, object>> expression, Expression<Func<T, object>> returnExpr)
        {
            return new SqlCase<T>().Case(expression, returnExpr);
        }


        public static T Case<T>(this T value, Expression<Func<T, ISqlCase<T>>> caseExpression)
        {
            return value;
        }

        public static T Case<T, THeader>(this T value, Expression<Func<T, ISqlCase<THeader>>> caseExpression)
        {
            return value;
        }
    }

    public class SqlCase<THeader> : SqlBuilderBase, ISqlCase<THeader>
    {
        public SqlCase()
        {
            Builder = new SqlQueryBuilder(LambdaResolver.GetTableName<THeader>(), DefaultAdapter, 0)
            {
                Operation = Adapter.SqlOperations.Case,
                ParameterPrefix = $"Case{ Guid.NewGuid().ToString("N").Substring(0, 5) }"
            };
            Resolver = new LambdaResolver(Builder);
        }

        public ISqlCase<THeader> Case(Expression<Func<THeader, object>> expression)
        {
            Builder.Case();
            Resolver.Case(expression);
            return this;
        }

        public ISqlCase<THeader> Case(Expression<Func<THeader, object>> expression, Expression<Func<THeader, object>> returnExpr)
        {
            Builder.Case();
            return When(expression, returnExpr);
        }

        public ISqlCase<THeader> Else(Expression<Func<THeader, object>> returnExpr)
        {
            Builder.Else();
            Resolver.Case(returnExpr);
            return this;
        }

        public ISqlCase<THeader> Else<T>(Expression<Func<THeader, T, object>> returnExpr)
        {
            Builder.Else();
            Resolver.Case(returnExpr);
            return this;
        }

        public ISqlCase<THeader> End()
        {
            Builder.End();
            return this;
        }

        public ISqlCase<THeader> When(Expression<Func<THeader, object>> expression, Expression<Func<THeader, object>> returnExpr)
        {
            Builder.When();
            Resolver.Case(expression);
            Builder.Then();
            Resolver.Case(returnExpr);
            return this;
        }

        public ISqlCase<THeader> When<T>(Expression<Func<THeader, T, object>> expression, Expression<Func<THeader, T, object>> returnExpr)
        {
            Builder.When();
            Resolver.Case(expression);
            Builder.Then();
            Resolver.Case(returnExpr);
            return this;
        }
    }

    public interface ISqlCase<THeader> : ISqlBuilderResult<THeader>
    {
        ISqlCase<THeader> Case(Expression<Func<THeader, object>> expression);
        ISqlCase<THeader> When(Expression<Func<THeader, object>> expression, Expression<Func<THeader, object>> returnExpr);
        ISqlCase<THeader> When<T>(Expression<Func<THeader, T, object>> expression, Expression<Func<THeader, T, object>> returnExpr);
        ISqlCase<THeader> Else(Expression<Func<THeader, object>> returnExpr);
        ISqlCase<THeader> Else<T>(Expression<Func<THeader, T, object>> returnExpr);
        ISqlCase<THeader> End();
    }

}
