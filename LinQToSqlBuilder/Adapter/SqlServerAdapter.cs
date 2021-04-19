using Dapper.SqlBuilder.ValueObjects;

namespace Dapper.SqlBuilder.Adapter
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

        public string DatePart(string column, DatePart datePart)
        {
            return $"DATEPART({ datePart }, { column })";
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
         
        public string SubQueryStringPage(string subQuery, string selection, string source, string conditions, string order, int pageSize, int pageIndex = 0)
        {
            if (pageIndex == 0)
                return $"SELECT TOP({pageSize}) {selection} FROM (\r\n {subQuery} \r\n) {source} {conditions} {order}";

            return
                $"SELECT {selection} FROM (\r\n {subQuery} \r\n) {source} {conditions} {order} OFFSET {pageSize * pageIndex} ROWS FETCH NEXT {pageSize} ROWS ONLY";
        }
    }
}