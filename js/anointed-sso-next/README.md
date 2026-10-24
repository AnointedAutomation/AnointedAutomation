<p align="center"><img src="https://raw.githubusercontent.com/AnointedAutomation/AnointedAutomation/master/js/anointed-sso/assets/banner.jpg" alt="Anointed Automation" width="720"></p>

# @anointedautomation/sso-next

Jesus is King ✝️

"Sign in with Anointed Automation" for the **Next.js App Router**. Three route handlers (`start`, `callback`,
`backchannelLogout`) built on [@anointedautomation/sso](../anointed-sso), which does all the OpenID Connect work
(PKCE S256, state, nonce, ID token validation). **Server only**: it holds your client secret. Add the
[React button](../anointed-sso-react) on the client.

## Install

```bash
npm install @anointedautomation/sso-next @anointedautomation/sso
```

## Environment variables

```
ANOINTED_OAUTH_ISSUER=https://api.anointedautomation.net/
ANOINTED_OAUTH_CLIENT_ID=aa_<your key id>
ANOINTED_OAUTH_CLIENT_SECRET=<server only, never NEXT_PUBLIC_>
ANOINTED_OAUTH_REDIRECT_URI=https://yourapp.example/api/auth/anointed/callback
```

Get the credentials in the partner portal; see
[How to get access](../../docs/SIGN_IN_WITH_ANOINTED_AUTOMATION.md#2-how-to-get-access). The redirect URI must be
https and registered exactly.

## Quick start

**1. `lib/anointed.js`** (server only; build once)

```js
import { createAnointedClient } from "@anointedautomation/sso";
import { createAnointedRouteHandlers, redirectTo } from "@anointedautomation/sso-next";

/** Throws when a variable is missing: no fallbacks. */
function requireEnv(name) {
  const value = process.env[name];
  if (value === undefined || value === "") throw new Error(`Missing environment variable ${name}`);
  return value;
}

const client = createAnointedClient({
  issuer: requireEnv("ANOINTED_OAUTH_ISSUER"),
  clientId: requireEnv("ANOINTED_OAUTH_CLIENT_ID"),
  clientSecret: requireEnv("ANOINTED_OAUTH_CLIENT_SECRET"),
  redirectUri: requireEnv("ANOINTED_OAUTH_REDIRECT_URI"),
});

export const anointed = createAnointedRouteHandlers({
  client,
  async onSignedIn(result, request, { client }) {
    const user = result.user; // { sub, email, emailVerified, name, ... } key your users on sub

    // First sign-in: we generated a password for the new account on YOUR site. Take it once and store your
    // own hash. Never create an account with a null password. null means a returning user.
    const password = await client.takeNewUserPassword(user.sub);
    if (password !== null) await createLocalUser({ sub: user.sub, email: user.email, passwordHash: await hash(password) });

    const sessionCookie = await yourSessionCookie(user); // HttpOnly; Secure; SameSite=Lax; Path=/
    return redirectTo(request, result.returnTo === null ? "/" : result.returnTo, [sessionCookie]);
  },
  onError(error, request) {
    // Branch on error.code, show a generic message, never echo error.message.
    return redirectTo(request, error.code === "access_denied" ? "/?signin=cancelled" : "/?signin=failed");
  },
  async onLogout(logout) {
    // Back-channel logout: end every session for logout.sub (or logout.sid), ignore a repeated logout.jti.
  },
});
```

**2. Three route files**

```js
// app/api/auth/anointed/start/route.js
import { anointed } from "../../../../../lib/anointed.js";
export const dynamic = "force-dynamic";
export const GET = anointed.start;
```

```js
// app/api/auth/anointed/callback/route.js  (register this URL as your redirect URI)
import { anointed } from "../../../../../lib/anointed.js";
export const dynamic = "force-dynamic";
export const GET = anointed.callback;
```

```js
// app/api/auth/anointed/backchannel-logout/route.js  (register as your backchannel_logout_uri)
import { anointed } from "../../../../../lib/anointed.js";
export const dynamic = "force-dynamic";
export const POST = anointed.backchannelLogout;
```

**3. The button** (client component)

```jsx
import { AnointedSignInButton } from "@anointedautomation/sso-react";
<AnointedSignInButton startUrl="/api/auth/anointed/start" returnTo="/dashboard" />
```

## API

`createAnointedRouteHandlers({ client, onSignedIn, onError?, onLogout?, returnToParam? })` returns
`{ start, callback, backchannelLogout }`. Each is `(request: Request) => Promise<Response>`.

| Option | Meaning |
|---|---|
| `client` | **Required.** From `createAnointedClient` in `@anointedautomation/sso` |
| `onSignedIn(result, request, { client })` | **Required.** Create YOUR session, return a `Response`. The flow cookie is cleared for you |
| `onError(error, request)` | Default: a plain 400. Branch on `error.code` |
| `onLogout(logout, request)` | Needed by `backchannelLogout`; calling that handler without it throws a `config` error |
| `returnToParam` | Query parameter `start` reads. Default `returnTo` |

`redirectTo(request, target, setCookies?)` builds the 302 for `onSignedIn`, resolving `target` against the request
and appending each full `Set-Cookie` value. Missing or invalid options throw `AnointedSsoError("config")` when
the handlers are created.

Everything else (scopes, several hosts, `allowedReturnOrigins`, refresh, revoke, error codes, security
checklist) is in the [@anointedautomation/sso README](../anointed-sso/README.md).

## Testing

```bash
npm test   # node:test with Request objects against the fake provider in ../anointed-sso/test/helpers/fakeIdp.js
```

In this repository `npm test` links the sibling `../anointed-sso` when the registry copy is not installed.

## License

[MIT](../../LICENSE). Copyright © Anointed Automation Corporation. Stewarded by Alexander Fields. Jesus is King ✝️
