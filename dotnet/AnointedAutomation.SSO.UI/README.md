# AnointedAutomation.SSO.UI

Jesus is King ✝️

The ASP.NET Core front end for "Sign in with Anointed Automation": brand-styled, accessible sign-in and
sign-out buttons for **Razor Pages / MVC** (Tag Helpers) and **Blazor** (Blazor Server, or a Blazor Web App
with static SSR or interactive server rendering), plus `MapAnointedAutomationAuth()` minimal API routes.

It sits on top of [AnointedAutomation.SSO](https://github.com/AnointedAutomation/AnointedAutomation/tree/master/dotnet/AnointedAutomation.SSO),
which does the OpenID Connect work. The design is a **backend for frontend (BFF)**: the browser only ever holds
your own session cookie. No ID, access or refresh token is sent to the browser, and the user-info route
returns claims, never tokens. The buttons look the same as the
[React button](https://github.com/AnointedAutomation/AnointedAutomation/tree/master/js/anointed-sso-react).

**Server rendering only.** Do not use this in Blazor WebAssembly: there is no client secret in the browser.
A WebAssembly app calls the `user` and `sign-out` routes of its ASP.NET Core host instead.

Starting fresh? `dotnet new install AnointedAutomation.Templates`, then `dotnet new anointed-razor` or
`dotnet new anointed-blazor` ([templates](https://github.com/AnointedAutomation/AnointedAutomation/tree/master/dotnet/templates/AnointedAutomation.Templates)).

## Install

```bash
dotnet add package AnointedAutomation.SSO.UI
```

## Wire it up (`Program.cs`)

```csharp
using AnointedAutomation.SSO;
using AnointedAutomation.SSO.UI;
using Microsoft.AspNetCore.Authentication.Cookies;

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = AnointedAutomationDefaults.AuthenticationScheme;
    })
    .AddCookie()
    .AddAnointedAutomation(options =>
    {
        options.ClientId = builder.Configuration["Anointed:ClientId"];         // required, no default
        options.ClientSecret = builder.Configuration["Anointed:ClientSecret"]; // server only
        options.CallbackPath = "/auth/anointed/callback";
    });
builder.Services.AddRazorPages(); // or AddRazorComponents(); both register antiforgery

WebApplication app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapAnointedAutomationAuth("/auth/anointed");
```

`AddAnointedAutomation` throws at startup when the client id or secret is missing.

### Routes

| Route | What it does |
|---|---|
| `GET /auth/anointed/start?returnTo=/account` | Starts sign-in. `returnTo` must be a same-site path (`/account`); anything else lands on `/`. |
| `POST /auth/anointed/sign-out` | Ends the local session. Needs a form antiforgery token (the buttons add it) or, from script, the header `X-Anointed-CSRF: 1` (answers `204`). |
| `GET /auth/anointed/user` | `{"isAuthenticated":true,"sub":"...","email":"...","name":null,"preferredUsername":null,"picture":null}` for single page apps. Never a token. `Cache-Control: no-store`. |

Options (`app.MapAnointedAutomationAuth("/auth/anointed", o => { ... })`): `ChallengeScheme` (default `Anointed`),
`CookieScheme` (default `Cookies`), `ReturnToParam` (default `returnTo`), `DefaultReturnPath` (default `/`).
The call returns the route group so you can add rate limiting or other conventions.

Sign-out ends the local session only. To react to a sign-out at Anointed Automation, map the back-channel logout
endpoint from AnointedAutomation.SSO.

## Razor Pages / MVC

`_ViewImports.cshtml`:

```cshtml
@addTagHelper *, AnointedAutomation.SSO.UI
```

```cshtml
@if (User.Identity.IsAuthenticated)
{
    <anointed-sign-out action="/auth/anointed/sign-out" />
}
else
{
    <anointed-sign-in start-url="/auth/anointed/start" return-to="/account" />
}
```

Attributes: `start-url` (required) / `action` (required), `return-to`, `return-to-param`, `label`. Pass `class` to
drop the built-in inline look and style the button yourself.

## Blazor

`_Imports.razor`: `@using AnointedAutomation.SSO.UI.Components`

```razor
<AuthorizeView>
    <Authorized><AnointedSignOutButton Action="/auth/anointed/sign-out" /></Authorized>
    <NotAuthorized><AnointedSignInButton StartUrl="/auth/anointed/start" ReturnTo="/account" /></NotAuthorized>
</AuthorizeView>
```

The sign-in button is a real link marked `data-enhance-nav="false"` so Blazor does a full page load to the start
route. The sign-out button is a plain POST form with `<AntiforgeryToken />`, so it works in static SSR and in
interactive server mode. Parameters: `StartUrl` / `Action` (required), `ReturnTo`, `ReturnToParam`, `Label`,
`Class`, plus any extra attributes.

## Accessibility

Real `<a>` and `<button>` elements, visible text label, decorative logo with `alt=""` and `aria-hidden="true"`,
white on `#1f2a44` (contrast well above WCAG AA).

## License

MIT.
