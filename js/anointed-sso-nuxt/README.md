<p align="center"><img src="https://raw.githubusercontent.com/AnointedAutomation/AnointedAutomation/master/js/anointed-sso/assets/banner.jpg" alt="Anointed Automation" width="720"></p>

# @anointedautomation/sso-nuxt

Jesus is King ✝️

"Sign in with Anointed Automation" for **Nuxt 3 and Nuxt 4**. Four server route handlers (`start`, `callback`,
`logout`, `backchannelLogout`) built on [@anointedautomation/sso](https://github.com/AnointedAutomation/AnointedAutomation/tree/master/js/anointed-sso), which does all the OpenID
Connect work (PKCE S256, state, nonce, ID token validation). **Server only**: it holds your client secret. Add the
[Vue button](https://github.com/AnointedAutomation/AnointedAutomation/tree/master/js/anointed-sso-vue) on the client.

## 5-minute quick start

**1. Install**

```bash
npm install @anointedautomation/sso-nuxt @anointedautomation/sso @anointedautomation/sso-vue
```

**2. Environment variables** (`.env`; read them in server code only, never expose them in `runtimeConfig.public`)

```
ANOINTED_OAUTH_ISSUER=https://api.anointedautomation.net/
ANOINTED_OAUTH_CLIENT_ID=aa_<your key id>
ANOINTED_OAUTH_CLIENT_SECRET=<server only>
ANOINTED_OAUTH_REDIRECT_URI=https://yourapp.example/api/auth/anointed/callback
```

Get the credentials in the partner portal; see
[How to get access](https://github.com/AnointedAutomation/AnointedAutomation/blob/master/docs/SIGN_IN_WITH_ANOINTED_AUTOMATION.md#2-how-to-get-access). The redirect URI must be
https and registered exactly.

**3. `server/utils/anointed.js`** (build once; Nitro auto-imports it)

```js
import { createAnointedClient } from "@anointedautomation/sso";
import { createAnointedEventHandlers, redirectTo } from "@anointedautomation/sso-nuxt";

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

export const anointed = createAnointedEventHandlers({
  client,
  async onSignedIn(result, request, { client, event }) {
    const user = result.user; // { sub, email, emailVerified, name, ... } key your users on sub

    // First sign-in: we generated a password for the new account on YOUR site. Take it once and store your
    // own hash. Never create an account with a null password. null means a returning user.
    const password = await client.takeNewUserPassword(user.sub);
    if (password !== null) await createLocalUser({ sub: user.sub, email: user.email, passwordHash: await hash(password) });

    const sessionCookie = await yourSessionCookie(user); // HttpOnly; Secure; SameSite=Lax; Path=/
    return redirectTo(request, result.returnTo === null ? "/" : result.returnTo, [sessionCookie]);
  },
  onSignOut() {
    // End YOUR session. Return the cookies that clear it; the browser then goes to Anointed Automation to sign out.
    return { setCookies: ["app_session=; Path=/; HttpOnly; Secure; SameSite=Lax; Max-Age=0"] };
  },
  postLogoutRedirectUri: "https://yourapp.example/", // optional, register it first
  onError(error, request) {
    // Branch on error.code, show a generic message, never echo error.message.
    return redirectTo(request, error.code === "access_denied" ? "/?signin=cancelled" : "/?signin=failed");
  },
  async onLogout(logout) {
    // Back-channel logout: end every session for logout.sub (or logout.sid), ignore a repeated logout.jti.
  },
});
```

**4. Four route files**

```js
// server/api/auth/anointed/start.get.js
export default defineEventHandler(anointed.start);
```

```js
// server/api/auth/anointed/callback.get.js  (register this URL as your redirect URI)
export default defineEventHandler(anointed.callback);
```

```js
// server/api/auth/anointed/logout.js
export default defineEventHandler(anointed.logout);
```

```js
// server/api/auth/anointed/backchannel-logout.post.js  (register as your backchannel_logout_uri)
export default defineEventHandler(anointed.backchannelLogout);
```

**5. The button** (in any page or component)

```vue
<script setup>
import { AnointedSignInButton } from "@anointedautomation/sso-vue";
</script>

<template>
  <AnointedSignInButton start-url="/api/auth/anointed/start" return-to="/dashboard" />
</template>
```

## API

`createAnointedEventHandlers({ client, onSignedIn, onError?, onLogout?, onSignOut?, postLogoutRedirectUri?, returnToParam? })`
returns `{ start, callback, logout, backchannelLogout }`. Each is `(event) => Promise<Response>`, for
`defineEventHandler(...)`.

| Option | Meaning |
|---|---|
| `client` | **Required.** From `createAnointedClient` in `@anointedautomation/sso` |
| `onSignedIn(result, request, { client, event })` | **Required.** Create YOUR session, return a `Response`. The flow cookie is cleared for you. `event` is the h3 event |
| `onError(error, request)` | Default: a plain 400. Branch on `error.code` |
| `onLogout(logout, request)` | Needed by `backchannelLogout`; calling that handler without it throws a `config` error |
| `onSignOut(request, { client, event })` | Needed by `logout`. Return `{ setCookies?, idTokenHint? }`; the browser is then redirected to the provider's end-session endpoint |
| `postLogoutRedirectUri` | https URL to come back to after the provider signs out. Register it first |
| `returnToParam` | Query parameter `start` reads. Default `returnTo` |

`redirectTo(request, target, setCookies?)` builds the 302 for `onSignedIn`, resolving `target` against the request
and appending each full `Set-Cookie` value. Missing or invalid options throw `AnointedSsoError("config")` when
the handlers are created. h3 v2 events (`event.req`) and h3 v1 events (Nuxt 3 and 4 today) both work.

Everything else (scopes, several hosts, `allowedReturnOrigins`, refresh, revoke, error codes, security checklist)
is in the [@anointedautomation/sso README](https://github.com/AnointedAutomation/AnointedAutomation/blob/master/js/anointed-sso/README.md).

## Testing

```bash
npm test   # node:test with web Requests against the fake provider in ../anointed-sso/test/helpers/fakeIdp.js, plus a real h3 app
```

In this repository `npm test` links the sibling `../anointed-sso` when the registry copy is not installed.

## License

[MIT](https://github.com/AnointedAutomation/AnointedAutomation/blob/master/LICENSE). Copyright © Anointed Automation Corporation. Stewarded by Alexander Fields. Jesus is King ✝️
