<p align="center"><img src="https://raw.githubusercontent.com/AnointedAutomation/AnointedAutomation/master/js/anointed-sso/assets/banner.jpg" alt="Anointed Automation" width="720"></p>

# @anointedautomation/sso-vue

Jesus is King ✝️

A "Sign in with Anointed Automation" button and composable for Vue 3. It only sends the browser to **your
server's** start route. There are no secrets, no tokens and no OIDC logic in the browser. Works in Vite, Nuxt 3/4
and any other Vue 3.3+ setup. No framework imports, no build step (render-function component).

## 5-minute quick start

**1. Install** (peer dependency: `vue >= 3.3`)

```bash
npm install @anointedautomation/sso-vue
```

**2. Add the button**

```vue
<script setup>
import { AnointedSignInButton } from "@anointedautomation/sso-vue";
</script>

<template>
  <AnointedSignInButton start-url="/api/auth/anointed/start" return-to="/dashboard" />
</template>
```

**3. Add the server routes.** The server holds the client secret and does the OpenID Connect exchange:
[@anointedautomation/sso-nuxt](https://github.com/AnointedAutomation/AnointedAutomation/tree/master/js/anointed-sso-nuxt) for Nuxt, or
[@anointedautomation/sso](https://github.com/AnointedAutomation/AnointedAutomation/tree/master/js/anointed-sso) for Express and any fetch-style runtime.
Get credentials in the partner portal ([How to get access](https://github.com/AnointedAutomation/AnointedAutomation/blob/master/docs/SIGN_IN_WITH_ANOINTED_AUTOMATION.md#2-how-to-get-access)).

Or build your own button with the composable:

```vue
<script setup>
import { useAnointedSignIn } from "@anointedautomation/sso-vue";
const { signIn } = useAnointedSignIn({ startUrl: "/api/auth/anointed/start" });
</script>

<template>
  <button type="button" @click="signIn('/dashboard')">Continue with Anointed Automation</button>
</template>
```

`signIn(returnTo?)` navigates the browser to `startUrl?returnTo=<returnTo>`. Your server validates `returnTo`
(same-site paths only by default) and finishes the sign-in.

## Props and options

| Name | Where | Meaning |
|---|---|---|
| `startUrl` / `start-url` | button, composable | **Required.** Path or URL of your server's start route. Missing means an error, there is no default |
| `returnTo` / `return-to` | button | Where to land after sign-in. Omit for none |
| `returnToParam` / `return-to-param` | button, composable | Query parameter your server reads. Default `returnTo` (match the server's `returnToParam`) |
| `label` | button | Button text. Default "Sign in with Anointed Automation" |
| `class` | button | Your CSS class. When set, the built-in inline look is dropped |
| `disabled` | button | Disables the button |
| `navigate` | button, composable | Override `window.location.assign` (tests, custom routers) |

The button emits `click` before navigating. It is a real `<button type="button">`, so keyboard and screen readers
work. The logo is decorative (`alt=""`, `aria-hidden`).

In Nuxt, the start route is a full-page redirect to another site, so keep the default `navigate` (do not use
`navigateTo` with client-side routing).

## Testing

```bash
npm test   # installs the dev dependencies on first run, then node:test with @vue/server-renderer
```

## License

[MIT](https://github.com/AnointedAutomation/AnointedAutomation/blob/master/LICENSE). Copyright © Anointed Automation, LLC. Stewarded by Alexander Fields. Jesus is King ✝️
