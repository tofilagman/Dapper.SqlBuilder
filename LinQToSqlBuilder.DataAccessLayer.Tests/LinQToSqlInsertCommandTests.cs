using System;
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
                CreatedBy   = "TestSystem",
                CreatedDate = DateTimeOffset.Now,
                Description = "Created from Test System",
                Name        = "TestUserGroup",
                IsDeleted   = false
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
                                       CreatedBy   = "TestSystem",
                                       CreatedDate = DateTimeOffset.Now,
                                       Description = "Created from Test System",
                                       Name        = "TestUserGroup",
                                       IsDeleted   = false
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
            var query = SqlBuilder.InsertMany<UserGroup>(_ => new []
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


            Assert.AreEqual("INSERT INTO [UsersGroup] ([CreatedBy], [CreatedDate], [Description], [Name], [IsDeleted]) "+
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
                                       CreatedBy     = "Cloning System",
                                       CreatedDate   = DateTimeOffset.Now,
                                       Description   = userGroup.Description,
                                       Name          = userGroup.Name,
                                       IsDeleted     = userGroup.IsDeleted,
                                       IsUndeletable = userGroup.IsUndeletable,
                                       OriginalId    = userGroup.Id
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
    }
}
