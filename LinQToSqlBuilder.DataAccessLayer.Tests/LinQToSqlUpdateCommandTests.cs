using System;
using Dapper.SqlBuilder;
using Dapper.SqlBuilder.Adapter;
using LinQToSqlBuilder.DataAccessLayer.Tests.Entities;
using NUnit.Framework;

namespace LinQToSqlBuilder.DataAccessLayer.Tests
{
    public class LinQToSqlUpdateCommandTests
    {
        [SetUp]
        public void Setup()
        {
            SqlBuilder.SetAdapter(new SqlServerAdapter());
        }

        [Test]
        public void UpdateByFieldValue()
        {
            //using var scope = new TransactionScope();
            var userEmail = "user@email.com";
            var userId = 5;
            var query = SqlBuilder.Update<User>(_ => new User
            {
                Email =
                                           _.Email.Replace("oldemail@domain.com", "newEmail@domain.com"),
                LastChangePassword = DateTimeOffset.Now.AddDays(-1),
                FailedLogIns = _.FailedLogIns - 2
            })
                                  .Where(user => user.Id == userId);

            Assert.AreEqual("UPDATE [Users] " +
                            "SET " +
                            "[Email] = REPLACE([Email], @Param1, @Param2), " +
                            "[LastChangePassword] = @Param3, " +
                            "[FailedLogIns] = [FailedLogIns] - @Param4 " +
                            "WHERE [Users].[Id] = @Param5",
                            query.CommandText);
        }

        [Test]
        public void UpdateByCustomObject()
        {
            var fdg = DateTime.Now;
             
            var query = SqlBuilder.Update<UserLog>(x => new UserLog
            {
                DateSlide = fdg
            })
              .Where(x => x.Guid == Guid.NewGuid());

            Assert.AreEqual("UPDATE [userlog] SET [DateSlide] = @Param1 WHERE [userlog].[Guid] = @Param2", query.CommandText);
        }

        [Test]
        public void UpdateSingleRecord()
        {
            var undelete = true; 
            var query = SqlBuilder.Update<UserGroup>(_ => new UserGroup
            {
                CreatedBy = "TestSystem",
                CreatedDate = DateTimeOffset.Now,
                Description = "Created from Test System",
                Name = "TestUserGroup",
                ID_FilingStatus = FilingStatus.Approved,
                IsDeleted = false,
                IsUndeletable = !undelete
            });

            Assert.AreEqual("UPDATE [UsersGroup] SET [CreatedBy] = @Param1, [CreatedDate] = @Param2, [Description] = @Param3, [Name] = @Param4, [ID_FilingStatus] = @Param5, [IsDeleted] = @Param6, [IsUndeletable] = @Param7",
                            query.CommandText);
            Assert.AreEqual(7, query.CommandParameters.Count);
        }
    }
}