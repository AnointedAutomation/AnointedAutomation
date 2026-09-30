# AnointedAutomation.Optimization

Reusable optimization utilities for .NET: DataTable/DataReader conversion, CSV generation, string and array helpers, chunking, Roman numerals, JSON flattening, file management, and null-safe field accessors over `System.Text.Json.JsonElement` and MongoDB `BsonDocument`.

[![NuGet](https://img.shields.io/nuget/v/AnointedAutomation.Optimization.svg)](https://www.nuget.org/packages/AnointedAutomation.Optimization) [![Downloads](https://img.shields.io/nuget/dt/AnointedAutomation.Optimization.svg)](https://www.nuget.org/packages/AnointedAutomation.Optimization)

## Installation

```bash
dotnet add package AnointedAutomation.Optimization
```

- Target framework: `net10.0`
- Dependencies: `MongoDB.Bson` (used only by the `BsonMap` accessors)

Namespace: `AnointedAutomation.Optimization`.

## What is inside

| Type | Purpose |
|---|---|
| `Utility` | DataTable/DataReader to list mapping, CSV writers/readers, base64 encode/decode, random string generation, chunking (`ToChunks`), jagged array conversion, substring helpers, recursive delete. |
| `ChunkDataReader` | An in-memory `IDataReader` over a list of rows, for feeding batched data into readers/exporters. |
| `JsonFlattener` | Flattens nested JSON into a single-level dictionary of underscore-joined keys; dictionary to single-row `DataTable`. Built on the dependency-free `System.Text.Json`. |
| `Roman` | Integer to/from Roman numerals (with overline multipliers for large numbers) and whole-word detection. |
| `FileManagement` | Idempotent directory/file create, delete (with try variants), in-place line replacement, download-to-file. |
| `FileComparator` | Byte-for-byte file compare and replace. |
| `JsonElementExtensions` | Null-safe, strict and coercing accessors over `JsonElement` (strings, numbers, booleans, arrays, GraphQL connection enumeration). |
| `BsonMap` | Null-safe, strict and coercing accessors over `BsonDocument` / `BsonValue`. |

## Quick start

```csharp
using AnointedAutomation.Optimization;

// Chunk a big sequence.
foreach (var batch in bigList.ToChunks(500))
{
    Process(batch);
}

// Read a JSON field safely.
double? price = element.GetDoubleCoercedOrNull("price", NumberStyles.Any);

// Roman numerals.
string mmxxvi = Roman.IntegerToRoman(2026); // "MMXXVI"
```

## License

MIT. See [LICENSE](https://github.com/AnointedAutomation/AnointedAutomation/blob/master/LICENSE).
Copyright © Anointed Automation, LLC. Stewarded by Alexander Fields.

## Support This Project

This library is free and open source. The best way to support the work is to shop with us:

- **Christian items:** [https://store.anointed.company](https://store.anointed.company)
- **Everything else:** [https://www.mart.club](https://www.mart.club)

Found a bug or have a request? Open an issue: [https://github.com/AnointedAutomation/AnointedAutomation/issues](https://github.com/AnointedAutomation/AnointedAutomation/issues)
