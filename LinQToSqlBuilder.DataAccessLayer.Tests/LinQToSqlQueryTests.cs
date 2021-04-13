using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper.SqlBuilder;
using Dapper.SqlBuilder.Adapter;
using Dapper.SqlBuilder.Extensions;
using LinQToSqlBuilder.DataAccessLayer.Tests.Base;
using LinQToSqlBuilder.DataAccessLayer.Tests.Entities;
using NUnit.Framework;

namespace LinQToSqlBuilder.DataAccessLayer.Tests
{
    [TestFixture]
    public class LinQToSqlQueryTests : TestBase
    {
        [SetUp]
        public void Setup()
        {
            SqlBuilder.SetAdapter(new SqlServerAdapter());
        }

        [Test]
        public void QueryCount()
        {
            var query = SqlBuilder.Count<User>(_ => _.Id)
                                  .Where(_ => _.Id > 10);

            Assert.AreEqual($"SELECT COUNT([Users].[Id]) FROM [Users] " +
                            $"WHERE [Users].[Id] > @Param1",
                            query.CommandText);

            query = SqlBuilder.Count<User>()
                                  .Where(_ => _.Id > 10);

            Assert.AreEqual($"SELECT COUNT(*) FROM [Users] " +
                            $"WHERE [Users].[Id] > @Param1",
                            query.CommandText);
        }

        [Test]
        public void QueryWithPagination()
        {
            var query = SqlBuilder.Select<User>()
                                  .OrderBy(_ => _.Id)
                                  .Take(10);

            Assert.AreEqual($"SELECT TOP(10) [Users].* FROM [Users] ORDER BY [Users].[Id]",
                            query.CommandText);
        }

        [Test]
        public void QueryFieldsWithPagination()
        {
            var query = SqlBuilder.Select<User, UserViewModel>(user => new UserViewModel
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Id = user.Id
            })
                                  .Where(_ => !_.RecordDeleted)
                                  .OrderBy(_ => _.Id)
                                  .Take(10);

            Assert.AreEqual($"SELECT TOP(10) [Users].[Email], " +
                            $"[Users].[FirstName], " +
                            $"[Users].[LastName], " +
                            $"[Users].[Id] " +
                            $"FROM [Users] " +
                            $"WHERE NOT [Users].[RecordDeleted] = @Param1 " +
                            $"ORDER BY [Users].[Id]",
                            query.CommandText);
        }

        [Test]
        public void QueryWithPagination2()
        {
            var query = SqlBuilder.Select<User>()
                                  .Where(_ => _.ModifiedDate > DateTimeOffset.Now.Date.AddDays(-50))
                                  .OrderBy(_ => _.Id)
                                  .Take(10)
                                  .Skip(1);


            Assert.AreEqual($"SELECT [Users].* " +
                            $"FROM [Users] " +
                            $"WHERE [Users].[ModifiedDate] > @Param1 " +
                            $"ORDER BY [Users].[Id] " +
                            $"OFFSET 10 ROWS FETCH NEXT 10 ROWS ONLY",
                            query.CommandText);
        }

        [Test]
        public void FindByFieldValue()
        {
            var userEmail = "user@domain1.com";

            var query = SqlBuilder.Select<User>()
                                  .Where(user => user.Email == userEmail);

            Assert.AreEqual("SELECT [Users].* FROM [Users] WHERE [Users].[Email] = @Param1",
                            query.CommandText);

            Assert.AreEqual(userEmail,
                            query.CommandParameters.First().Value);
        }

        [Test]
        public void FindByFieldValueAndGetOnlyOneResult()
        {
            var userEmail = "user@domain1.com";

            var query = SqlBuilder.SelectSingle<User>()
                                  .Where(user => user.Email == userEmail);

            Assert.AreEqual("SELECT TOP(1) [Users].* FROM [Users] WHERE [Users].[Email] = @Param1",
                            query.CommandText);

            Assert.AreEqual(userEmail,
                            query.CommandParameters.First().Value);
        }

        [Test]
        public void FindByFieldValueLike()
        {
            const string searchTerm = "domain.com";

            var query = SqlBuilder.Select<User>()
                                  .Where(user => user.Email.Contains(searchTerm));

            Assert.AreEqual("SELECT [Users].* " +
                            "FROM [Users] " +
                            "WHERE [Users].[Email] LIKE @Param1",
                            query.CommandText);
        }

        [Test]
        public void FindByJoinedEntityValue()
        {
            var email = $"someemail@domain.com";
            var groupId = 3;
            var query = SqlBuilder.Select<User>()
                                  .InnerJoin<UserUserGroup>((@user, @group) => user.Id == group.UserId)
                                  .InnerJoin<UserGroup>((group, g) => group.UserGroupId == g.Id)
                                  .Where(group => group.Id == groupId);

            Assert.AreEqual("SELECT [Users].*, [UsersUserGroup].*, [UsersGroup].* " +
                            "FROM [Users] " +
                            "INNER JOIN [UsersUserGroup] ON [Users].[Id] = [UsersUserGroup].[UserId] " +
                            "INNER JOIN [UsersGroup] ON [UsersUserGroup].[UserGroupId] = [UsersGroup].[Id] " +
                            "WHERE [UsersGroup].[Id] = @Param1",
                            query.CommandText);


            var query2 = SqlBuilder.Select<User>()
                                   .Where(user => user.Email == email)
                                   .InnerJoin<UserUserGroup>((@user, @group) => user.Id == group.UserId)
                                   .InnerJoin<UserGroup>((group, g) => group.UserGroupId == g.Id)
                                   .Where(group => group.Id == groupId);

            Assert.AreEqual("SELECT [Users].*, [UsersUserGroup].*, [UsersGroup].* " +
                            "FROM [Users] " +
                            "INNER JOIN [UsersUserGroup] ON [Users].[Id] = [UsersUserGroup].[UserId] " +
                            "INNER JOIN [UsersGroup] ON [UsersUserGroup].[UserGroupId] = [UsersGroup].[Id] " +
                            "WHERE [Users].[Email] = @Param1 " +
                            "AND [UsersGroup].[Id] = @Param2",
                            query2.CommandText);

            //var result = new Dictionary<long, User>();
            //var results = Connection.Query<User, UserGroup, User>(query.CommandText,
            //                                                      (user, group) =>
            //                                                      {
            //                                                          if (!result.ContainsKey(user.Id))
            //                                                          {
            //                                                              user.Groups = new List<UserGroup>();
            //                                                              result.Add(user.Id, user);
            //                                                          }

            //                                                          result[user.Id].Groups.Add(group);
            //                                                          return user;
            //                                                      },
            //                                                      query.CommandParameters,
            //                                                      splitOn: "UserId,UserGroupId")
            //                        .ToList();
        }

        [Test]
        public void OrderEntitiesByField()
        {
            var query = SqlBuilder.Select<UserGroup>()
                                  .OrderBy(_ => _.Name);

            Assert.AreEqual("SELECT [UsersGroup].* " +
                            "FROM [UsersGroup] " +
                            "ORDER BY [UsersGroup].[Name]",
                            query.CommandText);
        }

        [Test]
        public void OrderEntitiesByFieldDescending()
        {
            var query = SqlBuilder.Select<UserGroup>()
                                  .OrderByDescending(_ => _.Name);


            Assert.AreEqual("SELECT [UsersGroup].* " +
                            "FROM [UsersGroup] " +
                            "ORDER BY [UsersGroup].[Name] DESC",
                            query.CommandText);
        }

        [Test]
        public void SetAdapter()
        {
            SqlBuilder.SetAdapter(new MySqlAdapter());

            Assert.IsTrue(true);
        }

        [Test]
        public void WhereEnum()
        {
            var userType = UserTypeEnum.Player;
            var query = SqlBuilder.Select<PermissionGroup>().Where(x => x.UserType == userType);
            Assert.AreEqual(query.CommandText, "SELECT [permissiongroups].* FROM [permissiongroups] WHERE [permissiongroups].[UserType] = @Param1");
            Assert.AreEqual(query.CommandParameters.First().Value, 4);
        }

        [Test]
        public void WhereEnumInPlace()
        {
            var query = SqlBuilder.Select<PermissionGroup>().Where(x => x.UserType == UserTypeEnum.Player);
            Assert.AreEqual(query.CommandText, "SELECT [permissiongroups].* FROM [permissiongroups] WHERE [permissiongroups].[UserType] = @Param1");
            Assert.AreEqual(query.CommandParameters.First().Value, 4);
        }

        [Test]
        public void WhereIsInLambdaList()
        {
            var permItem = SqlBuilder.Select<PermissionGroup>().WhereIsIn(x => x.ID_Perm, new List<int> { 1, 2, 4 });

            Assert.AreEqual(permItem.CommandText, "SELECT [permissiongroups].* FROM [permissiongroups] WHERE [permissiongroups].[ID_Perm] IN (@Param1,@Param2,@Param3)");
            Assert.AreEqual(permItem.CommandParameters.Count, 3);
        }

        [Test]
        public void WhereIsInLambdaArray()
        {
            var permItem = SqlBuilder.Select<PermissionGroup>().WhereIsIn(x => x.ID_Perm, new int[] { 1, 2, 4 });

            Assert.AreEqual(permItem.CommandText, "SELECT [permissiongroups].* FROM [permissiongroups] WHERE [permissiongroups].[ID_Perm] IN (@Param1,@Param2,@Param3)");
            Assert.AreEqual(permItem.CommandParameters.Count, 3);
        }

        [Test]
        public void WhereIsInLambdaLinq()
        {
            var perm = new List<PermissionGroup>()
            {
                new  PermissionGroup{ ID = 1, Name = "Test1" },
                new  PermissionGroup{ ID = 1, Name = "Test2" },
                new  PermissionGroup{ ID = 1, Name = "Test3" },
                new  PermissionGroup{ ID = 1, Name = "Test4" },
                new  PermissionGroup{ ID = 1, Name = "Test5" },
            };

            var permItem = SqlBuilder.Select<PermissionGroup>().WhereIsIn(x => x.ID_Perm, perm.Select(x => x.ID));

            Assert.AreEqual(permItem.CommandText, "SELECT [permissiongroups].* FROM [permissiongroups] WHERE [permissiongroups].[ID_Perm] IN (@Param1,@Param2,@Param3,@Param4,@Param5)");
            Assert.AreEqual(permItem.CommandParameters.Count, 5);
        }

        [Test]
        public void WhereBetween()
        {
            var perm = SqlBuilder.Select<PermissionGroup>().WhereBetween(x => x.Date, DateTime.Now, DateTime.Now.AddDays(1));
            Assert.AreEqual(perm.CommandText, "SELECT [permissiongroups].* FROM [permissiongroups] WHERE ([permissiongroups].[Date] BETWEEN @Param1 AND @Param2)");
            Assert.AreEqual(perm.CommandParameters.Count, 2);
        }

        [Test]
        public void WhereBetweenAnd()
        {
            var perm = SqlBuilder.Select<PermissionGroup>().Where(x => x.Name == "Test").AndBetween(x => x.Date, DateTime.Now, DateTime.Now.AddDays(1));
            Assert.AreEqual(perm.CommandText, "SELECT [permissiongroups].* FROM [permissiongroups] WHERE [permissiongroups].[Name] = @Param1 AND ([permissiongroups].[Date] BETWEEN @Param2 AND @Param3)");
            Assert.AreEqual(perm.CommandParameters.Count, 3);
        }

        [Test]
        public void WhereNotBetween()
        {
            var perm = SqlBuilder.Select<PermissionGroup>().WhereNotBetween(x => x.Date, DateTime.Now, DateTime.Now.AddDays(1));
            Assert.AreEqual(perm.CommandText, "SELECT [permissiongroups].* FROM [permissiongroups] WHERE ([permissiongroups].[Date] NOT BETWEEN @Param1 AND @Param2)");
            Assert.AreEqual(perm.CommandParameters.Count, 2);
        }

        [Test]
        public void WhereBetweenMySql()
        {
            SqlBuilder.SetAdapter(new MySqlAdapter());
            var perm = SqlBuilder.Select<PermissionGroup>().Where(x => x.Name == "Test").AndBetween(x => x.Date, DateTime.Now, DateTime.Now.AddDays(1));
            Assert.AreEqual(perm.CommandText, "SELECT permissiongroups.* FROM permissiongroups WHERE permissiongroups.Name = @Param1 AND (permissiongroups.Date BETWEEN @Param2 AND @Param3)");
            Assert.AreEqual(perm.CommandParameters.Count, 3);
        }

        [Test]
        public void MultipleQuery()
        {
            var qry = SqlBuilder
                .From<PermissionGroup>(x => x.Where(y => y.ID == 2))
                .From<UserGroup>(x => x.Where(y => y.IsDeleted == true));

            var commandQry = new StringBuilder();
            commandQry.AppendLine("SELECT [permissiongroups].* FROM [permissiongroups] WHERE [permissiongroups].[ID] = @Param1");
            commandQry.Append("SELECT [UsersGroup].* FROM [UsersGroup] WHERE [UsersGroup].[IsDeleted] = @Param2");

            Assert.AreEqual(commandQry.ToString(), qry.CommandText);
            Assert.AreEqual(2, qry.CommandParameters.Count);
        }

        [Test]
        public void MultipleQueryNoParam()
        {
            var qry = SqlBuilder
                .From<PermissionGroup>()
                .From<UserGroup>();

            var commandQry = new StringBuilder();
            commandQry.AppendLine("SELECT [permissiongroups].* FROM [permissiongroups]");
            commandQry.Append("SELECT [UsersGroup].* FROM [UsersGroup]");

            Assert.AreEqual(commandQry.ToString(), qry.CommandText);
            Assert.AreEqual(0, qry.CommandParameters.Count);
        }

        [Test]
        public void Join()
        {
            var qry = SqlBuilder
                .Select<UserGroup>(x => x.IsUndeletable).Where(x => x.Id == 3 || x.IsDeleted)
                .LeftJoin<UserUserGroup>((x, y) => x.Id == y.UserGroupId, x => x.UserId)
                .LeftJoin<User>((x, y) => x.UserId == y.Id, x => new { x.Email, x.FirstName }).Where(x => x.Id == 2);
            Assert.IsNotNull(qry.CommandText);

            var cmd = "SELECT [UsersGroup].[IsUndeletable], [UsersUserGroup].[UserId], [Users].[Email], [Users].[FirstName] ";
            cmd += "FROM [UsersGroup] ";
            cmd += "LEFT JOIN [UsersUserGroup] ON [UsersGroup].[Id] = [UsersUserGroup].[UserGroupId] ";
            cmd += "LEFT JOIN [Users] ON [UsersUserGroup].[UserId] = [Users].[Id] ";
            cmd += "WHERE ([UsersGroup].[Id] = @Param1 OR [UsersGroup].[IsDeleted] = @Param2) AND [Users].[Id] = @Param3";


            Assert.AreEqual(cmd, qry.CommandText);
            Assert.AreEqual(3, qry.CommandParameters.Count);
        }

        [Test]
        public void JoinMultipleOn()
        {
            var qry = SqlBuilder
                .Select<UserGroup>(x => x.IsUndeletable).Where(x => x.Id == 3 || x.IsDeleted)
                .LeftJoin<User>((x, y) => x.Id == y.Id && y.FirstName == "Jose", x => new { x.Email, x.FirstName }).Where(x => x.Id == 2);
            Assert.IsNotNull(qry.CommandText);

            var cmd = "SELECT [UsersGroup].[IsUndeletable], [Users].[Email], [Users].[FirstName] ";
            cmd += "FROM [UsersGroup] ";
            cmd += "LEFT JOIN [Users] ON ([UsersGroup].[Id] = [Users].[Id] AND [Users].[FirstName] = @Param3) ";
            cmd += "WHERE ([UsersGroup].[Id] = @Param1 OR [UsersGroup].[IsDeleted] = @Param2) AND [Users].[Id] = @Param4";

            Assert.AreEqual(cmd, qry.CommandText);
            Assert.AreEqual(4, qry.CommandParameters.Count);
        }

        [Test]
        public void JoinMultipleOnWithSelectAll()
        {
            var qry = SqlBuilder
                .Select<UserGroup>().Where(x => x.Id == 3 || x.IsDeleted)
                .LeftJoin<User>((x, y) => x.Id == y.Id && y.FirstName == "Jose", x => x);
            Assert.IsNotNull(qry.CommandText);

            var cmd = "SELECT [Users].* FROM [UsersGroup] ";
            cmd += "LEFT JOIN [Users] ON ([UsersGroup].[Id] = [Users].[Id] AND [Users].[FirstName] = @Param3) ";
            cmd += "WHERE ([UsersGroup].[Id] = @Param1 OR [UsersGroup].[IsDeleted] = @Param2)";

            Assert.AreEqual(cmd, qry.CommandText);
            Assert.AreEqual(3, qry.CommandParameters.Count);
        }

        [Test]
        public void Result()
        {
            var qry = SqlBuilder
                .Select<UserGroup>().Where(x => x.Id == 3 || x.IsDeleted)
                .LeftJoin<User>((x, y) => x.Id == y.Id && y.FirstName == "Jose")
                .Result<PermissionGroup, UserGroup>((x, y) => new UserGroup { Id = x.Id, Name = y.ResourcePath });

            Assert.IsNotNull(qry.CommandText);

            var cmd = "SELECT [Users].[Id], [permissiongroups].[ResourcePath] [Name] FROM [UsersGroup] ";
            cmd += "LEFT JOIN [Users] ON ([UsersGroup].[Id] = [Users].[Id] AND [Users].[FirstName] = @Param3) ";
            cmd += "WHERE ([UsersGroup].[Id] = @Param1 OR [UsersGroup].[IsDeleted] = @Param2)";

            Assert.AreEqual(cmd, qry.CommandText);
            Assert.AreEqual(3, qry.CommandParameters.Count);
        }

        [Test]
        public void ResultWithAsHelper()
        {
            var qry = SqlBuilder
                .Select<UserGroup>().Where(x => x.Id == 3 || x.IsDeleted)
                .LeftJoin<User>((x, y) => x.Id == y.Id && y.FirstName == "Jose")
                .Result<PermissionGroup, UserGroup>((x, y) => new UserGroup
                {
                    Id = x.Id,
                    Name = y.ResourcePath,
                    ID_FilingStatus = y.Name.As<FilingStatus>(),
                    CreatedBy = y.Date.As<string>(),
                    Description = y.UserType.As<string>()
                });

            Assert.IsNotNull(qry.CommandText);

            var cmd = "SELECT [Users].[Id], [permissiongroups].[ResourcePath] [Name], [permissiongroups].[Name] [ID_FilingStatus], [permissiongroups].[Date] [CreatedBy], [permissiongroups].[UserType] [Description] ";
            cmd += "FROM [UsersGroup] LEFT JOIN [Users] ON ([UsersGroup].[Id] = [Users].[Id] AND [Users].[FirstName] = @Param3) ";
            cmd += "WHERE ([UsersGroup].[Id] = @Param1 OR [UsersGroup].[IsDeleted] = @Param2)";

            Assert.AreEqual(cmd, qry.CommandText);
            Assert.AreEqual(3, qry.CommandParameters.Count);
        }

        [Test]
        public void ResultWithFormatHelper()
        {
            var qry = SqlBuilder
                .Select<UserGroup>().Where(x => x.Id == 3 || x.IsDeleted)
                .LeftJoin<User>((x, y) => x.Id == y.Id && y.FirstName == "Jose")
                .Result<PermissionGroup, UserGroup>((x, y) => new UserGroup
                {
                    CreatedBy = y.Date.FormatSql("HH:mm"),
                });

            Assert.IsNotNull(qry.CommandText);

            var cmd = "SELECT FORMAT([permissiongroups].[Date], 'HH:mm') [CreatedBy] FROM [UsersGroup] ";
            cmd += "LEFT JOIN [Users] ON ([UsersGroup].[Id] = [Users].[Id] AND [Users].[FirstName] = @Param3) ";
            cmd += "WHERE ([UsersGroup].[Id] = @Param1 OR [UsersGroup].[IsDeleted] = @Param2)";

            Assert.AreEqual(cmd, qry.CommandText);
            Assert.AreEqual(3, qry.CommandParameters.Count);
        }

        [Test]
        public void ResultWithIsNullHelper()
        {
            var ndate = DateTime.Now;
            var qry = SqlBuilder
                .Select<UserGroup>().Where(x => x.Id == 3 || x.IsDeleted)
                .LeftJoin<User>((x, y) => x.Id == y.Id && y.FirstName == "Jose")
                .Result<PermissionGroup, UserGroup>((x, y) => new UserGroup
                {
                    CreatedBy = y.Name.IsNullSql("ss"),
                    Description = y.Name.IsNullSql(x.LastName),
                    ModifiedDate = y.Date.IsNullSql(DateTime.Now),
                    CreatedDate = y.Date.IsNullSql(ndate),
                });

            Assert.IsNotNull(qry.CommandText);

            var cmd = $"SELECT ISNULL([permissiongroups].[Name], 'ss') [CreatedBy], ISNULL([permissiongroups].[Name], [Users].[LastName]) [Description], ISNULL([permissiongroups].[Date], GETDATE()) [ModifiedDate], ISNULL([permissiongroups].[Date], '{ ndate }') [CreatedDate] ";
            cmd += "FROM [UsersGroup] LEFT JOIN [Users] ON ([UsersGroup].[Id] = [Users].[Id] AND [Users].[FirstName] = @Param3) ";
            cmd += "WHERE ([UsersGroup].[Id] = @Param1 OR [UsersGroup].[IsDeleted] = @Param2)";

            Assert.AreEqual(cmd, qry.CommandText);
            Assert.AreEqual(3, qry.CommandParameters.Count);
        }

        [Test]
        public void ResultWithIsNullHelperForNullableTypes()
        {
            var ndate = DateTime.Now;
            var qry = SqlBuilder
                .Select<UserGroup>().Where(x => x.Id == 3 || x.IsDeleted)
                .LeftJoin<User>((x, y) => x.Id == y.Id && y.FirstName == "Jose")
                .Result<PermissionGroup, UserGroup>((x, y) => new UserGroup
                {
                    WorkCredit = y.WorkCredit.IsNullSql(0),
                    IsDeleted = y.ntf.IsNullSql(false)
                });

            Assert.IsNotNull(qry.CommandText);

            var cmd = $"SELECT ISNULL([permissiongroups].[WorkCredit], 0) [WorkCredit], ISNULL([permissiongroups].[ntf], 0) [IsDeleted] ";
            cmd += "FROM [UsersGroup] LEFT JOIN [Users] ON ([UsersGroup].[Id] = [Users].[Id] AND [Users].[FirstName] = @Param3) ";
            cmd += "WHERE ([UsersGroup].[Id] = @Param1 OR [UsersGroup].[IsDeleted] = @Param2)";

            Assert.AreEqual(cmd, qry.CommandText);
            Assert.AreEqual(3, qry.CommandParameters.Count);
        }

        [Test]
        public void ResultWithConcatHelper()
        {
            var ndate = DateTime.Now;
            var qry = SqlBuilder
                .Select<UserGroup>().Where(x => x.Id == 3 || x.IsDeleted)
                .LeftJoin<User>((x, y) => x.Id == y.Id && y.FirstName == "Jose")
                .Result<PermissionGroup, UserGroup>((x, y) => new UserGroup
                {
                    CreatedBy = y.Name.ConcatSql("ss", " ", y.Date),
                    Description = y.Name.ConcatSql(x.LastName, " ", x.FirstName),
                    Name = y.Date.ConcatSql(DateTime.Now, 32, ndate),
                    ModifiedBy = y.WorkCredit.ConcatSql(0),
                });

            Assert.IsNotNull(qry.CommandText);

            var cmd = $"SELECT CONCAT([permissiongroups].[Name], 'ss', ' ', [permissiongroups].[Date]) [CreatedBy], CONCAT([permissiongroups].[Name], [Users].[LastName], ' ', [Users].[FirstName]) [Description], CONCAT([permissiongroups].[Date], GETDATE(), 32, '{ ndate }') [Name], CONCAT([permissiongroups].[WorkCredit], 0) [ModifiedBy] ";
            cmd += "FROM [UsersGroup] LEFT JOIN [Users] ON ([UsersGroup].[Id] = [Users].[Id] AND [Users].[FirstName] = @Param3) ";
            cmd += "WHERE ([UsersGroup].[Id] = @Param1 OR [UsersGroup].[IsDeleted] = @Param2)";

            Assert.AreEqual(cmd, qry.CommandText);
            Assert.AreEqual(3, qry.CommandParameters.Count);
        }

        [Test, Ignore("Incomplete Impl")]
        public void ResultWithCaseHelper()
        {
            var ndate = DateTime.Now;
            var qry = SqlBuilder
                .Select<UserGroup>().Where(x => x.Id == 3 || x.IsDeleted)
                .LeftJoin<User>((x, y) => x.Id == y.Id && y.FirstName == "Jose")
                .Result<PermissionGroup, UserGroup>((x, y) => new UserGroup
                {
                    CreatedBy = y.Case(z => z.Name == "ss").Then("sed")
                                .When(z=> z.ResourcePath != "nds").Then("swd")
                                .When(z=> z.ntf == null).Then(y.ResourcePath)
                                .Else(y.Name).End<string>(),
                });

            Assert.IsNotNull(qry.CommandText);

            var cmd = $"SELECT CONCAT([permissiongroups].[Name], 'ss', ' ', [permissiongroups].[Date]) [CreatedBy], CONCAT([permissiongroups].[Name], [Users].[LastName], ' ', [Users].[FirstName]) [Description], CONCAT([permissiongroups].[Date], GETDATE(), '32', '{ ndate }') [Name] ";
            cmd += "FROM [UsersGroup] LEFT JOIN [Users] ON ([UsersGroup].[Id] = [Users].[Id] AND [Users].[FirstName] = @Param3) ";
            cmd += "WHERE ([UsersGroup].[Id] = @Param1 OR [UsersGroup].[IsDeleted] = @Param2)";

            Assert.AreEqual(cmd, qry.CommandText);
            Assert.AreEqual(3, qry.CommandParameters.Count);
        }

        [Test]
        public void UnionQuery()
        {
            var qry = SqlBuilder
                .Union<PermissionGroup>(x => x.Select(x=> new { x.ID, x.Name }).Where(y => y.ID == 2))
                .Union<UserGroup>(x => x.Select(x=> new { ID = x.Id, x.Name }).Where(y => y.IsDeleted == true));

            var commandQry = new StringBuilder();
            commandQry.AppendLine("SELECT [permissiongroups].[ID], [permissiongroups].[Name] FROM [permissiongroups] WHERE [permissiongroups].[ID] = @Param1");
            commandQry.AppendLine("UNION ALL");
            commandQry.Append("SELECT [UsersGroup].[Id] [ID], [UsersGroup].[Name] FROM [UsersGroup] WHERE [UsersGroup].[IsDeleted] = @Param2");

            Assert.AreEqual(commandQry.ToString(), qry.CommandText);
            Assert.AreEqual(2, qry.CommandParameters.Count);
        }

        [Test]
        public void UnionQueryNoParam()
        {
            var qry = SqlBuilder
                .Union<PermissionGroup>(x=> x.Select(x=> new { x.ID, x.Name }))
                .Union<UserGroup>(x=> x.Select(x=> new { ID = x.Id, x.Name }));

            var commandQry = new StringBuilder();
            commandQry.AppendLine("SELECT [permissiongroups].[ID], [permissiongroups].[Name] FROM [permissiongroups]");
            commandQry.AppendLine("UNION ALL");
            commandQry.Append("SELECT [UsersGroup].[Id] [ID], [UsersGroup].[Name] FROM [UsersGroup]");

            Assert.AreEqual(commandQry.ToString(), qry.CommandText);
            Assert.AreEqual(0, qry.CommandParameters.Count);
        }


    }
}