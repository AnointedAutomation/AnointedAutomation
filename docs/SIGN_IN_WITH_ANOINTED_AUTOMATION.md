# Sign in with Anointed Automation

Jesus is King ✝️

A practical guide for clients and partners who want a "Sign in with Anointed Automation" button on their own
site or app. Anointed Automation is a standard OpenID Connect (OIDC) provider, so any OIDC library works; for
Node, Deno and edge runtimes there is a ready-made kit, [`@anointedautomation/sso`](../js/anointed-sso).

## 1. What it is

Your user clicks **Sign in with Anointed Automation** on your site and is sent to Anointed Automation. There
they:

1. Sign in to their Anointed Automation account (or are already signed in).
2. See a consent screen naming your app and what it will receive (for example "your verified email").
3. If your app asks for the `profile` scope and they have no username yet, pick a unique username first.
4. Approve. They land back on your callback URL, and your server receives a verified identity.

Consent is remembered per user and per app, so the next sign-in with the same scopes goes straight through.
Users can disconnect your app at any time (see section 6).

Banned or temporarily locked accounts cannot sign in to your app, and their existing tokens stop working on
the next refresh or userinfo call.

## 2. How to get access

Sign-in is not self-service. It is a capability switched on for one of your Anointed Automation API keys, and
only the Anointed Automation owner can do that.

1. **Ask Anointed Automation.** Email the owner with your app's name, the exact callback URL(s) your server
   will use, the scopes you need (start with `openid email`), and optionally a post-logout redirect URL and a
   back-channel logout URL.
2. **The owner creates or approves your API key** in the admin console (Admin > API Keys). The key's client
   name is your partner slug, and a partner record with that slug must already exist. Partner keys normally
   get the `partner` preset (product feed, user provisioning, store-credit rewards, read of your own
   earnings); sign-in is NOT a scope and is not part of any preset.
3. **The owner switches on sign-in for that key** (Admin > API Keys > your key > Sign-in) and enters your
   exact callback URLs and allowed scopes.

Be clear about what you can and cannot do yourself:

- Keys and permissions are **never granted automatically**. Every key, every scope and the sign-in
  capability are set by the owner, by hand.
- You **cannot** add scopes, add callback URLs, or turn sign-in on. Ask the owner for any change.
- The **partner portal** only shows your keys (prefix, scopes, state, and your sign-in client_id and
  registered URLs) and lets you **rotate** a key or the sign-in client secret. A rotation keeps exactly the
  same permissions, callback URLs and client_id; it only replaces the secret. Rotation is confirmed with a
  code emailed to your account address, and the owner is notified.
- If the owner disables or deletes your key, or turns sign-in off, your app stops working at once: every user
  grant is revoked and your back-channel logout URL (if any) is notified.

## 3. What you receive

| Item | Value |
|---|---|
| `client_id` | `aa_<your key id>`. It never changes, not even when your key is rotated |
| `client_secret` | Shown **once**, when sign-in is first enabled or the secret is rotated. Anointed Automation stores only a hash and can never show it again |
| Issuer | `https://api.anointedautomation.net/` |
| Discovery | `https://api.anointedautomation.net/.well-known/openid-configuration` |

Store the client secret only in your server's secret store or environment (for example
`ANOINTED_OAUTH_CLIENT_SECRET`). Never put it in source control, an `.env.example`, logs, a browser bundle or
a mobile app. Your app is a confidential client: the token exchange always happens on your server.

If you lose the secret, rotate it (section 6). Nobody can read the old one back.

## 4. Integration

### Rules the provider enforces

- **Flow:** Authorization Code with PKCE only. `code_challenge_method=S256` is required; `plain` is rejected.
  No implicit flow, no password grant, no client credentials.
- **`state` is required** on every authorize request.
- **Redirect URIs** are exact string matches against what the owner registered: absolute `https` URLs, no
  fragment, no wildcards. A trailing slash or a different port is a different URI. Plain `http`, including
  `localhost`, cannot be registered; test against an https tunnel or staging host.
- **Client authentication** at the token endpoint uses your `client_id` and `client_secret`.
- **Scopes:** `openid` (always), `email`, `profile`, `offline_access`. Your client can use `openid`,
  `offline_access`, and whichever of `email` and `profile` the owner enabled for it.

### Node, Deno and edge: `@anointedautomation/sso`

```bash
npm install @anointedautomation/sso
```

```
ANOINTED_OAUTH_CLIENT_ID=aa_<your key id>
ANOINTED_OAUTH_CLIENT_SECRET=<server only>
ANOINTED_OAUTH_REDIRECT_URI=https://yourapp.example/auth/anointed/callback
```

**Express** (from [examples/express](../js/anointed-sso/examples/express)):

```js
import { ANOINTED_ISSUER, createAnointedClient, createExpressHandlers } from "@anointedautomation/sso";

function requireEnv(name) {
  const value = process.env[name];
  if (value === undefined || value.trim() === "") throw new Error(`Missing required environment variable ${name}`);
  return value;
}

const client = createAnointedClient({
  issuer: ANOINTED_ISSUER,
  clientId: requireEnv("ANOINTED_OAUTH_CLIENT_ID"),
  clientSecret: requireEnv("ANOINTED_OAUTH_CLIENT_SECRET"),
  redirectUri: requireEnv("ANOINTED_OAUTH_REDIRECT_URI"),
  // scope: "openid email",  // the default; add "profile" or "offline_access" only if you need them
});

const anointed = createExpressHandlers(client, {
  onSignedIn(result, req, res) {
    // Find or create YOUR user by result.user.sub, start YOUR session, then:
    res.redirect(result.returnTo === null ? "/" : result.returnTo);
  },
  onLogout(logout) {
    // Back-channel logout: end every session for logout.sub (or logout.sid). Ignore a repeated logout.jti.
  },
});

app.get("/auth/anointed/start", anointed.start);
app.get("/auth/anointed/callback", anointed.callback);
app.post("/auth/anointed/backchannel-logout", anointed.backchannelLogout);
```

**Next.js App Router** (from [examples/nextjs](../js/anointed-sso/examples/nextjs)):

```js
// lib/anointed.js
import { ANOINTED_ISSUER, createAnointedClient, createWebHandlers } from "@anointedautomation/sso";

export const handlers = createWebHandlers(
  createAnointedClient({
    issuer: ANOINTED_ISSUER,
    clientId: requireEnv("ANOINTED_OAUTH_CLIENT_ID"),
    clientSecret: requireEnv("ANOINTED_OAUTH_CLIENT_SECRET"),
    redirectUri: requireEnv("ANOINTED_OAUTH_REDIRECT_URI"),
  }),
  {
    async onSignedIn(result, request) {
      // Find or create YOUR user by result.user.sub, then redirect with YOUR session cookie.
      return new Response(null, { status: 302, headers: { location: "/", "set-cookie": await yourSessionCookie(result.user) } });
    },
  },
);

// app/api/auth/anointed/start/route.js
export const dynamic = "force-dynamic";
export function GET(request) { return handlers.start(request); }

// app/api/auth/anointed/callback/route.js
export const dynamic = "force-dynamic";
export function GET(request) { return handlers.callback(request); }
```

The same `createWebHandlers` works on Base44 (Deno), Cloudflare Workers, Bun and Hono; see
[examples/base44](../js/anointed-sso/examples/base44). The kit handles PKCE, `state`, `nonce`, the flow cookie,
ID token validation and the RFC 9207 `iss` check for you. Full API:
[js/anointed-sso/README.md](../js/anointed-sso/README.md).

### .NET (ASP.NET Core)

Use the [AnointedAutomation.SSO](https://www.nuget.org/packages/AnointedAutomation.SSO) NuGet package
(`dotnet add package AnointedAutomation.SSO`). It configures the standard
`Microsoft.AspNetCore.Authentication.OpenIdConnect` handler for you (authorization code + PKCE S256, `state`,
`nonce`, RS256-only ID token validation, `openid email` by default, raw claim names, verified email required)
and adds a back-channel logout endpoint and a fail-open session status client:

```csharp
using AnointedAutomation.SSO;

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = AnointedAutomationDefaults.AuthenticationScheme; // "Anointed"
    })
    .AddCookie()
    .AddAnointedAutomation(options =>
    {
        options.ClientId = RequiredEnv("ANOINTED_OAUTH_CLIENT_ID");
        options.ClientSecret = RequiredEnv("ANOINTED_OAUTH_CLIENT_SECRET");
        options.CallbackPath = "/auth/anointed/callback"; // must match the registered URL exactly
        // options.AdditionalScopes.Add("profile");       // only if you need the name or username
    });

// after builder.Build():
app.MapAnointedBackchannelLogout("/auth/anointed/backchannel-logout", async (logout, httpContext) =>
{
    // End every session for logout.Sub (or logout.Sid). Retries with the same jti are answered for you.
});
```

Key your users on `User.GetAnointedSubject()` (`sub`). `ClientId`, `ClientSecret` and `CallbackPath` are
required and the app fails at startup without them. `RequiredEnv` is your own helper that throws when the
variable is missing. Behind a reverse proxy, enable forwarded headers so the handler builds an `https`
redirect URI. To end a local session when the user's Anointed Automation account or grant ends, add
`builder.Services.AddAnointedSessionStatus(...)` (your partner API key needs the `sessions:status` scope); it
fails open, so an outage never signs your users out. Full API and security checklist:
[dotnet/AnointedAutomation.SSO/README.md](../dotnet/AnointedAutomation.SSO/README.md).

### Any other stack (plain OIDC)

Read the endpoints from discovery rather than hard-coding them. Today they are:

| Endpoint | URL |
|---|---|
| Discovery | `https://api.anointedautomation.net/.well-known/openid-configuration` |
| Authorize | `https://api.anointedautomation.net/connect/authorize` |
| Token | `https://api.anointedautomation.net/connect/token` |
| UserInfo | `https://api.anointedautomation.net/connect/userinfo` |
| Revocation | `https://api.anointedautomation.net/connect/revoke` |
| End session | `https://api.anointedautomation.net/connect/logout` |
| JWKS | the `jwks_uri` in discovery |

Steps:

1. Generate a random `state`, a random `nonce`, and a PKCE `code_verifier` (43 to 128 characters). Keep all
   three server side, bound to the browser (for example in a short-lived `HttpOnly` cookie).
2. Redirect to the authorize endpoint with `response_type=code`, `client_id`, `redirect_uri`,
   `scope=openid email`, `state`, `nonce`, `code_challenge=BASE64URL(SHA256(code_verifier))` and
   `code_challenge_method=S256`.
3. On the callback, check `state` first (constant-time compare). If `error` is present, stop. Check that `iss`
   equals the issuer.
4. POST to the token endpoint (`application/x-www-form-urlencoded`): `grant_type=authorization_code`, `code`,
   `redirect_uri`, `code_verifier`, plus client authentication (`client_id` and `client_secret`). The code is
   valid for 2 minutes and works once.
5. Validate the `id_token`: RS256 signature against the JWKS, `iss` exactly `https://api.anointedautomation.net/`,
   `aud` contains your `client_id`, `exp` and `iat` within a small clock skew, and `nonce` equals the one you
   stored. Only then trust `sub`.

Access tokens are encrypted and opaque to you; use them only as a Bearer token on the userinfo endpoint.

### Token lifetimes

| Token | Lifetime |
|---|---|
| Authorization code | 2 minutes, single use |
| Access token | 15 minutes |
| ID token | 15 minutes |
| Refresh token (`offline_access` only) | 30 days. Store the new refresh token returned by every refresh |

The token endpoint is rate limited to 30 requests per minute per IP address.

## 5. Claims and linking users

| Claim | Scope | Notes |
|---|---|---|
| `sub` | `openid` | Stable Anointed Automation user id. **Key your users on this** |
| `sid` | `openid` | The grant id; back-channel logout tokens carry the same value |
| `email` | `email` | Lowercased, and only sent when the address is verified |
| `email_verified` | `email` | Always `true` when `email` is present |
| `name` | `profile` | Display name, when set |
| `preferred_username` | `profile` | A unique handle the user chose. Asking for `profile` makes users without one pick one before consenting |
| `given_name`, `family_name` | `profile` | When set |
| `picture` | `profile` | Avatar URL served by Anointed Automation |

Linking to your own users:

- Store `sub` on your user record and look users up by it. Email and username can change; `sub` never does.
- **Never link or merge accounts on an email unless `email_verified` is `true`.** Anointed Automation only
  releases verified email, but check it anyway so a future change or a different provider cannot open a hole.
  The Node kit (`requireVerifiedEmail`) and the .NET package (`RequireVerifiedEmail`) refuse a sign-in without a
  verified email when you request `email`.
- Request the smallest scope that works. `openid email` is the default; add `profile` only when you need the
  name or username, and `offline_access` only when you call the API on the user's behalf later.

## 6. Security, rotation and revocation

Checklist:

- [ ] Client secret only in your server's secret store; never in code, git, logs or client bundles.
- [ ] Redirect URIs are https and registered exactly.
- [ ] PKCE S256, `state` and `nonce` on every sign-in (the kit and the .NET handler do this).
- [ ] ID token fully validated (signature, `iss`, `aud`, `exp`, `nonce`) before trusting `sub`.
- [ ] Users keyed on `sub`; no account linking on an unverified email.
- [ ] Your session cookie is `HttpOnly`, `Secure`, `SameSite=Lax` or stricter, and the session id rotates at
      sign-in.
- [ ] Error pages show a generic message, never the raw provider error.
- [ ] A refresh that returns `invalid_grant`, or a back-channel logout, ends that user's session.
- [ ] Users who signed up only through Anointed Automation have another way back in (email reset or a local
      password).

**Rotating the client secret.** In the partner portal, request a rotation of the sign-in client secret (or of
the whole API key, optionally with a new sign-in secret). A code is emailed to your account address; confirm
it and the new secret is shown once. The `client_id`, callback URLs, scopes and every user's grant stay the
same. Deploy the new secret promptly: the old one stops working at once. You can also ask the owner to rotate
it for you. A full key rotation can keep the old API key working for 24 hours; the sign-in secret has no
overlap.

**How users revoke your app.** A signed-in user opens Settings > **Connected apps** on anointedautomation.net
and disconnects your app, or signs out of every connected app at once. The owner can also revoke a user's
grants from the admin console. Revocation kills that user's refresh and access tokens for your app.

**Back-channel logout.** If you gave the owner a back-channel logout URL (https, exact, no wildcards), we POST
a signed Logout Token (`logout_token` form field) to it whenever a user's grant ends: the user disconnects your
app or signs out of all apps, an admin revokes it, the account is deleted or merged, or your app is disabled.
The token carries `sub`, `sid` and the back-channel logout event. End every session for that `sub` or `sid`,
drop any refresh token you hold, and ignore a `jti` you already processed. Failed deliveries are retried with
backoff (30 seconds doubling up to 6 hours, 12 attempts). The kit's `backchannelLogout` handler and
`verifyLogoutToken`, and the .NET package's `MapAnointedBackchannelLogout` and `AnointedLogoutTokenValidator`,
do the validation for you.

**Sign-out.** To end the user's session at Anointed Automation too, redirect to the end session endpoint
(`client.buildLogoutUrl()` in the kit). A `post_logout_redirect_uri` must be one the owner registered.

## 7. Troubleshooting

| Symptom | Cause and fix |
|---|---|
| `redirect_uri` mismatch (`invalid_request` at authorize) | The URL you send is not an exact match for a registered one. Compare scheme, host, port, path and trailing slash. Only the owner can add a URL, and it must be https |
| `invalid_client` | Wrong or rotated `client_secret`, wrong `client_id`, or sign-in was turned off / the key was disabled or deleted. Check the portal's integration view for your client_id; rotate the secret if unsure |
| `unauthorized_client` at authorize | Sign-in is turned off for this app. Ask the owner |
| Scope rejected at authorize | You requested a scope the owner did not enable for your client. Request less, or ask for it |
| `invalid_grant` on the code exchange | The code is older than 2 minutes, was already used, or `redirect_uri` / `code_verifier` does not match the authorize request |
| `invalid_grant` on refresh | The user disconnected your app or was banned, or the refresh token expired or was already used. Sign the user in again |
| `state` mismatch (`state_mismatch`, `flow_missing`, `flow_expired` in the kit) | Stale tab, blocked or cross-site cookies, or the flow cookie expired (10 minutes in the kit). Restart sign-in; make sure the callback is on the same site that set the cookie |
| `nonce` mismatch | The ID token belongs to a different sign-in attempt (two tabs, or a replayed callback). Restart sign-in; store one nonce per attempt |
| `exp` / `iat` rejected | Your server clock is off. Sync it with NTP; allow about 60 seconds of skew, not more |
| `email_not_verified` (kit) | The user has no verified email. Ask them to verify it at Anointed Automation, or handle the no-email case yourself |
| HTTP 429 from the token endpoint | More than 30 token requests per minute from one IP. Back off and retry |

Still stuck? Email the owner with your `client_id`, the time of the failure (UTC) and the exact error.
