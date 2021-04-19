using System.Linq.Expressions;
using System.Reflection;
using Dapper.SqlBuilder.Builder;

namespace Dapper.SqlBuilder.UpdateResolver
{
    internal interface IUpdateStatementResolver
    {
        MethodInfo SupportedMethod { get; }

        void ResolveStatement(string sourceAlias, SqlQueryBuilder builder,
                              MethodCallExpression callExpression,
                              object[] arguments);
    }
}
