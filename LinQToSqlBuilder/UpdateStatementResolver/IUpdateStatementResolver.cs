using System.Linq.Expressions;
using System.Reflection;
using Dapper.SqlBuilder.Builder;

namespace Dapper.SqlBuilder.UpdateStatementResolver
{
    internal interface IUpdateStatementResolver
    {
        MethodInfo SupportedMethod { get; }

        void ResolveStatement(SqlQueryBuilder      builder,
                              MethodCallExpression callExpression,
                              object[]             arguments);
    }
}