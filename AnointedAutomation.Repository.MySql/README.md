# AnointedAutomation.Repository.MySql

A generic repository and helper for MySQL on Entity Framework Core (Pomelo provider). `GenericRepository<T>` wraps any `DbContext` with async CRUD, paging, ordering and projection, and `MySqlHelper` adds connection testing, raw SQL, transactions and a log buffer, following the same style as [AnointedAutomation.Repository.Mongo](https://www.nuget.org/packages/AnointedAutomation.Repository.Mongo).

[![NuGet](https://img.shields.io/nuget/v/AnointedAutomation.Repository.MySql.svg)](https://www.nuget.org/packages/AnointedAutomation.Repository.MySql) [![Downloads](https://img.shields.io/nuget/dt/AnointedAutomation.Repository.MySql.svg)](https://www.nuget.org/packages/AnointedAutomation.Repository.MySql)

## Installation

```bash
dotnet add package AnointedAutomation.Repository.MySql
```

- Target framework: `net10.0`
- Dependencies:
  - [AnointedAutomation.Logging](https://www.nuget.org/packages/AnointedAutomation.Logging)
  - [Microsoft.EntityFrameworkCore](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore) 10.0.12
  - [Pomelo.EntityFrameworkCore.MySql](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql) 9.0.0

## Quick start

Use `GenericRepository<T>` over your own `DbContext`, which defines the entity model:

```csharp
using System.Collections.Generic;
using System.Threading.Tasks;
using AnointedAutomation.Repository.MySql;
using Microsoft.EntityFrameworkCore;

public class Customer
{
    public int Id { get; set; }
    public string Email { get; set; }
    public bool IsActive { get; set; }
}

public class ShopDbContext : DbContext
{
    public ShopDbContext(DbContextOptions<ShopDbContext> options) : base(options) { }
    public DbSet<Customer> Customers { get; set; }
}

public static class Example
{
    public static async Task RunAsync(string connectionString)
    {
        DbContextOptions<ShopDbContext> options = new DbContextOptionsBuilder<ShopDbContext>()
            .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
            .Options;

        using ShopDbContext context = new ShopDbContext(options);
        IGenericRepository<Customer> customers = new GenericRepository<Customer>(context);

        Customer added = await customers.AddAsync(new Customer { Email = "john@example.com", IsActive = true });
        Customer byId = await customers.GetByIdAsync(added.Id);
        IEnumerable<Customer> active = await customers.FindAsync(c => c.IsActive);
        IEnumerable<Customer> firstPage = await customers.GetPagedAsync(c => c.IsActive, c => c.Email, descending: false, skip: 0, take: 20);
        int count = await customers.CountAsync();
        await customers.DeleteAsync(added.Id);
    }
}
```

## API overview

All types are in the `AnointedAutomation.Repository.MySql` namespace.

### `IGenericRepository<T>` / `GenericRepository<T>`

Constructor: `GenericRepository<T>(DbContext context)`. Every write calls `SaveChangesAsync` immediately.

| Area | Methods |
|---|---|
| Read | `GetByIdAsync(object id)`, `GetAllAsync()`, `FindAsync(predicate)`, `FindSingleAsync(predicate)`, `GetPagedAsync(predicate, orderBy, descending, skip, take)`, `GetOrderedAsync(orderBy, descending, predicate = null)`, `SelectAsync(predicate, selector)`, `ExistsAsync(predicate)`, `AnyAsync(predicate = null)`, `CountAsync(predicate = null)` |
| Write | `AddAsync(entity)`, `AddRangeAsync(entities)`, `UpdateAsync(entity)`, `UpdateRangeAsync(entities)`, `DeleteAsync(object id)`, `DeleteAsync(T entity)`, `DeleteRangeAsync(predicate)` |

### `IMySqlHelper` / `MySqlHelper`

| Member | Description |
|---|---|
| `DbContext Database`, `string ConnectionString`, `string DbName` | Settable state. `new MySqlHelper(string connectionString)` sets `ConnectionString` and creates `Database` via `CreateDbContext`. |
| `CreateAsync`, `GetByIdAsync`, `GetAllAsync`, `GetFilteredAsync`, `FirstOrDefaultAsync`, `UpdateAsync`, `DeleteAsync`, `ExistsAsync`, `AnyAsync`, `CountAsync`, `GetPagedAsync`, `GetOrderedAsync`, `SelectAsync` | Generic (`<T>`) versions of the repository operations, run against `Database`. |
| `ExecuteRawSqlAsync(string sql, params object[] parameters)` | Runs `ExecuteSqlRawAsync`; returns rows affected. |
| `ExecuteInTransactionAsync(Func<Task> action)` | Commits if the action succeeds, rolls back and rethrows if it throws. |
| `TestConnection()` | `true` if the database is reachable; logs success or the error. |
| `GetTableNames()` | Table names from `INFORMATION_SCHEMA` for the current database, or null on error. |
| `CreateDbContext(string connectionString)` | Creates a `DynamicDbContext` using Pomelo with `ServerVersion.AutoDetect` (this opens a connection to detect the server version). |
| `static ConnectionStringBuilder(server, database, username, password, int port = 3306)` | Builds `Server=...;Port=...;Database=...;User=...;Password=...;`. |
| `static MySqlHelperConnector(IMySqlHelper, string connectionString)` | Creates the context, sets the connection string and tests the connection. |
| `static GetLogs()`, `static ClearLogs()`, events `LogAdded`, `LogCleared` | Static `LogMessage` buffer. |

### Other types

| Type | Description |
|---|---|
| `IMySqlHelperFactory` / `MySqlHelperFactory` | `Create(string connectionString)` returns one cached `IMySqlHelper` per connection string; `Remove(connectionString)` and `ClearCache()` evict. |
| `DynamicDbContext` | The empty `DbContext` created by `MySqlHelper.CreateDbContext`. |

## Using `MySqlHelper` with your own model

`DynamicDbContext` has no entity types configured, so the helper's generic CRUD methods only work once `Database` is a context that maps your entities. Assign your own context:

```csharp
using AnointedAutomation.Repository.MySql;
using Microsoft.EntityFrameworkCore;

public static class HelperSetup
{
    public static IMySqlHelper Create(DbContext appContext, string connectionString)
    {
        IMySqlHelper helper = new MySqlHelper
        {
            Database = appContext,
            ConnectionString = connectionString
        };
        return helper;
    }
}
```

With your context in place, `TestConnection`, `GetTableNames`, `ExecuteRawSqlAsync`, `ExecuteInTransactionAsync` and the generic CRUD methods all run against it.

## Dependency injection

`GenericRepository<T>` takes a `DbContext`, so expose your context under that base type:

```csharp
using AnointedAutomation.Repository.MySql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public static class MySqlSetup
{
    public static IServiceCollection AddShopData(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<ShopDbContext>(o =>
            o.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
        services.AddScoped<DbContext>(sp => sp.GetRequiredService<ShopDbContext>());
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        return services;
    }
}
```

`AddDbContext` comes from Microsoft.EntityFrameworkCore; the `IServiceCollection` extension style assumes an ASP.NET Core or Generic Host app.

## Related packages

- [AnointedAutomation.Logging](https://www.nuget.org/packages/AnointedAutomation.Logging): the `LogMessage` type used for the log buffer.
- [AnointedAutomation.Repository.Mongo](https://www.nuget.org/packages/AnointedAutomation.Repository.Mongo): the MongoDB counterpart.
- [AnointedAutomation.Objects](https://www.nuget.org/packages/AnointedAutomation.Objects): plain models you can map with EF Core.

## License

MIT. See [LICENSE](https://github.com/AnointedAutomation/AnointedAutomation/blob/master/LICENSE).

## Support This Project

This library is free and open source. The best way to support the work is to shop with us:

- **Christian items:** [https://store.anointed.company](https://store.anointed.company)
- **Everything else:** [https://www.mart.club](https://www.mart.club)

Found a bug or have a request? Open an issue at [https://github.com/AnointedAutomation/AnointedAutomation/issues](https://github.com/AnointedAutomation/AnointedAutomation/issues).
