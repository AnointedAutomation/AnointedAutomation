<p align="center"><img src="https://raw.githubusercontent.com/AnointedAutomation/AnointedAutomation/master/js/anointed-sso/assets/banner.jpg" alt="Anointed Automation" width="720"></p>

# AnointedSSO (Android)

Jesus is King ✝️

"Sign in with Anointed Automation" for **Android 7.0+ (API 24)** apps. Kotlin, Custom Tabs, one dependency
(`androidx.browser`). Jetpack Compose button included (optional).

## How it works

Your client secret NEVER goes in the app (anyone can pull it out of an APK). So the app does not talk to
Anointed Automation directly:

1. The app opens your server's start route in a Custom Tab.
2. Your server runs the normal flow with [`@anointedautomation/sso`](../../js/anointed-sso) (or any OIDC
   library) and keeps the secret.
3. When it's done, your server sends the person back to the app with whatever the app needs, for example a
   short-lived one-time code your app trades for its own session.
4. The kit checks the `aa_state` it sent came back (constant-time compare, so a link nobody started is
   refused) and hands you the query parameters.

## Install

From Maven Central:

```kotlin
// app/build.gradle.kts (mavenCentral() is in every new Android project)
implementation("com.anointedautomation:anointedsso:0.1.1")
```

From source: clone this repo, then either

- include the module: in your `settings.gradle.kts`,
  `include(":anointedsso"); project(":anointedsso").projectDir = file("/path/to/AnointedAutomation/android/AnointedSSO/anointedsso")`
  and `implementation(project(":anointedsso"))`, or
- publish to Maven Local: `cd android/AnointedSSO && gradle :anointedsso:publishToMavenLocal`,
  add `mavenLocal()` and use `com.anointedautomation:anointedsso:0.1.1`.

## Use it

Manifest: an intent-filter on the activity that receives the callback.

```xml
<!-- App Link (preferred): verify your domain with /.well-known/assetlinks.json -->
<activity android:name=".SignInDoneActivity" android:exported="true" android:launchMode="singleTask">
    <intent-filter android:autoVerify="true">
        <action android:name="android.intent.action.VIEW" />
        <category android:name="android.intent.category.DEFAULT" />
        <category android:name="android.intent.category.BROWSABLE" />
        <data android:scheme="https" android:host="app.yourapp.com" android:path="/anointed/done" />
    </intent-filter>
</activity>

<!-- or a custom scheme (see the table below) -->
<intent-filter>
    <action android:name="android.intent.action.VIEW" />
    <category android:name="android.intent.category.DEFAULT" />
    <category android:name="android.intent.category.BROWSABLE" />
    <data android:scheme="myapp" android:host="anointed-sign-in" />
</intent-filter>
```

Code (keep the `AnointedSignIn` instance in a singleton or application scope so the pending state survives
the activity being recreated; pass `AnointedPreferencesStore(context)` as a third argument to also survive the
process being killed):

```kotlin
val signIn = AnointedSignIn(
    startUrl = Uri.parse("https://api.yourapp.com/auth/anointed/start"),
    callback = AnointedCallback.Https(host = "app.yourapp.com", path = "/anointed/done"),
)

// Compose
AnointedSignInButton(onClick = { signIn.signIn(activity) })

// In the activity that owns the intent-filter (onCreate and onNewIntent)
if (signIn.isCallback(intent)) {
    try {
        val params = signIn.handleCallback(intent)   // e.g. mapOf("code" to "...") from YOUR server
        yourApi.exchange(params.getValue("code"))    // your own session
    } catch (e: AnointedSignInException) {
        // e.code: state_mismatch, missing_state, provider_error, invalid_callback, no_pending_request, config
    }
}
```

Compose is a `compileOnly` dependency: the button needs `androidx.compose.foundation` and `material3` in your app,
and nothing else in the kit touches Compose. The logo is live at
`https://www.anointedautomation.net/favicon.png`; pass it to the button's `logo` slot with your image loader.

### Callback choices

| | Needs | Notes |
|---|---|---|
| `AnointedCallback.Https(host, path)` | Verified App Link for your domain (`assetlinks.json`) | Preferred: no other app can claim it |
| `AnointedCallback.Scheme("myapp", via)` | Custom scheme `myapp` in the manifest, an https `via` route on your server | `via` forwards every query parameter to `myapp://anointed-sign-in`. Any app can register a scheme, which is why the `aa_state` check matters |

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

// Only for AnointedCallback.Scheme: forward to the app.
app.get("/auth/anointed/app-done", (req, res) => {
  res.redirect("myapp://anointed-sign-in?" + new URLSearchParams(req.query).toString());
});
```

Send the person back with `?error=<code>` (keeping `aa_state`) to fail the sign-in; the app gets an
`AnointedSignInException` with code `provider_error` and the server's code as the message.

Credentials (`client_id` = `aa_<your key id>`, the secret, your https callback URL) are self-serve for Tech
partners in the partner portal ([**Keys**](https://www.anointedautomation.net/partner?tab=keys) and [**Sign in**](https://www.anointedautomation.net/partner?tab=signin) tabs, confirmed with an emailed code). See
[How to get access](../../docs/SIGN_IN_WITH_ANOINTED_AUTOMATION.md#2-how-to-get-access).

## New accounts get a password (server side)

On a person's first ever sign-in to your app, Anointed Automation generates a password for their account on
YOUR service and shows it to them once on the consent screen. Your server (never the app: it needs the client
secret) takes it once with `client.takeNewUserPassword(user.sub)` from
[`@anointedautomation/sso`](../../js/anointed-sso#new-accounts-get-a-password) right after the callback, and
stores its own hash. Never create the account with a null password. Nothing changes in the app.

## Tests

```
cd android/AnointedSSO
gradle :anointedsso:testDebugUnitTest
```

The request and callback logic (`AnointedSignInRequest`) is plain JVM Kotlin, so its JUnit tests need no
emulator.
