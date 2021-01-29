using Dapper.SqlBuilder;
using Dapper.SqlBuilder.Adapter;
using LinQToSqlBuilder.DataAccessLayer.Tests.Entities;
using NUnit.Framework;

namespace LinQToSqlBuilder.DataAccessLayer.Tests
{
    [TestFixture]
    public class LinQToSqlDeleteCommandTests
    {
        [SetUp]
        public void Setup()
        {
            SqlBuilder.SetAdapter(new SqlServerAdapter());
        }

        [Test]
        public void DeleteByFieldValue()
        {
            var query = SqlBuilder.Delete<CloneUserGroup>(_ => _.IsDeleted);


            Assert.AreEqual(query.CommandText,
                            "DELETE FROM [CloneUserGroup] WHERE [CloneUserGroup].[IsDeleted] = @Param1");
        }
    }
}