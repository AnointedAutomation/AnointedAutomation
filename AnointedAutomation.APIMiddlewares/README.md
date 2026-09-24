# AnointedAutomation.APIMiddlewares

ASP.NET Core middleware and filters for protecting small APIs: API key authentication, an in memory IP blacklist, automatic banning of clients that probe for endpoints that do not exist, and optional garbage collection when the API has been idle.

[![NuGet](https://img.shields.io/nuget/v/AnointedAutomation.APIMiddlewares.svg)](https://www.nuget.org/packages/AnointedAutomation.APIMiddlewares) [![Downloads](https://img.shields.io/nuget/dt/AnointedAutomation.APIMiddlewares.svg)](https://www.nuget.org/packages/AnointedAutomation.APIMiddlewares)

## Installation

```bash
dotnet add package AnointedAutomation.APIMiddlewares
```

- Target framework: `net10.0`
- Framework reference: `Microsoft.AspNetCore.App` (ASP.NET Core shared framework)
- Dependencies:
  - [AnointedAutomation.Logging](https://www.nuget.org/packages/AnointedAutomation.Logging)
  - [AnointedAutomation.Memory](https://www.nuget.org/packages/AnointedAutomation.Memory)

## Quick start

```csharp
using System;
using AnointedAutomation.APIMiddleware;
using Microsoft.AspNetCore.Builder;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
WebApplication app = builder.Build();

// Reject blacklisted IPs with 403 before anything else runs.
app.UseMiddleware<IPBlacklistMiddleware>();

// Count 404/401/403 responses per IP and ban repeat offenders.
app.UseMiddleware<InvalidEndpointTrackerMiddleware>();

// Require an API key header on everything under /api (constant-time comparison).
app.UseMiddleware<ApiKeyAuthMiddleware>(
    "X-Api-Key",                                          // header name
    Environment.GetEnvironmentVariable("MY_API_KEY"),     // expected key
    "/api");                                              // protected path prefix

// Track endpoint hits; run a GC when every endpoint has been idle for 10 minutes.
app.UseMiddleware<EndpointAccessMiddleware>((TimeSpan?)TimeSpan.FromMinutes(10), true);

app.MapControllers();
app.Run();
```

Note the namespace: the code lives in `AnointedAutomation.APIMiddleware` (singular), even though the package id is `AnointedAutomation.APIMiddlewares`.

## API overview

### Middleware (`AnointedAutomation.APIMiddleware`)

| Type | What it does |
|---|---|
| `ApiKeyAuthMiddleware` | Constructor `(RequestDelegate next, string headerName, string expectedKey, string protectedPathPrefix)`. Requests whose path starts with the prefix must send the header with the exact key or get `401`. Uses `CryptographicOperations.FixedTimeEquals`. Also exposes `static bool IsAuthorized(string presentedKey, byte[] expectedKey)`. The prefix must start with `/`. |
| `IPBlacklistMiddleware` | Returns `403` for any client IP in `IPBlacklist`. Also owns the shared middleware log buffer: `static AddLog(LogMessage)`, `static GetLogs()`, `static ClearLogs()`, and the static events `LogAdded` and `LogCleared`. |
| `InvalidEndpointTrackerMiddleware` | Returns `403` for blacklisted IPs. After the rest of the pipeline runs, records a failed attempt for a `404` with an empty body (except `/`), a `401` or a `403`. An IP is banned after 10 failed attempts, or immediately if it requests a path ending in `.env`. Attempt counts and logs reset every 24 hours. `static ClearFailedAttempts()` resets counts. |
| `AttemptInfo` | Attempt record for one IP: `int Count`, `HashSet<string> Paths`. |
| `EndpointAccessMiddleware` | Constructor `(RequestDelegate next, TimeSpan? timeout, bool cleanMem = false)`. Records the last access time per path. When `cleanMem` is true, a one minute timer runs `AnointedAutomation.Optimization.Memory.GarbageCollection.PerformGarbageCollection` once every endpoint has been idle longer than `timeout`. `static bool HasBeenHitRecently(string path)` reports whether a path was hit in the last 5 minutes. |

### Filters (`AnointedAutomation.APIMiddleware.Filters`)

| Type | What it does |
|---|---|
| `APIKeyAttribute` | `[APIKey]` action filter for controllers or actions. Reads the header name from the `API_KEY_NAME` environment variable and the expected key from `API_KEY`; a missing or wrong key returns `401` and logs a warning with the client IP. |

### Objects (`AnointedAutomation.APIMiddleware.Objects`)

| Type | What it does |
|---|---|
| `IPBlacklist` | Static in memory blacklist: `AddBannedIP(string ip, string reason)`, `RemoveBannedIP(string ip)`, `IsIPBlocked(string ipAddress)`, `GetBlockReason(string ipAddress)`, `ClearBlacklist()`, and the events `IPBanned` (`EventHandler<BannedIP>`) and `IPUnbanned` (`EventHandler<string>`). |
| `BannedIP` | `_id`, `Ipv4`, `Ipv6`, `Reason`. Handy as a persistence shape for bans. |

### Utility (`AnointedAutomation.APIMiddleware.Utility`)

| Type | What it does |
|---|---|
| `APIUtility` | `GetClientPublicIPAddress(HttpContext)` and `GetClientPublicIPAddress(ActionExecutingContext)`. Uses the first `X-Forwarded-For` value, then `RemoteIpAddress`. Returns `198.51.100.255` for `::1` or when no IP is available, and logs why. |

## Using the `[APIKey]` filter

```csharp
using AnointedAutomation.APIMiddleware.Filters;
using Microsoft.AspNetCore.Mvc;

[APIKey]
[ApiController]
[Route("[controller]")]
public class SecureController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok("Secure data");
}
```

Required environment variables:

```
API_KEY_NAME=X-Api-Key
API_KEY=your-secret-key
```

`ApiKeyAuthMiddleware` is the newer option: it takes its values as arguments instead of reading environment variables, and compares keys in constant time.

## Persisting and observing bans

The blacklist is in memory and is lost on restart. Use the events to persist bans and reload them at startup:

```csharp
using System;
using AnointedAutomation.APIMiddleware;
using AnointedAutomation.APIMiddleware.Objects;
using AnointedAutomation.Optimization.Logging;

IPBlacklist.IPBanned += (sender, banned) =>
    Console.WriteLine($"Banned {banned.Ipv4}: {banned.Reason}");

IPBlacklist.AddBannedIP("203.0.113.7", "Loaded from database");

IPBlacklistMiddleware.LogAdded += (sender, e) =>
    Console.WriteLine($"[{e.log.messageType}] {e.log.message}");

IPBlacklistMiddleware.AddLog(LogMessage.Warning("Suspicious activity detected."));
```

Note that `IPBlacklistMiddleware.ClearLogs()` and `IPBlacklist.ClearBlacklist()` also remove every event subscriber, and `InvalidEndpointTrackerMiddleware` calls `ClearLogs()` during its 24 hour reset, so resubscribe to `IPBlacklistMiddleware.LogAdded` if you rely on it long term.

## Notes

- The client IP comes from `X-Forwarded-For` when present. Only trust it behind a reverse proxy that sets it.
- All state (blacklist, attempt counts, logs) is static and per process.

## Related packages

- [AnointedAutomation.Logging](https://www.nuget.org/packages/AnointedAutomation.Logging): the `LogMessage` type used for the middleware log buffer.
- [AnointedAutomation.Memory](https://www.nuget.org/packages/AnointedAutomation.Memory): the garbage collection helper used by `EndpointAccessMiddleware`.
- [AnointedAutomation.Objects.API](https://www.nuget.org/packages/AnointedAutomation.Objects.API): ASP.NET Core helpers such as the shared JSON casing convention.

## License

MIT. See [LICENSE](https://github.com/AnointedAutomation/AnointedAutomation/blob/master/LICENSE).

## Support This Project

This library is free and open source. The best way to support the work is to shop with us:

- **Christian items:** [https://store.anointed.company](https://store.anointed.company)
- **Everything else:** [https://www.mart.club](https://www.mart.club)

Found a bug or have a request? Open an issue at [https://github.com/AnointedAutomation/AnointedAutomation/issues](https://github.com/AnointedAutomation/AnointedAutomation/issues).
