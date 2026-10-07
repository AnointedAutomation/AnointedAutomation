# AnointedAutomation.SSO

Jesus is King ✝️

"Sign in with Anointed Automation" for **ASP.NET Core** in about five minutes. This package is a thin, opinionated
layer over the standard `Microsoft.AspNetCore.Authentication.OpenIdConnect` handler (it does not hand-roll the
protocol), plus the two things that handler does not give you: a **back-channel logout** endpoint and a
**session status** client.

Anointed Automation is a standard OpenID Connect provider:

| | |
|---|---|
| Issuer | `https://api.anointedautomation.net/` |
| Discovery | `https://api.anointedautomation.net/.well-known/openid-configuration` |
| ID tokens | RS256, verified against the published JWKS |
| Flow | Authorization code + PKCE (`S256`) only; `state` required; `nonce` used |
| Full guide | [Sign in with Anointed Automation](https://github.com/AnointedAutomation/AnointedAutomation/blob/master/docs/SIGN_IN_WITH_ANOINTED_AUTOMATION.md): access, credentials, claims, rotation, troubleshooting |
| Node / Deno / edge | [@anointedautomation/sso](https://github.com/AnointedAutomation/AnointedAutomation/tree/master/js/anointed-sso) |

**Server only.** This package holds your client secret and your partner API key. Never ship either to a
browser, a Blazor WebAssembly app, a MAUI or other mobile/desktop app, or anything else that runs on a user's
device. Use it in your ASP.NET Core server.

## 5-minute quick start

### 1. Get your credentials

Sign-in is a capability on your existing Anointed Automation partner API key. We enable it for you and send you:

- `client_id`: `aa_<your key id>`
- `client_secret`: shown exactly once, sent over a private channel. Store it only in your server's secret store.

Tell us your exact callback URL(s) (absolute `https`, exact match, no wildcards) and, if you want back-channel
logout, your back-channel logout URL.

### 2. Install

```bash
dotnet add package AnointedAutomation.SSO
```

### 3. Wire it up (`Program.cs`)

```csharp
using AnointedAutomation.SSO;
using Microsoft.AspNetCore.Authentication.Cookies;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = AnointedAutomationDefaults.AuthenticationScheme; // "Anointed"
    })
    .AddCookie(options =>
    {
        // Optional: end the local session as soon as Anointed Automation says it ended (fail open).
        options.Events.OnValidatePrincipal = context =>
            AnointedSessionStatusCookieEvents.ValidatePrincipalAsync(context, grant: true);
    })
    .AddAnointedAutomation(options =>
    {
        options.ClientId = RequiredEnv("ANOINTED_OAUTH_CLIENT_ID");         // aa_<your key id>
        options.ClientSecret = RequiredEnv("ANOINTED_OAUTH_CLIENT_SECRET"); // server only
        options.CallbackPath = "/auth/anointed/callback";                   // must match the registered URL
        // options.AdditionalScopes.Add(AnointedAutomationDefaults.ProfileScope); // only if you need it
    });

// Optional: lets the cookie hook above (or your own code) ask whether a session is still alive.
builder.Services.AddAnointedSessionStatus(options =>
{
    options.ApiKey = RequiredEnv("ANOINTED_API_KEY"); // needs the sessions:status scope; server only
    options.AppName = "Your App";
});

WebApplication app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/auth/anointed/start", () => Results.Challenge(
    new Microsoft.AspNetCore.Authentication.AuthenticationProperties { RedirectUri = "/" },
    new[] { AnointedAutomationDefaults.AuthenticationScheme }));

app.MapAnointedBackchannelLogout("/auth/anointed/backchannel-logout", async (logout, httpContext) =>
{
    // End every session for logout.Sub (or logout.Sid) and drop any refresh token you hold for that user.
    await httpContext.RequestServices.GetRequiredService<IYourSessionStore>().EndAllAsync(logout.Sub, logout.Sid);
});

app.Run();

static string RequiredEnv(string name)
{
    string value = Environment.GetEnvironmentVariable(name);
    if (string.IsNullOrWhiteSpace(value))
    {
        throw new InvalidOperationException("Missing required environment variable " + name);
    }
    return value;
}
```

Environment variables used above:

```
ANOINTED_OAUTH_CLIENT_ID=aa_<your key id>
ANOINTED_OAUTH_CLIENT_SECRET=<server only>
ANOINTED_API_KEY=<server only, only for session status>
```

Behind a reverse proxy, enable forwarded headers (`app.UseForwardedHeaders()`) so the handler builds an
`https` redirect URI.

### 4. Read the user

```csharp
string sub = User.GetAnointedSubject();   // stable user id: key your users on this
string sid = User.GetAnointedSessionId(); // grant id, matches back-channel logout's Sid (or null)
string email = User.GetAnointedEmail();   // verified email (null without the email scope)
```

The handler keeps the raw OIDC claim names (`sub`, `email`, `email_verified`, `name`, `preferred_username`,
`given_name`, `family_name`, `picture`, `sid`); `User.Identity.Name` is the `name` claim.

## What `AddAnointedAutomation` sets

| Setting | Value |
|---|---|
| Authority | `https://api.anointedautomation.net/` (https metadata required) |
| ResponseType / ResponseMode | `code` / `query` |
| PKCE | on (`S256`) |
| Scopes | cleared, then `openid email` plus `AdditionalScopes` |
| MapInboundClaims | false (raw claim names) |
| Name claim | `name` |
| ID token algorithms | RS256 only |
| SaveTokens | false unless you set `SaveTokens = true` |
| GetClaimsFromUserInfoEndpoint | false |
| Verified email | with the email scope, a sign-in whose `email_verified` is not true is failed (`RequireVerifiedEmail`, default true) |

`ClientId` (must start with `aa_`), `ClientSecret` and `CallbackPath` are required and have no defaults: a
missing or invalid value throws `InvalidOperationException` while the app starts. Your own
`options.Events` are kept: the verified-email check runs first, then your `OnTokenValidated`. For anything
else, `options.ConfigureOpenIdConnect = oidc => { ... }` gets the last word on the `OpenIdConnectOptions`. A
second overload takes your own scheme name: `AddAnointedAutomation("MyScheme", options => ...)`.

A failed sign-in (user declined, unverified email) surfaces through the handler's `OnRemoteFailure` event;
handle it there and show a generic message.

## Scope advice (read this)

- **The default scope is `openid email`. Keep it unless you truly need more.**
- **Requesting `profile` makes our consent screen stop any user who has no unique handle yet and ask them to
  pick a username first.** That is an extra step in your sign-in, so only add `profile` when your app really
  needs the user's name or username.
- `offline_access` adds a refresh token. Only ask for it if you call our API on the user's behalf later (and
  then set `SaveTokens = true`).
- **Key your users on `sub`, never on email.** Email and username can change; `sub` never does.
- `email` is only ever sent when it is verified, and the package refuses a sign-in without a verified email
  when you request `email`, so you never link accounts on an unverified address.

## Back-channel logout

`MapAnointedBackchannelLogout(pattern, onLogout)` maps an anonymous POST endpoint. Register its full https URL
with us. We call it when a user disconnects your app, signs out of all apps, their account is deleted or
merged, an admin revokes their grant, or your app is disabled.

The endpoint validates the Logout Token exactly as OpenID Connect Back-Channel Logout 1.0 asks: RS256 only
(no `none`, no HMAC), signature against our JWKS (refetched on an unknown `kid`), `typ` `logout+jwt`, exact
`iss`, `aud` containing your client_id, `iat` no older than 5 minutes (`BackchannelLogoutMaxAge`), `exp` not
passed, the back-channel event in `events`, a `jti`, `sub` and/or `sid`, and no `nonce`, with 60 seconds of
clock skew (`ClockSkew`).

| Situation | Answer | `onLogout` called |
|---|---|---|
| Valid token | 200 | yes, once |
| Same `jti` again (our retry) | 200 | no |
| Invalid token, or no `logout_token` | 400 | no |
| `onLogout` throws, or our keys cannot be read | 500 (we retry with the same `jti`) | the retry runs it again |

Processed `jti` values are remembered in memory until the token expires. If your app runs on more than one
instance, register your own `IAnointedLogoutReplayStore` (for example over Redis) before
`AddAnointedAutomation`. `onLogout` should be idempotent either way. To validate a token yourself, resolve
`AnointedLogoutTokenValidator` and call `ValidateAsync(token, cancellationToken)`.

## Session status (fail open)

`IAnointedSessionStatus.CheckAsync(userId, grant, cancellationToken)` asks
`GET https://api.anointedautomation.net/api/session-status?userId=<sub>&grant=<true|false>` whether a session
may continue. Pass `grant: true` for a session that came from Sign in with Anointed Automation (it also ends
when the user disconnected your app). Your API key needs the `sessions:status` scope.

The client exchanges your API key for a machine token at `POST /api/auth/machine-token`, caches it until shortly
before it expires, re-exchanges it once on a 401, times out each request after 3 seconds, and reuses each answer
for 5 minutes per (`userId`, `grant`).

**It FAILS OPEN.** Only a 200 that says `active: false` is `Outcome = Ended` (`ShouldEndSession == true`, with
`Reason` one of `account_gone`, `account_banned`, `signin_off`, `app_disconnected`). A network error, a timeout,
any other status, or a body it cannot read is `Outcome = Unknown`: it is not cached, it is logged, and you
should **keep** the session. An outage at Anointed Automation must never sign all your users out.

`AnointedSessionStatusCookieEvents.ValidatePrincipalAsync(context, grant)` is a ready `OnValidatePrincipal`
hook: it checks the cookie principal's `sub`, and only on `Ended` rejects the principal and signs the cookie out.
A principal without `sub` (for example a local login) is left alone.

## Security checklist

- [ ] The client secret and API key are only in your server's secret store. Not in code, git, `appsettings.json`
      committed to source control, logs, a Blazor WebAssembly app, a MAUI app, or anything shipped to a device.
- [ ] Your redirect URI is https and registered exactly. Anointed Automation never registers an http callback.
- [ ] Users are keyed on `sub`, never on email or username.
- [ ] You never auto-link accounts on an unverified email (the package enforces this with the `email` scope).
- [ ] You request the smallest scope that works: `openid email` by default.
- [ ] Your session cookie is `HttpOnly`, `Secure`, `SameSite=Lax` (or stricter).
- [ ] `OnRemoteFailure` shows a generic message and never echoes the raw provider error to the user.
- [ ] You honor revocation: a back-channel logout (or an `Ended` session status) ends the user's session.
- [ ] Account recovery: a user who signed up only through Anointed Automation can still get back in.
- [ ] Your server clock is NTP-synced.

## License

[MIT](https://github.com/AnointedAutomation/AnointedAutomation/blob/master/LICENSE). Copyright © Anointed
Automation, LLC. Stewarded by Alexander Fields. Jesus is King ✝️
