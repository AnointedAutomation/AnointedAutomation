# @anointedautomation/sso

Jesus is King ✝️

"Sign in with Anointed Automation" for your app in about five minutes. A dependency-free OpenID Connect
client (authorization code + PKCE S256, `state`, `nonce`) for **Node 18+, Deno (Base44) and edge runtimes**,
with ready-made route handlers for **Express**, **Next.js (App Router)** and any **fetch-style** runtime.

Anointed Automation is a standard OpenID Connect provider:

| | |
|---|---|
| Issuer | `https://api.anointedautomation.net/` |
| Discovery | `https://api.anointedautomation.net/.well-known/openid-configuration` |
| ID tokens | RS256, verified against the published JWKS |
| Flow | Authorization code + PKCE (`S256`) only; `state` required; `nonce` used |
| Full guide | [Sign in with Anointed Automation](../../docs/SIGN_IN_WITH_ANOINTED_AUTOMATION.md): access, credentials, .NET and plain OIDC, claims, rotation, troubleshooting |

The kit is **server only**. It holds your client secret and performs the token exchange. Never import it
from browser or mobile code.

## 5-minute quick start

### 1. Get your credentials

Sign-in is a capability on your existing Anointed Automation partner API key. We enable it for you
(Admin > API Keys > your key > **Sign-in**) and send you:

- `client_id`: `aa_<your key id>`
- `client_secret`: shown exactly once, sent over a private channel. Store it only in your server's
  secret store. If it leaks, ask us to rotate it.

Tell us your exact callback URL(s): absolute `https` only, exact match, no fragment, no wildcards. Plain
`http`, including `localhost`, cannot be registered; for local development use an https tunnel or a staging
host. (The kit itself accepts an `http://localhost` redirect URI only so it can run against a local test
provider; Anointed Automation will refuse it.)

### 2. Install

```bash
npm install @anointedautomation/sso
```

### 3. Set server environment variables

```
ANOINTED_OAUTH_CLIENT_ID=aa_<your key id>
ANOINTED_OAUTH_CLIENT_SECRET=<server only>
ANOINTED_OAUTH_REDIRECT_URI=https://yourapp.example/auth/anointed/callback
```

### 4. Add two routes

**Express**

```js
import { ANOINTED_ISSUER, createAnointedClient, createExpressHandlers } from "@anointedautomation/sso";

const client = createAnointedClient({
  issuer: ANOINTED_ISSUER,
  clientId: requireEnv("ANOINTED_OAUTH_CLIENT_ID"),
  clientSecret: requireEnv("ANOINTED_OAUTH_CLIENT_SECRET"),
  redirectUri: requireEnv("ANOINTED_OAUTH_REDIRECT_URI"),
});

const anointed = createExpressHandlers(client, {
  onSignedIn(result, req, res) {
    // Find or create YOUR user by result.user.sub, start YOUR session, then:
    res.redirect(result.returnTo === null ? "/" : result.returnTo);
  },
});

app.get("/auth/anointed/start", anointed.start);
app.get("/auth/anointed/callback", anointed.callback);
```

**Next.js App Router** (`app/api/auth/anointed/start/route.js` and `.../callback/route.js`)

```js
import { ANOINTED_ISSUER, createAnointedClient, createWebHandlers } from "@anointedautomation/sso";

const handlers = createWebHandlers(createAnointedClient({ issuer: ANOINTED_ISSUER, /* ...env as above */ }), {
  onSignedIn: async (result, request) => {
    // Find or create YOUR user by result.user.sub, then return a redirect that sets YOUR session cookie.
    return new Response(null, { status: 302, headers: { location: "/", "set-cookie": await yourSessionCookie(result.user) } });
  },
});

export const dynamic = "force-dynamic";
export const GET = (request) => handlers.start(request); // and handlers.callback in the callback route
```

**Base44 / Deno / Cloudflare Workers / Bun / Hono**: the same `createWebHandlers`, since they all speak
`Request -> Response`. See [examples/base44](./examples/base44).

### 5. Add the button

```html
<a href="/auth/anointed/start">
  <img src="https://www.anointedautomation.net/favicon.svg" alt="" width="20" height="20">
  <span>Sign in with Anointed Automation</span>
</a>
```

Add `?returnTo=/some/path` to land somewhere specific afterwards (same-site paths only by default; anything
else is dropped). To send users back to other sites you trust, see
[Several hosts and cross-site return](#several-hosts-and-cross-site-return).

That's it. `result.user` is:

```js
{ sub, email, emailVerified, name, preferredUsername, givenName, familyName, picture, sid }
```

`email` is the verified address, trimmed and lowercased; the raw claim is still on `result.claims.email`.

## Scope advice (read this)

- **The default scope is `openid email`. Keep it unless you truly need more.**
- **Requesting `profile` makes our consent screen stop any user who has no unique handle yet and ask them
  to pick a username first.** That is an extra step in your sign-in, so only add `profile` when your app
  really needs the user's name or username: `scope: "openid email profile"`.
- `offline_access` adds a refresh token. Only ask for it if you call our API on the user's behalf later.
- **Key your users on `sub`, never on email.** Email and username can change; `sub` never does.
- `email` is only ever sent when it is verified. With the `email` scope the kit also rejects a sign-in
  without a verified email (`requireVerifiedEmail`, default on), so you never link accounts on an
  unverified address.

## Runnable examples

| Example | Run |
|---|---|
| [Express](./examples/express) | create `.env` (below), then `cd examples/express && npm install && npm start` |
| [Next.js App Router](./examples/nextjs) | create `.env.local` (below), then `cd examples/nextjs && npm install && npm run dev` |
| [Base44 backend functions](./examples/base44) | copy `functions/*` into your Base44 project's `base44/functions/` |

The Express and Next.js examples read these (they crash at startup when one is missing):

```
ANOINTED_OAUTH_CLIENT_ID=aa_<your key id>
ANOINTED_OAUTH_CLIENT_SECRET=<server only>
ANOINTED_OAUTH_REDIRECT_URI=https://<your-tunnel-host>/auth/anointed/callback
SESSION_SECRET=<node -e "console.log(require('crypto').randomBytes(32).toString('base64url'))">
PORT=3000
```

For Next.js the redirect URI is `https://<your-tunnel-host>/api/auth/anointed/callback` and `PORT` is not
used. The redirect URI must be an https URL registered with us, so run the example behind an https tunnel
(for example `cloudflared tunnel --url http://localhost:3000`) and register the tunnel's callback URL.

The Express and Next.js examples also implement back-channel logout and a sign-out route.

### Base44

Base44 backend functions run on Deno. Copy the three folders from
[examples/base44/functions](./examples/base44/functions) into your project's `base44/functions/` folder:

| Function | URL | Purpose |
|---|---|---|
| `anointedStart` | `https://<your-app-domain>/functions/anointedStart` | The button's `href` |
| `anointedCallback` | `https://<your-app-domain>/functions/anointedCallback` | Register this as your redirect URI |
| `anointedMe` | `https://<your-app-domain>/functions/anointedMe` | Your React frontend asks "who is signed in?" |

Set the secrets with `base44 secrets set` (`ANOINTED_OAUTH_CLIENT_ID`, `ANOINTED_OAUTH_CLIENT_SECRET`,
`ANOINTED_OAUTH_REDIRECT_URI`, `SESSION_SECRET`) and create an `AnointedUser` entity with `sub` and `email`
fields. The functions import the kit with `npm:@anointedautomation/sso@0.2.0`. `anointedStart` does not
need `ANOINTED_OAUTH_CLIENT_SECRET`; only `anointedCallback` does.

## API

```js
const client = createAnointedClient({
  issuer, clientId, redirectUri,   // required
  clientSecret,          // required by the code exchange, refresh and revoke; not needed to start
  allowedRedirectUris,   // optional exact list every redirect URI must be on
  allowedReturnOrigins,  // optional origins an absolute returnTo may use, e.g. ["https://*.example.com"]
  isAllowedReturnTo,     // optional (url: URL) => boolean for an absolute returnTo
  scope,                 // default "openid email"
  requireVerifiedEmail,  // default true when scope includes email
  cookieName,            // default "aa_sso_flow"
  cookiePath,            // default "/"
  cookieDomain,          // default host-only
  flowTtlSeconds,        // default 600
  clockSkewSeconds,      // default 60
  metadataTtlSeconds,    // discovery + JWKS cache, default 3600
  fetch,                 // default globalThis.fetch
});
```

The constructor validates everything immediately and throws `AnointedSsoError` with code `config` on a
missing or invalid value (fail fast). `redirectUri` is a string or a function of the request; see below.

| Method | What it does |
|---|---|
| `createAuthorizeRequest({ returnTo?, prompt?, loginHint?, extraParams?, redirectUri?, request? })` | `{ url, state, redirectUri, cookie, setCookieHeader }`: redirect to `url` and set the flow cookie |
| `handleCallback({ url \| query, cookieValue, redirectUri?, request? })` | Checks state, flow age, `iss`, provider errors; exchanges the code; validates the ID token; returns `{ user, claims, tokens, returnTo }` |
| `refresh(refreshToken, { expectedSub? })` | New tokens (needs `offline_access`). Refresh tokens rotate: store the new one |
| `revoke(token, { tokenTypeHint? })` | RFC 7009 revocation. Call it on sign-out when you hold a refresh token |
| `fetchUserInfo(accessToken)` | Standard OIDC claims for the approved scopes |
| `buildLogoutUrl({ idTokenHint?, postLogoutRedirectUri?, state? })` | Provider logout URL |
| `verifyLogoutToken(logoutToken, { maxAgeSeconds? })` | Validates a back-channel Logout Token; returns `{ sub, sid, jti, exp }` |
| `validateReturnTo(value)` | The safe return target (path or allowlisted URL) or `null` |
| `resolveRedirectUri(request?)` | The validated redirect URI for a request |
| `clearFlowCookieHeader()` | `Set-Cookie` that deletes the flow cookie (the adapters do this for you) |

Adapters: `createExpressHandlers(client, { onSignedIn?, onError?, onLogout?, returnToParam? })` and
`createWebHandlers(client, { onSignedIn?, onError?, onLogout?, returnToParam? })` return
`{ start, callback, backchannelLogout }`. `onSignedIn` is needed by `callback` only (calling `callback`
without it throws a `config` error), so a start-only route needs no options. `returnToParam` names the
query parameter `start` reads (default `returnTo`). `backchannelLogout` exists only when you pass `onLogout`.

### Several hosts and cross-site return

One client can serve several hosts. Pass `redirectUri` as a function of the request (the web adapter passes
the `Request`, the Express adapter the node request), or pass `redirectUri` per call. Every value must be
https and, with `allowedRedirectUris`, exactly one of that list (the kit also accepts `http://localhost` for
a local test provider, but Anointed Automation only registers https callbacks). Pin the list
whenever the URI comes from a `Host` header, so a spoofed header cannot steer the flow anywhere else. Each
URI must also be registered with us.

```js
const client = createAnointedClient({
  issuer: ANOINTED_ISSUER,
  clientId,
  clientSecret,
  redirectUri: (request) => new URL("/auth/anointed/callback", request.url).toString(),
  allowedRedirectUris: ["https://a.example/auth/anointed/callback", "https://b.example/auth/anointed/callback"],
  allowedReturnOrigins: ["https://www.example.com", "https://*.example.com"],
});
const handlers = createWebHandlers(client, { returnToParam: "next", onSignedIn });
```

`allowedReturnOrigins` lets an absolute `returnTo` point at those origins. `https://*.example.com` covers any
subdomain at any depth but not `example.com` itself (list it separately); a port must match exactly;
`http://` is accepted only for a loopback host such as `http://localhost:3000`. `isAllowedReturnTo(url)`
adds your own rule. Even with either option the kit never accepts another scheme (`javascript:`, `data:`),
protocol-relative `//host`, credentials in the URL, backslashes or control characters, and the value is
re-validated when the callback reads the flow cookie. With neither option only same-site paths are kept.

### Errors

Every failure is an `AnointedSsoError` with a stable `code`. Branch on the code and show the user a
generic message; `error.message` is for your logs only.

| Code | Meaning | Typical handling |
|---|---|---|
| `access_denied` | The user declined | Back to your login page, no error banner |
| `state_mismatch`, `flow_missing`, `flow_expired` | Stale tab, forged request, or cookies blocked | Restart sign-in |
| `token_exchange` with `oauthError: "invalid_grant"` | Reused or expired code; on refresh: the user disconnected your app | Sign in again |
| `id_token_invalid`, `issuer_mismatch` | Token failed validation | Log and refuse |
| `email_not_verified` | No verified email | Refuse, or set `requireVerifiedEmail: false` and handle it yourself |
| `discovery`, `http` | We could not be reached | Retry later |
| `config` | Your configuration is wrong | Fix it; thrown at startup (or at the exchange when `clientSecret` is missing) |
| `redirect_uri_rejected` | A per-request redirect URI was not https or not on `allowedRedirectUris` | Refuse; check the `Host` the request arrived with |

## Back-channel logout

Register a `backchannel_logout_uri` with us and mount `backchannelLogout` there (POST). We call it when a
user disconnects your app, their account is deleted or merged, an admin revokes their grant, or your app is
disabled. In `onLogout`, end every session for `logout.sub` (or `logout.sid`), drop any refresh token you
hold, and ignore a `jti` you have already processed. The handler answers 200 / 400 / 500 exactly as the
spec and our retry policy expect.

## Security checklist

- [ ] The client secret is only in your server's secret store. Not in code, git, `.env.example`, logs,
      browser bundles or mobile apps. (In Next.js never prefix it with `NEXT_PUBLIC_`.)
- [ ] Your redirect URI is https and registered exactly. Anointed Automation never registers an http
      callback, `localhost` included; the kit refuses http except on localhost (local test providers only).
- [ ] Users are keyed on `sub`, never on email or username.
- [ ] You never auto-link accounts on an unverified email (the kit enforces this with the `email` scope).
- [ ] You request the smallest scope that works: `openid email` by default.
- [ ] Your session cookie is `HttpOnly`, `Secure`, `SameSite=Lax` (or stricter), and you rotate the
      session id at sign-in.
- [ ] `onError` shows a generic message and never echoes `error.message` to the user.
- [ ] You honor revocation: a refresh `invalid_grant` or a back-channel logout ends the user's session.
- [ ] Account recovery: a user who signed up only through Anointed Automation can still get back in (email
      reset or a local password).
- [ ] Your server clock is NTP-synced (ID token `exp`/`iat` are checked with 60 seconds of skew).

What the kit already does for you: PKCE S256 with a fresh 64-character verifier, 256-bit `state` and
`nonce`, constant-time state and nonce comparison, state checked before anything else (also on error
callbacks), a 10-minute flow cookie (`HttpOnly`, `SameSite=Lax`, `Secure` on https), RFC 9207 `iss` check,
RS256-only signature verification (no `alg: none`, no HMAC confusion), exact `iss`, `aud`/`azp`, `exp`/`iat`
with skew, `sub` presence, JWKS caching with one refetch on an unknown `kid` (key rotation), open-redirect
safe `returnTo` (same-site paths, or only the origins you allowlist), https-only redirect URIs (optionally
pinned to an exact list), and https-only provider endpoints.

## Migrating from 0.1.0

0.2.0 is backward compatible for apps that pass a static `redirectUri` and a `clientSecret`, with three
visible changes:

- `user.email` is now trimmed and lowercased. If you stored the exact claim, read `result.claims.email`.
- `clientSecret` is optional at construction. A client without one can start a sign-in; the code exchange,
  refresh and revoke throw `AnointedSsoError("config")` instead of failing at startup.
- The adapters no longer throw at construction when `onSignedIn` is missing; `callback` throws instead.

New: `redirectUri` as a function, per-call `redirectUri` / `request`, `allowedRedirectUris`,
`allowedReturnOrigins`, `isAllowedReturnTo`, `returnToParam`, `client.validateReturnTo`,
`client.resolveRedirectUri`, `AuthorizeRequest.redirectUri`, and the `redirect_uri_rejected` error code.
`client.redirectUri` is `null` when `redirectUri` is a function.

## Testing

```bash
npm test                                              # Node: unit tests + a mocked end-to-end flow
deno test --allow-net --no-check test/deno/smoke.ts   # Deno (Base44 runtime) smoke test
```

The tests run against a local fake provider (`test/helpers/fakeIdp.js`) with its own throwaway keys; they
never call api.anointedautomation.net.

## License

[MIT](../../LICENSE). Copyright © Anointed Automation, LLC. Stewarded by Alexander Fields. Jesus is King ✝️
