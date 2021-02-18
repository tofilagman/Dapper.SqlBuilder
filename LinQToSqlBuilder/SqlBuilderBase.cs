﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Dapper.SqlBuilder.Adapter;
using Dapper.SqlBuilder.Builder;
using Dapper.SqlBuilder.Resolver;
using Dapper.SqlBuilder.ValueObjects;

namespace Dapper.SqlBuilder
{
    /// <summary>
    /// Represents the basic operations / properties to generate the SQL queries
    /// </summary>
    public abstract class SqlBuilderBase
    {
        internal static ISqlAdapter DefaultAdapter;
        internal SqlQueryBuilder Builder;
        internal LambdaResolver Resolver;

        internal SqlOperations Operation
        {
            get => Builder.Operation;
            set => Builder.Operation = value;
        }

        internal SqlQueryBuilder SqlBuilder => Builder;

        public string CommandText => Regex.Replace(Builder.CommandText, "\\s+", " ").Trim();

        public IDictionary<string, object> CommandParameters => Builder.Parameters;

        public string[] SplitColumns => Builder.SplitColumns.ToArray();

        public static void SetAdapter(ISqlAdapter adapter)
        {
            DefaultAdapter = adapter ?? new SqlServerAdapter();
        }
    }

}
