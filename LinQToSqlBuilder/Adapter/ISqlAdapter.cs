using Dapper.SqlBuilder.ValueObjects;
using System.Collections.Generic;

namespace Dapper.SqlBuilder.Adapter
{
    /// <summary>
    /// SQL adapter provides db specific functionality related to db specific SQL syntax
    /// </summary>
    public interface ISqlAdapter
    {
        string QueryString(string selection, string source, string conditions, string order, string grouping, string having);

        string QueryStringPage(string selection, string source, string conditions, string order, int pageSize, int pageIndex = 0);

        string SubQueryString(string subQuery, string selection, string source, string conditions, string order, string grouping, string having);

        string SubQueryStringPage(string subQuery, string selection, string source, string conditions, string order, int pageSize, int pageIndex = 0);

        string InsertCommand(string source, List<Dictionary<string, object>> values, string output = "");

        string InsertFromCommand(string target, string source, List<Dictionary<string, object>> values,
                                 string conditions);

        string UpdateCommand(string updates, string source,  string sourceAlias, string conditions);

        string DeleteCommand(string source, string sourceAlias, string conditions);

        string Table(string tableName);

        string Table(string tableName, string alias);

        string Field(string fieldName);

        string Field(string tableName, string fieldName);

        string Parameter(string parameterId);

        string WhereCommand(string conditions);

        string Alias(string alias);

        string CurrentDate();
        string IsNull();
        string Format();
        string Concat();

        string DatePart(string column, DatePart datePart);
    }

    enum SqlOperations
    {
        Query,
        Insert,
        InsertFrom,
        Update,
        Delete,
        Case,
        SubQuery
    }

    enum JoinType
    {
        InnerJoin,
        LeftJoin,
        LeftOuterJoin,
        RightJoin,
        RightOuterJoin,
        FullJoin,
        FullOuterJoin,
        CrossJoin
    }
}
