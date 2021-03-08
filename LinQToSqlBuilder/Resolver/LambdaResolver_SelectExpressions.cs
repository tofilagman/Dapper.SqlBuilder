using System;
using System.Linq;
using System.Linq.Expressions;
using Dapper.SqlBuilder.Adapter;
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
            var rebuilder = new SqlJoinBuilder<T1, T2>(Builder.CurrentParamIndex).Build(expression);
            foreach (var p in rebuilder.CommandParameters)
                Builder.Parameters.Add(p);
            Builder.CurrentParamIndex = rebuilder.Builder.CurrentParamIndex;
            Builder.Join(GetTableName<T2>(), rebuilder.Builder.WhereCommandText, joinType);
        }

        [Obsolete]
        public void Join<T1, T2, TKey>(Expression<Func<T1, TKey>> leftExpression, Expression<Func<T1, TKey>> rightExpression, JoinType joinType)
        {
            Join<T1, T2>(GetMemberExpression(leftExpression.Body), GetMemberExpression(rightExpression.Body), joinType);
        }

        [Obsolete]
        public void Join<T1, T2>(MemberExpression leftExpression, MemberExpression rightExpression, JoinType joinType)
        {
            Builder.Join(GetTableName<T1>(), GetTableName<T2>(), GetColumnName(leftExpression), GetColumnName(rightExpression), joinType);
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
                    Select<T>(GetMemberExpression(expression));
                    break;
                case ExpressionType.New:
                    foreach (MemberExpression memberExp in (expression as NewExpression).Arguments)
                        Select<T>(memberExp);
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
                if (mce.Method.Name != nameof(Extensions.TypeExtensions.As))
                    throw new Exception("Use As<> extension to map type differences");

                var column = GetColumnName(mce); 
                Builder.Select(GetTableName(mce), column, alias);
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

        public void SelectWithFunction<T>(Expression<Func<T, object>> expression, SelectFunction selectFunction)
        {
            SelectWithFunction<T>(expression.Body, selectFunction);
        }

        private void SelectWithFunction<T>(Expression expression, SelectFunction selectFunction)
        {
            var fieldName = GetColumnName(GetMemberExpression(expression));
            Builder.Select(GetTableName<T>(), fieldName, selectFunction);
        }

        public void SelectWithFunction<T>(SelectFunction selectFunction)
        {
            Builder.Select(selectFunction);
        }

        public void GroupBy<T>(Expression<Func<T, object>> expression)
        {
            GroupBy<T>(GetMemberExpression(expression.Body));
        }

        private void GroupBy<T>(MemberExpression expression)
        {
            var fieldName = GetColumnName(GetMemberExpression(expression));
            Builder.GroupBy(GetTableName<T>(), fieldName);
        }
    }
}
