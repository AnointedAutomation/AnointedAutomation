# AnointedAutomation.Repository.Mongo

Jesus is King ✝️

A thin, testable wrapper over the official MongoDB .NET driver. `IMongoHelper` gives you collection name based CRUD, paging, projection, aggregation and index management; `MongoRepository<TDoc>` is a base class for per collection repositories; and the BSON helpers register class maps and casing conventions for the [AnointedAutomation.Objects](https://www.nuget.org/packages/AnointedAutomation.Objects) models.

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
| `MongoHelper.ConnectionStringBuilder(username, password, cluster, region)` | Builds `mongodb+srv://user:pass@{cluster}.{region}.mongodb.net/?retryWrites=true&w=majority`, URL encoding the password. |
| `MongoHelper.MongoHelperConnector(IMongoHelper, dbName, username, password, cluster, region)` | Builds the connection string, connects, tests the connection and sets `dbName`. |
| `IMongoHelper.TestConnection()` | Returns the collection names, or null (and logs an error) if the connection fails. |

### `IMongoHelper` operations

Every method takes the collection name as its first argument.

| Area | Methods |
|---|---|
| Create | `CreateDocumentAsync`, `CreateManyAsync` |
| Bulk | `BulkWriteAsync` (any mix of insert/update/replace/delete models, 1000 per round trip, unordered by default; returns `BulkWriteSummary`; failed models raise `BulkWriteFailedException` with the applied counts and each failure's index) |
| Read | `GetByIdAsync`, `GetSingleAsync`, `GetFilteredDocumentsAsync`, `GetAllDocumentsAsync`, `GetAllDocumentIdsAsync`, `GetPagedAsync`, `GetProjectedAsync`, `DistinctAsync`, `AggregateAsync`, `CountAsync`, `ExistsAsync` |
| Update | `UpdateDocumentAsync`, `UpdateManyAsync`, `UpdateByIdAsync`, `UpsertAsync`, `ReplaceDocumentAsync`, `ReplaceByIdAsync`, `FindOneAndUpdateAsync` |
| Delete | `DeleteDocumentAsync` (one), `DeleteManyAsync`, `DeleteByIdAsync`, `FindOneAndDeleteAsync` |
| Indexes | `EnsureIndexAsync(string, MongoIndexSpec)`, `EnsureIndexesAsync(string, IEnumerable<MongoIndexSpec>)` |

- Filters are `FilterDefinition<T>`. `GetSingleAsync`, `GetFilteredDocumentsAsync`, `DeleteDocumentAsync`, `CountAsync` and `ExistsAsync` also accept an `Expression<Func<T, bool>>`.
- The `...ByIdAsync` methods treat a 24 character hex id as an `ObjectId` and any other string as a plain string `_id`.
- `GetAllDocumentsAsync` (and `MongoRepository<TDoc>.GetAllAsync`, which forwards it) is `[Obsolete]` since 2.2.0 (a compiler warning, not an error): it loads the whole collection into memory. Page with `GetDocumentIdsPageAsync` or `GetPagedAsync`, or use `CountAsync` / `AggregateAsync`. It will be removed in a later major version.
- 3.0.0 (major: three members added to `IMongoHelper`; derive from `DelegatingMongoHelper` and you need no change): `BulkWriteAsync`, plus bounded reads `GetFilteredDocumentsAsync(collection, filter, sort, limit)` and `GetProjectedAsync(collection, filter, projection, sort, limit)` (limit must be 1 or more). Prefer them over the unbounded `GetFilteredDocumentsAsync(collection, filter)` on collections that can grow.

### Routing helpers

| Type | Description |
|---|---|
| `DelegatingMongoHelper` | Abstract `IMongoHelper` that forwards every member to `protected abstract IMongoHelper Current()`, resolved on each call (for example a per-tenant database). Derive from it instead of implementing `IMongoHelper` by hand, so new interface members are forwarded without changes in your code. All members are virtual. The `database` and `dbName` setters throw `InvalidOperationException` with the overridable `protected virtual string NeverSetMessage`. |

```csharp
public sealed class TenantMongoHelper : DelegatingMongoHelper
{
    private readonly ITenantContext _tenant;
    public TenantMongoHelper(ITenantContext tenant) { _tenant = tenant; }
    protected override IMongoHelper Current() => _tenant.Mongo;
}
```

### Repository base

| Type | Description |
|---|---|
| `MongoRepository<TDoc>` | Abstract base with constructor `(IMongoHelper mongo, string collectionName)` and protected `Mongo` / `CollectionName`. Public methods: `GetByIdAsync`, `GetSingleAsync`, `GetFilteredAsync`, `GetAllAsync`, `GetPagedAsync`, `CountAsync`, `ExistsAsync`, `CreateAsync`, `UpsertAsync`, `ReplaceByIdAsync`, `UpdateByIdAsync`, `DeleteByIdAsync`, `DeleteManyAsync`. |
| `MongoDocument` | Abstract base with `string Id` mapped as `[BsonId]` / ObjectId, and `[BsonIgnoreExtraElements]`. |
| `CreatedMongoDocument` | Adds `DateTime CreatedAt` (UTC). For append-only records. |
| `TimestampedMongoDocument` | Adds `DateTime UpdatedAt` (UTC), equal to `CreatedAt` on insert. For mutable records. |
| `HistoriedMongoDocument` | Adds `List<UpdateStamp> UpdateHistory`, the newest changes capped by the repository. |
| `UpdateStamp` | One change: `DateTime At`, `string By` (user id, `system` or a job name), `List<string> Fields`. |
| `CreatedMongoRepository<TDoc>` | `CreateAsync` stamps `CreatedAt`. Override `UtcNow` for a fixed clock in tests. |
| `TimestampedMongoRepository<TDoc>` | Also stamps `UpdatedAt` on create, `UpdateByIdAsync`, `UpsertAsync` (plus `CreatedAt` on insert) and `ReplaceByIdAsync`. Your update must not set either field itself. |
| `HistoriedMongoRepository<TDoc>` | Also pushes an `UpdateStamp` on every write, keeping the newest `UpdateHistoryLimit` (default 50). Overloads taking `(string by, params string[] fields)` record who changed what; the plain ones record `system`. |

Property names are PascalCase with no `[BsonElement]` overrides, so element names follow the registered naming convention (`HybridElementNameConvention` stores `createdAt`, `updatedAt`, `UpdateHistory`). Skip the write entirely when nothing changed; every write counts as a change.

### Indexes

| Type | Description |
|---|---|
| `MongoIndexSpec` | `(string field, bool descending = false, bool unique = false, TimeSpan? expireAfter = null, string name = null)` or `(IReadOnlyList<MongoIndexKey> keys, ...)` for compound indexes. `ResolveName()` gives a deterministic name, so ensuring the same spec repeatedly is idempotent. |
| `MongoIndexKey` | `(string field, bool descending = false)`. |

### BSON and serialization

| Type | Description |
|---|---|
| `BsonClassMapRegistrar.RegisterClassMaps()` | Idempotent, thread safe. Registers `JObjectSerializer` for every Newtonsoft `JObject` and maps `AnointedAutomation.Objects.Account.User` with `UserId` as `_id` and extra elements ignored. |
| `BsonClassMapRegistrar.RegisterDerivedUser<T>()` | Registers a class derived from `User`. Call after `RegisterClassMaps()`. |
| `BsonClassMapRegistrar.RegisterHybridCasingConvention(string namespacePrefix = "AnointedAutomation")` | Opt in. Registers `HybridElementNameConvention` for types under the prefix. |
| `BsonClassMapRegistrar.RegisterSnakeCasingConvention(string namespacePrefix = "AnointedAutomation")` | Opt in. Registers `SnakeCaseElementNameConvention` (for example `LineItems` becomes `line_items`). |
| `HybridElementNameConvention` | Value type, enum and struct members become camelCase, reference type members PascalCase. Matches `JsonCasingConvention` in [AnointedAutomation.Objects.API](https://www.nuget.org/packages/AnointedAutomation.Objects.API). |
| `SnakeCaseElementNameConvention` | snake_case element names; `static string ToSnake(string)` is public. |
| `JObjectSerializer` | BSON serializer for Newtonsoft `JObject`. |
| `BsonMap` | `ToPlain(BsonValue)` and `ToDictionary(BsonDocument)` convert BSON into plain dictionaries, lists and CLR scalars (ObjectId becomes its hex string). |

## Durability, reads and transactions (3.1.0)

All opt-in. With no options a helper is exactly what it was before: no write concern or read preference is set on the
collection handle and no `hello` is sent, so the connection string decides.

```csharp
MongoDurabilityOptions durability = new MongoDurabilityOptions(
        MongoWriteConcernMode.W1, journal: null, MongoTransactionMode.Auto)
    .ForCollections(new[] { "payments", "refunds" }, MongoDurabilityPolicy.MoneyWrites);
MongoReadOptions reads = new MongoReadOptions(ReadPreference.Primary);

IMongoHelperFactory factory = new MongoHelperFactory(durability, reads);
MongoHelper helper = (MongoHelper)factory.Create("shop", connectionString);
await helper.ValidateAsync(); // optional: fail at startup instead of on first use

await helper.RunInTransactionAsync(async session =>
{
    IMongoCollection<Payment> payments = helper.GetCollection<Payment>("payments");
    if (session == null) { await payments.InsertOneAsync(payment); }
    else { await payments.InsertOneAsync(session, payment); }
});
```

| Setting | Standalone mongod | Replica set or sharded cluster |
|---|---|---|
| `WriteConcern.Default` | driver / connection string default | same |
| `WriteConcern.W1` | acknowledged by the server | acknowledged by the primary only; a failover can roll it back |
| `WriteConcern.Majority` | **throws** `InvalidOperationException` on validation (no majority to wait for) | acknowledged by a majority; survives failover |
| `Journal = true` | waits for the on-disk journal | waits for the journal (with Majority: on a majority) |
| `Journal = null` | server default | server default |
| `Transactions.Off` | callback runs with a null session | same |
| `Transactions.Auto` | callback runs with a null session (no transactions exist); `EffectiveTransactionMode` is `Off` | session + transaction; `EffectiveTransactionMode` is `Required` |
| `Transactions.Required` | **throws** `InvalidOperationException` | session + transaction (driver `WithTransactionAsync`, retries transient errors) |

- Validation runs `hello` once per helper (on first use when `Majority` or transactions are configured, or when you call
  `ValidateAsync`). A reply with `setName` is a replica set, `msg: "isdbgrid"` is sharded, anything else is standalone.
  Nothing is silently downgraded: if a setting cannot be honored the helper throws.
- A single-node replica set (`mongod --replSet rs0` plus `rs.initiate()`) is enough for `Majority` and transactions in
  development.
- Inside a transaction the driver ignores per-collection write concerns, so `RunInTransactionAsync` starts the
  transaction with the strictest configured policy (for example `MoneyWrites` if any collection uses it), read concern
  majority for a majority policy, and read preference primary. With no policy it uses the driver defaults.
  `BuildTransactionOptions()` returns the same options if you start your own session.
- `MongoReadOptions` only chooses which member serves reads. It is not a durability setting.
- Per-collection policies match the exact collection name (ordinal) and replace the global write concern and journal for
  that collection only.
- Write concern errors (`MongoWriteConcernException`) reach the caller unchanged.

## Casing conventions

Both conventions are opt in, so installing or upgrading the package changes nothing on its own. Register one once at startup, before `RegisterClassMaps()` and before any Mongo call, and only after your stored data uses the matching key casing:

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
- **Pets:** [https://www.animaldander.com](https://www.animaldander.com)
- **Everything else:** [https://www.mart.club](https://www.mart.club)

Found a bug or have a request? Open an issue at [https://github.com/AnointedAutomation/AnointedAutomation/issues](https://github.com/AnointedAutomation/AnointedAutomation/issues).


## 1.3.0 additions

- `AnointedAutomation.Repository.Mongo.Bson.HybridCasing.Register(namespacePrefix, denyList, registerClassMaps, toleranceAssemblies, ignoreExtraElements)`: one idempotent bootstrap that registers the hybrid convention, runs your class maps, then applies IgnoreExtraElements to everything else, in that order.
- `Bson.BsonToleranceRegistrar.RegisterIgnoreExtraElements(assemblies, namespacePrefix)`.
- `Bson.LenientStringSerializer`: reads Int32/Int64/Double legacy values as strings.
- `AnointedAutomation.Repository.Mongo.Bson.BsonValueExtensions`: lenient, never-throwing BsonDocument/BsonValue readers.
- The casing conventions now delegate to `AnointedAutomation.Serialization.Naming.NamingRules`.
