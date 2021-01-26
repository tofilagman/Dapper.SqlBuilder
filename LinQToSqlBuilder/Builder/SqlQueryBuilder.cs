﻿/* License: http://www.apache.org/licenses/LICENSE-2.0 */

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using Dapper.SqlBuilder.Adapter;

namespace Dapper.SqlBuilder.Builder
{
    /// <summary>
    /// Provides methods to build up SQL query, adding up parameters and conditions to the query and generate the final SQL statement
    /// </summary>
    internal partial class SqlQueryBuilder
    {
        internal ISqlAdapter Adapter { get; set; }

        internal SqlOperations Operation { get; set; } = SqlOperations.Query;

        private const string ParameterPrefix = "Param";

        private readonly List<string> _updateValues = new List<string>();

        private List<string> TableNames { get; } = new List<string>();
        private List<string> JoinExpressions { get; } = new List<string>();
        private List<string> SelectionList { get; } = new List<string>();
        private List<string> WhereConditions { get; } = new List<string>();
        private List<string> OrderByList { get; } = new List<string>();
        private List<string> GroupByList { get; } = new List<string>();
        private List<string> HavingConditions { get; } = new List<string>();
        internal List<string> SplitColumns { get; } = new List<string>();

        private int? _pageSize;

        private int _pageIndex;

        public int CurrentParamIndex { get; private set; }

        private string Source
        {
            get
            {
                var joinExpression = string.Join(" ", JoinExpressions);
                return $"{Adapter.Table(TableNames.First())} {joinExpression}";
            }
        }

        private string Selection
        {
            get
            {
                if (SelectionList.Count == 0)
                {
                    if (!JoinExpressions.Any())
                        return $"{Adapter.Table(TableNames.First())}.*";

                    var joinTables = TableNames.Select(_ => $"{Adapter.Table(_)}.*");

                    var selection = string.Join(", ", joinTables);

                    return selection;
                }

                return string.Join(", ", SelectionList);
            }
        }

        private string Conditions => WhereConditions.Count == 0 ? "" : "WHERE " + string.Join("", WhereConditions);

        private string UpdateValues => string.Join(", ", _updateValues);

        private string _insertOutput { get; set; } = "";

        private List<Dictionary<string, object>> InsertValues { get; } = new List<Dictionary<string, object>>();

        private string Order => OrderByList.Count == 0 ? "" : "ORDER BY " + string.Join(", ", OrderByList);

        private string Grouping => GroupByList.Count == 0 ? "" : "GROUP BY " + string.Join(", ", GroupByList);

        private string Having => HavingConditions.Count == 0 ? "" : "HAVING " + string.Join(" ", HavingConditions);

        public IDictionary<string, object> Parameters { get; private set; }

        public string CommandText
        {
            get
            { 
                return GenerateQueryCommand(); 
            }
        }

        internal SqlQueryBuilder(string tableName, ISqlAdapter adapter)
        {
            TableNames.Add(tableName);
            Adapter = adapter;
            Parameters = new ExpandoObject();
            CurrentParamIndex = 0;
        }

        #region helpers
        private string NextParamId()
        {
            ++CurrentParamIndex;
            return ParameterPrefix + CurrentParamIndex.ToString(CultureInfo.InvariantCulture);
        }

        private void AddParameter(string key, object value)
        {
            if (!Parameters.ContainsKey(key))
                Parameters.Add(key, value);
        }
        #endregion
    }
}
