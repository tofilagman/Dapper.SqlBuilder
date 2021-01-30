LinQ to SQL Builder - small .NET library supports creating SQL queries and commands in a strongly typed fashion.
---------------------------------------------------------------------------
  
Repack
==========
This lib was repack and use to replace Dapper.Contrib to work for [MsSQL, MySql] SQL Syntax and was taken from https://github.com/dotnetbrightener/LinQ-To-Sql-Builder

Nuget Package
============

[https://www.nuget.org/packages/Dapper.SqlBuilder.Unofficial/](https://www.nuget.org/packages/Dapper.SqlBuilder.Unofficial/)

To implement Dapper Chain Extensions

[https://www.nuget.org/packages/Dapper.SqlBuilder.Unofficial.Extensions/](https://www.nuget.org/packages/Dapper.SqlBuilder.Unofficial.Extensions/)

At the moment the library supports .Net Core 2.1 and above.

Installation
============

Install using Package Reference
---

1. Create a `nuget.config` in your project / solution folder with the following content:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
    <packageSources>        
        <add key="githubDotnetBrightener" value="https://nuget.pkg.github.com/dotnetbrightener/index.json" />
    </packageSources>
    <packageSourceCredentials>
        <githubDotnetBrightener>
            <add key="Username" value="[your_github_username]" />
            <add key="ClearTextPassword" value="[github_access_token]" />
        </githubDotnetBrightener>
    </packageSourceCredentials>
</configuration>
```

2. Replace `[your_github_username]` with your Github account, and `[github_access_token]` with your developer personal access token.

To generate your token if you do not have one, visit [Personal Access Tokens](https://github.com/settings/tokens), then click the [Generate new token] button. Make sure you select `read:packages` scope to be able to download packages from Github Packages. Once done, copy the generated token as it will no longer be shown to you once you leave this page.

3. Use the following command to install the package to your project
   
```
dotnet add [YOUR_PROJECT_NAME] package Dapper.SqlBuilder
```

You can optionally specified version by using `--version [version]` parameter
 
Usage
=====

## Set Provider
```csharp
//MSSQL
SqlBuilder.SetAdapter(new SqlServerAdapter());
//MySql
SqlBuilder.SetAdapter(new MySqlAdapter());
```

## Simple Select

This basic example queries the database for 10 User and order them by their registration date using **Dapper**:
```csharp
var query = SqlBuilder.Select<User>()
                      .OrderBy(_ => _.RegistrationDate)
                      .Take(10);
                      
var results = Connection.Query<User>(query.CommandText, query.CommandParameters);
```

As you can see the CommandText property will return the SQL string itself, while the CommandParameters property refers to a dictionary of SQL parameters. 

## Select query with Join
The below example performs a query to the User table, and join it with UserGroup table to returns a many to many relationship mapping specified using **Dapper** mapping API

```csharp
var query = SqlBuilder.Select<User>()
                    //.Where(user => user.Email == email)
                      .Join<UserUserGroup>((@user, @group) => user.Id == group.UserId)
                      .Join<UserGroup>((group,     g) => group.UserGroupId == g.Id)
                      .Where(group => group.Id == groupId);

var result = new Dictionary<long, User>();
var results = Connection.Query<User, UserGroup, User>(query.CommandText,
                                                        (user, group) =>
                                                        {
                                                            if (!result.ContainsKey(user.Id))
                                                            {
                                                                user.Groups = new List<UserGroup>();
                                                                result.Add(user.Id, user);
                                                            }

                                                            result[user.Id].Groups.Add(group);
                                                            return user;
                                                        },
                                                        query.CommandParameters,
                                                        splitOn: "UserId,UserGroupId")
                        .ToList();
```
## Insert single record

The example below will generate an insert command with one record.

```csharp
var query = SqlBuilder.Insert<UserGroup>(_ => new UserGroup
            {
                CreatedBy   = "TestSystem",
                CreatedDate = DateTimeOffset.Now,
                Description = "Created from Test System",
                Name        = "TestUserGroup",
                IsDeleted   = false
            });

var results = Connection.Execute(query.CommandText, query.CommandParameters);
```

## Insert multiple records

The example below will generate an insert command with multiple records.

```csharp
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

var results = Connection.Execute(query.CommandText, query.CommandParameters);
```

## Insert by copying from another table

Sometimes we need to copy a bunch of records from one table to another. For instance, if we have an order that contains few products, and the quantity of the products are being updated before the order gets finalized. So we need to keep the inventory history records of all products that are being updated from time to time. Using Entity Framework, we could have loaded all inventory records of the specified products, then create a copied object and insert them to the inventory history. The more products you have, the slower performance you will suffer because you will have to deal with the data that are in memory versus the data that are being processed by other request(s).

```csharp
var query = SqlBuilder.InsertFrom<Inventory, InventoryHistory>(inventory => new InventoryHistory()
                                   {
                                       CreatedBy        = "Cloning System",
                                       CreatedDate      = DateTimeOffset.Now,
                                       StockQuantity    = inventory.StockQuantity,
                                       ReservedQuantity = inventory.ReservedQuantity,
                                       IsDeleted        = inventory.IsDeleted,
                                       InventoryId      = inventory.Id,
                                       ProductId        = inventory.ProductId
                                   })
                                  .WhereIsIn(inventory => inventory.ProductId, new long[] { /*... obmited values, describes the list of product ids */ });

Assert.AreEqual("INSERT INTO [dbo].[InventoryHistory] ([CreatedBy], [CreatedDate], [StockQuantity], [ReservedQuantity], [IsDeleted], [InventoryId], [ProductId]) " +
                "SELECT " +
                "@Param1 as [CreatedBy], " +
                "@Param2 as [CreatedDate], " +
                "[dbo].[Inventory].[StockQuantity] as [StockQuantity], " +
                "[dbo].[Inventory].[ReservedQuantity] as [ReservedQuantity], " +
                "[dbo].[Inventory].[IsDeleted] as [IsDeleted], " +
                "[dbo].[Inventory].[Id] as [InventoryId] " +
                "[dbo].[Inventory].[ProductId] as [ProductId] " +
                "FROM [dbo].[Inventory] " +
                "WHERE [dbo].[Inventory].[ProductId] IS IN @Param3",
                query.CommandText);
```

## Update a record
The example below will generate a command to update the User table, provides 3 properties to be updated, where `user.Id` equals the given value `userId`
```csharp
var query = SqlBuilder.Update<User>(_ => new User
                                   {
                                       Email              = _.Email.Replace("@domain1.com", "@domain2.com"),
                                       LastChangePassword = DateTimeOffset.Now,
                                       FailedLogIns       = _.FailedLogIns + 1
                                   })
                    .Where(user => user.Id == userId);

var result = Connection.Execute(query.CommandText, query.CommandParameters);
// this will return the affected rows of the query
```

## Delete a record / multiple records by condition
The example below will generate a command to delete from User table where the `user.Email` equals the specified `userEmail` value:
```csharp
string userEmail = "query_email@domain1.com";

var query = SqlBuilder.Delete<User>()
                    .Where(user => user.Email == userEmail);
                    // .Where(user => user.Email.Contains("part_of_email_to_search"));

var result = Connection.Execute(query.CommandText, query.CommandParameters);
```

Reference
=========

[https://github.com/dotnetbrightener/LinQ-To-Sql-Builder](https://github.com/dotnetbrightener/LinQ-To-Sql-Builder)

[https://github.com/DomanyDusan/lambda-sql-builder](https://github.com/DomanyDusan/lambda-sql-builder)

[https://github.com/mladenb/sql-query-builder](https://github.com/mladenb/sql-query-builder)