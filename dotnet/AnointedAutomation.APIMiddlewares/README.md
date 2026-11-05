# AnointedAutomation.APIMiddlewares

Jesus is King ✝️

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
| `ApiKeyAuthMiddleware` | Constructor `(RequestDelegate next, string headerName, string expectedKey, string protectedPathPrefix)`. Requests whose path starts with the prefix must send the header with the exact key or get `401`. Uses `CryptographicOperations.FixedTimeEquals`. Also exposes `static bool IsAuthorized(string presentedKey, byte[] expectedKey)`. The prefix must start with `/`. Since 1.2.0 a null or empty `expectedKey` throws `ArgumentException` when the pipeline is built, so a missing key crashes at startup instead of answering `401` to every request. |
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

## Honeypot and bot defense

Reusable, opt in pieces. Nothing is active until you turn it on, and every block is logged at Warning with the IP, the rule and the path (no silent drops).

### Middleware: honeypot routes, User-Agent rules, rate limits

```json
{
  "BotDefense": {
    "Enabled": true,
    "ExemptPaths": [ "/health" ],
    "HoneypotPaths": { "Enabled": true, "BanDuration": "1.00:00:00" },
    "UserAgent": { "Enabled": true, "BlockEmpty": false },
    "RateLimit": {
      "Enabled": true,
      "PerIpLimit": 300,
      "PerIpWindow": "00:01:00",
      "Routes": [ { "PathPrefix": "/api/auth", "Limit": 10, "Window": "00:05:00" } ]
    },
    "ClientIp": { "TrustedProxyProviders": [ "Cloudflare" ], "HonorCfConnectingIp": true }
  }
}
```

```csharp
using AnointedAutomation.APIMiddleware.BotDefense;

builder.Services.AddBotDefense(builder.Configuration.GetSection("BotDefense"));
app.UseBotDefense();
```

- Order: exempt paths, verified crawlers, ban check, honeypot routes, User-Agent rules, rate limits.
- Lists you configure (`HoneypotPaths.Paths`, `HoneypotPaths.FileNames`, `UserAgent.GoodCrawlers`) REPLACE the built-in defaults; leave one empty to use the defaults.
- Honeypot routes: a hit on `/wp-admin`, `/wp-login.php`, `/xmlrpc.php`, `/phpmyadmin`, `/.git`, `/cgi-bin` (set `Paths`) or on a file named `.env`, `.git` or `.aws` at any depth (set `FileNames`) bans that IP for `BanDuration` and answers `ResponseStatus` (404). Paths match by segment, so `/wp-administrator` is not a hit. Only enable this on routes your own site never uses.
- Zip bomb: set `HoneypotPaths.ServeEnvZipBomb` to answer a honeypot hit on a `.env*` file with a generated zip (one `.env` entry of `ZipBombExpandedBytes` zeros, default 512 MiB, about 0.5 MB on the wire) instead of the empty response. It is built in code on first use and cached; no binary is stored in the repo.
- User-Agent rules: blocks a built-in list of scanner tools (sqlmap, nikto, nmap, masscan, nuclei and similar; generic clients such as curl and python-requests are deliberately not listed), your own `BlockedFragments`, and optionally an empty User-Agent. `GoodCrawlers` (Googlebot, Bingbot, Applebot, YandexBot, Baiduspider by default) pass every rule, honeypots and bans included, only after reverse DNS confirms them (the PTR name must end in the crawler's documented domain and resolve forward to the same IP; results are cached for 12 hours, at most 10,000 entries). A request that claims to be one but fails the check is blocked as spoofed. A DNS failure is logged at Warning and counts as unverified.
- Rate limits: fixed window per IP (`PerIpLimit`) and per route prefix (longest matching prefix wins). Over the limit returns `429` with `Retry-After`. The table is capped at 50,000 keys; when it is full of live windows, expired ones are swept (at most every 30 seconds), and new keys are admitted uncounted with a logged Warning rather than growing memory.
- Ban store: `IBanStore` (`GetBanAsync`, `BanAsync`, `UnbanAsync`). The default `InMemoryBanStore` is per process and capped at 100,000 bans. For bans that survive restarts or are shared between instances, implement `IBanStore` over Mongo, Redis or any store and register it before `AddBotDefense`. If the store throws, the failure is logged at Error and `BlockWhenBanStoreFails` decides (false lets the request through, true answers `503`).
- Client IP and exempt paths follow the same trusted proxy rules as the IP allowlist.

### Form honeypot field and timing check

```csharp
builder.Services.AddFormBotGuard(options =>
{
    options.Enabled = true;
    options.HoneypotFieldName = "website";
    options.MinSubmitSeconds = 3;
    options.SigningKey = Environment.GetEnvironmentVariable("FORM_GUARD_KEY"); // at least 16 characters, from your secret store
});

// Render: put guard.CreateTimingToken() in a hidden "t" field, and a "website" input hidden with CSS.
// Submit:
FormBotCheckResult result = guard.Check(HttpContext, form["website"], form["t"]);
if (result != FormBotCheckResult.Ok)
{
    return Ok(new { success = true }); // fake success: the bot learns nothing. The attempt is already logged.
}
```

A filled honeypot field is a bot, a form submitted faster than `MinSubmitSeconds` is a bot, and a missing, forged (HMAC signed) or expired timing token counts as a bot when the timing check is on.

### Captcha verification

```csharp
builder.Services.AddCaptchaVerifier(options =>
{
    options.Provider = CaptchaProvider.ReCaptchaV3; // or Turnstile, HCaptcha
    options.SecretKey = Environment.GetEnvironmentVariable("CAPTCHA_SECRET");
    options.MinScore = 0.5;
});

CaptchaResult verdict = await captcha.VerifyAsync(token, remoteIp, cancellationToken);
if (!verdict.Success) { /* reject, verdict.Reason says why */ }
```

reCAPTCHA v3, Cloudflare Turnstile and hCaptcha share the same siteverify protocol, so `ICaptchaVerifier` covers all three. When the provider returns a score (reCAPTCHA v3 always does) it must be at or above `MinScore`. A provider outage, HTTP error or unreadable answer logs at Error and fails closed.

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

## IP allowlist (`IpAllowlistMiddleware`)

Blocks every request from an IP that is not on the allowlist (`403`, logged at Warning with the IP, method, path and reason). Off by default. Entries are single IPs or CIDR ranges, IPv4 and IPv6 (IPv4 must be a full dotted quad: short forms such as `10.1` are rejected at startup), and you can add provider presets for CDNs.

```json
{
  "IpAllowlist": {
    "Enabled": true,
    "Allowlist": [ "203.0.113.0/24", "2001:db8::/32" ],
    "Providers": [ "Cloudflare" ],
    "ExemptPaths": [ "/health", "/api/webhooks" ],
    "ClientIp": { "TrustedProxyProviders": [ "Cloudflare" ], "HonorCfConnectingIp": true },
    "RefreshEnabled": true,
    "RefreshInterval": "1.00:00:00"
  }
}
```

```csharp
using AnointedAutomation.APIMiddleware.Security;

builder.Services.AddIpAllowlist(builder.Configuration.GetSection("IpAllowlist"));
// or: builder.Services.AddIpAllowlist(options => { options.Enabled = true; options.Allowlist.Add("203.0.113.7"); });

app.UseIpAllowlist(); // first in the pipeline
```

- Enabled with an empty `Allowlist` and no `Providers`, an invalid entry or an unknown provider throws when `UseIpAllowlist()` runs, so a bad configuration crashes at startup instead of failing open or blocking everyone silently.
- `Allowlist` entries are matched against the resolved client IP. `Providers` presets are matched against the DIRECT peer: "accept only traffic that arrives through this CDN". With `Providers: ["Cloudflare"]` every request that comes through Cloudflare is allowed, whoever the visitor is, and a request that does not come from a Cloudflare address is blocked even if it forges a Cloudflare address in a header. Do not use `Providers` when you want to restrict which visitors get in; use `Allowlist` for that (with the CDN as a trusted proxy so the visitor's address is resolved).
- Provider presets (shipped ranges, with the source URL and fetch date in `Security/IpProviderPresets.Data.cs`):

| Preset | Official source |
|---|---|
| `Cloudflare` | https://www.cloudflare.com/ips-v4 and https://www.cloudflare.com/ips-v6 |
| `Fastly` | https://api.fastly.com/public-ip-list |
| `CloudFront` | https://ip-ranges.amazonaws.com/ip-ranges.json (service `CLOUDFRONT`) |

  Evaluated and not shipped: Google Cloud (`cloud.json` lists every Google Cloud customer address, not just the CDN, so it is far too broad for an allowlist), Akamai, Vercel and Railway (no official public range list found). Add those through `Allowlist` yourself.
- Scheduled refresh (`RefreshEnabled`, `RefreshInterval`, `RefreshTimeout`): the shipped list is the starting set; each interval the provider's published list is downloaded and swapped in only if the whole download parses. Any failure (network, HTTP status, malformed or empty list) is logged at Error with the exception and the last good list stays in use. It never empties the list and never allows everyone. The refresh covers both `Providers` and `ClientIp.TrustedProxyProviders`, so trusted proxy ranges do not go stale.
- Client IP: `X-Forwarded-For` and `CF-Connecting-IP` are honored only when the direct peer is in `ClientIp.TrustedProxies` or one of the `ClientIp.TrustedProxyProviders` ranges. `X-Forwarded-For` is read from the right, skipping trusted hops, so a client cannot forge an earlier entry. With no trusted proxies configured the headers are never used. A request with no resolvable client address is blocked. `CF-Connecting-IP` (with `HonorCfConnectingIp`) is believed only when the direct peer is a Cloudflare edge address (the Cloudflare ranges, refreshed when the `Cloudflare` preset is in use), never from another trusted proxy, which would pass a client's forged header straight through.
- Shared CDN ranges are shared: trusting a CDN preset such as `Fastly` or `CloudFront` as a proxy (or allowing it in `Providers`) means any customer of that CDN can put a service in front of your origin and supply whatever `X-Forwarded-For` it likes. Only trust a shared CDN's ranges when the origin also verifies the CDN is yours (an origin secret header, mutual TLS, or a private origin).
- `ExemptPaths` are path prefixes matched by segment, case-insensitively (`/health` matches `/health/live`, not `/healthz`).

## Notes

- The client IP comes from `X-Forwarded-For` when present. Only trust it behind a reverse proxy that sets it.
- All state (blacklist, attempt counts, logs) is static and per process.

## Country blocking (`CountryBlockMiddleware`)

Blocks (`403`) requests by country of origin. Off by default. `Mode` and `UnknownCountry` have no default: when enabled you must choose them, so the behavior for unknown countries is always an explicit decision. Every block is logged at Warning with the IP, country and path.

```json
{
  "CountryBlock": {
    "Enabled": true,
    "Mode": "Block",
    "Countries": [ "RU", "KP" ],
    "UnknownCountry": "Allow",
    "ExemptPaths": [ "/health" ],
    "UseCfIpCountryHeader": true,
    "ClientIp": { "TrustedProxyProviders": [ "Cloudflare" ], "HonorCfConnectingIp": true }
  }
}
```

```csharp
using AnointedAutomation.APIMiddleware.Security;

builder.Services.AddCountryBlock(builder.Configuration.GetSection("CountryBlock"));
app.UseCountryBlock();
```

- `Mode`: `Block` blocks the listed countries; `AllowOnly` allows only the listed ones. `Countries` are ISO 3166-1 alpha-2 codes (case-insensitive). `UnknownCountry`: `Block` or `Allow`, for requests whose country cannot be determined (including a request with no client address and, until the first data load, every request).
- Data source: the public "delegated extended" statistics files that the five Regional Internet Registries publish (ARIN, RIPE NCC, APNIC, LACNIC, AFRINIC). No key, no account, and nothing is committed to this repository: the package downloads them at startup and then every `RefreshInterval` (7 days by default; `RetryInterval` 15 minutes while nothing has loaded). Set `DataUrls` to use mirrors: whatever you list there replaces the five defaults (leave it out to use the defaults). The lookup is built in memory (about 240k IPv4 and 55k IPv6 ranges, a couple of seconds to build). The country is where the registry allocated the block, which is coarser than commercial geolocation: it is good for blocking whole countries, not for finding a city, and a block can be used abroad.
- Failure handling: if any file fails to download or has no ranges, the whole refresh is discarded, logged at Error with the exception, and the last good data keeps serving. Until a first successful load the `UnknownCountry` setting decides.
- Other data sources considered: MaxMind GeoLite2 (free, but needs an account and license key to download, and its EULA restricts redistribution), DB-IP Lite (CC BY 4.0, needs attribution), IP2Location LITE (account and attribution). To use one, implement `ICountryLookup` (one method, `string Lookup(IPAddress)`), register it before `AddCountryBlock`, and set `DownloadRirData` to false. Keep any database file out of source control and point your implementation at its path from your own configuration.
- `UseCfIpCountryHeader`: behind Cloudflare, uses the `CF-IPCountry` header, but only when the direct peer is a trusted proxy (`ClientIp.TrustedProxies` or `TrustedProxyProviders`) AND a Cloudflare edge address (the refreshed Cloudflare list when `AddIpAllowlist` is registered, otherwise the ranges shipped with the package); from any other peer, including a trusted non-Cloudflare load balancer, the header is ignored. `XX` and `T1` mean unknown and fall back to the lookup.
- Client IP resolution, `ExemptPaths` and trusted proxy rules are the same as the IP allowlist (`ClientIp`, `ExemptPaths`). Loopback and private-network clients have no country and skip the check unless you set `AllowPrivateAddresses` to false.

## Related packages

- [AnointedAutomation.Logging](https://www.nuget.org/packages/AnointedAutomation.Logging): the `LogMessage` type used for the middleware log buffer.
- [AnointedAutomation.Memory](https://www.nuget.org/packages/AnointedAutomation.Memory): the garbage collection helper used by `EndpointAccessMiddleware`.
- [AnointedAutomation.Objects.API](https://www.nuget.org/packages/AnointedAutomation.Objects.API): ASP.NET Core helpers such as the shared JSON casing convention.

## License

MIT. See [LICENSE](https://github.com/AnointedAutomation/AnointedAutomation/blob/master/LICENSE).

## Support This Project

This library is free and open source. The best way to support the work is to shop with us:

- **Christian items:** [https://store.anointed.company](https://store.anointed.company)
- **Pets:** [https://www.animaldander.com](https://www.animaldander.com)
- **Everything else:** [https://www.mart.club](https://www.mart.club)

Found a bug or have a request? Open an issue at [https://github.com/AnointedAutomation/AnointedAutomation/issues](https://github.com/AnointedAutomation/AnointedAutomation/issues).
