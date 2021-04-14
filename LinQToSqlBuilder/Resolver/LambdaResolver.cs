﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;
using Dapper.SqlBuilder.Builder;
using Dapper.SqlBuilder.UpdateResolver;

namespace Dapper.SqlBuilder.Resolver
{
    /// <summary>
    /// Provides methods to perform resolution to SQL expressions from given lambda expressions
    /// </summary>
    internal partial class LambdaResolver
    {
        private readonly Dictionary<ExpressionType, string> _operationDictionary =
            new Dictionary<ExpressionType, string>()
            {
                {ExpressionType.Equal, "="},
                {ExpressionType.NotEqual, "!="},
                {ExpressionType.GreaterThan, ">"},
                {ExpressionType.LessThan, "<"},
                {ExpressionType.GreaterThanOrEqual, ">="},
                {ExpressionType.LessThanOrEqual, "<="}
            };

        private SqlQueryBuilder Builder { get; set; }

        public LambdaResolver(SqlQueryBuilder builder)
        {
            Builder = builder;
        }

        #region helpers

        private static readonly List<IUpdateStatementResolver> StatementResolvers = new List<IUpdateStatementResolver>
        {
            new StringReplaceUpdateResolver()
        };

        public static void RegisterResolver(IUpdateStatementResolver resolver)
        {
            StatementResolvers.Add(resolver);
        }

        public static string GetColumnName<T>(Expression<Func<T, object>> selector)
        {
            return GetColumnName(GetMemberExpression(selector.Body));
        }

        public static string GetColumnName(Expression expression)
        {
            if (expression.NodeType != ExpressionType.Call)
            {
                var member = GetMemberExpression(expression);
                return GetColumnName(member);
            }
            else
            {
                var mem = (expression as MethodCallExpression).Arguments[0];
                return GetColumnName(mem);
            }
        }

        public static string GetColumnName(MemberAssignment expression)
        {
            return GetColumnName(expression.Member);
        }

        private static string GetColumnName(MemberExpression member)
        {
            var memberInfo = member.Member;
            return GetColumnName(memberInfo);
        }

        private static string GetColumnName(MemberInfo memberInfo)
        {
            var column = memberInfo.GetCustomAttribute<ColumnAttribute>();

            if (column != null)
                return $"{column.Name}";
            else
                return $"{memberInfo.Name}";
        }

        public static string GetTableName<T>()
        {
            return GetTableName(typeof(T));
        }

        public static string GetTableName(Type type)
        {
            var tableAttribute = type.GetCustomAttribute<TableAttribute>();
            if (tableAttribute != null)
                if (string.IsNullOrEmpty(tableAttribute.Schema))
                    return tableAttribute.Name;
                else
                    return $"{tableAttribute.Schema}.{tableAttribute.Name}";
            else
                return $"{type.Name}";
        }

        private static string GetTableName(MemberExpression expression)
        {
            return GetTableName(expression.Expression.Type);
        }

        private static BinaryExpression GetBinaryExpression(Expression expression)
        {
            if (expression is BinaryExpression binaryExpression)
                return binaryExpression;

            throw new ArgumentException("Binary expression expected");
        }

        private static MemberExpression GetMemberExpression(Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.MemberAccess:
                    return expression as MemberExpression;
                case ExpressionType.Convert:
                    return GetMemberExpression((expression as UnaryExpression)?.Operand);
                case ExpressionType.Lambda:
                    return GetMemberExpression((expression as LambdaExpression).Body);
                case ExpressionType.Constant:
                    var fk = new FakeObject { Data = (expression as ConstantExpression).Value };
                    return Expression.PropertyOrField(Expression.Constant(fk), nameof(FakeObject.Data));
            }

            throw new ArgumentException("Member expression expected");
        }

        #endregion
    }

    class FakeObject
    {
        public object Data { get; set; }
    }
}
