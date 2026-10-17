<p align="center"><img src="https://raw.githubusercontent.com/AnointedAutomation/AnointedAutomation/master/js/anointed-sso/assets/banner.jpg" alt="Anointed Automation" width="720"></p>

# AnointedSSO (Swift)

Jesus is King ✝️

"Sign in with Anointed Automation" for **iOS 15+ and macOS 12+** apps. Swift Package Manager, no dependencies.

## How it works

Your client secret NEVER goes in the app (anyone can pull it out of an .ipa). So the app does not talk to
Anointed Automation directly:

1. The app opens your server's start route in the system sign-in sheet (`ASWebAuthenticationSession`).
2. Your server runs the normal flow with [`@anointedautomation/sso`](../../js/anointed-sso) (or any OIDC
   library) and keeps the secret.
3. When it's done, your server sends the person back to the app with whatever the app needs, for example a
   short-lived one-time code your app trades for its own session.
4. The kit checks the `aa_state` it sent came back (so a link nobody started is refused) and hands you the
   query items.

## Install

Xcode > File > Add Package Dependencies > `https://github.com/AnointedAutomation/AnointedAutomation`,
product `AnointedSSO`. Or in `Package.swift`:

```swift
.package(url: "https://github.com/AnointedAutomation/AnointedAutomation", from: "0.1.0")
```

## Use it

```swift
import AnointedSSO

let signIn = AnointedSignIn(
    start: URL(string: "https://api.yourapp.com/auth/anointed/start")!,
    callback: .https(host: "app.yourapp.com", path: "/anointed/done")   // iOS 17.4+
)

// SwiftUI
AnointedSignInButton {
    Task {
        do {
            let items = try await signIn.signIn()      // e.g. ["code": "..."] from YOUR server
            try await yourApi.exchange(items["code"]!) // your own session
        } catch AnointedSignInError.cancelled {
            // the person closed the sheet
        }
    }
}
```

### Callback choices

| | Needs | Notes |
|---|---|---|
| `.https(host:path:)` | iOS 17.4+ / macOS 14.4+, your domain in Associated Domains (`webcredentials:`) | Preferred: no other app can claim it |
| `.scheme("myapp", via:)` | iOS 15+, URL scheme `myapp` in Info.plist, an https `via` route on your server | `via` forwards every query item to `myapp://anointed-sign-in` |

## Server side (Node example)

```js
const client = createAnointedClient({
  issuer: ANOINTED_ISSUER,
  clientId: requireEnv("ANOINTED_OAUTH_CLIENT_ID"),
  clientSecret: requireEnv("ANOINTED_OAUTH_CLIENT_SECRET"),
  redirectUri: requireEnv("ANOINTED_OAUTH_REDIRECT_URI"),
  allowedReturnOrigins: ["https://app.yourapp.com"], // the app's return origin (or your own api origin for `via`)
});

const anointed = createExpressHandlers(client, {
  async onSignedIn(result, req, res) {
    const user = await findOrCreateUser(result.user.sub, result.user.email);
    const code = await mintOneTimeAppCode(user); // single use, ~60 seconds
    const back = new URL(result.returnTo);       // carries aa_state; keep it
    back.searchParams.set("code", code);
    res.redirect(back.toString());
  },
});

app.get("/auth/anointed/start", anointed.start);
app.get("/auth/anointed/callback", anointed.callback);

// Only for .scheme(..., via:): forward to the app.
app.get("/auth/anointed/app-done", (req, res) => {
  res.redirect("myapp://anointed-sign-in?" + new URLSearchParams(req.query).toString());
});
```

Send the person back with `?error=<code>` (keeping `aa_state`) to fail the sign-in; the app gets
`AnointedSignInError.serverError(code)`.

Credentials (`client_id` = `aa_<your key id>`, the secret, your https callback URL) are self-serve for Tech
partners in the partner portal (**Keys** and **Sign in** tabs, confirmed with an emailed code). See
[How to get access](../../docs/SIGN_IN_WITH_ANOINTED_AUTOMATION.md#2-how-to-get-access).

## Releasing

Bump `swift/AnointedSSO/VERSION` (MAJOR.MINOR.PATCH) and merge to `master`. The `Swift AnointedSSO release`
workflow tags that commit with the version, which is what Swift Package Manager and the
[Swift Package Index](https://swiftpackageindex.com) read. Every tag on this repo is a Swift package version, so
only that workflow creates tags.
