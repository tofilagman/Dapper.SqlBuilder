LinQ to SQL Builder - small .NET library supports creating SQL queries and commands in a strongly typed fashion.
---------------------------------------------------------------------------
  
Repack
==========
This lib was repack and use for Dapper.Contrib to work for [MsSQL, MySql] SQL Syntax and was taken from https://github.com/dotnetbrightener/LinQ-To-Sql-Builder 

.NET versions supported
===

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

Reference
=========

[https://github.com/dotnetbrightener/LinQ-To-Sql-Builder](https://github.com/dotnetbrightener/LinQ-To-Sql-Builder)

[https://github.com/DomanyDusan/lambda-sql-builder](https://github.com/DomanyDusan/lambda-sql-builder)

[https://github.com/mladenb/sql-query-builder](https://github.com/mladenb/sql-query-builder)