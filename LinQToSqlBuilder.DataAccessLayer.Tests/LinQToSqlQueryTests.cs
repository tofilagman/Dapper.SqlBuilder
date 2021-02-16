using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper.SqlBuilder;
using Dapper.SqlBuilder.Adapter;
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
                                  .Join<UserUserGroup>((@user, @group) => user.Id == group.UserId)
                                  .Join<UserGroup>((group, g) => group.UserGroupId == g.Id)
                                  .Where(group => group.Id == groupId);

            Assert.AreEqual("SELECT [Users].*, [UsersUserGroup].*, [UsersGroup].* " +
                            "FROM [Users] " +
                            "JOIN [UsersUserGroup] ON [Users].[Id] = [UsersUserGroup].[UserId] " +
                            "JOIN [UsersGroup] ON [UsersUserGroup].[UserGroupId] = [UsersGroup].[Id] " +
                            "WHERE [UsersGroup].[Id] = @Param1",
                            query.CommandText);


            var query2 = SqlBuilder.Select<User>()
                                   .Where(user => user.Email == email)
                                   .Join<UserUserGroup>((@user, @group) => user.Id == group.UserId)
                                   .Join<UserGroup>((group, g) => group.UserGroupId == g.Id)
                                   .Where(group => group.Id == groupId);

            Assert.AreEqual("SELECT [Users].*, [UsersUserGroup].*, [UsersGroup].* " +
                            "FROM [Users] " +
                            "JOIN [UsersUserGroup] ON [Users].[Id] = [UsersUserGroup].[UserId] " +
                            "JOIN [UsersGroup] ON [UsersUserGroup].[UserGroupId] = [UsersGroup].[Id] " +
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
    }
}