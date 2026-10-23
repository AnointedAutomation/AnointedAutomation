<p align="center"><img src="https://raw.githubusercontent.com/AnointedAutomation/AnointedAutomation/master/js/anointed-sso/assets/banner.jpg" alt="Anointed Automation" width="720"></p>

# @anointedautomation/sso-react

Jesus is King ✝️

A "Sign in with Anointed Automation" button and hook for React. It only sends the browser to **your server's**
start route. There are no secrets, no tokens and no OIDC logic in the browser. Works in Vite, Create React App,
Next.js (App Router or Pages), Remix and any other React 18+ setup. No framework imports.

## Install

```bash
npm install @anointedautomation/sso-react
```

Peer dependency: `react >= 18`. TypeScript users also need `@types/react`.

## Use

```jsx
import { AnointedSignInButton } from "@anointedautomation/sso-react";

export function LoginPage() {
  return <AnointedSignInButton startUrl="/api/auth/anointed/start" returnTo="/dashboard" />;
}
```

Or build your own button with the hook:

```jsx
import { useAnointedSignIn } from "@anointedautomation/sso-react";

export function MyButton() {
  const { signIn } = useAnointedSignIn({ startUrl: "/api/auth/anointed/start" });
  return <button onClick={() => signIn("/dashboard")}>Continue with Anointed Automation</button>;
}
```

`signIn(returnTo?)` navigates the browser to `startUrl?returnTo=<returnTo>`. Your server validates `returnTo`
(same-site paths only by default) and finishes the sign-in.

## Props and options

| Name | Where | Meaning |
|---|---|---|
| `startUrl` | button, hook | **Required.** Path or URL of your server's start route. Missing means an error, there is no default |
| `returnTo` | button | Where to land after sign-in. Omit for none |
| `returnToParam` | button, hook | Query parameter your server reads. Default `returnTo` (match the server's `returnToParam`) |
| `label` | button | Button text. Default "Sign in with Anointed Automation" |
| `className` | button | Your CSS class. When set, the built-in inline look is dropped |
| `disabled` | button | Disables the button |
| `navigate` | button, hook | Override `window.location.assign` (tests, custom routers) |

The button is a real `<button type="button">`, so keyboard and screen readers work. The logo is decorative
(`alt=""`, `aria-hidden`). In Next.js the files carry `"use client"`, so import them from any component.

## Server side

The server holds the client secret and does the OpenID Connect exchange. Use
[@anointedautomation/sso](../anointed-sso) (Express, any fetch-style runtime) or
[@anointedautomation/sso-next](../anointed-sso-next) (Next.js App Router route handlers) for the start and
callback routes, and read the full guide in
[Sign in with Anointed Automation](../../docs/SIGN_IN_WITH_ANOINTED_AUTOMATION.md).

## Testing

```bash
npm test   # installs the dev dependencies on first run, then node:test with react-dom/server
```

## License

[MIT](../../LICENSE). Copyright © Anointed Automation, LLC. Stewarded by Alexander Fields. Jesus is King ✝️
