using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Dapper.SqlBuilder.Adapter;
using Dapper.SqlBuilder.Extensions;
using Dapper.SqlBuilder.Resolver;
using Dapper.SqlBuilder.ValueObjects;

namespace Dapper.SqlBuilder.Builder
{
    /// <summary>
    /// Implements the SQL building for JOIN, ORDER BY, SELECT, and GROUP BY
    /// </summary>
    internal partial class SqlQueryBuilder
    {
        [Obsolete]
        public void Join(string originalTableName, string joinTableName, string leftField, string rightField, JoinType joinType)
        {
            var join = GetJoinExpression(joinType);
            var joinAlias = LambdaResolver.Shortener(joinTableName, true);

            var joinString =
                $"{join} { joinAlias } " +
                $"ON {Adapter.Field(originalTableName, leftField)} = {Adapter.Field(joinAlias, rightField)}";

            TableNames.Add(new TableValue { TableName = joinTableName, Alias = joinAlias });
            JoinExpressions.Add(joinString);
            SplitColumns.Add(rightField);
        }

        public void Join(string joinTableName, SqlQueryBuilder joinQuery, JoinType joinType)
        {
            joinQuery.TableNames.AddRange(TableNames);

            var join = GetJoinExpression(joinType);
            var joinAlias = LambdaResolver.Shortener(joinTableName, true);

            if (TableNames.Any(x => x.TableName == joinTableName))
            {
                var cnt = TableNames.Where(x => x.TableName == joinTableName).Count();
                TableNames.Add(new TableValue { TableName = joinTableName, Alias = $"{joinAlias}{ cnt }" });
            }
            else
                TableNames.Add(new TableValue { TableName = joinTableName, Alias = joinAlias });

            var joinString =
               $"{join} { joinTableName } { joinAlias } " +
               $"ON { joinQuery.WhereCommandText }";

            JoinExpressions.Add(joinString);
        }

        public void Join<T1, T2>(string joinTableName, SqlJoinBuilder<T1, T2> joinBuilder, Expression<Func<T1, T2, bool>> expression, JoinType joinType)
        {
            joinBuilder.Builder.TableNames.AddRange(TableNames);
            var joinQuery = joinBuilder.Build(expression).Builder;

            foreach (var p in joinBuilder.CommandParameters)
                Parameters.Add(p);
            CurrentParamIndex = joinBuilder.CurrentParamIndex;

            Join(joinTableName, joinQuery, joinType);
        }

        private string GetJoinExpression(JoinType joinType)
        {
            switch (joinType)
            {
                case JoinType.LeftJoin:
                    return "LEFT JOIN";
                case JoinType.LeftOuterJoin:
                    return "LEFT OUTER JOIN";
                case JoinType.FullJoin:
                    return "FULL JOIN";
                case JoinType.FullOuterJoin:
                    return "FULL OUTER JOIN";
                case JoinType.RightJoin:
                    return "RIGHT JOIN";
                case JoinType.RightOuterJoin:
                    return "RIGHT OUTER JOIN";
                case JoinType.InnerJoin:
                    return "INNER JOIN";
                case JoinType.CrossJoin:
                    return "CROSS JOIN";
                default:
                    return "JOIN";
            }
        }

        public void OrderBy(string tableName, string fieldName, bool desc = false)
        {
            var order = Adapter.Field(GetTableAlias(tableName), fieldName);
            if (desc)
                order += " DESC";

            OrderByList.Add(order);
        }

        public void Select(string tableName)
        {
            var selectionString = $"{ GetTableAlias(tableName) }.*";
            SelectionList.Add(selectionString);
        }

        public void Select(string tableName, string fieldName)
        {
            SelectionList.Add(Adapter.Field(GetTableAlias(tableName), fieldName));
        }

        public void Select(string tablename, string fieldName, string alias)
        {
            var tableAlias = GetTableAlias(tablename);
            if (string.IsNullOrEmpty(alias) || fieldName == alias)
                Select(tableAlias, fieldName);
            else
                SelectionList.Add($"{ Adapter.Field(tableAlias, fieldName) } { Adapter.Alias(alias) }");
        }

        public void SelectFormat(string tableName, string fieldName, string alias, string format)
        {
            var tableAlias = GetTableAlias(tableName);
            if (string.IsNullOrEmpty(alias) || fieldName == alias)
                SelectionList.Add($"{ Adapter.Format() }({ Adapter.Field(tableAlias, fieldName) }, '{ format }') { Adapter.Alias(fieldName) }");
            else
                SelectionList.Add($"{ Adapter.Format() }({ Adapter.Field(tableAlias, fieldName) }, '{ format }') { Adapter.Alias(alias) }");
        }

        public void SelectIsNull(string tableName, string fieldName, string alias, object nullValue)
        {
            var tableAlias = GetTableAlias(tableName);
            if (string.IsNullOrEmpty(alias) || fieldName == alias)
                SelectionList.Add($"{ Adapter.IsNull() }({ Adapter.Field(tableAlias, fieldName) }, { nullValue.SafeValue(Adapter.CurrentDate()) }) { Adapter.Alias(fieldName) }");
            else
                SelectionList.Add($"{ Adapter.IsNull() }({ Adapter.Field(tableAlias, fieldName) }, { nullValue.SafeValue(Adapter.CurrentDate()) }) { Adapter.Alias(alias) }");
        }

        public void SelectIsNull(string tableName, string fieldName, string alias, string nullValue, string nullTableName)
        {
            var tableAlias = GetTableAlias(tableName);
            if (string.IsNullOrEmpty(alias) || fieldName == alias)
                SelectionList.Add($"{ Adapter.IsNull() }({ Adapter.Field(tableAlias, fieldName) }, { Adapter.Field(nullTableName, nullValue) }) { Adapter.Alias(fieldName) }");
            else
                SelectionList.Add($"{ Adapter.IsNull() }({ Adapter.Field(tableAlias, fieldName) }, { Adapter.Field(nullTableName, nullValue) }) { Adapter.Alias(alias) }");
        }

        public void SelectConcatSql(string tableName, string fieldName, string alias, IEnumerable<string> values)
        {
            var tableAlias = GetTableAlias(tableName);
            if (string.IsNullOrEmpty(alias) || fieldName == alias)
                SelectionList.Add($"{ Adapter.Concat() }({ Adapter.Field(tableAlias, fieldName) }, { string.Join(", ", values) }) { Adapter.Alias(fieldName) }");
            else
                SelectionList.Add($"{ Adapter.Concat() }({ Adapter.Field(tableAlias, fieldName) }, { string.Join(", ", values) }) { Adapter.Alias(alias) }");
        }

        public void SelectDatePartSql(string tableName, string fieldName, string alias, DatePart datePart)
        {
            var tableAlias = GetTableAlias(tableName);
            if (string.IsNullOrEmpty(alias) || fieldName == alias)
                SelectionList.Add($"{ Adapter.DatePart(Adapter.Field(tableAlias, fieldName), datePart) } { Adapter.Alias(fieldName) }");
            else
                SelectionList.Add($"{ Adapter.DatePart(Adapter.Field(tableAlias, fieldName), datePart) } { Adapter.Alias(alias) }");
        }

        public void Select(string tableName, string fieldName, SelectFunctionType selectFunction)
        {
            var selectionString = string.Empty;
            var tableAlias = GetTableAlias(tableName);
            if (selectFunction == SelectFunctionType.CUSTOM)
            {
                selectionString = Adapter.Field(fieldName);
            }
            else
                selectionString = $"{selectFunction}({Adapter.Field(tableAlias, fieldName)})";

            SelectionList.Add(selectionString);
        }

        public void Select(string tableName, string fieldName, string alias, SelectFunctionType selectFunction)
        {
            var tableAlias = GetTableAlias(tableName);
            if (string.IsNullOrEmpty(alias) || fieldName == alias)
                Select(tableAlias, fieldName, selectFunction);
            else
            {
                var selectionString = $"{selectFunction}({Adapter.Field(tableAlias, fieldName)}) {Adapter.Alias(alias)}";
                SelectionList.Add(selectionString);
            }
        }

        public void Select(SelectFunctionType selectFunction)
        {
            var selectionString = string.Empty;
            if (selectFunction == SelectFunctionType.CUSTOM)
                selectionString = $"*";
            else
                selectionString = $"{selectFunction}(*)";

            SelectionList.Add(selectionString);
        }

        public void SelectCase(string commandText, string alias)
        {
            if (!commandText.EndsWith("END"))
                throw new Exception($"{alias}: Case command is not completed");
            SelectionList.Add($"{ commandText } {Adapter.Alias(alias)}");
        }

        public void GroupBy(string tableName, string fieldName)
        {
            GroupByList.Add(Adapter.Field(GetTableAlias(tableName), fieldName));
        }

        public void SkipPages(int skipPages)
        {
            _pageIndex = skipPages;
        }

        public void Take(int pageSize)
        {
            _pageSize = pageSize;
        }

        private string GenerateQueryCommand()
        {
            if (!_pageSize.HasValue || _pageSize == 0)
                return Adapter.QueryString(Selection, Source, Conditions, Grouping, Having, Order);

            if (_pageIndex > 0 && OrderByList.Count == 0)
                throw new Exception("Pagination requires the ORDER BY statement to be specified");

            return Adapter.QueryStringPage(Selection, Source, Conditions, Order, _pageSize.Value, _pageIndex);
        }

        private string GenerateWhereCommand()
        {
            var fields = WhereConditions.Count == 0 ? "" : string.Join("", WhereConditions);
            return Adapter.WhereCommand(fields);
        }

        private string GenerateSubQueryCommand()
        {
            foreach (var ndc in SubQuery.CommandParameters)
                AddParameter(ndc.Key, ndc.Value);

            if (!_pageSize.HasValue || _pageSize == 0)
                return Adapter.SubQueryString(SubQuery.CommandText, Selection, Source, Conditions, Grouping, Having, Order);

            if (_pageIndex > 0 && OrderByList.Count == 0)
                throw new Exception("Pagination requires the ORDER BY statement to be specified");

            return Adapter.SubQueryStringPage(SubQuery.CommandText, Selection, Source, Conditions, Order, _pageSize.Value, _pageIndex);
        }
    }
}
