using System;
using System.Collections.Generic;
using Dapper.SqlBuilder.Adapter;
using Dapper.SqlBuilder.Extensions;
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
            var joinString =
                $"{join} {Adapter.Table(joinTableName)} " +
                $"ON {Adapter.Field(originalTableName, leftField)} = {Adapter.Field(joinTableName, rightField)}";

            TableNames.Add(joinTableName);
            JoinExpressions.Add(joinString);
            SplitColumns.Add(rightField);
        }

        public void Join(string joinTableName, string commandText, JoinType joinType)
        {
            var join = GetJoinExpression(joinType);
            var joinString =
                $"{join} {Adapter.Table(joinTableName)} " +
                $"ON {commandText}";

            TableNames.Add(joinTableName);
            JoinExpressions.Add(joinString);
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
            var order = Adapter.Field(tableName, fieldName);
            if (desc)
                order += " DESC";

            OrderByList.Add(order);
        }

        public void Select(string tableName)
        {
            var selectionString = $"{Adapter.Table(tableName)}.*";
            SelectionList.Add(selectionString);
        }

        public void Select(string tableName, string fieldName)
        {
            SelectionList.Add(Adapter.Field(tableName, fieldName));
        }

        public void Select(string tablename, string fieldName, string alias)
        {
            if (string.IsNullOrEmpty(alias) || fieldName == alias)
                Select(tablename, fieldName);
            else
                SelectionList.Add($"{ Adapter.Field(tablename, fieldName) } { Adapter.Alias(alias) }");
        }

        public void SelectFormat(string tableName, string fieldName, string alias, string format)
        {
            if (string.IsNullOrEmpty(alias) || fieldName == alias)
                SelectionList.Add($"{ Adapter.Format() }({ Adapter.Field(tableName, fieldName) }, '{ format }') { Adapter.Alias(fieldName) }");
            else
                SelectionList.Add($"{ Adapter.Format() }({ Adapter.Field(tableName, fieldName) }, '{ format }') { Adapter.Alias(alias) }");
        }

        public void SelectIsNull(string tableName, string fieldName, string alias, object nullValue)
        {
            if (string.IsNullOrEmpty(alias) || fieldName == alias)
                SelectionList.Add($"{ Adapter.IsNull() }({ Adapter.Field(tableName, fieldName) }, { nullValue.SafeValue(Adapter.CurrentDate()) }) { Adapter.Alias(fieldName) }");
            else
                SelectionList.Add($"{ Adapter.IsNull() }({ Adapter.Field(tableName, fieldName) }, { nullValue.SafeValue(Adapter.CurrentDate()) }) { Adapter.Alias(alias) }");
        }

        public void SelectIsNull(string tableName, string fieldName, string alias, string nullValue, string nullTableName)
        {
            if (string.IsNullOrEmpty(alias) || fieldName == alias)
                SelectionList.Add($"{ Adapter.IsNull() }({ Adapter.Field(tableName, fieldName) }, { Adapter.Field(nullTableName, nullValue) }) { Adapter.Alias(fieldName) }");
            else
                SelectionList.Add($"{ Adapter.IsNull() }({ Adapter.Field(tableName, fieldName) }, { Adapter.Field(nullTableName, nullValue) }) { Adapter.Alias(alias) }");
        }

        public void SelectConcatSql(string tableName, string fieldName, string alias, IEnumerable<string> values)
        {
            if (string.IsNullOrEmpty(alias) || fieldName == alias)
                SelectionList.Add($"{ Adapter.Concat() }({ Adapter.Field(tableName, fieldName) }, { string.Join(", ", values) }) { Adapter.Alias(fieldName) }");
            else
                SelectionList.Add($"{ Adapter.Concat() }({ Adapter.Field(tableName, fieldName) }, { string.Join(", ", values) }) { Adapter.Alias(alias) }");
        }

        public void Select(string tableName, string fieldName, SelectFunction selectFunction)
        {
            var selectionString = $"{selectFunction}({Adapter.Field(tableName, fieldName)})";
            SelectionList.Add(selectionString);
        }

        public void Select(string tableName, string fieldName, string alias, SelectFunction selectFunction)
        {
            if (string.IsNullOrEmpty(alias) || fieldName == alias)
                Select(tableName, fieldName, selectFunction);
            else
            {
                var selectionString = $"{selectFunction}({Adapter.Field(tableName, fieldName)}) {Adapter.Alias(alias)}";
                SelectionList.Add(selectionString);
            }
        }

        public void Select(SelectFunction selectFunction)
        {
            var selectionString = $"{selectFunction}(*)";
            SelectionList.Add(selectionString);
        }

        public void GroupBy(string tableName, string fieldName)
        {
            GroupByList.Add(Adapter.Field(tableName, fieldName));
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

            return Adapter.QueryStringPage(Source, Selection, Conditions, Order, _pageSize.Value, _pageIndex);
        }

        private string GenerateWhereCommand()
        {
            var fields = WhereConditions.Count == 0 ? "" : string.Join("", WhereConditions);
            return Adapter.WhereCommand(fields);
        }
    }
}
