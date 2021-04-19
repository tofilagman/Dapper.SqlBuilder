using Dapper.SqlBuilder.Builder;
using Dapper.SqlBuilder.Resolver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Dapper.SqlBuilder
{
    public partial class SqlBuilder<T>
    {
        public ISqlBuilder<TResult> ScalarResult<TResult>(Expression<Func<T, dynamic>> expression)
        {
            Resolver.Select(expression);
            return new SqlBuilder<TResult>(this.Builder, this.Resolver);
        }

        public ISqlBuilder<TResult> Result<TResult>(Expression<Func<T, TResult>> expression)
        {
            Resolver.Select(expression);
            return new SqlBuilder<TResult>(this.Builder, this.Resolver);
        }

        public ISqlBuilder<TResult> Result<T2, TResult>(Expression<Func<T, T2, TResult>> expression)
        {
            Resolver.Select(expression);
            return new SqlBuilder<TResult>(this.Builder, this.Resolver);
        }

        public ISqlBuilder<T> Result<T2, T3, TResult>(Expression<Func<T, T2, T3, TResult>> expression)
        {
            Resolver.Select(expression);
            return this;
        }

        public ISqlBuilder<T> Result<T2, T3, T4, TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression)
        {
            Resolver.Select(expression);
            return this;
        }

        public ISqlBuilder<T> Result<T2, T3, T4, T5, TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression)
        {
            Resolver.Select(expression);
            return this;
        }

        public ISqlBuilder<T> Result<T2, T3, T4, T5, T6, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression)
        {
            Resolver.Select(expression);
            return this;
        }

        public ISqlBuilder<T> Result<T2, T3, T4, T5, T6, T7, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression)
        {
            Resolver.Select(expression);
            return this;
        }

        public ISqlBuilder<T> Result<T2, T3, T4, T5, T6, T7, T8, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression)
        {
            Resolver.Select(expression);
            return this;
        }

        public ISqlBuilder<T> Result<T2, T3, T4, T5, T6, T7, T8, T9, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression)
        {
            Resolver.Select(expression);
            return this;
        }

        public ISqlBuilder<T> Result<T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression)
        {
            Resolver.Select(expression);
            return this;
        }

        public ISqlBuilder<T> Result<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>> expression)
        {
            Resolver.Select(expression);
            return this;
        }

        public ISqlBuilder<T> Result<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>> expression)
        {
            Resolver.Select(expression);
            return this;
        }

        public ISqlBuilder<T> Result<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>> expression)
        {
            Resolver.Select(expression);
            return this;
        }

        public ISqlBuilder<T> Result<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>> expression)
        {
            Resolver.Select(expression);
            return this;
        }

        public ISqlBuilder<T> Result<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>> expression)
        {
            Resolver.Select(expression);
            return this;
        }

        public ISqlBuilder<T> Result<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>> expression)
        {
            Resolver.Select(expression);
            return this;
        }
    }

    public partial interface ISqlBuilder<T>
    {
        ISqlBuilder<TResult> ScalarResult<TResult>(Expression<Func<T, dynamic>> expression);
        ISqlBuilder<TResult> Result<TResult>(Expression<Func<T, TResult>> expression);
        ISqlBuilder<TResult> Result<T2, TResult>(Expression<Func<T, T2, TResult>> expression);
        ISqlBuilder<T> Result<T2, T3, TResult>(Expression<Func<T, T2, T3, TResult>> expression);
        ISqlBuilder<T> Result<T2, T3, T4, TResult>(Expression<Func<T, T2, T3, T4, TResult>> expression);
        ISqlBuilder<T> Result<T2, T3, T4, T5, TResult>(Expression<Func<T, T2, T3, T4, T5, TResult>> expression);
        ISqlBuilder<T> Result<T2, T3, T4, T5, T6, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, TResult>> expression);
        ISqlBuilder<T> Result<T2, T3, T4, T5, T6, T7, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, TResult>> expression);
        ISqlBuilder<T> Result<T2, T3, T4, T5, T6, T7, T8, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, TResult>> expression);
        ISqlBuilder<T> Result<T2, T3, T4, T5, T6, T7, T8, T9, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression);
        ISqlBuilder<T> Result<T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression);
        ISqlBuilder<T> Result<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>> expression);
        ISqlBuilder<T> Result<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>> expression);
        ISqlBuilder<T> Result<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>> expression);
        ISqlBuilder<T> Result<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>> expression);
        ISqlBuilder<T> Result<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>> expression);
        ISqlBuilder<T> Result<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(Expression<Func<T, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>> expression);
    }

}
