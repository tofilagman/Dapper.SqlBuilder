using System;
using System.Collections.Generic;
using System.Text;
using Dapper.SqlBuilder;
using Dapper.SqlBuilder.Adapter;
using LinQToSqlBuilder.DataAccessLayer.Tests.Base;
using LinQToSqlBuilder.DataAccessLayer.Tests.Entities;
using NUnit.Framework;

namespace LinQToSqlBuilder.DataAccessLayer.Tests
{
    [TestFixture]
    public class LinQToSqlInsertCommandTests : TestBase
    {
        [SetUp]
        public void Setup()
        {
            SqlBuilder.SetAdapter(new SqlServerAdapter());
        }

        [Test]
        public void InsertSingleRecord()
        {
            var query = SqlBuilder.Insert<UserGroup>(_ => new UserGroup
            {
                CreatedBy = "TestSystem",
                CreatedDate = DateTimeOffset.Now,
                Description = "Created from Test System",
                Name = "TestUserGroup",
                IsDeleted = false
            });

            Assert.AreEqual("INSERT INTO [UsersGroup] ([CreatedBy], [CreatedDate], [Description], [Name], [IsDeleted]) " +
                            "VALUES (@Param1, @Param2, @Param3, @Param4, @Param5)",
                            query.CommandText);
        }

        [Test]
        public void InsertSingleRecordWithOutputIdentity()
        {
            var query = SqlBuilder.Insert<UserGroup>(_ => new UserGroup
            {
                CreatedBy = "TestSystem",
                CreatedDate = DateTimeOffset.Now,
                Description = "Created from Test System",
                Name = "TestUserGroup",
                IsDeleted = false
            })
                                  .OutputIdentity();

            Assert.AreEqual("INSERT INTO [UsersGroup] ([CreatedBy], [CreatedDate], [Description], [Name], [IsDeleted]) " +
                            "OUTPUT Inserted.[Id] " +
                            "VALUES (@Param1, @Param2, @Param3, @Param4, @Param5)",
                            query.CommandText);
        }

        [Test]
        public void InsertMultipleRecords()
        {
            var query = SqlBuilder.InsertMany<UserGroup>(_ => new[]
            {
                new UserGroup
                {
                    CreatedBy   = "TestSystem",
                    CreatedDate = DateTimeOffset.Now,
                    Description = "Created from Test System",
                    Name        = "TestUserGroup",
                    IsDeleted   = false
                },

                new UserGroup
                {
                    CreatedBy   = "TestSystem",
                    CreatedDate = DateTimeOffset.Now,
                    Description = "Created from Test System",
                    Name        = "TestUserGroup2",
                    IsDeleted   = false
                },

                new UserGroup
                {
                    CreatedBy   = "TestSystem",
                    CreatedDate = DateTimeOffset.Now,
                    Description = "Created from Test System",
                    Name        = "TestUserGroup3",
                    IsDeleted   = false
                }
            });


            Assert.AreEqual("INSERT INTO [UsersGroup] ([CreatedBy], [CreatedDate], [Description], [Name], [IsDeleted]) " +
                            "VALUES (@Param1, @Param2, @Param3, @Param4, @Param5), " +
                            "(@Param6, @Param7, @Param8, @Param9, @Param10), " +
                            "(@Param11, @Param12, @Param13, @Param14, @Param15)",
                            query.CommandText);
        }

        [Test]
        public void InsertRecordFromAnotherTables()
        {
            var query = SqlBuilder.InsertFrom<UserGroup, CloneUserGroup>(userGroup => new CloneUserGroup()
            {
                CreatedBy = "Cloning System",
                CreatedDate = DateTimeOffset.Now,
                Description = userGroup.Description,
                Name = userGroup.Name,
                IsDeleted = userGroup.IsDeleted,
                IsUndeletable = userGroup.IsUndeletable,
                OriginalId = userGroup.Id
            })
                                  .Where(group => group.IsDeleted == false);

            Assert.AreEqual("INSERT INTO [CloneUserGroup] ([CreatedBy], [CreatedDate], [Description], [Name], [IsDeleted], [IsUndeletable], [OriginalId]) " +
                          "SELECT " +
                          "@Param1 as [CreatedBy], " +
                          "@Param2 as [CreatedDate], " +
                          "[UsersGroup].[Description] as [Description], " +
                          "[UsersGroup].[Name] as [Name], " +
                          "[UsersGroup].[IsDeleted] as [IsDeleted], " +
                          "[UsersGroup].[IsUndeletable] as [IsUndeletable], " +
                          "[UsersGroup].[Id] as [OriginalId] " +
                          "FROM [UsersGroup] " +
                          "WHERE [UsersGroup].[IsDeleted] = @Param3",
                            query.CommandText);
        }


        [Test]
        public void InsertWithIdentityKey()
        {
            var perm = new PermissionGroup
            {
                ID_Perm = 2,
                Name = "Test",
                ResourcePath = "ss",
                UserType = UserTypeEnum.Agent
            };

            var query = SqlBuilder.Insert(perm);
            Assert.AreEqual("INSERT INTO [permissiongroups] ([Name], [ResourcePath], [UserType], [Date], [ID_Perm], [WorkCredit], [ntf]) VALUES (@Param1, @Param2, @Param3, @Param4, @Param5, @Param6, @Param7)", query.CommandText);
        }

        [Test , Ignore("Obsolete")]
        public void InsertMultipleTableObjectArray()
        {
            var arr = new[]
           {
                new UserGroup
                {
                    CreatedBy   = "TestSystem",
                    CreatedDate = DateTimeOffset.Now,
                    Description = "Created from Test System",
                    Name        = "TestUserGroup",
                    IsDeleted   = false
                },

                new UserGroup
                {
                    CreatedBy   = "TestSystem",
                    CreatedDate = DateTimeOffset.Now,
                    Description = "Created from Test System",
                    Name        = "TestUserGroup2",
                    IsDeleted   = false
                },

                new UserGroup
                {
                    CreatedBy   = "TestSystem",
                    CreatedDate = DateTimeOffset.Now,
                    Description = "Created from Test System",
                    Name        = "TestUserGroup3",
                    IsDeleted   = false
                }
            };

            var query = SqlBuilder.InsertMany<UserGroup>(_ => arr);

            Assert.AreEqual("INSERT INTO [UsersGroup] ([Name], [Description], [IsUndeletable], [IsDeleted], [CreatedDate], [ModifiedDate], [CreatedBy], [ModifiedBy]) VALUES (@Param1, @Param2, @Param3, @Param4, @Param5, @Param6, @Param7, @Param8), (@Param9, @Param10, @Param11, @Param12, @Param13, @Param14, @Param15, @Param16), (@Param17, @Param18, @Param19, @Param20, @Param21, @Param22, @Param23, @Param24)",
                            query.CommandText);
        }

        [Test , Ignore("Obsolete")]
        public void InsertMultipleTableObjectList()
        {
            var arr = new List<UserGroup>
           {
                new UserGroup
                {
                    CreatedBy = "TestSystem",
                    CreatedDate = DateTimeOffset.Now,
                    Description = "Created from Test System",
                    Name = "TestUserGroup",
                    IsDeleted = false
                },

                new UserGroup
                {
                    CreatedBy = "TestSystem",
                    CreatedDate = DateTimeOffset.Now,
                    Description = "Created from Test System",
                    Name = "TestUserGroup2",
                    IsDeleted = false
                },

                new UserGroup
                {
                    CreatedBy = "TestSystem",
                    CreatedDate = DateTimeOffset.Now,
                    Description = "Created from Test System",
                    Name = "TestUserGroup3",
                    IsDeleted = false
                }
            };

            var query = SqlBuilder.InsertMany<UserGroup>(_ => arr);

            Assert.AreEqual("INSERT INTO [UsersGroup] ([Name], [Description], [IsUndeletable], [IsDeleted], [CreatedDate], [ModifiedDate], [CreatedBy], [ModifiedBy]) VALUES (@Param1, @Param2, @Param3, @Param4, @Param5, @Param6, @Param7, @Param8), (@Param9, @Param10, @Param11, @Param12, @Param13, @Param14, @Param15, @Param16), (@Param17, @Param18, @Param19, @Param20, @Param21, @Param22, @Param23, @Param24)",
                            query.CommandText);
        }

        [Test, Ignore("Obsolete")]
        public void InsertMultipleTableObjectArrayByParam()
        {
            var arr = new[]
           {
                new UserGroup
                {
                    CreatedBy   = "TestSystem",
                    CreatedDate = DateTimeOffset.Now,
                    Description = "Created from Test System",
                    Name        = "TestUserGroup",
                    IsDeleted   = false
                },

                new UserGroup
                {
                    CreatedBy   = "TestSystem",
                    CreatedDate = DateTimeOffset.Now,
                    Description = "Created from Test System",
                    Name        = "TestUserGroup2",
                    IsDeleted   = false
                },

                new UserGroup
                {
                    CreatedBy   = "TestSystem",
                    CreatedDate = DateTimeOffset.Now,
                    Description = "Created from Test System",
                    Name        = "TestUserGroup3",
                    IsDeleted   = false
                }
            };

            var query = SqlBuilder.InsertMany(arr);

            Assert.AreEqual("INSERT INTO [UsersGroup] ([Name], [Description], [IsUndeletable], [IsDeleted], [CreatedDate], [ModifiedDate], [CreatedBy], [ModifiedBy]) VALUES (@Param1, @Param2, @Param3, @Param4, @Param5, @Param6, @Param7, @Param8), (@Param9, @Param10, @Param11, @Param12, @Param13, @Param14, @Param15, @Param16), (@Param17, @Param18, @Param19, @Param20, @Param21, @Param22, @Param23, @Param24)",
                            query.CommandText);
        }

        [Test]
        public void testNotMember()
        {
            var not = true;

            var query = SqlBuilder.Insert<UserGroup>(x => new UserGroup { 
                IsDeleted = !not
            });

            Assert.AreEqual("INSERT INTO [UsersGroup] ([IsDeleted]) VALUES (@Param1)", query.CommandText);
            Assert.AreEqual(1, query.CommandParameters.Count);
            Assert.AreEqual(false, query.CommandParameters["Param1"]);
        }

        [Test]
        public void InsertManyRecords()
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
                query.Insert(x => new UserGroup { ID_FilingStatus = nd.ID_FilingStatus, CreatedBy = nd.CreatedBy });
            }

            var result = new StringBuilder();
            result.AppendLine("INSERT INTO [UsersGroup] ([ID_FilingStatus], [CreatedBy]) VALUES (@Param1, @Param2)");
            result.AppendLine("INSERT INTO [UsersGroup] ([ID_FilingStatus], [CreatedBy]) VALUES (@Param3, @Param4)");
            result.AppendLine("INSERT INTO [UsersGroup] ([ID_FilingStatus], [CreatedBy]) VALUES (@Param5, @Param6)");
            result.AppendLine("INSERT INTO [UsersGroup] ([ID_FilingStatus], [CreatedBy]) VALUES (@Param7, @Param8)");
            result.AppendLine("INSERT INTO [UsersGroup] ([ID_FilingStatus], [CreatedBy]) VALUES (@Param9, @Param10)");
            result.Append("INSERT INTO [UsersGroup] ([ID_FilingStatus], [CreatedBy]) VALUES (@Param11, @Param12)");

            Assert.AreEqual(result.ToString(), query.CommandText);
            Assert.AreEqual(12, query.CommandParameters.Count);

        }

    }
}
