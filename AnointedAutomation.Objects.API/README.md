# AnointedAutomation.Objects.API

ASP.NET Core companions to [AnointedAutomation.Objects](https://www.nuget.org/packages/AnointedAutomation.Objects): an in-memory `IFormFile`, the shared hybrid-casing `System.Text.Json` convention used by AnointedAutomation APIs, and a never-throwing GraphQL response reader.

[![NuGet](https://img.shields.io/nuget/v/AnointedAutomation.Objects.API.svg)](https://www.nuget.org/packages/AnointedAutomation.Objects.API) [![Downloads](https://img.shields.io/nuget/dt/AnointedAutomation.Objects.API.svg)](https://www.nuget.org/packages/AnointedAutomation.Objects.API)

## Installation

```bash
dotnet add package AnointedAutomation.Objects.API
```

- Target framework: `net10.0`
- Framework reference: `Microsoft.AspNetCore.App` (ASP.NET Core shared framework)
- Dependencies:
  - [AnointedAutomation.Objects](https://www.nuget.org/packages/AnointedAutomation.Objects) (brings the account, billing and `ResponseData` models)

## Quick start

Apply the JSON casing convention to both MVC controllers and minimal APIs:

```csharp
using AnointedAutomation.Objects.API;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(o => JsonCasingConvention.Configure(o.JsonSerializerOptions));
builder.Services.ConfigureHttpJsonOptions(o => JsonCasingConvention.Configure(o.SerializerOptions));

WebApplication app = builder.Build();
app.MapControllers();
app.Run();
```

## API overview

All types are in the `AnointedAutomation.Objects.API` namespace.

| Type | Description |
|---|---|
| `JsonCasingConvention` | `static void Configure(JsonSerializerOptions options)` adds the convention to existing options; `static JsonSerializerOptions Options` is a ready-made, reusable instance for serializing outside the MVC/minimal API pipeline. |
| `CustomFormFile` | `IFormFile` backed by a `byte[]`. Constructor `(string fileName, byte[] content)`; properties `FileName`, `Content`, `Length`, `ContentType` (always `application/octet-stream`), `Name` (always `file`), `ContentDisposition`, `Headers`; methods `CopyTo`, `CopyToAsync`, `OpenReadStream`. |
| `GraphQlEnvelope` | `static GraphQlResult Read(string body)` parses a GraphQL HTTP response body. Never throws: empty or invalid bodies become transport errors. |
| `GraphQlResult` | `Data` (`JsonElement`), `HasData`, `TransportErrors`, `Errors`, and per-mutation helpers `Payload(field)`, `UserErrors(field)`, `FirstError(field)`, `Succeeded(field)`. |
| `GraphQlError` | One top-level error: `Message`, plus raw JSON `Path` and `Extensions` (or null). |

## JSON casing rules

`JsonCasingConvention` makes the wire casing match the AnointedAutomation model convention while your C# stays PascalCase:

- Classes: value-type members (`int`, `bool`, `DateTime`, enums, and their nullables) are camelCase; reference-type members (strings, objects, arrays) are PascalCase.
- Structs: every member is camelCase.
- Enums serialize as camelCase strings (integers are still accepted on input).
- A member with `[JsonPropertyName]` keeps its explicit name, and anonymous types are left untouched.

```csharp
using System.Text.Json;
using AnointedAutomation.Objects.API;

string json = JsonSerializer.Serialize(
    new OrderDto { Id = 7, Name = "Hoodie", TotalPrice = 49.99m },
    JsonCasingConvention.Options);
// {"id":7,"Name":"Hoodie","totalPrice":49.99}

public class OrderDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal TotalPrice { get; set; }
}
```

The same rule is applied to MongoDB documents by `HybridElementNameConvention` in [AnointedAutomation.Repository.Mongo](https://www.nuget.org/packages/AnointedAutomation.Repository.Mongo).

## Reading GraphQL responses

```csharp
using AnointedAutomation.Objects.API;

string body = "{\"data\":{\"productUpdate\":{\"userErrors\":[{\"message\":\"Title can't be blank\"}]}}}";

GraphQlResult result = GraphQlEnvelope.Read(body);
if (!result.Succeeded("productUpdate"))
{
    string error = result.FirstError("productUpdate"); // "Title can't be blank"
}
```

`FirstError` returns the first top-level error if any, otherwise the first `data.{field}.userErrors[].message`, otherwise null. The shape matches the Shopify Admin GraphQL API, but nothing here is Shopify-specific.

## Building a form file in memory

```csharp
using System.IO;
using System.Threading.Tasks;
using AnointedAutomation.Objects.API;
using Microsoft.AspNetCore.Http;

public static class Uploads
{
    public static async Task<long> SaveAsync(byte[] bytes, string path)
    {
        IFormFile file = new CustomFormFile("report.pdf", bytes);
        using FileStream stream = File.Create(path);
        await file.CopyToAsync(stream);
        return file.Length;
    }
}
```

Useful for tests, or for passing generated content to code that expects an `IFormFile`.

## Related packages

- [AnointedAutomation.Objects](https://www.nuget.org/packages/AnointedAutomation.Objects): the core models (`User`, billing records, `ResponseData<T>`, `PaginatedResponse<T>`).
- [AnointedAutomation.Repository.Mongo](https://www.nuget.org/packages/AnointedAutomation.Repository.Mongo): the matching Mongo casing convention.
- [AnointedAutomation.APIMiddlewares](https://www.nuget.org/packages/AnointedAutomation.APIMiddlewares): ASP.NET Core middleware for API keys and IP blocking.

## License

MIT. See [LICENSE](https://github.com/AnointedAutomation/AnointedAutomation/blob/master/LICENSE).

## Support This Project

This library is free and open source. The best way to support the work is to shop with us:

- **Christian items:** [https://store.anointed.company](https://store.anointed.company)
- **Everything else:** [https://www.mart.club](https://www.mart.club)

Found a bug or have a request? Open an issue at [https://github.com/AnointedAutomation/AnointedAutomation/issues](https://github.com/AnointedAutomation/AnointedAutomation/issues).
