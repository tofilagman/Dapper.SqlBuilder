using System;
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
    }
}