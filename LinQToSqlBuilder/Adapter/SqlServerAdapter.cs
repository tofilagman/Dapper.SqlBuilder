﻿namespace Dapper.SqlBuilder.Adapter
{
    public class SqlServerAdapter : SqlServerAdapterBase, ISqlAdapter
    {
        public string Concat()
        {
            return "CONCAT";
        }

        public string CurrentDate()
        {
            return "GETDATE()";
        }

        public string Format()
        {
            return "FORMAT";
        }

        public string IsNull()
        {
            return "ISNULL";
        }

        public string QueryStringPage(string selection, string source, string conditions, string order,
                                      int pageSize, int pageIndex = 0)
        {
            if (pageIndex == 0)
                return $"SELECT TOP({pageSize}) {selection} FROM {source} {conditions} {order}";

            return
                $@"SELECT {selection} FROM {source} {conditions} {order} OFFSET {pageSize * pageIndex} ROWS FETCH NEXT {pageSize} ROWS ONLY";
        }
    }
}