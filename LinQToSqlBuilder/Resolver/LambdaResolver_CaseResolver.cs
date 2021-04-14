using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Dapper.SqlBuilder.Resolver
{
    partial class LambdaResolver
    {
        public void Case<T>(Expression<Func<T, object>> expression)
        {
            var expressionTree = ResolveQuery((dynamic)expression.Body);
            BuildSql(expressionTree);
        } 

        public void Case<THeader, T>(Expression<Func<THeader, T, object>> expression)
        {
            var expressionTree = ResolveQuery((dynamic)expression.Body);
            BuildSql(expressionTree);
        }
    }
}
