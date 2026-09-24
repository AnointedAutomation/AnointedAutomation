# AnointedAutomation.Repository.Mongo

A thin, testable wrapper over the official MongoDB .NET driver. `IMongoHelper` gives you collection-name-based CRUD, paging, projection, aggregation and index management; `MongoRepository<TDoc>` is a base class for per-collection repositories; and the BSON helpers register class maps and casing conventions for the [AnointedAutomation.Objects](https://www.nuget.org/packages/AnointedAutomation.Objects) models.

[![NuGet](https://img.shields.io/nuget/v/AnointedAutomation.Repository.Mongo.svg)](https://www.nuget.org/packages/AnointedAutomation.Repository.Mongo) [![Downloads](https://img.shields.io/nuget/dt/AnointedAutomation.Repository.Mongo.svg)](https://www.nuget.org/packages/AnointedAutomation.Repository.Mongo)

## Installation

```bash
dotnet add package AnointedAutomation.Repository.Mongo
```

- Target framework: `net10.0`
- Dependencies:
  - [AnointedAutomation.Logging](https://www.nuget.org/packages/AnointedAutomation.Logging)
  - [AnointedAutomation.Objects](https://www.nuget.org/packages/AnointedAutomation.Objects)
  - [MongoDB.Driver](https://www.nuget.org/packages/MongoDB.Driver) 3.11.2
  - [MongoDB.Bson](https://www.nuget.org/packages/MongoDB.Bson) 3.11.2
  - [MongoDB.Libmongocrypt](https://www.nuget.org/packages/MongoDB.Libmongocrypt) 1.12.0
  - [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json) 13.0.4

## Quick start

A document type, a repository and DI registration (the `IServiceCollection` extension assumes an ASP.NET Core or Generic Host app):

```csharp
using System;
using System.Threading.Tasks;
using AnointedAutomation.Repository.Mongo;
using Microsoft.Extensions.DependencyInjection;

public class Product : MongoDocument
{
    public string Name { get; set; }
    public decimal Price { get; set; }
}

public class ProductRepository : MongoRepository<Product>
{
    public ProductRepository(IMongoHelper mongo) : base(mongo, "products") { }

    public Task<Product> GetByNameAsync(string name) => GetSingleAsync(p => p.Name == name);
}

public static class MongoSetup
{
    public static IServiceCollection AddMongo(this IServiceCollection services, string connectionString)
    {
        // Once, at startup, before any Mongo call.
        BsonClassMapRegistrar.RegisterClassMaps();

        services.AddSingleton<IMongoHelperFactory, MongoHelperFactory>();
        services.AddSingleton<IMongoHelper>(sp =>
            sp.GetRequiredService<IMongoHelperFactory>().Create("shop", connectionString));
        services.AddSingleton<ProductRepository>();
        return services;
    }
}
```

## API overview

All types are in the `AnointedAutomation.Repository.Mongo` namespace.

### Connecting

| Member | Description |
|---|---|
| `new MongoHelper(string dbName, string connectionString)` | Connects and sets `database`. (Does not set `dbName`.) |
| `IMongoHelperFactory.Create(string dbName, string connectionString)` | `MongoHelperFactory` returns one cached `IMongoHelper` per connection string and database, with `database` and `dbName` set. |
| `MongoHelper.ConnectionStringBuilder(username, password, cluster, region)` | Builds `mongodb+srv://user:pass@{cluster}.{region}.mongodb.net/?retryWrites=true&w=majority`, URL-encoding the password. |
| `MongoHelper.MongoHelperConnector(IMongoHelper, dbName, username, password, cluster, region)` | Builds the connection string, connects, tests the connection and sets `dbName`. |
| `IMongoHelper.TestConnection()` | Returns the collection names, or null (and logs an error) if the connection fails. |

### `IMongoHelper` operations

Every method takes the collection name as its first argument.

| Area | Methods |
|---|---|
| Create | `CreateDocumentAsync`, `CreateManyAsync` |
| Read | `GetByIdAsync`, `GetSingleAsync`, `GetFilteredDocumentsAsync`, `GetAllDocumentsAsync`, `GetAllDocumentIdsAsync`, `GetPagedAsync`, `GetProjectedAsync`, `DistinctAsync`, `AggregateAsync`, `CountAsync`, `ExistsAsync` |
| Update | `UpdateDocumentAsync`, `UpdateManyAsync`, `UpdateByIdAsync`, `UpsertAsync`, `ReplaceDocumentAsync`, `ReplaceByIdAsync`, `FindOneAndUpdateAsync` |
| Delete | `DeleteDocumentAsync` (one), `DeleteManyAsync`, `DeleteByIdAsync`, `FindOneAndDeleteAsync` |
| Indexes | `EnsureIndexAsync(string, MongoIndexSpec)`, `EnsureIndexesAsync(string, IEnumerable<MongoIndexSpec>)` |

- Filters are `FilterDefinition<T>`. `GetSingleAsync`, `GetFilteredDocumentsAsync`, `DeleteDocumentAsync`, `CountAsync` and `ExistsAsync` also accept an `Expression<Func<T, bool>>`.
- The `...ByIdAsync` methods treat a 24-character hex id as an `ObjectId` and any other string as a plain string `_id`.
- `GetAllDocumentsAsync` loads the whole collection; prefer `GetPagedAsync`, `CountAsync` or `AggregateAsync` for large collections.

### Repository base

| Type | Description |
|---|---|
| `MongoRepository<TDoc>` | Abstract base with constructor `(IMongoHelper mongo, string collectionName)` and protected `Mongo` / `CollectionName`. Public methods: `GetByIdAsync`, `GetSingleAsync`, `GetFilteredAsync`, `GetAllAsync`, `GetPagedAsync`, `CountAsync`, `ExistsAsync`, `CreateAsync`, `UpsertAsync`, `ReplaceByIdAsync`, `UpdateByIdAsync`, `DeleteByIdAsync`, `DeleteManyAsync`. |
| `MongoDocument` | Abstract base with `string Id` mapped as `[BsonId]` / ObjectId, and `[BsonIgnoreExtraElements]`. |
| `AuditableMongoDocument` | Adds `DateTime createdAt` and `updatedAt`. You set these yourself. |

### Indexes

| Type | Description |
|---|---|
| `MongoIndexSpec` | `(string field, bool descending = false, bool unique = false, TimeSpan? expireAfter = null, string name = null)` or `(IReadOnlyList<MongoIndexKey> keys, ...)` for compound indexes. `ResolveName()` gives a deterministic name, so ensuring the same spec repeatedly is idempotent. |
| `MongoIndexKey` | `(string field, bool descending = false)`. |

### BSON and serialization

| Type | Description |
|---|---|
| `BsonClassMapRegistrar.RegisterClassMaps()` | Idempotent, thread-safe. Registers `JObjectSerializer` for every Newtonsoft `JObject` and maps `AnointedAutomation.Objects.Account.User` with `UserId` as `_id` and extra elements ignored. |
| `BsonClassMapRegistrar.RegisterDerivedUser<T>()` | Registers a class derived from `User`. Call after `RegisterClassMaps()`. |
| `BsonClassMapRegistrar.RegisterHybridCasingConvention(string namespacePrefix = "AnointedAutomation")` | Opt-in. Registers `HybridElementNameConvention` for types under the prefix. |
| `BsonClassMapRegistrar.RegisterSnakeCasingConvention(string namespacePrefix = "AnointedAutomation")` | Opt-in. Registers `SnakeCaseElementNameConvention` (for example `LineItems` becomes `line_items`). |
| `HybridElementNameConvention` | Value-type, enum and struct members become camelCase, reference-type members PascalCase. Matches `JsonCasingConvention` in [AnointedAutomation.Objects.API](https://www.nuget.org/packages/AnointedAutomation.Objects.API). |
| `SnakeCaseElementNameConvention` | snake_case element names; `static string ToSnake(string)` is public. |
| `JObjectSerializer` | BSON serializer for Newtonsoft `JObject`. |
| `BsonMap` | `ToPlain(BsonValue)` and `ToDictionary(BsonDocument)` convert BSON into plain dictionaries, lists and CLR scalars (ObjectId becomes its hex string). |

## Casing conventions

Both conventions are opt-in, so installing or upgrading the package changes nothing on its own. Register one once at startup, before `RegisterClassMaps()` and before any Mongo call, and only after your stored data uses the matching key casing:

```csharp
using AnointedAutomation.Repository.Mongo;

BsonClassMapRegistrar.RegisterHybridCasingConvention("MyCompany.MyApi");
BsonClassMapRegistrar.RegisterClassMaps();
```

An explicit `[BsonElement("...")]` always wins over a convention. Names that are not just a casing variant of the member name (`_id`, snake_case external fields, deliberate renames) are left alone.

## Indexes example

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AnointedAutomation.Repository.Mongo;

public static class Indexes
{
    public static Task EnsureAsync(IMongoHelper mongo) =>
        mongo.EnsureIndexesAsync("sessions", new List<MongoIndexSpec>
        {
            new MongoIndexSpec("Email", unique: true),
            new MongoIndexSpec("createdAt", expireAfter: TimeSpan.FromDays(1)),
            new MongoIndexSpec(new List<MongoIndexKey>
            {
                new MongoIndexKey("UserId"),
                new MongoIndexKey("createdAt", descending: true)
            })
        });
}
```

## Logging

`MongoHelper` keeps a static buffer of `LogMessage` entries (for example connection failures):

```csharp
using System;
using AnointedAutomation.Optimization.Logging;
using AnointedAutomation.Repository.Mongo;

MongoHelper.LogAdded += (sender, e) => Console.WriteLine(e.log.message);

foreach (LogMessage log in MongoHelper.GetLogs())
{
    Console.WriteLine($"[{log.messageType}] {log.message}");
}

MongoHelper.ClearLogs(); // raises LogCleared
```

## Related packages

- [AnointedAutomation.Objects](https://www.nuget.org/packages/AnointedAutomation.Objects): the models `BsonClassMapRegistrar` maps.
- [AnointedAutomation.Logging](https://www.nuget.org/packages/AnointedAutomation.Logging): the `LogMessage` type used for the log buffer.
- [AnointedAutomation.Objects.API](https://www.nuget.org/packages/AnointedAutomation.Objects.API): the matching JSON casing convention.
- [AnointedAutomation.Repository.MySql](https://www.nuget.org/packages/AnointedAutomation.Repository.MySql): the same repository style for MySQL.

## License

MIT. See [LICENSE](https://github.com/AnointedAutomation/AnointedAutomation/blob/master/LICENSE).

## Support This Project

This library is free and open source. The best way to support the work is to shop with us:

- **Christian items:** [https://store.anointed.company](https://store.anointed.company)
- **Everything else:** [https://www.mart.club](https://www.mart.club)

Found a bug or have a request? Open an issue at [https://github.com/AnointedAutomation/AnointedAutomation/issues](https://github.com/AnointedAutomation/AnointedAutomation/issues).
