using Dapper.SqlBuilder.Adapter;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Dapper.SqlBuilder
{
    public partial class SqlBuilder<T>
    {
        private ISqlBuilder<T2> Join<T2>(Expression<Func<T, T2, bool>> expression, JoinType joinType)
        {
            var joinQuery = new SqlBuilder<T2>(Builder, Resolver);
            Resolver.Join(expression, joinType);
            return joinQuery;
        }

        private ISqlBuilder<T2> Join<T2, T3>(Expression<Func<T, T2, T3, bool>> expression, JoinType joinType)
        {
            var joinQuery = new SqlBuilder<T2>(Builder, Resolver);
            Resolver.Join(expression, joinType);
            return joinQuery;
        }

        private ISqlBuilder<T2> Join<T2>(Expression<Func<T, T2, bool>> expression, Expression<Func<T2, object>> columns, JoinType joinType)
        {
            var joinQuery = new SqlBuilder<T2>(Builder, Resolver).Select(columns);
            Resolver.Join(expression, joinType);
            return joinQuery;
        }

        private ISqlBuilder<T2> Join<T2, T3>(Expression<Func<T, T2, T3, bool>> expression, Expression<Func<T2, T3, object>> columns, JoinType joinType)
        {
            var joinQuery = new SqlBuilder<T2>(Builder, Resolver).Select(columns);
            Resolver.Join(expression, joinType);
            return joinQuery;
        }

        public ISqlBuilder<T2> InnerJoin<T2>(Expression<Func<T, T2, bool>> expression) => Join(expression, JoinType.InnerJoin);
        public ISqlBuilder<T2> InnerJoin<T2, T3>(Expression<Func<T, T2, T3, bool>> expression) => Join(expression, JoinType.InnerJoin);
        public ISqlBuilder<T2> InnerJoin<T2>(Expression<Func<T, T2, bool>> expression, Expression<Func<T2, object>> columns) => Join(expression, columns, JoinType.InnerJoin);
        public ISqlBuilder<T2> InnerJoin<T2, T3>(Expression<Func<T, T2, T3, bool>> expression, Expression<Func<T2, T3, object>> columns) => Join(expression, columns, JoinType.InnerJoin);
        public ISqlBuilder<T2> LeftJoin<T2>(Expression<Func<T, T2, bool>> expression) => Join(expression, JoinType.LeftJoin);
        public ISqlBuilder<T2> LeftJoin<T2, T3>(Expression<Func<T, T2, T3, bool>> expression) => Join(expression, JoinType.LeftJoin);
        public ISqlBuilder<T2> LeftJoin<T2>(Expression<Func<T, T2, bool>> expression, Expression<Func<T2, object>> columns) => Join(expression, columns, JoinType.LeftJoin);
        public ISqlBuilder<T2> LeftJoin<T2, T3>(Expression<Func<T, T2, T3, bool>> expression, Expression<Func<T2, T3, object>> columns) => Join(expression, columns, JoinType.LeftJoin);
        public ISqlBuilder<T2> LeftOuterJoin<T2>(Expression<Func<T, T2, bool>> expression) => Join(expression, JoinType.LeftOuterJoin);
        public ISqlBuilder<T2> LeftOuterJoin<T2, T3>(Expression<Func<T, T2, T3, bool>> expression) => Join(expression, JoinType.LeftOuterJoin);
        public ISqlBuilder<T2> LeftOuterJoin<T2>(Expression<Func<T, T2, bool>> expression, Expression<Func<T2, object>> columns) => Join(expression, columns, JoinType.LeftOuterJoin);
        public ISqlBuilder<T2> LeftOuterJoin<T2, T3>(Expression<Func<T, T2, T3, bool>> expression, Expression<Func<T2, T3, object>> columns) => Join(expression, columns, JoinType.LeftOuterJoin);
        public ISqlBuilder<T2> RightJoin<T2>(Expression<Func<T, T2, bool>> expression) => Join(expression, JoinType.RightJoin);
        public ISqlBuilder<T2> RightJoin<T2, T3>(Expression<Func<T, T2, T3, bool>> expression) => Join(expression, JoinType.RightJoin);
        public ISqlBuilder<T2> RightJoin<T2>(Expression<Func<T, T2, bool>> expression, Expression<Func<T2, object>> columns) => Join(expression, columns, JoinType.RightJoin);
        public ISqlBuilder<T2> RightJoin<T2, T3>(Expression<Func<T, T2, T3, bool>> expression, Expression<Func<T2, T3, object>> columns) => Join(expression, columns, JoinType.RightJoin);
        public ISqlBuilder<T2> RightOuterJoin<T2>(Expression<Func<T, T2, bool>> expression) => Join(expression, JoinType.RightOuterJoin);
        public ISqlBuilder<T2> RightOuterJoin<T2, T3>(Expression<Func<T, T2, T3, bool>> expression) => Join(expression, JoinType.RightOuterJoin);
        public ISqlBuilder<T2> RightOuterJoin<T2>(Expression<Func<T, T2, bool>> expression, Expression<Func<T2, object>> columns) => Join(expression, columns, JoinType.RightOuterJoin);
        public ISqlBuilder<T2> RightOuterJoin<T2, T3>(Expression<Func<T, T2, T3, bool>> expression, Expression<Func<T2, T3, object>> columns) => Join(expression, columns, JoinType.RightOuterJoin);
        public ISqlBuilder<T2> FullJoin<T2>(Expression<Func<T, T2, bool>> expression) => Join(expression, JoinType.FullJoin);
        public ISqlBuilder<T2> FullJoin<T2, T3>(Expression<Func<T, T2, T3, bool>> expression) => Join(expression, JoinType.FullJoin);
        public ISqlBuilder<T2> FullJoin<T2>(Expression<Func<T, T2, bool>> expression, Expression<Func<T2, object>> columns) => Join(expression, columns, JoinType.FullJoin);
        public ISqlBuilder<T2> FullJoin<T2, T3>(Expression<Func<T, T2, T3, bool>> expression, Expression<Func<T2, T3, object>> columns) => Join(expression, columns, JoinType.FullJoin);
        public ISqlBuilder<T2> FullOuterJoin<T2>(Expression<Func<T, T2, bool>> expression) => Join(expression, JoinType.FullOuterJoin);
        public ISqlBuilder<T2> FullOuterJoin<T2, T3>(Expression<Func<T, T2, T3, bool>> expression) => Join(expression, JoinType.FullOuterJoin);
        public ISqlBuilder<T2> FullOuterJoin<T2>(Expression<Func<T, T2, bool>> expression, Expression<Func<T2, object>> columns) => Join(expression, columns, JoinType.FullOuterJoin);
        public ISqlBuilder<T2> FullOuterJoin<T2, T3>(Expression<Func<T, T2, T3, bool>> expression, Expression<Func<T2, T3, object>> columns) => Join(expression, columns, JoinType.FullOuterJoin);
        public ISqlBuilder<T2> CrossJoin<T2>(Expression<Func<T, T2, bool>> expression) => Join(expression, JoinType.CrossJoin);
        public ISqlBuilder<T2> CrossJoin<T2, T3>(Expression<Func<T, T2, T3, bool>> expression) => Join(expression, JoinType.CrossJoin);
        public ISqlBuilder<T2> CrossJoin<T2>(Expression<Func<T, T2, bool>> expression, Expression<Func<T2, object>> columns) => Join(expression, columns, JoinType.CrossJoin);
        public ISqlBuilder<T2> CrossJoin<T2, T3>(Expression<Func<T, T2, T3, bool>> expression, Expression<Func<T2, T3, object>> columns) => Join(expression, columns, JoinType.CrossJoin);
    }

    public partial interface ISqlBuilder<T>
    {
        ISqlBuilder<T2> InnerJoin<T2>(Expression<Func<T, T2, bool>> expression);
        ISqlBuilder<T2> InnerJoin<T2, T3>(Expression<Func<T, T2, T3, bool>> expression);
        ISqlBuilder<T2> InnerJoin<T2>(Expression<Func<T, T2, bool>> expression, Expression<Func<T2, object>> columns);
        ISqlBuilder<T2> InnerJoin<T2, T3>(Expression<Func<T, T2, T3, bool>> expression, Expression<Func<T2, T3, object>> columns);
        ISqlBuilder<T2> LeftJoin<T2>(Expression<Func<T, T2, bool>> expression);
        ISqlBuilder<T2> LeftJoin<T2, T3>(Expression<Func<T, T2, T3, bool>> expression);
        ISqlBuilder<T2> LeftJoin<T2>(Expression<Func<T, T2, bool>> expression, Expression<Func<T2, object>> columns);
        ISqlBuilder<T2> LeftJoin<T2, T3>(Expression<Func<T, T2, T3, bool>> expression, Expression<Func<T2, T3, object>> columns); 
        ISqlBuilder<T2> LeftOuterJoin<T2>(Expression<Func<T, T2, bool>> expression);
        ISqlBuilder<T2> LeftOuterJoin<T2, T3>(Expression<Func<T, T2, T3, bool>> expression);
        ISqlBuilder<T2> LeftOuterJoin<T2>(Expression<Func<T, T2, bool>> expression, Expression<Func<T2, object>> columns);
        ISqlBuilder<T2> LeftOuterJoin<T2, T3>(Expression<Func<T, T2, T3, bool>> expression, Expression<Func<T2, T3, object>> columns);
        ISqlBuilder<T2> RightJoin<T2>(Expression<Func<T, T2, bool>> expression);
        ISqlBuilder<T2> RightJoin<T2, T3>(Expression<Func<T, T2, T3, bool>> expression);
        ISqlBuilder<T2> RightJoin<T2>(Expression<Func<T, T2, bool>> expression, Expression<Func<T2, object>> columns);
        ISqlBuilder<T2> RightJoin<T2, T3>(Expression<Func<T, T2, T3, bool>> expression, Expression<Func<T2, T3, object>> columns);
        ISqlBuilder<T2> RightOuterJoin<T2>(Expression<Func<T, T2, bool>> expression);
        ISqlBuilder<T2> RightOuterJoin<T2, T3>(Expression<Func<T, T2, T3, bool>> expression);
        ISqlBuilder<T2> RightOuterJoin<T2>(Expression<Func<T, T2, bool>> expression, Expression<Func<T2, object>> columns);
        ISqlBuilder<T2> RightOuterJoin<T2, T3>(Expression<Func<T, T2, T3, bool>> expression, Expression<Func<T2, T3, object>> columns);
        ISqlBuilder<T2> FullJoin<T2>(Expression<Func<T, T2, bool>> expression);
        ISqlBuilder<T2> FullJoin<T2, T3>(Expression<Func<T, T2, T3, bool>> expression);
        ISqlBuilder<T2> FullJoin<T2>(Expression<Func<T, T2, bool>> expression, Expression<Func<T2, object>> columns);
        ISqlBuilder<T2> FullJoin<T2, T3>(Expression<Func<T, T2, T3, bool>> expression, Expression<Func<T2, T3, object>> columns);
        ISqlBuilder<T2> FullOuterJoin<T2>(Expression<Func<T, T2, bool>> expression);
        ISqlBuilder<T2> FullOuterJoin<T2, T3>(Expression<Func<T, T2, T3, bool>> expression);
        ISqlBuilder<T2> FullOuterJoin<T2>(Expression<Func<T, T2, bool>> expression, Expression<Func<T2, object>> columns);
        ISqlBuilder<T2> FullOuterJoin<T2, T3>(Expression<Func<T, T2, T3, bool>> expression, Expression<Func<T2, T3, object>> columns);
        ISqlBuilder<T2> CrossJoin<T2>(Expression<Func<T, T2, bool>> expression);
        ISqlBuilder<T2> CrossJoin<T2, T3>(Expression<Func<T, T2, T3, bool>> expression);
        ISqlBuilder<T2> CrossJoin<T2>(Expression<Func<T, T2, bool>> expression, Expression<Func<T2, object>> columns);
        ISqlBuilder<T2> CrossJoin<T2, T3>(Expression<Func<T, T2, T3, bool>> expression, Expression<Func<T2, T3, object>> columns);
    }
}
