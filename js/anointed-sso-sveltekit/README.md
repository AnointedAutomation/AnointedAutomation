<p align="center"><img src="https://raw.githubusercontent.com/AnointedAutomation/AnointedAutomation/master/js/anointed-sso/assets/banner.jpg" alt="Anointed Automation" width="720"></p>

# @anointedautomation/sso-sveltekit

Jesus is King ✝️

"Sign in with Anointed Automation" for **SvelteKit (Svelte 5)**. Server handlers for `start`, `callback`, `logout`
and back-channel logout (as a `handle` hook or as `+server.js` helpers) built on
[@anointedautomation/sso](https://github.com/AnointedAutomation/AnointedAutomation/tree/master/js/anointed-sso),
which does all the OpenID Connect work (PKCE S256, state, nonce, ID token validation). Plus an accessible,
brand-styled sign-in button. The client secret lives **only on the server**; the button is a plain link to your
start route.

## Install

```bash
npm install @anointedautomation/sso-sveltekit @anointedautomation/sso
```

## Environment variables

Get the credentials in the partner portal; see
[How to get access](https://github.com/AnointedAutomation/AnointedAutomation/blob/master/docs/SIGN_IN_WITH_ANOINTED_AUTOMATION.md#2-how-to-get-access).
The redirect URI must be https and registered exactly.

```
ANOINTED_OAUTH_CLIENT_ID=aa_<your key id>
ANOINTED_OAUTH_CLIENT_SECRET=<server only, never PUBLIC_>
ANOINTED_OAUTH_REDIRECT_URI=https://yourapp.example/auth/anointed/callback
```

## 5-minute quick start

**1. `src/lib/server/anointed.js`** (server only; build once)

```js
import { env } from "$env/dynamic/private";
import { ANOINTED_ISSUER, createAnointedClient } from "@anointedautomation/sso";
import { createAnointedHandlers, redirectTo } from "@anointedautomation/sso-sveltekit";

/** Throws when a variable is missing: no fallbacks. */
function requireEnv(name) {
  const value = env[name];
  if (value === undefined || value === "") throw new Error(`Missing environment variable ${name}`);
  return value;
}

const client = createAnointedClient({
  issuer: ANOINTED_ISSUER,
  clientId: requireEnv("ANOINTED_OAUTH_CLIENT_ID"),
  clientSecret: requireEnv("ANOINTED_OAUTH_CLIENT_SECRET"),
  redirectUri: requireEnv("ANOINTED_OAUTH_REDIRECT_URI"),
});

export const anointed = createAnointedHandlers({
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
  onSignOut(request) {
    return redirectTo(request, "/", ["session=; Path=/; HttpOnly; Secure; SameSite=Lax; Max-Age=0"]);
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

**2. Mount the routes**, either with one hook or with `+server.js` files.

*Option A, `src/hooks.server.js`* (serves `GET /auth/anointed/start`, `GET /auth/anointed/callback`,
`POST /auth/anointed/logout`, `POST /auth/anointed/backchannel-logout`):

```js
import { sequence } from "@sveltejs/kit/hooks";
import { createAnointedHandle } from "@anointedautomation/sso-sveltekit";
import { anointed } from "$lib/server/anointed.js";

export const handle = sequence(
  createAnointedHandle({ handlers: anointed, basePath: "/auth/anointed" }),
  // ...your own hooks, for example the one that reads your session cookie
);
```

*Option B, `+server.js` files* (the handlers take a standard `Request`):

```js
// src/routes/auth/anointed/start/+server.js
import { anointed } from "$lib/server/anointed.js";
export const GET = ({ request }) => anointed.start(request);
```

```js
// src/routes/auth/anointed/callback/+server.js   (register this URL as your redirect URI)
import { anointed } from "$lib/server/anointed.js";
export const GET = ({ request }) => anointed.callback(request);
```

```js
// src/routes/auth/anointed/logout/+server.js
import { anointed } from "$lib/server/anointed.js";
export const POST = ({ request }) => anointed.logout(request);
```

```js
// src/routes/auth/anointed/backchannel-logout/+server.js   (register as your backchannel_logout_uri)
import { anointed } from "$lib/server/anointed.js";
export const POST = ({ request }) => anointed.backchannelLogout(request);
```

**3. Allow the back-channel POST.** Anointed Automation calls your logout URL server to server with a form
body and no browser `Origin` header, which SvelteKit's CSRF origin check rejects with a 403 in production. A
missing `Origin` never matches a named entry in `trustedOrigins`, so allow it in `svelte.config.js` with
`kit: { csrf: { trustedOrigins: ['*'] } }` (or the deprecated `kit: { csrf: { checkOrigin: false } }`, removed in
SvelteKit 3; see the [SvelteKit configuration docs](https://svelte.dev/docs/kit/configuration#csrf)). This turns
off SvelteKit's origin check for every form POST in the app, so keep your own CSRF protection on other form
actions. The token is verified by signature, issuer, audience and `jti`, so the route does not rely on the origin check. Skip
this step if you do not use back-channel logout.

**4. The button** (in any `.svelte` file; it is a link, so it also works without JavaScript)

```svelte
<script>
  import { AnointedSignInButton } from "@anointedautomation/sso-sveltekit/client";
</script>

<AnointedSignInButton startUrl="/auth/anointed/start" returnTo="/dashboard" />
```

Props: `startUrl` (required), `returnTo`, `returnToParam` (must match the server's, default `returnTo`), `label`
(default "Sign in with Anointed Automation"), `class` (adds your class; the built-in look stays, override it with
your CSS) and `disabled`. The link has `data-sveltekit-reload` so the browser does a full navigation to the
provider.

## API (server)

`createAnointedHandlers({ client, onSignedIn, onSignOut?, onError?, onLogout?, returnToParam? })` returns
`{ start, callback, logout, backchannelLogout }`. Each is `(request: Request) => Promise<Response>`.

| Option | Meaning |
|---|---|
| `client` | **Required.** From `createAnointedClient` in `@anointedautomation/sso` |
| `onSignedIn(result, request, { client })` | **Required.** Create YOUR session, return a `Response`. The flow cookie is cleared for you |
| `onSignOut(request, { client })` | Needed by `logout`: end YOUR session, return a `Response`. Use a POST form to call it |
| `onError(error, request)` | Default: a plain 400. Branch on `error.code` |
| `onLogout(logout, request)` | Needed by `backchannelLogout` |
| `returnToParam` | Query parameter `start` reads. Default `returnTo` |

Calling `logout` or `backchannelLogout` without its callback throws a `config` error. Missing or invalid options
throw `AnointedSsoError("config")` when the handlers are created.

`createAnointedHandle({ handlers, basePath })` returns a SvelteKit `handle` for the four routes under `basePath`
(absolute, no trailing slash). `redirectTo(request, target, setCookies?)` builds the 302 for your callbacks and can
carry `Set-Cookie` headers, which SvelteKit's own `redirect()` cannot.

Everything else (scopes, `allowedReturnOrigins`, refresh, revoke, error codes, security checklist) is in the
[@anointedautomation/sso README](https://github.com/AnointedAutomation/AnointedAutomation/blob/master/js/anointed-sso/README.md).
Using React or Next.js instead? See
[sso-react](https://github.com/AnointedAutomation/AnointedAutomation/tree/master/js/anointed-sso-react) and
[sso-next](https://github.com/AnointedAutomation/AnointedAutomation/tree/master/js/anointed-sso-next).

## Testing

```bash
npm test   # node:test: Request objects against the fake provider in ../anointed-sso/test/helpers/fakeIdp.js, and a server render of the button
```

In this repository `npm test` installs `svelte` and links the sibling `../anointed-sso` when the registry copy is
not installed.

## License

[MIT](https://github.com/AnointedAutomation/AnointedAutomation/blob/master/LICENSE). Copyright © Anointed Automation, LLC. Stewarded by Alexander Fields. Jesus is King ✝️
