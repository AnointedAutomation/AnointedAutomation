# AnointedAutomation.Repository.MySql

Generic MySQL repository library using Entity Framework Core. Provides a consistent interface for database operations, mirroring the patterns established in `AnointedAutomation.Repository.Mongo`.

## Features

- Generic CRUD operations via `IGenericRepository<T>`
- Connection pooling via `MySqlHelperFactory` with `ConcurrentDictionary` caching
- Async/await pattern throughout
- Integration with `AnointedAutomation.Logging`
- Fail-fast on missing configuration (no fallback values)

## Installation

```bash
dotnet add package AnointedAutomation.Repository.MySql
```

## Usage

### Basic Setup

```csharp
// Create factory (singleton recommended)
IMySqlHelperFactory factory = new MySqlHelperFactory();

// Get or create a MySQL helper instance
string connectionString = "Server=localhost;Database=mydb;User=root;Password=pass;";
IMySqlHelper helper = factory.Create(connectionString);

// Use generic repository
IGenericRepository<User> userRepo = new GenericRepository<User>(helper.Database);
User user = await userRepo.GetByIdAsync(1);
```

### With Dependency Injection

```csharp
// In Program.cs
services.AddSingleton<IMySqlHelperFactory, MySqlHelperFactory>();
services.AddScoped<IMySqlHelper>(sp =>
{
    IMySqlHelperFactory factory = sp.GetRequiredService<IMySqlHelperFactory>();
    string connectionString = Environment.GetEnvironmentVariable("MYSQL_CONNECTION_STRING");
    return factory.Create(connectionString);
});
services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
```

### Generic Repository Operations

```csharp
// Create
User newUser = await userRepo.AddAsync(new User { Name = "John" });

// Read
User user = await userRepo.GetByIdAsync(1);
IEnumerable<User> allUsers = await userRepo.GetAllAsync();
IEnumerable<User> admins = await userRepo.FindAsync(u => u.Role == "admin");

// Update
user.Name = "Jane";
await userRepo.UpdateAsync(user);

// Delete
await userRepo.DeleteAsync(1);

// Utility
bool exists = await userRepo.ExistsAsync(u => u.Email == "test@example.com");
int count = await userRepo.CountAsync(u => u.IsActive);
```

## License

MIT License - See LICENSE file for details.

## Author

Created by Alexander Fields
GitHub: [https://github.com/AnointedAutomation](https://github.com/AnointedAutomation)
