# MongoHelper

## Overview

The **MongoHelper** library, developed by Anointed Automation, LLC, provides a simple and efficient wrapper for MongoDB database operations in .NET. It offers utility methods to handle common MongoDB operations such as creating, reading, updating, and deleting documents, alongside helper functions for managing connections, logging, and BSON serialization.

---

## Features

- **Base Document Classes**: `MongoDocument` and `AuditableMongoDocument` for consistent entity definitions.
- Simplifies MongoDB connection setup.
- CRUD operations for MongoDB documents:
  - Create: Insert documents.
  - Read: Fetch documents using filters or retrieve all documents.
  - Update: Modify existing documents.
  - Delete: Remove documents based on filters.
- **BSON Class Map Registration**: Automatic configuration of base objects for MongoDB serialization.
- **JObject Serialization**: Built-in support for Newtonsoft.Json JObject <-> MongoDB BSON conversion.
- Logging functionality to capture MongoDB activity.
- Event-driven log management (`LogAdded`, `LogCleared`).
- Connection string builder for secure MongoDB access.
- Helper methods to extract document IDs dynamically.

---

## Installation

Install via NuGet:

```bash
dotnet add package AnointedAutomation.Repository.Mongo
```

Or clone the repository and include the library in your project. Ensure you have the following dependencies installed:

- `MongoDB.Driver`
- `AnointedAutomation.Objects`
- `AnointedAutomation.Logging`
- `Newtonsoft.Json`

---

## Usage

### Setting Up BsonClassMapRegistrar (IMPORTANT)

Before performing any MongoDB operations, you must register the BSON class maps. Call this once at application startup:

```csharp
using AnointedAutomation.Repository.Mongo;

// In your Program.cs or startup code:
BsonClassMapRegistrar.RegisterClassMaps();
```

This registers:
- **User class** with `UserId` as the MongoDB document `_id`
- **JObjectSerializer** for Newtonsoft.Json JObject properties
- Proper inheritance handling for derived user classes

### Registering Derived User Classes

If you have a custom user class that extends `User`, register it after the base class maps:

```csharp
using AnointedAutomation.Repository.Mongo;

// Register base class maps first
BsonClassMapRegistrar.RegisterClassMaps();

// Then register your derived user class
BsonClassMapRegistrar.RegisterDerivedUser<MyCustomUser>();
```

This ensures proper BSON serialization for your custom user types in MongoDB.

### Using Base Document Classes

The library provides two abstract base classes for your MongoDB entities:

#### MongoDocument

Basic base class with proper ObjectId handling:

```csharp
using AnointedAutomation.Repository.Mongo;

public class Product : MongoDocument
{
    public string Name { get; set; }
    public decimal Price { get; set; }
}
```

This gives you:
- `Id` property with `[BsonId]` and `[BsonRepresentation(BsonType.ObjectId)]`
- `[BsonIgnoreExtraElements]` for forward compatibility

#### AuditableMongoDocument

Extends `MongoDocument` with audit timestamps:

```csharp
using AnointedAutomation.Repository.Mongo;

public class Order : AuditableMongoDocument
{
    public string CustomerId { get; set; }
    public decimal Total { get; set; }
}
```

This adds:
- `createdAt` - creation timestamp (DateTime is a value type)
- `updatedAt` - last update timestamp

### Setting Up MongoHelper

```csharp
using AnointedAutomation.Repository.Mongo;

// Initialize with database name and connection string
MongoHelper mongoHelper = new MongoHelper("YourDatabaseName", "YourConnectionString");
```

### Connection String Builder

You can dynamically create a secure connection string:

```csharp
string connectionString = MongoHelper.ConnectionStringBuilder(
    "username",
    "password",
    "clusterName",
    "region"
);
```

### Basic CRUD Operations

#### Create a Document
```csharp
await mongoHelper.CreateDocumentAsync("CollectionName", new { Name = "John Doe", Age = 30 });
```

#### Read Documents
Fetch all documents:
```csharp
var documents = await mongoHelper.GetAllDocumentsAsync<MyDocument>("CollectionName");
```

Fetch documents with a filter:
```csharp
var filter = Builders<MyDocument>.Filter.Eq(doc => doc.Name, "John Doe");
var filteredDocuments = await mongoHelper.GetFilteredDocumentsAsync("CollectionName", filter);
```

#### Update a Document
```csharp
var filter = Builders<MyDocument>.Filter.Eq(doc => doc.Id, 1);
var update = Builders<MyDocument>.Update.Set(doc => doc.Name, "Jane Doe");
await mongoHelper.UpdateDocumentAsync("CollectionName", filter, update);
```

#### Delete a Document
```csharp
var filter = Builders<MyDocument>.Filter.Eq(doc => doc.Id, 1);
await mongoHelper.DeleteDocumentAsync("CollectionName", filter);
```

### Testing Database Connection
```csharp
List<string> collections = mongoHelper.TestConnection();
if (collections != null)
{
    Console.WriteLine("Connection successful!");
}
```

---

## Database-Agnostic Objects

This library is designed to work with pure POCO objects from `AnointedAutomation.Objects`. The BSON serialization is configured via `BsonClassMap` rather than attributes, allowing the same object classes to be used with:

- **MongoDB** (using this library)
- **SQL Server** (using Entity Framework Fluent API)
- **MySQL** (using Entity Framework or Dapper)
- **Any other database** (just add appropriate configuration)

No need for separate `MongoUser`, `SqlUser`, etc. classes - just use the base `User` class and configure serialization externally.

---

## Events

### `LogAdded` Event

Triggered when a new log is added:

```csharp
MongoHelper.LogAdded += (sender, args) =>
{
    Console.WriteLine($"New Log: {args.LogMessage}");
};
```

### `LogCleared` Event

Triggered when logs are cleared:

```csharp
MongoHelper.LogCleared += (sender, args) =>
{
    Console.WriteLine("Logs cleared.");
};
```

---

## Logging

### Retrieve Logs
```csharp
var logs = MongoHelper.GetLogs();
foreach (var log in logs)
{
    Console.WriteLine(log.Message);
}
```

### Clear Logs
```csharp
MongoHelper.ClearLogs();
```

---

## Requirements

- .NET 8.0 or later
- MongoDB Instance
- NuGet Packages:
  - MongoDB.Driver
  - AnointedAutomation.Objects
  - AnointedAutomation.Logging
  - Newtonsoft.Json

---

## License

This project is licensed under the MIT License. See the `LICENSE` file for details.

---

## Author

Created by **Alexander Fields**
Copyright © 2023 Anointed Automation, LLC

For inquiries, please contact [Anointed Automation](https://anointedautomation.com).
GitHub: [https://github.com/AnointedAutomation](https://github.com/AnointedAutomation)
