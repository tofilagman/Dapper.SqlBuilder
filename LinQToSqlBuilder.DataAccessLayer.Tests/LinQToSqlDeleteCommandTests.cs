using Dapper.SqlBuilder;
using Dapper.SqlBuilder.Adapter;
using LinQToSqlBuilder.DataAccessLayer.Tests.Entities;
using NUnit.Framework;
using System;

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
                            "DELETE FROM CloneUserGroup WHERE CloneUserGroup.[IsDeleted] = @Param1");
        }

        [Test, Ignore("For Further Testing")]
        public void DeleteByInvalidExpression()
        { 
            Assert.Throws<ArgumentException>(() => {
                SqlBuilder.Delete<PermissionGroup>(x => 1 == 1);
            });
        }

        [Test]
        public void DeleteAll()
        {
            var query = SqlBuilder.Delete<PermissionGroup>();
            Assert.AreEqual(query.CommandText, "DELETE FROM permissiongroups");
            Assert.IsTrue(query.CommandParameters.Count == 0);
        }
    }
}