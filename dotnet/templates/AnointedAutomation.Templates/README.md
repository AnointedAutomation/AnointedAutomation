# AnointedAutomation.Templates

Jesus is King ✝️

`dotnet new` templates for "Sign in with Anointed Automation", wired to
[AnointedAutomation.SSO](https://github.com/AnointedAutomation/AnointedAutomation/tree/master/dotnet/AnointedAutomation.SSO)
and [AnointedAutomation.SSO.UI](https://github.com/AnointedAutomation/AnointedAutomation/tree/master/dotnet/AnointedAutomation.SSO.UI).

| Template | Short name | What you get |
|---|---|---|
| Razor Pages app | `anointed-razor` | Cookie session, `MapAnointedAutomationAuth`, `<anointed-sign-in>` / `<anointed-sign-out>` in the layout |
| Blazor Web App (interactive server) | `anointed-blazor` | Same server wiring, `<AnointedSignInButton>` / `<AnointedSignOutButton>` inside `<AuthorizeView>` |

Both are backend for frontend apps: the browser only holds the session cookie, never a token.

## Use

```bash
dotnet new install AnointedAutomation.Templates
dotnet new anointed-razor -n MyApp      # or: dotnet new anointed-blazor -n MyApp
cd MyApp
dotnet user-secrets set "Anointed:ClientId" "aa_<your key id>"
dotnet user-secrets set "Anointed:ClientSecret" "<your client secret>"
dotnet run
```

The app refuses to start until both secrets are set (no defaults). In production, supply
`Anointed__ClientId` and `Anointed__ClientSecret` from your secret store instead.

Get the client id and secret in the partner portal (https://www.anointedautomation.net/partner): create a key on
the Keys tab, then turn on sign-in on the Sign in tab and register your exact callback URL,
`https://<your host>/auth/anointed/callback` (for local runs, the https URL `dotnet run` prints). Full guide:
[Sign in with Anointed Automation](https://github.com/AnointedAutomation/AnointedAutomation/blob/master/docs/SIGN_IN_WITH_ANOINTED_AUTOMATION.md).

## Routes in the generated app

| Route | Purpose |
|---|---|
| `GET /auth/anointed/start?returnTo=/path` | Start sign-in |
| `/auth/anointed/callback` | OpenID Connect callback (handled by AnointedAutomation.SSO) |
| `POST /auth/anointed/sign-out` | End the local session |
| `GET /auth/anointed/user` | Signed-in user as JSON (no tokens) |

## Uninstall

```bash
dotnet new uninstall AnointedAutomation.Templates
```

## License

MIT.
