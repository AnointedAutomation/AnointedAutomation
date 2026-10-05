# AnointedAutomation.Serialization

Jesus is King ✝️

Shared serialization helpers for .NET 10. No MongoDB dependency (the BSON pieces live in `AnointedAutomation.Repository.Mongo`).

- `AnointedAutomation.Serialization.Naming.NamingRules`: `ToCamel`, `ToCamelJson`, `ToPascal`, `ToSnake`, and the hybrid rule (`IsCamelMember` / `ToHybrid`: value-type, enum and struct members camelCase, reference-type members PascalCase).
- `AnointedAutomation.Serialization.SystemTextJson`: `JsonCasingConvention`, `UtcDateTimeJsonConverter`, `JsonPresets` (shared read-only option sets) and `AnointedJson.ConfigureApi` (one call for MVC and Minimal API hosts).
- `AnointedAutomation.Serialization.Newtonsoft`: `TolerantEnumConverter`, `StringOrBoolConverter`, `DateFormatConverter`, `NullOnErrorConverter`.
- `AnointedAutomation.Serialization.Json`: lenient `JsonElement` readers (`JsonElementExtensions`, `ResponseJson`).

```csharp
builder.Services.AddControllers().AddJsonOptions(o => AnointedJson.ConfigureApi(o.JsonSerializerOptions));
builder.Services.ConfigureHttpJsonOptions(o => AnointedJson.ConfigureApi(o.SerializerOptions));
```

- Target framework: `net10.0`
- Dependencies: `Newtonsoft.Json`

## License

MIT. `DateFormatConverter` and `NullOnErrorConverter` are derived from [ShopifySharp](https://github.com/nozzlegear/ShopifySharp) (Copyright (c) 2015 Joshua Harms, MIT).
