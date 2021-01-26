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
            if (pageIndex == 0)
                return $"SELECT {selection} FROM {source} {conditions} {order}";

            return
                $@"SELECT {selection} FROM {source} {conditions} {order}  LIMIT {pageIndex}, {pageSize}"; // OFFSET {pageSize * pageIndex} ROWS FETCH NEXT {pageSize} ROWS ONLY";
        }

        public string Table(string tableName)
        {
            return tableName;
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
                        ? $"SELECT LAST_INSERT_ID() "
                        : string.Empty
                )
                   .Trim();
        }
    }
}
