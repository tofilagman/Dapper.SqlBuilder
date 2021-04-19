﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Dapper.SqlBuilder.Adapter
{
    /// <summary>
    /// Generates the SQL queries that are compatible to all supported databases
    /// </summary>
    public class SqlAdapterBase
    {
        public string QueryString(string selection,
                                  string source,
                                  string conditions,
                                  string order = "",
                                  string grouping = "",
                                  string having = "")
        {
            return $"SELECT {selection} FROM {source} {conditions} {order} {grouping} {having}"
               .Trim();
        }

        public string SubQueryString(string subQuery,
                                  string selection,
                                  string source,
                                  string conditions,
                                  string order = "",
                                  string grouping = "",
                                  string having = "")
        {
            return $"SELECT {selection} FROM (\r\n {subQuery} \r\n) {source} {conditions} {order} {grouping} {having}"
               .Trim();
        }

        public virtual string InsertCommand(string target, List<Dictionary<string, object>> values, string output = "")
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
                (
                    !string.IsNullOrEmpty(output)
                        ? $"OUTPUT Inserted.{output} "
                        : string.Empty
                ) +
                $"VALUES ({string.Join("), (", valuesToInsert)})"
                   .Trim();
        }

        public virtual string InsertFromCommand(string target, string source, List<Dictionary<string, object>> values, string conditions)
        {
            var fieldsToInsert = values.First()
                                       .Select(rowValue => rowValue.Key)
                                       .ToList();

            var valuesToInsert = new List<string>();

            foreach (var rowValue in values)
            {
                valuesToInsert.Add(string.Join(", ", rowValue.Select(_ => _.Value + " as " + _.Key)));
            }

            return
                $"INSERT INTO {target} ({string.Join(", ", fieldsToInsert)}) " +
                $"SELECT {string.Join(", ", valuesToInsert)} " +
                $"FROM {source} " +
                $"{conditions}"
               .Trim();
        }

        public string UpdateCommand(string updates, string source, string sourceAlias, string conditions)
        {
            return $"UPDATE {sourceAlias} " +
                   $"SET {updates} " +
                   $"FROM {source} " +
                   $"{conditions}"
               .Trim();
        }

        public string DeleteCommand(string source, string sourceAlias, string conditions)
        {
            return $"DELETE { sourceAlias } FROM {source} " +
                   $"{conditions}"
               .Trim();
        }

        public string WhereCommand(string conditions)
        {
            return $"{conditions}".Trim();
        }
    }
}