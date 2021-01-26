using System.Collections.Generic;

namespace Dapper.SqlBuilder.Adapter
{
    /// <summary>
    /// SQL adapter provides db specific functionality related to db specific SQL syntax
    /// </summary>
    public interface ISqlAdapter
    {
        string QueryString(string selection, string source, string conditions, 
            string order, string grouping, string having);

        string QueryStringPage(string selection, string source, string conditions, string order,
            int pageSize, int pageIndex = 0);
         
        string Table(string tableName);
        
        string Field(string fieldName);

        string Field(string tableName, string fieldName);

        string Parameter(string parameterId);
    }

    enum SqlOperations
    {
        Query
    }
}
