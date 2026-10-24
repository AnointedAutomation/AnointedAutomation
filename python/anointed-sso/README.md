# anointedautomation-sso

Jesus is King ✝️

"Sign in with Anointed Automation" for Python: a server-side OpenID Connect kit with a framework-neutral core and thin
adapters for Django, FastAPI/Starlette and Flask. Authorization code + PKCE (S256), `state`, `nonce`, RS256 ID token
verification through the issuer's JWKS, refresh, revoke, userinfo, logout, back-channel logout (validation plus replay
protection) and a fail-open session status client. Python 3.10+, MIT.

Server only: the client holds your client secret and does the token exchange. Never ship it to a browser or a mobile app.
Get your `client_id` and `client_secret` from the partner portal; the full guide is
[Sign in with Anointed Automation](https://github.com/AnointedAutomation/AnointedAutomation/blob/master/docs/SIGN_IN_WITH_ANOINTED_AUTOMATION.md).

```bash
pip install anointedautomation-sso                # core
pip install "anointedautomation-sso[flask]"       # or [django], [fastapi]
```

```
ANOINTED_OAUTH_CLIENT_ID=aa_<your key id>
ANOINTED_OAUTH_CLIENT_SECRET=<server only>
ANOINTED_OAUTH_REDIRECT_URI=https://yourapp.example/auth/anointed/callback
```

Register the callback URL (exact `https` match) and, optionally, the back-channel logout URL on the portal's **Sign in**
tab. Nothing has a hidden default: a missing required setting raises `AnointedSsoError` with code `config`.

## Core

```python
from anointedautomation_sso import ANOINTED_ISSUER, AnointedClient, require_env

client = AnointedClient(
    issuer=ANOINTED_ISSUER,
    client_id=require_env("ANOINTED_OAUTH_CLIENT_ID"),
    client_secret=require_env("ANOINTED_OAUTH_CLIENT_SECRET"),
    redirect_uri=require_env("ANOINTED_OAUTH_REDIRECT_URI"),
    # scope="openid email",   # the default; add "profile" or "offline_access" only if you need them
)

auth = client.create_authorize_request(return_to="/account")
# 302 to auth.url and set the flow cookie: auth.cookie_name / auth.cookie_value / auth.cookie_options
# (or send auth.set_cookie_header as is).

result = client.handle_callback(query=request_query_dict, cookie_value=flow_cookie_value)
result.user.sub        # key YOUR users on this
result.user.email      # verified email only, trimmed and lowercase
result.return_to       # safe same-site path or None
# Always clear the flow cookie afterwards (client.clear_flow_cookie_header()), on success and failure.
```

What `handle_callback` enforces: the flow cookie exists, `state` matches (constant time) before anything else, the flow is
not older than 10 minutes, the RFC 9207 `iss` parameter (when sent) is the issuer, then the code exchange with the PKCE
verifier and a fully verified ID token (RS256 only, `iss`, `aud`/`azp`, `exp`/`iat` with skew, `nonce`, `sub`, verified
email when the `email` scope is requested). Failures raise `AnointedSsoError`; branch on `err.code`
(`state_mismatch`, `flow_expired`, `access_denied`, `id_token_invalid`, `email_not_verified`, `token_exchange`, ...) and
never show `str(err)` to users.

Other calls: `refresh(refresh_token, expected_sub=...)` (a new refresh token each time; `invalid_grant` means signed out),
`revoke(token)`, `fetch_user_info(access_token)`, `build_logout_url(id_token_hint=..., post_logout_redirect_uri=...)`,
`take_new_user_password(sub)` (the one-time password generated for a brand new account on your site; hash it yourself).

`return_to` is open-redirect safe: same-site paths only, unless you list `allowed_return_origins` (for example
`["https://www.example.com", "https://*.example.com"]`) or an `is_allowed_return_to` predicate. Even then it is https only.

## Flask

```python
from flask import Flask, redirect, session
from anointedautomation_sso import ANOINTED_ISSUER, AnointedClient, require_env
from anointedautomation_sso.adapters.flask import create_blueprint

app = Flask(__name__)
client = AnointedClient(
    issuer=ANOINTED_ISSUER,
    client_id=require_env("ANOINTED_OAUTH_CLIENT_ID"),
    client_secret=require_env("ANOINTED_OAUTH_CLIENT_SECRET"),
    redirect_uri=require_env("ANOINTED_OAUTH_REDIRECT_URI"),
)

def on_signed_in(result):
    session["user_sub"] = result.user.sub      # find or create YOUR user by sub
    return redirect(result.return_to or "/")

def on_logout(logout):
    ...  # end every session for logout.sub (or logout.sid)

app.register_blueprint(create_blueprint(client, on_signed_in=on_signed_in, on_logout=on_logout), url_prefix="/auth/anointed")
# GET /auth/anointed/start?returnTo=/account, GET /auth/anointed/callback, POST /auth/anointed/backchannel-logout
```

## Django

```python
# myapp/anointed.py
from django.contrib.auth import get_user_model, login
from django.http import HttpResponseRedirect
from anointedautomation_sso import ANOINTED_ISSUER, AnointedClient, require_env
from anointedautomation_sso.adapters.django import create_views

client = AnointedClient(
    issuer=ANOINTED_ISSUER,
    client_id=require_env("ANOINTED_OAUTH_CLIENT_ID"),
    client_secret=require_env("ANOINTED_OAUTH_CLIENT_SECRET"),
    redirect_uri=require_env("ANOINTED_OAUTH_REDIRECT_URI"),
)

def on_signed_in(request, result):
    user, _ = get_user_model().objects.get_or_create(username=f"aa_{result.user.sub}", defaults={"email": result.user.email or ""})
    login(request, user)
    return HttpResponseRedirect(result.return_to or "/")

def on_logout(logout):
    ...  # delete the sessions of logout.sub (or logout.sid)

views = create_views(client, on_signed_in=on_signed_in, on_logout=on_logout)

# urls.py
urlpatterns = [path("auth/anointed/", include(views.urls()))]
```

The back-channel route is CSRF-exempt by design (the provider posts without your cookies); everything on it is verified
by the Logout Token signature instead. Behind a proxy set `SECURE_PROXY_SSL_HEADER` as usual; the redirect URI is the
fixed value you configured, not derived from the request.

## FastAPI / Starlette

```python
from fastapi import FastAPI
from fastapi.responses import RedirectResponse
from anointedautomation_sso import ANOINTED_ISSUER, AnointedClient, require_env
from anointedautomation_sso.adapters.fastapi import create_router   # Starlette: create_routes

app = FastAPI()
client = AnointedClient(
    issuer=ANOINTED_ISSUER,
    client_id=require_env("ANOINTED_OAUTH_CLIENT_ID"),
    client_secret=require_env("ANOINTED_OAUTH_CLIENT_SECRET"),
    redirect_uri=require_env("ANOINTED_OAUTH_REDIRECT_URI"),
)

async def on_signed_in(request, result):
    request.session["user_sub"] = result.user.sub      # needs SessionMiddleware, or use your own session
    return RedirectResponse(result.return_to or "/", status_code=302)

async def on_logout(logout):
    ...  # end every session for logout.sub (or logout.sid)

app.include_router(create_router(client, on_signed_in=on_signed_in, on_logout=on_logout), prefix="/auth/anointed")
```

Callbacks may be sync or async. The blocking core runs in a worker thread, so the event loop is never blocked. For plain
Starlette use `create_routes(...)` and mount the returned routes.

## Back-channel logout

When someone signs out or disconnects your app, Anointed Automation POSTs a signed `logout_token` to your back-channel
logout URL. `process_backchannel_logout` (used by every adapter) validates it (RS256 via JWKS, `typ: logout+jwt`, `iss`,
`aud`, `iat`, the logout event, no `nonce`, a `jti`, `sub` and/or `sid`) and answers: 400 for a bad token, 200 for a
repeated `jti` (your handler is not run twice), 500 when the keys cannot be read or your handler raised (the provider
retries, and the `jti` is forgotten so the retry runs). The default replay store is in-memory; with several processes pass a
shared `LogoutReplayStore` (`try_add(jti, remember_until)` and `remove(jti)`) backed by Redis or your database.

## Session status

End a local session when the user's Anointed Automation account or grant ends. It needs a partner API key with the
`sessions:status` permission (ask us to add it) and **fails open**: an outage never signs your users out.

```python
from anointedautomation_sso import SessionStatusClient

status = SessionStatusClient(api_key=require_env("ANOINTED_API_KEY"), app_name="My App")
check = status.check(user_sub, grant=True)
if check.should_end_session:
    ...  # sign the user out locally; check.reason says why
```

## Security checklist

- Keep `client_secret` and API keys in your secret store, never in source control, logs or a browser bundle.
- Redirect URIs must be exact `https` matches; use an https tunnel or staging host to test.
- Key your users on `sub`, not on email.
- Run the callback and the back-channel route over HTTPS only; send `Cache-Control: no-store` (the adapters do).
- Use a shared replay store when you run more than one process.

## Development

```bash
python -m venv .venv && .venv/bin/pip install -e ".[test]" && .venv/bin/python -m pytest
```

Source: [python/anointed-sso](https://github.com/AnointedAutomation/AnointedAutomation/tree/master/python/anointed-sso).
The Node kit is [@anointedautomation/sso](https://github.com/AnointedAutomation/AnointedAutomation/tree/master/js/anointed-sso)
and the .NET kit is [AnointedAutomation.SSO](https://github.com/AnointedAutomation/AnointedAutomation/tree/master/dotnet/AnointedAutomation.SSO).
