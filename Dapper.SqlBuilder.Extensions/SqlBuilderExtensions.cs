using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.SqlBuilder.Extensions
{
    public static class SqlBuilderExtensions
    {
        public static async Task DeleteTableAsync<T>(this IDbConnection con, IDbTransaction tran = null)
        {
            var delSql = SqlBuilder.Delete<T>(x => 1 == 1);
            await con.ExecuteAsync(delSql.CommandText, transaction: tran);
        }

        public static async Task<int> CountAsync<T>(this IDbConnection con, IDbTransaction tran = null)
        {
            var cntSQL = SqlBuilder.Count<T>();
            return await con.QuerySingleAsync<int>(cntSQL.CommandText, transaction: tran);
        }

        public static IEnumerable<T> Query<T>(this SqlBuilder<T> sqlBuilder, IDbConnection con, IDbTransaction tran = null, bool buffered = true, int? timeOut = null)
        {
            return con.Query<T>(sqlBuilder.CommandText, sqlBuilder.CommandParameters, tran, buffered, timeOut);
        }

        public static async Task<IEnumerable<T>> QueryAsync<T>(this SqlBuilder<T> sqlBuilder, IDbConnection con, IDbTransaction tran = null, int? timeOut = null)
        {
            return await con.QueryAsync<T>(sqlBuilder.CommandText, sqlBuilder.CommandParameters, tran, timeOut);
        }

        public static async Task<T> QuerySingleAsync<T>(this SqlBuilder<T> sqlBuilder, IDbConnection con, IDbTransaction tran = null, int? timeOut = null)
        {
            return await con.QuerySingleAsync<T>(sqlBuilder.CommandText, sqlBuilder.CommandParameters, tran, timeOut);
        }

        public static T QuerySingleOrDefault<T>(this SqlBuilder<T> sqlBuilder, IDbConnection con, IDbTransaction tran = null, int? timeOut = null)
        {
            return con.QuerySingleOrDefault<T>(sqlBuilder.CommandText, sqlBuilder.CommandParameters, tran, timeOut);
        }

        public static async Task<T> QuerySingleOrDefaultAsync<T>(this SqlBuilder<T> sqlBuilder, IDbConnection con, IDbTransaction tran = null, int? timeOut = null)
        {
            return await con.QuerySingleOrDefaultAsync<T>(sqlBuilder.CommandText, sqlBuilder.CommandParameters, tran, timeOut);
        }

        public static int Execute<T>(this SqlBuilder<T> sqlBuilder, IDbConnection con, IDbTransaction tran = null, int? timeOut = null)
        {
            return con.Execute(sqlBuilder.CommandText, sqlBuilder.CommandParameters, tran, timeOut);
        }

        public static async Task<int> ExecuteAsync<T>(this SqlBuilder<T> sqlBuilder, IDbConnection con, IDbTransaction tran = null, int? timeOut = null)
        {
            return await con.ExecuteAsync(sqlBuilder.CommandText, sqlBuilder.CommandParameters, tran, timeOut);
        }

        public static int QueryIdentityInt<T>(this SqlBuilder<T> sqlBuilder, IDbConnection con, IDbTransaction tran = null, int? timeOut = null)
        {
            var iden = sqlBuilder.OutputIdentity();
            return con.QuerySingle<int>(iden.CommandText, iden.CommandParameters, tran, timeOut);
        }

        public static async Task<int> QueryIdentityIntAsync<T>(this SqlBuilder<T> sqlBuilder, IDbConnection con, IDbTransaction tran = null, int? timeOut = null)
        {
            var iden = sqlBuilder.OutputIdentity();
            return await con.QuerySingleAsync<int>(iden.CommandText, iden.CommandParameters, tran, timeOut);
        }

        public static long QueryIdentityLong<T>(this SqlBuilder<T> sqlBuilder, IDbConnection con, IDbTransaction tran = null, int? timeOut = null)
        {
            var iden = sqlBuilder.OutputIdentity();
            return con.QuerySingle<long>(iden.CommandText, iden.CommandParameters, tran, timeOut);
        }

        public static async Task<long> QueryIdentityLongAsync<T>(this SqlBuilder<T> sqlBuilder, IDbConnection con, IDbTransaction tran = null, int? timeOut = null)
        {
            var iden = sqlBuilder.OutputIdentity();
            return await con.QuerySingleAsync<long>(iden.CommandText, iden.CommandParameters, tran, timeOut);
        }

    }
}
