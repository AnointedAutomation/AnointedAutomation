<p align="center"><img src="https://raw.githubusercontent.com/AnointedAutomation/AnointedAutomation/master/js/anointed-sso/assets/banner.jpg" alt="Anointed Automation" width="720"></p>

# @anointedautomation/sso-react-native

Jesus is King ✝️

"Sign in with Anointed Automation" for **React Native and Expo**: an accessible brand-styled button and a
`useAnointedSignIn` hook. The app only opens **your server's** start route in the system auth session and reads
the deep-link callback. No secrets, tokens or OIDC logic in the app. Same contract as the
[Swift](https://github.com/AnointedAutomation/AnointedAutomation/tree/master/swift/AnointedSSO) and
[Android](https://github.com/AnointedAutomation/AnointedAutomation/tree/master/android/AnointedSSO) kits.

## How it works

Your client secret NEVER goes in the app (anyone can pull it out of a bundle).

1. The app opens your server's start route (`?returnTo=<callback>?aa_state=<random>`) in the system auth session.
2. Your server runs the normal flow with [`@anointedautomation/sso`](https://github.com/AnointedAutomation/AnointedAutomation/tree/master/js/anointed-sso) and keeps the secret.
3. It sends the person back to the app with what the app needs, for example a short-lived one-time code.
4. The kit checks the `aa_state` it sent came back (so a link nobody started is refused) and hands you the query items.

## Install

```bash
npm install @anointedautomation/sso-react-native
```

Peers: `react >= 18`, `react-native >= 0.72`, and `expo-web-browser` (optional: only for the default opener).

## Quick start: Expo

```bash
npx expo install expo-web-browser expo-crypto
```

```jsx
import { getRandomBytes } from "expo-crypto";
import { AnointedSignInButton, useAnointedSignIn, schemeCallback } from "@anointedautomation/sso-react-native";

export function LoginScreen() {
  const { signIn, status, error } = useAnointedSignIn({
    start: "https://api.yourapp.com/auth/anointed/start",
    callback: schemeCallback("myapp", "https://api.yourapp.com/auth/anointed/app-done"),
    randomBytes: getRandomBytes, // only needed when globalThis.crypto.getRandomValues is missing
  });

  async function onPress() {
    const items = await signIn(); // e.g. { code: "..." } from YOUR server, or null on failure
    if (items) await yourApi.exchange(items.code); // trade it for your own session
  }

  return <AnointedSignInButton onPress={onPress} loading={status === "pending"} />;
}
```

Set `"scheme": "myapp"` in `app.json`. When `status === "error"`, `error.code` says why (below); `cancelled` is
not a failure worth reporting.

## Quick start: bare React Native

Bring any auth-session opener that returns `{ type: "success", url }` or `{ type: "cancel" }`, for example
[react-native-inappbrowser-reborn](https://github.com/proyecto26/react-native-inappbrowser):

```jsx
import { InAppBrowser } from "react-native-inappbrowser-reborn";
import { AnointedSignInButton, useAnointedSignIn, schemeCallback } from "@anointedautomation/sso-react-native";

const opener = (startUrl, callbackUrl) => InAppBrowser.openAuth(startUrl, callbackUrl);

const { signIn } = useAnointedSignIn({
  start: "https://api.yourapp.com/auth/anointed/start",
  callback: schemeCallback("myapp", "https://api.yourapp.com/auth/anointed/app-done"),
  opener,
});
```

No auth-session library? `createLinkingOpener(Linking)` opens the system browser and waits for your scheme's
deep link (register the scheme in `Info.plist` and `AndroidManifest.xml`).

## Server side

```js
const anointed = createExpressHandlers(client, {
  async onSignedIn(result, req, res) {
    const user = await findOrCreateUser(result.user.sub, result.user.email);
    const code = await mintOneTimeAppCode(user);  // single use, ~60 seconds
    const back = new URL(result.returnTo);        // carries aa_state; keep it
    back.searchParams.set("code", code);
    res.redirect(back.toString());
  },
});
app.get("/auth/anointed/start", anointed.start);
app.get("/auth/anointed/callback", anointed.callback);

// Only for schemeCallback(...): forward every query item to the app.
app.get("/auth/anointed/app-done", (req, res) => {
  res.redirect("myapp://anointed-sign-in?" + new URLSearchParams(req.query).toString());
});
```

Allow your `via` origin (or app return origin) in `allowedReturnOrigins`. To fail a sign-in, send the person back
with `?error=<code>` and keep `aa_state`. Full guide:
[Sign in with Anointed Automation](https://github.com/AnointedAutomation/AnointedAutomation/blob/master/docs/SIGN_IN_WITH_ANOINTED_AUTOMATION.md).

## API

| Export | Meaning |
|---|---|
| `AnointedSignInButton` | Props: `onPress` (required), `label`, `disabled`, `loading`, `style`, `textStyle`. `Pressable` with `accessibilityRole="button"`, a label, busy state, 48 pt height, decorative logo |
| `useAnointedSignIn(options)` | Returns `{ signIn, status, result, error, reset }`. `signIn()` resolves with the items or `null` |
| `createAnointedSignIn(options)` | Same without React: `{ signIn }` that throws `AnointedSignInError` |
| `schemeCallback(scheme, via)` | Custom scheme. `via` is an https route on your server that forwards to `<scheme>://anointed-sign-in`. Works everywhere |
| `httpsCallback(host, path)` | Universal / app link on your domain. Needs platform support and an opener that matches https callbacks |
| `createExpoOpener()` / `createLinkingOpener(Linking)` | Built-in openers. Or pass your own `opener(startUrl, callbackUrl, { ephemeral })` |

Options: `start` (required, https), `callback` (required), `opener`, `ephemeral` (no shared browser cookies),
`randomBytes`. There are no defaults for `start` or `callback`; a missing or invalid one throws at once. The
state is never made from `Math.random`: without `crypto.getRandomValues` or `randomBytes` it throws.

### Error codes (`error.code`)

`config`, `cancelled` (the person closed the sheet), `state_mismatch`, `missing_state` (the callback is not ours),
`server_error` (your server sent `?error=`; the message is its code), `presentation` (the system sheet failed).
None is retried for you.

## Testing

```bash
npm test   # node --test with a mocked opener and a react-native stub
```

## License

[MIT](https://github.com/AnointedAutomation/AnointedAutomation/blob/master/LICENSE). Copyright © Anointed Automation Corporation. Stewarded by Alexander Fields. Jesus is King ✝️
