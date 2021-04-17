using System;
using System.Text;
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

            Assert.AreEqual("UPDATE Users " +
                            "SET " +
                            "[Email] = REPLACE([Email], @Param1, @Param2), " +
                            "[LastChangePassword] = @Param3, " +
                            "[FailedLogIns] = [FailedLogIns] - @Param4 " +
                            "WHERE Users.[Id] = @Param5",
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

            Assert.AreEqual("UPDATE userlog SET [DateSlide] = @Param1 WHERE userlog.[Guid] = @Param2", query.CommandText);
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

            Assert.AreEqual("UPDATE UsersGroup SET [CreatedBy] = @Param1, [CreatedDate] = @Param2, [Description] = @Param3, [Name] = @Param4, [ID_FilingStatus] = @Param5, [IsDeleted] = @Param6, [IsUndeletable] = @Param7",
                            query.CommandText);
            Assert.AreEqual(7, query.CommandParameters.Count);
        }

        [Test]
        public void UpdateManyRecords()
        {
            var nlst = new[]
            {
                new UserGroup{ Id  =1, ID_FilingStatus = FilingStatus.Filed, CreatedBy = "test" },
                new UserGroup{ Id  =2, ID_FilingStatus = FilingStatus.Approved, CreatedBy = "test2" },
                new UserGroup{ Id  =3, ID_FilingStatus = FilingStatus.Cancelled, CreatedBy = "test3" },
                new UserGroup{ Id  =4, ID_FilingStatus = FilingStatus.DisApproved, CreatedBy = "test4" },
                new UserGroup{ Id  =5, ID_FilingStatus = FilingStatus.Cancelled, CreatedBy = "test5" },
                new UserGroup{ Id  =5, ID_FilingStatus = FilingStatus.Filed, CreatedBy = "test6" },
            };

            var query = SqlBuilder.Many<UserGroup>();
            foreach (var nd in nlst)
            {
                query.Update(x => new UserGroup { ID_FilingStatus = nd.ID_FilingStatus, CreatedBy = nd.CreatedBy }).Where(y => y.Id == nd.Id);
            }

            var result = new StringBuilder();
            result.AppendLine("UPDATE UsersGroup SET [ID_FilingStatus] = @Param1, [CreatedBy] = @Param2 WHERE UsersGroup.[Id] = @Param3");
            result.AppendLine("UPDATE UsersGroup SET [ID_FilingStatus] = @Param4, [CreatedBy] = @Param5 WHERE UsersGroup.[Id] = @Param6");
            result.AppendLine("UPDATE UsersGroup SET [ID_FilingStatus] = @Param7, [CreatedBy] = @Param8 WHERE UsersGroup.[Id] = @Param9");
            result.AppendLine("UPDATE UsersGroup SET [ID_FilingStatus] = @Param10, [CreatedBy] = @Param11 WHERE UsersGroup.[Id] = @Param12");
            result.AppendLine("UPDATE UsersGroup SET [ID_FilingStatus] = @Param13, [CreatedBy] = @Param14 WHERE UsersGroup.[Id] = @Param15");
            result.Append("UPDATE UsersGroup SET [ID_FilingStatus] = @Param16, [CreatedBy] = @Param17 WHERE UsersGroup.[Id] = @Param18");

            Assert.AreEqual(result.ToString(), query.CommandText);
            Assert.AreEqual(18, query.CommandParameters.Count);

        }
    }
}