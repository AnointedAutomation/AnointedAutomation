# Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
"""Core client: config, authorize request, callback, tokens, refresh, logout url, userinfo."""

from __future__ import annotations

import base64
import hashlib
import json
from urllib.parse import parse_qs, urlsplit

import pytest

from anointedautomation_sso import ANOINTED_ISSUER, AnointedClient, AnointedSsoError, require_env
from anointedautomation_sso.http import HttpResponse

from .conftest import CLIENT_ID, CLIENT_SECRET, NOW, REDIRECT_URI, Provider, make_key, start_flow


def code_of(call) -> str:
    with pytest.raises(AnointedSsoError) as err:
        call()
    return err.value.code


def callback(client, auth, state, code="abc", **extra):
    query = {"state": state, "code": code}
    query.update(extra)
    return client.handle_callback(query=query, cookie_value=auth.cookie_value)


def test_missing_required_config_raises():
    for kwargs in (
        dict(issuer="", client_id=CLIENT_ID, redirect_uri=REDIRECT_URI),
        dict(issuer=ANOINTED_ISSUER, client_id="", redirect_uri=REDIRECT_URI),
        dict(issuer=ANOINTED_ISSUER, client_id=CLIENT_ID, redirect_uri=""),
        dict(issuer=ANOINTED_ISSUER, client_id=CLIENT_ID, redirect_uri="http://app.example/cb"),
        dict(issuer=ANOINTED_ISSUER, client_id=CLIENT_ID, redirect_uri=REDIRECT_URI + "#frag"),
        dict(issuer=ANOINTED_ISSUER, client_id=CLIENT_ID, redirect_uri=REDIRECT_URI, scope="email"),
        dict(issuer=ANOINTED_ISSUER, client_id=CLIENT_ID, redirect_uri=REDIRECT_URI, client_secret=" "),
        dict(issuer=ANOINTED_ISSUER, client_id=CLIENT_ID, redirect_uri=REDIRECT_URI, cookie_name="bad name"),
    ):
        assert code_of(lambda kw=kwargs: AnointedClient(**kw)) == "config"


def test_require_env(monkeypatch):
    monkeypatch.delenv("AA_TEST_VAR", raising=False)
    with pytest.raises(AnointedSsoError):
        require_env("AA_TEST_VAR")
    monkeypatch.setenv("AA_TEST_VAR", "  ")
    with pytest.raises(AnointedSsoError):
        require_env("AA_TEST_VAR")
    monkeypatch.setenv("AA_TEST_VAR", "v")
    assert require_env("AA_TEST_VAR") == "v"


def test_authorize_request_has_pkce_state_nonce(client, provider):
    auth = client.create_authorize_request(return_to="/account", prompt="login", login_hint="a@b.c", extra_params={"x": "1"})
    url = urlsplit(auth.url)
    assert url.geturl().startswith(ANOINTED_ISSUER + "connect/authorize?")
    q = {k: v[0] for k, v in parse_qs(url.query).items()}
    assert q["client_id"] == CLIENT_ID and q["redirect_uri"] == REDIRECT_URI
    assert q["response_type"] == "code" and q["scope"] == "openid email"
    assert q["code_challenge_method"] == "S256"
    assert q["prompt"] == "login" and q["login_hint"] == "a@b.c" and q["x"] == "1"
    assert q["state"] and q["nonce"] and q["state"] == auth.state
    flow = client.decode_flow_cookie(auth.cookie_value)
    digest = hashlib.sha256(flow["verifier"].encode()).digest()
    assert base64.urlsafe_b64encode(digest).rstrip(b"=").decode() == q["code_challenge"]
    assert flow["nonce"] == q["nonce"] and flow["return_to"] == "/account"
    assert "HttpOnly" in auth.set_cookie_header and "Secure" in auth.set_cookie_header
    assert "SameSite=Lax" in auth.set_cookie_header and "Max-Age=600" in auth.set_cookie_header


def test_reserved_extra_params_rejected(client):
    assert code_of(lambda: client.create_authorize_request(extra_params={"state": "x"})) == "config"


def test_unsafe_return_to_dropped(client):
    for bad in ("//evil.example", "https://evil.example/", "/\\evil", "javascript:alert(1)"):
        auth = client.create_authorize_request(return_to=bad)
        assert client.decode_flow_cookie(auth.cookie_value)["return_to"] is None


def test_full_callback_success(client, provider):
    auth, state = start_flow(client, provider, return_to="/dash")
    result = callback(client, auth, state)
    assert result.user.sub == "user-1"
    assert result.user.email == "alex@example.com" and result.user.email_verified
    assert result.user.sid == "grant-1"
    assert result.return_to == "/dash"
    assert result.tokens.refresh_token == "rt" and result.tokens.expires_at == NOW + 3600
    form = provider.form_of_last_token_call()
    assert form["grant_type"] == "authorization_code" and form["code"] == "abc"
    assert form["redirect_uri"] == REDIRECT_URI and form["client_secret"] == CLIENT_SECRET
    flow = client.decode_flow_cookie(auth.cookie_value)
    assert form["code_verifier"] == flow["verifier"]


def test_callback_accepts_full_url(client, provider):
    auth, state = start_flow(client, provider)
    result = client.handle_callback(url=f"{REDIRECT_URI}?code=abc&state={state}", cookie_value=auth.cookie_value)
    assert result.user.sub == "user-1"


def test_callback_failures(client, provider):
    auth, state = start_flow(client, provider)
    assert code_of(lambda: client.handle_callback(query={"state": state, "code": "c"}, cookie_value=None)) == "flow_missing"
    assert code_of(lambda: client.handle_callback(query={"state": state, "code": "c"}, cookie_value="garbage")) == "flow_missing"
    assert code_of(lambda: callback(client, auth, "wrong")) == "state_mismatch"
    assert code_of(lambda: client.handle_callback(query={"code": "c"}, cookie_value=auth.cookie_value)) == "state_mismatch"
    assert code_of(lambda: callback(client, auth, state, code="")) == "missing_code"
    assert code_of(lambda: callback(client, auth, state, iss="https://evil.example/")) == "issuer_mismatch"
    assert code_of(lambda: callback(client, auth, state, error="access_denied")) == "access_denied"
    assert code_of(lambda: callback(client, auth, state, error="server_error")) == "provider_error"
    assert code_of(lambda: client.handle_callback(cookie_value=auth.cookie_value)) == "config"


def test_state_checked_before_provider_error(client, provider):
    auth, _ = start_flow(client, provider)
    assert code_of(lambda: callback(client, auth, "wrong", error="access_denied")) == "state_mismatch"


def test_flow_expired(provider):
    clock = [NOW]
    c = AnointedClient(issuer=ANOINTED_ISSUER, client_id=CLIENT_ID, client_secret=CLIENT_SECRET, redirect_uri=REDIRECT_URI, http=provider, now=lambda: clock[0])
    auth, state = start_flow(c, provider)
    clock[0] += 601
    assert code_of(lambda: callback(c, auth, state)) == "flow_expired"


def test_matching_iss_accepted(client, provider):
    auth, state = start_flow(client, provider)
    assert callback(client, auth, state, iss=ANOINTED_ISSUER).user.sub == "user-1"


@pytest.mark.parametrize(
    "overrides",
    [
        {"iss": "https://evil.example/"},
        {"aud": "other"},
        {"aud": [CLIENT_ID, "other"]},
        {"azp": "other"},
        {"exp": NOW - 1000},
        {"iat": NOW + 1000},
        {"nonce": "wrong"},
        {"sub": None},
        {"exp": None},
    ],
)
def test_bad_id_token_claims_rejected(client, provider, overrides):
    auth, state = start_flow(client, provider)
    token = provider.id_token(**overrides)
    if "nonce" in overrides:
        provider.pending_nonce = overrides["nonce"]
        token = provider.id_token(nonce="wrong")
    provider.token_response = {"id_token": token, "access_token": "at"}
    assert code_of(lambda: callback(client, auth, state)) == "id_token_invalid"


def test_id_token_signed_by_other_key_rejected(client, provider):
    auth, state = start_flow(client, provider)
    other, _ = make_key("k1")
    claims = {"iss": ANOINTED_ISSUER, "aud": CLIENT_ID, "sub": "u", "exp": NOW + 60, "iat": NOW, "nonce": provider.pending_nonce, "email": "a@b.c", "email_verified": True}
    provider.token_response = {"id_token": provider.sign(claims, key=other)}
    assert code_of(lambda: callback(client, auth, state)) == "id_token_invalid"


def test_non_rs256_rejected(client, provider):
    import jwt as pyjwt

    auth, state = start_flow(client, provider)
    claims = {"iss": ANOINTED_ISSUER, "aud": CLIENT_ID, "sub": "u", "exp": NOW + 60, "iat": NOW, "nonce": provider.pending_nonce}
    provider.token_response = {"id_token": pyjwt.encode(claims, "x" * 40, algorithm="HS256", headers={"kid": "k1"})}
    assert code_of(lambda: callback(client, auth, state)) == "id_token_invalid"
    provider.token_response = {"id_token": pyjwt.encode(claims, key=None, algorithm="none")}
    assert code_of(lambda: callback(client, auth, state)) == "id_token_invalid"


def test_unverified_email_blocked_by_default_but_optional(client, provider):
    auth, state = start_flow(client, provider)
    provider.token_response = {"id_token": provider.id_token(email_verified=False)}
    assert code_of(lambda: callback(client, auth, state)) == "email_not_verified"
    lenient = AnointedClient(issuer=ANOINTED_ISSUER, client_id=CLIENT_ID, client_secret=CLIENT_SECRET, redirect_uri=REDIRECT_URI, http=provider, now=lambda: NOW, require_verified_email=False)
    auth, state = start_flow(lenient, provider)
    provider.token_response = {"id_token": provider.id_token(email_verified=False)}
    user = callback(lenient, auth, state).user
    assert user.email is None and user.email_verified is False


def test_token_endpoint_error_carries_oauth_error(client, provider):
    auth, state = start_flow(client, provider)
    provider.token_status = 400
    provider.token_error = "invalid_grant"
    with pytest.raises(AnointedSsoError) as err:
        callback(client, auth, state)
    assert err.value.code == "token_exchange" and err.value.oauth_error == "invalid_grant" and err.value.status == 400


def test_secret_required_for_exchange(provider):
    c = AnointedClient(issuer=ANOINTED_ISSUER, client_id=CLIENT_ID, redirect_uri=REDIRECT_URI, http=provider, now=lambda: NOW)
    auth, state = start_flow(c, provider)
    assert code_of(lambda: callback(c, auth, state)) == "config"


def test_jwks_refetched_once_on_key_rotation(client, provider):
    client.get_jwks()  # cache the old set
    new_key, new_jwk = make_key("k2")
    provider.jwks = {"keys": [new_jwk]}
    auth, state = start_flow(client, provider)
    claims = {"iss": ANOINTED_ISSUER, "aud": CLIENT_ID, "sub": "u2", "exp": NOW + 60, "iat": NOW, "nonce": provider.pending_nonce, "email": "a@b.c", "email_verified": True}
    provider.token_response = {"id_token": provider.sign(claims, key=new_key, kid="k2")}
    assert callback(client, auth, state).user.sub == "u2"
    assert sum(1 for c in provider.calls if c[1].endswith("jwks.json")) == 2


def test_discovery_cached_and_validated(client, provider):
    client.discover()
    client.discover()
    assert sum(1 for c in provider.calls if c[1].endswith("openid-configuration")) == 1
    provider.discovery_override = {"issuer": "https://evil.example/"}
    bad = AnointedClient(issuer=ANOINTED_ISSUER, client_id=CLIENT_ID, redirect_uri=REDIRECT_URI, http=provider, now=lambda: NOW)
    assert code_of(bad.discover) == "discovery"
    provider.discovery_override = {"code_challenge_methods_supported": ["plain"]}
    assert code_of(bad.discover) == "discovery"
    provider.discovery_override = {"token_endpoint": "http://insecure.example/token"}
    assert code_of(bad.discover) == "discovery"


def test_discovery_network_failure(provider):
    def boom(*a):
        raise OSError("down")

    c = AnointedClient(issuer=ANOINTED_ISSUER, client_id=CLIENT_ID, redirect_uri=REDIRECT_URI, http=boom)
    assert code_of(c.discover) == "discovery"


def test_refresh(client, provider):
    provider.token_response = {"access_token": "at2", "refresh_token": "rt2", "expires_in": 60, "id_token": provider.id_token(nonce=None)}
    tokens, claims = client.refresh("rt", expected_sub="user-1")
    assert tokens.refresh_token == "rt2" and claims["sub"] == "user-1"
    assert provider.form_of_last_token_call()["grant_type"] == "refresh_token"
    assert code_of(lambda: client.refresh("rt", expected_sub="someone-else")) == "id_token_invalid"
    assert code_of(lambda: client.refresh("")) == "config"


def test_revoke_and_userinfo_and_logout_url(client, provider):
    client.revoke("rt")
    assert [c for c in provider.calls if c[1].endswith("/connect/revocation")]
    provider.handlers[ANOINTED_ISSUER + "connect/userinfo"] = lambda m, h, d: (
        HttpResponse(200, json.dumps({"sub": "user-1", "email": "a@b.c"})) if h.get("authorization") == "Bearer at" else HttpResponse(401, "")
    )
    assert client.fetch_user_info("at")["sub"] == "user-1"
    assert code_of(lambda: client.fetch_user_info("bad")) == "http"
    url = client.build_logout_url(id_token_hint="idt", post_logout_redirect_uri="https://app.example/bye", state="s")
    q = parse_qs(urlsplit(url).query)
    assert q["client_id"] == [CLIENT_ID] and q["id_token_hint"] == ["idt"] and q["state"] == ["s"]
    assert code_of(lambda: client.build_logout_url(post_logout_redirect_uri="http://app.example/bye")) == "config"


def test_take_new_user_password(client, provider):
    prefix = ANOINTED_ISSUER + "api/oauth/partner-password"
    provider.handlers[prefix] = lambda m, h, d: HttpResponse(200, json.dumps({"Data": {"Password": "pw"}}))
    assert client.take_new_user_password("user-1") == "pw"
    body = json.loads([c for c in provider.calls if c[1] == prefix][0][2])
    assert body == {"clientId": CLIENT_ID, "clientSecret": CLIENT_SECRET, "subject": "user-1"}
    provider.handlers[prefix] = lambda m, h, d: HttpResponse(404, "")
    assert client.take_new_user_password("user-1") is None
    provider.handlers[prefix] = lambda m, h, d: HttpResponse(500, "")
    assert code_of(lambda: client.take_new_user_password("user-1")) == "http"


def test_redirect_uri_function_and_allowlist(provider):
    seen = []

    def resolver(request):
        seen.append(request)
        return f"https://{request}/auth/anointed/callback"

    c = AnointedClient(
        issuer=ANOINTED_ISSUER, client_id=CLIENT_ID, client_secret=CLIENT_SECRET, redirect_uri=resolver, http=provider, now=lambda: NOW,
        allowed_redirect_uris=["https://a.example/auth/anointed/callback"],
    )
    assert c.create_authorize_request(request="a.example").redirect_uri == "https://a.example/auth/anointed/callback"
    assert code_of(lambda: c.create_authorize_request(request="b.example")) == "redirect_uri_rejected"
    assert code_of(lambda: c.create_authorize_request(redirect_uri="http://a.example/x")) == "redirect_uri_rejected"


def test_loopback_http_allowed_cookie_not_secure(provider):
    c = AnointedClient(issuer=ANOINTED_ISSUER, client_id=CLIENT_ID, redirect_uri="http://localhost:3000/cb", http=provider, now=lambda: NOW)
    assert "Secure" not in c.create_authorize_request().set_cookie_header


def test_clear_cookie_header(client):
    header = client.clear_flow_cookie_header()
    assert header.startswith("aa_sso_flow=;") and "Max-Age=0" in header
