using Dapper.SqlBuilder.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dapper.SqlBuilder.Adapter
{
    public class MySqlAdapter : SqlAdapterBase, ISqlAdapter
    {
        public string Field(string fieldName)
        {
            return fieldName;
        }

        public string Field(string tableName, string fieldName)
        {
            return $"{tableName}.{fieldName}";
        }

        public string Parameter(string parameterId)
        {
            return "@" + parameterId;
        }

        public string QueryStringPage(string selection, string source, string conditions, string order, int pageSize, int pageIndex = 0)
        {
            if (pageIndex == 0 && pageSize > 0)
                return $"SELECT {selection} FROM {source} {conditions} {order} LIMIT {pageSize}";

            return
                $"SELECT {selection} FROM {source} {conditions} {order}  LIMIT {pageIndex}, {pageSize}";
        }

        public string SubQueryStringPage(string subQuery, string selection, string source, string conditions, string order, int pageSize, int pageIndex = 0)
        {
            if (pageIndex == 0 && pageSize > 0)
                return $"SELECT {selection} FROM (\r\n {subQuery} \r\n) {source} {conditions} {order} LIMIT {pageSize}";

            return
                $"SELECT {selection} FROM (\r\n {subQuery} \r\n) {source} {conditions} {order}  LIMIT {pageIndex}, {pageSize}";
        }

        public string Table(string tableName)
        {
            return tableName;
        }

        public string Table(string tableName, string alias)
        {
            return $"{tableName} {alias}";
        }

        public override string InsertCommand(string target, List<Dictionary<string, object>> values, string output = "")
        {
            var fieldsToInsert = values.First()
                                       .Select(rowValue => rowValue.Key)
                                       .ToList();
            var valuesToInsert = new List<string>();
            foreach (var rowValue in values)
            {
                valuesToInsert.Add(string.Join(", ", rowValue.Select(_ => _.Value)));
            }

            return
                $"INSERT INTO {target} ({string.Join(", ", fieldsToInsert)}) " +
                $"VALUES ({string.Join("), (", valuesToInsert)}) " +
                (
                    !string.IsNullOrEmpty(output)
                        ? $"; SELECT LAST_INSERT_ID(); "
                        : string.Empty
                )
                   .Trim();
        }

        public string Alias(string alias)
        {
            return alias;
        }

        public string CurrentDate()
        {
            return "CURRENT_TIMESTAMP";
        }

        public string IsNull()
        {
            return "IFNULL";
        }

        public string Format()
        {
            return "DATE_FORMAT";
        }

        public string Concat()
        {
            return "CONCAT";
        }

        public string DatePart(string column, DatePart datePart)
        {
            return datePart switch
            {
                ValueObjects.DatePart.YEAR => $"YEAR({ column })",
                ValueObjects.DatePart.MONTH => $"MONTH({ column })",
                ValueObjects.DatePart.DAY => $"DAY({ column })",
                ValueObjects.DatePart.DAYOFYEAR => $"DAYOFYEAR({ column })",
                ValueObjects.DatePart.HOUR => $"HOUR({ column })",
                ValueObjects.DatePart.MINUTE => $"MINUTE({ column })",
                ValueObjects.DatePart.SECOND => $"SECOND({ column })",
                ValueObjects.DatePart.MILLISECOND => $"MILLISECOND({ column })",
                ValueObjects.DatePart.MICROSECOND => $"MICROSECOND({ column })",
                ValueObjects.DatePart.WEEK => $"WEEK({ column })",
                ValueObjects.DatePart.WEEKDAY => $"WEEKDAY({ column })",
                _ => throw new InvalidOperationException("Specified DatePart is not supported")
            };
        }
    }
}
