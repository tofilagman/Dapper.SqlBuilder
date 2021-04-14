using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.SqlBuilder.Builder
{
    internal partial class SqlQueryBuilder
    {
        public void Case()
        {
            if (SelectionList.Count > 0)
                throw new Exception("Case should not be called twice");

            WhereConditions.Add(" CASE ");
        }

        public void When()
        {
            WhereConditions.Add(" WHEN ");
        }

        public void Then()
        {
            WhereConditions.Add(" THEN ");
        }

        public void Else()
        {
            WhereConditions.Add(" ELSE ");
        }

        public void End()
        {
            WhereConditions.Add(" END ");
        }
         
    }
}
