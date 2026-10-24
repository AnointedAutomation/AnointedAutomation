# AnointedAutomation.SSO.Owin

Jesus is King ✝️

"Sign in with Anointed Automation" for **classic ASP.NET on .NET Framework 4.7.2+**: MVC 5 and Web Forms on
OWIN/Katana. This package is a thin, opinionated layer over the standard `Microsoft.Owin.Security.OpenIdConnect`
middleware (it does not hand-roll the protocol), plus what that middleware does not give you: a **back-channel
logout** endpoint, a **session status** client and a **brand sign-in button** for Razor and Web Forms.

On ASP.NET Core use [AnointedAutomation.SSO](https://github.com/AnointedAutomation/AnointedAutomation/tree/master/dotnet/AnointedAutomation.SSO) instead.

| | |
|---|---|
| Issuer | `https://api.anointedautomation.net/` |
| Discovery | `https://api.anointedautomation.net/.well-known/openid-configuration` |
| ID tokens | RS256, verified against the published JWKS |
| Flow | Authorization code + PKCE (`S256`) only; `state` required; `nonce` used |
| Full guide | [Sign in with Anointed Automation](https://github.com/AnointedAutomation/AnointedAutomation/blob/master/docs/SIGN_IN_WITH_ANOINTED_AUTOMATION.md): access, credentials, claims, rotation, troubleshooting |

**Server only.** This package holds your client secret and your partner API key. Never ship either to a browser
or anything else that runs on a user's device.

## 1. Get your credentials

Sign-in is self-serve for Tech partners. Apply at https://www.anointedautomation.net/partners/apply, then in the
partner portal create an API key on the [**Keys** tab](https://www.anointedautomation.net/partner?tab=keys), and on
the [**Sign in** tab](https://www.anointedautomation.net/partner?tab=signin) turn sign-in on for that key with your
exact callback URL (absolute `https`, exact match) and, if you want it, your back-channel logout URL. You get:

- `client_id`: `aa_<your key id>`
- `client_secret`: shown exactly once. Store it only in your server's secret store.

Steps in detail: [How to get access](https://github.com/AnointedAutomation/AnointedAutomation/blob/master/docs/SIGN_IN_WITH_ANOINTED_AUTOMATION.md#2-how-to-get-access).

## 2. Install

```powershell
Install-Package AnointedAutomation.SSO.Owin
Install-Package Microsoft.Owin.Host.SystemWeb
```

`Microsoft.Owin.Host.SystemWeb` runs the OWIN pipeline inside IIS / IIS Express and gives you
`SystemWebCookieManager` and `HttpContext.GetOwinContext()`.

## 3. Startup (shared by MVC 5 and Web Forms)

`Startup.cs` at the project root:

```csharp
using AnointedAutomation.SSO.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Host.SystemWeb;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;
using System.Configuration;
using System.Threading.Tasks;

[assembly: OwinStartup(typeof(YourApp.Startup))]

namespace YourApp
{
    public class Startup
    {
        public static IAnointedSessionStatus SessionStatus;

        public void Configuration(IAppBuilder app)
        {
            // Optional: lets the cookie hook below ask whether a session is still alive (fail open).
            SessionStatus = AnointedSessionStatusClient.Create(new AnointedSessionStatusOptions
            {
                ApiKey = Required("ANOINTED_API_KEY"), // needs the sessions:status scope; server only
                AppName = "Your App",
            }, app.CreateLogger<AnointedSessionStatusClient>());

            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = CookieAuthenticationDefaults.AuthenticationType,
                CookieManager = new SystemWebCookieManager(), // avoids the classic Katana/System.Web lost-cookie bug
                CookieHttpOnly = true,
                CookieSecure = CookieSecureOption.Always,
                Provider = new CookieAuthenticationProvider
                {
                    OnValidateIdentity = context =>
                        AnointedSessionStatusCookieEvents.ValidateIdentityAsync(context, SessionStatus, grant: true),
                },
            });

            app.UseAnointedAutomation(new AnointedAutomationOptions
            {
                ClientId = Required("ANOINTED_OAUTH_CLIENT_ID"),         // aa_<your key id>
                ClientSecret = Required("ANOINTED_OAUTH_CLIENT_SECRET"), // server only
                RedirectUri = "https://www.example.com/auth/anointed/callback", // must match the registered URL
                ConfigureOpenIdConnect = oidc => oidc.CookieManager = new SystemWebCookieManager(),
            });

            app.MapAnointedBackchannelLogout("/auth/anointed/backchannel-logout", (logout, owinContext) =>
            {
                // End every session for logout.Sub (or logout.Sid) and drop any refresh token you hold.
                return YourSessionStore.EndAllAsync(logout.Sub, logout.Sid);
            });
        }

        private static string Required(string name)
        {
            // appSettings: keep the secrets out of source control (a non-committed file, or your host's app settings).
            string value = ConfigurationManager.AppSettings[name];
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidOperationException("Missing required setting " + name);
            }
            return value;
        }
    }
}
```

`appSettings` keys used above:

```
ANOINTED_OAUTH_CLIENT_ID=aa_<your key id>
ANOINTED_OAUTH_CLIENT_SECRET=<server only>
ANOINTED_API_KEY=<server only, only for session status>
```

## 4a. MVC 5 quick start

A start action that challenges the Anointed scheme (`Controllers/AuthController.cs`):

```csharp
using AnointedAutomation.SSO.Owin;
using Microsoft.Owin.Security;
using System.Web;
using System.Web.Mvc;

public class AuthController : Controller
{
    [AllowAnonymous]
    [Route("auth/anointed/start")]
    public ActionResult Start(string returnTo)
    {
        string target = Url.IsLocalUrl(returnTo) ? returnTo : "/";
        HttpContext.GetOwinContext().Authentication.Challenge(
            new AuthenticationProperties { RedirectUri = target },
            AnointedAutomationDefaults.AuthenticationType); // "Anointed"
        return new HttpUnauthorizedResult();
    }
}
```

The button in any Razor view (add `<add namespace="AnointedAutomation.SSO.Owin" />` to `Views/web.config` or use
`@using AnointedAutomation.SSO.Owin`):

```cshtml
@Html.AnointedSignInButton("~/auth/anointed/start")
@Html.AnointedSignInButton("~/auth/anointed/start", "/account")
@Html.AnointedSignInButton(new AnointedSignInButtonOptions { StartUrl = "~/auth/anointed/start", CssClass = "btn btn-dark" })
```

Read the user in a controller:

```csharp
string sub = ((ClaimsPrincipal)User).GetAnointedSubject();   // stable user id: key your users on this
string sid = ((ClaimsPrincipal)User).GetAnointedSessionId(); // grant id, matches back-channel logout's Sid
string email = ((ClaimsPrincipal)User).GetAnointedEmail();   // verified email (null without the email scope)
```

## 4b. Web Forms quick start

A start page (`Auth/Start.aspx.cs`):

```csharp
using AnointedAutomation.SSO.Owin;
using Microsoft.Owin.Security;
using System;
using System.Web;

public partial class Start : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string returnTo = Request.QueryString["returnTo"];
        bool local = !string.IsNullOrEmpty(returnTo) && returnTo.StartsWith("/") && !returnTo.StartsWith("//") && !returnTo.StartsWith("/\\");
        Context.GetOwinContext().Authentication.Challenge(
            new AuthenticationProperties { RedirectUri = local ? returnTo : "/" },
            AnointedAutomationDefaults.AuthenticationType);
        Response.StatusCode = 401;
        Response.End();
    }
}
```

The button on any page or master page:

```aspx
<%@ Register Assembly="AnointedAutomation.SSO.Owin" Namespace="AnointedAutomation.SSO.Owin.WebForms" TagPrefix="anointed" %>

<anointed:AnointedSignInButton runat="server" StartUrl="~/Auth/Start.aspx" ReturnTo="/Account.aspx" />
```

(Or register the tag once in `web.config` under `system.web/pages/controls`.) Read the user with
`Context.GetOwinContext().Authentication.User.GetAnointedSubject()`.

## What `UseAnointedAutomation` sets

| Setting | Value |
|---|---|
| Authority | `https://api.anointedautomation.net/` (https metadata required) |
| AuthenticationType | `Anointed` (change with `AuthenticationType`) |
| ResponseType / ResponseMode | `code` / `query`, `RedeemCode = true` |
| PKCE | on (`S256`) |
| RedirectUri / CallbackPath | your `RedirectUri` / its path |
| Scope | `openid email` plus `AdditionalScopes` |
| Claims | raw OIDC names (`MapInboundClaims = false`), name claim `name` |
| ID token algorithms | RS256 only |
| SaveTokens | false unless you set `SaveTokens = true` |
| Verified email | with the email scope, a sign-in whose `email_verified` is not true is refused (`RequireVerifiedEmail`, default true) |

`ClientId` (must start with `aa_`), `ClientSecret` and `RedirectUri` (absolute `https` with its own path) are
required and have no defaults: a missing or invalid value throws `InvalidOperationException` while the app starts.
Your own `Notifications` are kept: the verified-email check runs first, then your `SecurityTokenValidated`. A
refused sign-in throws `AnointedSignInRejectedException`, which Katana hands to the `AuthenticationFailed`
notification: handle it there, call `HandleResponse()`, and redirect to a generic error page.
`ConfigureOpenIdConnect = oidc => { ... }` gets the last word on the `OpenIdConnectAuthenticationOptions`.

## Scope advice

- **The default scope is `openid email`. Keep it unless you truly need more.**
- Requesting `profile` makes users without a username pick one before consenting. Add it only when you need it.
- `offline_access` adds a refresh token; ask for it only if you call our API later (then set `SaveTokens = true`).
- **Key your users on `sub`, never on email.**

## Back-channel logout

`app.MapAnointedBackchannelLogout(path, onLogout)` answers anonymous POSTs at that path (it runs in the OWIN
pipeline before MVC, so no antiforgery token is involved; other methods get 405). Register its full https URL with
us. It validates the Logout Token exactly as OpenID Connect Back-Channel Logout 1.0 asks: RS256 only, signature
against our JWKS (refetched on an unknown `kid`), `typ` `logout+jwt`, exact `iss`, `aud` containing your
client_id, `iat` no older than 5 minutes (`BackchannelLogoutMaxAge`), `exp` not passed, the back-channel event,
a `jti`, `sub` and/or `sid`, and no `nonce`, with 60 seconds of clock skew (`ClockSkew`).

| Situation | Answer | `onLogout` called |
|---|---|---|
| Valid token | 200 | yes, once |
| Same `jti` again (our retry) | 200 | no |
| Invalid token, or no `logout_token` | 400 | no |
| `onLogout` throws, or our keys cannot be read | 500 (we retry with the same `jti`) | the retry runs it again |

Processed `jti` values are remembered in memory until the token expires. On more than one server, set
`AnointedAutomationOptions.LogoutReplayStore` to your own `IAnointedLogoutReplayStore` (for example over Redis).

## Session status (fail open)

`IAnointedSessionStatus.CheckAsync(userId, grant, cancellationToken)` asks
`GET https://api.anointedautomation.net/api/session-status?userId=<sub>&grant=<true|false>` whether a session may
continue. Your API key needs the `sessions:status` scope (ask us to add it). The client exchanges the key for a
machine token, caches it, re-exchanges once on a 401, times out each request after 3 seconds and reuses each
answer for 5 minutes. Create ONE client per app with `AnointedSessionStatusClient.Create(options, logger)`.

**It FAILS OPEN.** Only a 200 that says `active: false` is `Ended`. A network error, a timeout, any other status
or an unreadable body is `Unknown`: not cached, logged, and you **keep** the session.
`AnointedSessionStatusCookieEvents.ValidateIdentityAsync(context, status, grant)` is a ready `OnValidateIdentity`
hook: only on `Ended` does it reject the identity and sign the cookie out.

## Security checklist

- [ ] The client secret and API key live only in your server's secret store (not in a committed `web.config`).
- [ ] Your redirect URI is https and registered exactly.
- [ ] Users are keyed on `sub`, never on email or username.
- [ ] Session cookies are `HttpOnly`, `Secure`, and use `SystemWebCookieManager`.
- [ ] `AuthenticationFailed` shows a generic message and never echoes the raw provider error.
- [ ] You honor revocation: a back-channel logout (or an `Ended` session status) ends the user's session.
- [ ] Your server clock is NTP-synced.

## License

[MIT](https://github.com/AnointedAutomation/AnointedAutomation/blob/master/LICENSE). Copyright © Anointed
Automation, LLC. Stewarded by Alexander Fields. Jesus is King ✝️
