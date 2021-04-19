using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Dapper.SqlBuilder.Adapter;
using Dapper.SqlBuilder.Builder;
using Dapper.SqlBuilder.Extensions;
using Dapper.SqlBuilder.Resolver.ExpressionTree;
using Dapper.SqlBuilder.ValueObjects;

namespace Dapper.SqlBuilder.Resolver
{
    /// <summary>
    /// Provides methods to perform resolution to SQL expressions for SELECT, JOIN, ORDER BY query from given lambda expressions
    /// </summary>
    partial class LambdaResolver
    {
        public void Join<T1, T2>(Expression<Func<T1, T2, bool>> expression, JoinType joinType)
        {
            var rebuilder = new SqlJoinBuilder<T1, T2>(Builder.CurrentParamIndex); //.Build(expression);
            //foreach (var p in rebuilder.CommandParameters)
            //    Builder.Parameters.Add(p);
            //Builder.CurrentParamIndex = rebuilder.Builder.CurrentParamIndex;
            Builder.Join(GetTableName<T2>(), rebuilder, expression, joinType);
        }

        public void Join<T1, T2, T3>(Expression<Func<T1, T2, T3, bool>> expression, JoinType joinType)
        {
            var rebuilder = new SqlJoinBuilder<T1, T2, T3>(Builder.CurrentParamIndex).Build(expression);
            foreach (var p in rebuilder.CommandParameters)
                Builder.Parameters.Add(p);
            Builder.CurrentParamIndex = rebuilder.Builder.CurrentParamIndex;
            Builder.Join(GetTableName<T2>(), rebuilder.Builder, joinType);
        }
        public void Join<T1, T2, T3, T4>(Expression<Func<T1, T2, T3, T4, bool>> expression, JoinType joinType)
        {
            var rebuilder = new SqlJoinBuilder<T1, T2, T3, T4>(Builder.CurrentParamIndex).Build(expression);
            foreach (var p in rebuilder.CommandParameters)
                Builder.Parameters.Add(p);
            Builder.CurrentParamIndex = rebuilder.Builder.CurrentParamIndex;
            Builder.Join(GetTableName<T2>(), rebuilder.Builder, joinType);
        }

        public void OrderBy<T>(Expression<Func<T, object>> expression, bool desc = false)
        {
            var fieldName = GetColumnName(GetMemberExpression(expression.Body));
            Builder.OrderBy(GetTableName<T>(), fieldName, desc);
        }

        public void Select<T>(Expression<Func<T, object>> expression)
        {
            Select<T>(expression.Body);
        }

        public void Select<T, TResult>(Expression<Func<T, TResult>> expression)
        {
            Select<T>(expression.Body);
        }

        private void Select<T>(Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Parameter:
                    Builder.Select(GetTableName(expression.Type));
                    break;
                case ExpressionType.Convert:
                case ExpressionType.MemberAccess:
                    if (ParseMethodCallExpression(expression, out var mace))
                    {
                        SelectWithSqlFunctionCall(mace as MethodCallExpression, null);
                    }
                    else
                    {
                        Select<T>(mace);
                    }
                    break;
                case ExpressionType.New:
                    var nxprs = (expression as NewExpression);

                    for (var i = 0; i < nxprs.Arguments.Count; i++)
                    {
                        var expr = nxprs.Arguments[i];
                        var mem = nxprs.Members[i];

                        if (expr is MethodCallExpression mce)
                        {
                            SelectWithSqlFunctionCall(mce, mem.Name);
                        }
                        else
                            Select<T>((MemberExpression)expr, mem.Name);
                    }

                    break;
                case ExpressionType.MemberInit:
                    if (expression is MemberInitExpression memberInitExpression)
                    {
                        foreach (var memberExp in memberInitExpression.Bindings)
                        {
                            if (memberExp is MemberAssignment assignmentExpression)
                            {
                                Select<T>(assignmentExpression);
                            }
                        }
                        break;
                    }

                    throw new ArgumentException("Invalid expression");
                default:
                    throw new ArgumentException("Invalid expression");
            }
        }

        private void Select<T>(MemberExpression expression, string alias = null)
        {
            if (expression.Type.IsClass && expression.Type != typeof(String))
                Builder.Select(GetTableName(expression.Type));
            else if (expression?.Expression.Type.IsClass == true)
                Builder.Select(GetTableName(expression.Expression.Type), GetColumnName(expression), alias);
            else
                Builder.Select(GetTableName<T>(), GetColumnName(expression), alias);
        }

        private void Select<T>(MemberAssignment member)
        {
            var exp = member.Expression;
            var alias = GetColumnName(member.Member);

            if (member.Expression is MemberExpression memberExpression)
            {
                Select<T>(memberExpression, alias);
            }
            else if (member.Expression is MethodCallExpression mce)
            {
                SelectWithSqlFunctionCall(mce, alias);
            }
            else if (member.Expression is UnaryExpression ue)
            {
                SelectUnaryWithCall<T>(ue, alias);
            }
            else
            {
                var column = GetColumnName(exp);

                if (exp.Type.IsClass && exp.Type != typeof(String))
                    Builder.Select(GetTableName(member.Expression.Type));
                else
                    Builder.Select(GetTableName<T>(), column, alias);
            }
        }

        private void SelectUnaryWithCall<T>(UnaryExpression ex, string alias)
        {
            if (ex.Operand is MethodCallExpression mcue)
                SelectWithSqlFunctionCall(mcue, alias);
            else if (ex.Operand is UnaryExpression eus)
            {
                SelectUnaryWithCall<T>(eus, alias);
            }
            else
                Select<T>(ex.Operand);
        }

        public string GetTableName(Expression expression)
        {
            if (expression is MethodCallExpression mce)
            {
                return GetTableName(mce.Arguments[0]);
            }
            if (expression is MemberExpression me)
            {
                return GetTableName(me.Expression.Type);
            }
            else if (expression is UnaryExpression un)
            {
                return GetTableName(un.Operand);
            }

            throw new Exception("Cant define a table from an expression");
        }

        public void SelectWithFunction<T, TResult>(Expression<Func<T, TResult>> expression, SelectFunctionType selectFunction)
        {
            SelectWithFunction<T>(expression.Body, selectFunction);
        }

        public void SelectWithFunction<T, TResult>(string functionStatement, Expression<Func<T, TResult>> expression, params object[] args)
        {
            SelectWithFunction<T>(functionStatement, expression?.Body, args);
        }

        private void SelectWithFunction<T>(Expression expression, SelectFunctionType selectFunction)
        {
            var fieldName = GetColumnName(GetMemberExpression(expression));
            Builder.Select(GetTableName<T>(), fieldName, selectFunction);
        }

        private void SelectWithFunction<T>(string functionStatement, Expression expression, object[] args = null)
        {
            var prms = Builder.ParseParameter(functionStatement);

            if (args == null && prms.Count > 0)
                throw new InvalidOperationException("Function Statement requires a parameter");

            if (prms.Count != args.Length)
                throw new InvalidOperationException("Function Statement parameters does not match the parameter specified");

            for (var i = 0; i < prms.Count; i++)
                Builder.AddParameter(prms[i], args[i]);

            if (expression != null)
            {
                Select<T>(expression);
            }
            else
            {
                Builder.Select(GetTableName<T>());
            }
        }

        public void SelectWithFunction<T>(SelectFunctionType selectFunction)
        {
            Builder.Select(selectFunction);
        }

        public void GroupBy<T>(Expression<Func<T, object>> expression)
        {
            GroupBy<T>(GetMemberExpression(expression.Body));
        }

        private void GroupBy<T>(Expression expression)
        {
            var fieldName = GetColumnName(GetMemberExpression(expression));
            Builder.GroupBy(GetTableName<T>(), fieldName);
        }

        private void SelectWithSqlFunctionCall(MethodCallExpression mce, string alias)
        {
            if (mce.Method.Name == nameof(Extensions.TypeExtensions.As))
            {
                var column = GetColumnName(mce);
                Builder.Select(GetTableName(mce), column, alias);
                return;
            }

            if (mce.Method.Name == nameof(Extensions.TypeExtensions.FormatSql))
            {
                var column = GetColumnName(mce);
                var format = GetExpressionValue(mce.Arguments[1]).ToString();
                Builder.SelectFormat(GetTableName(mce), column, alias, format);
                return;
            }

            if (mce.Method.Name == nameof(Extensions.TypeExtensions.IsNullSql))
            {
                var column = GetColumnName(mce);
                if (mce.Arguments[1] is MemberExpression me)
                {
                    if (IsDateNow(me))
                    {
                        Builder.SelectIsNull(GetTableName(mce), column, alias, Builder.Adapter.CurrentDate());
                        return;
                    }
                    else if (me.Expression is ParameterExpression)
                    {
                        var nullColumn = GetColumnName(mce.Arguments[1]);
                        var nullTableName = GetTableName(mce.Arguments[1]);
                        Builder.SelectIsNull(GetTableName(mce), column, alias, nullColumn, nullTableName);
                        return;
                    }
                }

                var nullValue = GetExpressionValue(mce.Arguments[1]);
                Builder.SelectIsNull(GetTableName(mce), column, alias, nullValue);
                return;
            }

            if (mce.Method.Name == nameof(Extensions.TypeExtensions.ConcatSql))
            {
                var column = GetColumnName(mce);

                if (mce.Arguments[1] is NewArrayExpression nae)
                {
                    var nlst = new List<string>();
                    for (var i = 0; i < nae.Expressions.Count; i++)
                    {
                        var expr = ResolveExpression(nae.Expressions[i]);

                        if (expr is MemberExpression me)
                        {
                            if (IsDateNow(me))
                            {
                                nlst.Add(Builder.Adapter.CurrentDate());
                                continue;
                            }
                            else if (me.Expression is ParameterExpression)
                            {
                                var nullColumn = GetColumnName(expr);
                                var nullTableName = GetTableName(expr);
                                nlst.Add(Builder.Adapter.Field(Builder.GetTableAlias(nullTableName), nullColumn));
                                continue;
                            }
                        }

                        var nullValue = GetExpressionValue(expr);
                        nlst.Add(nullValue.SafeValue());
                    }

                    Builder.SelectConcatSql(GetTableName(mce), column, alias, nlst);
                    return;
                }
            }

            if (mce.Method.DeclaringType.Name == typeof(SqlCase).Name)
            {
                var column = GetColumnName(mce);
                if (mce.Arguments[1] is UnaryExpression me)
                {
                    if (me.Operand is LambdaExpression le)
                    {
                        var nls = new List<object>();
                        foreach (var gd in le.Parameters)
                        {
                            nls.Add(null);
                        }

                        var vad = le.Compile().DynamicInvoke(nls.ToArray());
                        var npc = vad as ISqlBuilder;

                        foreach (var np in npc.CommandParameters)
                            Builder.Parameters.Add(np);

                        Builder.SelectCase(npc.CommandText, alias);
                        return;
                    }
                }
            }

            if (mce.Method.Name == nameof(Extensions.TypeExtensions.DatePartSql))
            {
                var column = GetColumnName(mce);
                var partValue = (DatePart)GetExpressionValue(mce.Arguments[1]);
                Builder.SelectDatePartSql(GetTableName(mce), column, alias, partValue);
                return;
            }

            throw new Exception("Use As<> extension to map type differences");
        }

        private Expression ResolveExpression(Expression ex)
        {
            if (ex is UnaryExpression eus)
                return ResolveExpression(eus.Operand);
            return ex;
        }

        public bool IsDateNow(MemberExpression me)
        {
            return me.Member.Name == "Now" && me.Type == typeof(DateTime);
        }

    }
}
