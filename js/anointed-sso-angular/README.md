<p align="center"><img src="https://raw.githubusercontent.com/AnointedAutomation/AnointedAutomation/master/js/anointed-sso/assets/banner.jpg" alt="Anointed Automation" width="720"></p>

# @anointedautomation/sso-angular

Jesus is King ✝️

A "Sign in with Anointed Automation" button, service and provider for Angular 17+ (standalone components). It only
sends the browser to **your server's** start route. There are no secrets, no tokens and no OIDC logic in the browser.

## 5-minute quick start

### 1. Server: add the two routes

The server holds the client secret and does the OpenID Connect exchange with
[@anointedautomation/sso](https://github.com/AnointedAutomation/AnointedAutomation/tree/master/js/anointed-sso).
Get credentials first: [How to get access](https://github.com/AnointedAutomation/AnointedAutomation/blob/master/docs/SIGN_IN_WITH_ANOINTED_AUTOMATION.md#2-how-to-get-access).

```bash
npm install @anointedautomation/sso   # on your Node server, never in the Angular app
```

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

app.get("/api/auth/anointed/start", anointed.start);
app.get("/api/auth/anointed/callback", anointed.callback);
```

### 2. Angular: install

```bash
npm install @anointedautomation/sso-angular
```

Peer dependencies: `@angular/core` and `@angular/common` 17 or newer.

### 3. Angular: register the start route

```ts
// app.config.ts
import { ApplicationConfig } from "@angular/core";
import { provideAnointedSignIn } from "@anointedautomation/sso-angular";

export const appConfig: ApplicationConfig = {
  providers: [provideAnointedSignIn({ startUrl: "/api/auth/anointed/start" })],
};
```

### 4. Angular: add the button

```ts
import { Component } from "@angular/core";
import { AnointedSignInButtonComponent } from "@anointedautomation/sso-angular";

@Component({
  selector: "app-login",
  standalone: true,
  imports: [AnointedSignInButtonComponent],
  template: `<anointed-sign-in-button returnTo="/dashboard" />`,
})
export class LoginComponent {}
```

Or use the service from your own button:

```ts
import { inject } from "@angular/core";
import { AnointedSignInService } from "@anointedautomation/sso-angular";

const anointed = inject(AnointedSignInService);
anointed.signIn("/dashboard"); // navigates to /api/auth/anointed/start?returnTo=%2Fdashboard
```

Your server validates `returnTo` (same-site paths only by default) and finishes the sign-in. During development the
Angular dev server must proxy `/api` to your Node server so the start route and the session cookie share one origin.

## API

| Name | Meaning |
|---|---|
| `provideAnointedSignIn({ startUrl, returnToParam?, navigate? })` | Registers the config. `startUrl` is **required**, there is no default. `returnToParam` defaults to `returnTo` (match the server's `returnToParam`). `navigate` overrides `window.location.assign` (tests, custom routers) |
| `<anointed-sign-in-button>` | Inputs: `returnTo`, `returnToParam`, `label` (default "Sign in with Anointed Automation"), `disabled`, `logoUrl`. Output: `signInStarted` |
| `AnointedSignInService.signIn(returnTo?, returnToParam?)` | Navigates the browser to the start route |
| `buildStartUrl(startUrl, returnTo?, returnToParam?)` | Pure URL helper |

The button is a real `<button type="button">`, so keyboard and screen readers work, and it has a visible focus ring.
The logo is decorative (`alt=""`, `aria-hidden`). Restyle it with the CSS custom properties `--anointed-sign-in-bg`,
`--anointed-sign-in-color`, `--anointed-sign-in-radius` and `--anointed-sign-in-focus` on the element or an ancestor.

`signIn` is a full page load (the start route redirects to another site), so call it from a click handler, not
during server-side rendering.

## Testing

```bash
npm test   # installs the dev dependencies on first run, builds with ng-packagr, then node:test against the built package
```

`npm publish` runs the same build through `prepublishOnly`. The package ships Angular partial-compilation output
(`dist/`), which the Angular CLI of any 17+ application links at build time.

## License

[MIT](https://github.com/AnointedAutomation/AnointedAutomation/blob/master/LICENSE). Copyright © Anointed Automation Corporation. Stewarded by Alexander Fields. Jesus is King ✝️
